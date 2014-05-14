//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

[RequireComponent (typeof (Axles))]
[RequireComponent (typeof (Rigidbody))]
public class CarDynamics : MonoBehaviour {
 	float frontnforce;
	float backnforce;
	float factor1;
	float factor2;
	float factor3;
	float deltaFactor;
	[HideInInspector]
	public float factor;
			
	[HideInInspector]
	public float velo;
		
	public enum Controller {axis,mouse,mobile,external,ai};
	public Controller controller;
		
	[HideInInspector]
	public CarController carController;
	AxisCarController axisCarController;
	MouseCarController mouseCarcontroller;
	MobileCarController mobileCarController;
	Drivetrain drivetrain;	
	BrakeLights brakeLights;
	DashBoard dashBoard;
	SteeringWheel steeringWheel;
	SoundController soundController;
	Axles axles;
		
	//[HideInInspector]
	//public Wheel[] frontWheels;
	//[HideInInspector]
	//public Wheel[] rearWheels;
	//[HideInInspector]
	//public Wheel[] axles.allWheels;
	//[HideInInspector]
	//public Wheel[] otherWheels;

	[HideInInspector]
	public float transitionDamperVelo=0.3f;
	
	// A transform object which marks the car's center of gravity.
	// Cars with a higher CoG tend to tilt more in corners.
	// The further the CoG is towards the rear of the car, the more the car tends to oversteer. 
	// If this is not set, the center of mass is calculated from the colliders.
	public Transform centerOfMass;
	GameObject centerOfMassObject;
	Vector3 deltaCenterOfMass;
	Vector3 originalCenterOfMass;
	Transform myTransform;
	
	// used to damp oscilations at low speed
	public float dampAbsRoadVelo=8;
	
	// A factor applied to the car's inertia tensor. 
	// Unity calculates the inertia tensor based on the car's collider shape.
	// This factor lets you scale the tensor, in order to make the car more or less dynamic.
	// A higher inertia makes the car change direction slower, which can make it easier to respond to.
	public float inertiaFactor = 1f;
	
	public float frontRearWeightRepartition=0.5f;
	public float frontRearBrakeBalance=0.65f;
	public float frontRearHandBrakeBalance=0;
		
	public bool enableForceFeedback=false;
	
	[HideInInspector]
	public float forceFeedback;	
	
	[HideInInspector]
	public bool tridimensionalTire=false;	
	
	public enum Tires{competition_front,competition_rear, supersport_front,supersport_rear, sport_front,sport_rear, touring_front,touring_rear, offroad_front,offroad_rear, truck_front,truck_rear};
	
	[HideInInspector]
	public float airDensity =1.2041f; // density of air in kg/m3 at 20°C; 998.2071f density of water in kg/m3 at  20°C (used in wing and aerodynamic resistance calculation)
	public Skidmarks skidmarks;

	[HideInInspector]
	public float xlocalPosition;
	[HideInInspector]
	public float xlocalPosition_orig;
	[HideInInspector]
	public float zlocalPosition;
	[HideInInspector]
	public float zlocalPosition_orig;
	[HideInInspector]
	public float ylocalPosition;
	[HideInInspector]
	public float ylocalPosition_orig;
	
	float normalForceR;
	float normalForceF;
	float antiRollBarForce;
	
	[HideInInspector]
	public float fixedTimeStepScalar;
	[HideInInspector]
	public float invFixedTimeStepScalar;
	
	float invAllWheelsLength;

	Rigidbody body;
	bool tiresFound=false;
		
	float RoundTo (float value, int precision){
		int boh=1;
		for (int i =1; i<=precision; i++ ){
			boh=boh*10;
		}			
		return  Mathf.Round(value*boh) / boh; 
	}
	
	public float GetCentrifugalAccel(){
		float val = 0f;
		foreach(Wheel w in axles.allWheels){
			val += w.Fy;
		}
		return val*invAllWheelsLength/rigidbody.mass; // doesnt work with variable body
	}
	
