//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

[ExecuteInEditMode()] 
public class DashBoard : MonoBehaviour {

	public int depth=2;	
	public enum Docking {Left, Right};
	public enum Unit {Kmh, Mph};

	public Texture2D tachoMeter;
	[HideInInspector]
	public bool tachoMeterDisabled=false;
	public Vector2 tachoMeterPosition;
	public Docking tachoMeterDocking=Docking.Left;
	public float tachoMeterDimension=1;
	public Texture2D tachoMeterNeedle;
	public Vector2 tachoMeterNeedleSize;
  public float tachoMeterNeedleAngle;
	float actualtachoMeterNeedleAngle;
	public float RPMFactor=3.5714f;
	
	public Texture2D speedoMeter;
	[HideInInspector]
	public bool speedoMeterDisabled=false;
	public Vector2 speedoMeterPosition;
	public Docking speedoMeterDocking=Docking.Right;
	public float speedoMeterDimension=1;
	public Texture2D speedoMeterNeedle;
	public Vector2 speedoMeterNeedleSize;
	public float speedoMeterNeedleAngle;
	float actualspeedoMeterNeedleAngle;
	public float speedoMeterFactor;
	public enum SpeedoMeterType{RigidBody,Wheel};
	public SpeedoMeterType speedoMeterType;	
	
	public Unit digitalSpeedoUnit;
	public GUIStyle digitalSpeedoStyle;
	public Vector2 digitalSpeedoPosition;
	public Docking digitalSpeedoDocking;
		
	public GUIStyle gearMonitorStyle;
	public Vector2 gearMonitorPosition;
	public Docking gearMonitorDocking;

	public Texture2D clutchMonitor;
	public Texture2D throttleMonitor;
	public Texture2D brakeMonitor;
	public Vector2 pedalsMonitorPosition;
	
	public Texture2D ABS;
	public Texture2D TCS;
	public Texture2D ESP;
	public Vector2 dashboardLightsPosition;
	public Docking dashboardLightsDocking;
	public float dashboardLightsDimension=1;

	public GameObject digitalSpeedoOnBoard;
	public GameObject digitalGearOnBoard;
	public GameObject tachoMeterNeedleOnBoard;
	public GameObject speedoMeterNeedleOnBoard;
	TextMesh textMeshSpeed;
	TextMesh textMeshGear;

	[HideInInspector]
	public bool	showGUIDashboard=true;
	
	[HideInInspector]
	public CarController carController;
	Drivetrain drivetrain;
	
	Rect tachoMeterRect;
	Rect tachoMeterNeedleRect;
	Rect speedoMeterRect;
	Rect speedoMeterNeedleRect;
	Rect instrumentalPanelRect;
	Rect gearRect;
	Rect speedoRect;
	
	Vector2 pivot;
	float speedoMeterVelo;
	float digitalSpeedoVelo;
	float factor;
	float absVelo;
	int sign=1;
	int shift=0;
	
	void Start(){
		drivetrain=transform.parent.GetComponent<Drivetrain>();
		if (digitalSpeedoOnBoard!=null) textMeshSpeed=digitalSpeedoOnBoard.GetComponent<TextMesh>();
		if (digitalGearOnBoard!=null) textMeshGear=digitalGearOnBoard.GetComponent<TextMesh>();
		if (digitalSpeedoUnit==Unit.Kmh) factor=3.6f;
		else factor=2.237f;
		
	}
		
