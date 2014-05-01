using System;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
  public event Action Pressed;
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      var handler = Pressed;
      if (handler != null)
        Pressed();
      Debug.LogWarning("Pressed "+ gameObject.name);
    }
	}
}

