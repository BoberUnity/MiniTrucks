//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public abstract class Modifier : MonoBehaviour {
		
	public float minVelocity=35;
	public float overallStrength=0.5f;
	public float COGHelperStrength=1;
	public float torqueHelperStrength=1;
	public float gripHelperStrength=1;
	protected CarDynamics carDynamics;
	protected Axles axles;
	protected float COGYShift;
	protected float torque;
	protected float gripSlip;
	protected float gripVelo;
	protected Rigidbody body;
	protected float lateralSlip;
	protected float absLateralSlip;
	protected float veloKmh;
		
	void Start(){
		body=rigidbody;
		carDynamics = GetComponent<CarDynamics>();
		axles = GetComponent<Axles>();
		if (overallStrength<0) overallStrength=0;
		if (COGHelperStrength<0) COGHelperStrength=0;
		if (torqueHelperStrength<0) torqueHelperStrength=0;
		if (gripHelperStrength<0) gripHelperStrength=0;
		
	}

	void OnDisable(){
		ResetParameters();
	}	
	
	// Abstract methods
	protected abstract void COGHelper(out float COGYShift, float absLateralSlip, float overallStrength);
	protected abstract void TorqueHelper(out float torque,float absLateralSlip, float lateralSlip, float overallStrength);
	protected abstract void GripHelper(out float gripSlip, out float gripVelo, float absLateralSlip, float overallStrength);
	protected abstract void ResetParameters();
	
	void FixedUpdate(){
		overallStrength=Mathf.Clamp01(overallStrength);
		veloKmh=Mathf.Abs(carDynamics.velo*3.6f);
		if (veloKmh>minVelocity) { //we enable this modifier only for speed > minVelocity (in km/h)
			lateralSlip=MaxLateralSlip();
			absLateralSlip=Mathf.Abs(lateralSlip);
			COGHelper(out COGYShift, absLateralSlip, overallStrength);
			TorqueHelper(out torque, absLateralSlip, lateralSlip, overallStrength);
			GripHelper(out gripSlip, out gripVelo,absLateralSlip, overallStrength);				
		}
		else{
			ResetParameters();
		}
	}
		
	protected void SetCOGYPosition(float COGYShift){
		if (carDynamics.centerOfMass!=null) {
			carDynamics.centerOfMass.localPosition=new Vector3(carDynamics.xlocalPosition, carDynamics.ylocalPosition - COGYShift, carDynamics.zlocalPosition);
			body.centerOfMass = carDynamics.centerOfMass.localPosition;
		}
	}
		
	float MaxLateralSlip() {
		float val=0;
		float temp=0;
		foreach(Wheel w in axles.allWheels){
			temp = Mathf.Abs(w.lateralSlip);
			if (val<temp) val=w.lateralSlip;
		}
		return val;
	}
}