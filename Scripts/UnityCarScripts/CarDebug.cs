//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class CarDebug : MonoBehaviour {

	Drivetrain drivetrain;
	Axles axles;
	ForceFeedback forceFeedback;
		
	void Start(){
		drivetrain=GetComponent<Drivetrain>();
		axles=GetComponent <Axles>();
		forceFeedback=GetComponent <ForceFeedback>();
	}
 	
	public float RoundTo (float value, int precision){
		int boh=1;
		for (int i =1; i<=precision; i++ ){
			boh=boh*10;
		}			
		return  Mathf.Round(value * boh) / boh; 
	}
	
	void OnGUI () {
		//drivetrain.cs
		GUILayout.Label("clutch.GetClutchPosition: "+drivetrain.clutch.GetClutchPosition());
		GUILayout.Label("currentPower: "+Mathf.Round(drivetrain.currentPower));
		GUILayout.Label("engFricTorque: "+drivetrain.frictionTorque +" engineTorque: "+drivetrain.torque) ;
		GUILayout.Label("clutchDrag: "+drivetrain.clutchDragImpulse/Time.fixedDeltaTime + "  torque - frictionTorque: " + (drivetrain.torque - drivetrain.frictionTorque));
		GUILayout.Label("CanShiftAgain: "+drivetrain.CanShiftAgain + " changingGear: " +drivetrain.changingGear );
		GUILayout.Label("clutch.speedDiff: " + drivetrain.clutch.speedDiff );
		GUILayout.Label("gear: " + drivetrain.gear + " " + drivetrain.clutchEngageSpeed);
		GUILayout.Label("maxPowerDriveShaft: "+drivetrain.maxPowerDriveShaft*drivetrain.powerMultiplier + " maxNetPower: " + drivetrain.maxNetPower*drivetrain.powerMultiplier + " maxTorque: " + drivetrain.maxTorque + " maxNetTorque: " + drivetrain.maxNetTorque);
		GUILayout.Label("startTorque: " + drivetrain.startTorque);
		GUILayout.Label("% loss power: "+((drivetrain.maxPowerDriveShaft-drivetrain.maxNetPower)/drivetrain.maxPowerDriveShaft)*100f  + "%");
		if (forceFeedback!=null) GUILayout.Label("force feedback: "+forceFeedback.force);
		//automatic = GUILayout.Toggle(automatic, "Automatic Transmission"); 
		
		//wheel.cs
		foreach(Wheel w in axles.allWheels){
			//if (carcontr) GUI.Label (new Rect(0,380,600,200), "carcontr.frontRearWeightRepartition: " +carcontr.frontRearWeightRepartition); else GUI.Label (new Rect(0,380,600,200), "carcontr.frontRearWeightRepartition: ");
			//GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");GUILayout.Label("");
			
			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,280,600,200), "hitDown.distance - radius : " +w.hitDown.distance); }
			if (w.wheelPos==WheelPos.FRONT_RIGHT ) {GUI.Label (new Rect(300,300,600,200), "hitDown.normal .x         .z " + w.hitDown.normal.x + " " + w.hitDown.normal.z);}
			if (w.wheelPos==WheelPos.FRONT_RIGHT ) {GUI.Label (new Rect(300,320,600,200), "groundNormal.x groundNormal.z " + w.groundNormal.x + " " + w.groundNormal.z);}
			//if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,320,600,200), "model.GetComponent<test>() : " +model.GetComponent<test>().onGround);}
			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,340,600,200), "wheelRoadVeloLatFL: " +w.wheelRoadVeloLat);}
			if (w.wheelPos==WheelPos.FRONT_RIGHT ) {GUI.Label (new Rect(300,360,600,200), "wheelRoadVeloLatFR: " +w.wheelRoadVeloLat);}
			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,380,600,200), "absRoadVelo : " +Mathf.Abs (w.wheelRoadVelo));}
			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,400,600,200), "idealSlipRatioFL :" +w.idealSlipRatio+ " idealSlipAngleFL " + w.idealSlipAngle);}
			if (w.wheelPos==WheelPos.FRONT_RIGHT ) {GUI.Label (new Rect(300,420,600,200), "idealSlipRatioFR :" +w.idealSlipRatio+ " idealSlipAngleFR "   + w.idealSlipAngle);}
			if (w.wheelPos==WheelPos.REAR_LEFT ) {GUI.Label (new Rect(300,440,600,200), "idealSlipRatioRL :" +w.idealSlipRatio+ " idealSlipAngleRL " + w.idealSlipAngle);}
			if (w.wheelPos==WheelPos.REAR_RIGHT ) {GUI.Label (new Rect(300,460,600,200), "idealSlipRatioRR :" +w.idealSlipRatio+ " idealSlipAngleRR " + w.idealSlipAngle);}
			//if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,520,600,200), "roadForce: " +roadForce.x);}
			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,480,600,200), "slipRatioFL: " + w.slipRatio+ "    " + "Fx: "+w.Fx+ " longitudinalSlip:" + RoundTo(w.longitudinalSlip,3) + " normalForce:" + w.normalForce);}
			if (w.wheelPos==WheelPos.FRONT_RIGHT ) {GUI.Label (new Rect(300,500,600,200), "slipRatioFR: " + w.slipRatio+ "    " + "Fx: "+w.Fx+ " longitudinalSlip:" + RoundTo(w.longitudinalSlip,3) + " normalForce:" + w.normalForce);}
			if (w.wheelPos==WheelPos.REAR_LEFT ) {GUI.Label (new Rect(300,520,600,200), "slipRatioRL: " + w.slipRatio+ "    " + "Fx: "+w.Fx + " longitudinalSlip:" + RoundTo(w.longitudinalSlip,3) + " normalForce:" + w.normalForce);}
			if (w.wheelPos==WheelPos.REAR_RIGHT ) {GUI.Label (new Rect(300,540,600,200), "slipRatioRR: " + w.slipRatio+ "    " + "Fx: "+w.Fx+ " longitudinalSlip:" + RoundTo(w.longitudinalSlip,3) + " normalForce:" + w.normalForce);}
			
			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,560,600,200), "slipAngleFL: " + w.slipAngle+ "    " + "Fy: "+w.Fy + " lateralSlip:" + RoundTo(w.lateralSlip,3) + " rho:" + Mathf.Sqrt((w.lateralSlip)*(w.lateralSlip) + (w.longitudinalSlip)*(w.longitudinalSlip)));}
			if (w.wheelPos==WheelPos.FRONT_RIGHT ) {GUI.Label (new Rect(300,580,600,200), "slipAngleFR: " + w.slipAngle+ "    " + "Fy: "+w.Fy+ " lateralSlip:" + RoundTo(w.lateralSlip,3)+ " rho:" + Mathf.Sqrt((w.lateralSlip)*(w.lateralSlip) + (w.longitudinalSlip)*(w.longitudinalSlip)));}
			if (w.wheelPos==WheelPos.REAR_LEFT ) {GUI.Label (new Rect(300,600,600,200), "slipAngleRL: " + w.slipAngle+ "    " + "Fy: "+w.Fy+ " lateralSlip:" + RoundTo(w.lateralSlip,3)+ " rho:" + Mathf.Sqrt((w.lateralSlip)*(w.lateralSlip) + (w.longitudinalSlip)*(w.longitudinalSlip)));}
			if (w.wheelPos==WheelPos.REAR_RIGHT ) {GUI.Label (new Rect(300,620,600,200), "slipAngleRR: " + w.slipAngle+ "    " + "Fy: "+ w.Fy+ " lateralSlip:" + RoundTo(w.lateralSlip,3) + " rho:" + Mathf.Sqrt((w.lateralSlip)*(w.lateralSlip) + (w.longitudinalSlip)*(w.longitudinalSlip)));}
