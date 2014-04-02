using UnityEngine;

public class RotatePivot180 : MonoBehaviour {
	
	private Quaternion Rotation;
	
	void Awake () {
		
		Rotation = Quaternion.AngleAxis(180, Vector3.up);
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		//Vector3[] normals = mesh.normals;
		int i = 0;
		while (i < vertices.Length) {
			vertices[i] = Rotation * vertices[i];
			i++;
			mesh.vertices = vertices;
			mesh.RecalculateNormals();
		}
	}
}
