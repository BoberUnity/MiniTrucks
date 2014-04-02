//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public enum WheelPos {FRONT_LEFT, FRONT_RIGHT, REAR_LEFT, REAR_RIGHT};
[RequireComponent (typeof (Rigidbody))]
[ExecuteInEditMode()]
public class Axles : MonoBehaviour {
	//[HideInInspector]
	//public Wheel[] frontWheels =new Wheel[2];
	//[HideInInspector]
	//public Wheel[] rearWheels =new Wheel[2];
	[HideInInspector]
	public Wheel[] otherWheels=new Wheel[0];
	[HideInInspector]
	public Wheel[] allWheels;
	
	public Axle frontAxle=new Axle();
	public Axle rearAxle=new Axle();
	
	public Axle[] otherAxles=new Axle[0];
		
/* 	void OnEnable() {
		CheckWheels();
	}
 */	
	void CheckWheels(){
		if (frontAxle.leftWheel==null) Debug.LogWarning("UnityCar: front left wheel not assigned " + " (" +transform.name+ ")");
		if (frontAxle.rightWheel==null) Debug.LogWarning("UnityCar: front right wheel not assigned " + " (" +transform.name+ ")");
		if (rearAxle.leftWheel==null) Debug.LogWarning("UnityCar: rear left wheel not assigned " + " (" +transform.name+ ")");
		if (rearAxle.rightWheel==null) Debug.LogWarning("UnityCar: rear right wheel not assigned " + " (" +transform.name+ ")");
	}

	void Start(){
		CheckWheels();
	}	
	
	void Awake(){
		SetWheels();
	}
	
	public void SetWheels(){
		if (frontAxle.leftWheel) frontAxle.leftWheel.wheelPos=WheelPos.FRONT_LEFT;		
		if (frontAxle.rightWheel) frontAxle.rightWheel.wheelPos=WheelPos.FRONT_RIGHT;
		if (rearAxle.leftWheel) rearAxle.leftWheel.wheelPos=WheelPos.REAR_LEFT;
		if (rearAxle.rightWheel) rearAxle.rightWheel.wheelPos=WheelPos.REAR_RIGHT;
		
		frontAxle.wheels =new Wheel[0];
		if (frontAxle.leftWheel!=null && frontAxle.rightWheel!=null) {
			frontAxle.wheels =new Wheel[2];
			frontAxle.wheels[0]=frontAxle.leftWheel;
			frontAxle.wheels[1]=frontAxle.rightWheel;
		}
		else if (frontAxle.leftWheel!=null || frontAxle.rightWheel!=null){
			frontAxle.wheels =new Wheel[1];
			if (frontAxle.leftWheel!=null) frontAxle.wheels[0]=frontAxle.leftWheel;
			else frontAxle.wheels[0]=frontAxle.rightWheel;
		}
		frontAxle.camber=Mathf.Clamp(frontAxle.camber,-10,10);
		
		rearAxle.wheels =new Wheel[0];
		if (rearAxle.leftWheel!=null && rearAxle.rightWheel!=null) {
			rearAxle.wheels =new Wheel[2];
			rearAxle.wheels[0]=rearAxle.leftWheel;
			rearAxle.wheels[1]=rearAxle.rightWheel;
		}
		else if (rearAxle.leftWheel!=null || rearAxle.rightWheel!=null) {
			rearAxle.wheels =new Wheel[1];
			if (rearAxle.leftWheel!=null) rearAxle.wheels[0]=rearAxle.leftWheel;
			else rearAxle.wheels[0]=rearAxle.rightWheel;
		}
		rearAxle.camber=Mathf.Clamp(rearAxle.camber,-10,10);
		
		Wheel[] m_otherWheels=new Wheel[otherAxles.Length*2];
		int i=0;
		foreach(Axle axle in otherAxles){
			if (axle.leftWheel!=null && axle.rightWheel!=null) {
				axle.wheels =new Wheel[2];
				axle.wheels[0]=m_otherWheels[i]=axle.leftWheel;
				axle.wheels[1]=m_otherWheels[i+1]=axle.rightWheel;
				i+=2;
			}
			else{
				axle.wheels =new Wheel[1];
				if (axle.leftWheel!=null) axle.wheels[0]=m_otherWheels[0]=axle.leftWheel;
				else axle.wheels[0]=m_otherWheels[0]=axle.rightWheel;
				i+=1;
			}
			axle.camber=Mathf.Clamp(axle.camber,-10,10);
		}		
		
		otherWheels=new Wheel[i];
		m_otherWheels.CopyTo(otherWheels, 0);
		
		allWheels = new Wheel[frontAxle.wheels.Length + rearAxle.wheels.Length + otherWheels.Length];
		
		frontAxle.wheels.CopyTo(allWheels, 0);
		rearAxle.wheels.CopyTo(allWheels, frontAxle.wheels.Length);
		if (otherWheels.Length!=0) otherWheels.CopyTo(allWheels,frontAxle.wheels.Length + rearAxle.wheels.Length);
	}	
}

[System.Serializable]
public class Axle
{
	public Wheel leftWheel;
	public Wheel rightWheel;
	[HideInInspector]
	public Wheel[] wheels;
	public bool powered;
	public float suspensionTravel=0.2f;
	public float suspensionRate=20000;
	public float bumpRate=4000;
	public float reboundRate=4000;
	public float fastBumpFactor=0.3f;
	public float fastReboundFactor=0.3f;
	public float antiRollBarRate=10000;
	public float brakeFrictionTorque=1500;
	public float handbrakeFrictionTorque=0;
	public float maxSteeringAngle=0;
	public float forwardGripFactor=1;
	public float sidewaysGripFactor=1;
	public float camber=0;
	[HideInInspector]
	public float deltaCamber;
	[HideInInspector]
	public float oldCamber;	
	public CarDynamics.Tires tires;
	public float tiresPressure=200;
	public float optimalTiresPressure=200;
}