/* 			if (w.wheelPos==WheelPos.FRONT_LEFT ) {GUI.Label (new Rect(300,560,600,200), "IdealSlipAngleFL: "+w.idealSlipAngle + "    " + w.slipAngle);}
			if (w.wheelPos==WheelPos.REAR_LEFT ) {GUI.Label (new Rect(300,580,600,200), "IdealSlipAngleRL: "+w.idealSlipAngle + "    " + w.slipAngle);}
			if (w.wheelPos==WheelPos.REAR_LEFT ) {GUI.Label (new Rect(300,600,600,200), "IdealSlipRatioRL: "+w.idealSlipRatio + "    " + w.slipRatio);}
			if (w.wheelPos==WheelPos.REAR_RIGHT ) {GUI.Label (new Rect(300,620,600,200), "IdealSlipRatioRR: "+w.idealSlipRatio + "    " + w.slipRatio);}
 */			//if (w.wheelPos==WheelPos.REAR_LEFT ) {GUI.Label (new Rect(300,580,600,200), "ca cb: "+ca+ "        " + cb + "     "  + (ca+cb));}
			
			//if (w.wheelPos==WheelPos.FRONT_LEFT || w.wheelPos==WheelPos.FRONT_RIGHT ) GUI.Label (new Rect(300,400,600,200), "normalForceF "  + (normalForceFL+normalForceFR)/2 ); 
			//if (w.wheelPos==WheelPos.REAR_LEFT ) GUI.Label (new Rect(300,420,600,200), "normalForceR "  + (normalForceRL+normalForceRR)/2 ); 
			//if (w.wheelPos==WheelPos.REAR_LEFT ) GUI.Label (new Rect(0,380,600,200), "wheelVelo" + "RL: " + body.velocity.x);+ " " + Mathf.Sin(body.rotation.x)*(normalForce)*radius+  " " + driveTorque);
			//if (w.wheelPos==WheelPos.REAR_RIGHT ) GUI.Label (new Rect(0,400,600,200), "wheelVelo" + "RR: " +body.velocity.y);//+ " " + Mathf.Sin(body.rotation.x)*(normalForce)*radius+  " " + driveTorque);
			//if (w.wheelPos==WheelPos.FRONT_LEFT ) GUI.Label (new Rect(0,420,600,200), "wheelVelo" + "FL: " + body.velocity.z);//+ " " + Mathf.Sin(body.rotation.x)*(normalForce)*radius+  " " + driveTorque);
			//if (w.wheelPos==WheelPos.FRONT_RIGHT ) GUI.Label (new Rect(0,440,600,200), "wheelVelo" + "FR: " + trs.eulerAngles.x + " " + Mathf.Sin(Mathf.Deg2Rad*trs.eulerAngles.x));+ " " + Mathf.Sin(body.rotation.x)*(normalForce)*radius+  " " + driveTorque);			
		}
	}	
}