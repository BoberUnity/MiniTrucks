//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

[RequireComponent (typeof (CarCamerasController))]
public class CarCameras : MonoBehaviour {
	public enum Cameras {SmoothLookAt,MouseOrbit,FixedTo,Map};
	public Cameras mycamera;
	public Transform target;
	public float distance = 10.0f;
	public float height = 2.5f;
	public float yawAngle =0;
	public float pitchAngle =-2.5f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	[HideInInspector]
	public bool dampFixedCamera;
	[HideInInspector]
	public bool mouseOrbitFixedCamera;
	[HideInInspector]
	public bool driverView=false;
	
	//Mouse Orbit params
	float xSpeed = 10f;
	float ySpeed = 10f;
	float yMinLimit = -20f;
	float yMaxLimit = 80f;
	[HideInInspector]
	public float distanceMin = 4f;
	[HideInInspector]
	public float distanceMax = 30f;
	
	float mouseDamping=10;
	[HideInInspector]
	public float x,y;
	
	float currentYawRotationAngle;
	float wantedYawRotationAngle;
	float currentHeight;
	float wantedHeight;
	float deltaPitchAngle;
	Vector3 deltaMovement;
	Vector3 oldVelocity;
	Vector3 deltaVelocity;
	Vector3 velocity;
	Vector3 accel;
	float centrifugalAccel;
	CarDynamics cardynamics;
		
	[HideInInspector]
	public Transform myTransform;	
	[HideInInspector]
	public Transform mtarget;
	
	Quaternion rotation;
	
	public void Start(){
		myTransform=transform;
		mtarget=target;
		if(mtarget) cardynamics = mtarget.GetComponent<CarDynamics>();
	}
	
	void LateUpdate () {
		if (target){
			if (mycamera==Cameras.MouseOrbit){ // mouse orbit
 				x += Input.GetAxis("Mouse X")*xSpeed;
				y -= Input.GetAxis("Mouse Y")*ySpeed;
		 
				y = ClampAngle(y, yMinLimit, yMaxLimit);
				
				rotation =Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime*mouseDamping);
		 
				distance -= Input.GetAxis("Mouse ScrollWheel")*5;
				distance = Mathf.Clamp(distance, distanceMin, distanceMax);
				
				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
				Vector3 position = rotation*negDistance + target.position;
		 
				myTransform.rotation = rotation;
				myTransform.position = position;
			}
			else if (mycamera==Cameras.Map){ //Map Camera
				myTransform.position=new Vector3(target.position.x,myTransform.position.y,target.position.z);
				myTransform.eulerAngles=new Vector3(myTransform.eulerAngles.x,target.eulerAngles.y,myTransform.eulerAngles.z);
			}
			else if (mycamera==Cameras.SmoothLookAt){ //SmoothLookAt
				// Calculate the current yaw rotation angles
				wantedYawRotationAngle = target.eulerAngles.y + yawAngle;
				currentYawRotationAngle = myTransform.eulerAngles.y;

				wantedHeight = target.position.y + height;
				currentHeight = myTransform.position.y;
				
				// Damp the rotation around the y-axis
				currentYawRotationAngle = Mathf.LerpAngle (currentYawRotationAngle, wantedYawRotationAngle, rotationDamping*Time.deltaTime);
				
				// Damp the height
				currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping*Time.deltaTime);
				
				// Convert the angle into a rotation
				Quaternion currentYawRotation = Quaternion.Euler(0, currentYawRotationAngle, 0);

				// Set the position of the camera on the x-z plane to distance meters behind the target
				myTransform.position = target.position;
				myTransform.position -= currentYawRotation*Vector3.forward*distance;

				// Set the height of the camera
				myTransform.position = new Vector3(myTransform.position.x, currentHeight,myTransform.position.z);
				
				// Always look at the target
				myTransform.LookAt (new Vector3(target.position.x,target.position.y + height + pitchAngle,target.position.z));
				
			}
			else if (mycamera==Cameras.FixedTo){ //FixedTo
				if (mouseOrbitFixedCamera==true){
					x += Input.GetAxis("Mouse X")*xSpeed;
					y -= Input.GetAxis("Mouse Y")*ySpeed;
					y = ClampAngle(y, yMinLimit, yMaxLimit);
				}
				else{
					x=y=0;
				}
				
				rotation =Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(y+target.eulerAngles.x+pitchAngle+deltaPitchAngle, x+target.eulerAngles.y+yawAngle, target.eulerAngles.z), Time.time);
				myTransform.rotation = rotation;
				
				if (dampFixedCamera==true){
					Vector3 nextPosition = new Vector3(target.position.x, target.position.y + height,target.position.z) - myTransform.forward*distance - (deltaMovement.z*target.forward + deltaMovement.y*target.up + deltaMovement.x*target.right);
					myTransform.position = nextPosition;
				}
				else{
					myTransform.eulerAngles=new Vector3 (target.eulerAngles.x+pitchAngle, target.eulerAngles.y+yawAngle, target.eulerAngles.z);
					myTransform.position = new Vector3(target.position.x, target.position.y + height,target.position.z) - myTransform.forward*distance;				
				}
			}				
		}
	}
	
	void FixedUpdate(){
		if (dampFixedCamera==true){
			oldVelocity=velocity;
			velocity=mtarget.InverseTransformDirection(mtarget.rigidbody.velocity);
			deltaVelocity=velocity - oldVelocity;
			
			if (cardynamics!=null) centrifugalAccel=cardynamics.GetCentrifugalAccel();
			accel=deltaVelocity/Time.deltaTime + centrifugalAccel*Vector3.right;
			//if (Mathf.Abs(accel.x)>100) accel.x=100*Mathf.Sign(accel.x);
			//if (Mathf.Abs(accel.y)>100) accel.y=100*Mathf.Sign(accel.y);
			//if (Mathf.Abs(accel.z)>100) accel.z=100*Mathf.Sign(accel.z);
			
			deltaMovement = accel*Time.deltaTime*Time.deltaTime*5;
			deltaMovement.z = Mathf.Clamp(deltaMovement.z,-0.2f,0.2f);
			deltaMovement.y = Mathf.Clamp(deltaMovement.y,-0.1f,0.1f);
			deltaMovement.x = Mathf.Clamp(deltaMovement.x,-0.01f,0.01f);
			deltaPitchAngle=deltaMovement.y*20 - deltaMovement.z*20;
			deltaPitchAngle=Mathf.Clamp(deltaPitchAngle,-5f,5f);
		}
	}	
		
	static float ClampAngle(float yawAngle, float min, float max){
	
		if (yawAngle < -360F) yawAngle += 360F;
		else if (yawAngle > 360F) yawAngle -= 360F;
		
		return Mathf.Clamp(yawAngle, min, max);
	}	
}