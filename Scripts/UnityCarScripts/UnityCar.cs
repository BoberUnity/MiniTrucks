//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
//using UnityEditor;

[ExecuteInEditMode()]
//[AddComponentMenu("Physics/UnityCar")]
public static class UnityCar {
	static Drivetrain drivetrain;
	static AxisCarController axisCarController;
	static CarDynamics carDynamics;
	static SoundController soundController;
	static Rigidbody mrigidbody;
	static Axles axles;
	static Wheel wheel;
	static GameObject wheelFL;
	static GameObject wheelFR;
	static GameObject wheelRL;
	static GameObject wheelRR;
	static GameObject model;	
	static BoxCollider boxColliderDown;
	static BoxCollider boxColliderUp;
	static GameObject bodyDown;
	static GameObject bodyUp;
	static GameObject mcollider;
	static GameObject dashBoard;
	static GameObject centerOfMassObject;
	
	//[MenuItem("GameObject/Create Other/UnityCar")]
	public static GameObject CreateNewCar(){
		Debug.LogWarning("Create new car");
		GameObject unityCarObj=new GameObject("UnityCar");
						
		if (unityCarObj.rigidbody==null){
			mrigidbody=unityCarObj.AddComponent<Rigidbody>() as Rigidbody;
			mrigidbody.mass=1000;
			mrigidbody.angularDrag=0;
			mrigidbody.interpolation=RigidbodyInterpolation.Interpolate;
		}
		else
			mrigidbody=unityCarObj.GetComponent<Rigidbody>();

		if (unityCarObj.GetComponent<CarDynamics>()==null)
			carDynamics=unityCarObj.AddComponent<CarDynamics>() as CarDynamics;
		else
			carDynamics=unityCarObj.GetComponent<CarDynamics>();
			
		centerOfMassObject=new GameObject("COG");
		centerOfMassObject.transform.parent=unityCarObj.transform;
		centerOfMassObject.transform.localPosition=Vector3.zero;
		centerOfMassObject.transform.localRotation=Quaternion.identity;
		
		carDynamics.centerOfMass=centerOfMassObject.transform;			
				
		if (unityCarObj.GetComponent<Drivetrain>()==null)
			drivetrain =unityCarObj.AddComponent<Drivetrain>() as Drivetrain;
		else
			drivetrain=unityCarObj.GetComponent<Drivetrain>();
			
		
		if (unityCarObj.GetComponent<AxisCarController>()==null)
			axisCarController=unityCarObj.AddComponent<AxisCarController>() as AxisCarController;
		else
			axisCarController=unityCarObj.GetComponent<AxisCarController>();
			
/* 
		if (unityCarObj.GetComponent<SoundController>()==null)
			soundController=unityCarObj.AddComponent<SoundController>() as SoundController;
		else
			soundController=unityCarObj.GetComponent<SoundController>();
			
		soundController.enabled=false; */

		if (unityCarObj.GetComponent<Axles>()==null)
			axles =unityCarObj.AddComponent<Axles>() as Axles;
		else
			axles =unityCarObj.GetComponent<Axles>();
			
		// to avoid car assigned to default layer
		unityCarObj.layer=1; 

		if (unityCarObj.transform.Find("Body")==null){
			bodyDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
			bodyDown.name="Body";
			Object.DestroyImmediate(bodyDown.collider);
			bodyDown.transform.parent=unityCarObj.transform;
			bodyDown.transform.localPosition=Vector3.zero;
			bodyDown.transform.localRotation=Quaternion.identity;
			bodyDown.transform.localScale=new Vector3(1.5f,0.5f,4);
			
			bodyUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
			bodyUp.name="Body";
			Object.DestroyImmediate(bodyUp.collider);
			bodyUp.transform.parent=unityCarObj.transform;
			bodyUp.transform.localRotation=Quaternion.identity;
			bodyUp.transform.localScale=new Vector3(bodyDown.transform.localScale.x,bodyDown.transform.localScale.y*0.666f,bodyDown.transform.localScale.z/2);
			bodyUp.transform.localPosition=new Vector3(0,bodyDown.transform.localScale.y - (bodyDown.transform.localScale.y-bodyUp.transform.localScale.y)/2,0);
		}
		else{
			bodyDown=unityCarObj.transform.Find("Body").gameObject;
		}
		
		if (unityCarObj.transform.Find("Collider")==null){
			mcollider= new GameObject("ColliderDown");
			mcollider.transform.parent=unityCarObj.transform;
			mcollider.transform.localPosition=Vector3.zero;
			mcollider.transform.localRotation=Quaternion.identity;
			boxColliderDown = mcollider.gameObject.AddComponent<BoxCollider>() as BoxCollider;
			boxColliderDown.transform.localScale=new Vector3(1.5f,0.5f,4);

			mcollider= new GameObject("ColliderUp");
			mcollider.transform.parent=unityCarObj.transform;
			mcollider.transform.localRotation=Quaternion.identity;		
			boxColliderUp = mcollider.gameObject.AddComponent<BoxCollider>() as BoxCollider;
			boxColliderUp.transform.localScale=new Vector3(boxColliderDown.transform.localScale.x,boxColliderDown.transform.localScale.y*0.666f,boxColliderDown.transform.localScale.z/2);
			boxColliderUp.transform.localPosition=new Vector3(0,boxColliderDown.transform.localScale.y-(boxColliderDown.transform.localScale.y-boxColliderUp.transform.localScale.y)/2,0);			
		}
		else{
			mcollider=unityCarObj.transform.Find("Collider").gameObject;
			boxColliderDown = mcollider.gameObject.GetComponent<BoxCollider>();
		}
		
 		if (unityCarObj.transform.Find("wheelFL")==null){
			wheelFL = new GameObject("wheelFL");
			wheelFL.transform.parent=unityCarObj.transform;
			wheel = wheelFL.gameObject.AddComponent<Wheel>() as Wheel;
			wheelFL.transform.localPosition=new Vector3(-boxColliderDown.transform.localScale.x/2+wheel.width/2, -0.1f, boxColliderDown.transform.localScale.z/2 - boxColliderDown.transform.localScale.z/8);
			wheelFL.transform.localRotation=Quaternion.identity;
	
			wheel.showForces=false;
			wheel.wheelPos=WheelPos.FRONT_LEFT;
			axles.frontAxle.leftWheel=wheel;

			model = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			model.name="modelFL";
			Object.DestroyImmediate(model.collider);			
			model.transform.parent=wheelFL.transform;
			model.transform.localPosition=Vector3.zero;
			model.transform.localRotation=Quaternion.identity;
			model.transform.localScale=new Vector3(wheel.width,wheel.radius*2,wheel.radius*2);
			wheel.model=model;
		}
		else{
			wheelFL=unityCarObj.transform.Find("wheelFL").gameObject;
		}
		
		if (LayerMask.NameToLayer("Wheel")!=-1) wheelFL.gameObject.layer=LayerMask.NameToLayer("Wheel");
		
  	if (unityCarObj.transform.Find("wheelFR")==null){
			wheelFR = new GameObject("wheelFR");
			wheelFR.transform.parent=unityCarObj.transform;
			wheel = wheelFR.gameObject.AddComponent<Wheel>() as Wheel;
			wheelFR.transform.localPosition=new Vector3(boxColliderDown.transform.localScale.x/2-wheel.width/2, -0.1f, boxColliderDown.transform.localScale.z/2 - boxColliderDown.transform.localScale.z/8);
			wheelFR.transform.localRotation=Quaternion.identity;		
			
			wheel.showForces=false;
			wheel.wheelPos=WheelPos.FRONT_RIGHT;
			axles.frontAxle.rightWheel=wheel;

			model = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			model.name="modelFR";
			Object.DestroyImmediate(model.collider);			
			model.transform.parent=wheelFR.transform;
			model.transform.localPosition=Vector3.zero;
			model.transform.localRotation=Quaternion.identity;
			model.transform.localScale=new Vector3(wheel.width,wheel.radius*2,wheel.radius*2);
			wheel.model=model;
		}
		else{
			wheelFR=unityCarObj.transform.Find("wheelFR").gameObject;
		}
		if (LayerMask.NameToLayer("Wheel")!=-1) wheelFR.gameObject.layer=LayerMask.NameToLayer("Wheel");
		
  	if (unityCarObj.transform.Find("wheelRL")==null){
			wheelRL = new GameObject("wheelRL");
			wheelRL.transform.parent=unityCarObj.transform;
			wheel = wheelRL.gameObject.AddComponent<Wheel>() as Wheel;
			wheelRL.transform.localPosition=new Vector3(-boxColliderDown.transform.localScale.x/2+wheel.width/2, -0.1f, -(boxColliderDown.transform.localScale.z/2 - boxColliderDown.transform.localScale.z/8));
			wheelRL.transform.localRotation=Quaternion.identity;		
			
			wheel.showForces=false;
			wheel.wheelPos=WheelPos.REAR_LEFT;
			axles.rearAxle.leftWheel=wheel;
			
			model = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			model.name="modelRL";
			Object.DestroyImmediate(model.collider);			
			model.transform.parent=wheelRL.transform;
			model.transform.localPosition=Vector3.zero;
			model.transform.localRotation=Quaternion.identity;
			model.transform.localScale=new Vector3(wheel.width,wheel.radius*2,wheel.radius*2);
			wheel.model=model;
		}
		else{
			wheelRL=unityCarObj.transform.Find("wheelRL").gameObject;
		}		
		if (LayerMask.NameToLayer("Wheel")!=-1) wheelRL.gameObject.layer=LayerMask.NameToLayer("Wheel");
		
  	if (unityCarObj.transform.Find("wheelRR")==null){
			wheelRR = new GameObject("wheelRR");
			wheelRR.transform.parent=unityCarObj.transform;
			wheel = wheelRR.gameObject.AddComponent<Wheel>() as Wheel;
			wheelRR.transform.localPosition=new Vector3(boxColliderDown.transform.localScale.x/2-wheel.width/2, -0.1f, -(boxColliderDown.transform.localScale.z/2 - boxColliderDown.transform.localScale.z/8));
			wheelRR.transform.localRotation=Quaternion.identity;		
			
			wheel.showForces=false;
			wheel.wheelPos=WheelPos.REAR_RIGHT;
			axles.rearAxle.rightWheel=wheel;
			
			model = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			model.name="modelRR";
			Object.DestroyImmediate(model.collider);			
			model.transform.parent=wheelRR.transform;
			model.transform.localPosition=Vector3.zero;
			model.transform.localRotation=Quaternion.identity;
			model.transform.localScale=new Vector3(wheel.width,wheel.radius*2,wheel.radius*2);
			wheel.model=model;
		}
		else{
			wheelRR=unityCarObj.transform.Find("wheelRR").gameObject;
		}		
		if (LayerMask.NameToLayer("Wheel")!=-1) wheelRR.gameObject.layer=LayerMask.NameToLayer("Wheel");
				
		axles.frontAxle.maxSteeringAngle=33;
		axles.frontAxle.handbrakeFrictionTorque=0;
		axles.rearAxle.maxSteeringAngle=0;
		axles.rearAxle.handbrakeFrictionTorque=1000;
		axles.frontAxle.tires=CarDynamics.Tires.competition_front;
		axles.rearAxle.tires=CarDynamics.Tires.competition_rear;

		axles.SetWheels();
			
/*  		dashBoard = (GameObject)Instantiate(Resources.Load("Samples/Prefabs/Cars/DashBoard"));
		if (dashBoard ){
			dashBoard.transform.parent=unityCarObj.transform;
			dashBoard.transform.localPosition=Vector3.zero;
			dashBoard.transform.localRotation=Quaternion.identity;				
		} */

		// These are to avoid warning about "assigned but never used"
		drivetrain.enabled=true;
		axisCarController.enabled=true;
		  		
		return unityCarObj;
	}
}