	public void FixPhysX(){
		// used to force PhysX to update weight transfer calculation when moving colliders
		// used to avoid weird bump stop calculation error
		body.collisionDetectionMode=CollisionDetectionMode.Continuous;
		body.collisionDetectionMode=CollisionDetectionMode.Discrete;
		body.centerOfMass-=deltaCenterOfMass;
	}
	
	void Awake(){
		body=rigidbody;
		originalCenterOfMass=body.centerOfMass;
		myTransform=transform;
		drivetrain= GetComponent<Drivetrain>();
		axisCarController = GetComponent <AxisCarController>();
		mouseCarcontroller = GetComponent <MouseCarController>();
		mobileCarController = GetComponent <MobileCarController>();
		brakeLights=GetComponent <BrakeLights>();
		dashBoard = myTransform.GetComponentInChildren <DashBoard>();
		steeringWheel = transform.GetComponentInChildren <SteeringWheel>();
		soundController=GetComponent<SoundController>();
		SetController(controller.ToString());
		axles=GetComponent<Axles>();
		//frontWheels=axles.frontAxle.wheels;
		//rearWheels=axles.rearAxle.wheels;
		//otherWheels=axles.otherWheels;
		//axles.allWheels=axles.axles.allWheels;
		invAllWheelsLength=1f/axles.allWheels.Length;
		fixedTimeStepScalar=0.02f/Time.fixedDeltaTime;
		invFixedTimeStepScalar=1/fixedTimeStepScalar;		
	}
		
	void Start(){
		body.inertiaTensor *= inertiaFactor;
		axles.frontAxle.oldCamber=axles.frontAxle.camber;
		axles.rearAxle.oldCamber=axles.rearAxle.camber;
		foreach(Axle axle in axles.otherAxles){
			axle.oldCamber=axle.camber;
		}
		SetCenterOfMass();
		SetWheelsParams();
		SetBrakes();
		SetTiresType();
		
		xlocalPosition_orig=xlocalPosition;
		ylocalPosition_orig=ylocalPosition;
		zlocalPosition_orig=zlocalPosition;
	}
	
	public Vector3 BoundingSize(Collider[] colliders){
		float sizex=0,sizey=0,sizez=0;
		if (colliders.Length!=0) {
			foreach(Collider mcollider in colliders){
				if (mcollider.gameObject.layer!=LayerMask.NameToLayer("Wheel") && mcollider.transform.GetComponent<FuelTank>()==null){
					sizex+=mcollider.bounds.size.x;
					sizey+=mcollider.bounds.size.y;
					sizez+=mcollider.bounds.size.z;
				}
			}
		}	
		return new Vector3(sizex,sizey,sizez);
	}	
	
	public void SetCenterOfMass(Vector3? COGPosition=null){
		if (centerOfMass == null){
			centerOfMassObject=new GameObject("COG");
			centerOfMassObject.transform.parent=transform;
			centerOfMass=centerOfMassObject.transform;
			if (COGPosition!=null) rigidbody.centerOfMass=centerOfMass.localPosition=COGPosition.Value;
			else centerOfMass.localPosition=rigidbody.centerOfMass;
		}
		else{
			if (COGPosition!=null) rigidbody.centerOfMass=centerOfMass.localPosition=COGPosition.Value;
			else rigidbody.centerOfMass=centerOfMass.localPosition;
			deltaCenterOfMass=originalCenterOfMass - centerOfMass.localPosition;
		}
		xlocalPosition=centerOfMass.localPosition.x;
		ylocalPosition=centerOfMass.localPosition.y;
		zlocalPosition=centerOfMass.localPosition.z;
	}

