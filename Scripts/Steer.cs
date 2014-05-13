using UnityEngine;

public class Steer : MonoBehaviour
{
  [SerializeField] private UISlider uISliderSteer = null;
  public AxisCarController axisCarController = null;
  [SerializeField] private UISprite rulSprite = null;
  [SerializeField] private float steerSpeed = 90;
  private float realSteerSpeed = 0;
  [SerializeField] private float angle = 0;
  [SerializeField] private float anglePrev = 0;
  [SerializeField] private float a2 = 0;//(-180;180)
  //[SerializeField] private float a3 = 0;
  [SerializeField] private int round = 0;
	void Update ()
	{
    //realSteerSpeed = Mathf.Clamp(uISliderSteer.SteerRot, realSteerSpeed - Time.deltaTime*steerSpeed, realSteerSpeed + Time.deltaTime*steerSpeed);
    
	  //realSteerSpeed = 300;
    //if (axisCarController != null)
    //  axisCarController.SteerUsed = realSteerSpeed;

    if (axisCarController != null)
    {
      anglePrev = angle;
      angle = uISliderSteer.AngleFactor;
      
      if (angle > anglePrev + 300 )
        round -= 1;

      if (angle < anglePrev - 300 )
        round += 1;

      a2 = angle + round * 360;
      
      if (a2 > 540)
      {
        a2 -= 360; 
        round -= 1;
      }
      if (a2 < -540)
      {
        a2 += 360; 
        round += 1;
      }
      a2 = Mathf.Clamp(a2, -170, 170);
      
      if (a2 > 180)
        axisCarController.SteerUsed = -a2/180 + 2 ;
      else
        axisCarController.SteerUsed = -a2/180;
    }
    
    if (Input.GetKey("left"))
      axisCarController.SteerUsed = -0.7f;
      //realSteerSpeed = -300; 
    if (Input.GetKey("right"))
      axisCarController.SteerUsed = 0.7f;
    
	  //rulSprite.transform.eulerAngles = new Vector3(0, 0, -realSteerSpeed*1);
    rulSprite.transform.eulerAngles = new Vector3(0, 0, a2);
	}
}
