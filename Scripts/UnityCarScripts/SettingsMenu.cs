//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena and DEtH_MoroZ
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
using System.Collections;

public class SettingsMenu : MonoBehaviour {
	bool toggleTxt;
	//[HideInInspector]
	public GameObject selectedCar;
	bool isEnabled=false;
	
	public bool stressTest=false;	

	//selection grid variables
	Rect GridRect;
	int GridInt;
	int oldGridInt;
	string[] GridEntrys;
	
	Rect ToggleRect;
	Vector2 BumpSize;
	Vector2 StartSize = new Vector2(1024.0f, 768.0f); //start resolution of window
	public Texture2D grid;
	public int gridWidth=354;
	public int gridHeight=262;
	public GUIStyle powerStyle;
	public GUIStyle torqueStyle;
	Color[] buffer;	
	
	float Offset;
	Rect[] NameLabelRect;
	Rect[] SliderRect;
	Rect GearsWindowRect;	
	Rect SettingsWindowRect;
	//scroll rects
	Rect ScrollRect;
	Vector2 ScrollPosition;
	Rect ScrollViewRect;
	Rect[] ScrollLabelsRect;
	Rect[] ScrollSlidersRect;
	
	Color[] colors;
			
	//car scripts variables
	Rigidbody mrigidbody;
	Transform mTransform;
	CarDynamics carDynamics;
	Drivetrain drivetrain;
	CarDebug carDebug;
	CarDamage carDamage;
	AerodynamicResistance aerodynamicResistance;
	CarController carController;
	DashBoard dashBoard;
	Arcader arcader;
	Setup setup;
	Axles axles;
	FuelTank[] fuelTanks;
	float[] currentFuels;
	bool showForces=false;
	bool TCSChanged=false;
	bool ESPChanged=false;
	string[] tireTypes = new string[] {"Competition Front","Competition Rear", "SuperSport Front","SuperSport Rear", "Sport Front","Sport Rear", "Touring Front","Touring Rear", "Offroad Front", "Offroad Rear", "Truck Front", "Truck Rear"};
	int tiresTypeFront;
	int tiresTypeRear;
	float mass;

	string[] transmissionTypes = new string[] {"RWD", "FWD", "AWD"};
	int transmissionType;
	int oldTransmissionType;

	bool mouseController;
	bool lastMouseController;	

	float timer;
	float timer1;
	bool steerAssistanceStateChanged=false;
	bool startTimer=false;
	bool startTimer1=false;
	bool ESPStateChanged=false;
	bool ABSStateChanged=false;
	bool TCSStateChanged=false;
	[HideInInspector]
	public int carsNumber;
	float engageRPM;
	int maxRPM;
	float factor;
	float m_maxTorque;
	//float m_maxNetTorque;
	float oldactualMaxPower,actualMaxPower;
	int i, floor,top;
	int maxKmh;
	//bool ESP;
	int entrysCount;
	bool showPopupDialog = false;
	string message;
	Vector3 boundingSize;
	float zlocalPositionLimit;

	void Awake() {		
		NameLabelRect = new Rect[30];
		SliderRect = new Rect[30];
		GridInt = 0;
		transmissionTypes = new string[] {"RWD", "FWD", "AWD"};
		colors = new Color[] {Color.red,Color.blue,Color.blue,Color.green,Color.yellow,Color.magenta,Color.black,Color.gray};
		ScrollPosition = Vector2.zero;
		if (grid!=null) buffer=grid.GetPixels();
	}
	
	void Start(){
		if (selectedCar!=null) StartCoroutine(ChangeCar(selectedCar));
	}
	
	public IEnumerator ChangeCar(GameObject mselectedCar){
		if(mselectedCar!=null){
			mTransform=mselectedCar.transform;
			mrigidbody = mselectedCar.GetComponent<Rigidbody>();
			carDynamics = mselectedCar.GetComponent<CarDynamics>();
			drivetrain = mselectedCar.GetComponent<Drivetrain>();
			aerodynamicResistance = mselectedCar.GetComponent<AerodynamicResistance>();
			carDebug =  mTransform.GetComponent<CarDebug>();
			carDamage = mselectedCar.GetComponent<CarDamage>();
			carController = mselectedCar.GetComponent<CarDynamics>().carController;
			dashBoard=mselectedCar.transform.GetComponentInChildren<DashBoard>();
			arcader=mselectedCar.transform.GetComponentInChildren<Arcader>();
			setup= mselectedCar.GetComponent<Setup>();
			axles=mselectedCar.GetComponent<Axles>();
			fuelTanks=mselectedCar.GetComponentsInChildren<FuelTank>();
			currentFuels=new float[fuelTanks.Length];
			
			if (setup!=null && setup.enabled==true) {while (setup.loadingSetup==true) yield return new WaitForSeconds(0.02f);}
			if (drivetrain.engineTorqueFromFile==true) drivetrain.CalcValues(factor,drivetrain.engineTorqueFromFile);
			drivetrain.engineTorqueFromFile=false;

			if (Application.isEditor && setup!=null && setup.enabled==true){
				GridEntrys = new string[] {"Engine", "Transmission", "Suspensions", "Brakes" ,"Tires", "Body", "Assistance","Save Setup"};
				entrysCount=8;
			}
			else{
				GridEntrys = new string[] {"Engine", "Transmission", "Suspensions", "Brakes" ,"Tires", "Body", "Assistance"};
				entrysCount=7;
			}			
			
			if (arcader) arcader.enabled=false;
			m_maxTorque=drivetrain.maxTorque;
			//ESP=carController.ESP;
			selectedCar=mselectedCar;
			//carDynamics.SetTiresType();
			tiresTypeFront=(int)axles.frontAxle.tires;
			tiresTypeRear=(int)axles.rearAxle.tires;
			//drivetrain.SetTransmission(drivetrain.transmission);
			transmissionType=oldTransmissionType=(int)drivetrain.transmission;
			//SetCOGPosition(carDynamics.zlocalPosition);
			
			boundingSize=carDynamics.BoundingSize(selectedCar.GetComponentsInChildren<Collider>());
			zlocalPositionLimit=0.8f*boundingSize.x/4.5f;
						
			engageRPM=drivetrain.engageRPM;
			maxRPM=(Mathf.CeilToInt(drivetrain.maxRPM/1000)+1)*1000;
			maxKmh=Mathf.RoundToInt(maxRPM*axles.frontAxle.leftWheel.radius*2*0.1885f/(drivetrain.gearRatios[drivetrain.gearRatios.Length-1]*drivetrain.finalDriveRatio)); // Mathf.PI*3.6f/60 -> 0.1885
			mass=mrigidbody.mass;
			
			StartSize = new Vector2(Screen.width, Screen.height);
			if (grid!=null) floor=(grid.height - gridHeight)/2;
			top=gridHeight+Mathf.RoundToInt(gridHeight*0.17f)+floor;
			RectCalculation(StartSize);
			ScrollRectCalculation(StartSize, drivetrain.gearRatios.Length-2);					
			factor=1;
			if (grid!=null){
				switch(GridInt) {
					case 0: ApplyEngineTorque();break;
					case 1: ApplyGears();break;
				}
			}
		}
	}		