  public void SetBrakes()
  {
    frontRearBrakeBalance = Mathf.Clamp01(frontRearBrakeBalance);
    frontRearHandBrakeBalance = Mathf.Clamp01(frontRearHandBrakeBalance);
    foreach (Wheel w in axles.frontAxle.wheels)
    {
      w.brakeFrictionTorque = axles.frontAxle.brakeFrictionTorque * Mathf.Min(frontRearBrakeBalance, 0.5f) * 2;
      w.handbrakeFrictionTorque = axles.frontAxle.handbrakeFrictionTorque * Mathf.Min(frontRearHandBrakeBalance, 0.5f) * 2;
    }
    foreach (Wheel w in axles.rearAxle.wheels)
    {
      w.brakeFrictionTorque = axles.rearAxle.brakeFrictionTorque * Mathf.Min(1 - frontRearBrakeBalance, 0.5f) * 2;
      w.handbrakeFrictionTorque = axles.rearAxle.handbrakeFrictionTorque * Mathf.Min(1 - frontRearHandBrakeBalance, 0.5f) * 2;
    }
    foreach (Axle axle in axles.otherAxles)
    {
      foreach (Wheel w in axle.wheels)
      {
        w.brakeFrictionTorque = axle.brakeFrictionTorque;
        w.handbrakeFrictionTorque = axle.handbrakeFrictionTorque;
      }
    }
  }
	
	public void SetWheelsParams(){
		foreach(Wheel w in axles.frontAxle.wheels){
			if (w!=null){
				w.forwardGripFactor=axles.frontAxle.forwardGripFactor;
				w.sidewaysGripFactor=axles.frontAxle.sidewaysGripFactor;
				w.suspensionTravel=axles.frontAxle.suspensionTravel;
				w.suspensionRate=axles.frontAxle.suspensionRate;
				w.bumpRate=axles.frontAxle.bumpRate;
				w.reboundRate=axles.frontAxle.reboundRate;
				w.fastBumpFactor=axles.frontAxle.fastBumpFactor;
				w.fastReboundFactor=axles.frontAxle.fastReboundFactor;
				w.pressure=axles.frontAxle.tiresPressure;
				w.optimalPressure=axles.frontAxle.optimalTiresPressure;
				w.SetTireStiffness();
				w.maxSteeringAngle=axles.frontAxle.maxSteeringAngle;
				w.axleWheelsLength=axles.frontAxle.wheels.Length;
				w.axlesNumber=axles.otherAxles.Length+2;
				if (w.wheelPos==WheelPos.FRONT_RIGHT){
					w.deltaCamber=-axles.frontAxle.deltaCamber;
					w.camber=-axles.frontAxle.camber;
				}
				else{
					w.deltaCamber=axles.frontAxle.deltaCamber;
					w.camber=axles.frontAxle.camber;
				}
			}
		}	
		foreach(Wheel w in axles.rearAxle.wheels){
			if (w!=null){
				w.forwardGripFactor=axles.rearAxle.forwardGripFactor;
				w.sidewaysGripFactor=axles.rearAxle.sidewaysGripFactor;
				w.suspensionTravel=axles.rearAxle.suspensionTravel;
				w.suspensionRate=axles.rearAxle.suspensionRate;
				w.bumpRate=axles.rearAxle.bumpRate;
				w.reboundRate=axles.rearAxle.reboundRate;
				w.fastBumpFactor=axles.rearAxle.fastBumpFactor;
				w.fastReboundFactor=axles.rearAxle.fastReboundFactor;
				w.pressure=axles.rearAxle.tiresPressure;
				w.optimalPressure=axles.rearAxle.optimalTiresPressure;
				w.SetTireStiffness();
				w.maxSteeringAngle=-axles.rearAxle.maxSteeringAngle;
				w.axleWheelsLength=axles.rearAxle.wheels.Length;
				w.axlesNumber=axles.otherAxles.Length+2;
				if (w.wheelPos==WheelPos.REAR_RIGHT){
					w.deltaCamber=-axles.rearAxle.deltaCamber;
					w.camber=-axles.rearAxle.camber;
				}
				else{
					w.deltaCamber=axles.rearAxle.deltaCamber;
					w.camber=axles.rearAxle.camber;
				}
			}
		}
		foreach(Axle axle in axles.otherAxles){
			foreach(Wheel w in axle.wheels){
				if (w!=null){
					w.forwardGripFactor=axle.forwardGripFactor;
					w.sidewaysGripFactor=axle.sidewaysGripFactor;
					w.suspensionTravel=axle.suspensionTravel;
					w.suspensionRate=axle.suspensionRate;
					w.bumpRate=axle.bumpRate;
					w.reboundRate=axle.reboundRate;
					w.fastBumpFactor=axle.fastBumpFactor;
					w.fastReboundFactor=axle.fastReboundFactor;
					w.pressure=axle.tiresPressure;
					w.optimalPressure=axle.optimalTiresPressure;
					w.SetTireStiffness();
					w.maxSteeringAngle=axle.maxSteeringAngle;
					w.axleWheelsLength=axle.wheels.Length;
					w.axlesNumber=axles.otherAxles.Length+2;
					if (w==axle.rightWheel){
						w.deltaCamber=-axle.deltaCamber;
						w.camber=-axle.camber;
					}
					else{
						w.deltaCamber=axle.deltaCamber;
						w.camber=axle.camber;
					}
				}
			}
		}	
	}
	
