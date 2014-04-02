//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class BrakeLights: MonoBehaviour
{
	[HideInInspector]
	public CarController carController;
	public Material brakeLights;
	float startValue;
	float intensityValue;
	
	void Awake(){
		carController = transform.GetComponent<CarController>();
		if (brakeLights) startValue=brakeLights.GetFloat("_Intensity");
		
	}
	
	void FixedUpdate(){
		if (brakeLights){
			if (carController.brakeKey){
				if (intensityValue< startValue+1){
					intensityValue+=Time.deltaTime/0.1f;
					brakeLights.SetFloat("_Intensity", intensityValue);
				}
				
			}
			else{
				if (intensityValue> startValue){
					intensityValue-=Time.deltaTime/0.1f;
					brakeLights.SetFloat("_Intensity", intensityValue);
				}
			}
		}
	}
}