	void Update() {		
		if(Input.GetKeyUp(KeyCode.B))
			isEnabled = !isEnabled;
			
		if (Input.GetKeyUp (KeyCode.N))
			if (dashBoard!=null) dashBoard.gameObject.SetActive(!dashBoard.gameObject.activeSelf);

		if (Input.GetKeyUp(KeyCode.G)){
			showForces=!showForces;
			foreach(Wheel w in axles.allWheels){
				w.showForces=showForces;
			}
		}
				
		if (Input.GetKeyDown(KeyCode.Escape)){
			Restore();
			Application.LoadLevel(0);
		}
		
		if (Input.GetKeyDown (KeyCode.LeftShift)){
			drivetrain.engageRPM=drivetrain.maxPowerRPM;
			if (carController.TCS==true) {carController.TCS=false;TCSChanged=true;}
			if (carController.ESP==true) {carController.ESP=false;ESPChanged=true;}
		}
		
		if (Input.GetKeyUp (KeyCode.LeftShift)){
			drivetrain.engageRPM=engageRPM;
			if (TCSChanged==true) {carController.TCS=true;TCSChanged=false;}
			if (ESPChanged==true) {carController.ESP=true;ESPChanged=false;}
		}		
		
		if (Input.GetKeyDown (KeyCode.R)){
			if (Input.GetKey (KeyCode.LeftShift)){
				carDamage.repair=true;
			}
			else{
				ResetCarPosition();
			}
		}
		
		if (Input.GetKeyDown (KeyCode.F1)){
			if (Time.timeScale!=1) Time.timeScale=1;
			else Time.timeScale=0.05f;
		}

		if (Input.GetKeyDown (KeyCode.F2) && Application.isEditor){
			if (carDebug==null) carDebug=mTransform.gameObject.AddComponent<CarDebug>();
			else carDebug.enabled=!carDebug.enabled;
		}
				
		if (startTimer) {
			timer+=Time.deltaTime;
			if (timer >=2.5f) {timer=0; startTimer=false;}
		}
		
		// Handbrake Logic
		if (carController!=null){
			if (carController.handbrakeInput!=0) {
				startTimer=true; 
				if (carController.steerAssistance==true) {carController.steerAssistance=false; steerAssistanceStateChanged=true;}
				if (carController.ESP==true) {carController.ESP=false; ESPStateChanged=true;}
				if (carController.ABS==true) {carController.ABS=false; ABSStateChanged=true;}
				if (drivetrain.autoClutch==true) drivetrain.clutch.SetClutchPosition(0);
				if (carController.TCS==true) {TCSStateChanged=true;}
			}
			else {
				if (steerAssistanceStateChanged==true && startTimer==false) {carController.steerAssistance=!carController.steerAssistance; steerAssistanceStateChanged=false;}
				if (ESPStateChanged==true && startTimer==false) {carController.ESP=!carController.ESP; ESPStateChanged=false;}
				if (ABSStateChanged==true) {carController.ABS=!carController.ABS; ABSStateChanged=false;}
				if (TCSStateChanged==true && startTimer==false) {TCSStateChanged=false;}
			}
		}
		
		if (carDynamics!=null){
			if (carDynamics.AllWheelsOnGround()==false && mrigidbody.velocity.sqrMagnitude<=1) startTimer1=true;
			else startTimer1=false;
		}
		
		if (startTimer1) {
			timer1+=Time.deltaTime;
			if (timer1 >=2) {ResetCarPosition();timer1=0; startTimer1=false;}
		}
		else
			timer1=0;
	}
		
	void ResetCarPosition(){
		mTransform.position=new Vector3(mTransform.position.x,mTransform.position.y+3f,mTransform.position.z);
		mTransform.eulerAngles=new Vector3(0,mTransform.eulerAngles.y,0);
		mrigidbody.angularVelocity=Vector3.zero;
		mrigidbody.velocity=Vector3.zero;	
	}
	
	void ShowPopupDialog(int windowID) {
		GUI.Label(new Rect(20, 20, 200, 40), message);
		if (GUI.Button(new Rect(80, 60, 60, 30), "OK")) showPopupDialog=!showPopupDialog;
	}

	void OnGUI() {
		if (showPopupDialog){
			GUI.Window(1, new Rect(Screen.width/2 - 110, Screen.height/2 - 40, 220, 100), ShowPopupDialog, "UnityCar 2.0 Pro");
			GUI.BringWindowToFront(1);
		}

		int fact=1;
		if (stressTest==true) fact=2;
		GUI.Box (new Rect(Screen.width/2 -245,Screen.height-20*fact,480,40*fact), "");
		if (stressTest==true) GUI.Label(new Rect(Screen.width/2 -140,Screen.height-40,Screen.width,40), "Press F10 to instantiate a car (number of cars: "+carsNumber+")");
		if (mTransform!=null) {
			GUI.Label(new Rect(Screen.width/2 -245,Screen.height-20,Screen.width,40), "Press B to toggle Car Setup Window, PAGE UP and PAGE DOWN to change car");
		
			if (isEnabled==true){
				SettingsWindowRect = new Rect(10.0f, 5.0f, StartSize.x - 20.0f, StartSize.y - 20.0f);
			}
			else {
				SettingsWindowRect = new Rect(10.0f, 5.0f, StartSize.x - 20.0f, 30.0f);
			}
			SettingsWindowRect = GUI.Window(0, SettingsWindowRect, SettingsWindow, "Car Settings");
		}
	}
	
	//window's function witch switch
	void SettingsWindow(int windowID) {
		if (isEnabled==true){
			oldGridInt=GridInt;
			GridInt = GUI.SelectionGrid(GridRect, GridInt, GridEntrys, entrysCount);
			switch(GridInt) {
				case 0: GridInt = 0;Engine();break;
				case 1: GridInt = 1;Transmission();break;
				case 2: GridInt = 2;Suspensions();break;
				case 3: GridInt = 3;Brakes();break;	
				case 4: GridInt = 4;Tires();break;
				case 5: GridInt = 5;Body();break;
				case 6: GridInt = 6;Assistance();break;
				case 7: GridInt = 7;SaveSetup();break;
			}
		}
		string text=(isEnabled==true?"Minimize Setup Window":"Expand Setup Window");
		isEnabled = GUI.Toggle(ToggleRect, isEnabled, text);
	}
	
	void SaveSetup(){
		GridInt=oldGridInt;
		if (setup.filePath!=""){
			if (setup.SaveSetup()==true) message="Setup succesfully saved.";
			else message="An error occurred during saving setup.";
		}
		else{
			message="No setup file set.";
		}
		showPopupDialog=true;
	}
	
	//engine settings
	void Engine() {
		//power Moltiplier
		GUI.Label(NameLabelRect[0], "Power Moltiplier: "+drivetrain.powerMultiplier+ "X");
		drivetrain.powerMultiplier= RoundTo(GUI.HorizontalSlider(SliderRect[0],drivetrain.powerMultiplier, 0.1f, 5f),1);
		
		GUI.Label(NameLabelRect[1], "max Power: " + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier) + " HP (Max NET Power: " + Mathf.Round(drivetrain.maxNetPower*drivetrain.powerMultiplier) + " HP @ " + drivetrain.maxNetPowerRPM+" RPM)");
		if (drivetrain.engineTorqueFromFile==false) drivetrain.maxPower= RoundTo(GUI.HorizontalSlider(SliderRect[1],drivetrain.maxPower, 10, drivetrain.maxTorque),0);

		GUI.Label(NameLabelRect[2], "max Power RPM: " + drivetrain.maxPowerRPM + " RPM");
		if (drivetrain.engineTorqueFromFile==false) drivetrain.maxPowerRPM= RoundTo(GUI.HorizontalSlider(SliderRect[2],drivetrain.maxPowerRPM, drivetrain.maxTorqueRPM + drivetrain.maxTorqueRPM*0.25f, drivetrain.maxRPM),0);

		GUI.Label(NameLabelRect[3], "max Torque: " + m_maxTorque*drivetrain.powerMultiplier + " Nm (Max NET Torque: " + Mathf.Round(drivetrain.maxNetTorque*drivetrain.powerMultiplier) + " Nm @ " + drivetrain.maxNetTorqueRPM+" RPM)");
		if (drivetrain.engineTorqueFromFile==false) drivetrain.maxTorque= RoundTo(GUI.HorizontalSlider(SliderRect[3],drivetrain.maxTorque, drivetrain.maxPower, drivetrain.maxPower*8),0);

