//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
using System.Collections; // needed for IEnumerator
using System.Collections.Generic; //needed for List class

[RequireComponent (typeof (Axles))]
public class Drivetrain : MonoBehaviour {
	[HideInInspector]
	public bool engineTorqueFromFile=false;
	[HideInInspector]
	public int torqueRPMValuesLen=0;
	[HideInInspector]
	public float[,] torqueRPMValues = new float[0,0];

	[HideInInspector]
	public Clutch clutch;
	[HideInInspector]
	public CarController carController;
	Setup setup;
	Axles axles;
	[HideInInspector]
	public FuelTank[] fuelTanks;
	
	// Cached rigidbody
	Rigidbody body;	
	Transform myTransform;

	// All the wheels the drivetrain should power
	[HideInInspector]
	public Wheel[] poweredWheels;
			
	// engine's maximal power (in HP) and relative RPM
	public float maxPower = 210;
	public float maxPowerRPM=5000;

	// engine's maximal torque (in Nm) and relative RPM
	public float maxTorque=360;
	public float maxTorqueRPM=2500;
	
	[HideInInspector]
	public float originalMaxPower = 210;	
	
	[HideInInspector]
	public float maxNetPower;
	[HideInInspector]
	public float maxNetPowerRPM;
	[HideInInspector]
	public float maxNetTorque;
	[HideInInspector]
	public float maxNetTorqueRPM;
	
	[HideInInspector]
	public float torque;
	float netTorque;
	float netTorqueImpulse;
	[HideInInspector]
	public float wheelTireVelo;
	
	// powerband RPM range
	public float minRPM = 1000;
	public float maxRPM=6000;
	// can engine stall?
	public bool canStall = false;
	[HideInInspector]
	public bool startEngine=false;
	
	public bool revLimiter=false;
	public float revLimiterTime = 0.1f; //50ms
	[HideInInspector]
	public bool revLimiterTriggered=false;
	[HideInInspector]
	public bool revLimiterReleased=false;
	float timer;
		
	// engine inertia (how fast the engine spins up), in kg*m^2
	public float engineInertia = 0.3f;
	
	// drivetrain inertia in kg*m^2 
	public float drivetrainInertia=0.02f;
	float rotationalInertia;
	
	//engine's friction coefficient.
	//this cause the engine to slow down, and cause engine braking
	public float engineFrictionFactor=0.25f;

	// Engine orientation (typically either Vector3.forward or Vector3.right). 
	// This determines how the car body moves as the engine revs up.	
	public Vector3 engineOrientation = Vector3.forward;

	public enum Transmissions {RWD, FWD, AWD, XWD};
	public Transmissions transmission;
	
	// The gear ratios, including neutral (0) and reverse (negative) gears
	public float[] gearRatios={-2.66f, 0f, 2.66f, 1.91f, 1.39f, 1f, 0.71f};
	[HideInInspector]
	public int neutral=1; //index of neutral gear;
	[HideInInspector]
	public int first=0;
	[HideInInspector]
	public int firstReverse=0;
	
	// The final drive ratio, which is multiplied to each gear ratio
	public float finalDriveRatio = 6.09f;
	
	// Coefficient determining how much torque is Transferred between the wheels when they move at 
	// different speeds, to simulate differential locking.
	public float differentialLockCoefficient = 80;

	// shifter with one button for every gear
	public bool shifter=false;	
	// shift gears automatically?
	public bool automatic = true;
	// shift in reverse automatically? (with automatic mode)
	public bool autoReverse = true;	
	public float shiftDownRPM=2000;
	public float shiftUpRPM;
	// How long the car takes to shift gears in secs
	public float shiftTime = 0.5f;
	[HideInInspector]
	public float clutchMaxTorque=0; // clutch max torque. If left to zero, its calculated automatically from engine torque

	// engage and disengage clutch automatically?
	public bool autoClutch = true;
	
	//clutch engaging and disengaging RPMs with autoclutch
	public float engageRPM=1500;
	public float disengageRPM=1000;
	
