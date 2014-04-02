//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

// This class calculates the aerodynamic friction of a car
public class AerodynamicResistance : MonoBehaviour {

	public float Cx=0.30f; //coefficient of friction, a factor depending on the shape of a car and determined by experiment; for a late model Corvette it is about 0.30
	public float Area=1.858f; //frontal area of the car in square meters. For a Corvette it is about 1.858  square meters.
	//const float rho=1.2041f; // density of air  in kg/m3 at  20 °C
	public float dragForce;
	//public float KWPower;

	Rigidbody body;
	//Transform mytransform;
	CarDynamics cardynamics;
	
	void Start(){
		body=rigidbody;
		//mytransform=transform;
		cardynamics = GetComponent<CarDynamics>();
	}
	void FixedUpdate () {
		//dragForce=1/2f*Cx*Area*rho*body.velocity.x*body.velocity.x;
		if (body.velocity.sqrMagnitude <=0.001f) 
			dragForce=0; 
		else 
			dragForce =0.5f*Cx*Area*cardynamics.airDensity*body.velocity.sqrMagnitude;
		
		//KWPower=dragForce*body.velocity.magnitude/1000*1.36f;
		body.AddForce(-dragForce*body.velocity.normalized);
	}
}