	public void SetTiresType(){
		foreach(Wheel w in axles.frontAxle.wheels){
			LoadTiresData(w,axles.frontAxle.tires.ToString());
		}
		if (tiresFound==false) Debug.LogWarning("UnityCar: Tires \"" + axles.frontAxle.tires.ToString() +"\" not found. Using standard tires data on front axle (" +myTransform.name+ ")");
		else Debug.Log("UnityCar: Tires \"" + axles.frontAxle.tires.ToString() + "\" loaded on front axle (" +myTransform.name+ ")");
		
		foreach(Wheel w in axles.rearAxle.wheels){
			LoadTiresData(w,axles.rearAxle.tires.ToString());
		}
		if (tiresFound==false) Debug.LogWarning("UnityCar: Tires \"" + axles.rearAxle.tires.ToString() +"\" not found. Using standard tires data on rear axle (" +myTransform.name+ ")");
		else Debug.Log("UnityCar: Tires \"" + axles.rearAxle.tires.ToString() + "\" loaded on rear axle (" +myTransform.name+ ")");
		
		int i=1;
		foreach(Axle axle in axles.otherAxles){
			foreach(Wheel w in axle.wheels){
				LoadTiresData(w,axle.tires.ToString());
			}
			if (tiresFound==false) Debug.LogWarning("UnityCar: Tires  \"" + axle.tires.ToString() +"\" not found. Using standard tires data on other axle"+i+" (" +myTransform.name+ ")");
			else Debug.Log("UnityCar: Tires \"" + axle.tires.ToString() + "\" loaded on other axle"+i+" (" +myTransform.name+ ")");
			i++;
		}
	}
	
