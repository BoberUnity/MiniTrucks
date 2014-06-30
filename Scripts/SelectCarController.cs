using System;
using System.Collections;
using UnityEngine;

public class SelectCarController : MonoBehaviour
{
  [Serializable] public class EnemyCar
  {
    [SerializeField] private GameObject carPrefab = null;
    [SerializeField] private string truckName = "";
    [SerializeField] private int price = 10000;
    [SerializeField] private int maxSpeed = 80;
    public Material[] Materials = null;
    [SerializeField] private int tunSpeed = 0;
    [SerializeField] private int tunBrake = 0;
    [SerializeField] private int tunHandling = 0;
    [SerializeField] private float maxPower = 100;
    [SerializeField] private float maxTorque = 300;
    [SerializeField] private float brake = 2500;
    [SerializeField] private float sideways = 1;

    private int currentMaterial = 0;
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

    public int CurrentMaterial
    {
      get { return currentMaterial; }
      set { currentMaterial = value; }
    }

    public int MaxSpeed
    {
      get { return maxSpeed; }
    }

    public int TunSpeed
    {
      get { return tunSpeed; }
      set { tunSpeed = value; }
    }

    public int TunBrake
    {
      get { return tunBrake; }
      set { tunBrake = value; }
    }

    public int TunHandling
    {
      get { return tunHandling; }
      set { tunHandling = value; }
    }

    public float MaxPower
    {
      get { return maxPower; }
    }

    public float MaxTorque
    {
      get { return maxTorque; }
    }

    public float Brake
    {
      get { return brake; }
    }

    public float Sideways
    {
      get { return sideways; }
    }
  }

  [SerializeField] private ButtonHandler buttonStart = null;
  [SerializeField] private ButtonHandler buttonContinue = null;
  [SerializeField] private ButtonHandler buttonNextCar = null;
  [SerializeField] private ButtonHandler buttonPrevCar = null;
  [SerializeField] private ButtonHandler buttonBuyCar = null;
  [SerializeField] private ButtonHandler buttonUpgradeCar = null;
  [SerializeField] private ButtonHandler buttonRace = null;
  [SerializeField] private ButtonHandler buttonRace2 = null;
  [SerializeField] private ButtonHandler buttonRestart = null;
  [SerializeField] private ButtonHandler buttonPaint = null;
  [SerializeField] private ButtonRul buttonRulRight = null;
  [SerializeField] private ButtonRul buttonRulLeft = null;
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private GameObject cameraGarage = null;
  [SerializeField] private GameObject cameraMap = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private ButtonBrake buttonBrake = null;
  [SerializeField] private ButtonThrottle buttonNitro = null;
  [SerializeField] private Strelki strelki = null;
  [SerializeField] private ButtonTuning buttonTuningEng = null;
  [SerializeField] private ButtonTuning buttonTuningHand = null;
  [SerializeField] private ButtonTuning buttonTuningBrake = null;
  //[SerializeField] private ButtonTuning buttonTuningMaxSpeed = null;
  [SerializeField] private UILabel maxSpeedIndicator = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UIPanel gragePanel = null;
  [SerializeField] private UIPanel infoPanel = null;
  [SerializeField] private MapScroll map = null;
  [SerializeField] public EnemyCar[] enemyCar = null;
  [SerializeField] private Transform carGaragePos = null;
  [SerializeField] private Transform podium = null;
  [SerializeField] private Transform[] carLevelPos = null;//0-busy; 1-fish; 2-oil; 3-west
  [SerializeField] private int gold = 50000;
  [SerializeField] private UILabel priceIndicator = null;
  [SerializeField] private UILabel nameIndicator = null;
  [SerializeField] private UILabel powerLabel = null;
  [SerializeField] private UILabel handlingLabel = null;
  [SerializeField] private UILabel brakeLabel = null;
  [SerializeField] private ButtonGoToGarage[] goToGarageButtons = null;//Кнопка перехода в гараж из меню станции
  public ButtonAddTrailer[] buttonsAddTrailer = null;
  private Vector3 beforeGaragePosition = Vector3.zero;
  private Quaternion beforeGarageRotation = Quaternion.identity;
  private bool first = true;//Первый запуск
  public float VolumeEffects = 0.5f;
  public float VolumeMusic = 0.5f;
  public event Action<int> ChangeGold;
  [SerializeField] private UILabel imeiLabel = null;

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
  public int currentCar = 0;
  public RaceStart raceStart = null;

  private void Start()
	{
    buttonNextCar.Pressed += NextCar;
    buttonPrevCar.Pressed += PrevCar;
    buttonBuyCar.Pressed += BuyCar;
    buttonRace.Pressed += Race;
    buttonRace2.Pressed += Race;
    buttonContinue.Pressed += Continue;
    buttonStart.Pressed += StartGame;
    buttonRestart.Pressed += Restart;
    buttonPaint.Pressed += PaintCar;
    goToGarageButtons = FindObjectsOfType(typeof(ButtonGoToGarage)) as ButtonGoToGarage[];//Кнопки переходи в гараж
    foreach (var butt in goToGarageButtons)
    {
      butt.Pressed += GoToGarage;
    }
    buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];