		GUI.Label(NameLabelRect[4], "max Torque RPM: " + drivetrain.maxTorqueRPM + " RPM");
		if (drivetrain.engineTorqueFromFile==false) drivetrain.maxTorqueRPM= RoundTo(GUI.HorizontalSlider(SliderRect[4],drivetrain.maxTorqueRPM, 500, drivetrain.maxPowerRPM - drivetrain.maxPowerRPM*0.25f),0);	

		GUI.Label(NameLabelRect[5], "Engine Friction: " + drivetrain.engineFrictionFactor);
		drivetrain.engineFrictionFactor= RoundTo(GUI.HorizontalSlider(SliderRect[5],drivetrain.engineFrictionFactor, 0.1f, 0.5f),2);
		
		GUI.Label(NameLabelRect[6], "Engine Inertia: " + drivetrain.engineInertia);
		drivetrain.engineInertia= RoundTo(GUI.HorizontalSlider(SliderRect[6],drivetrain.engineInertia, 0.1f, 10f),2);

		drivetrain.revLimiter = GUI.Toggle(NameLabelRect[7], drivetrain.revLimiter, " Rev Limiter");
		drivetrain.canStall = GUI.Toggle(new Rect(NameLabelRect[7].x,NameLabelRect[7].y+20,NameLabelRect[7].width, NameLabelRect[7].height), drivetrain.canStall, " Engine Can Stall");
		