	public float _fuelConsumptionAtCostantSpeed = 4.3f; // liters per 100km driving at costant speed
	public float _fuelConsumptionSpeed=130; // speed (km/h) at which vehicle cunsumes fuelConsumptionAtCostantSpeed value
	public float currentConsumption; // liters per 100km
	[HideInInspector]
	public float istantConsumption;
	[HideInInspector]
	public float RPMAtSpeedInLastGear;
	float secondsToCover100Km; // driving at fuelConsumptionSpeed
	
	[HideInInspector]
	public float clutchEngageSpeed;
	float clutchPosition;
	
	[HideInInspector]
	public float throttle;
	[HideInInspector]
	public float idlethrottle;
	float idlethrottleMinRPMDown;
	float idleNetTorque;
	[HideInInspector]
	public float startTorque;
	float startThrottle;
	float nextStartImpulse;
	float duration;

	[HideInInspector]
	public bool shiftTriggered=false;
	[HideInInspector]
	public bool soundPlayed=false;
	[HideInInspector]
	public float clutchDragImpulse;	
	float wheelImpulse;
	float TransferredTorque;
	//float driveShaftSpeed;
	[HideInInspector]
	public float differentialSpeed;
	float clutchSpeed;
	[HideInInspector]
	public bool engaging=false;
	
	float shiftTimer;
	float TimeToShiftAgain;
	[HideInInspector]
	public bool CanShiftAgain=true;
	float ShiftDelay = -1;
	float lastShiftTime = -1;
	
	// state
	public int gear = 1; 
	public float rpm;
	float slipRatio;
	float idealSlipRatio;

	float engineAngularVelo;
	[HideInInspector]
	public 	float angularVelo2RPM=30/Mathf.PI;
	[HideInInspector]
	public 	float RPM2angularVelo=Mathf.PI/30;
	[HideInInspector]
	public 	float KW2CV=1.359f;
	[HideInInspector]
	public 	float CV2KW=0.7358f;
	
	[HideInInspector]
	public float maxPowerDriveShaft;
	[HideInInspector]
	public float currentPower;

	float maxPowerKW;
	float maxPowerAngVel;
	float maxPowerEngineTorque;	
	float P1,P2,P3;
	
	[HideInInspector]
	public float curveFactor;	
	
	[HideInInspector]
	public float frictionTorque;
	[HideInInspector]
	public float powerMultiplier=1;	
	[HideInInspector]
	public float externalMultiplier=1;	
	[HideInInspector]
	public float ratio;
	[HideInInspector]
	public float lastGearRatio;
	
	[HideInInspector]
	public bool changingGear=false;
	bool shiftImmediately=false;
	int nextGear;
	float lockingTorqueImpulse;
	float max_power;
	[HideInInspector]	
	public float drivetrainFraction;
	
	[HideInInspector]
	public float velo;
	[HideInInspector]
	bool fuel=true;
	
	[HideInInspector]
	public float RPMAt130Kmh;
		
	float Sqr (float x) { return x*x; }
	
	// // Fuel consumption in liters/100km. If fuel consumption == 0 ---> The engine doesn't consume any fuel at all
	public float fuelConsumptionAtCostantSpeed{
		get{return _fuelConsumptionAtCostantSpeed;}
		set{
			if (value < 0f){
				_fuelConsumptionAtCostantSpeed = 0f;
			}
			else{
				_fuelConsumptionAtCostantSpeed = value;
			}
		}
	}

	public float fuelConsumptionSpeed{
		get{return _fuelConsumptionSpeed;}
		set{
			if (value < 1f){
				_fuelConsumptionSpeed = 1f;
			}
			else{
				_fuelConsumptionSpeed = value;
			}
		}
	}	
	
	void Awake(){
		engineTorqueFromFile=false;
		torqueRPMValuesLen=0;
		body=rigidbody;	
		myTransform=transform;
		clutch = new Clutch();
		carController = GetComponent <CarController>();
		setup= GetComponent <Setup>();
		axles=GetComponent <Axles>();
		fuelTanks=GetComponentsInChildren<FuelTank>();
		poweredWheels=axles.rearAxle.wheels; // to avoid NullReferenceException
	}
	
