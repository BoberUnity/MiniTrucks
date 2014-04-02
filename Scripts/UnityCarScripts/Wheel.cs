//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class Wheel : MonoBehaviour {
	[HideInInspector]
	public WheelPos wheelPos;
	[HideInInspector]
	public PhysicMaterial physicMaterial;
	
	// Graphical wheel representation (to be rotated accordingly)
	public GameObject model;
	public GameObject caliperModel;
	LineRenderer suspensionLineRenderer;
	[HideInInspector]
	public bool showForces=false;
	[HideInInspector]
	public bool isPowered;
	
	float relaxLong,relaxLat;
	float tireDeflection; // vertical deflection of the tire
	float lateralTireDeflection; // lateral deflection of the tire
	float lateralTireStiffness; // lateral stiffness of the tire. The higher is the lateral stiffness, the lower is the lateral deflection of the tire in curves (tire is more stable). This influence force feedback too.
	float longitudinalTireStiffness;
	float overTurningMoment;
	
	[HideInInspector]
	public float pressure=200; //wheel inflation pressure in kPa
	[HideInInspector]
	public float optimalPressure=200; // tire pressure where grip is maximum. If pressure is lower, grip is decreased proportionally. If pressure is higher, grip is always maximum.  (in real life sport tires optimal pressure is 200 kPa, truck tires 800 kPa)
	public bool tirePuncture=false;
	float pressureFactor;
	[HideInInspector]
	public bool rimScraping=false;
	float verticalTireStiffness;
	float tireDampingRate;
	[HideInInspector]
	public bool tirePressureEnabled=true;
	
	float cos;
	float differentialSlipRatio=0f;
	float tanSlipAngle=0f;
	float deltaRatio1;
	float deltaAngle1;
	float localScale;
	
	float lateralSlipVelo;	
	float longitunalSlipVelo;
	float[] slipRatio_hat;
	float[] slipAngle_hat;
	Vector3 force;
	Vector3 totalForce;
	[HideInInspector]
	public Vector3 pos;
	Vector3 modelPosition;
	[HideInInspector]
	public float Fx;
	[HideInInspector]
	public float maxFx;
	[HideInInspector]
	public float Fy;
	[HideInInspector]
	public float maxFy;
	[HideInInspector]
	public float Mz;
	[HideInInspector]
	public float maxMz;
	Vector3 latForce;
	Vector3 longForce;
	
	int layerMask;
	
	public bool onGroundDown;
	[HideInInspector]
	public RaycastHit hitDown;
	
	[HideInInspector]
	public float originalMass;
	// Wheel mass in kg
	public float mass=50;
	// Wheel radius in meters
	public float radius = 0.34f;
	// Rim radius in meters
	public float rimRadius;
	float sidewallHeight=0;
	
	// Wheel width in meters
	public float width = 0.2f;
	// Wheel suspension travel in meters (we use suspensionTravel as it was the suspension length, so min_len=0 and max_len=suspensionTravel)
	[HideInInspector]
	public float suspensionTravel = 0.2f;
	// suspensionRate is the spring rate (in newtons per meter) of the suspension weighted respect the position of the wheel
	[HideInInspector]
	public float suspensionRate=20000; 
	[HideInInspector]
	public float bumpRate = 4000;
	[HideInInspector]
	public float reboundRate = 4000;
	[HideInInspector]
	public float fastBumpFactor=0.3f;
	[HideInInspector]
	public float fastReboundFactor=0.3f;

	// Wheel rotational inertia (moment of inertia) in kg*m^2 (for a wheel the moment of inertia is 1/2M*r^2  so 10* 0.37^2 = 10*0.1369 = 1.369
	[HideInInspector]
	public float rotationalInertia = 0; //1.8f;
	[HideInInspector]
	public float totalRotationalInertia;

	[HideInInspector]
	public float brakeFrictionTorque = 1500; // Maximal braking torque (in Nm)
	[HideInInspector]
	public float handbrakeFrictionTorque = 0; // Maximal handbrake torque (in Nm)
	// Rolling Resistance (friction torque in Nm) 
	float rollingResistanceTorque;
	[HideInInspector]
	public float rollingFrictionCoefficient=0.018f; //tarmac	
	
	//total friction torque (brake, handbrake, rollingResistanceTorque, wheelFrictionTorque)
	float totalFrictionTorque;
	float totalFrictionTorqueImpulse;
	float frictionAngularDelta;
	// Coefficient of static friction of rubber on asphalt/concrete  http://en.wikipedia.org/wiki/Friction#Static_friction
	float staticFrictionCoefficient=1f;

	//friction coefficient used to simulate different surfaces (asphalt,grass, ecc.)
	[HideInInspector]
	public float gripMaterial=1f; // 0.4 - 0.5 grass, mud ; 0.2 ice
	[HideInInspector]
	public float sidewaysGripFactor=1;
	[HideInInspector]
	public float forwardGripFactor=1;
	[HideInInspector]
	public float gripPressure=1;	
	[HideInInspector]
	public float gripSlip=0;
	[HideInInspector]
	public float gripVelo=0;
	
	float m_gripMaterial;
	
	[HideInInspector]
	public float maxSteeringAngle = 33f; // Maximal steering angle (in degrees)
	
	//Pacejka coefficients
	[HideInInspector]
	public float[] a={1.5f,-40f,1600f,2600f,8.7f,0.014f,-0.24f,1.0f,-0.03f,-0.0013f,-0.06f,-8.5f,-0.29f,17.8f,-2.4f}; //lateral values
	[HideInInspector]
	public float[] b={1.5f,-80f,1950f,23.3f,390f,0.05f,0f,0.055f,-0.024f,0.014f,0.26f}; // longitudinal values
	[HideInInspector]
	public float[] c={2.2f,-3.9f,-3.9f,-1.26f,-8.2f,0.025f,0f,0.044f,-0.58f,0.18f,0.043f,0.048f,-0.0035f,-0.18f,0.14f,-1.029f,0.27f,-1.1f}; // aligning values
	
	CarDynamics cardynamics;
	Drivetrain drivetrain;
	PhysicMaterials physicMaterials;
	Axles axles;
	Transform myTransform;
	
	//friction torque relative to the wheel, its used to stop the wheel when the car its upside down and the wheel its rotating
	float wheelFrictionTorque=0.5f;
	[HideInInspector]
	public float lockingTorqueImpulse = 0; // locking torque impulse applied to this wheel
	[HideInInspector]
	public float roadTorqueImpulse; // road torque impulse applied to this wheel
	[HideInInspector]
	public float drivetrainInertia = 0; // drivetrain rotationalInertia as currently connected to this wheel
	// brake input
	public float brake = 0;
	// handbrake input
	public float handbrake = 0;
	// steering input
	public float steering = 0;
	[HideInInspector]
	public float deltaSteering;
	[HideInInspector]
	public float antiRollBarForce = 0; // suspension force externally applied by anti-roll bars
	
	[HideInInspector]
	public float wheelImpulse;
	[HideInInspector]
	public float angularVelocity;
	float oldAngularVelocity;
	[HideInInspector]
	public float slipVelo;
	float slipVeloSmoke;
	float slipVeloThreshold=4;
	
	[HideInInspector]
	public float slipRatio;
	[HideInInspector]
	public float slipAngle;
	[HideInInspector]
	public float longitudinalSlip;
	[HideInInspector]
	public float lateralSlip;
	[HideInInspector]
	public float idealSlipRatio;
	[HideInInspector]
	public float idealSlipAngle;
	float slipSkidAmount;
	float slipSmokeAmount;
	
	public float compression;
	float overTravel;
	
	// state
	[HideInInspector]
	public float wheelTireVelo;
	[HideInInspector]
	public float wheelRoadVelo;
	[HideInInspector]
	public float absRoadVelo;
	[HideInInspector]
	public float dampAbsRoadVelo;
	[HideInInspector]
	public float wheelRoadVeloLat; 
	[HideInInspector]
	public Vector3 wheelVelo;
	[HideInInspector]
	public Vector3 groundNormal;
	float rotation;

	public float normalForce;
	float suspensionForce;
	float tireForce;
	float bumpStopForce;
	float springForce;
	float criticalDamping;
	float radialDampingRatio;
	
	float normalVelocity;
	float oldNormalVelocity;
	float deltaVelocity;
	float accel;
	float nextVelocity;
	float nextCompression;
	float deflectionVelocity;	
	
	float inclination;
	[HideInInspector]
	public float camber;	
	Quaternion camberRotation;
	[HideInInspector]
	public float deltaCamber;	
	float inclination_sin;
	float inclination_rad;
	
	float roadDistance;
	float springLength;
	float radiusLoaded;
	
	Vector3 roadForce;
	Vector3 up, right,forwardNormal, rightNormal;//,forward;
	Quaternion localRotation = Quaternion.identity;
	int lastSkid = -1;
	
	// cached values
	Rigidbody body;
	Transform trs;
	Transform modelTransform;
	Transform caliperModelTransform;
		
	Skidmarks skidmarks;
	ParticleEmitter skidSmoke;
	bool isSkidSmoke=true;
	[HideInInspector]
	public int axleWheelsLength;
	[HideInInspector]
	public int axlesNumber;
	
	float velo;	
	float B_;
	float b_;
	
	//Pseudo ArcTan function faster than Mathf.Atan
	float PseudoAtan(float x){
		float absx=Mathf.Abs(x);
		return x*(1f + 1.1f*absx)/(1f + 2*(1.6f*absx + 1.1f*x*x)/Mathf.PI);
	}
	
	float CalcLongitudinalForce(float Fz,float slipRatio)
	{
		slipRatio*=100f; 		//covert to %
		float FzFz=Fz*Fz;

		//shape factor
		float C=b[0];
				
		// peak factor
		float D=maxFy=(b[1]*FzFz + b[2]*Fz)*m_gripMaterial*forwardGripFactor;
		
		float BCD=(b[3]*FzFz+ b[4]*Fz)*Mathf.Exp(-b[5]*Fz);
		relaxLong=0;
		if (pressure!=0 && tirePressureEnabled==true && longitudinalTireStiffness>0) relaxLong=BCD/longitudinalTireStiffness;
		
		// stiffness factor
		float B=BCD/(C*D);
		
		// curvature factor
		float E=b[6]*FzFz + b[7]*Fz + b[8];
		
		// horizontal shift
		float Sh = 0;
		
		// composite
		float S = slipRatio+ Sh; 
		
		float BS=B*S;
		
		// longitudinal force
		float Fx=D*Mathf.Sin(C*PseudoAtan(BS - E*(BS - PseudoAtan(BS))));
		
		return Fx;
	}
		
	float CalcLateralForce(float Fz,float slipAngle, float inclination)
	{
		float FzFz=Fz*Fz;

		//shape factor
		float C=a[0];
		
		// peak factor
		float D=maxFx=(a[1]*FzFz + a[2]*Fz)*m_gripMaterial*sidewaysGripFactor;
		
		float BCD=a[3]*Mathf.Sin(2*PseudoAtan(Fz/a[4]))*(1 - a[5]*Mathf.Abs(inclination));
		relaxLat=0;
		if (pressure!=0 &&  tirePressureEnabled==true && lateralTireStiffness>0) relaxLat=BCD/lateralTireStiffness;
		
		// stiffness factor
		float B=BCD/(C*D);
		
		// curvature factor
		float E=a[6]*Fz + a[7];
		
		// horizontal shift
		float Sh = 0;
		
		// vertical shift
		//float Sv = a[11]*Fz*inclination + a[12]*Fz + a[13];	// Pacejka89
		float Sv = -a[11]*Fz*Mathf.Abs(inclination*Mathf.Clamp(absRoadVelo*100,0,1)) + a[12]*Fz + a[13];	// Pacejka89 with fix to avoid lateral sliding with two wheels on a kerb

		// composite
		float S = slipAngle + Sh;
		
		float BS=B*S;
		
		// lateral force
		float Fy=D*Mathf.Sin(C*PseudoAtan(BS - E*(BS - PseudoAtan(BS)))) + Sv;
		
		return Fy;
	}
	
	// aligning force(steering wheel force feedback)
  	float CalcAligningForce(float Fz, float slipAngle, float inclination)
	{
		float FzFz=Fz*Fz;

		//shape factor
		float C=c[0];
		
		// peak factor
		float D = maxMz = (c[1]*FzFz + c[2]*Fz)*m_gripMaterial*sidewaysGripFactor;
				
		float BCD = (c[3]*FzFz + c[4]*Fz)*(1 - c[6]*Mathf.Abs(inclination))*Mathf.Exp(-c[5]*Fz);
		
		// stiffness factor
		float B = BCD/(C*D);
		
		// curvature factor
		float E = (c[7]*FzFz + c[8]*Fz + c[9])*(1 - c[10]*Mathf.Abs(inclination));

		// horizontal shift
		float Sh = 0;//c[11]*inclination + c[12]*Fz + c[13];
		
		// vertical shift
		float Sv = (c[14]*FzFz + c[15]*Fz)*inclination + c[16]*Fz + c[17];
		
		// composite
		float S = slipAngle + Sh;
		
		float BS=B*S;
		
		float Mz = D*Mathf.Sin(C*PseudoAtan(BS + E*(PseudoAtan(BS) - BS))) + Sv;

		return Mz;
	}
	
	Vector3 CalcForces(float Fz){
	
		Fz*=0.001f; 	//convert to kN

		//Clamp normal load
		float forcesScale=1;
		float clampFz=20f;
		
		if (Fz>clampFz) {
			forcesScale=Mathf.Clamp(Fz/clampFz,1,2);
			Fz=clampFz;
		}		
		
		// get ideal slip ratio
		if (slipRatio_hat!=null && slipAngle_hat!=null) LookupIdealSlipRatioIdealSlipAngle(Fz);

		wheelTireVelo = angularVelocity*radiusLoaded;
		absRoadVelo = Mathf.Abs(wheelRoadVelo);
				
 		B_=relaxLong*100;	//relaxation long
		b_=relaxLat*100;	//relaxation lat
		
		if (B_<0.35f*cardynamics.invFixedTimeStepScalar) B_=0.35f*cardynamics.invFixedTimeStepScalar;
		if (b_<0.5f*cardynamics.invFixedTimeStepScalar) b_=0.5f*cardynamics.invFixedTimeStepScalar;
		
		// damp sliAngle and slipRatio oscilation
		float factor=velo*0.02f; // divided by 50
		if (factor<1) factor=1;
		B_*=factor;
		b_*=factor;
		
		float max_dampAbsRoadVelo_absRoadVelo=Mathf.Max(absRoadVelo,dampAbsRoadVelo);		
		deltaRatio1 = (wheelTireVelo - wheelRoadVelo) -  max_dampAbsRoadVelo_absRoadVelo*differentialSlipRatio;
		deltaRatio1/= B_;
		differentialSlipRatio += deltaRatio1*Time.deltaTime;
		slipRatio = differentialSlipRatio; 
		slipRatio = Mathf.Clamp(slipRatio ,-1.5f,1.5f);
		
		float slipAngleFactor=1;
		float absSlipAngle=Mathf.Abs(slipAngle);
		float halfIdeal=idealSlipAngle*0.5f;
		if (absSlipAngle<halfIdeal && (wheelPos==WheelPos.REAR_RIGHT || wheelPos==WheelPos.REAR_LEFT)) {
			slipAngleFactor=(absSlipAngle - cardynamics.factor*absSlipAngle + halfIdeal*cardynamics.factor)/halfIdeal;
		}		
		
 		deltaAngle1=wheelRoadVeloLat - max_dampAbsRoadVelo_absRoadVelo*tanSlipAngle;
		deltaAngle1/=b_;
		tanSlipAngle += deltaAngle1*Time.deltaTime;
		slipAngle=(-PseudoAtan(tanSlipAngle)*Mathf.Rad2Deg)/slipAngleFactor;
		slipAngle = Mathf.Clamp(slipAngle,-90f,90f);
				
		longitudinalSlip = slipRatio/idealSlipRatio;
		lateralSlip = slipAngle/idealSlipAngle;
		
		m_gripMaterial=(gripMaterial + gripSlip + gripVelo)*gripPressure*forcesScale;
				
		float rho = Mathf.Max(Mathf.Sqrt(longitudinalSlip*longitudinalSlip + lateralSlip*lateralSlip),0.0001f); // avoid divide-by-zero		
		Fx = (longitudinalSlip/rho)*CalcLongitudinalForce(Fz, rho*idealSlipRatio);
		Fy = (lateralSlip/rho)*CalcLateralForce(Fz, rho*idealSlipAngle, inclination);
		if (cardynamics.enableForceFeedback==true && maxSteeringAngle!=0) Mz = CalcAligningForce(Fz, slipAngle, inclination);
		else Mz=0;
		
		if (float.IsInfinity(Fx) || float.IsNaN(Fx))  Fx=0;
		if (float.IsInfinity(Fy) || float.IsNaN(Fy))  Fy=0;
		
		return new Vector3 (Fx,Fy,Mz);
	}
	
	void LookupIdealSlipRatioIdealSlipAngle(float load)
	{

		int HAT_ITERATIONS = slipRatio_hat.GetLength(0);

		float HAT_LOAD = 0.5f;
		float nf = load;
		if (nf < HAT_LOAD)
		{
			idealSlipRatio = slipRatio_hat[0];
			idealSlipAngle = slipAngle_hat[0];
		}
		else if (nf >= HAT_LOAD*HAT_ITERATIONS)
		{
			idealSlipRatio = slipRatio_hat[HAT_ITERATIONS-1];
			idealSlipAngle = slipAngle_hat[HAT_ITERATIONS-1];
		}
		else
		{
			int lbound = (int)(nf/HAT_LOAD);
			lbound--;
			if (lbound < 0)
				lbound = 0;
			if (lbound >= (int)slipRatio_hat.GetLength(0))
				lbound = (int)slipRatio_hat.GetLength(0)-1;
			float blend = (nf-HAT_LOAD*(lbound+1))/HAT_LOAD;
			idealSlipRatio = slipRatio_hat[lbound]*(1.0f-blend)+slipRatio_hat[lbound+1]*blend;
			idealSlipAngle = slipAngle_hat[lbound]*(1.0f-blend)+slipAngle_hat[lbound+1]*blend;
		}
	}	

	public void CalculateIdealSlipRatioIdealSlipAngle(int tablesize){
		float HAT_LOAD = 0.5f;
		System.Array.Resize(ref slipRatio_hat,tablesize);
		System.Array.Resize(ref slipAngle_hat,tablesize);
		for (int i = 0; i < tablesize; i++)
		{
			FindIdealSlipRatioIdealSlipAngle((i+1)*HAT_LOAD, i,400);
		}
	}
		
	void FindIdealSlipRatioIdealSlipAngle(float load, int i, int iterations)
	{
		float x, y, ymax;
		ymax = 0;
		y=0;
		for (x = -2f; x < 2f; x += 4f/iterations)
		{
			y = CalcLongitudinalForce(load, x);
			if (y > ymax)
			{
				slipRatio_hat[i] = x;
				ymax = y;
			}
		}

		ymax = 0;
		for (x = -20f; x < 20f; x += 40f/iterations)		//angles are in radians (radians goes from 0 rads (0 degrees) to 6.28 rads (360 degrees) so it should be enough a range from -10 to +10)
		{
			y = CalcLateralForce(load, x,0);
			if (y > ymax)
			{
				slipAngle_hat[i] = x;
				ymax = y;
			}
		}
	}
	
	void Awake(){
		m_gripMaterial=gripMaterial;
	}
	
	void Start(){
		myTransform=transform;
		
		trs = myTransform.parent;
		while (trs != null && trs.rigidbody == null) trs = trs.parent;
		if (trs != null) body = trs.rigidbody;
		
		trs = myTransform.parent;
		while (trs.GetComponent<PhysicMaterials>() == null)  {if (trs.parent) trs = trs.parent; else break;}
		physicMaterials = trs.GetComponent<PhysicMaterials>();

		trs = myTransform.parent;		
		while (trs.GetComponent<CarDynamics>() == null)  trs = trs.parent;
		cardynamics = trs.GetComponent<CarDynamics>();
		drivetrain = trs.GetComponent<Drivetrain>();
		axles = trs.GetComponent<Axles>();
		
		localScale=1/(trs.localScale.y*myTransform.localScale.y);
		
		layerMask=1<<trs.gameObject.layer | 1<<myTransform.gameObject.layer;//LayerMask.NameToLayer("Wheel");
		layerMask=~layerMask;	
		
		radiusLoaded=radius;
		if (rimRadius==0) rimRadius=radius*0.735f;
		sidewallHeight=radius - rimRadius;

		if (mass<50*cardynamics.invFixedTimeStepScalar) mass=50*cardynamics.invFixedTimeStepScalar;
		if (rotationalInertia==0 || rotationalInertia<(mass/2)*radius*radius) rotationalInertia=(mass/2)*radius*radius;
		originalMass=mass;
				
		if (model==null) {
			model = new GameObject("temp_model");
			model.transform.parent=transform;
			model.transform.localPosition=new Vector3(0,0,0);
			model.transform.localRotation=Quaternion.identity;
			Debug.LogWarning("UnityCar: wheel model in " + wheelPos + " is missing. Using empty object" + " (" +trs.name+ ")");
		}
		
		modelTransform=model.transform;
		if (caliperModel!=null) caliperModelTransform=caliperModel.transform;
	
		skidmarks = cardynamics.skidmarks;
		if (skidmarks) skidSmoke = skidmarks.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;
		
		suspensionLineRenderer=gameObject.GetComponent<LineRenderer>();
		if (suspensionLineRenderer==null) suspensionLineRenderer = gameObject.AddComponent<LineRenderer>();
		suspensionLineRenderer.material = new Material(Shader.Find("Diffuse"));
		suspensionLineRenderer.material.color = Color.yellow;
		suspensionLineRenderer.SetWidth(0.01f,0.1f);
		suspensionLineRenderer.useWorldSpace = false;
		suspensionLineRenderer.castShadows = false;
	
		camber*=1; // to avoid never assigned warning
		if (camber==0){
			up = myTransform.up;
		}
		else{
			camberRotation = Quaternion.AngleAxis(-camber,forwardNormal);
			up = camberRotation*myTransform.up;
		}			
		
		SetTireStiffness();
	}
	
	public void SetTireStiffness(){
		sidewallHeight=radius - rimRadius;
		verticalTireStiffness=(radius/rimRadius)*0.25f*width*pressure*1000/sidewallHeight;
		radialDampingRatio=0.6f; // 60% of critical damping
		criticalDamping=2*Mathf.Sqrt(mass*verticalTireStiffness);
		tireDampingRate=criticalDamping*radialDampingRatio;
		
		if (pressure==0) lateralTireStiffness=0;
		else lateralTireStiffness=4*width*(pressure + 0.24f*optimalPressure)*(1 - (sidewallHeight/radius)*(radius/width))*1000;
		if (lateralTireStiffness!=0 && lateralTireStiffness<20000) lateralTireStiffness=20000;		
		longitudinalTireStiffness=4*lateralTireStiffness;
	}
	
	Vector3 RoadForce(float normalForce,  Vector3 groundNormal) {
		
		Vector3 forces;
		
		isSkidSmoke=true;
		if (physicMaterials){ 
			physicMaterial = hitDown.collider.sharedMaterial;
 			if (physicMaterial ==physicMaterials.track || physicMaterial==null){
				isSkidSmoke=true;
				gripMaterial=physicMaterials.trackGrip; 
				rollingFrictionCoefficient=physicMaterials.trackRollingFriction;
				staticFrictionCoefficient=physicMaterials.trackStaticFriction;
			}
			else if (physicMaterial ==physicMaterials.grass){
				isSkidSmoke=false;
				gripMaterial=physicMaterials.grassGrip; 
				rollingFrictionCoefficient=physicMaterials.grassRollingFriction;
				staticFrictionCoefficient=physicMaterials.grassStaticFriction;
			}
			else if (physicMaterial ==physicMaterials.sand){
				isSkidSmoke=false;
				gripMaterial=physicMaterials.sandGrip; 
				rollingFrictionCoefficient=physicMaterials.sandRollingFriction;
				staticFrictionCoefficient=physicMaterials.sandStaticFriction;
			}
			else if (physicMaterial ==physicMaterials.offRoad){
				isSkidSmoke=false;
				gripMaterial=physicMaterials.offRoadGrip; 
				rollingFrictionCoefficient=physicMaterials.offRoadRollingFriction;
				staticFrictionCoefficient=physicMaterials.offRoadStaticFriction;
			} 
		}
		
		float angle = maxSteeringAngle*steering;
		localRotation = Quaternion.Euler (0,angle,0); 

		//forward = myTransform.TransformDirection (localRotation*Vector3.forward);
		right = myTransform.TransformDirection (localRotation*Vector3.right);

		inclination_sin = Vector3.Dot(right,groundNormal);
		rightNormal=right - groundNormal*inclination_sin;
		forwardNormal=Vector3.Cross(rightNormal,groundNormal);
		
		inclination_rad = Mathf.Asin(inclination_sin);
		inclination=-(inclination_rad*Mathf.Rad2Deg + camber);
		inclination=Mathf.Clamp(inclination,-15,15);
		//inclination=0;
	
		wheelRoadVelo = Vector3.Dot(wheelVelo, forwardNormal);
		wheelRoadVeloLat = Vector3.Dot(wheelVelo, rightNormal);
		
		forces= CalcForces(normalForce);
		longForce=forwardNormal*forces.x;
		latForce=rightNormal*forces.y;

		totalForce = longForce + latForce;
		roadTorqueImpulse = forces.x*radiusLoaded*Time.deltaTime;
		
		wheelImpulse=0;
		if (drivetrain!=null && drivetrain.clutch!=null){
			if (drivetrain.ratio!=0 && drivetrain.clutch.GetClutchPosition()!=0) wheelImpulse=CalcWheelImpulse(totalFrictionTorqueImpulse, roadTorqueImpulse, totalRotationalInertia, angularVelocity);
		}		

		return totalForce;
	}
	
	float CalcWheelImpulse(float totalFrictionTorqueImpulse, float roadTorqueImpulse, float totalRotationalInertia, float angularVelocity)
	{
		float angularMomentum = Mathf.Clamp(angularVelocity*totalRotationalInertia, -totalFrictionTorqueImpulse, totalFrictionTorqueImpulse);
		return -(angularMomentum + roadTorqueImpulse); // should be angularMomentum*Time.deltaTime (angular impulse is change in angular momentum)
	}		
	
	void CalcWheelMovement(){
		if (model != null){			
			rotation += angularVelocity*Time.deltaTime;
			if (float.IsInfinity(rotation) || float.IsNaN(rotation)) rotation=0;
			modelTransform.localPosition = Vector3.up*(compression - suspensionTravel)*localScale;
			modelTransform.localRotation = Quaternion.Euler(Mathf.Rad2Deg*rotation, maxSteeringAngle*steering, 0);
			myTransform.localEulerAngles= new Vector3(myTransform.localEulerAngles.x,myTransform.localEulerAngles.y,camber);
			if (caliperModel!=null){
				caliperModelTransform.localPosition=modelTransform.localPosition;
				caliperModelTransform.localRotation=Quaternion.Euler(0,maxSteeringAngle*steering,0);
			}
		}
	}
	
	void Update(){		
		if (skidSmoke) CalcSkidSmoke();
		
		CalcWheelMovement();
		
		//Debug.DrawRay(modelPosition, suspensionForce/1000 ,Color.yellow);
		Debug.DrawRay(hitDown.point, up*100,Color.red);
		Debug.DrawRay(hitDown.point, groundNormal*100,Color.green);
		Debug.DrawRay(hitDown.point, longForce/1000,Color.red);
		Debug.DrawRay(hitDown.point, latForce/1000,Color.blue);
	}	
	
	void FixedUpdate() {
		velo=cardynamics.velo*3.6f;
		pos = myTransform.position;
		
		if (camber==0){
			up = myTransform.up;
		}
		else if (deltaCamber!=0) {
			camberRotation = Quaternion.AngleAxis(-camber,forwardNormal);
			up = camberRotation*myTransform.up;
		}
		
		modelPosition = modelTransform.position;
		
		wheelVelo = body.GetPointVelocity(pos);
		dampAbsRoadVelo = cardynamics.dampAbsRoadVelo;
		oldNormalVelocity=normalVelocity;
		rollingResistanceTorque=normalForce*rollingFrictionCoefficient*radiusLoaded;
		
		totalRotationalInertia = rotationalInertia + drivetrainInertia;
		totalFrictionTorque = brakeFrictionTorque*2*brake + handbrakeFrictionTorque*2*handbrake + rollingResistanceTorque + wheelFrictionTorque;
		totalFrictionTorqueImpulse = totalFrictionTorque*Time.deltaTime;
		frictionAngularDelta = totalFrictionTorqueImpulse/totalRotationalInertia;
		
		onGroundDown=Physics.Raycast(pos, -up, out hitDown, suspensionTravel + radiusLoaded,  layerMask);
		
		if (onGroundDown)
		{
			cos=Vector3.Dot(hitDown.normal, up);
			
			groundNormal = hitDown.normal;

			float height=0;
/* 			Renderer renderer  = hitDown.collider.renderer;
			Texture2D tex = renderer.material.mainTexture as Texture2D;
			if (tex){
				Vector2 pixelUV = hitDown.textureCoord;
				pixelUV.x *= tex.width;
				pixelUV.y *= tex.height;
				float HeightOfHeightmap=0.05f;
				float ActualHeight = tex.GetPixel( (int)pixelUV.x, (int)pixelUV.y).grayscale * HeightOfHeightmap;
				height=HeightOfHeightmap - ActualHeight;
			}
 */			
			roadDistance = hitDown.distance + height;
   		if (cos>=0.99f) normalVelocity = -Vector3.Dot(wheelVelo, groundNormal)*cos;
			else normalVelocity = -Vector3.Dot(wheelVelo, up);
						
			deltaVelocity=normalVelocity - oldNormalVelocity;
			accel=deltaVelocity/Time.deltaTime;			
			nextVelocity=normalVelocity + accel*Time.deltaTime;
			
			gripPressure=1;
			pressureFactor=1;
			if (tirePuncture==true) {
				rimScraping=true;
				radiusLoaded=radius - sidewallHeight;
				gripPressure=0.1f;
			}
			else{
				rimScraping=false;
				if (pressure!=0 && tirePressureEnabled==true) {
					pressureFactor=pressure/optimalPressure;
					if (pressureFactor<0.5f) pressureFactor=0.5f;
					gripPressure=Mathf.Sqrt(pressureFactor);
					if (gripPressure>1) gripPressure=1;
				}
			}
			
			if (pressure==0 || tirePuncture==true || tirePressureEnabled==false) springLength=(roadDistance - radiusLoaded);
			
			compression =(suspensionTravel - springLength);
			//compression =suspensionTravel - (hitDown.distance - radius);
			nextCompression=compression + nextVelocity*Time.deltaTime;
			overTravel=compression - suspensionTravel;
			//normalCompression = compression/suspensionTravel;
			compression = Mathf.Clamp(compression,0,suspensionTravel);
			
 			if (pressure!=0 && tirePressureEnabled==true && tirePuncture==false){
				tireForce=TireForce();
			}
			else {
				tireDeflection=0;
				deflectionVelocity=0;
				tireForce=0;
				radiusLoaded=radius;
				verticalTireStiffness=lateralTireStiffness=longitudinalTireStiffness=0;
			}
			
			suspensionForce = SuspensionForce(compression);
			
			bumpStopForce=0;
			if (overTravel>=0 || nextCompression>=suspensionTravel){ 
				bumpStopForce=BumpStopForce(overTravel) - suspensionForce;
				if (bumpStopForce<0) bumpStopForce=0;
				body.AddForceAtPosition(bumpStopForce*up, pos);
			}
			
			normalForce=suspensionForce + bumpStopForce;
			
			if (suspensionLineRenderer!=null){
				if (showForces==true) {suspensionLineRenderer.enabled = true; suspensionLineRenderer.SetPosition(1, new Vector3(0,0.0005f*suspensionForce,0)); }
				else suspensionLineRenderer.enabled=false;
			}
			
			roadForce = RoadForce(normalForce, groundNormal);	
			
			//this code must be executed after RoadForce() function in order to avoid wheel oscillations
			oldAngularVelocity=angularVelocity;
			angularVelocity += (lockingTorqueImpulse - roadTorqueImpulse)/totalRotationalInertia;
			if (Mathf.Abs(angularVelocity) > frictionAngularDelta){
				angularVelocity -= frictionAngularDelta*Mathf.Sign(angularVelocity);
			}
			else
				angularVelocity = 0;

			// damp low speed wheel oscillations
			float deltaAngularVelocity=angularVelocity - oldAngularVelocity;
			float delta=deltaAngularVelocity*0.85f*Mathf.Clamp01(cardynamics.invFixedTimeStepScalar);
			angularVelocity-=delta;
			
			// lateral defletion and overturning moment
			lateralTireDeflection=0;
			if (pressure!=0 && tirePressureEnabled==true && lateralTireStiffness!=0){
				lateralTireDeflection=Fy/lateralTireStiffness;
				if (Mathf.Abs(lateralTireDeflection)<=0.0001f) lateralTireDeflection=0;
				overTurningMoment=0;
				if (lateralTireDeflection!=0){
					overTurningMoment=-normalForce*lateralTireDeflection;
					body.AddTorque(myTransform.forward*overTurningMoment);
					
					//incremental aligning torque due to longitudinal force
					Mz+=Fx*lateralTireDeflection;
				}
			}

			body.AddForceAtPosition(roadForce + suspensionForce*groundNormal, modelPosition);
									
			//Tire static friction
 			float velocitySqrMagnitude= body.velocity.sqrMagnitude;
 			if (velocitySqrMagnitude<=4f){
				float weightForce=CalculateFractionalMass()*-Physics.gravity.y;
				//lateral tire static friction
				Vector3 rightN=Vector3.Cross(myTransform.forward,groundNormal);
				Vector3 rNormal=rightNormal;
				if (velocitySqrMagnitude<1 && (brake!=0 || handbrake!=0)) rNormal=right*(1-Mathf.Max(brake,handbrake)) - rightN*Mathf.Max(brake,handbrake);
				float cosRight=Vector3.Dot(-rNormal, Vector3.up);
				float latGravityForce=weightForce*cosRight*cos;
				
				float staticFrictionForce=staticFrictionCoefficient*m_gripMaterial*sidewaysGripFactor*weightForce;
				float forceLateral=latGravityForce;
				if (staticFrictionForce<Mathf.Abs(latGravityForce)) forceLateral=staticFrictionForce*Mathf.Sign(latGravityForce);
				body.AddForceAtPosition(forceLateral*-rNormal, modelPosition);
				Debug.DrawRay(modelPosition, forceLateral*-rNormal/1000,Color.white);

				//longitudinal tire static friction
 				if (velocitySqrMagnitude<1 && (brake!=0 || handbrake!=0)){
					Vector3 fNormal=Vector3.Cross(myTransform.right,groundNormal);
					float cosForward=Vector3.Dot(fNormal, Vector3.up);
					float brakeFactor=1;
					float handbrakeFactor=1;
					if (wheelPos==WheelPos.FRONT_LEFT || wheelPos==WheelPos.FRONT_RIGHT){
						if (cardynamics.frontRearBrakeBalance>=0.9f || cardynamics.frontRearBrakeBalance<=0.1f) brakeFactor=cardynamics.frontRearBrakeBalance*2*brake;
						handbrakeFactor=cardynamics.frontRearHandBrakeBalance*2*handbrake;
					}
					else{
						if (cardynamics.frontRearBrakeBalance>=0.9f || cardynamics.frontRearBrakeBalance<=0.1f) brakeFactor=(1-cardynamics.frontRearBrakeBalance)*2*brake;
						handbrakeFactor=(1-cardynamics.frontRearHandBrakeBalance)*2*handbrake;
						}
						
					if (brakeFactor<1) brakeFactor=1;
					if (handbrakeFactor<1) handbrakeFactor=1;
					
					float forwardGravityForce=weightForce*cosForward*m_gripMaterial*forwardGripFactor*Mathf.Max(brakeFactor,handbrakeFactor);
					
					float forwardFrictionForce=totalFrictionTorque; //should be forwardFrictionForce=totalFrictionTorque/radiusLoaded;
					float forceForward=forwardGravityForce;
					if (forwardFrictionForce<Mathf.Abs(forwardGravityForce)) forceForward=forwardFrictionForce*Mathf.Sign(forwardGravityForce);
					body.AddForceAtPosition(forceForward*fNormal*Mathf.Max(brake,handbrake), modelPosition);
					Debug.DrawRay(modelPosition, forceForward*fNormal*Mathf.Max(brake,handbrake)/1000,Color.yellow);
				}
			}
 			
			longitunalSlipVelo = Mathf.Abs(wheelTireVelo - wheelRoadVelo);
			lateralSlipVelo = Vector3.Dot(wheelVelo, trs.right);//wheelRoadVeloLat;
			float longSquare=longitunalSlipVelo*longitunalSlipVelo;
			float latSquare=lateralSlipVelo*lateralSlipVelo;
			slipVelo = Mathf.Sqrt(longSquare + latSquare);
			slipVeloSmoke=Mathf.Sqrt(longSquare + Mathf.Abs(lateralSlipVelo)*0.001f);
			
			if (skidmarks!=null) CalcSkidmarks();
			
		}
		else
		{
			compression=0;
			//normalCompression=0;
			normalForce=0;
			roadTorqueImpulse=0;
			wheelImpulse=0;
			wheelTireVelo=wheelRoadVelo=absRoadVelo=wheelRoadVeloLat=0;
			wheelVelo=Vector3.zero;
			relaxLong=relaxLat=tireDeflection=lateralTireDeflection=0;
			suspensionForce=0;
			roadDistance=0;
			radiusLoaded=radius;
			springLength=suspensionTravel;
			roadForce = Vector3.zero;
			Fx=Fy=0;
			slipAngle=slipRatio=0;
			idealSlipRatio=idealSlipAngle=0;
			if (suspensionLineRenderer!=null) suspensionLineRenderer.enabled = false;
			lastSkid=-1;
			slipRatio=0;slipVelo=0;
			longitudinalSlip=0;lateralSlip=0;
			slipVeloSmoke=0;
			rimScraping=false;
			angularVelocity += (lockingTorqueImpulse - roadTorqueImpulse)/totalRotationalInertia;
			if (Mathf.Abs(angularVelocity) > frictionAngularDelta){
				angularVelocity -= frictionAngularDelta*Mathf.Sign(angularVelocity);
			}
			else
				angularVelocity = 0;			
		}
	}
	
	float  BumpStopForce(float overtravel) {
		float fractionalMass=(body.mass/axlesNumber)/axleWheelsLength;
		float bumpStopMaximumRate=fractionalMass*-Physics.gravity.y*32*Mathf.Clamp01(cardynamics.fixedTimeStepScalar*2)*axleWheelsLength;
		float springForce = bumpStopMaximumRate*(overtravel+0.01f);
		float criticalDamping=2*Mathf.Sqrt(fractionalMass*bumpStopMaximumRate);
   	float normalVelocityB; 
		if (cos>=0.8f) normalVelocityB = -Vector3.Dot(wheelVelo, groundNormal)*cos;
		else normalVelocityB = -Vector3.Dot(wheelVelo, up);
		float dampingForce=criticalDamping*cardynamics.fixedTimeStepScalar*normalVelocityB;
		
		float force=springForce + dampingForce;
			
		return force;
	}
	
	//fixes flipping issue when suspensions are bottomed down and wheel very inclined respect of the ground 
