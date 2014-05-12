﻿using UnityEngine;

public class Steer : MonoBehaviour
{
  [SerializeField] private UISlider uISliderSteer = null;
  public AxisCarController axisCarController = null;
  [SerializeField] private UISprite rulSprite = null;
  [SerializeField] private float steerSpeed = 90;
  private float realSteerSpeed = 0;
  [SerializeField] private float angle = 0;

	void Update ()
	{
	  realSteerSpeed = Mathf.Clamp(uISliderSteer.SteerRot, realSteerSpeed - Time.deltaTime*steerSpeed, realSteerSpeed + Time.deltaTime*steerSpeed);
    if (Input.GetKey("left"))
      realSteerSpeed = -300; 
    if (Input.GetKey("right"))
      realSteerSpeed = 300;
    if (axisCarController != null)
      axisCarController.SteerUsed = realSteerSpeed;
    
	  //rulSprite.transform.eulerAngles = new Vector3(0, 0, -(uISliderSteer.value - 0.5f) * 270);
    rulSprite.transform.eulerAngles = new Vector3(0, 0, -realSteerSpeed*1);

	  angle = uISliderSteer.Angle;
	}
}
