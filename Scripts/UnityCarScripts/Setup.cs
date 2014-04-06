//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//========================================================================================================================

using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

[ExecuteInEditMode]
public class Setup : MonoBehaviour {

	string setupFileText="";
	string[] dataLines;
	Drivetrain drivetrain;
	CarDynamics cardynamics;
	ForceFeedback forceFeedback;
	PhysicMaterials physicMaterials;
	Arcader arcader;
	AerodynamicResistance aerodynamicResistance;
	Axles axles;
	AxisCarController axisCarController;
	MouseCarController mouseCarController;
	MobileCarController mobileCarController;
	FuelTank[] fuelTanks;
	Wing[] wings;
	[HideInInspector]
	public bool	loadingSetup=false;
	[HideInInspector]
	public bool	savingSetup=false;
	public string filePath=""; // ="" mandatory to avoid null reference error (string variables are instantiated as null, not empty string)
	public bool usePersistentDataPath=false;
	string value;
	string message;
	StreamWriter writer;
	StreamReader reader;
	int index0, index1, index2, index3, index4, index5, i;
	
	void Awake(){
		loadingSetup=true;
		SetComponent();
	}
	
	void SetComponent(){
		drivetrain = GetComponent<Drivetrain>();
		cardynamics = GetComponent<CarDynamics>();
		forceFeedback= GetComponent<ForceFeedback>();
		physicMaterials =GetComponent<PhysicMaterials>();
		arcader= GetComponent<Arcader>();
		aerodynamicResistance= GetComponent<AerodynamicResistance>();
		axles= GetComponent<Axles>();
		axisCarController = GetComponent <AxisCarController>();
		mouseCarController = GetComponent <MouseCarController>();
		mobileCarController = GetComponent <MobileCarController>();		
		fuelTanks=GetComponentsInChildren<FuelTank>();
		wings=GetComponentsInChildren<Wing>();
	}
	
	void Start(){	
		if (filePath!=""){
			LoadSetup();
		}
		else loadingSetup=false;
	}
		
	public void SaveValue(string parameter,string value,string section){
		index1=setupFileText.IndexOf("["+section+"]");
		if (index1!=-1){ // section found
			index2=setupFileText.IndexOf('[',index1+1);
			if (index2!=-1) // next section found
				index2-=2;
			else // next section not found
				index2=setupFileText.Length;

			string pattern=string.Format(@"\b{0}\b", parameter);
			Regex reg = new Regex(@pattern);
			string txt=setupFileText.Substring(index1,index2-index1);
			index5=reg.Match(txt).Index;
			if (index5!=0){ // parameter found
				index2=setupFileText.IndexOf("=",index5+index1) + 1;
				index3=setupFileText.IndexOf('\n',index5+index1);
				index4=setupFileText.IndexOf('#',index5+index1);
				if (index4!=-1) index3=Mathf.Min(index3,index4);
				if (index3==-1) index3=txt.Length + index1; // end of the section
				setupFileText=setupFileText.Remove(index2 ,index3-index2);
				setupFileText=setupFileText.Insert(index2 ,value);
			}
			else{ // parameter not found
				setupFileText=setupFileText.Insert(index2,'\n'+parameter+"="+value);
			}
		}
		else{ // section not found
			string txt="";
			if (setupFileText.Length!=0) txt="\n\n";
			setupFileText=setupFileText.Insert(setupFileText.Length,txt+"["+section+"]");
			index2=setupFileText.Length;
			setupFileText=setupFileText.Insert(index2,'\n'+parameter+"="+value);
		}
	}
	
	public string LoadValue(string parameter,string section){
		string retValue="";
		index0=setupFileText.IndexOf("["+section+"]");
		if (index0!=-1){ // section found	
			string subString=setupFileText.Substring(index0);
			string pattern=string.Format(@"\b{0}\b", parameter);
			Regex reg = new Regex(@pattern);
			index1=reg.Match(subString).Index;
			if (index1!=0){ // parameter found
				index2=subString.Substring(index1).IndexOf('\n');
				if (index2==-1) index2=subString.Length - index1; // no "\n" found, we are at the end of the file
				index3=subString.Substring(index1,index2).IndexOf('#');
				if (index3!=-1) index2=index3; // ending "#" character found
				string txt=subString.Substring(index1-2,index2+2); //+-2 to be sure to include beginning "#" character
				index3=txt.IndexOf("#");
				if (index3==-1){ // beginning "#" character not found, we can load the value
					index3=txt.IndexOf("=");
					retValue=txt.Substring(index3 + 1);
				}
			}
		}
		return retValue;
	}
	
	bool LoadFromFile(string filePath, bool checkIfNotExist){
		try{
			if (File.Exists(filePath)){
				reader = new StreamReader(filePath);
				setupFileText=reader.ReadToEnd();
				reader.Close();
				return true;
			}
			else if (checkIfNotExist==true || filePath==""){			
				message="(file '" + filePath +"' couldn't be found)";
				return false;
			}
			else
				return true;
		}
		catch {
			return false;
		}
	}
		
	public bool SaveToFile(string filePath){
		try{
      if (!File.Exists(filePath)) Debug.Log("UnityCar: file '" + filePath + "' not found, creating it");
      writer = new StreamWriter(filePath);
			writer.Write(setupFileText);
			writer.Close();
			return true;
		}
		catch{
			return false;
		}
	}
	
	public bool SaveSetup()
  {
    Debug.LogWarning("isSaved2");
    bool saved=false;
		message="";
		SetComponent();
		string mfilePath=filePath;
		if (usePersistentDataPath==true) mfilePath=Application.persistentDataPath + "\\" + filePath;
		if (filePath!="")
    {
      Debug.LogWarning("isSaved1");
			if (LoadFromFile(mfilePath,false)==true)
      {
				setupFileText="";
				savingSetup=true;
				SaveBodyData();
				SaveEngineData();
				SaveTransmissionData();
				SaveSuspensionsData();
				SaveBrakesData();
				SaveTiresData();
				SaveWheelsData();
				SaveWingsData();
				SaveControllerTypeData();
				SaveControllersData();
				SavePhysicMaterialsData();
				SaveArcaderData();
				SaveFuelTanksData();
				SaveForceFeedBackData();
				saved=SaveToFile(mfilePath);
        Debug.LogWarning("isSaved");
				if (saved==true) Debug.Log("UnityCar: setup saved succesfully in the file '"+ mfilePath+"'");
			}
		}
		else{
			message="(file path empty)";
		}
		
		if (message!="") Debug.LogError("UnityCar: error during setup saving "+ message);
		
		savingSetup=false;
		return saved;
	}
	
