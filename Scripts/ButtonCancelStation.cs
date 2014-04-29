using System;
using UnityEngine;

public class ButtonCancelStation : MonoBehaviour
{
  [SerializeField] private RaceStart raceStart = null;
  
	protected virtual void OnPress(bool isPressed)
	{
	  if (!isPressed)
	  {
      raceStart.ExitStation();
	  }
	}
}
