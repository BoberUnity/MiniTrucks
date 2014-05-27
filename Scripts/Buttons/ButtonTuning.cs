using UnityEngine;

public class ButtonTuning : MonoBehaviour
{
  [SerializeField] private int id = 0;//0-enhine; 1-handling; 2-brakes
  [SerializeField] private SelectCarController selectCarController = null;
  [SerializeField] private int price = 500;
  [SerializeField] private UILabel powerIndicator = null;
  [SerializeField] private UILabel handlingIndicator = null;
  [SerializeField] private UILabel brakesIndicator = null;
  [SerializeField] private UILabel maxSpeedIndicator = null;
  public Drivetrain drivetrain = null;
  public Axles axles = null;
  //public Axles axlesTrailer = null;
  public Setup setup = null;
  public CarDynamics carDynamics = null;
  //public Setup setupTrailer = null;
  public int TunStep = 0;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed && TunStep < 5 && selectCarController.Gold >= price)
    {
      TunStep += 1;
      selectCarController.Gold -= price;
      if (id == 0)
      {
        drivetrain.maxPower += 50;
        powerIndicator.text = drivetrain.maxPower.ToString("f0");
        drivetrain.maxTorque += 150;
        selectCarController.TunningMaxSpeed();
        maxSpeedIndicator.text = drivetrain.gameObject.GetComponent<AxisCarController>().MaxSpeed.ToString("f0");
        selectCarController.SetRegParamSpeed();
      }

      if (id == 1)
      {
        axles.frontAxle.sidewaysGripFactor += 0.2f;
        axles.rearAxle.sidewaysGripFactor += 0.2f;
        foreach (Axle axle in axles.otherAxles)
        {
          axle.sidewaysGripFactor += 0.2f;
        }
        carDynamics.SetWheelsParams();
        handlingIndicator.text = axles.frontAxle.sidewaysGripFactor.ToString("f1");
        selectCarController.SetRegParamHandling();
      }

      if (id == 2)
      {
        axles.frontAxle.brakeFrictionTorque += 500;
        axles.rearAxle.brakeFrictionTorque += 500;
        foreach (Axle axle in axles.otherAxles)
        {
          axle.brakeFrictionTorque += 500;
        }
        carDynamics.SetBrakes();
        brakesIndicator.text = axles.frontAxle.brakeFrictionTorque.ToString("f0");
        selectCarController.SetRegParamBrake();
      }

      if (setup != null)
      {
        if (setup.SaveToFile(setup.filePath))
        {
          setup.SaveSetup();
        }
      }

      //if (id == 3)
      //{
      //  selectCarController.TunningMaxSpeed();
      //  maxSpeedIndicator.text = drivetrain.gameObject.GetComponent<AxisCarController>().MaxSpeed.ToString("f0");
      //  selectCarController.SetRegParam();
      //}
    }
	}
}

