using UnityEngine;

public class ButtonTuning : MonoBehaviour 
{
  [SerializeField] private Drivetrain drivetrain = null;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      drivetrain.maxPower += 1000;
      drivetrain.maxTorque += 1000;
    }
	}
}