	IEnumerator Start() {

		if (setup!=null && setup.enabled==true) {while (setup.loadingSetup==true) yield return new WaitForSeconds(0.02f);}
		
		CalcValues(1,engineTorqueFromFile);
		if (shiftUpRPM==0) shiftUpRPM=maxPowerRPM;
		
		bool found=false;
		for (int i =0; i<gearRatios.Length; i++ ){
			if (gearRatios[i]==0) {neutral=i; first=neutral+1; firstReverse=neutral-1; found=true;}
		}
		if (found==false) Debug.LogError("UnityCar: Neutral gear (a gear with value 0) is missing in gearRatios array. Neutral gear is mandatory" + " (" +myTransform.name+ ")");
		
		SetTransmission(transmission);
		
		if (clutch.maxTorque==0) CalcClutchTorque();
		if (shiftTime==0) shiftTime=0.5f;

		lastGearRatio=gearRatios[gearRatios.Length-1]*finalDriveRatio;
		
		// Make sure the property getters methods are called
		fuelConsumptionAtCostantSpeed = fuelConsumptionAtCostantSpeed;
		fuelConsumptionSpeed=fuelConsumptionSpeed;
		RPMAtSpeedInLastGear=CalcRPMAtSpeedInLastGear(fuelConsumptionSpeed);
		secondsToCover100Km=(100/fuelConsumptionSpeed)*3600;
		//-------------------------------------------------------------------------
		// 2769 seconds = (100km / 130km/h) * 3600 seconds/hour
		//-------------------------------------------------------------------------			
		
		CalcIdleThrottle();
		DisengageClutch();
		StartEngine();
	}
	
	public float CalcRPMAtSpeedInLastGear(float speed){
		if (speed>0) return speed/(axles.frontAxle.leftWheel.radius*2*0.1885f/(Mathf.Abs(gearRatios[gearRatios.Length-1])*finalDriveRatio)); // Mathf.PI*3.6f/60 -> 0.1885	
		else return 0;
	}
		
	public void CalcClutchTorque(){
		clutchMaxTorque=Mathf.Round(maxNetTorque*1.6f)*powerMultiplier;
		clutch.maxTorque=clutchMaxTorque;	
	}
	
	public void SetTransmission(Transmissions transmission){
		foreach(Wheel w in axles.allWheels){
			w.lockingTorqueImpulse=0;
			w.drivetrainInertia=0;
			w.isPowered=false;
		}
		if (transmission==Transmissions.FWD){
			foreach(Wheel w in axles.frontAxle.wheels){
				w.isPowered=true;
			}
			poweredWheels=axles.frontAxle.wheels;
			axles.frontAxle.powered=true;
			axles.rearAxle.powered=false;
			foreach(Axle axle in axles.otherAxles){
				axle.powered=false;
			}
		}
		else if (transmission==Transmissions.RWD){
			foreach(Wheel w in axles.rearAxle.wheels){
				w.isPowered=true;
			}
			poweredWheels=axles.rearAxle.wheels;
			axles.frontAxle.powered=false;
			axles.rearAxle.powered=true;
			foreach(Axle axle in axles.otherAxles){
				axle.powered=false;
			}
		}
		else if (transmission==Transmissions.XWD){
			List<Wheel> wheelsList = new List<Wheel>();
			if (axles.frontAxle.powered==true){
				foreach(Wheel w in axles.frontAxle.wheels){
					w.isPowered=true;
					wheelsList.Add(w);
				}
			}
			if (axles.rearAxle.powered==true){
				foreach(Wheel w in axles.rearAxle.wheels){
					w.isPowered=true;
					wheelsList.Add(w);
				}
			}			
			foreach(Axle axle in axles.otherAxles){
				if (axle.powered==true){
					foreach(Wheel w in axle.wheels){
						w.isPowered=true;
						wheelsList.Add(w);
					}
				}
			}
			poweredWheels=wheelsList.ToArray();//new Wheel[axles.frontAxle.wheels.Length + axles.rearAxle.wheels.Length + OtherAxlesWheels.Length];
			//axles.frontAxle.wheels.CopyTo(poweredWheels, 0);
			//axles.rearAxle.wheels.CopyTo(poweredWheels, axles.frontAxle.wheels.Length);
			//OtherAxlesWheels.CopyTo(poweredWheels, axles.rearAxle.wheels.Length);
		}
		else if (transmission==Transmissions.AWD){
			foreach(Wheel w in axles.allWheels){
				w.isPowered=true;
			}				
			poweredWheels=axles.allWheels;
			axles.frontAxle.powered=true;
			axles.rearAxle.powered=true;
			foreach(Axle axle in axles.otherAxles){
				axle.powered=true;
			}			
		}
		
		drivetrainFraction = 1f/poweredWheels.Length;
	}
	
