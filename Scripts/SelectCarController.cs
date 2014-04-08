using UnityEngine;

public class SelectCarController : MonoBehaviour
{
  [SerializeField] private ButtonChangeCar buttonNextCar   = null;
  [SerializeField] private ButtonChangeCar buttonPrevCar = null;
  [SerializeField] private ButtonChangeCar buttonSelectCar = null;
  [SerializeField]
  private CarCameras carcameras = null;
  [SerializeField]
  private Steer steer = null;
  [SerializeField]
  private Button2D buttonBrake = null;
  [SerializeField]
  private Button2D buttonNitro = null;
  [SerializeField]
  private ButtonTuning buttonTuningEng = null;
  [SerializeField]
  private ButtonTuning buttonTuningHand = null;

  [SerializeField] private GameObject[] cars = null;
  [SerializeField] private Transform carGaragePos = null;
  [SerializeField] private Transform carLevelPos = null;
  private GameObject car = null;
  private int currentCar = 0;

  private void Start()
	{
    buttonNextCar.Pressed += NextCar;
    buttonPrevCar.Pressed += PrevCar;
    buttonSelectCar.Pressed += SelectCar;
    car = Instantiate(cars[0], carGaragePos.position, Quaternion.identity) as GameObject;
	}

  private void OnDestroy()
  {
    buttonNextCar.Pressed -= NextCar;
    buttonPrevCar.Pressed -= PrevCar;
    buttonSelectCar.Pressed -= SelectCar;
  }

  private void NextCar() 
  {
	  Destroy(car);
	  currentCar += 1;
    if (currentCar > cars.Length - 1)
      currentCar = 0;
    car = Instantiate(cars[currentCar], carGaragePos.position, Quaternion.identity) as GameObject;
	}

  private void PrevCar()
  {
    Destroy(car);
    currentCar -= 1;
    if (currentCar < 0)
      currentCar = cars.Length - 1;
    car = Instantiate(cars[currentCar], carGaragePos.position, Quaternion.identity) as GameObject;
  }

  private void SelectCar()
  {
    car.transform.position = carLevelPos.position;
    carcameras.gameObject.SetActive(true);
    gameObject.SetActive(false);
    CameraTarget cts = car.GetComponentInChildren<CameraTarget>();  //Находим грузовик, на который будет нацелена камера
    if (cts != null)
    {
      carcameras.target = cts.transform;
      AxisCarController aCC = cts.GetComponent<AxisCarController>();
      aCC.On = true;
      steer.axisCarController = aCC;
      buttonBrake.axisCarController = aCC;
      buttonNitro.axisCarController = aCC;
      buttonTuningEng.drivetrain = cts.GetComponent<Drivetrain>();
      buttonTuningHand.axles = cts.GetComponent<Axles>();
      buttonTuningEng.setup = cts.GetComponent<Setup>();
    }
    Trailer trailer = car.GetComponentInChildren<Trailer>();  //Находим прицепа, 
    if (trailer != null)
    {
      buttonTuningHand.axlesTrailer = trailer.GetComponent<Axles>();
      buttonTuningEng.setupTrailer = trailer.GetComponent<Setup>();
    }
  }
}