/* 	float  BumpStopForce(float overtravel) {
		float fractionalMass=(body.mass/axlesNumber)/axleWheelsLength;
		float bumpStopMaximumRate=fractionalMass*-Physics.gravity.y*25*axleWheelsLength;
		float springForce = bumpStopMaximumRate*(overtravel+0.01f);
		float criticalDamping=2*Mathf.Sqrt(fractionalMass*bumpStopMaximumRate);
		float sin=Mathf.Abs(Mathf.Sin(Vector3.Angle(wheelVelo, myTransform.right)*Mathf.Deg2Rad));
		float velo=Vector3.Dot(wheelVelo, groundNormal)*cos*cos;
		if (sin<0.95f) velo=Vector3.Dot(wheelVelo, up);
		float dampingForce=criticalDamping*cardynamics.fixedTimeStepScalar*-velo;

		float force=springForce + dampingForce;
			
		return force;
	}	 */
	
	float CalculateFractionalMass(){
		float repart;
		float frontRearWeightRepartition=cardynamics.frontRearWeightRepartition;
		if (frontRearWeightRepartition==0 || frontRearWeightRepartition==1) frontRearWeightRepartition=0.5f;
		if (wheelPos==WheelPos.FRONT_LEFT || wheelPos==WheelPos.FRONT_RIGHT) 
			repart=frontRearWeightRepartition/axles.frontAxle.wheels.Length;
		else 
			repart=(1 - frontRearWeightRepartition)/axles.rearAxle.wheels.Length;
		
		return repart*body.mass;
	}
	
	void CalcSkidmarks(){
		if (slipVelo>slipVeloThreshold){
			slipSkidAmount=(slipVelo - slipVeloThreshold)/15;
			skidmarks.markWidth=width;
			lastSkid = skidmarks.AddSkidMark(hitDown.point, hitDown.normal, slipSkidAmount,lastSkid);
		}
		else
			lastSkid = -1;
	}
	
	void CalcSkidSmoke(){
		if (slipVeloSmoke>slipVeloThreshold && isSkidSmoke==true){
			slipSmokeAmount=(slipVeloSmoke - slipVeloThreshold)/15;
			Vector3 staticVel = myTransform.TransformDirection(skidSmoke.localVelocity) + skidSmoke.worldVelocity;
			skidSmoke.Emit(hitDown.point + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)), staticVel + (wheelVelo*0.05f), Random.Range(skidSmoke.minSize, skidSmoke.maxSize)*Mathf.Clamp(slipSmokeAmount*0.1f,0.5f,1), Random.Range(skidSmoke.minEnergy, skidSmoke.maxEnergy), Color.white);
		}
	}

	float SuspensionForce(float compression) {
		springForce = suspensionRate*compression;
		
		float damperForce;
		float forces=springForce;		
		
		//damp high suspension stiffness oscilations
		normalVelocity-=deltaVelocity*0.05f;
		
		normalVelocity-=deflectionVelocity;
		
		float transitionVelo = cardynamics.transitionDamperVelo;
		if (normalVelocity > 0)
		{
			if (normalVelocity < transitionVelo)
				damperForce = normalVelocity*bumpRate;
			else
				damperForce = (normalVelocity - transitionVelo)*bumpRate*fastBumpFactor + transitionVelo*bumpRate;
		}
		else 
		{
			if (normalVelocity > -transitionVelo)
				damperForce = normalVelocity*reboundRate;
			else
				damperForce = (normalVelocity + transitionVelo)*reboundRate*fastReboundFactor - transitionVelo*reboundRate;
		}
		
		forces += damperForce;
		forces += antiRollBarForce;
		
		suspensionForce = forces;
		if (suspensionForce <0) suspensionForce=0;
		
		float tf=0;
		if (pressure!=0 && tirePressureEnabled==true && tirePuncture==false){
			// if mass<150, with high tire pressure oscilations occurs
			float min_mass=150*cardynamics.invFixedTimeStepScalar;
			float m_mass=mass<min_mass?min_mass:mass;
			float accel2 = (suspensionForce - tireForce)/m_mass;
			deflectionVelocity += accel2*Time.deltaTime;
			springLength += (deflectionVelocity - normalVelocity)*Time.deltaTime;
			if (springLength <=0) {springLength =0;tf=tireForce - suspensionForce + damperForce; if (tf<0) tf=0;}
			else if (springLength >suspensionTravel) springLength =suspensionTravel;
			//else if (radiusLoaded==radius - sidewallHeight) springLength=(roadDistance - radiusLoaded);
			//else if (radiusLoaded>radius) springLength+=radiusLoaded - radius;
			//if (roadDistance > suspensionTravel + radius) roadDistance = suspensionTravel + radius;
			if (cardynamics.invFixedTimeStepScalar>=1) deflectionVelocity=0;
		}		
		
		suspensionForce+= tf;
		
		return suspensionForce;
	}

	float TireForce() {
		float damperForce;
		
		radiusLoaded = roadDistance - springLength;
		radiusLoaded= Mathf.Clamp(radiusLoaded,radius - sidewallHeight,radius);
		//if (radiusLoaded>radius) radiusLoaded=radius;
		tireDeflection = radius - radiusLoaded;
		tireDeflection=Mathf.Clamp(tireDeflection, 0, sidewallHeight);
		
		float forces=verticalTireStiffness*tireDeflection;		
		damperForce = deflectionVelocity*tireDampingRate; 
		forces+=damperForce;
				
		tireForce=forces;
		if (tireForce<0) tireForce=0;
		
		return tireForce;
	}		
}