	public float CalcEngineTorque(float factor, float rpm){
		if (engineTorqueFromFile==true)
			return CalcEngineTorqueExt(factor, rpm);
		else
			return CalcEngineTorqueInt(factor, rpm);
	}	
	
	
 	float CalcEngineTorqueExt(float factor, float RPM){
		if(torqueRPMValuesLen!=0) 
		{
			int rp= FindRightPoint(RPM);
			if(rp==0 || rp==torqueRPMValuesLen) // To the left of the graph or to the right of the graph; use 0
				return 0;
			else
			{
				float result=(RPM-torqueRPMValues[rp,0])/(torqueRPMValues[rp-1,0] - torqueRPMValues[rp,0])*torqueRPMValues[rp-1,1] - (RPM - torqueRPMValues[rp-1,0])/(torqueRPMValues[rp-1,0] - torqueRPMValues[rp,0])*torqueRPMValues[rp,1];
				return result*factor;
			}
		}
		else
			return 0;
	}	

 	int FindRightPoint(float RPM)
	{
		int i;
		for(i=0; i<=torqueRPMValuesLen-1; i++){
			if(torqueRPMValues[i,0]>RPM) break;
		}	  
	  return i;	
	}
		
	float CalcEngineTorqueInt(float factor, float rpm) 
	{
		float result;
		if(rpm < maxTorqueRPM)
			result = maxTorque*(-Sqr(rpm/maxTorqueRPM - 1) + 1);
		else {
			float maxPowerTorque = (maxPower*CV2KW*1000)/maxPowerAngVel;
			float aproxFactor = (maxTorque - maxPowerTorque)/(2*maxTorqueRPM*maxPowerRPM - Sqr(maxPowerRPM) - Sqr(maxTorqueRPM));
			float torque = aproxFactor*Sqr(rpm - maxTorqueRPM) + maxTorque;
			result=torque>0?torque:0;
		} 
		
		if(rpm<0 || result<0) result=0;
		
		return result*factor;
	}		
			
	// torque curve is calculated from maxPower and maxPowerRPM using a polynomial expression given in Motor Vehicle Dynamics, Genta (1997)
	public float CalcEngineTorqueInt_reference(float factor, float RPM) 
	{
		float result;		
		float currentAngularVelo=RPM*RPM2angularVelo;
				
		result=P1 + P2*currentAngularVelo + P3*(currentAngularVelo*currentAngularVelo);
		
		if (RPM<maxTorqueRPM) result*=1 - Sqr(RPM/maxTorqueRPM - 1);
		
		return result*1000*factor;
	}	
	
	public float CalcEngineFrictionTorque(float factor,float rpm)
	{
		float static_friction = 0.1f;
		if (rpm<minRPM) static_friction=1 - 0.9f*(rpm/minRPM); // rpm=minRPM -> static_friction=0.1f; rpm=0 -> static_friction=1;
		float frictionTorque = maxPowerEngineTorque*factor*engineFrictionFactor*(static_friction + (1.0f - static_friction)*rpm/maxRPM);

		return frictionTorque;
	}
	
	// engine power in CV
	float CalcEnginePower(float rpm,bool total, float factor){
		if (total)
			return (CalcEngineTorque(factor,rpm) - CalcEngineFrictionTorque(factor,rpm))*rpm*RPM2angularVelo*0.001f*KW2CV; 
		else
			return CalcEngineTorque(factor,rpm)*rpm*RPM2angularVelo*0.001f*KW2CV; 
	}
	
	public void StartEngine() {
		engineAngularVelo=(minRPM*RPM2angularVelo)*1.5f;
		//rpm=engineAngularVelo*angularVelo2RPM;
	}
	
