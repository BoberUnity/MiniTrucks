using UnityEngine;

public class ButtonBrake : MonoBehaviour 
{
  public AxisCarController axisCarController = null;
  

  protected virtual void OnPress(bool isPressed)
  {
    axisCarController.BrakeUsed = isPressed;
  }
}

