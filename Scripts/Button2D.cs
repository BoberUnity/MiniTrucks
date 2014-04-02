using UnityEngine;

public class Button2D : MonoBehaviour 
{
  public AxisCarController axisCarController = null;
  [SerializeField] private int id = 0; //0-brake; 1- nitro

  protected virtual void OnPress(bool isPressed)
  {
    if (id == 0)
      axisCarController.BrakeUsed = isPressed;
    if (id == 1)
      axisCarController.NitroUsed = isPressed;
	}
}