	void CalcEngineMaxPower(float powerMultiplier, bool setMaxPower){
		for (float m_maxPowerRPM = minRPM; m_maxPowerRPM < maxRPM; m_maxPowerRPM++)
		{
			float tmpPower1=CalcEnginePower(m_maxPowerRPM ,true,powerMultiplier);
			float tmpPower2=CalcEnginePower(m_maxPowerRPM+1,true,powerMultiplier);

			if (tmpPower2> tmpPower1)
			{
				maxNetPowerRPM=m_maxPowerRPM+1; 
				maxNetPower=tmpPower2;
			}
			
			if (setMaxPower==true){
				tmpPower1=CalcEnginePower(m_maxPowerRPM ,false,powerMultiplier);
				tmpPower2=CalcEnginePower(m_maxPowerRPM+1,false,powerMultiplier);

				if (tmpPower2> tmpPower1)
				{
					maxPowerRPM=m_maxPowerRPM+1; 
					maxPower=tmpPower2;
				}
			}
		}
	}
	
	void CalcengineMaxTorque(float powerMultiplier, bool setMaxTorque){
		for (float m_maxTorqueRPM = minRPM; m_maxTorqueRPM< maxRPM; m_maxTorqueRPM++)
		{
			float tmpTorque1=CalcEngineTorque(powerMultiplier,m_maxTorqueRPM) - CalcEngineFrictionTorque(powerMultiplier,m_maxTorqueRPM);
			float tmpTorque2=CalcEngineTorque(powerMultiplier,m_maxTorqueRPM+1) - CalcEngineFrictionTorque(powerMultiplier,m_maxTorqueRPM+1);
			
			if (tmpTorque2 > tmpTorque1)
			{
				maxNetTorqueRPM=m_maxTorqueRPM+1; 
				maxNetTorque=tmpTorque2;
			}
			
			if (setMaxTorque==true) {
				tmpTorque1=CalcEngineTorque(powerMultiplier,m_maxTorqueRPM);
				tmpTorque2=CalcEngineTorque(powerMultiplier,m_maxTorqueRPM+1);
				
				if (tmpTorque2 > tmpTorque1)
				{
					maxTorqueRPM=m_maxTorqueRPM+1; 
					maxTorque=tmpTorque2;
				}
			}
		}
	}	
	
	public void CalcIdleThrottle(){
		float idleFrictionTorque=CalcEngineFrictionTorque(powerMultiplier,minRPM);
		float idleTorque=CalcEngineTorque(powerMultiplier,minRPM);
		idleNetTorque=idleTorque - idleFrictionTorque;
		for (idlethrottle = 0f; idlethrottle < 1.0f; idlethrottle += 0.0001f){
			if (idleTorque*idlethrottle >= idleFrictionTorque) break;
		}
		idlethrottleMinRPMDown=idlethrottle;
	}
		
	public void CalcValues(float externalFactor, bool setMaxPower){
		maxPowerAngVel=maxPowerRPM*RPM2angularVelo;	
		maxPowerKW=maxPower*CV2KW*externalFactor;
		maxPowerDriveShaft=maxPower*externalFactor;
		P1=maxPowerKW/maxPowerAngVel;
		P2=maxPowerKW/(maxPowerAngVel*maxPowerAngVel);
		P3=(-maxPowerKW)/(maxPowerAngVel*maxPowerAngVel*maxPowerAngVel);		
		maxPowerEngineTorque=CalcEngineTorque(1,maxPowerRPM);
		CalcengineMaxTorque(1,setMaxPower);
		CalcEngineMaxPower(1,setMaxPower);
		originalMaxPower=maxPower;
		curveFactor=externalFactor;
	}		
		