	public void LoadTiresData(Wheel w, string tires){

		tiresFound=true;
 		if (tires=="competition_front"){
			System.Array.Copy(TireParameters.aCompetitionFront, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bCompetitionFront, w.b, w.b.Length);
			if (TireParameters.cCompetitionFront.Length!=0) System.Array.Copy(TireParameters.cCompetitionFront, w.c, w.c.Length);
		}
		else if (tires=="competition_rear"){
			System.Array.Copy(TireParameters.aCompetitionRear, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bCompetitionRear, w.b, w.b.Length);
			if (TireParameters.cCompetitionRear.Length!=0) System.Array.Copy(TireParameters.cCompetitionRear, w.c, w.c.Length);
		}		
		else if (tires=="supersport_front"){
			System.Array.Copy(TireParameters.aSuperSportFront, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bSuperSportFront, w.b, w.b.Length);
			if (TireParameters.cSuperSportFront.Length!=0) System.Array.Copy(TireParameters.cSuperSportFront, w.c, w.c.Length);
		}
		else if (tires=="supersport_rear"){
			System.Array.Copy(TireParameters.aSuperSportRear, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bSuperSportRear, w.b, w.b.Length);
			if (TireParameters.cSuperSportRear.Length!=0) System.Array.Copy(TireParameters.cSuperSportRear, w.c, w.c.Length);
		}		
		
		else if (tires=="sport_front"){
			System.Array.Copy(TireParameters.aSportFront, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bSportFront, w.b, w.b.Length);
			if (TireParameters.cSportFront.Length!=0) System.Array.Copy(TireParameters.cSportFront, w.c, w.c.Length);
		}
		else if (tires=="sport_rear"){
			System.Array.Copy(TireParameters.aSportRear, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bSportRear, w.b, w.b.Length);
			if (TireParameters.cSportRear.Length!=0) System.Array.Copy(TireParameters.cSportRear, w.c, w.c.Length);
		}
		else if (tires=="touring_front"){
			System.Array.Copy(TireParameters.aTouringFront, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bTouringFront, w.b, w.b.Length);
			if (TireParameters.cTouringFront.Length!=0) System.Array.Copy(TireParameters.cTouringFront, w.c, w.c.Length);
		}
		else if (tires=="touring_rear"){
			System.Array.Copy(TireParameters.aTouringRear, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bTouringRear, w.b, w.b.Length);
			if (TireParameters.cTouringRear.Length!=0) System.Array.Copy(TireParameters.cTouringRear, w.c, w.c.Length);
		}		
		else if (tires=="offroad_front"){	//OffRoad tires
			System.Array.Copy(TireParameters.aOffRoadFront, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bOffRoadFront, w.b, w.b.Length);
			if (TireParameters.cOffRoadFront.Length!=0) System.Array.Copy(TireParameters.cOffRoadFront, w.c, w.c.Length);
		}
		else if (tires=="offroad_rear"){	//OffRoad tires
			System.Array.Copy(TireParameters.aOffRoadRear, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bOffRoadRear, w.b, w.b.Length);
			if (TireParameters.cOffRoadRear.Length!=0) System.Array.Copy(TireParameters.cOffRoadRear, w.c, w.c.Length);
		}
		else if (tires=="truck_front"){	//Truck truck tires
			System.Array.Copy(TireParameters.aTruckFront, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bTruckFront, w.b, w.b.Length);
			if (TireParameters.cTruckFront.Length!=0) System.Array.Copy(TireParameters.cTruckFront, w.c, w.c.Length);
		}
		else if (tires=="truck_rear"){	//Truck truck tires
			System.Array.Copy(TireParameters.aTruckRear, w.a, w.a.Length);
			System.Array.Copy(TireParameters.bTruckRear, w.b, w.b.Length);
			if (TireParameters.cTruckRear.Length!=0) System.Array.Copy(TireParameters.cTruckRear, w.c, w.c.Length);
		}
		else{
			tiresFound=false;
		}
		w.CalculateIdealSlipRatioIdealSlipAngle(20);
	}
	
	public void SetController(string strcontroller){
		axisCarController = GetComponent <AxisCarController>(); // to make this function works with SaveSetup()
		mouseCarcontroller = GetComponent <MouseCarController>(); // to make this function works with SaveSetup()
		mobileCarController = GetComponent <MobileCarController>(); // to make this function works with SaveSetup()
		if (strcontroller==Controller.axis.ToString() ) 
    {
      if (axisCarController==null)
      {
				axisCarController=transform.gameObject.AddComponent<AxisCarController>();
			}
			axisCarController.enabled=true;
			carController=axisCarController;
			if (drivetrain!=null) drivetrain.carController=axisCarController;
			if (brakeLights!=null) brakeLights.carController=axisCarController;
			if (dashBoard!=null) dashBoard.carController=axisCarController;
			if (steeringWheel!=null) steeringWheel.carController=axisCarController;
			if (soundController!=null) soundController.carController=axisCarController;
			if (mouseCarcontroller!=null) mouseCarcontroller.enabled=false;
			if (mobileCarController!=null) mobileCarController.enabled=false;
			controller=Controller.axis;
		}	
		else if (strcontroller==Controller.mouse.ToString()){
			if (mouseCarcontroller==null){
				mouseCarcontroller=transform.gameObject.AddComponent<MouseCarController>();
			}
			mouseCarcontroller.enabled=true;
			mouseCarcontroller.smoothInput=false;
			carController=mouseCarcontroller;
			if (drivetrain!=null) drivetrain.carController=mouseCarcontroller;
			if (brakeLights!=null) brakeLights.carController=mouseCarcontroller;
			if (dashBoard!=null) dashBoard.carController=mouseCarcontroller;
			if (steeringWheel!=null) steeringWheel.carController=mouseCarcontroller;
			if (soundController!=null) soundController.carController=mouseCarcontroller;
			if (axisCarController!=null) axisCarController.enabled=false;
			if (mobileCarController!=null) mobileCarController.enabled=false;
			controller=Controller.mouse;
		}
 		else if (strcontroller==Controller.mobile.ToString()){
			if (mobileCarController==null){
				mobileCarController=transform.gameObject.AddComponent<MobileCarController>();
			}
			mobileCarController.enabled=true;
			carController=mobileCarController;
			if (drivetrain!=null) drivetrain.carController=mobileCarController;
			if (brakeLights!=null) brakeLights.carController=mobileCarController;
			if (dashBoard!=null) dashBoard.carController=mobileCarController;
			if (steeringWheel!=null) steeringWheel.carController=mobileCarController;
			if (soundController!=null) soundController.carController=mobileCarController;
			if (axisCarController!=null) axisCarController.enabled=false;
			if (mouseCarcontroller!=null) mouseCarcontroller.enabled=false;
			controller=Controller.mobile;
		}
	
		else if (strcontroller==Controller.external.ToString() ){
			if (mouseCarcontroller!=null) {mouseCarcontroller.throttle=drivetrain.throttle=0;mouseCarcontroller.enabled=false;}
			if (axisCarController!=null) {axisCarController.throttle=drivetrain.throttle=0;axisCarController.enabled=false;}
			if (mobileCarController!=null) {mobileCarController.throttle=drivetrain.throttle=0;mobileCarController.enabled=false;}
			controller=Controller.external;
		}		
	}
	
