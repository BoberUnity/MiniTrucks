using System;
using System.Collections;
using UnityEngine;

public class SelectCarController : MonoBehaviour
{
  [Serializable] private class EnemyCar
  {
    [SerializeField] private GameObject carPrefab = null;
    [SerializeField] private string truckName = "";
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

    public string TruckName
    {
      get { return truckName;}
    }
  }
  
  [SerializeField] private ButtonHandler buttonNextCar = null;
  [SerializeField] private ButtonHandler buttonPrevCar = null;
  [SerializeField] private ButtonHandler buttonBuyCar = null;
  [SerializeField] private ButtonHandler buttonUpgradeCar = null;
  [SerializeField] private ButtonHandler buttonRace = null;
  [SerializeField] private ButtonHandler buttonRace2 = null;
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
  [SerializeField] private UILabel priceIndicator = null;
  [SerializeField] private UILabel nameIndicator = null;
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
    buttonUpgradeCar.Pressed += SelectCar;
    buttonRace.Pressed += Race;
    buttonRace2.Pressed += Race;
    goToGarageButtons = FindObjectsOfType(typeof(ButtonGoToGarage)) as ButtonGoToGarage[];//Кнопки переходи в гараж
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed += GoToGarage;
    }
    buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
    character = Instantiate(enemyCar[0].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
      character.transform.parent = podium;
    buttonUpgradeCar.gameObject.SetActive(false);
    buttonRace2.gameObject.SetActive(false);
    buttonBuyCar.gameObject.SetActive(true);
    priceIndicator.text = enemyCar[0].Price.ToString("f0");
    nameIndicator.text = enemyCar[0].TruckName;
	}

  private void OnDestroy()
  {
    buttonNextCar.Pressed -= NextCar;
    buttonPrevCar.Pressed -= PrevCar;
    buttonBuyCar.Pressed -= BuyCar;
    buttonUpgradeCar.Pressed -= SelectCar;
    buttonRace.Pressed -= Race;
    buttonRace2.Pressed -= Race;
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed -= GoToGarage;
    }
  }

  private void NextCar() 
  {
	  currentCar += 1;
    if (currentCar > enemyCar.Length - 1)
      currentCar = 0;
    ChangeCar();
  }

  private void PrevCar()
  {
    currentCar -= 1;
    if (currentCar < 0)
      currentCar = enemyCar.Length - 1;
    ChangeCar();
  }

  private void ChangeCar()
  {
    Destroy(character);
    character = Instantiate(enemyCar[currentCar].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
    }
    buttonUpgradeCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonRace2.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
    priceIndicator.text = enemyCar[currentCar].Price.ToString("f0");
    nameIndicator.text = enemyCar[currentCar].TruckName;
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
    buttonUpgradeCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonRace2.gameObject.SetActive(enemyCar[currentCar].HasBought);
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
