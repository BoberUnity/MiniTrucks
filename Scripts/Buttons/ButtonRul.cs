using UnityEngine;

public class ButtonRul : MonoBehaviour
{
  public AxisCarController axisCarController = null;
  [SerializeField] private bool left = false;

  protected virtual void OnPress(bool isPressed)
  {
    if (isPressed)
    {
      if (left)
        axisCarController.SteerUsed = -0.9f;
      else
        axisCarController.SteerUsed = 0.9f;
    }
    else 
    {
      axisCarController.SteerUsed = 0.0f;
    }
  }

  //private  void Update()
  //{
  //  if (left && !press && axisCarController != null)
  //  {
  //    if (Input.GetKey("left"))
  //    {
  //      axisCarController.SteerUsed = -0.9f;
  //    }

  //    if (Input.GetKey("right"))
  //    {
  //      axisCarController.SteerUsed = 0.9f;
  //    }

  //    if (!Input.GetKey("left") && !Input.GetKey("right"))
  //      axisCarController.SteerUsed = 0.0f;
  //  }
  //}
}