		Rect containerGridRect=new Rect(NameLabelRect[20].x+50,NameLabelRect[20].y,800,600);
		if (grid!=null){
			GUI.BeginGroup(containerGridRect);
				float offsetX= (grid.width - gridWidth)/2;
				float offsetY= (grid.height - gridHeight)/2 - 10;
				
				if (drivetrain.torqueRPMValuesLen>0) drivetrain.engineTorqueFromFile=GUI.Toggle(new Rect(offsetX, -gridHeight*0.2f + offsetY-48, 340,20), drivetrain.engineTorqueFromFile, " Use Table Data");
				
				GUI.Label(new Rect(offsetX+85, -gridHeight*0.2f + offsetY, 340,20), "Current NET Power: " + Mathf.Round(drivetrain.currentPower) + " HP @ " + Mathf.Round(drivetrain.rpm) +" RPM",powerStyle); 
				//torque
				GUI.Label(new Rect(gridWidth + offsetX, gridHeight*0.33f + offsetY, 140,20), "" + Mathf.Round(m_maxTorque*drivetrain.powerMultiplier*1.33f) + " Nm",torqueStyle);
				GUI.Label(new Rect(gridWidth + offsetX, gridHeight*0.5f + offsetY, 140,20), "" + Mathf.Round(m_maxTorque*drivetrain.powerMultiplier) + " Nm",torqueStyle);
				GUI.Label(new Rect(gridWidth + offsetX, gridHeight*0.67f + offsetY,140,20), "" + Mathf.Round(m_maxTorque*drivetrain.powerMultiplier*0.66f) + " Nm",torqueStyle);
				GUI.Label(new Rect(gridWidth + offsetX, gridHeight*0.84f + offsetY,140,20), "" + Mathf.Round(m_maxTorque*drivetrain.powerMultiplier*0.33f) + " Nm",torqueStyle);
				GUI.Label(new Rect(gridWidth + offsetX, gridHeight*1 + offsetY-5,140,20), "" + 0 + " Nm", torqueStyle);

				//power
				GUI.Label(new Rect(offsetX - 8, -gridHeight*0.17f + offsetY, 140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier*1.166f) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, -gridHeight*0.01f + offsetY, 140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.16f + offsetY, 140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier*0.83f) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.33f + offsetY, 140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier*0.664f) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.5f + offsetY, 140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier*0.498f) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.67f + offsetY,140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier*0.332f) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.84f + offsetY,140,20), "" + Mathf.Round(drivetrain.maxPower*drivetrain.powerMultiplier*0.166f) + " CV",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*1 + offsetY-5,140,20), "" + 0 + " CV",powerStyle);

				//RPMs
				float mgridWidth=gridWidth*0.04f;
				GUI.Label(new Rect(offsetX, gridHeight + offsetY + 15, 140,20), "" + 0 ,torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.125f - mgridWidth, gridHeight + offsetY + 15, 140,20), "" + Mathf.Round(maxRPM*0.125f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.25f - mgridWidth, gridHeight + offsetY + 15, 140,20), "" + Mathf.Round(maxRPM*0.25f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.375f - mgridWidth, gridHeight + offsetY + 15, 140,20), "" + Mathf.Round(maxRPM*0.375f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.5f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.5f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.625f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.625f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.75f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.75f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.875f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.875f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*1 - mgridWidth, gridHeight + offsetY + 15,140,20), "" + maxRPM + " RPM",torqueStyle);

				Rect gridRect=new Rect(0, 0,grid.width, grid.height);
				GUI.DrawTexture(gridRect, grid);
			
			GUI.EndGroup();
		}

		if (GUI.changed){
			if (grid!=null) ApplyEngineTorque();
			drivetrain.externalMultiplier=drivetrain.maxPower/drivetrain.originalMaxPower;
			drivetrain.CalcIdleThrottle();
			drivetrain.CalcClutchTorque();
			carController.externalTCSThreshold=-Mathf.Clamp01((drivetrain.powerMultiplier*drivetrain.externalMultiplier - 1)/3);			
		}		
	}
	
	void OnApplicationQuit(){
		Restore();
	}

	void Restore(){
		if (grid!=null){ 
			grid.SetPixels(buffer);
			grid.Apply(false);
		}
	}		
	
	void ApplyEngineTorque() {	
		grid.SetPixels(buffer);
		int rpmNormalized,torqueNormalized,powerNormalized,referenceTorqueNorm,netTorqueNormalized,netPowerNormalized;
		float torque, power, netTorque, netPower, referenceTorque;
		factor=1;
		drivetrain.CalcValues(factor,drivetrain.engineTorqueFromFile);
		for (float rpm=0; rpm<=maxRPM; rpm+=1){
			torque=drivetrain.CalcEngineTorque(1,rpm);
			oldactualMaxPower=actualMaxPower;
			actualMaxPower=torque*rpm*drivetrain.RPM2angularVelo*0.001f*drivetrain.KW2CV;
			if (actualMaxPower>drivetrain.maxPower && actualMaxPower>oldactualMaxPower && torque<drivetrain.maxTorque) factor=drivetrain.maxPower/actualMaxPower;
		}
		m_maxTorque=Mathf.Round(drivetrain.maxTorque*factor);
		//m_maxNetTorque=Mathf.Round(drivetrain.maxNetTorque*factor);
		//float minRPM=Mathf.Min(drivetrain.minRPM,500);
		for (float rpm=0; rpm<=maxRPM; rpm+=1){
			torque=drivetrain.CalcEngineTorque(drivetrain.powerMultiplier*factor,rpm);
			if (torque>m_maxTorque*drivetrain.powerMultiplier) torque=m_maxTorque*drivetrain.powerMultiplier;
			power=torque*rpm*drivetrain.RPM2angularVelo*0.001f*drivetrain.KW2CV;
			referenceTorque = drivetrain.CalcEngineTorqueInt_reference(drivetrain.powerMultiplier,rpm);
			torqueNormalized = Mathf.RoundToInt((torque/(m_maxTorque*drivetrain.powerMultiplier))*gridHeight*0.5f + floor);
			powerNormalized = Mathf.RoundToInt((power/(drivetrain.maxPower*drivetrain.powerMultiplier))*gridHeight + floor);
			referenceTorqueNorm = Mathf.RoundToInt((referenceTorque/(drivetrain.maxTorque*drivetrain.powerMultiplier))*gridHeight*0.5f + floor);
			
			netTorque=torque - drivetrain.CalcEngineFrictionTorque(drivetrain.powerMultiplier,rpm);
			netTorqueNormalized = Mathf.RoundToInt((netTorque/(m_maxTorque*drivetrain.powerMultiplier))*gridHeight*0.5f + floor);
			netPower=netTorque*rpm*drivetrain.RPM2angularVelo*0.001f*drivetrain.KW2CV;
			netPowerNormalized = Mathf.RoundToInt((netPower/(drivetrain.maxPower*drivetrain.powerMultiplier))*gridHeight + floor);
			
			rpmNormalized = Mathf.RoundToInt((rpm/(maxRPM))*gridWidth + (grid.width - gridWidth)/2 - 1);
			
			grid.SetPixel(rpmNormalized,Mathf.Clamp(torqueNormalized,floor,top),Color.black);
			grid.SetPixel(rpmNormalized,Mathf.Clamp(powerNormalized,floor,top),Color.red);
			grid.SetPixel(rpmNormalized,Mathf.Clamp(netTorqueNormalized,floor,top),Color.black);
			grid.SetPixel(rpmNormalized,Mathf.Clamp(netPowerNormalized,floor,top),Color.red);
			if (drivetrain.engineTorqueFromFile==false) grid.SetPixel(rpmNormalized,Mathf.Clamp(referenceTorqueNorm,floor,top),Color.blue);
		}
		grid.Apply(false);
	}

	void ApplyGears() {	
		grid.SetPixels(buffer);
		int index=0;
		maxKmh=Mathf.RoundToInt(maxRPM*axles.frontAxle.leftWheel.radius*2*0.1885f/(drivetrain.gearRatios[drivetrain.gearRatios.Length-1]*drivetrain.finalDriveRatio)); // Mathf.PI*3.6f/60 -> 0.1885
		for(int a = 0; a < drivetrain.gearRatios.Length; a++) {
			if (index==colors.Length) index=0;
			for (float rpm=0; rpm<=maxRPM; rpm+=1){
				if (a!=1){ // skip neutral
					float kmh=rpm*axles.frontAxle.leftWheel.radius*2*0.1885f/(Mathf.Abs(drivetrain.gearRatios[a])*drivetrain.finalDriveRatio); // Mathf.PI*3.6f/60 -> 0.1885
					int kmhNormalized = Mathf.RoundToInt((kmh/maxKmh)*gridHeight + floor);
					if (kmhNormalized<=top){
						int rpmNormalized = Mathf.RoundToInt((rpm/(maxRPM))*gridWidth + (grid.width - gridWidth)/2 - 1);
						grid.SetPixel(rpmNormalized,Mathf.Clamp(kmhNormalized,floor,top),colors[index]);
					}
				}
			}
			index++;
		}
		
		grid.Apply(false);
	}	
	
	//transmission settings
	void Transmission() {		
		// <-------
		//lock coeff
		GUI.Label(NameLabelRect[0], "Differential Lock Coefficient: "+ RoundTo(drivetrain.differentialLockCoefficient,0) + "%");
		drivetrain.differentialLockCoefficient = GUI.HorizontalSlider(SliderRect[0], RoundTo(drivetrain.differentialLockCoefficient,0), 0, 100);
		//Type of drive
		GUI.Label(NameLabelRect[1], "Type of drive: " + drivetrain.transmission.ToString());
		transmissionType = GUI.SelectionGrid(SliderRect[1], transmissionType, transmissionTypes, 3);
		//more setting for diffs
				
		Rect containerGridRect=new Rect(NameLabelRect[1].x-25,NameLabelRect[1].y-30,800,600);
		if (grid!=null){
			GUI.BeginGroup(containerGridRect);
				float offsetX= (grid.width - gridWidth)/2;
				float offsetY= (grid.height - gridHeight)/2 - 10;
							
				//speed
				GUI.Label(new Rect(offsetX - 8, -gridHeight*0.17f + offsetY, 140,20), "" + maxKmh + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, -gridHeight*0.01f + offsetY, 140,20), "" + Mathf.Round(maxKmh*0.857f) + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.16f + offsetY, 140,20), "" + Mathf.Round(maxKmh*0.714f) + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.33f + offsetY, 140,20), "" + Mathf.Round(maxKmh*0.571f) + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.5f + offsetY, 140,20), "" + Mathf.Round(maxKmh*0.42857f) + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.67f + offsetY,140,20), "" + Mathf.Round(maxKmh*0.2857f) + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*0.84f + offsetY,140,20), "" + Mathf.Round(maxKmh*0.143f) + " kmh",powerStyle);
				GUI.Label(new Rect(offsetX - 8, gridHeight*1 + offsetY-5,140,20), "" + 0 + " kmh",powerStyle);

				//RPMs
				float mgridWidth=gridWidth*0.04f;
				GUI.Label(new Rect(offsetX, gridHeight + offsetY + 15, 140,20), "" + 0 ,torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.125f - mgridWidth, gridHeight + offsetY + 15, 140,20), "" + Mathf.Round(maxRPM*0.125f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.25f - mgridWidth, gridHeight + offsetY + 15, 140,20), "" + Mathf.Round(maxRPM*0.25f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.375f - mgridWidth, gridHeight + offsetY + 15, 140,20), "" + Mathf.Round(maxRPM*0.375f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.5f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.5f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.625f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.625f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.75f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.75f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*0.875f - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM*0.875f),torqueStyle);
				GUI.Label(new Rect(offsetX + gridWidth*1 - mgridWidth, gridHeight + offsetY + 15,140,20), "" + Mathf.Round(maxRPM) + " RPM",torqueStyle);

				Rect gridRect=new Rect(0, 0,grid.width, grid.height);
				GUI.DrawTexture(gridRect, grid);
			
			GUI.EndGroup();
		}
		
		string gear;
		if (drivetrain.gear<drivetrain.neutral) gear="R";//reverse
		else if (drivetrain.gear==drivetrain.neutral) gear="H"; // neutral
		else gear=""+(drivetrain.gear-drivetrain.neutral); 
			
		GUI.Label(new Rect(NameLabelRect[20].x+40,NameLabelRect[20].y,NameLabelRect[20].width,NameLabelRect[20].height), "Current Gear Engaged: "+ gear + " @ " + Mathf.Round(drivetrain.rpm) +" RPM");
		GUI.Label(new Rect(NameLabelRect[20].x+40,NameLabelRect[20].y+30,NameLabelRect[20].width,NameLabelRect[20].height), "Current Speed: " + Mathf.Round(Mathf.Abs(drivetrain.velo)*3.6f )+ " km/h");
		//drivetrain.differentialLockCoefficient = GUI.HorizontalSlider(SliderRect[20], RoundTo(drivetrain.differentialLockCoefficient,0), 0, 100);		
				
		//------->
		// Size of gears
		GUI.Label(NameLabelRect[10], "Number of Gears: " +(drivetrain.gearRatios.Length-2));
		// final drive
		GUI.Label(NameLabelRect[11], "Final Drive Ratio: " + drivetrain.finalDriveRatio);
		drivetrain.finalDriveRatio = RoundTo(GUI.HorizontalSlider(SliderRect[11],drivetrain.finalDriveRatio, 0.5f, 15f),2);
		//reverse gear
		GUI.Label(NameLabelRect[12], "Reverse Gear Ratio: " + drivetrain.gearRatios[0]);
		drivetrain.gearRatios[0] = RoundTo(GUI.HorizontalSlider(SliderRect[12],drivetrain.gearRatios[0], -0.5f, -15f),2);
		
		ScrollPosition = GUI.BeginScrollView(ScrollViewRect, ScrollPosition, ScrollRect);
		for(int a = 0; a < drivetrain.gearRatios.Length-2; a++) {
			GUI.Label(ScrollLabelsRect[a]  , "Gear " +(a + 1) + " Ratio: " + drivetrain.gearRatios[a+2]);
			drivetrain.gearRatios[a+2] = RoundTo(GUI.HorizontalSlider(ScrollSlidersRect[a],drivetrain.gearRatios[a+2], 0.5f, 15f),3);
		}
		GUI.EndScrollView();
		
		if (GUI.changed) {
			if (grid!=null) ApplyGears();
			if (oldTransmissionType!=transmissionType) SetTransmission();
		}
	}
	
	//suspension settings
	void Suspensions() {
		// <------- FRONT SUSPENSION
		//suspension travel
		GUI.Label(NameLabelRect[0], "Front Suspension Travel: " + axles.frontAxle.suspensionTravel + " m");
		axles.frontAxle.suspensionTravel = RoundTo(GUI.HorizontalSlider(SliderRect[0],axles.frontAxle.suspensionTravel, 0.1f, 0.5f),2);
		//spring
		GUI.Label(NameLabelRect[1], "Front Suspension Stiffness: " + RoundTo(axles.frontAxle.suspensionRate/1000,1) +" N/mm");
		axles.frontAxle.suspensionRate = RoundTo(GUI.HorizontalSlider(SliderRect[1],axles.frontAxle.suspensionRate, 0, 150000),1);
		//bump
		GUI.Label(NameLabelRect[2], "Front Bump Damping: " +RoundTo(axles.frontAxle.bumpRate/1000,1) + " Ns/mm");
		axles.frontAxle.bumpRate = RoundTo(GUI.HorizontalSlider(SliderRect[2],axles.frontAxle.bumpRate, 0, 12000),1);
		//rebound
		GUI.Label(NameLabelRect[3], "Front Rebound Damping: " + RoundTo(axles.frontAxle.reboundRate/1000,1) + " Ns/mm");
		axles.frontAxle.reboundRate = RoundTo(GUI.HorizontalSlider(SliderRect[3],axles.frontAxle.reboundRate, 0, 12000),1);
		//antiroll bar stiffness
		GUI.Label(NameLabelRect[4], "Front Antirollbar Stiffness: " +RoundTo(axles.frontAxle.antiRollBarRate/1000,1) + " N/mm");
		axles.frontAxle.antiRollBarRate = RoundTo(GUI.HorizontalSlider(SliderRect[4],axles.frontAxle.antiRollBarRate, 0, 100000),1);
		//camber
		GUI.Label(NameLabelRect[5], "Front Camber: " +RoundTo(axles.frontAxle.camber,0) + "°");
		axles.frontAxle.camber = RoundTo(GUI.HorizontalSlider(SliderRect[5],axles.frontAxle.camber, -10, 10),0);
		
		// --------> REAR Suspension
		//Suspension travel
		GUI.Label(NameLabelRect[10], "Rear Suspension Travel: " + axles.rearAxle.suspensionTravel + " m");
		axles.rearAxle.suspensionTravel = RoundTo(GUI.HorizontalSlider(SliderRect[10],axles.rearAxle.suspensionTravel, 0.1f, 0.5f),2);
		//spring
		GUI.Label(NameLabelRect[11], "Rear Suspension Stiffness: " + RoundTo(axles.rearAxle.suspensionRate/1000,1) + " N/mm");
		axles.rearAxle.suspensionRate = RoundTo(GUI.HorizontalSlider(SliderRect[11],axles.rearAxle.suspensionRate, 0, 150000),1);
		//bump
		GUI.Label(NameLabelRect[12], "Rear Bump Damping: " +RoundTo(axles.rearAxle.bumpRate/1000,1) + " Ns/mm");
		axles.rearAxle.bumpRate = RoundTo(GUI.HorizontalSlider(SliderRect[12],axles.rearAxle.bumpRate,  0, 12000),1);
		//rebound
		GUI.Label(NameLabelRect[13], "Rear Rebound Damping: " + RoundTo(axles.rearAxle.reboundRate/1000,1) + " Ns/mm");
		axles.rearAxle.reboundRate = RoundTo(GUI.HorizontalSlider(SliderRect[13],axles.rearAxle.reboundRate, 0, 12000),1);
		//antiroll bar stiffness
		GUI.Label(NameLabelRect[14], "Rear Antirollbar Stiffness: " +RoundTo(axles.rearAxle.antiRollBarRate/1000,1) + " N/mm");
		axles.rearAxle.antiRollBarRate = RoundTo(GUI.HorizontalSlider(SliderRect[14],axles.rearAxle.antiRollBarRate, 0, 100000),1);
		//camber
		GUI.Label(NameLabelRect[15], "Rear Camber: " +RoundTo(axles.rearAxle.camber,0) + "°");
		axles.rearAxle.camber = RoundTo(GUI.HorizontalSlider(SliderRect[15],axles.rearAxle.camber, -10, 10),0);
		
		// ONE VALUE FOR FRONT/REAR
		//fast bump factor
		GUI.Label(NameLabelRect[20], "Front Fast Bump Factor: " + axles.frontAxle.fastBumpFactor);
		axles.frontAxle.fastBumpFactor = RoundTo(GUI.HorizontalSlider(SliderRect[20],axles.frontAxle.fastBumpFactor, 0.0f, 1.0f),1);
		//fast rebound factor
		GUI.Label(NameLabelRect[21], "Front Fast Rebound Factor: " + axles.frontAxle.fastReboundFactor);
		axles.frontAxle.fastReboundFactor = RoundTo(GUI.HorizontalSlider(SliderRect[21],axles.frontAxle.fastReboundFactor, 0.0f, 1.0f),1);
		//maximum steering lock front
		GUI.Label(NameLabelRect[22], "Front Wheels Maximum Steering Angle: " + RoundTo(axles.frontAxle.maxSteeringAngle,0) + "°");
		axles.frontAxle.maxSteeringAngle = RoundTo(GUI.HorizontalSlider(SliderRect[22],axles.frontAxle.maxSteeringAngle, 0f, 45f),0);
		//maximum steering lock rear
		GUI.Label(NameLabelRect[23], "Rear Wheels Maximum Steering Angle: " + RoundTo(axles.rearAxle.maxSteeringAngle,0) + "°");
		axles.rearAxle.maxSteeringAngle = RoundTo(GUI.HorizontalSlider(SliderRect[23],axles.rearAxle.maxSteeringAngle, 0f, 45f),0);
		
		if (GUI.changed) {
			carDynamics.SetWheelsParams();
			//float suspensionTravel=(carDynamics.suspensionTravelFront + carDynamics.suspensionTravelRear)/2;
			//if (suspensionTravel>carDynamics.suspensionTravel) carDynamics.ylocalPosition=carDynamics.ylocalPosition_orig - (suspensionTravel - carDynamics.suspensionTravel);
			//else carDynamics.ylocalPosition=carDynamics.ylocalPosition_orig;
		}
	}
	
	//brakes parameters
	void Brakes() {
		//front brakes power
		GUI.Label(NameLabelRect[0], "Front Maximum Brake Torque: "+ axles.frontAxle.brakeFrictionTorque + " Nm");
		axles.frontAxle.brakeFrictionTorque = RoundTo(GUI.HorizontalSlider(SliderRect[0],axles.frontAxle.brakeFrictionTorque, 0, 10000),0);
		//rear brakes power
		GUI.Label(NameLabelRect[1], "Rear Maximum Brake Torque: "+ axles.rearAxle.brakeFrictionTorque + " Nm");
		axles.rearAxle.brakeFrictionTorque = RoundTo(GUI.HorizontalSlider(SliderRect[1],axles.rearAxle.brakeFrictionTorque, 0, 10000),0);
		//brake balance
		GUI.Label(NameLabelRect[2], "Brake Balance (front: " + carDynamics.frontRearBrakeBalance*100 +"%" + " rear: " + (1-carDynamics.frontRearBrakeBalance)*100 +"%)");
		carDynamics.frontRearBrakeBalance = RoundTo(GUI.HorizontalSlider(SliderRect[2],carDynamics.frontRearBrakeBalance, 0f, 1f),2);

		//front handbrakes power
		GUI.Label(NameLabelRect[3], "Front Maximum Handbrake Torque: "+ axles.frontAxle.handbrakeFrictionTorque + " Nm");
		axles.frontAxle.handbrakeFrictionTorque = RoundTo(GUI.HorizontalSlider(SliderRect[3],axles.frontAxle.handbrakeFrictionTorque, 0, 10000),0);
		//rear handbrakes power
		GUI.Label(NameLabelRect[4], "Rear Maximum Handbrake Torque: "+ axles.rearAxle.handbrakeFrictionTorque + " Nm");
		axles.rearAxle.handbrakeFrictionTorque = RoundTo(GUI.HorizontalSlider(SliderRect[4],axles.rearAxle.handbrakeFrictionTorque, 0, 10000),0);
		//handbrake balance
		GUI.Label(NameLabelRect[5], "Handbrake Balance (front: " + carDynamics.frontRearHandBrakeBalance*100 +"%" + " rear: " + (1-carDynamics.frontRearHandBrakeBalance)*100 +"%)");
		carDynamics.frontRearHandBrakeBalance = RoundTo(GUI.HorizontalSlider(SliderRect[5],carDynamics.frontRearHandBrakeBalance, 0f, 1f),2);
	}
	
	//Tires settings
	void Tires() {
		//front tire forward grip factor
		GUI.Label(NameLabelRect[0], "Front Forward Grip: " + axles.frontAxle.forwardGripFactor);
		axles.frontAxle.forwardGripFactor = RoundTo(GUI.HorizontalSlider(SliderRect[0],axles.frontAxle.forwardGripFactor, 0.1f, 2f),1);			
		//front tire sideways grip factor
		GUI.Label(NameLabelRect[1], "Front Sideways Grip: " + axles.frontAxle.sidewaysGripFactor);
		axles.frontAxle.sidewaysGripFactor = RoundTo(GUI.HorizontalSlider(SliderRect[1],axles.frontAxle.sidewaysGripFactor, 0.1f, 2f),1);			
		//rear tire forward grip factor
		GUI.Label(NameLabelRect[2], "Rear Forward Grip: " + axles.rearAxle.forwardGripFactor);
		axles.rearAxle.forwardGripFactor = RoundTo(GUI.HorizontalSlider(SliderRect[2],axles.rearAxle.forwardGripFactor, 0.1f, 2f),1);			
		//rear tire sideways grip factor
		GUI.Label(NameLabelRect[3], "Rear Sideways Grip: " + axles.rearAxle.sidewaysGripFactor);
		axles.rearAxle.sidewaysGripFactor = RoundTo(GUI.HorizontalSlider(SliderRect[3],axles.rearAxle.sidewaysGripFactor, 0.1f, 2f),1);			

		if (Time.fixedDeltaTime>0.02f){
			GUI.enabled=false;
			//front tire pressure
			GUI.Label(NameLabelRect[4], "[Tire Pressure Calculation Disabled. Decrease TimeStep to enable it]");
			axles.frontAxle.tiresPressure = RoundTo(GUI.HorizontalSlider(SliderRect[4],axles.frontAxle.tiresPressure, 0, 400.5f),2);
			//rear tire pressure
			GUI.Label(NameLabelRect[5], "[Tire Pressure Calculation Disabled. Decrease TimeStep to enable it]");			
			axles.rearAxle.tiresPressure = RoundTo(GUI.HorizontalSlider(SliderRect[5],axles.rearAxle.tiresPressure, 0, 400.5f),2);			
		}
		else{
			//front tire pressure
			GUI.Label(NameLabelRect[4], "Front Tires Pressure: " + RoundTo(axles.frontAxle.tiresPressure,0) + " kPa (" + RoundTo(axles.frontAxle.tiresPressure/101.325f, 1) + " atm)");
			axles.frontAxle.tiresPressure = RoundTo(GUI.HorizontalSlider(SliderRect[4],axles.frontAxle.tiresPressure, 0, 400.5f),2);
			//rear tire pressure
			GUI.Label(NameLabelRect[5], "Rear Tires Pressure: " + RoundTo(axles.rearAxle.tiresPressure,0) + " kPa (" + RoundTo(axles.rearAxle.tiresPressure/101.325f, 1) + " atm)");
			axles.rearAxle.tiresPressure = RoundTo(GUI.HorizontalSlider(SliderRect[5],axles.rearAxle.tiresPressure, 0, 400.5f),2);
		}
		GUI.enabled=true;
		GUI.Label(NameLabelRect[10], "Tires Type Front:");
		Rect SliderRectCustomFront=new Rect(SliderRect[10].x,SliderRect[10].y,SliderRect[10].width,SliderRect[10].height+150);
		tiresTypeFront = GUI.SelectionGrid (SliderRectCustomFront, tiresTypeFront, tireTypes, 2);
		
		GUI.Label(NameLabelRect[14], "Tires Type Rear:");
		Rect SliderRectCustomRear=new Rect(SliderRect[14].x,SliderRect[14].y,SliderRect[14].width,SliderRect[14].height+150);
		tiresTypeRear = GUI.SelectionGrid (SliderRectCustomRear, tiresTypeRear, tireTypes, 2);

		if (GUI.changed){
			foreach(Wheel w in axles.allWheels) {
				if (Time.fixedDeltaTime>0.02f) w.tirePressureEnabled=false;
				else w.tirePressureEnabled=true;
			}
			carDynamics.SetWheelsParams();
			SetTiresType(tiresTypeFront, axles.frontAxle);
			SetTiresType(tiresTypeRear, axles.rearAxle);
			carDynamics.SetTiresType();
		}
	}
	
	//body settings
	void Body() {
		//mass from 500 to 10000kg
		GUI.Label(NameLabelRect[0], "Vehicle Weight: " + mrigidbody.mass + " Kg");
		mrigidbody.mass = GUI.HorizontalSlider(SliderRect[0], RoundTo(mrigidbody.mass,0),  500, 10000);
		//COG position + Weight(F:R)
		GUI.Label(NameLabelRect[1], "Weight Repartition (Front:Rear) " + carDynamics.frontRearWeightRepartition*100 + ":" +(100 -carDynamics.frontRearWeightRepartition*100));
		carDynamics.zlocalPosition = GUI.HorizontalSlider(SliderRect[1],carDynamics.zlocalPosition, zlocalPositionLimit, -zlocalPositionLimit);
		
		if (aerodynamicResistance!=null){
			//Cx
			GUI.Label(NameLabelRect[2],"Drag Coefficient, Cx: " + aerodynamicResistance.Cx);
			aerodynamicResistance.Cx = RoundTo(GUI.HorizontalSlider(SliderRect[2], aerodynamicResistance.Cx,  0.01f, 1.0f),2);
			//Area
			GUI.Label(NameLabelRect[3],"Drag Area, CxA: " + aerodynamicResistance.Area + " square meters");
			aerodynamicResistance.Area = RoundTo(GUI.HorizontalSlider(SliderRect[3], aerodynamicResistance.Area,  0.001f, 20.0f),3);
		}
		
		if (fuelTanks.Length!=0){
			i=0;
			for(int a = 0; a < fuelTanks.Length; a++) {
				GUI.Label(NameLabelRect[20+i],"Tank"+(a+1)+" Capacity: " + fuelTanks[a].tankCapacity + " liters");
				fuelTanks[a].tankCapacity = RoundTo(GUI.HorizontalSlider(SliderRect[20+i], fuelTanks[a].tankCapacity,  0f, 800f),0);

				GUI.Label(NameLabelRect[21+i],"Tank"+(a+1)+" Current Fuel: " + fuelTanks[a].currentFuel + " liters");
				currentFuels[a] = RoundTo(GUI.HorizontalSlider(SliderRect[21+i], fuelTanks[a].currentFuel,  0f, fuelTanks[a].tankCapacity),0);
								
				i=i+2;
			}
		
			GUI.Label(NameLabelRect[10],"Fuel Consumption At "+drivetrain.fuelConsumptionSpeed + " km/h: " + drivetrain.fuelConsumptionAtCostantSpeed + " liters per 100 kms");
			drivetrain.fuelConsumptionAtCostantSpeed = RoundTo(GUI.HorizontalSlider(SliderRect[10], drivetrain.fuelConsumptionAtCostantSpeed,  0f, 50),1);

			GUI.Label(NameLabelRect[11],"Fuel Consumption Speed: " + drivetrain.fuelConsumptionSpeed + " km/h");
			drivetrain.fuelConsumptionSpeed = RoundTo(GUI.HorizontalSlider(SliderRect[11], drivetrain.fuelConsumptionSpeed,  1f, 200),0);
			
			GUI.Label(NameLabelRect[12],"Current Consumption: " + RoundTo(drivetrain.currentConsumption,2) + " liters per 100 kms");
			
		}

		if (GUI.changed){			
			carDynamics.FixPhysX();
			SetCOGPosition(carDynamics.zlocalPosition);
			for(int a = 0; a < fuelTanks.Length; a++) {
				fuelTanks[a].currentFuel=currentFuels[a];
			}
			drivetrain.RPMAtSpeedInLastGear=drivetrain.CalcRPMAtSpeedInLastGear(drivetrain.fuelConsumptionSpeed);
			
			// to avoid wheel oscillations due to high car mass 
			float factor=mrigidbody.mass/mass;
			if (factor>1){
				foreach(Wheel w in axles.allWheels){
					w.mass=w.originalMass*factor;
					w.rotationalInertia=(w.mass/2)*w.radius*w.radius;
				}
			}
		}		
	}
	
	//assistance system settings
	void Assistance() {			
		//ABS
		carController.ABS = GUI.Toggle(new Rect(NameLabelRect[0].x,NameLabelRect[0].y,NameLabelRect[0].width-100, NameLabelRect[7].height), carController.ABS, "ABS (AntiLock Braking)");
		//TCS
		carController.TCS = GUI.Toggle(NameLabelRect[1], carController.TCS, "TCS (Traction Control)");
		//ESP
		carController.ESP = GUI.Toggle(NameLabelRect[2], carController.ESP, "ESP (Stability Control)");
		//Steering assistance
		carController.steerAssistance = GUI.Toggle(NameLabelRect[3], carController.steerAssistance, "Steer Assistance");
		//is gearbox automatic?
		drivetrain.automatic = GUI.Toggle(NameLabelRect[4], drivetrain.automatic, "Automatic Transmission");
		//is clutch automatic?
		drivetrain.autoClutch = GUI.Toggle(NameLabelRect[5], drivetrain.autoClutch, "Automatic Clutch");
		//is reverse automatic?
		drivetrain.autoReverse = GUI.Toggle(NameLabelRect[6], drivetrain.autoReverse, "Automatic Reverse");
		//mouse controller
		mouseController=GUI.Toggle(NameLabelRect[7], carDynamics.controller == CarDynamics.Controller.mouse, "Mouse Controller");

		//Arcade mode
		if (arcader!=null) {
			arcader.enabled = GUI.Toggle(NameLabelRect[20], arcader.enabled, "Arcade Mode");
			if (arcader.enabled==true){

				GUI.Label(NameLabelRect[21],"Minimum Velocity: " + arcader.minVelocity + " km/h");
				arcader.minVelocity = RoundTo(GUI.HorizontalSlider(SliderRect[21], arcader.minVelocity,  0f, 50),0);
				
				GUI.Label(NameLabelRect[22],"Overall Strength: " + arcader.overallStrength);
				arcader.overallStrength = RoundTo(GUI.HorizontalSlider(SliderRect[22], arcader.overallStrength,  0f, 1),1);
				
				GUI.Label(NameLabelRect[23],"COG Helper Strength: " + arcader.COGHelperStrength);
				arcader.COGHelperStrength = RoundTo(GUI.HorizontalSlider(SliderRect[23], arcader.COGHelperStrength,  0f, 2),1);
				
				GUI.Label(NameLabelRect[24],"Torque Helper Strength: " + arcader.torqueHelperStrength);
				arcader.torqueHelperStrength = RoundTo(GUI.HorizontalSlider(SliderRect[24], arcader.torqueHelperStrength,  0f, 2),1);
				
				GUI.Label(NameLabelRect[25],"Grip Helper Strength: " + arcader.gripHelperStrength);
				arcader.gripHelperStrength = RoundTo(GUI.HorizontalSlider(SliderRect[25], arcader.gripHelperStrength,  0f, 2),1);			
			}
		}
		
		//3d tire (disabled temporary)
		//carDynamics.tridimensionalTire = GUI.Toggle(NameLabelRect[21], carDynamics.tridimensionalTire, "Tridimensional Tire");
			
		//throttle time
		GUI.Label(NameLabelRect[10], "Time to fully engage throttle: "+carController.throttleTime +" secs");
		carController.throttleTime = RoundTo(GUI.HorizontalSlider(SliderRect[10], carController.throttleTime, 0.001f, 1.0f), 3);
		//throttle release time
		GUI.Label(NameLabelRect[11], "Time to fully release throttle: "+carController.throttleReleaseTime +" secs");
		carController.throttleReleaseTime = RoundTo(GUI.HorizontalSlider(SliderRect[11],carController.throttleReleaseTime, 0.001f, 1.0f), 3);
		//brakes time
		GUI.Label(NameLabelRect[12], "Time to fully engage brakes: "+carController.brakesTime +" secs");
		carController.brakesTime = RoundTo(GUI.HorizontalSlider(SliderRect[12],carController.brakesTime, 0.001f, 1.0f), 3);
		//brakes release time
		GUI.Label(NameLabelRect[13], "Time to fully release brakes: "+carController.brakesReleaseTime +" secs");
		carController.brakesReleaseTime = RoundTo(GUI.HorizontalSlider(SliderRect[13],carController.brakesReleaseTime, 0.001f, 1.0f), 3);
		//steer time
		GUI.Label(NameLabelRect[14], "Time to fully turn steering wheel: "+carController.steerTime +" secs");
		carController.steerTime = RoundTo(GUI.HorizontalSlider(SliderRect[14],carController.steerTime, 0.01f, 1.0f), 2);
		//steer release time
		GUI.Label(NameLabelRect[15], "Time to fully release steering wheel: "+carController.steerReleaseTime +" secs");
		carController.steerReleaseTime = RoundTo(GUI.HorizontalSlider(SliderRect[15],carController.steerReleaseTime, 0.01f, 1.0f), 2);
		
		GUI.Label(NameLabelRect[16], "Fixed TimeStep: "+Time.fixedDeltaTime +" secs ("+(1/Time.fixedDeltaTime)+" Hz)");
		Time.fixedDeltaTime = RoundTo(GUI.HorizontalSlider(SliderRect[16],Time.fixedDeltaTime, 0.01f, 0.05f), 2);
		
		if (GUI.changed){
			foreach(Wheel w in axles.allWheels) {
				if (Time.fixedDeltaTime>0.02f) w.tirePressureEnabled=false;
				else w.tirePressureEnabled=true;
			}
			carDynamics.SetWheelsParams();
			if (arcader!=null) {
				if (arcader.enabled==true) carController.ESP=false;
				//else if (carController.ESP!=ESP) carController.ESP=ESP;
			}
			
			if (lastMouseController != mouseController){
				if(mouseController) carDynamics.controller = CarDynamics.Controller.mouse;
				else carDynamics.controller = CarDynamics.Controller.axis;
				if(carDynamics.carController!=null) {
					carDynamics.SetController(carDynamics.controller.ToString());
					carController=carDynamics.carController;
				}
			}
		}
		lastMouseController=mouseController;		
	}
	
	void RectCalculation(Vector2 Size) {
		ToggleRect = new Rect(8.0f, 0.0f, 200.0f, 20.0f);
		
		SettingsWindowRect = new Rect(10.0f, 5.0f, Size.x - 20.0f, Size.y - 20.0f);
		GridRect = new Rect(10.0f, 20.0f, SettingsWindowRect.width - 20.0f, 20.0f);
	
		//size of each controls is 20 pixels, minimal offset is 5 pixels
		float LeftOffset =((SettingsWindowRect.height - 50.0f)/20.0f)/5.0f;
		float RightOffset = LeftOffset;
		for(int a = 0; a < 10; a++) {
		NameLabelRect[a] = new Rect(10.0f, 50.0f+(a*(50.0f+LeftOffset)), SettingsWindowRect.width/3.0f +100f, 22f);
		SliderRect[a] = new Rect(NameLabelRect[a].x, NameLabelRect[a].y+20.0f, SettingsWindowRect.width/3.0f - 10.0f, NameLabelRect[a].height);
		}
		
		for(int b = 0; b < 10; b++) {
		NameLabelRect[10+b] = new Rect(SettingsWindowRect.width/3.0f * 2.0f, 50.0f+(b*(50.0f+RightOffset)), SettingsWindowRect.width/3.0f + 100f, 22f);
		SliderRect[10+b] = new Rect(NameLabelRect[10+b].x, NameLabelRect[10+b].y+20.0f, SettingsWindowRect.width/3.0f - 10.0f, NameLabelRect[b].height);
		}
	
		for(int c = 0; c < 10; c++) {
		NameLabelRect[20+c] = new Rect(SettingsWindowRect.width/3.0f , 50.0f+(c*(50.0f+RightOffset)), SettingsWindowRect.width/3.0f + 100f, 22f);
		SliderRect[20+c] = new Rect(NameLabelRect[20+c].x, NameLabelRect[20+c].y+20.0f, SettingsWindowRect.width/3.0f - 10.0f, NameLabelRect[c].height);	
		}
		
		ScrollViewRect = new Rect(SettingsWindowRect.width/3.0f * 2.0f, 50.0f+(3*(50.0f+RightOffset)), SettingsWindowRect.width/3.0f - 10.0f, SettingsWindowRect.height-SliderRect[23].y);
		//ScrollRect = new Rect(0, 0, ScrollViewRect.width, 50.0f+((drivetrain.gearRatios.Length-2)*(50.0f+RightOffset)));
	}
		
	//calculations for scroll view of gears
	void ScrollRectCalculation(Vector2 Size, int AmountOfGears) {
		
		ScrollLabelsRect = new Rect[AmountOfGears];
		ScrollSlidersRect = new Rect[AmountOfGears];
		
		for(int a = 0; a < AmountOfGears; a++) {
		ScrollLabelsRect[a] = new Rect(NameLabelRect[0].x, NameLabelRect[0].y-50 +(50 * a) , NameLabelRect[0].width-50, NameLabelRect[0].height);
		ScrollSlidersRect[a] = new Rect(SliderRect[0].x, SliderRect[0].y-50 +(50 *a), SliderRect[0].width-50, SliderRect[0].height);
		}
		ScrollRect = new Rect(0, 0, ScrollViewRect.width-50, 50.0f+(AmountOfGears*50.0f));
	}	

	void SetCOGPosition(float zlocalPosition){
		carDynamics.centerOfMass.localPosition=new Vector3(carDynamics.centerOfMass.localPosition.x, carDynamics.ylocalPosition, zlocalPosition);
		mrigidbody.centerOfMass = carDynamics.centerOfMass.localPosition;	
	}
	
	void SetTiresType(int tiresType, Axle axle){
		switch (tiresType){
			case 0:
				axle.tires=CarDynamics.Tires.competition_front;
				break;
			case 1:
				axle.tires=CarDynamics.Tires.competition_rear;
				break;
			case 2:
				axle.tires=CarDynamics.Tires.supersport_front;
				break;
			case 3:
				axle.tires=CarDynamics.Tires.supersport_rear;
				break;
			case 4:
				axle.tires=CarDynamics.Tires.sport_front;
				break;
			case 5:
				axle.tires=CarDynamics.Tires.sport_rear;
				break;
			case 6:
				axle.tires=CarDynamics.Tires.touring_front;
				break;
			case 7:
				axle.tires=CarDynamics.Tires.touring_rear;
				break;
			case 8:
				axle.tires=CarDynamics.Tires.offroad_front;
				break;
			case 9:
				axle.tires=CarDynamics.Tires.offroad_rear;
				break;
			case 10:
				axle.tires=CarDynamics.Tires.truck_front;
				break;
			case 11:
				axle.tires=CarDynamics.Tires.truck_rear;
				break;
		}
	}
	
	void SetTransmission(){
		switch (transmissionType){
			case 0:
				drivetrain.transmission=Drivetrain.Transmissions.RWD;
				break;
			case 1:
				drivetrain.transmission=Drivetrain.Transmissions.FWD;
				break;
			case 2:
				drivetrain.transmission=Drivetrain.Transmissions.AWD;
				break;
		}
		drivetrain.SetTransmission(drivetrain.transmission);
		oldTransmissionType=transmissionType;	
	}	

	float RoundTo(float value, int precision){
		int boh=1;
		for(int i =1; i<=precision; i++ ){
			boh=boh*10;
		}			
		return  Mathf.Round(value * boh) / boh; 
	}	
}

// http://www.unifycommunity.com/wiki/index.php?title=PopupList
// edited by DEtH_MoroZ for using strings array as entry
public class Popup {
	static int popupListHash = "PopupList".GetHashCode();
	
	public static bool List (Rect position, ref bool showList, ref int listEntry, string[] listContent) {
		int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
		bool done = false;
		switch (Event.current.GetTypeForControl(controlID)) {
				case EventType.mouseDown:
						if (position.Contains(Event.current.mousePosition)) {
								GUIUtility.hotControl = controlID;
								showList = true;
						}
						break;
				case EventType.mouseUp:
						if (showList) {
								done = true;
						}
						break;
		}
		
		GUI.Label(position, listContent[listEntry]);
		if (showList) {
				Rect listRect = new Rect(position.x, position.y, position.width, (position.height+20)*listContent.Length);
				GUI.Box(listRect, "");
				listEntry = GUI.SelectionGrid(listRect, listEntry, listContent, 1);
		}
		if (done) {
				showList = false;
		}
		return done;
	}
}