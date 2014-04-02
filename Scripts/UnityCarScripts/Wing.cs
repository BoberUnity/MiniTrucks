//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

// This class is used to simulate aerodynamic wings.
// Add it to a GameObject to have lift or downForce applied at that position.
public class Wing : MonoBehaviour {
	
	public float dragCoefficient=0.05f; // drag coefficient coefficient of the wing
	public int angleOfAttack=1; // positive angle of attack -> downForce;  negative angle of attack -> lift
	public float area=1; // area of the wing
	public float downForce;
	public float dragForce;
	//const float rho=1.2041f; // density of air  in kg/m3 at  20 °C

	Rigidbody body;
	Transform myTransform;
	CarDynamics cardynamics;
	
	void Start () {
		Transform trs = transform;
		myTransform=transform;
				
		while (trs != null && trs.rigidbody == null)
			trs = trs.parent;
		if (trs != null)
			body = trs.rigidbody;
		
		trs = transform;
		while (trs.GetComponent<CarDynamics>() == null)  trs = trs.parent;
		cardynamics = trs.GetComponent<CarDynamics>();		
	}
	
	void FixedUpdate () {
		if (body != null ){
			float sqrMagnitude=body.velocity.x*body.velocity.x + body.velocity.z*body.velocity.z;
			if (sqrMagnitude >0.1f){
				downForce = 0.5f*area*angleOfAttack*dragCoefficient*cardynamics.airDensity*sqrMagnitude;
				dragForce = 0.5f*dragCoefficient*area*cardynamics.airDensity*sqrMagnitude;
				body.AddForceAtPosition(-downForce*myTransform.up, myTransform.position);
				body.AddForceAtPosition(-dragForce*myTransform.forward, myTransform.position);
				Debug.DrawRay(myTransform.position, -downForce*myTransform.up/1000,Color.white);
				Debug.DrawRay(myTransform.position, -dragForce*myTransform.forward/1000,Color.white);
			}
		}
	}	
}
