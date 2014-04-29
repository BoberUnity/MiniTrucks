using UnityEngine;

public class SelectCarController : MonoBehaviour
{
  [SerializeField] private ButtonChangeCar buttonNextCar   = null;
  [SerializeField] private ButtonChangeCar buttonPrevCar = null;
  [SerializeField] private ButtonChangeCar buttonSelectCar = null;
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private ButtonThrottle buttonBrake = null;
  [SerializeField] private ButtonThrottle buttonNitro = null;
  [SerializeField] private ButtonTuning buttonTuningEng = null;
  [SerializeField] private ButtonTuning buttonTuningHand = null;
  [SerializeField] private ButtonAddTrailer[] buttonsAddTrailer = null;
  [SerializeField] private Map map = null;

  [SerializeField] private GameObject[] cars = null;
  [SerializeField] private Transform carGaragePos = null;
  [SerializeField] private Transform podium = null;
  [SerializeField] private Transform carLevelPos = null;
  [SerializeField] private ButtonHandler garageButton = null;//Кнопка перехода в гараж из меню станции
  private GameObject car = null;
  private int currentCar = 0;

  private void Start()
	{
    buttonNextCar.Pressed += NextCar;
    buttonPrevCar.Pressed += PrevCar;
    buttonSelectCar.Pressed += SelectCar;
    garageButton.Pressed += GoToGarage;
    car = Instantiate(cars[0], carGaragePos.position, Quaternion.identity) as GameObject;
    if (car != null)
      car.transform.parent = podium;
	}

  private void OnDestroy()
  {
    buttonNextCar.Pressed -= NextCar;
    buttonPrevCar.Pressed -= PrevCar;
    buttonSelectCar.Pressed -= SelectCar;
    garageButton.Pressed -= GoToGarage;
  }

  private void NextCar() 
  {
	  Destroy(car);
	  currentCar += 1;
    if (currentCar > cars.Length - 1)
      currentCar = 0;
    car = Instantiate(cars[currentCar], carGaragePos.position, Quaternion.identity) as GameObject;
    if (car != null)
      car.transform.parent = podium;
	}

  private void PrevCar()
  {
    Destroy(car);
    currentCar -= 1;
    if (currentCar < 0)
      currentCar = cars.Length - 1;
    car = Instantiate(cars[currentCar], carGaragePos.position, Quaternion.identity) as GameObject;
    if (car != null)
      car.transform.parent = podium;
  }

  private void SelectCar()
  {
    car.transform.parent = null;
    car.transform.position = carLevelPos.position;
    car.transform.rotation = carLevelPos.rotation;
    
    carcameras.gameObject.SetActive(true);
    gameObject.SetActive(false);
    CameraTarget tractor = car.GetComponentInChildren<CameraTarget>();  //Находим грузовик, на который будет нацелена камера
    if (tractor != null)
    {
      carcameras.target = tractor.transform;
      foreach (var butt in buttonsAddTrailer)
      {
        butt.TruckCar = tractor.transform;
      }
      AxisCarController aCC = tractor.GetComponent<AxisCarController>();
      aCC.On = true;
      steer.axisCarController = aCC;
      buttonBrake.axisCarController = aCC;
      buttonNitro.axisCarController = aCC;
      buttonTuningEng.drivetrain = tractor.GetComponent<Drivetrain>();
      buttonTuningHand.axles = tractor.GetComponent<Axles>();
      buttonTuningEng.setup = tractor.GetComponent<Setup>();
      map.Truck = tractor.transform;
    }
  }

  private void GoToGarage()
  {
    gameObject.SetActive(true);
    carcameras.gameObject.SetActive(false);
    car.transform.position = carGaragePos.position;
    car.transform.rotation = carGaragePos.rotation;
    CameraTarget tractor = car.GetComponentInChildren<CameraTarget>();  //Находим грузовик
    if (tractor != null)
    {
      tractor.transform.localPosition = Vector3.zero;
    }
    car.transform.parent = podium;

    
    
  }
}