	void SaveBodyData(){
		SaveValue("weight",rigidbody.mass+"","body");
		SaveValue("weightRepartition",cardynamics.frontRearWeightRepartition+"","body");
		if (cardynamics.centerOfMass!=null) SaveValue("centerOfMass",cardynamics.centerOfMass.localPosition+"","body");
		else SaveValue("centerOfMass",rigidbody.centerOfMass+"","body");
		SaveValue("inertiaFactor",cardynamics.inertiaFactor+"","body");
		if (aerodynamicResistance!=null) SaveValue("dragCoefficent",aerodynamicResistance.Cx+"","body");
		if (aerodynamicResistance!=null) SaveValue("dragArea",aerodynamicResistance.Area+"","body");
	}
	
	void SaveEngineData(){
		if (drivetrain!=null){
			SaveValue("maxPower",drivetrain.maxPower+"","engine");
			SaveValue("maxPowerRPM",drivetrain.maxPowerRPM+"","engine");
			SaveValue("maxTorque",drivetrain.maxTorque+"","engine");
			SaveValue("maxTorqueRPM",drivetrain.maxTorqueRPM+"","engine");
			SaveValue("minRPM",drivetrain.minRPM+"","engine");
			SaveValue("maxRPM",drivetrain.maxRPM+"","engine");
			SaveValue("revLimiter",drivetrain.revLimiter+"","engine");
			SaveValue("revLimiterTime",drivetrain.revLimiterTime+"","engine");
			SaveValue("engineInertia",drivetrain.engineInertia+"","engine");
			SaveValue("engineFrictionFactor",drivetrain.engineFrictionFactor+"","engine");
			SaveValue("engineOrientation",drivetrain.engineOrientation+"","engine");
			SaveValue("canStall",drivetrain.canStall+"","engine");
			SaveValue("fuelConsumptionAtCostantSpeed",drivetrain.fuelConsumptionAtCostantSpeed+"","engine");
			SaveValue("fuelConsumptionSpeed",drivetrain.fuelConsumptionSpeed+"","engine");
		}
	}
		
	void SaveTransmissionData(){
		if (drivetrain!=null){
			SaveValue("transmissionType",drivetrain.transmission+"","transmission");	
			SaveValue("finalDriveRatio",drivetrain.finalDriveRatio+"","transmission");
			SaveValue("drivetrainInertia",drivetrain.drivetrainInertia+"","transmission");
			SaveValue("differentialLockCoefficient",drivetrain.differentialLockCoefficient+"","transmission");
			SaveValue("shifter",drivetrain.shifter+"","transmission");
			SaveValue("automatic",drivetrain.automatic+"","transmission");
			SaveValue("autoReverse",drivetrain.autoReverse+"","transmission");
			SaveValue("shiftDownRPM",drivetrain.shiftDownRPM+"","transmission");
			SaveValue("shiftUpRPM",drivetrain.shiftUpRPM+"","transmission");
			SaveValue("shiftTime",drivetrain.shiftTime+"","transmission");
			SaveValue("clutchMaxTorque",drivetrain.clutchMaxTorque+"","transmission");
			SaveValue("autoClutch",drivetrain.autoClutch+"","transmission");
			SaveValue("engageRPM",drivetrain.engageRPM+"","transmission");
			SaveValue("disengageRPM",drivetrain.disengageRPM+"","transmission");
			SaveValue("gears",drivetrain.gearRatios.Length-2+""	,"transmission");
			if (drivetrain.gearRatios.Length>0) SaveValue("gear-ratio-r",drivetrain.gearRatios[0]+"","transmission");
			if (drivetrain.gearRatios.Length>2) SaveValue("gear-ratio-1",drivetrain.gearRatios[2]+"","transmission");
			if (drivetrain.gearRatios.Length>3) SaveValue("gear-ratio-2",drivetrain.gearRatios[3]+"","transmission");
			if (drivetrain.gearRatios.Length>4) SaveValue("gear-ratio-3",drivetrain.gearRatios[4]+"","transmission");
			if (drivetrain.gearRatios.Length>5) SaveValue("gear-ratio-4",drivetrain.gearRatios[5]+"","transmission");
			if (drivetrain.gearRatios.Length>6) SaveValue("gear-ratio-5",drivetrain.gearRatios[6]+"","transmission");
			if (drivetrain.gearRatios.Length>7) SaveValue("gear-ratio-6",drivetrain.gearRatios[7]+"","transmission");
			if (drivetrain.gearRatios.Length>8) SaveValue("gear-ratio-7",drivetrain.gearRatios[8]+"","transmission");
			if (drivetrain.gearRatios.Length>9) SaveValue("gear-ratio-8",drivetrain.gearRatios[9]+"","transmission");
			if (drivetrain.gearRatios.Length>10) SaveValue("gear-ratio-9",drivetrain.gearRatios[10]+"","transmission");
			if (drivetrain.gearRatios.Length>11) SaveValue("gear-ratio-10",drivetrain.gearRatios[11]+"","transmission");
			if (drivetrain.gearRatios.Length>12) SaveValue("gear-ratio-11",drivetrain.gearRatios[12]+"","transmission");
			if (drivetrain.gearRatios.Length>13) SaveValue("gear-ratio-12",drivetrain.gearRatios[13]+"","transmission");
			if (drivetrain.gearRatios.Length>14) SaveValue("gear-ratio-13",drivetrain.gearRatios[14]+"","transmission");
			if (drivetrain.gearRatios.Length>15) SaveValue("gear-ratio-14",drivetrain.gearRatios[15]+"","transmission");
			if (drivetrain.gearRatios.Length>16) SaveValue("gear-ratio-15",drivetrain.gearRatios[16]+"","transmission");
			if (drivetrain.gearRatios.Length>17) SaveValue("gear-ratio-16",drivetrain.gearRatios[17]+"","transmission");
			if (drivetrain.gearRatios.Length>18) SaveValue("gear-ratio-17",drivetrain.gearRatios[18]+"","transmission");
			if (drivetrain.gearRatios.Length>19) SaveValue("gear-ratio-18",drivetrain.gearRatios[19]+"","transmission");
			if (drivetrain.gearRatios.Length>20) SaveValue("gear-ratio-19",drivetrain.gearRatios[20]+"","transmission");
			if (drivetrain.gearRatios.Length>21) SaveValue("gear-ratio-20",drivetrain.gearRatios[21]+"","transmission");			
		}
	}
	
