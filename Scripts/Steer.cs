using UnityEngine;

public class Steer : MonoBehaviour
{
  [SerializeField] private UISlider uISliderSteer = null;
  public AxisCarController axisCarController = null;
  [SerializeField] private UISprite rulSprite = null;
  [SerializeField] private float steerSpeed = 600;
  //private float realSteerSpeed = 0;
  private float angle = 0;
  private float anglePrev = 0;
  private float angleNormalize = 0;//(-180;180)
  private int round = 0;
  private float spriteAngle = 0;
  public bool Acsel = false;
	
  private void Update ()
	{
    if (axisCarController != null)
    {
      if (Acsel)
      {
        float acs = Mathf.Clamp(Input.acceleration.x * 2.0f, -1, 1);
        axisCarController.SteerUsed = acs;
        angleNormalize = -acs * 170;
      }
      else
      {
        anglePrev = angle;
        angle = uISliderSteer.AngleFactor;
        if (angle < 0.1f && angle > -0.1f) //отпускание тача
        {
          angleNormalize = 0;
          round = 0;
        }

        if (angle > anglePrev + 300)
          round -= 1;

        if (angle < anglePrev - 300)
          round += 1;

        angleNormalize = angle + round*360;

        if (angleNormalize > 540)
        {
          angleNormalize -= 360;
          round -= 1;
        }
        if (angleNormalize < -540)
        {
          angleNormalize += 360;
          round += 1;
        }
        angleNormalize = Mathf.Clamp(angleNormalize, -170, 170);

        if (angleNormalize > 180)
          axisCarController.SteerUsed = -angleNormalize/180 + 2;
        else
          axisCarController.SteerUsed = -angleNormalize/180;

        if (Input.GetKey("left"))
        {
          angleNormalize = 170;
          axisCarController.SteerUsed = -0.9f;
        }

        if (Input.GetKey("right"))
        {
          angleNormalize = -170;
          axisCarController.SteerUsed = 0.9f;
        }
      }
    }
    
	  //rulSprite.transform.eulerAngles = new Vector3(0, 0, -realSteerSpeed*1);
    //Скорость вращения руля, возврат
    spriteAngle = Mathf.Lerp(spriteAngle, Mathf.Clamp(angleNormalize, spriteAngle - Time.deltaTime * steerSpeed, spriteAngle + Time.deltaTime * steerSpeed), 0.7f);
    rulSprite.transform.eulerAngles = new Vector3(0, 0, spriteAngle);
	}
}
