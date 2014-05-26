using UnityEngine;

public class ButtonTuning : MonoBehaviour
{
  [SerializeField] private int id = 0;//0-enhine; 1-handling; 2-brakes
  
  [SerializeField] private UILabel powerIndicator = null;
  [SerializeField] private UILabel handlingIndicator = null;
  [SerializeField] private UILabel brakesIndicator = null;
  [SerializeField] private UILabel maxSpeedIndicator = null;

  [SerializeField] private SelectCarController selectCarController = null;
  public Drivetrain drivetrain = null;
  public Axles axles = null;
  //public Axles axlesTrailer = null;
  public Setup setup = null;
  public CarDynamics carDynamics = null;
  //public Setup setupTrailer = null;
  public int TunStep = 0;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed && TunStep < 5)
    {
      TunStep += 1;
      if (id == 0)
      {
        drivetrain.maxPower += 100;
        powerIndicator.text = drivetrain.maxPower.ToString("f0");
        drivetrain.maxTorque += 300;
      }

      if (id == 1)
      {
        axles.frontAxle.sidewaysGripFactor = 0.5f;
        axles.rearAxle.sidewaysGripFactor = 0.5f;
        foreach (Axle axle in axles.otherAxles)
        {
          axle.sidewaysGripFactor = 0.5f;
        }
        carDynamics.SetWheelsParams();
        handlingIndicator.text = axles.frontAxle.sidewaysGripFactor.ToString("f1");
      }

      if (id == 2)
      {
        axles.frontAxle.brakeFrictionTorque = 20000;
        axles.rearAxle.brakeFrictionTorque = 20000;
        foreach (Axle axle in axles.otherAxles)
        {
          axle.brakeFrictionTorque = 20000;
        }
        carDynamics.SetBrakes();
        brakesIndicator.text = axles.frontAxle.brakeFrictionTorque.ToString("f0");
      }

      if (setup != null)
      {
        if (setup.SaveToFile(setup.filePath))
        {
          setup.SaveSetup();
        }
      }

      if (id == 3)
      {
        selectCarController.TunningMaxSpeed();
        maxSpeedIndicator.text = drivetrain.gameObject.GetComponent<AxisCarController>().MaxSpeed.ToString("f0");
        selectCarController.SetRegParam();
      }
    }
	}
}

