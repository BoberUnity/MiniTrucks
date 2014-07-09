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
  public CarDynamics carDynamics = null;
  
  [SerializeField] private UILabel tunStepLab = null;
  [SerializeField] private int tunStep = 0;

  public int TunStep
  {
    get { return tunStep;}
    set
    {
      tunStep = value;
      if (tunStep > 0)
      {
        if (id == 0)
        {
          drivetrain.maxPower = selectCarController.enemyCar[selectCarController.currentCar].MaxPower + TunStep * 100;
          powerIndicator.text = drivetrain.maxPower.ToString("f0");
          drivetrain.maxTorque = selectCarController.enemyCar[selectCarController.currentCar].MaxTorque + TunStep * 550;
          drivetrain.gameObject.GetComponent<AxisCarController>().MaxSpeed = selectCarController.enemyCar[selectCarController.currentCar].MaxSpeed + TunStep*3;
          maxSpeedIndicator.text = drivetrain.gameObject.GetComponent<AxisCarController>().MaxSpeed.ToString("f0");
          selectCarController.enemyCar[selectCarController.currentCar].TunSpeed = tunStep;
          PlayerPrefs.SetInt("TunSpeed" + selectCarController.currentCar.ToString("f0"), tunStep);
        }
        if (id == 1)
        {
          axles.frontAxle.sidewaysGripFactor = selectCarController.enemyCar[selectCarController.currentCar].Sideways + TunStep * 0.2f;
          axles.rearAxle.sidewaysGripFactor = selectCarController.enemyCar[selectCarController.currentCar].Sideways + TunStep * 0.2f;
          foreach (Axle axle in axles.otherAxles)
          {
            axle.sidewaysGripFactor = selectCarController.enemyCar[selectCarController.currentCar].Sideways + TunStep * 0.2f;
          }
          carDynamics.SetWheelsParams();
          handlingIndicator.text = axles.frontAxle.sidewaysGripFactor.ToString("f1");
          selectCarController.enemyCar[selectCarController.currentCar].TunHandling = tunStep;
          PlayerPrefs.SetInt("TunHandling" + selectCarController.currentCar.ToString("f0"), tunStep);
        }

        if (id == 2)
        {
          axles.frontAxle.brakeFrictionTorque = selectCarController.enemyCar[selectCarController.currentCar].Brake + TunStep * 500;
          axles.rearAxle.brakeFrictionTorque = selectCarController.enemyCar[selectCarController.currentCar].Brake + TunStep * 500;
          foreach (Axle axle in axles.otherAxles)
          {
            axle.brakeFrictionTorque = selectCarController.enemyCar[selectCarController.currentCar].Brake + TunStep * 500; 
          }
          carDynamics.SetBrakes();
          brakesIndicator.text = axles.frontAxle.brakeFrictionTorque.ToString("f0");
          selectCarController.enemyCar[selectCarController.currentCar].TunBrake = tunStep;
          PlayerPrefs.SetInt("TunBrake" + selectCarController.currentCar.ToString("f0"), tunStep);
        } 
      }
    }
  }

  private void Update()
  {
    tunStepLab.text = tunStep.ToString("f0");
  }

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed && TunStep < 5 && selectCarController.Gold >= price)
    {
      TunStep += 1;
      selectCarController.Gold -= price;
    }
	}
}

