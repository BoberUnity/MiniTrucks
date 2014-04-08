using System;
using UnityEngine;

public class ButtonChangeCar : MonoBehaviour 
{
  
  public event Action Pressed = null; 
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      var handler = Pressed;
      if (handler != null)
        handler();
    }
  }
}

