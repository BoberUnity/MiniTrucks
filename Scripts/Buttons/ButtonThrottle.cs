using UnityEngine;

public class ButtonThrottle : MonoBehaviour 
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

  private void Update()
  {
    if (Input.GetKeyDown("down"))
      axisCarController.BrakeUsed = true;
    if (Input.GetKeyUp("down"))
      axisCarController.BrakeUsed = false;

    if (Input.GetKeyDown("up"))
      axisCarController.NitroUsed = true;
    if (Input.GetKeyUp("up"))
      axisCarController.NitroUsed = false;
  }
}