	void FixedUpdate(){
		body.centerOfMass = centerOfMass.localPosition; // needed to fix weird Unity 3.5 (or PhysX 2.8) bug: moving or translating car's transform in runtime resets rigidbody COG position
		
		fixedTimeStepScalar=0.02f/Time.fixedDeltaTime;
		invFixedTimeStepScalar=1/fixedTimeStepScalar;
		
		velo=Mathf.Abs(myTransform.InverseTransformDirection(body.velocity).z);
			
		if (Application.isEditor) {
			SetBrakes();
			SetController(controller.ToString());
			
			axles.frontAxle.deltaCamber=axles.frontAxle.camber - axles.frontAxle.oldCamber;
			if (axles.frontAxle.deltaCamber!=0) {
				axles.frontAxle.oldCamber=axles.frontAxle.camber;
				SetWheelsParams();
			}

			axles.rearAxle.deltaCamber=axles.rearAxle.camber - axles.rearAxle.oldCamber;
			if (axles.frontAxle.deltaCamber!=0) {
				axles.rearAxle.oldCamber=axles.rearAxle.camber;
				SetWheelsParams();
			}
			
			foreach(Axle axle in axles.otherAxles){
				axle.deltaCamber=axle.camber - axle.oldCamber;
				if (axle.deltaCamber!=0) {
					axle.oldCamber=axle.camber;
					SetWheelsParams();
				}
			}
		}
		
		if (axles.frontAxle.leftWheel!=null && axles.frontAxle.rightWheel!=null){
			antiRollBarForce = (axles.frontAxle.leftWheel.compression - axles.frontAxle.rightWheel.compression)*axles.frontAxle.antiRollBarRate;
			axles.frontAxle.leftWheel.antiRollBarForce = +antiRollBarForce;
			axles.frontAxle.rightWheel.antiRollBarForce= -antiRollBarForce;
		}
		
		if (axles.rearAxle.leftWheel!=null && axles.rearAxle.rightWheel!=null){
			antiRollBarForce = (axles.rearAxle.leftWheel.compression - axles.rearAxle.rightWheel.compression)*axles.rearAxle.antiRollBarRate;
			axles.rearAxle.leftWheel.antiRollBarForce = +antiRollBarForce;
			axles.rearAxle.rightWheel.antiRollBarForce= -antiRollBarForce;
		}
		
		foreach(Axle axle in axles.otherAxles){
			if (axle.leftWheel!=null && axle.rightWheel!=null){
				antiRollBarForce=(axle.leftWheel.compression - axle.rightWheel.compression)*axle.antiRollBarRate;
				axle.leftWheel.antiRollBarForce = +antiRollBarForce;
				axle.rightWheel.antiRollBarForce= -antiRollBarForce;
			}
		}
		
		normalForceF=normalForceR=0;
		foreach(Wheel w in axles.frontAxle.wheels){
			normalForceF+=w.normalForce;
		}
		if (axles.frontAxle.wheels.Length!=0) normalForceF/=axles.frontAxle.wheels.Length;
		
		foreach(Wheel w in axles.rearAxle.wheels){
			normalForceR+=w.normalForce;
		}
		if (axles.rearAxle.wheels.Length!=0) normalForceR/=axles.rearAxle.wheels.Length;
		
		if (normalForceF+normalForceR!=0) frontRearWeightRepartition=RoundTo(normalForceF/(normalForceF+normalForceR),2);
		
 		float nforce=(body.mass*-Physics.gravity.y)*0.25F;
		
		if (zlocalPosition>0) {
			frontnforce=nforce*(1+zlocalPosition);
			backnforce=nforce*(1-zlocalPosition );
		}
		else{
			frontnforce=nforce*(1+zlocalPosition);
			backnforce=nforce*(1-zlocalPosition);
		}
		
		if (normalForceR!=0) factor1 =normalForceF/(normalForceR);
	
		if (backnforce!=0) factor2 =frontnforce/(backnforce);
		
		factor3 =(factor2 + 0.25f*(1-factor2));
		if (factor1<factor3)
			deltaFactor=factor3-factor1;
		else
			deltaFactor=factor1-factor3;

		factor = factor3 - deltaFactor;
		
		factor=Mathf.Clamp(factor, 0.25f,1);
		
		forceFeedback=0;
		if (enableForceFeedback==true) {
			float wheelNum=0;
			foreach(Wheel w in axles.allWheels){
				if (w.maxSteeringAngle!=0) {forceFeedback += w.Mz;wheelNum+=1;}
			}
			forceFeedback/=wheelNum;		
			//forceFeedback = 0.5f*(axles.frontAxle[0].Mz  + axles.frontAxle[0].Mz);
		}
	}
	
