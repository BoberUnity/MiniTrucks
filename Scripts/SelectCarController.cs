using System.Collections;
using UnityEngine;

public class SelectCarController : MonoBehaviour
{
  [SerializeField] private ButtonHandler buttonNextCar = null;
  [SerializeField] private ButtonHandler buttonPrevCar = null;
  [SerializeField] private ButtonHandler buttonRace = null;
  [SerializeField] private ButtonGoToGarage[] goToGarageButtons = null;//Кнопка перехода в гараж из меню станции
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private GameObject cameraGarage = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private ButtonThrottle buttonBrake = null;
  [SerializeField] private ButtonThrottle buttonNitro = null;
  [SerializeField] private ButtonTuning buttonTuningEng = null;
  [SerializeField] private ButtonTuning buttonTuningHand = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private ButtonAddTrailer[] buttonsAddTrailer = null;
  [SerializeField] private Map map = null;
  [SerializeField] private GameObject[] cars = null;
  [SerializeField] private Transform carGaragePos = null;
  [SerializeField] private Transform podium = null;
  [SerializeField] private Transform carLevelPos = null;
  
  private GameObject character = null;
  private int currentCar = 0;
  public RaceStart raceStart = null;

  private void Start()
	{
    buttonNextCar.Pressed += NextCar;
    buttonPrevCar.Pressed += PrevCar;
    buttonRace.Pressed += Race;
    goToGarageButtons = FindObjectsOfType(typeof(ButtonGoToGarage)) as ButtonGoToGarage[];//Кнопки переходи в гараж
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed += GoToGarage;
    }
    buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
    character = Instantiate(cars[0], carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
      character.transform.parent = podium;
	}

  private void OnDestroy()
  {
    buttonNextCar.Pressed -= NextCar;
    buttonPrevCar.Pressed -= PrevCar;
    buttonRace.Pressed -= Race;
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed -= GoToGarage;
    }
  }

  private void NextCar() 
  {
    Destroy(character);
	  currentCar += 1;
    if (currentCar > cars.Length - 1)
      currentCar = 0;
    character = Instantiate(cars[currentCar], carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
    }
	}

  private void PrevCar()
  {
    Destroy(character);
    currentCar -= 1;
    if (currentCar < 0)
      currentCar = cars.Length - 1;
    character = Instantiate(cars[currentCar], carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
    }
  }

  private void Race()//ButtonRace
  {
    character.transform.parent = null;
    character.transform.position = carLevelPos.position;
    character.transform.rotation = carLevelPos.rotation;
    
    carcameras.gameObject.SetActive(true);
    cameraGarage.SetActive(false);
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();  //Находим грузовик, на который будет нацелена камера
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
      
      if (raceStart != null)//Если заходили в гагаж и меню станции
      {
        StartCoroutine(ShowStationMenu(gamePanel.animation.clip.length));//!!! Должно быть не gamePanel, a Upgrade Panel
        aCC.InStation = true;
        //Debug.LogWarning("STtcorr");
        //raceStart = null;
      }
      else//При первом запуске активируем Game Menu
      {
        aCC.InStation = false;
        StartCoroutine(ShowGameMenu(gamePanel.animation.clip.length));
      }
    }
  }

  private IEnumerator ShowStationMenu(float time)
  {
    yield return new WaitForSeconds(time);
    raceStart.stationPanel.transform.position = new Vector3(raceStart.stationPanel.transform.position.x, 0, 0);
    Debug.LogWarning("STt in corr");
    UIButton[] enableButtons = raceStart.stationPanel.GetComponentsInChildren<UIButton>();
    foreach (var eb in enableButtons)
    {
      eb.isEnabled = true;
    }
  }

  private IEnumerator ShowGameMenu(float time)
  {
    yield return new WaitForSeconds(time);
    gamePanel.transform.position = Vector3.zero;
    gamePanel.alpha = 1;
    UIButton[] enableButtons = gamePanel.GetComponentsInChildren<UIButton>();
    foreach (var eb in enableButtons)
    {
      eb.isEnabled = true;
    }
  }

  private void GoToGarage()
  {
    cameraGarage.SetActive(true);
    carcameras.gameObject.SetActive(false);
    character.transform.position = carGaragePos.position;
    character.transform.rotation = carGaragePos.rotation;
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();  //Находим грузовик
    if (tractor != null)
    {
      tractor.transform.localPosition = Vector3.zero;
    }
    character.transform.parent = podium;
  }
}