	void FixedUpdate () 
	{		
		if (clutch==null) {clutch= new Clutch();CalcClutchTorque();}
		
		if (shifter==true) automatic=false;

		ratio = gearRatios[gear]*finalDriveRatio;
		
		if (rpm <= minRPM+maxRPM*0.05f)
			idlethrottle=idlethrottleMinRPMDown*((minRPM+500 - Mathf.Clamp(rpm,minRPM,rpm))*0.002f);
		else 
			idlethrottle=0;

		currentPower=CalcEnginePower(rpm,true,powerMultiplier);
				
		float m_engineInertia=engineInertia*powerMultiplier*externalMultiplier;
		float m_drivetrainInertia=drivetrainInertia*powerMultiplier*externalMultiplier;
		
		velo=Mathf.Abs(myTransform.InverseTransformDirection(body.velocity).z);
						
		if ((rpm >= engageRPM || engaging==true) && autoClutch==true && carController.clutchInput==0 && clutch.GetClutchPosition()!=1 && carController.handbrakeInput==0 && ratio!=0)  {
			EngageClutch();
		}
			
		if ((rpm <= disengageRPM && engaging==false) && autoClutch==true) {
			DisengageClutch();
		}
		
		// execute Gear shifting
		if (changingGear==true) DoGearShifting();
		else lastShiftTime = 0;
			
		// Automatic gear shifting
		if (automatic)
		{
			autoClutch=true;
			if (CanShiftAgain==false){
				TimeToShiftAgain= Mathf.Clamp01((Time.time - ShiftDelay)/(shiftTime + shiftTime/2));
				if (TimeToShiftAgain>=1) {
					CanShiftAgain=true;
				}	
			}
			
			if (changingGear==false){		
				if (rpm >= shiftUpRPM){
					if (gear >= 0 && gear < gearRatios.Length - 1 && Mathf.Abs(slipRatio/idealSlipRatio)<=1 && clutch.GetClutchPosition()!=0 && clutch.speedDiff<50 && engaging==false){
						if (CanShiftAgain==true && OnGround()) {
							if (gearRatios[gear]>0) Shift(gear+1);
							else Shift(gear-1);
						}
						CanShiftAgain=false;
						ShiftDelay = Time.time;	
					}
				}
				else if (rpm <= shiftDownRPM ){
					if (gear !=first && gear!=firstReverse && gear!=neutral && gear > 0 && gear < gearRatios.Length && clutch.GetClutchPosition()!=0 && Mathf.Abs(clutch.speedDiff)<50 && engaging==false) {	// we dont shiftdown if we are in the first gear
						if (CanShiftAgain==true && OnGround()) {
							if (velo<3) { // if speed < 10 km/h we shift directly to the first gear
								Shift(first);
							}
							else{
								if (gearRatios[gear]>0) Shift(gear-1);
								else Shift(gear+1);
							}
							CanShiftAgain=false;
							ShiftDelay= Time.time;	
						}
					}	
				}
			}
		}
				
		float averageWheelsAngularVelo = 0;
		wheelImpulse=0;
		rotationalInertia=0;
		wheelTireVelo=0;
		//TODO to be moved in differential class
		foreach(Wheel w in poweredWheels){
			averageWheelsAngularVelo += w.angularVelocity;
			wheelImpulse+=w.wheelImpulse;
			rotationalInertia+=w.rotationalInertia;
			wheelTireVelo+=w.wheelTireVelo; // used in dashboard to calc actual wheel speed 
		}
		
		averageWheelsAngularVelo*=drivetrainFraction;
		wheelTireVelo*=drivetrainFraction;
		
		float totalRotationalInertia=m_drivetrainInertia+rotationalInertia;
				
		//driveShaftSpeed=averageWheelsAngularVelo*finalDriveRatio;
		// we assume the engine stalled if rpm<20
		if (rpm<20 && startTorque==0) {differentialSpeed=0;wheelImpulse=0;}
		clutchSpeed=differentialSpeed*ratio;
		
		fuel=true;
		if (fuelTanks.Length!=0){
			float velokmh=velo*3.6f;
			// check the fuel consumption...
			float speedFactor=Mathf.Clamp(velokmh,50,velokmh)/fuelConsumptionSpeed; // speed (drag) influece on consumption normalized at fuelConsumptionSpeed (130km/h) (clamped at 50km/h cause at lower speed drag is negligible)
			speedFactor*=speedFactor; // drag force grows with square of speed 
			if (RPMAtSpeedInLastGear!=0) istantConsumption=rpm*throttle*fuelConsumptionAtCostantSpeed/(RPMAtSpeedInLastGear*secondsToCover100Km)*speedFactor;
			if (velo>1) currentConsumption=(istantConsumption/velo)*100000; // velo*0.001f -> km/s ; 100/velo*0.001f -> (1/velo)*100000 seconds taken to travel 100 kms
			else currentConsumption=0;
			fuel=false;
			foreach(FuelTank fuelTank in fuelTanks){
				if (fuelTank.currentFuel!=0) fuel=true;
			}
		}
		torque = CalcEngineTorque(powerMultiplier,rpm)*(throttle + startThrottle)*(fuel?1:0);
		frictionTorque=CalcEngineFrictionTorque(powerMultiplier,rpm);
		startThrottle=0;
		startTorque=0;
		netTorque = torque - frictionTorque;
		if (rpm<20 && startTorque==0) netTorque=0;
		if (rpm<minRPM) startThrottle=1 - rpm/minRPM;
		
		if (startEngine==true && rpm<minRPM &&  Time.time>nextStartImpulse) {
			if (duration==0) duration=Time.time + 0.1f; // duration if start impulse 0.1 secs
			if (Time.time>duration) nextStartImpulse = Time.time + 0.2f; // every 0.2 secs
			startThrottle=1;
			startTorque=idleNetTorque*(fuel?1:0);
		}
		else{
			duration=0;
		}
		netTorqueImpulse = (netTorque+startTorque)*Time.deltaTime;
		
		if (engineAngularVelo >=maxRPM*RPM2angularVelo) {
			if (revLimiterTime==0 && revLimiter==true)
				engineAngularVelo=maxRPM*RPM2angularVelo;
			else
				revLimiterTriggered=true; 
		}
		else if (engineAngularVelo <=minRPM*RPM2angularVelo && canStall==false) {
			engineAngularVelo=minRPM*RPM2angularVelo;
		}
		else if (engineAngularVelo<0) {
			engineAngularVelo=0;
		}
		
		rpm = engineAngularVelo*angularVelo2RPM;
		
		if (ratio == 0 || clutch.GetClutchPosition()==0)
		{
			clutchDragImpulse=0;
			differentialSpeed=averageWheelsAngularVelo;
			if (autoClutch) DisengageClutch();
			
			// Apply torque to car body
			body.AddTorque(-engineOrientation*Mathf.Min(Mathf.Abs(netTorque),2000)*Mathf.Sign(netTorque));
		}
		else //clutch engaged or clutch engaging
		{	
			clutchDragImpulse = clutch.GetDragImpulse(engineAngularVelo, clutchSpeed, m_engineInertia, totalRotationalInertia, ratio,wheelImpulse, netTorqueImpulse);
		}

		engineAngularVelo += (netTorqueImpulse - clutchDragImpulse)/(m_engineInertia);
		differentialSpeed += (wheelImpulse + clutchDragImpulse*ratio)/(totalRotationalInertia);
		if (float.IsNaN(differentialSpeed)) differentialSpeed=0; // to avoid NAN errors with zero powered wheels
		float deltaDifferentialSpeed=differentialSpeed - averageWheelsAngularVelo;
		
		slipRatio = 0;
		idealSlipRatio=0;
		float maxAngVel=maxRPM/(Mathf.Abs(ratio)*angularVelo2RPM);
		foreach(Wheel w in poweredWheels)
		{
			if (revLimiter==true && w.angularVelocity>maxAngVel) w.angularVelocity = maxAngVel;
			lockingTorqueImpulse = (averageWheelsAngularVelo - w.angularVelocity)*differentialLockCoefficient*Time.deltaTime;
			w.drivetrainInertia =  m_drivetrainInertia*ratio*ratio*drivetrainFraction*clutch.GetClutchPosition(); // should be clutch.GetClutchPosition()*(engineInertia + gearInertia[curGear])*Sqr(ratio)
			w.angularVelocity +=deltaDifferentialSpeed;
			w.lockingTorqueImpulse = lockingTorqueImpulse;
			slipRatio += w.slipRatio*drivetrainFraction;
			idealSlipRatio += w.idealSlipRatio*drivetrainFraction;
		}
		
		if (revLimiter==true){
			if (revLimiterTriggered==true) {
				revLimiterReleased=false;
				timer+=Time.deltaTime;
				if (timer >=revLimiterTime) {
					timer=0; 
					revLimiterTriggered=false;
					revLimiterReleased=true;
				}
			}
			else
				revLimiterReleased=false;
		}
		else{
			revLimiterTriggered=false;
			revLimiterReleased=false;
		}
	}
	
