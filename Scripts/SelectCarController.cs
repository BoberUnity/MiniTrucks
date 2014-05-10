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

  [SerializeField] private ButtonHandler buttonContinue = null;
  [SerializeField] private ButtonHandler buttonNextCar = null;
  [SerializeField] private ButtonHandler buttonPrevCar = null;
  [SerializeField] private ButtonHandler buttonBuyCar = null;
  [SerializeField] private ButtonHandler buttonUpgradeCar = null;
  [SerializeField] private ButtonHandler buttonRace = null;
  [SerializeField] private ButtonHandler buttonRace2 = null;
  [SerializeField] private ButtonHandler buttonRestart = null;
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private GameObject cameraGarage = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private ButtonBrake buttonBrake = null;
  [SerializeField] private ButtonThrottle buttonNitro = null;
  [SerializeField] private ButtonTuning buttonTuningEng = null;
  [SerializeField] private ButtonTuning buttonTuningHand = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private Map map = null;
  [SerializeField] private EnemyCar[] enemyCar = null;
  [SerializeField] private Transform carGaragePos = null;
  [SerializeField] private Transform podium = null;
  [SerializeField] private Transform[] carLevelPos = null;
  [SerializeField] private int gold = 50000;
  [SerializeField] private UILabel priceIndicator = null;
  [SerializeField] private UILabel nameIndicator = null;
  [SerializeField] private UILabel powerLabel = null;
  [SerializeField] private ButtonGoToGarage[] goToGarageButtons = null;//Кнопка перехода в гараж из меню станции
  [SerializeField] private ButtonAddTrailer[] buttonsAddTrailer = null;
  private Vector3 beforeGaragePosition = Vector3.zero;
  private Quaternion beforeGarageRotation = Quaternion.identity;
  private bool first = true;//Первый запуск
  public event Action<int> ChangeGold;

  public int Gold
  {
    set
    {
      gold = value;
      PlayerPrefs.SetInt("Gold", gold);
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
    buttonContinue.Pressed += Continue;
    buttonRestart.Pressed += Restart;
    goToGarageButtons = FindObjectsOfType(typeof(ButtonGoToGarage)) as ButtonGoToGarage[];//Кнопки переходи в гараж
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed += GoToGarage;
    }
    buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
    character = Instantiate(enemyCar[0].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      //Transform tractorTransform = character.GetComponentInChildren<CameraTarget>().transform;
      //tractorTransform.position = carGaragePos.position;
      //tractorTransform.parent = podium;
      character.transform.parent = podium;
      powerLabel.text = character.GetComponentInChildren<Drivetrain>().maxPower.ToString("f0");
    }
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
    buttonContinue.Pressed -= Continue;
    buttonRestart.Pressed -= Restart;
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
      //Transform tractorTransform = character.GetComponentInChildren<CameraTarget>().transform;
      //tractorTransform.position = carGaragePos.position;
      //tractorTransform.parent = podium;
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
      powerLabel.text = character.GetComponentInChildren<Drivetrain>().maxPower.ToString("f0");
    }
    buttonUpgradeCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonRace2.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
    priceIndicator.text = enemyCar[currentCar].Price.ToString("f0");
    nameIndicator.text = enemyCar[currentCar].TruckName;
  }

  private void BuyCar()
  {
    if (!enemyCar[currentCar].HasBought && gold >= enemyCar[currentCar].Price)
    {
      enemyCar[currentCar].HasBought = true;
      Gold -= enemyCar[currentCar].Price;
      StartCoroutine(ActivateSelectButton(1));//Активируем кнопку Select через 1 сек после покупки
      buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
      PlayerPrefs.SetInt("HasCar" + currentCar.ToString("f0"), 1);
    }
  }

  private IEnumerator ActivateSelectButton(float time)
  {
    yield return new WaitForSeconds(time);
    buttonUpgradeCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonRace2.gameObject.SetActive(enemyCar[currentCar].HasBought);
  }

  private void SelectCar()//Выбор Upgrade
  {

  }

  private void Continue()
  {
    int i = 0;
    foreach (var ec in enemyCar)
    {
      if (PlayerPrefs.HasKey("HasCar" + i.ToString("f0")))
        ec.HasBought = true;
      i += 1;
    }
    
    if (PlayerPrefs.HasKey("CurrentCar"))
    {
      currentCar = PlayerPrefs.GetInt("CurrentCar");
      ChangeCar();
    }
    else Debug.LogWarning("Key was not found");

    if (PlayerPrefs.HasKey("Gold"))
    {
      Gold = PlayerPrefs.GetInt("Gold");
    }
    Race();
  }

  private void Race()//ButtonRace
  {
    PlayerPrefs.SetInt("CurrentCar", currentCar);
    character.transform.parent = null;
    if (PlayerPrefs.HasKey("StartCarPos"))
    {
      if (first)
      {
        character.transform.position = carLevelPos[PlayerPrefs.GetInt("StartCarPos")].position;
        character.transform.rotation = carLevelPos[PlayerPrefs.GetInt("StartCarPos")].rotation;
        Debug.Log("Start from " + PlayerPrefs.GetInt("StartCarPos") + " town");
      }
      else//Из гаража
      {
        character.transform.position = beforeGaragePosition;
        character.transform.rotation = beforeGarageRotation;
        Debug.Log("Start from Garage");
      }
    }
    else//Первый запуск
    {
      character.transform.position = carLevelPos[0].position;
      character.transform.rotation = carLevelPos[0].rotation;
      Debug.Log("First town");
    }
    //Следы
    Skidmarks skids = character.GetComponentInChildren<Skidmarks>();
    skids.gameObject.SetActive(true);
    skids.transform.position = Vector3.zero;
    skids.transform.rotation = Quaternion.identity;
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

  private void Restart()
  {
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();  //Находим грузовик
    if (tractor != null)
    {
      ButtonAddTrailer[] buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
      foreach (var button in buttonsAddTrailer)
      {
        button.ExitRace();
      }
      if (PlayerPrefs.HasKey("StartCarPos"))
      {
        if (first)
        {
          tractor.transform.position = carLevelPos[PlayerPrefs.GetInt("StartCarPos")].position;
          tractor.transform.rotation = carLevelPos[PlayerPrefs.GetInt("StartCarPos")].rotation;
        }
        else//Из гаража
        {
          tractor.transform.position = beforeGaragePosition;
          tractor.transform.rotation = beforeGarageRotation;
        }
      }
      else//Первый запуск
      {
        tractor.transform.position = carLevelPos[0].position;
        tractor.transform.rotation = carLevelPos[0].rotation;
      }
    }
  }

  private IEnumerator ShowStationMenu(float time)
  {
    yield return new WaitForSeconds(time);
    raceStart.stationPanel.transform.position = new Vector3(raceStart.stationPanel.transform.position.x, 0, 0);
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
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();  //Находим грузовик
    if (tractor != null)
    {
      beforeGaragePosition = tractor.transform.position;
      beforeGarageRotation = tractor.transform.rotation;
      first = false;
      cameraGarage.SetActive(true);
      carcameras.gameObject.SetActive(false);
      character.transform.position = carGaragePos.position;
      character.transform.rotation = carGaragePos.rotation;
      tractor.transform.localPosition = Vector3.zero;
      character.transform.parent = podium;
    }
  }
}
