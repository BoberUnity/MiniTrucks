//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class CarCamerasController : MonoBehaviour {
	
	CarCameras carCameras;
	float sizex,sizey,sizez;
	[HideInInspector]
	public float externalSizex;
	[HideInInspector]
	public float externalSizey;
	[HideInInspector]
	public float externalSizez;
	int i=0;
	
	void Awake(){
		carCameras = GetComponent<CarCameras>();
	}
	
	float FindMaxBoundingSize(Collider[] colliders){
		sizex=0;sizey=0;sizez=0;
		if (colliders.Length!=0) {
			foreach(Collider mcollider in colliders){
				if (mcollider.gameObject.layer!=LayerMask.NameToLayer("Wheel") && mcollider.transform.GetComponent<FuelTank>()==null){
					sizex+=mcollider.bounds.size.x;
					sizey+=mcollider.bounds.size.y;
					sizez+=mcollider.bounds.size.z;
				}
			}
		}	
		return Mathf.Max(sizex+externalSizex,sizey+externalSizey,sizez+externalSizez);
	}
	
	public void SetCamera(int index, Transform target, bool newTarget){
		i=index;
		if (newTarget==true) carCameras.mtarget=target;
		carCameras.dampFixedCamera=false;
		carCameras.mouseOrbitFixedCamera=false;
		carCameras.driverView = false;
		
		if (index==0 &&  target!=null){ //external
			carCameras.mycamera=CarCameras.Cameras.SmoothLookAt;
			carCameras.target=target;
			carCameras.rotationDamping = 3.0f;
			carCameras.heightDamping = 100f;
			Collider[] colliders=target.gameObject.GetComponentsInChildren<Collider>();
			carCameras.distance=FindMaxBoundingSize(colliders)*1.5f;
			if (carCameras.distance<4) carCameras.distance=4;
			carCameras.height=carCameras.distance/2;
			carCameras.pitchAngle=-carCameras.height/1.5f;
			carCameras.yawAngle=0;
		}
		else if (index==1 && target!=null){ //onboard
			carCameras.mycamera=CarCameras.Cameras.FixedTo;
			foreach(Transform child in carCameras.mtarget.gameObject.GetComponentsInChildren<Transform>()){
				if (child.gameObject.tag=="Fixed_Camera_Driver_View" || child.gameObject.name=="Fixed_Camera_Driver_View") carCameras.target=child;
			}
			carCameras.distance=0;
			carCameras.height=0;
			carCameras.pitchAngle=0;
			carCameras.yawAngle=0;
			carCameras.dampFixedCamera=true;
			carCameras.mouseOrbitFixedCamera=true;
			carCameras.x = 0;
			carCameras.y = 0;
			carCameras.driverView = true;
		}
		else if (index==2 && target!=null){ //flybird
			carCameras.mycamera=CarCameras.Cameras.SmoothLookAt;
			carCameras.target=target;
			carCameras.rotationDamping = 3.0f;
			carCameras.heightDamping = 100f;
			Collider[] colliders=target.gameObject.GetComponentsInChildren<Collider>();
			carCameras.distance=FindMaxBoundingSize(colliders);
			if (carCameras.distance<4) carCameras.distance=4;
			carCameras.distance*=2;
			carCameras.height=carCameras.distance/2;
			carCameras.pitchAngle=-carCameras.height/2.5f;
			carCameras.yawAngle=0;
		}
		else if (index==3 && target!=null){ //look at the rear left wheel
			carCameras.mycamera=CarCameras.Cameras.FixedTo;
			foreach(Transform child in carCameras.mtarget.gameObject.GetComponentsInChildren<Transform>()){
				if (child.gameObject.tag=="Fixed_Camera_1" || child.gameObject.name=="Fixed_Camera_1") carCameras.target=child;
			}
			carCameras.distance=0;
			carCameras.height=0;
			carCameras.pitchAngle=0;
			carCameras.yawAngle=0;
		}
		else if (index==4 && target!=null){ //look from back
			carCameras.mycamera=CarCameras.Cameras.FixedTo;
			foreach(Transform child in carCameras.mtarget.gameObject.GetComponentsInChildren<Transform>()){
				if (child.gameObject.tag=="Fixed_Camera_2" || child.gameObject.name=="Fixed_Camera_2") carCameras.target=child;
			}
			carCameras.distance=0;
			carCameras.height=0;
			carCameras.pitchAngle=0;
			carCameras.yawAngle=0;
		}
		else if (index==5 &&  target!=null){ //lateral
			carCameras.mycamera=CarCameras.Cameras.SmoothLookAt;
			carCameras.rotationDamping = 3.0f;
			carCameras.heightDamping = 50f;
			carCameras.target=target;
			Collider[] colliders=target.gameObject.GetComponentsInChildren<Collider>();
			carCameras.distance=FindMaxBoundingSize(colliders);
			if (carCameras.distance<4) carCameras.distance=4;
			carCameras.height=carCameras.distance/2;
			carCameras.pitchAngle=-carCameras.height;
			carCameras.yawAngle=90;
		}
		else if (index==6 &&  target!=null){ //mouse orbit
			carCameras.mycamera=CarCameras.Cameras.MouseOrbit;
			carCameras.target=target;
			if (carCameras.distance<carCameras.distanceMin || carCameras.distance>carCameras.distanceMax) carCameras.distance = 5f;
			carCameras.x = carCameras.myTransform.eulerAngles.y;
			carCameras.y = carCameras.myTransform.eulerAngles.x;
		}
		else if (index==7 &&  target!=null){ //Map Camera
			carCameras.mycamera=CarCameras.Cameras.Map;
			carCameras.target=target;
			carCameras.distance=0;
			carCameras.height=0;
			carCameras.pitchAngle=0;
			carCameras.yawAngle=0;			
		}		
	}
	
	void Update(){
		//Camera control
		if (carCameras.mycamera!=CarCameras.Cameras.Map){
			if (Input.GetKeyDown (KeyCode.C)) {
				i+=1;
				if (i==7) i=0;
				SetCamera(i, carCameras.mtarget, false);
			}
		}
	}
}