	void DoGearShifting(){
	
		 //from 1st reverse to neutral or from neutral to 1st gear
		if (shiftImmediately==true) 
		{
			gear=nextGear;
			if (nextGear !=neutral ) shiftTriggered=true; // in order to trigger the shift sound
			changingGear=false;
		}
		else
		{
			if (throttle<=idlethrottle+idlethrottle/10 || automatic==false) {
				if (lastShiftTime==0) lastShiftTime = Time.time;
				shiftTimer = (Time.time - lastShiftTime)/shiftTime;
				
				// disengage clutch
				if (shiftTimer <1f) DisengageClutch();
				
				// put in neutral
				if (shiftTimer >=0.33f) gear=neutral; 
				
				// select next gear and play sound
				if (shiftTimer >=0.66f) {
					gear=nextGear;
					if (soundPlayed==false) shiftTriggered=true;
				}
				
				// engage clutch
				if (shiftTimer >=1) {
					if (rpm < engageRPM || carController.clutchInput!=0) changingGear=false;
				}
			}
		}
	}
	
	void EngageClutch()
	{
		//if (engaging==false ) clutchPosition=0.25f;
		engaging=true;
		int sign=1;
		if (rpm<maxPowerRPM/2) {
			clutchEngageSpeed=Mathf.Clamp(clutchDragImpulse/netTorqueImpulse, Time.fixedDeltaTime*2,1f);
			if (clutchDragImpulse < netTorqueImpulse || netTorque<1) sign=1;
			else sign=-1;
		}
		else{
			clutchEngageSpeed=0.1f;
		}
		
		if (clutchEngageSpeed!=0) clutchPosition += (Time.deltaTime/clutchEngageSpeed)*sign;
		clutchPosition=Mathf.Clamp01(clutchPosition);
		clutch.SetClutchPosition(clutchPosition);
		if (clutchPosition==1) {engaging=false;changingGear=false;}
	}		
	