    if (Application.platform == RuntimePlatform.Android)
    {
      AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");
      imeiLabel.text = "IMEI:" + TM.Call<string>("getDeviceId");
    }
	}

  private void OnDestroy()
  {
    buttonNextCar.Pressed -= NextCar;
    buttonPrevCar.Pressed -= PrevCar;
    buttonBuyCar.Pressed -= BuyCar;
    buttonRace.Pressed -= Race;
    buttonRace2.Pressed -= Race;
    buttonContinue.Pressed -= Continue;
    buttonStart.Pressed -= StartGame;
    buttonRestart.Pressed -= Restart;
    buttonPaint.Pressed -= PaintCar;
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

  private void StartGame()
  {
    character = Instantiate(enemyCar[0].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      StartCoroutine(ReadRegParam(0.05f));
      SetTunningButtons(character.GetComponentInChildren<CameraTarget>());
    }
    buttonUpgradeCar.gameObject.SetActive(false);
    buttonRace2.gameObject.SetActive(false);
    buttonBuyCar.gameObject.SetActive(true);
    if (!enemyCar[currentCar].HasBought)
    {
      ResetTunning();
    }
    priceIndicator.text = enemyCar[0].Price.ToString("f0");
    nameIndicator.text = enemyCar[0].TruckName;
    StartCoroutine(UpdateMaxSpeedIndicator(0.05f));
  }

  private IEnumerator UpdateMaxSpeedIndicator(float time)
  {
    yield return new WaitForSeconds(time);
    maxSpeedIndicator.text = enemyCar[currentCar].MaxSpeed.ToString("f0");
  }

  private void ResetTunning()
  {
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();
    Drivetrain drivetrain = tractor.GetComponent<Drivetrain>();
    drivetrain.maxPower = enemyCar[currentCar].MaxPower;
    drivetrain.maxTorque = enemyCar[currentCar].MaxTorque;
    Axles axles = tractor.GetComponent<Axles>();
    axles.frontAxle.sidewaysGripFactor = enemyCar[currentCar].Sideways;
    axles.rearAxle.sidewaysGripFactor = enemyCar[currentCar].Sideways;
    axles.frontAxle.brakeFrictionTorque = enemyCar[currentCar].Brake;
    axles.rearAxle.brakeFrictionTorque = enemyCar[currentCar].Brake;
    foreach (Axle axle in axles.otherAxles)
    {
      axle.sidewaysGripFactor = enemyCar[currentCar].Sideways;
      axle.brakeFrictionTorque = enemyCar[currentCar].Brake;
    }
    //tractor.GetComponent<Setup>().SaveSetup();
    maxSpeedIndicator.text = enemyCar[currentCar].MaxSpeed.ToString("f0");
  }

  private void ChangeCar()
  {
    Destroy(character);
    Debug.LogWarning("Destroy(character); ChangeCar()");
    character = Instantiate(enemyCar[currentCar].CarPrefab, carGaragePos.position, Quaternion.identity) as GameObject;
    if (character != null)
    {
      character.transform.parent = podium;
      AxisCarController tractorACC = character.GetComponentInChildren<AxisCarController>();
      tractorACC.InStation = true;
      if (raceStart != null)//из меню станции
        raceStart.axisCarController = tractorACC;
      character.GetComponentInChildren<AxisCarController>().MaxSpeed = enemyCar[currentCar].MaxSpeed;
      SetTunningButtons(character.GetComponentInChildren<CameraTarget>());
    }
    buttonUpgradeCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonRace2.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonBuyCar.gameObject.SetActive(!enemyCar[currentCar].HasBought);
    priceIndicator.text = enemyCar[currentCar].Price.ToString("f0");
    nameIndicator.text = enemyCar[currentCar].TruckName;
    if (!enemyCar[currentCar].HasBought)
    {
      ResetTunning();
      ShowInfoMenu();
      StartCoroutine(UpdateMaxSpeedIndicator(0.05f));
      
    }
    else
      StartCoroutine(ReadRegParam(0.05f));
  }

  private IEnumerator ReadRegParam(float time)
  {
    yield return new WaitForSeconds(time);
    if (PlayerPrefs.HasKey("MaterialCar" + currentCar.ToString("f0")))//Если есть запись в реестре ставим материал
    {
      CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();
      tractor.BodyRenderer.material = enemyCar[currentCar].Materials[PlayerPrefs.GetInt("MaterialCar" + currentCar.ToString("f0"))];
    }
    
    if (PlayerPrefs.HasKey("TunSpeed" + currentCar.ToString("f0")))
    {
      enemyCar[currentCar].TunSpeed = PlayerPrefs.GetInt("TunSpeed" + currentCar.ToString("f0"));
      buttonTuningEng.TunStep = enemyCar[currentCar].TunSpeed;
    }
    else 
    {
      buttonTuningEng.TunStep = 0;
      maxSpeedIndicator.text = enemyCar[currentCar].MaxSpeed.ToString("f0");
      character.GetComponentInChildren<AxisCarController>().MaxSpeed = enemyCar[currentCar].MaxSpeed;
    }

    if (PlayerPrefs.HasKey("TunBrake" + currentCar.ToString("f0")))
    {
      enemyCar[currentCar].TunBrake = PlayerPrefs.GetInt("TunBrake" + currentCar.ToString("f0"));
      buttonTuningBrake.TunStep = enemyCar[currentCar].TunBrake;

    }
    else
      buttonTuningBrake.TunStep = 0;

    if (PlayerPrefs.HasKey("TunHandling" + currentCar.ToString("f0")))
    {
      enemyCar[currentCar].TunHandling = PlayerPrefs.GetInt("TunHandling" + currentCar.ToString("f0"));
      buttonTuningHand.TunStep = enemyCar[currentCar].TunHandling;
    }
    else
      buttonTuningHand.TunStep = 0;

    ShowInfoMenu();
  }

  private void ShowInfoMenu()
  {
    powerLabel.text = character.GetComponentInChildren<Drivetrain>().maxPower.ToString("f0");
    handlingLabel.text = character.GetComponentInChildren<Axles>().frontAxle.sidewaysGripFactor.ToString("f1");
    brakeLabel.text = character.GetComponentInChildren<Axles>().frontAxle.brakeFrictionTorque.ToString("f0");
    maxSpeedIndicator.text = character.GetComponentInChildren<AxisCarController>().MaxSpeed.ToString("f0");
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

  private void PaintCar()
  {
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();
    enemyCar[currentCar].CurrentMaterial += 1;
    if (enemyCar[currentCar].CurrentMaterial > 2)
      enemyCar[currentCar].CurrentMaterial = 0;
    tractor.BodyRenderer.material = enemyCar[currentCar].Materials[enemyCar[currentCar].CurrentMaterial];
    PlayerPrefs.SetInt("MaterialCar" + currentCar.ToString("f0"), enemyCar[currentCar].CurrentMaterial);
  }

  private IEnumerator ActivateSelectButton(float time)
  {
    yield return new WaitForSeconds(time);
    buttonUpgradeCar.gameObject.SetActive(enemyCar[currentCar].HasBought);
    buttonRace2.gameObject.SetActive(enemyCar[currentCar].HasBought);
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
      buttonRulRight.axisCarController = aCC;
      buttonRulLeft.axisCarController = aCC;
      buttonBrake.axisCarController = aCC;
      buttonNitro.axisCarController = aCC;
      strelki.drivetrain = tractor.GetComponent<Drivetrain>();

      map.Truck = tractor.transform;
      
      if (raceStart != null)//Если заходили в гагаж и меню станции
      {
        StartCoroutine(ShowStationMenu(gamePanel.animation.clip.length));//!!! Должно быть не gamePanel, a Upgrade Panel
        aCC.InStation = true;
      }
      else//При первом запуске или перезапуск из гонки активируем Game Menu
      {
        aCC.InStation = false;
        StartCoroutine(ShowGameMenu(gamePanel.animation.clip.length));
      }
    }
    gamePanel.enabled = true;
    cameraMap.SetActive(true);
    gragePanel.enabled = false;
    infoPanel.enabled = false;
  }

  private void SetTunningButtons(CameraTarget tractor)
  {
    buttonTuningEng.drivetrain = tractor.GetComponent<Drivetrain>();
    buttonTuningHand.axles = tractor.GetComponent<Axles>();
    buttonTuningHand.carDynamics = tractor.GetComponent<CarDynamics>();
    //buttonTuningHand.setup = tractor.GetComponent<Setup>();
    //buttonTuningEng.setup = tractor.GetComponent<Setup>();
    buttonTuningBrake.axles = tractor.GetComponent<Axles>();
    buttonTuningBrake.carDynamics = tractor.GetComponent<CarDynamics>();
    //buttonTuningBrake.setup = tractor.GetComponent<Setup>();
    //buttonTuningMaxSpeed.drivetrain = tractor.GetComponent<Drivetrain>();
    //buttonTuningEng.TunStep = enemyCar[currentCar].TunSpeed;
    //ostalnie?
  }

  private void Restart()
  {
    CameraTarget tractor = character.GetComponentInChildren<CameraTarget>();  //Находим грузовик
    if (tractor != null)
    {
      //ButtonAddTrailer[] buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
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

  public void DestroyCharacter()
  {
    Destroy(character);
    currentCar = 0;
    raceStart = null;
  }
}
