using UnityEngine;

public class MobileCarController : CarController{
	//-------------------------------------------------------------------------
	// Public class definition
	//-------------------------------------------------------------------------
	[System.Serializable]
	public class TouchButton{
		public GUITexture texture = null;
		public float alphaButtonDown = 0.20f;
		public float alphaButtonReleased = 0.30f;
	}
	
	//-------------------------------------------------------------------------
	// Public class properties
	//-------------------------------------------------------------------------
	public TouchButton throttleButton;
	public TouchButton brakeButton;
	public TouchButton handbrakeButton;
	public TouchButton gearUpButton;
	public TouchButton gearDownButton;
	
	//-------------------------------------------------------------------------
	// Override Methods
	//-------------------------------------------------------------------------
	protected override void GetInput(out float throttleInput, 
									out float brakeInput, 
									out float steerInput, 
									out float handbrakeInput,
									out float clutchInput,
									out bool startEngineInput,
									out int targetGear){
		
		clutchInput=0;
		startEngineInput=false;
		
		// Read the steering from accelerometers
		steerInput = Mathf.Clamp(-Input.acceleration.y, -1, 1);
		
		// Read the throttle, brake and handbrake input from touch buttons
		throttleInput = 0f;
		brakeInput = 0f;
		handbrakeInput = 0f;
		bool gearUp = false;
		bool gearDown = false;
		
		// Iterate over all touches...
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; ++i){
			Touch t = Input.GetTouch(i);
			
			// Check if the throttle button is pressed
			if (t.phase != TouchPhase.Ended &&
			    this.throttleButton != null && 
			    this.throttleButton.texture != null &&
			    this.throttleButton.texture.HitTest(t.position)){
				
				throttleInput = 1f;
				
				Color c = this.throttleButton.texture.color;
				c.a = this.throttleButton.alphaButtonDown;
				this.throttleButton.texture.color = c;
			}
			
			// Check if the brake button is pressed
			if (t.phase != TouchPhase.Ended &&
			    this.brakeButton != null && 
			    this.brakeButton.texture != null &&
			    this.brakeButton.texture.HitTest(t.position)){
				
				brakeInput = 1f;
				
				Color c = this.brakeButton.texture.color;
				c.a = this.brakeButton.alphaButtonDown;
				this.brakeButton.texture.color = c;
			}
			
			// Check if the handbrake button is pressed
			if (t.phase != TouchPhase.Ended &&
			    this.handbrakeButton != null && 
			    this.handbrakeButton.texture != null &&
			    this.handbrakeButton.texture.HitTest(t.position)){
				
				handbrakeInput = 1f;
				
				Color c = this.handbrakeButton.texture.color;
				c.a = this.handbrakeButton.alphaButtonDown;
				this.handbrakeButton.texture.color = c;
			}
			
			// Check if the "gear up" button has been pressed this frame
			if (t.phase == TouchPhase.Began &&
			    this.gearUpButton != null && 
			    this.gearUpButton.texture != null &&
			    this.gearUpButton.texture.HitTest(t.position)){
				
				gearUp = true;
				
				Color c = this.gearUpButton.texture.color;
				c.a = this.gearUpButton.alphaButtonDown;
				this.gearUpButton.texture.color = c;
			}
			
			// Check if the "gear down" button has been pressed this frame
			if (t.phase == TouchPhase.Began &&
			    this.gearDownButton != null && 
			    this.gearDownButton.texture != null &&
			    this.gearDownButton.texture.HitTest(t.position)){
				
				gearDown = true;
				
				Color c = this.gearDownButton.texture.color;
				c.a = this.gearDownButton.alphaButtonDown;
				this.gearDownButton.texture.color = c;
			}
		}
		
		// Change gear if necessary
		targetGear = drivetrain.gear;
		if (gearUp && !gearDown){
			++targetGear;
		}else if (gearDown && !gearUp){
			--targetGear;
		}
		
		// Finally, change the button alphas
		if (this.throttleButton != null && 
			this.throttleButton.texture != null &&
		    throttleInput < Mathf.Epsilon){
			
			Color c = this.throttleButton.texture.color;
			c.a = this.throttleButton.alphaButtonReleased;
			this.throttleButton.texture.color = c;
		}
		
		if (this.brakeButton != null && 
			this.brakeButton.texture != null &&
		    brakeInput < Mathf.Epsilon){
			
			Color c = this.brakeButton.texture.color;
			c.a = this.brakeButton.alphaButtonReleased;
			this.brakeButton.texture.color = c;
		}
		
		if (this.handbrakeButton != null && 
			this.handbrakeButton.texture != null &&
		    handbrakeInput < Mathf.Epsilon){
			
			Color c = this.handbrakeButton.texture.color;
			c.a = this.handbrakeButton.alphaButtonReleased;
			this.handbrakeButton.texture.color = c;
		}
		
		if (this.gearUpButton != null && 
			this.gearUpButton.texture != null &&
		    !gearUp){
			
			Color c = this.gearUpButton.texture.color;
			c.a = this.gearUpButton.alphaButtonReleased;
			this.gearUpButton.texture.color = c;
		}
		
		if (this.gearDownButton != null && 
			this.gearDownButton.texture != null &&
		    !gearDown){
			
			Color c = this.gearDownButton.texture.color;
			c.a = this.gearDownButton.alphaButtonReleased;
			this.gearDownButton.texture.color = c;
		}
	}
}
