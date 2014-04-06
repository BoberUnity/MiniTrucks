using UnityEngine;

public class ButtonTuning : MonoBehaviour 
{
  public Drivetrain drivetrain = null;
  public Setup setup = null;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      drivetrain.maxPower += 1000;
      drivetrain.maxTorque += 1000;
      if (setup != null)
      {
        if (setup.SaveToFile(setup.filePath))
        {
          setup.SaveSetup();
        }
      }
    }
	}
}