	public float SlipVelo() {
		float val = 0f;
		foreach(Wheel w in axles.allWheels){
			val += w.slipVelo;
		}
		return val*invAllWheelsLength;
	}
	
	public bool AllWheelsOnGround(){
		bool onGround=false;
		foreach(Wheel w in axles.allWheels){
			onGround=w.onGroundDown;
			if (onGround==true) break;
		}
		return onGround;
	}	
}

static class TireParameters {

	//Pacejka coefficients		
	public static float[] aCompetitionFront ={1.5f, 0f, 1600f, 2600f, 8.7f, 0.014f, -0.24f, 1f, -0.03f, -0.0013f, -0.15f, -8.5f, -0.29f, 17.8f, -2.4f};//lateral values front
	public static float[] aCompetitionRear = {1.5f, 0f, 1800f, 2600f, 8.7f, 0.014f, -0.24f, 1f, -0.03f, -0.0013f, -0.06f, -8.5f, -0.29f, 17.8f, -2.4f};//lateral values rear
	public static float[] bCompetitionFront ={1.5f, -10f, 1950f, 23.3f, 390f, 0.05f, 0f, 0.055f, -0.024f, 0.014f, 0.26f};//longitudinal values front
	public static float[] bCompetitionRear = {1.5f, -12f, 1700f, 23.3f, 370f, 0.1f, 0f, 0.055f, -0.024f, 0.014f, 0.26f};//longitudinal values rear
	public static float[] cCompetitionFront ={2.2f,-3.9f,-3.9f,-1.26f,-8.2f,0.025f,0f,0.044f,-0.58f,0.18f,0.043f,0.048f,-0.0035f,-0.18f,0.14f,-1.029f,0.27f,-1.1f};//aligning values front
	public static float[] cCompetitionRear = {2.2f,-4.1f,-3.9f,-1.36f,-8.1f,0.025f,0f,0.044f,-0.58f,0.18f,0.043f,0.048f,-0.0035f,-0.18f,0.14f,-1.029f,0.27f,-1.1f};//aligning values rear
	
