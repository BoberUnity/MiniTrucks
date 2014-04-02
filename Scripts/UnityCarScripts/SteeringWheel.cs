using UnityEngine;

public class SteeringWheel : MonoBehaviour {
	public float maxSteeringAngle=270;
	public bool rotateAroundY=false;
	Transform myTransform;
	[HideInInspector]
	public CarController carController;
	float z;
	
	void Start () {
		myTransform=transform;
		if (rotateAroundY==true) z=myTransform.localEulerAngles.y;
		else z=myTransform.localEulerAngles.z;
	}
	
	void Update () {
		if (carController) {
			if (rotateAroundY==true) myTransform.localEulerAngles=new Vector3 (myTransform.localEulerAngles.x, z+carController.steering*maxSteeringAngle, myTransform.localEulerAngles.z);
			else myTransform.localEulerAngles=new Vector3 (myTransform.localEulerAngles.x, myTransform.localEulerAngles.y, z+carController.steering*maxSteeringAngle);
		}
	}
}
