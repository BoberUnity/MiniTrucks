//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
// rolling friction coefficients:
//0.008 - 0.01 		Very good concrete 
//0.01 - 0.0125 		Very good tarmac 
//0.01 - 0.015 		Average concrete
//0.015 				Very good pavement 
//0.018 				Average tarmac <--
//0.02					poor condition concrete
//0.23					poor condition Tarmac
//0.033 - 0.055		Good stone paving 
//0.045				Good natural paving 
//0.025				Snow shallow (5cm)
//0.037				Snow thick (10cm) 
//0.08 - 0.16			Unmaintained natural road (grass, mud, and sand)
//0.15 - 0.3			loose sand 
//THESE VALUES ARE FOR ORDINARY CAR TYRES. RACE TIRES (SLICKS) HAVE DECREASED ROLLING FRICTION (FOR SMOOTHNESS AND HIGHER PRESSURE)

public class PhysicMaterials : MonoBehaviour {

	public PhysicMaterial track;
	public float trackGrip=1; 
	public float trackRollingFriction=0.018f;
	public float trackStaticFriction=1f;
	
	public PhysicMaterial grass;
	public float grassGrip=0.6f; 
	public float grassRollingFriction=0.05f;
	public float grassStaticFriction=0.6f;

	public PhysicMaterial sand;
	public float sandGrip=0.6f; 
	public float sandRollingFriction=0.15f;
	public float sandStaticFriction=0.8f;
	
	public PhysicMaterial offRoad;
	public float offRoadGrip=0.6f; 
	public float offRoadRollingFriction=0.05f;
	public float offRoadStaticFriction=0.9f;
}