	void SaveSuspensionsData(){
		SaveValue("suspensionTravel", axles.frontAxle.suspensionTravel+"","suspensions-frontAxle");
		SaveValue("suspensionRate", axles.frontAxle.suspensionRate+"","suspensions-frontAxle");
		SaveValue("bumpRate", axles.frontAxle.bumpRate+"","suspensions-frontAxle");
		SaveValue("reboundRate", axles.frontAxle.reboundRate+"","suspensions-frontAxle");
		SaveValue("fastBumpFactor", axles.frontAxle.fastBumpFactor+"","suspensions-frontAxle");
		SaveValue("fastReboundFactor", axles.frontAxle.fastReboundFactor+"","suspensions-frontAxle");
		SaveValue("camber", axles.frontAxle.camber+"","suspensions-frontAxle");
		SaveValue("antiRollBarRate", axles.frontAxle.antiRollBarRate+"","suspensions-frontAxle");
		SaveValue("maxSteeringAngle", axles.frontAxle.maxSteeringAngle+"","suspensions-frontAxle");
		
		SaveValue("suspensionTravel", axles.rearAxle.suspensionTravel+"","suspensions-rearAxle");
		SaveValue("suspensionRate", axles.rearAxle.suspensionRate+"","suspensions-rearAxle");
		SaveValue("bumpRate", axles.rearAxle.bumpRate+"","suspensions-rearAxle");
		SaveValue("reboundRate", axles.rearAxle.reboundRate+"","suspensions-rearAxle");
		SaveValue("fastBumpFactor", axles.rearAxle.fastBumpFactor+"","suspensions-rearAxle");
		SaveValue("fastReboundFactor", axles.rearAxle.fastReboundFactor+"","suspensions-rearAxle");
		SaveValue("camber", axles.rearAxle.camber+"","suspensions-rearAxle");
		SaveValue("antiRollBarRate", axles.rearAxle.antiRollBarRate+"","suspensions-rearAxle");
		SaveValue("maxSteeringAngle", axles.rearAxle.maxSteeringAngle+"","suspensions-rearAxle");
		
		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			SaveValue("suspensionTravel", axle.suspensionTravel+"","suspensions-otherAxle"+i);
			SaveValue("suspensionRate", axle.suspensionRate+"","suspensions-otherAxle"+i);
			SaveValue("bumpRate", axle.bumpRate+"","suspensions-otherAxle"+i);
			SaveValue("reboundRate", axle.reboundRate+"","suspensions-otherAxle"+i);
			SaveValue("fastBumpFactor", axle.fastBumpFactor+"","suspensions-otherAxle"+i);
			SaveValue("fastReboundFactor", axle.fastReboundFactor+"","suspensions-otherAxle"+i);
			SaveValue("camber", axle.camber+"","suspensions-otherAxle"+i);
			SaveValue("antiRollBarRate", axle.antiRollBarRate+"","suspensions-otherAxle"+i);
			SaveValue("maxSteeringAngle", axle.maxSteeringAngle+"","suspensions-otherAxle"+i);
		}
	}
	
	void SaveBrakesData(){
		SaveValue("brakeFrictionTorque", axles.frontAxle.brakeFrictionTorque+"","brakes-frontAxle");
		SaveValue("handbrakeFrictionTorque", axles.frontAxle.handbrakeFrictionTorque+"","brakes-frontAxle");

		SaveValue("BrakeFrictionTorque", axles.rearAxle.brakeFrictionTorque+"","brakes-rearAxle");
		SaveValue("HandbrakeFrictionTorque", axles.rearAxle.handbrakeFrictionTorque+"","brakes-rearAxle");
		
		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			SaveValue("brakeFrictionTorque",axle.brakeFrictionTorque+"","brakes-otherAxle"+i);
			SaveValue("handbrakeFrictionTorque",axle.handbrakeFrictionTorque+"","brakes-otherAxle"+i);
		}
		
		SaveValue("frontRearBrakeBalance", cardynamics.frontRearBrakeBalance+"","brakes");
		SaveValue("frontRearHandBrakeBalance", cardynamics.frontRearHandBrakeBalance+"","brakes");
	}
	
	void SaveTiresData(){
		SaveValue("tireType",axles.frontAxle.tires+"","tires-frontAxle");
		SaveValue("forwardGripFactor",axles.frontAxle.forwardGripFactor+"","tires-frontAxle");
		SaveValue("sidewaysGripFactor",axles.frontAxle.sidewaysGripFactor+"","tires-frontAxle");
		SaveValue("tiresPressure",axles.frontAxle.tiresPressure+"","tires-frontAxle");
		SaveValue("optimalTiresPressure",axles.frontAxle.optimalTiresPressure+"","tires-frontAxle");

		SaveValue("tireType",axles.rearAxle.tires+"","tires-rearAxle");
		SaveValue("forwardGripFactor",axles.rearAxle.forwardGripFactor+"","tires-rearAxle");
		SaveValue("sidewaysGripFactor",axles.rearAxle.sidewaysGripFactor+"","tires-rearAxle");
		SaveValue("tiresPressure",axles.rearAxle.tiresPressure+"","tires-rearAxle");
		SaveValue("optimalTiresPressure",axles.rearAxle.optimalTiresPressure+"","tires-rearAxle");
		
		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			SaveValue("tireType",axle.tires+"","tires-otherAxle"+i);
			SaveValue("forwardGripFactor",axle.forwardGripFactor+"","tires-otherAxle"+i);
			SaveValue("sidewaysGripFactor",axle.sidewaysGripFactor+"","tires-otherAxle"+i);
			SaveValue("tiresPressure",axle.tiresPressure+"","tires-otherAxle"+i);
			SaveValue("optimalTiresPressure",axle.optimalTiresPressure+"","tires-otherAxle"+i);
		}
	}
	
	void SaveWheelsData(){
		if (axles.frontAxle.leftWheel!=null){
			SaveWheelData(axles.frontAxle.leftWheel,"frontAxle-left");
		}
		if (axles.frontAxle.rightWheel!=null){
			SaveWheelData(axles.frontAxle.rightWheel,"frontAxle-right");
		}
		if (axles.rearAxle.leftWheel!=null){
			SaveWheelData(axles.rearAxle.leftWheel,"rearAxle-left");
		}
		if (axles.rearAxle.rightWheel!=null){
			SaveWheelData(axles.rearAxle.rightWheel,"rearAxle-right");
		}
		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			if (axle.leftWheel!=null){
				SaveWheelData(axle.leftWheel,"otherAxle"+i+"-left");
			}
			if (axle.rightWheel!=null){
				SaveWheelData(axle.rightWheel,"otherAxle"+i+"-right");
			}
		}
 	}
	
	void SaveWheelData(Wheel w, string wheelPosition){
		SaveValue("mass",w.mass+"","wheels-"+wheelPosition);
		SaveValue("radius",w.radius+"","wheels-"+wheelPosition);
		SaveValue("rimRadius",w.rimRadius+"","wheels-"+wheelPosition);
		SaveValue("width",w.width+"","wheels-"+wheelPosition);
	}
	
	void SaveWingsData(){
		i=0;
		foreach(Wing wing in wings){
			i++;
			SaveValue("dragCoefficient", wing.dragCoefficient+"","wing"+i);
			SaveValue("angleOfAttack", wing.angleOfAttack+"","wing"+i);
			SaveValue("area", wing.area+"","wing"+i);
		}
	}	
	
	void SaveControllerTypeData(){
		SaveValue("controller", cardynamics.controller+"","controllerType");
	}
	
	void SaveControllersData(){
		if (axisCarController!=null){
			SaveControllerData(axisCarController,CarDynamics.Controller.axis);
		}
		if (mouseCarController!=null){
			SaveControllerData(mouseCarController,CarDynamics.Controller.mouse);
		}
		if (mobileCarController!=null){
			SaveControllerData(mobileCarController,CarDynamics.Controller.mobile);
		}
	}
	
	void SaveControllerData(CarController carController, CarDynamics.Controller controller){
		SaveValue("smoothInput", carController.smoothInput+"",controller+"Controller");
		SaveValue("throttleTime", carController.throttleTime +"",controller+"Controller");
		SaveValue("throttleReleaseTime", carController.throttleReleaseTime +"",controller+"Controller");
		SaveValue("maxThrottleInReverse", carController.maxThrottleInReverse+"",controller+"Controller");
		SaveValue("brakesTime", carController.brakesTime +"",controller+"Controller");
		SaveValue("brakesReleaseTime", carController.brakesReleaseTime +"",controller+"Controller");
		SaveValue("steerTime", carController.steerTime +"",controller+"Controller");
		SaveValue("steerReleaseTime", carController.steerReleaseTime +"",controller+"Controller");
		SaveValue("veloSteerTime", carController.veloSteerTime+"",controller+"Controller");
		SaveValue("veloSteerReleaseTime", carController.veloSteerReleaseTime +"",controller+"Controller");
		SaveValue("steerCorrectionFactor", carController.steerCorrectionFactor +"",controller+"Controller");
		SaveValue("steerAssistance", carController.steerAssistance+"",controller+"Controller");
		SaveValue("SteerAssistanceMinVelocity", carController.SteerAssistanceMinVelocity+"",controller+"Controller");
		SaveValue("TCS", carController.TCS +"",controller+"Controller");
		SaveValue("TCSThreshold", carController.TCSThreshold+"",controller+"Controller");
		SaveValue("TCSMinVelocity", carController.TCSMinVelocity+"",controller+"Controller");
		SaveValue("ABS", carController.ABS+"",controller+"Controller");
		SaveValue("ABSThreshold",carController.ABSThreshold+"",controller+"Controller");
		SaveValue("ABSMinVelocity",carController.ABSMinVelocity+"",controller+"Controller");
		SaveValue("ESP",carController.ESP+"",controller+"Controller");
		SaveValue("ESPStrength",carController.ESPStrength+"",controller+"Controller");
		SaveValue("ESPMinVelocity",carController.ESPMinVelocity+"",controller+"Controller");	
	}

	void SavePhysicMaterialsData(){
		if (physicMaterials!=null){
			SaveValue("trackGrip", physicMaterials.trackGrip+"","physicMaterials");
			SaveValue("trackRollingFriction", physicMaterials.trackRollingFriction+"","physicMaterials");
			SaveValue("trackStaticFriction", physicMaterials.trackStaticFriction+"","physicMaterials");
			SaveValue("grassGrip", physicMaterials.grassGrip+"","physicMaterials");
			SaveValue("grassRollingFriction", physicMaterials.grassRollingFriction+"","physicMaterials");
			SaveValue("grassStaticFriction", physicMaterials.grassStaticFriction+"","physicMaterials");
			SaveValue("sandGrip", physicMaterials.sandGrip+"","physicMaterials");
			SaveValue("sandRollingFriction", physicMaterials.sandRollingFriction+"","physicMaterials");
			SaveValue("sandStaticFriction", physicMaterials.sandStaticFriction+"","physicMaterials");
			SaveValue("offRoadGrip", physicMaterials.offRoadGrip+"","physicMaterials");
			SaveValue("offRoadRollingFriction", physicMaterials.offRoadRollingFriction+"","physicMaterials");
			SaveValue("offRoadStaticFriction", physicMaterials.offRoadStaticFriction+"","physicMaterials");
		}
	}	
	
	void SaveArcaderData(){
		if (arcader!=null){
			SaveValue("minVelocity", arcader.minVelocity+"","arcader");
			SaveValue("overallStrength", arcader.overallStrength+"","arcader");
			SaveValue("COGHelperStrength", arcader.COGHelperStrength+"","arcader");
			SaveValue("torqueHelperStrength", arcader.torqueHelperStrength+"","arcader");
			SaveValue("gripHelperStrength", arcader.gripHelperStrength+"","arcader");
			
		}
	}

	void SaveFuelTanksData(){
		i=0;
		foreach(FuelTank fuelTank in fuelTanks){
			i++;	
			SaveValue("tankCapacity", fuelTank.tankCapacity+"","fuelTank"+i);
			SaveValue("currentFuel", fuelTank.currentFuel+"","fuelTank"+i);
			SaveValue("tankWeight", fuelTank.tankWeight+"","fuelTank"+i);
			SaveValue("fuelDensity", fuelTank.fuelDensity+"","fuelTank"+i);
		}
	}

	void SaveForceFeedBackData(){
		if (forceFeedback!=null) {
			SaveValue("enabled", cardynamics.enableForceFeedback+"","forcefeedback");
			SaveValue("factor", forceFeedback.factor+"","forcefeedback");
			SaveValue("multiplier", forceFeedback.multiplier+"","forcefeedback");
			SaveValue("smoothingFactor", forceFeedback.smoothingFactor+"","forcefeedback");
			SaveValue("clampValue", forceFeedback.clampValue+"","forcefeedback");
			SaveValue("invertForceFeedback", forceFeedback.invertForceFeedback+"","forcefeedback");
		}
	}
	
	public void LoadSetup(){
		loadingSetup=true;
		message="";
		SetComponent();
		string mfilePath=filePath;
		if (usePersistentDataPath==true) mfilePath=Application.persistentDataPath + "\\" + filePath;
		if (filePath!=""){
			if (LoadFromFile(mfilePath,true)==true){
				if (setupFileText.Contains("[body]")) LoadBodyData();
				if (setupFileText.Contains("[engine]")) LoadEngineData();
				if (setupFileText.Contains("[transmission]")) LoadTransmissionData();
				LoadSuspensionsData();
				LoadBrakesData();
				LoadTiresData();
				LoadWheelsData();
				LoadWingsData();
				if (setupFileText.Contains("[controllerType]")) LoadControllerTypeData();
				LoadControllersData();
				if (setupFileText.Contains("[physicMaterials]")) LoadPhysicMaterialsData();
				if (setupFileText.Contains("[arcader]")) LoadArcaderData();
				LoadFuelTanksData();
				if (setupFileText.Contains("[forcefeedback]")) LoadForceFeedBackData();
				
				Debug.Log("UnityCar: setup loaded succesfully from the file '"+ mfilePath+"'");
			}
		}
		else{
			message="(file path empty)";
		}
		
		if (message!="") Debug.LogError("UnityCar: error during setup loading "+ message);
		
		loadingSetup=false;
	}
	
	void LoadBodyData(){
		value=LoadValue("weight","body"); if (value!="") rigidbody.mass=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("weightRepartition","body"); if (value!="") cardynamics.frontRearWeightRepartition=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("centerOfMass","body"); if (value!="") {
			value=value.Replace("(","").Replace(")","");
			float value0=float.Parse(value.Split(',')[0],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			float value1=float.Parse(value.Split(',')[1],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			float value2=float.Parse(value.Split(',')[2],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			Vector3 COGPosition=new Vector3(value0,value1,value2);
			cardynamics.SetCenterOfMass(COGPosition);
		}
		value=LoadValue("inertiaFactor","body"); if (value!="") cardynamics.inertiaFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		if (aerodynamicResistance!=null) {value=LoadValue("dragCoefficent","body"); if (value!="") aerodynamicResistance.Cx=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if (aerodynamicResistance!=null) {value=LoadValue("dragArea","body"); if (value!="") aerodynamicResistance.Area=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
	}	
			
	void LoadEngineData(){
		dataLines = setupFileText.Split('\n');
		string dataPairs="";
		drivetrain.engineTorqueFromFile=false;
		drivetrain.torqueRPMValuesLen=0;
		
		value=LoadValue("minRPM","engine"); if (value!="") drivetrain.minRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("maxRPM","engine"); if (value!="") drivetrain.maxRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);		
		
		//torque data loading
		foreach (string line in dataLines)
		{
			if (line.Contains("torque-curve")) dataPairs+= line.Split('=')[1] + "\n";
		}
		
		if (dataPairs!=""){
			dataPairs= dataPairs.Substring(0,dataPairs.Length-1);
			dataLines=dataPairs.Split('\n');
			drivetrain.torqueRPMValues = new float[dataLines.Length,2];

			int lineNum = 0;
			foreach (string line in dataLines)
			{
				drivetrain.torqueRPMValues[lineNum,0] = float.Parse(line.Split(',')[0],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
				drivetrain.torqueRPMValues[lineNum,1] = float.Parse(line.Split(',')[1],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
				lineNum++;
			}
			
			drivetrain.torqueRPMValuesLen=drivetrain.torqueRPMValues.GetLength(0);
			
			if (drivetrain.maxRPM==0)
			{
				for (i=drivetrain.torqueRPMValuesLen-1; i>=0; i--){
					if (drivetrain.torqueRPMValues[i,1]>0) {
						drivetrain.maxRPM=drivetrain.torqueRPMValues[i,0];
						break;
					}
				}
			}
			drivetrain.engineTorqueFromFile=drivetrain.torqueRPMValuesLen>0;
		}
		if (drivetrain.engineTorqueFromFile==false){ // these values are calculated from the torque file
			value=LoadValue("maxPower","engine"); if (value!="") drivetrain.maxPower=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("maxPowerRPM","engine"); if (value!="") drivetrain.maxPowerRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("maxTorque","engine"); if (value!="") drivetrain.maxTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("maxTorqueRPM","engine"); if (value!="") drivetrain.maxTorqueRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}				
		value=LoadValue("canStall","engine"); if (value!="") drivetrain.canStall=bool.Parse(value);
		value=LoadValue("revLimiter","engine"); if (value!="") drivetrain.revLimiter=bool.Parse(value);
		value=LoadValue("revLimiterTime","engine"); if (value!="") drivetrain.revLimiterTime=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("engineInertia","engine"); if (value!="") drivetrain.engineInertia=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("engineFrictionFactor","engine"); if (value!="") drivetrain.engineFrictionFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("engineOrientation","engine"); if (value!="") {
			value=value.Replace("(","").Replace(")","");
			float value0=float.Parse(value.Split(',')[0],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			float value1=float.Parse(value.Split(',')[1],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			float value2=float.Parse(value.Split(',')[2],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			drivetrain.engineOrientation=new Vector3(value0,value1,value2);
		}
		value=LoadValue("canStall","engine");	if (value!="") drivetrain.canStall=bool.Parse(value);
		value=LoadValue("fuelConsumptionAtCostantSpeed","engine"); if (value!="") drivetrain.fuelConsumptionAtCostantSpeed=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("fuelConsumptionSpeed","engine"); if (value!="") drivetrain.fuelConsumptionSpeed=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		
	}

	void LoadTransmissionData(){
		value=LoadValue("transmissionType","transmission"); if (value=="RWD") drivetrain.transmission=Drivetrain.Transmissions.RWD; else if (value=="FWD") drivetrain.transmission=Drivetrain.Transmissions.FWD; else if (value=="AWD") drivetrain.transmission=Drivetrain.Transmissions.AWD;
		value=LoadValue("gears","transmission"); if (value!="") drivetrain.gearRatios=new float[int.Parse(value)+2];
		if(drivetrain.gearRatios.Length>0) {value=LoadValue("gear-ratio-r","transmission"); if (value!="") drivetrain.gearRatios[0]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>2) {value=LoadValue("gear-ratio-1","transmission"); if (value!="") drivetrain.gearRatios[2]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>3) {value=LoadValue("gear-ratio-2","transmission"); if (value!="") drivetrain.gearRatios[3]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>4) {value=LoadValue("gear-ratio-3","transmission"); if (value!="") drivetrain.gearRatios[4]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>5) {value=LoadValue("gear-ratio-4","transmission"); if (value!="") drivetrain.gearRatios[5]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>6) {value=LoadValue("gear-ratio-5","transmission"); if (value!="") drivetrain.gearRatios[6]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>7) {value=LoadValue("gear-ratio-6","transmission"); if (value!="") drivetrain.gearRatios[7]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>8) {value=LoadValue("gear-ratio-7","transmission"); if (value!="") drivetrain.gearRatios[8]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>9) {value=LoadValue("gear-ratio-8","transmission"); if (value!="") drivetrain.gearRatios[9]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>10) {value=LoadValue("gear-ratio-9","transmission"); if (value!="") drivetrain.gearRatios[10]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>11) {value=LoadValue("gear-ratio-10","transmission"); if (value!="") drivetrain.gearRatios[11]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>12) {value=LoadValue("gear-ratio-11","transmission"); if (value!="") drivetrain.gearRatios[12]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>13) {value=LoadValue("gear-ratio-12","transmission"); if (value!="") drivetrain.gearRatios[13]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>14) {value=LoadValue("gear-ratio-13","transmission"); if (value!="") drivetrain.gearRatios[14]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>15) {value=LoadValue("gear-ratio-14","transmission"); if (value!="") drivetrain.gearRatios[15]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>16) {value=LoadValue("gear-ratio-15","transmission"); if (value!="") drivetrain.gearRatios[16]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>17) {value=LoadValue("gear-ratio-16","transmission"); if (value!="") drivetrain.gearRatios[17]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>18) {value=LoadValue("gear-ratio-17","transmission"); if (value!="") drivetrain.gearRatios[18]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>19) {value=LoadValue("gear-ratio-18","transmission"); if (value!="") drivetrain.gearRatios[19]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>20) {value=LoadValue("gear-ratio-19","transmission"); if (value!="") drivetrain.gearRatios[20]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		if(drivetrain.gearRatios.Length>21) {value=LoadValue("gear-ratio-20","transmission"); if (value!="") drivetrain.gearRatios[21]=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
		value=LoadValue("finalDriveRatio","transmission"); if (value!="") drivetrain.finalDriveRatio=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("drivetrainInertia","transmission"); if (value!="") drivetrain.drivetrainInertia=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);		
		value=LoadValue("differentialLockCoefficient","transmission"); if (value!="") drivetrain.differentialLockCoefficient=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);			
		value=LoadValue("shifter","transmission"); if (value!="") drivetrain.shifter=bool.Parse(value);
		value=LoadValue("automatic","transmission"); if (value!="") drivetrain.automatic=bool.Parse(value);
		value=LoadValue("autoReverse","transmission"); if (value!="") drivetrain.autoReverse=bool.Parse(value);
		value=LoadValue("shiftDownRPM","transmission"); if (value!="") drivetrain.shiftDownRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("shiftUpRPM","transmission"); if (value!="" && value!="0") drivetrain.shiftUpRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("shiftTime","transmission"); if (value!="") drivetrain.shiftTime=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("clutchMaxTorque","transmission"); if (value!="") drivetrain.clutchMaxTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("autoClutch","transmission"); if (value!="") drivetrain.autoClutch=bool.Parse(value);
		value=LoadValue("engageRPM","transmission"); if (value!="") drivetrain.engageRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("disengageRPM","transmission"); if (value!="") drivetrain.disengageRPM=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}

	void LoadSuspensionsData(){
		value=LoadValue("suspensionTravel","suspensions-frontAxle"); if (value!="") axles.frontAxle.suspensionTravel=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("suspensionRate","suspensions-frontAxle"); if (value!="") axles.frontAxle.suspensionRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("bumpRate","suspensions-frontAxle"); if (value!="") axles.frontAxle.bumpRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("reboundRate","suspensions-frontAxle"); if (value!="") axles.frontAxle.reboundRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("fastBumpFactor","suspensions-frontAxle"); if (value!="") axles.frontAxle.fastBumpFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("fastReboundFactor","suspensions-frontAxle"); if (value!="") axles.frontAxle.fastReboundFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("camber","suspensions-frontAxle"); if (value!="") axles.frontAxle.camber=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("antiRollBarRate","suspensions-frontAxle"); if (value!="") axles.frontAxle.antiRollBarRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("maxSteeringAngle","suspensions-frontAxle"); if (value!="") axles.frontAxle.maxSteeringAngle=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		
		value=LoadValue("suspensionTravel","suspensions-rearAxle"); if (value!="") axles.rearAxle.suspensionTravel=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("suspensionRate","suspensions-rearAxle"); if (value!="") axles.rearAxle.suspensionRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("bumpRate","suspensions-rearAxle"); if (value!="") axles.rearAxle.bumpRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("reboundRate","suspensions-rearAxle"); if (value!="") axles.rearAxle.reboundRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("fastBumpFactor","suspensions-rearAxle"); if (value!="") axles.rearAxle.fastBumpFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("fastReboundFactor","suspensions-rearAxle"); if (value!="") axles.rearAxle.fastReboundFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("camber","suspensions-rearAxle"); if (value!="") axles.rearAxle.camber=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("antiRollBarRate","suspensions-rearAxle"); if (value!="") axles.rearAxle.antiRollBarRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("maxSteeringAngle","suspensions-rearAxle"); if (value!="") axles.rearAxle.maxSteeringAngle=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;		
			value=LoadValue("suspensionTravel","suspensions-otherAxle"+i); if (value!="") axle.suspensionTravel=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("suspensionRate","suspensions-otherAxle"+i); if (value!="") axle.suspensionRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("bumpRate","suspensions-otherAxle"+i); if (value!="") axle.bumpRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("reboundRate","suspensions-otherAxle"+i); if (value!="") axle.reboundRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("fastBumpFactor","suspensions-otherAxle"+i); if (value!="") axle.fastBumpFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("fastReboundFactor","suspensions-otherAxle"+i); if (value!="") axle.fastReboundFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("camber","suspensions-otherAxle"+i); if (value!="") axle.camber=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("antiRollBarRate","suspensions-otherAxle"+i); if (value!="") axle.antiRollBarRate=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("maxSteeringAngle","suspensions-otherAxle"+i); if (value!="") axle.maxSteeringAngle=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}
	}
	
	void LoadBrakesData(){
		value=LoadValue("brakeFrictionTorque","brakes-frontAxle"); if (value!="") axles.frontAxle.brakeFrictionTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("handbrakeFrictionTorque","brakes-frontAxle"); if (value!="") axles.frontAxle.handbrakeFrictionTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		
		value=LoadValue("brakeFrictionTorque","brakes-rearAxle"); if (value!="") axles.rearAxle.brakeFrictionTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);		
		value=LoadValue("handbrakeFrictionTorque","brakes-rearAxle"); if (value!="") axles.rearAxle.handbrakeFrictionTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		
		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			value=LoadValue("brakeFrictionTorque","brakes-otherAxle"+i); if (value!="") axle.brakeFrictionTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("handbrakeFrictionTorque","brakesotherAxle"+i); if (value!="") axle.handbrakeFrictionTorque=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}

		value=LoadValue("frontRearBrakeBalance","brakes"); if (value!="") cardynamics.frontRearBrakeBalance=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("frontRearHandBrakeBalance","brakes"); if (value!="") cardynamics.frontRearHandBrakeBalance=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}		

	void LoadTiresData(){
		value=LoadValue("tireType","tires-frontAxle").ToLower(); if (value!="") LoadTireType(value, axles.frontAxle);
		value=LoadValue("forwardGripFactor","tires-frontAxle"); if (value!="") axles.frontAxle.forwardGripFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("sidewaysGripFactor","tires-frontAxle"); if (value!="") axles.frontAxle.sidewaysGripFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("tiresPressure","tires-frontAxle"); if (value!="") axles.frontAxle.tiresPressure=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("optimalTiresPressure","tires-frontAxle"); if (value!="") axles.frontAxle.optimalTiresPressure=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		
		value=LoadValue("tireType","tires-rearAxle").ToLower(); if (value!="") LoadTireType(value, axles.rearAxle);
		value=LoadValue("forwardGripFactor","tires-rearAxle"); if (value!="") axles.rearAxle.forwardGripFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("sidewaysGripFactor","tires-rearAxle"); if (value!="") axles.rearAxle.sidewaysGripFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("tiresPressure","tires-rearAxle"); if (value!="") axles.rearAxle.tiresPressure=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("optimalTiresPressure","tires-rearAxle"); if (value!="") axles.rearAxle.optimalTiresPressure=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			value=LoadValue("tireType","tires-otherAxle"+i).ToLower(); if (value!="") LoadTireType(value, axle);
			value=LoadValue("forwardGripFactor","tires-otherAxle"+i); if (value!="") axle.forwardGripFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("sidewaysGripFactor","tires-otherAxle"+i); if (value!="") axle.sidewaysGripFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("tiresPressure","tires-otherAxle"+i); if (value!="") axle.tiresPressure=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("optimalTiresPressure","tires-otherAxle"+i); if (value!="") axle.optimalTiresPressure=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}		
 	}
	
	void LoadTireType(string value, Axle axle){
				 if (value=="competition_front") axle.tires=CarDynamics.Tires.competition_front;
		else if (value=="competition_rear")  axle.tires=CarDynamics.Tires.competition_rear;
		else if (value=="supersport_front")  axle.tires=CarDynamics.Tires.supersport_front;
		else if (value=="supersport_rear")   axle.tires=CarDynamics.Tires.supersport_rear;
		else if (value=="sport_front")       axle.tires=CarDynamics.Tires.sport_front;
		else if (value=="sport_rear")        axle.tires=CarDynamics.Tires.sport_rear;
		else if (value=="touring_front")     axle.tires=CarDynamics.Tires.touring_front;
		else if (value=="touring_rear")      axle.tires=CarDynamics.Tires.touring_rear;
		else if (value=="offroad_front")     axle.tires=CarDynamics.Tires.offroad_front;
		else if (value=="offroad_rear")      axle.tires=CarDynamics.Tires.offroad_rear;
		else if (value=="truck_front")       axle.tires=CarDynamics.Tires.truck_front;
		else if (value=="truck_rear")        axle.tires=CarDynamics.Tires.truck_rear;
		else Debug.LogWarning("UnityCar: tire type \""+ value + "\" in setup file \""+ filePath +"\" doesn't exist");
	}
	
	void LoadWheelsData(){
		if (axles.frontAxle.leftWheel!=null){
			LoadWheelData(axles.frontAxle.leftWheel,"frontAxle-left");
		}
		if (axles.frontAxle.rightWheel!=null){
			LoadWheelData(axles.frontAxle.rightWheel,"frontAxle-right");
		}
		if (axles.rearAxle.leftWheel!=null){
			LoadWheelData(axles.rearAxle.leftWheel,"rearAxle-left");
		}
		if (axles.rearAxle.rightWheel!=null){
			LoadWheelData(axles.rearAxle.rightWheel,"rearAxle-right");
		}
		i=0;
		foreach(Axle axle in axles.otherAxles){
			i++;
			if (axle.leftWheel!=null){
				LoadWheelData(axle.leftWheel,"otherAxle"+i+"-left");
			}
			if (axle.rightWheel!=null){
				LoadWheelData(axle.rightWheel,"otherAxle"+i+"-right");
			}
		}
 	}	
	
	void LoadWheelData(Wheel w, string wheelPosition){
		value=LoadValue("mass","wheels-"+wheelPosition); if (value!="") w.mass=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("radius","wheels-"+wheelPosition); if (value!="") w.radius=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("rimRadius","wheels-"+wheelPosition); if (value!="") w.rimRadius=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("width","wheels-"+wheelPosition); if (value!="") w.width=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}	
	
	void LoadWingsData(){
		i=0;
		foreach(Wing wing in wings){
			i++;
			value=LoadValue("dragCoefficient", "wing"+i); if (value!="") wing.dragCoefficient=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("angleOfAttack","wing"+i); if (value!="") wing.angleOfAttack=int.Parse(value);
			value=LoadValue("area","wing"+i); if (value!="") wing.area=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}
	}		
	
	void LoadControllerTypeData(){
		value=LoadValue("controller","controllerType").ToLower();
		if (value=="axis")          cardynamics.controller=CarDynamics.Controller.axis;
		else if (value=="mouse")    cardynamics.controller=CarDynamics.Controller.mouse;
		else if (value=="mobile")   cardynamics.controller=CarDynamics.Controller.mobile;
		else if (value=="external") cardynamics.controller=CarDynamics.Controller.external;
		cardynamics.SetController(cardynamics.controller.ToString());
	}

	void LoadControllersData(){
		if (axisCarController!=null){
			LoadControllerData(axisCarController,CarDynamics.Controller.axis);
		}
		if (mouseCarController!=null){
			LoadControllerData(mouseCarController,CarDynamics.Controller.mouse);
		}
		if (mobileCarController!=null){
			LoadControllerData(mobileCarController,CarDynamics.Controller.mobile);
		}			
	}
	
	void LoadControllerData(CarController carController, CarDynamics.Controller controller){
		value=LoadValue("smoothInput",controller+"Controller"); if (value!="") carController.smoothInput=bool.Parse(value);
		value=LoadValue("throttleTime",controller+"Controller"); if (value!="") carController.throttleTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("throttleReleaseTime",controller+"Controller"); if (value!="") carController.throttleReleaseTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("maxThrottleInReverse",controller+"Controller"); if (value!="") carController.maxThrottleInReverse=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("brakesTime",controller+"Controller"); if (value!="") carController.brakesTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("brakesReleaseTime",controller+"Controller"); if (value!="") carController.brakesReleaseTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("steerTime",controller+"Controller"); if (value!="") carController.steerTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("steerReleaseTime",controller+"Controller"); if (value!="") carController.steerReleaseTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("veloSteerTime",controller+"Controller"); if (value!="") carController.veloSteerTime=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("veloSteerReleaseTime",controller+"Controller"); if (value!="") carController.veloSteerReleaseTime =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("steerCorrectionFactor",controller+"Controller"); if (value!="") carController.steerCorrectionFactor =float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("steerAssistance",controller+"Controller"); if (value!="") carController.steerAssistance=bool.Parse(value);
		value=LoadValue("SteerAssistanceMinVelocity",controller+"Controller"); if (value!="") carController.SteerAssistanceMinVelocity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("TCS",controller+"Controller"); if (value!="") carController.TCS=bool.Parse(value);
		value=LoadValue("TCSThreshold",controller+"Controller"); if (value!="") carController.TCSThreshold=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("TCSMinVelocity",controller+"Controller"); if (value!="") carController.TCSMinVelocity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("ABS",controller+"Controller"); if (value!="") carController.ABS=bool.Parse(value);
		value=LoadValue("ABSThreshold",controller+"Controller"); if (value!="") carController.ABSThreshold=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("ABSMinVelocity",controller+"Controller"); if (value!="") carController.ABSMinVelocity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("ESP",controller+"Controller"); if (value!="") carController.ESP=bool.Parse(value);
		value=LoadValue("ESPStrength",controller+"Controller"); if (value!="") carController.ESPStrength=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		value=LoadValue("ESPMinVelocity",controller+"Controller"); if (value!="") carController.ESPMinVelocity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);	
	}
	
	void LoadPhysicMaterialsData(){
		if (physicMaterials!=null){
			value=LoadValue("trackGrip", "physicMaterials"); if (value!="") physicMaterials.trackGrip=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("trackRollingFriction", "physicMaterials"); if (value!="") physicMaterials.trackRollingFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("trackStaticFriction", "physicMaterials"); if (value!="") physicMaterials.trackStaticFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("grassGrip", "physicMaterials"); if (value!="") physicMaterials.grassGrip=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("grassRollingFriction", "physicMaterials"); if (value!="") physicMaterials.grassRollingFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("grassStaticFriction", "physicMaterials"); if (value!="") physicMaterials.grassStaticFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("sandGrip", "physicMaterials"); if (value!="") physicMaterials.sandGrip=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("sandRollingFriction", "physicMaterials"); if (value!="") physicMaterials.sandRollingFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("sandStaticFriction", "physicMaterials"); if (value!="") physicMaterials.sandStaticFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("offRoadGrip", "physicMaterials"); if (value!="") physicMaterials.offRoadGrip=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("offRoadRollingFriction", "physicMaterials"); if (value!="") physicMaterials.offRoadRollingFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("offRoadStaticFriction", "physicMaterials"); if (value!="") physicMaterials.offRoadStaticFriction=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}
	}		

	void LoadArcaderData(){
		if (arcader!=null) {
			value=LoadValue("minVelocity", "arcader"); if (value!="") arcader.minVelocity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("overallStrength", "arcader"); if (value!="") arcader.overallStrength=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("COGHelperStrength", "arcader"); if (value!="") arcader.COGHelperStrength=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("torqueHelperStrength", "arcader"); if (value!="") arcader.torqueHelperStrength=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("gripHelperStrength", "arcader"); if (value!="") arcader.gripHelperStrength=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			
		}
	}
	
	void LoadFuelTanksData(){
		i=0;
		foreach(FuelTank fuelTank in fuelTanks){
			i++;
			value=LoadValue("tankCapacity","fuelTank"+i); if (value!="") fuelTank.tankCapacity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("currentFuel", "fuelTank"+i); if (value!="") fuelTank.currentFuel=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("tankWeight", "fuelTank"+i); if (value!="") fuelTank.tankWeight=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("fuelDensity","fuelTank"+i); if (value!="") fuelTank.fuelDensity=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		}
	}	
	
	void LoadForceFeedBackData(){
		if (forceFeedback!=null) {
			value=LoadValue("enabled", "forcefeedback"); if (value!="") cardynamics.enableForceFeedback=bool.Parse(value);
			value=LoadValue("factor", "forcefeedback"); if (value!="") forceFeedback.factor=int.Parse(value);
			value=LoadValue("multiplier", "forcefeedback"); if (value!="") forceFeedback.multiplier=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("smoothingFactor", "forcefeedback"); if (value!="") forceFeedback.smoothingFactor=float.Parse(value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			value=LoadValue("clampValue", "forcefeedback"); if (value!="") forceFeedback.clampValue=int.Parse(value);
			value=LoadValue("invertForcefeedback", "forcefeedback"); if (value!="") forceFeedback.invertForceFeedback=bool.Parse(value);
		}
	}
	
}