	void OnGUI() {
		if (speedoMeterType==SpeedoMeterType.RigidBody) absVelo=Mathf.Abs(drivetrain.velo);
		else absVelo=Mathf.Abs(drivetrain.wheelTireVelo);
		speedoMeterVelo=absVelo*3.6f;		
		digitalSpeedoVelo=absVelo*factor;
		
		GUI.depth=depth;
		if (RPMFactor<0) RPMFactor=0;
		if (drivetrain) actualtachoMeterNeedleAngle=drivetrain.rpm*(RPMFactor/10) + tachoMeterNeedleAngle;
		
		if (speedoMeterFactor<0.5f) speedoMeterFactor=0.5f;
		if (drivetrain) actualspeedoMeterNeedleAngle=speedoMeterVelo*speedoMeterFactor + speedoMeterNeedleAngle;
		
		if (!Application.isPlaying || showGUIDashboard==true){
			// GUI dashboard
			if (tachoMeter) {
				float shiftTacho=0;
				if (tachoMeterDocking==Docking.Right) shiftTacho=Screen.width-speedoMeter.width;			
				tachoMeterRect=new Rect(tachoMeterPosition.x + shiftTacho, Screen.height-tachoMeter.height - tachoMeterPosition.y+4,tachoMeter.width*tachoMeterDimension, tachoMeter.height*tachoMeterDimension);
				GUI.DrawTexture(tachoMeterRect, tachoMeter); 

				if (tachoMeterNeedle) {
					if (tachoMeterNeedleSize==Vector2.zero) {
						tachoMeterNeedleSize.x=tachoMeterNeedle.width;
						tachoMeterNeedleSize.y=tachoMeterNeedle.height;
					}				
					tachoMeterNeedleRect=new Rect(tachoMeterRect.x + tachoMeterRect.width/2 - tachoMeterNeedleSize.x*tachoMeterDimension*0.5f , tachoMeterRect.y + tachoMeterRect.height/2 - tachoMeterNeedleSize.y*tachoMeterDimension*0.5f, tachoMeterNeedleSize.x*tachoMeterDimension, tachoMeterNeedleSize.y*tachoMeterDimension);
					pivot = new Vector2(tachoMeterNeedleRect.xMin + tachoMeterNeedleRect.width*0.5f, tachoMeterNeedleRect.yMin + tachoMeterNeedleRect.height* 0.5f);
					Matrix4x4 matrixBackup = GUI.matrix;
					GUIUtility.RotateAroundPivot(actualtachoMeterNeedleAngle, pivot);
					GUI.DrawTexture(tachoMeterNeedleRect, tachoMeterNeedle);
					GUI.matrix = matrixBackup;
				}
			}
			
			if (speedoMeter) {
				float shiftSpeedo=0;
				if (speedoMeterDocking==Docking.Right) shiftSpeedo=Screen.width-speedoMeter.width;
				Rect speedoMeterRect = new Rect(speedoMeterPosition.x + shiftSpeedo, Screen.height-speedoMeter.height - speedoMeterPosition.y+4,speedoMeter.width*speedoMeterDimension, speedoMeter.height*speedoMeterDimension);
				GUI.DrawTexture(speedoMeterRect, speedoMeter); 
				
				if (speedoMeterNeedle) {
					if (speedoMeterNeedleSize==Vector2.zero) {
						speedoMeterNeedleSize.x=speedoMeterNeedle.width;
						speedoMeterNeedleSize.y=speedoMeterNeedle.height;
					}
					speedoMeterNeedleRect=new Rect(speedoMeterRect.x + speedoMeterRect.width/2 - speedoMeterNeedleSize.x*speedoMeterDimension*0.5f , speedoMeterRect.y + speedoMeterRect.height/2 - speedoMeterNeedleSize.y*speedoMeterDimension*0.5f, speedoMeterNeedleSize.x*speedoMeterDimension, speedoMeterNeedleSize.y*speedoMeterDimension);
					pivot = new Vector2(speedoMeterNeedleRect.xMin + speedoMeterNeedleRect.width*0.5f, speedoMeterNeedleRect.yMin + speedoMeterNeedleRect.height*0.5f);
					Matrix4x4 matrixBackup = GUI.matrix;
					GUIUtility.RotateAroundPivot(actualspeedoMeterNeedleAngle, pivot);
					GUI.DrawTexture(speedoMeterNeedleRect, speedoMeterNeedle);
					GUI.matrix = matrixBackup;
				}			
			}
			
			sign=1;
			shift=0;
			if (dashboardLightsDocking==Docking.Right) {
				sign=-1;
				shift=Mathf.RoundToInt(Screen.width - TCS.width*3*dashboardLightsDimension);
			}
			
			bool isCarControllerNull=(carController==null);
			if (TCS) {
				bool TCSTriggered=false;
				if (isCarControllerNull==false) TCSTriggered=carController.TCSTriggered;
				else if (!Application.isPlaying) TCSTriggered=true;
				
				if (TCSTriggered) 
					GUI.DrawTexture(new Rect(sign*dashboardLightsPosition.x+shift, Screen.height-TCS.height*dashboardLightsDimension-dashboardLightsPosition.y,TCS.width*dashboardLightsDimension, TCS.height*dashboardLightsDimension), TCS);
			}
			if (ABS) {
				bool ABSTriggered=false;
				if (isCarControllerNull==false) ABSTriggered=carController.ABSTriggered;
				else if (!Application.isPlaying) ABSTriggered=true;

				if (ABSTriggered)
					GUI.DrawTexture(new Rect(TCS.width*dashboardLightsDimension+sign*dashboardLightsPosition.x+shift, Screen.height-ABS.height*dashboardLightsDimension-dashboardLightsPosition.y,ABS.width*dashboardLightsDimension, ABS.height*dashboardLightsDimension), ABS); 				
			}
			if (ESP) {
				bool ESPTriggered=false;
				if (isCarControllerNull==false) ESPTriggered=carController.ESPTriggered;
				else if (!Application.isPlaying) ESPTriggered=true;
				
				if (ESPTriggered)
					GUI.DrawTexture(new Rect(TCS.width*dashboardLightsDimension + ABS.width*dashboardLightsDimension + sign*dashboardLightsPosition.x+shift, Screen.height-ESP.height*dashboardLightsDimension-dashboardLightsPosition.y,ESP.width*dashboardLightsDimension, ESP.height*dashboardLightsDimension), ESP); 
			}
		
			if (throttleMonitor){
				float throttle=0;
				if (isCarControllerNull==false) throttle=carController.throttle;
				else if (!Application.isPlaying) throttle=1;
				Rect throttleMonitorRect=new Rect(pedalsMonitorPosition.x + Screen.width-10, Screen.height - pedalsMonitorPosition.y,10, -throttleMonitor.height*throttle);
				GUI.DrawTexture(throttleMonitorRect, throttleMonitor); 
			}
			if (brakeMonitor){
				float brake=0;
				if (isCarControllerNull==false) brake=carController.brake;
				else if (!Application.isPlaying) brake=1;
				Rect brakeMonitorRect=new Rect(pedalsMonitorPosition.x-12 + Screen.width-10, Screen.height - pedalsMonitorPosition.y,10, -brakeMonitor.height*brake);
				GUI.DrawTexture(brakeMonitorRect, brakeMonitor); 
			}
			if (clutchMonitor){
				float clutchPosition=0;
				if (drivetrain!=null && drivetrain.clutch!=null) clutchPosition=drivetrain.clutch.GetClutchPosition();
				else if (!Application.isPlaying) clutchPosition=0;
				Rect clutchMonitorRect=new Rect(pedalsMonitorPosition.x-24 + Screen.width-10, Screen.height - pedalsMonitorPosition.y,10, -clutchMonitor.height*(1-clutchPosition));
				GUI.DrawTexture(clutchMonitorRect, clutchMonitor);
			}

			if (drivetrain){
				sign=1;
				shift=0;
				if (gearMonitorDocking==Docking.Right) {sign=-1;shift=Screen.width-25;}
				gearRect=new Rect(sign*gearMonitorPosition.x + shift,-gearMonitorPosition.y + Screen.height-50,50,50);
				if (drivetrain.gear<drivetrain.neutral) GUI.Label (gearRect,"R",gearMonitorStyle); //reverse
				else if (drivetrain.gear==drivetrain.neutral) GUI.Label (gearRect,"H",gearMonitorStyle); // neutral
				else GUI.Label (gearRect,""+(drivetrain.gear-drivetrain.neutral),gearMonitorStyle); 
				sign=1;
				shift=0;
				if (digitalSpeedoDocking==Docking.Right) {sign=-1;shift=Screen.width-25;}
				speedoRect=new Rect(sign*digitalSpeedoPosition.x + shift,-digitalSpeedoPosition.y + Screen.height-50,50,50);
				GUI.Label(speedoRect,""+Mathf.Round(digitalSpeedoVelo),digitalSpeedoStyle);
			}
		}
		
		// onboard dashboard
		if (textMeshSpeed!=null){
			textMeshSpeed.text=""+Mathf.Round(digitalSpeedoVelo);
		}
		if (tachoMeterNeedleOnBoard!=null){
			tachoMeterNeedleOnBoard.transform.localRotation = Quaternion.Euler (tachoMeterNeedleOnBoard.transform.localEulerAngles.x,tachoMeterNeedleOnBoard.transform.localEulerAngles.y,actualtachoMeterNeedleAngle);
		}
		if (speedoMeterNeedleOnBoard!=null){
			speedoMeterNeedleOnBoard.transform.localRotation = Quaternion.Euler (speedoMeterNeedleOnBoard.transform.localEulerAngles.x,speedoMeterNeedleOnBoard.transform.localEulerAngles.y,actualspeedoMeterNeedleAngle);
		}
		if (textMeshGear!=null){
			if (drivetrain.gear<drivetrain.neutral) textMeshGear.text="R";//reverse
			else if (drivetrain.gear==drivetrain.neutral) textMeshGear.text="H"; // neutral
			else textMeshGear.text=""+(drivetrain.gear-drivetrain.neutral); 
		}
  }
}
