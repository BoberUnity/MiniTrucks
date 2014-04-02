//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class Arcader : Modifier {

	protected override void COGHelper(out float COGYShift, float absLateralSlip, float strength){
		//(0, 0)  (Xa,Ya)
		//(1, 0.35)  (Xb,Yb)
		//(X  - Xb)    /  (Xa-Xb)*Ya   -   (X - Xa)   / (Xa - Xb)*Yb;
		COGYShift=Mathf.Clamp01(absLateralSlip)*0.35f*strength*COGHelperStrength;  //linear interpolation
		SetCOGYPosition(COGYShift);	
	}
	
	protected override void TorqueHelper(out float torque,float absLateralSlip, float lateralSlip, float strength){
		//(1, 0)  (Xa,Ya)
		//(2, 1)  (Xb,Yb)
		//(X - Xb)/(Xa - Xb)*Ya   -   (X - Xa)/(Xa - Xb)*Yb;
		torque=(absLateralSlip -1)*Mathf.Sign(lateralSlip)*10000*strength*torqueHelperStrength;
		if (absLateralSlip>1) body.AddRelativeTorque(-Vector3.up*torque);	
	}

	protected override void GripHelper(out float gripSlip, out float gripVelo, float absLateralSlip, float strength){
		gripSlip=gripVelo=0;
		foreach (Wheel w in axles.allWheels){
			gripSlip=Mathf.Clamp01(absLateralSlip)*strength;
			w.gripSlip=gripSlip;
			//(ArcaderMinVelocity, 0)  (Xa,Ya)
			//(300, 0.4)  (Xb,Yb)
			//gripVelo=(X  - Xb)   / (Xa-Xb)*Ya   -   (X - Xa)  /(Xa - Xb)*Yb;
			gripVelo=(veloKmh - minVelocity)*0.0015f*strength*gripHelperStrength;
			w.gripVelo=gripVelo;  //linear interpolation			
		}		
	}
	
	protected override void ResetParameters(){
		if (carDynamics!=null) COGHelper(out COGYShift,0,0);
		if (axles.allWheels!=null) GripHelper(out gripSlip,out gripVelo,0,0);
	}		
}