	void DisengageClutch()
	{		
		// TODO gradually disengage the clutch?
		clutchPosition=0;
		clutch.SetClutchPosition(clutchPosition);
	}
	
	public bool OnGround(){
		bool onGround=false;
		if (poweredWheels!=null){
			foreach(Wheel w in poweredWheels){
				onGround=w.onGroundDown;
				if (onGround==true) break;
			}
		}
		return onGround;
	}
	
	public void Shift(int m_gear)
	{
		if (m_gear <= gearRatios.Length - 1 && m_gear >= 0 && changingGear==false){
			if (autoClutch==false && clutch.GetClutchPosition()!=0){
				// scratching noise?
			}
			else{
				soundPlayed=false;
				changingGear=true;
				nextGear=m_gear;
				if (nextGear==neutral || (gear==neutral && nextGear==first)  || (gear==neutral && nextGear==firstReverse) || shifter==true) { //from 1st reverse to neutral or from neutral to 1st gear
					shiftImmediately=true;
				}			
				else{
					shiftImmediately=false;
				}
			}
		}
	}	
}

public class Clutch {
	public float maxTorque;		
	public float speedDiff;
	float clutch_position;
	float impulseLimit;
		
	public float GetDragImpulse(float engine_speed, float drive_speed, float engineInertia, float totalRotationalInertia, float ratio, float wheelImpulse, float engineTorqueImpulse){
		
		float totalRotationalInertiaR=totalRotationalInertia/(ratio*ratio);
		impulseLimit=clutch_position*maxTorque*Time.deltaTime;
		speedDiff=engine_speed - drive_speed;
		
		float a=engineInertia*totalRotationalInertiaR*speedDiff;
		float b=engineInertia*(wheelImpulse/ratio);
		float c=totalRotationalInertiaR*engineTorqueImpulse;
		
		float lambda=(a - b + c)/(engineInertia + totalRotationalInertiaR);
		lambda = Mathf.Clamp(lambda, -impulseLimit, impulseLimit);		
		return lambda;
	}
	
	//set the clutch engagement, where 1.0 is fully engaged
	public void SetClutchPosition(float value){
		clutch_position = value;
	}
	
	public float GetClutchPosition() {
		return clutch_position;
	}	
}