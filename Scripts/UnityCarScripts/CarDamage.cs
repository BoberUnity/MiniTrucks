//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// CarDamage.cs Based on the work of BTM (http://forum.unity3d.com/members/20246-%95BTM)
// Info: http://forum.unity3d.com/threads/52040-Deform-Script-(early-stage)
// Modified by Michele Di Lena unitycar@unitypackages.net
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class CarDamage : MonoBehaviour
{
	struct permaVertsColl
	{
		public Vector3[] permaVerts;
	}
	public MeshCollider meshCollider; // car mesh collider
	//private MeshFilter meshFilterColl;
	public MeshFilter[] meshFilters; // array of the meshes deformed by the script. If left empty the script will use all the meshes of the car
	private MeshFilter[] m_meshFilters;
	public float deformNoise = 0.005f; //noise added to the deformation in order to simulate breaks
	public float deformRadius = 0.5f; //radius of the deformation from the collision point.

	float bounceBackSleepCap = 0.002f; // below this value mesh is considered repaired
	public float bounceBackSpeed = 2f; //speed at which object's mesh go back to it's original state after pressing repair key

	private Vector3[] colliderVerts;
	private permaVertsColl[] originalMeshData;
	private bool sleep = true;
	public float maxDeform = 0.5f; //maximum distance from it's original position that a vertex can move. If left to 0 the vertex will move with no limit
	float minForce = 5f; //below this value collisions are ignored. WARNING: values too low (<5) cause weird car damages
	public float multiplier = 0.1f; //the deformation value is the force of the collision*this value.
	public float YforceDamp = 1f; // damps of the strenght collisions in vertical direction. Values <1 will save the car from severe damages after jumps;. Vaules between 0.0 - 1.0
	[HideInInspector]
	public bool repair = false;
	Vector3 vec;
	Transform trs;
	Transform myTransform;
	GameObject GObody;
	CarDynamics carDynamics;
	Axles axles;
	Rigidbody body;	
	float sign=1;
	Quaternion rot = Quaternion.identity;
	int wheelLayer;
	int carLayer;
	int i;
	
	void Start()
	{
		myTransform=transform;
		body=rigidbody;	
    if(meshFilters.Length==0){
			//meshFilters = GetComponentsInChildren<MeshFilter>();
			m_meshFilters = GetComponentsInChildren<MeshFilter>();
				
			int k=0;
			for (i = 0; i < m_meshFilters.Length; i++){
				if (m_meshFilters[i].collider==null) k++;
			}
			meshFilters=new MeshFilter[k];
			k=0;
			for (i = 0; i < m_meshFilters.Length; i++){
				if (m_meshFilters[i].collider==null){
					meshFilters[k]=m_meshFilters[i];
					k++;
				}
			}
		}
		
		if (meshCollider!=null){
			//meshFilterColl=meshCollider.gameObject.GetComponent<MeshFilter>();
			colliderVerts = meshCollider.sharedMesh.vertices;
		}
	
		LoadoriginalMeshData();

		foreach(Transform child in transform){
			if (child.gameObject.tag=="Body" || child.gameObject.name=="Body" || child.gameObject.name=="body") GObody=child.gameObject;
		}
		if (GObody) {
			sign=Mathf.Cos(GObody.transform.localEulerAngles.y*Mathf.Deg2Rad);
			if (GObody.transform.localEulerAngles.x!=0) rot=Quaternion.AngleAxis(GObody.transform.localEulerAngles.x*3,Vector3.right);
		}
		
		carDynamics = GetComponent<CarDynamics>();
		axles = GetComponent<Axles>();
		wheelLayer=axles.allWheels[0].transform.gameObject.layer;
		carLayer=transform.gameObject.layer;
   }
	
	void LoadoriginalMeshData()
	{
		originalMeshData = new permaVertsColl[meshFilters.Length];
		for (i = 0; i < meshFilters.Length; i++)
		{
			originalMeshData[i].permaVerts = meshFilters[i].mesh.vertices;
		}		
	}
		
	void Update()
	{
		if (!sleep && repair && bounceBackSpeed > 0)
		{
			int k;
			sleep = true;
			for (k = 0; k < meshFilters.Length; k++)
			{
				Vector3[] vertices = meshFilters[k].mesh.vertices;
				if (originalMeshData==null) LoadoriginalMeshData();
				for (int i = 0; i < vertices.Length; i++)
				{
					vertices[i] += (originalMeshData[k].permaVerts[i] - vertices[i])*(Time.deltaTime*bounceBackSpeed);
					if ((originalMeshData[k].permaVerts[i] - vertices[i]).magnitude >= bounceBackSleepCap) sleep = false;
				}
				meshFilters[k].mesh.vertices=vertices;
				meshFilters[k].mesh.RecalculateNormals();
				meshFilters[k].mesh.RecalculateBounds();
			}
			if (meshCollider!=null) 
			{
				Mesh mesh = new Mesh();
				mesh.vertices=colliderVerts;
				mesh.triangles=meshCollider.sharedMesh.triangles;
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				meshCollider.sharedMesh=mesh;
				body.centerOfMass = carDynamics.centerOfMass.localPosition;						
			}
			if (sleep==true) repair=false;
		}
		if (Application.isEditor) YforceDamp=Mathf.Clamp01(YforceDamp);
	}	
	
	void OnCollisionEnter (Collision collision) {
		
		if (collision.contacts.Length > 0 && myTransform!=null){
			Vector3 colRelVel = collision.relativeVelocity;
      Debug.LogWarning("Udar " + colRelVel);
			colRelVel*=1 - Mathf.Abs(Vector3.Dot(myTransform.up,collision.contacts[0].normal))*YforceDamp;
			
			float cos=Mathf.Abs(Vector3.Dot(collision.contacts[0].normal,colRelVel.normalized));
			//float angle=Vector3.Angle(collision.contacts[0].normal,colRelVel);
			//float cos=Mathf.Abs(Mathf.Cos(angle*Mathf.Deg2Rad));
			
			if (colRelVel.magnitude*cos>= minForce) {
				sleep = false;
				
				vec = myTransform.InverseTransformDirection(colRelVel)*multiplier*0.1f;
				
				if (originalMeshData==null) LoadoriginalMeshData();
				
				for (int i = 0; i < meshFilters.Length; i++)
				{
					if (meshFilters[i].gameObject.layer != wheelLayer || carLayer==wheelLayer){
						DeformMesh(meshFilters[i].mesh, originalMeshData[i].permaVerts, collision, cos, meshFilters[i].transform,sign,rot);
					}
				}
				
				if (meshCollider!=null) {
					Mesh mesh = new Mesh();
					mesh.vertices=meshCollider.sharedMesh.vertices;
					mesh.triangles=meshCollider.sharedMesh.triangles;
					DeformMesh(mesh, colliderVerts, collision,cos,meshCollider.transform,1,Quaternion.identity);
					meshCollider.sharedMesh=mesh;
					meshCollider.sharedMesh.RecalculateNormals();
					meshCollider.sharedMesh.RecalculateBounds();
					body.centerOfMass = carDynamics.centerOfMass.localPosition;						
				}
			}
		}
	}
	
    void DeformMesh(Mesh mesh, Vector3[] originalMesh, Collision collision, float cos, Transform meshTransform, float sign, Quaternion rot)
    {		
		Vector3[] vertices = mesh.vertices;
		foreach (ContactPoint contact in collision.contacts) 
		{	
			Vector3 point =meshTransform.InverseTransformPoint(contact.point);
			for (int i=0; i<vertices.Length; i++) 
			{
				if ((point - vertices[i]).magnitude < deformRadius) 
				{
					vertices[i] += rot*((vec*(deformRadius - (point - vertices[i]).magnitude)/deformRadius)*cos + (Random.onUnitSphere*deformNoise))*sign;
					if (maxDeform > 0 && (vertices[i] - originalMesh[i]).magnitude > maxDeform) 
					{
						vertices[i] = originalMesh[i] + (vertices[i]-originalMesh[i]).normalized*maxDeform;
					}
				}
			}
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}