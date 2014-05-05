using System;
using System.Collections;
using UnityEngine;

public class SelectCarController : MonoBehaviour
{
  [Serializable] private class EnemyCar
  {
    [SerializeField] private GameObject carPrefab = null;
    [SerializeField] private int price = 10000;
    private bool hasBought = false;

    public GameObject CarPrefab
    {
      get { return carPrefab;}
    }

    public int Price
    {
      get { return price;}
    }

    public bool HasBought
    {
      get { return hasBought;}
      set { hasBought = value;}
    }
  }
  
  [SerializeField] private ButtonHandler buttonNextCar = null;
  [SerializeField] private ButtonHandler buttonPrevCar = null;
  [SerializeField] private ButtonHandler buttonBuyCar = null;
  [SerializeField] private ButtonHandler buttonSelectCar = null;
  [SerializeField] private ButtonHandler buttonRace = null;
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private GameObject cameraGarage = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private ButtonThrottle buttonBrake = null;
  [SerializeField] private ButtonThrottle buttonNitro = null;
  [SerializeField] private ButtonTuning buttonTuningEng = null;
  [SerializeField] private ButtonTuning buttonTuningHand = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private Map map = null;
  [SerializeField] private EnemyCar[] enemyCar = null;
  [SerializeField] private Transform carGaragePos = null;
  [SerializeField] private Transform podium = null;
  [SerializeField] private Transform carLevelPos = null;
  [SerializeField] private int gold = 50000;
  [SerializeField] private ButtonGoToGarage[] goToGarageButtons = null;//Кнопка перехода в гараж из меню станции
  [SerializeField] private ButtonAddTrailer[] buttonsAddTrailer = null;
  public event Action<int> ChangeGold;

  public int Gold
  {
    set
    {
      gold = value;
      var handler = ChangeGold;
      if (handler != null)
        handler(gold);
    }
    get { return gold; }
  }
  
  private GameObject character = null;
  private int currentCar = 0;
  public RaceStart raceStart = null;

  private void Start()
	{
    buttonNextCar.Pressed += NextCar;
    buttonPrevCar.Pressed += PrevCar;
    buttonBuyCar.Pressed += BuyCar;
    buttonSelectCar.Pressed += SelectCar;
    buttonRace.Pressed += Race;
    goToGarageButtons = FindObjectsOfType(typeof(ButtonGoToGarage)) as ButtonGoToGarage[];//Кнопки переходи в гараж
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed += GoToGarage;
    }
    buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
    character = Instantiate(enemyCar[0].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
      character.transform.parent = podium;
    buttonSelectCar.gameObject.SetActive(false);
    buttonBuyCar.gameObject.SetActive(true);
	}

  private void OnDestroy()
  {
    buttonNextCar.Pressed -= NextCar;
    buttonPrevCar.Pressed -= PrevCar;
    buttonBuyCar.Pressed -= BuyCar;
    buttonSelectCar.Pressed -= SelectCar;
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
    if (currentCar > enemyCar.Length - 1)
      currentCar = 0;
    character = Instantiate(enemyCar[currentCar].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
    }
    buttonSelectCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
	}

  private void PrevCar()
  {
    Destroy(character);
    currentCar -= 1;
    if (currentCar < 0)
      currentCar = enemyCar.Length - 1;
    character = Instantiate(enemyCar[currentCar].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
    }
    buttonSelectCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
  }

  private void BuyCar()
  {
    if (!enemyCar[currentCar].HasBought && gold > enemyCar[currentCar].Price)
    {
      enemyCar[currentCar].HasBought = true;
      Gold -= enemyCar[currentCar].Price;
      StartCoroutine(ActivateSelectButton(1));//Активируем кнопку Select через 1 сек после покупки
      buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
    }
  }

  private IEnumerator ActivateSelectButton(float time)
  {
    yield return new WaitForSeconds(time);
    buttonSelectCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
  }

  private void SelectCar()//Выбор 
  {
    
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
