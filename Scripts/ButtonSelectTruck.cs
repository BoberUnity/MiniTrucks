using UnityEngine;

public class ButtonSelectTruck : MonoBehaviour
{
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private ButtonThrottle buttonBrake = null;
  [SerializeField]
  private ButtonThrottle buttonNitro = null;
  [SerializeField] private ButtonTuning buttonTuningEng = null;
  [SerializeField] private ButtonTuning buttonTuningHand = null;
  [SerializeField] private Transform pos = null;
  [SerializeField] private GameObject[] trucks = null;
  [SerializeField] private int id = 0;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      GameObject truck = Instantiate(trucks[id], pos.position, Quaternion.identity) as GameObject;
      if (truck != null)
      {
        CameraTarget cts = truck.GetComponentInChildren<CameraTarget>();  //Находим грузовик, на который будет нацелена камера
        if (cts != null)
        {
          carcameras.target = cts.transform;
          AxisCarController aCC = cts.GetComponent<AxisCarController>();
          steer.axisCarController = aCC;
          buttonBrake.axisCarController = aCC;
          buttonNitro.axisCarController = aCC;
          buttonTuningEng.drivetrain = cts.GetComponent<Drivetrain>();
          buttonTuningHand.axles = cts.GetComponent<Axles>();
          buttonTuningEng.setup = cts.GetComponent<Setup>();
        }
        else
        {
          Debug.LogWarning("CameraTarget was not found");
        }

        Trailer trailer = truck.GetComponentInChildren<Trailer>();  //Находим прицепа, 
        if (trailer != null)
        {
          AxisCarController aCCF = trailer.GetComponent<AxisCarController>();
          buttonTuningHand.axlesTrailer = aCCF.GetComponent<Axles>();
          buttonTuningEng.setupTrailer = aCCF.GetComponent<Setup>();
        }
        else
        {
          Debug.LogWarning("Trailer was not found");
        }
      }
      else
      {
        Debug.LogWarning("truck = NULL");
      }
    }
	}
}