	public static float[] aSuperSportFront = {1.5f, -20f, 1600f, 2600f, 8.7f, 0.014f, -0.24f, 1f, -0.03f, -0.0013f, -0.15f, -8.5f, -0.29f, 17.8f, -2.4f};
	public static float[] aSuperSportRear =  {1.5f, -27f, 1800f, 2600f, 8.7f, 0.014f, -0.24f, 1f, -0.03f, -0.0013f, -0.06f, -8.5f, -0.29f, 17.8f, -2.4f};
	public static float[] bSuperSportFront = {1.5f, -40f, 1950f, 23.3f, 390f, 0.05f, 0f, 0.055f, -0.024f, 0.014f, 0.26f};
	public static float[] bSuperSportRear =  {1.5f, -55f, 1700f, 23.3f, 370f, 0.1f, 0f, 0.055f, -0.024f, 0.014f, 0.26f};	
	public static float[] cSuperSportFront = {};
	public static float[] cSuperSportRear =  {};	
  
	public static float[] aSportFront=       {1.4f, 0f, 1800f, 2600f, 8.726f, 0f, -0.359f, 1.0f, 0f, -0.00611f, -0.0322f, 0f, 0f, 0f, 0f};
	public static float[] aSportRear=        {1.4f, 0f, 1800f, 2600f, 8.726f, 0f, -0.359f, 1.0f, 0f, -0.00611f, -0.0322f, 0f, 0f, 0f, 0f};
	public static float[] bSportFront={1.65f, 0f, 1690f, 0f, 229f, 0f, 0f, 0f, -1.0f, 0f, 0f, 0f, 0f};
	public static float[] bSportRear= {1.65f, 0f, 1690f, 0f, 229f, 0f, 0f, 0f, -1.0f, 0f, 0f, 0f, 0f};
	public static float[] cSportFront={};
	public static float[] cSportRear= {};
  
	public static float[] aTouringFront={1.55f,-27f,1750f,1900f,7.2f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.15f,-8.5f,-0.29f,17.8f,-2.4f};
	public static float[] aTouringRear= {1.55f,-27f,1750f,1900f,7.2f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.15f,-8.5f,-0.29f,17.8f,-2.4f};
	public static float[] bTouringFront={1.65f,-55f,1800f,23.3f,410f,0.075f,0f,0.055f,-0.024f,0.014f,0.26f};
	public static float[] bTouringRear= {1.65f,-55f,1800f,23.3f,410f,0.075f,0f,0.055f,-0.024f,0.014f,0.26f};
	public static float[] cTouringFront={};
	public static float[] cTouringRear= {};
	
	public static float[] aOffRoadFront={1.4f,0f,1800f,2600f,8.7f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.06f,-8.5f,-0.29f,17.8f,-2.4f};
	public static float[] aOffRoadRear= {1.4f,0f,1800f,2600f,8.7f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.06f,-8.5f,-0.29f,17.8f,-2.4f};
	public static float[] bOffRoadFront={1.5f,0f,1700f,23.3f,370f,0.1f,0f,0.055f,-0.024f,0.014f,0.26f};
	public static float[] bOffRoadRear= {1.5f,0f,1700f,23.3f,370f,0.1f,0f,0.055f,-0.024f,0.014f,0.26f};
	public static float[] cOffRoadFront={};
	public static float[] cOffRoadRear= {};
  
	public static float[] aTruckFront={1.4f,0f,2000f,2600f,8.7f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.06f,-8.5f,-0.29f,17.8f,-2.4f};
	public static float[] aTruckRear= {1.4f,0f,2000f,2600f,8.7f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.06f,-8.5f,-0.29f,17.8f,-2.4f};
	public static float[] bTruckFront={1.5f,0f,2300f,23.3f,370f,0.1f,0f,0.055f,-0.024f,0.014f,0.26f};
	public static float[] bTruckRear= {1.5f,0f,2300f,23.3f,370f,0.1f,0f,0.055f,-0.024f,0.014f,0.26f};	
	public static float[] cTruckFront={};
	public static float[] cTruckRear= {};
}