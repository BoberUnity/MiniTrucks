using UnityEngine;

public class MouseCarController : CarController{

	public string clutchAxis="Clutch";
	public string shiftUpButton="ShiftUp";
	public string shiftDownButton="ShiftDown";
	public string startEngineButton="StartEngine";
	
	protected override void GetInput(out float throttleInput, 
									out float brakeInput, 
									out float steerInput, 
									out float handbrakeInput,
									out float clutchInput,
									out bool startEngineInput,
									out int targetGear){
		
		startEngineInput=Input.GetButtonDown (startEngineButton);
		
		if (Input.GetMouseButton(0)){
			throttleInput = 1f;
		}
		else{
			throttleInput = 0f;
		}
		
		if (Input.GetMouseButton(1)){
			brakeInput = 1f;
		}
		else{
			brakeInput = 0f;
		}
		
		if (Input.GetMouseButton(2)){
			handbrakeInput = 1f;
		}
		else{
			handbrakeInput = 0f;
		}

		
		steerInput = (Input.mousePosition.x - Screen.width*0.5f)/Screen.width*2;
		clutchInput =Input.GetAxisRaw (clutchAxis);
		
		// Gear shift
		targetGear = drivetrain.gear;
		if(Input.GetButtonDown(shiftUpButton)){
			++targetGear;
		}
		if(Input.GetButtonDown(shiftDownButton)){
			--targetGear;
		}
	}
}
