using UnityEngine;

public class ButtonTuning : MonoBehaviour
{
  [SerializeField] private int id = 0;//0-enhine; 1-handling; 2-brakes
  public Drivetrain drivetrain = null;
  public Axles axles = null;
  public Setup setup = null;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      if (id == 0)
      {
        drivetrain.maxPower += 1000;
        drivetrain.maxTorque += 1000;
      }

      if (id == 1)
      {
        axles.frontAxle.sidewaysGripFactor = 0.9f;
        axles.rearAxle.sidewaysGripFactor = 0.9f;
      }

      if (id == 2)
      {
        axles.frontAxle.brakeFrictionTorque = 7000;
        axles.rearAxle.brakeFrictionTorque = 7000;
      }

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

