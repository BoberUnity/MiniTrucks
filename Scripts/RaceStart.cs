using System;
using System.Collections;
using UnityEngine;

public class RaceStart : MonoBehaviour
{
  [Serializable] public class EnemiesPosition
  {
    [SerializeField] private GameObject enemyPref = null;
    [SerializeField] private Transform enemyPos = null;

    public GameObject EnemyPref
    {
      get { return enemyPref; }
    }

    public Transform EnemyPos
    {
      get { return enemyPos; }
    }
  }
  
  public UIWidget stationPanel = null;
  [SerializeField] private int id = 0;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UIPanel finishPanel = null;
  [SerializeField] private UILabel resultLabel = null;
  [SerializeField] private UILabel addGoldLabel = null;
  [SerializeField] private UILabel hasMoneyLabel = null;
  [SerializeField] private SelectCarController selectCarController = null;
  [SerializeField] private ButtonHandler buttonOk = null;
  [SerializeField] private StartClock startClock = null;
  [SerializeField] private Transform truckPos = null;
  [SerializeField] private BaggageLabel baggageLabel = null;
  public GameObject MapCamera = null;
  public ButtonAddTrailer buttonAddTrailer = null;//Активная гонка

  public AxisCarController axisCarController = null;
  public EnemiesPosition[] enemiesPos = null;
  public Transform CharPos = null;
  public int prize = 1;
  public event Action Finish;
  [SerializeField]
  private bool activ = false;//true, когда едет эту гонку
  private int price = 0;
  [SerializeField] private string[] enemyNames = new string[4];
  public MapScroll mapScroll = null;
  
  public int Price
  {
    set { price = value; }
  }
  
  public bool Activ
  {
    set
    {
      activ = value;
      mapScroll.ActivFinis = activ ? id : -1;
    }
    get { return activ; }
  }

  private void Start()
  {
    buttonOk.Pressed += ExitFinishMenu;
    StartCoroutine(DisableStationPanel(0.3f));
  }

  private IEnumerator DisableStationPanel(float time)//Часы убрали 0
  {
    yield return new WaitForSeconds(time);
    SetPanelState(false);
  }

  private void SetPanelState(bool state)
  {
    stationPanel.gameObject.SetActive(state);
  }

  private void OnDestroy()
  {
    buttonOk.Pressed -= ExitFinishMenu;
  }

  public void ExitStation()//Нажатие кнопки гонки ButtonAddTrailer
  {
    stationPanel.animation.Play();
    UIButton[] disableButtons = stationPanel.GetComponentsInChildren<UIButton>();
    foreach (var db in disableButtons)
    {
      db.isEnabled = false;
    }
    gamePanel.alpha = 1;
    gamePanel.enabled = true;
    MapCamera.SetActive(true);
    StartCoroutine(DisableStationPanel(stationPanel.animation.clip.length+0.2f));
  }

  public void StartRace()
  {
    axisCarController.InStation = false;
    selectCarController.NitroToFuel();//Cancel
  }

  public void ClockOn()
  {
    startClock.ClockOn();
    StartCoroutine(ClockOff(1));//Должно быть 4 сек!!!!!!!!!!!!!!!!!!!!
  }

  private IEnumerator ClockOff(float time)//Часы убрали 0
  {
    yield return new WaitForSeconds(time);
    StartRace();
  }
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name.Substring(0, 2) == "TE")
    {
      if (activ)//finish
      {
        bool first = true;//Чтобы один и тот же грузовик не въехал 2 раза на финиш
        foreach (var nam in enemyNames)
        {
          if (other.gameObject.name == nam)
            first = false;
        }
        if (first)
        {
          enemyNames[prize - 1] = other.gameObject.name;
          prize += 1;
        }
      }
    }
    
    if (other.gameObject.name == "Traktor")//+ при переходе из гаража
    {
      PlayerPrefs.SetInt("StartCarPos", id);
      Debug.LogWarning("Tractor collider Enter");
      if (other.GetComponent<CharacterJoint>() == null)
      {
        selectCarController.raceStart = this;
        truckPos.position = other.transform.position;
        MapCamera.SetActive(false);
        SetPanelState(true);
        axisCarController = other.gameObject.GetComponent<AxisCarController>();
        axisCarController.InStation = true;
        gamePanel.alpha = 0;
        gamePanel.enabled = false;
        StartCoroutine(ShowStationMenu(finishPanel.animation.clip.length));
  
        UIButton[] enableButtons = stationPanel.GetComponentsInChildren<UIButton>();
        foreach (var eb in enableButtons)
        {
          eb.isEnabled = true;
        }
      }
      else
      { 
        if (activ)//finish
        {
          if (other.gameObject.GetComponent<CharacterJoint>() != null)
          {
            selectCarController.raceStart = this;
            truckPos.position = other.transform.position;
            MapCamera.SetActive(false);
            SetPanelState(true);
            axisCarController = other.gameObject.GetComponent<AxisCarController>();
            axisCarController.InStation = true;
            finishPanel.transform.position = Vector3.zero;
            finishPanel.enabled = true;
            gamePanel.alpha = 0;
            gamePanel.enabled = false;
            price = (int)(price*other.gameObject.GetComponent<BlowController>().Condition/100);
            if (prize == 1)
            {
              resultLabel.text = "1-st";
              buttonAddTrailer.Medal = 0;
            }
            if (prize == 2)
            {
              resultLabel.text = "2-nd";
              price = (int)(price*0.7f);
              buttonAddTrailer.Medal = Mathf.Min(1, buttonAddTrailer.Medal);
            }
            if (prize == 3)
            {
              resultLabel.text = "3-rd";
              price = (int)(price * 0.6f);
              buttonAddTrailer.Medal = Mathf.Min(2, buttonAddTrailer.Medal);
            }
            if (prize == 4)
            {
              resultLabel.text = "4-th";
              price = (int)(price * 0.5f);
            }
            if (prize == 5)
            {
              resultLabel.text = "5-th";
              price = (int)(price * 0.4f);
            }
            addGoldLabel.text = price.ToString("f0");
            
            buttonOk.GetComponent<UIButton>().isEnabled = true;
            StartCoroutine(AddGold(1));
            prize = 1;
            for (int i = 0; i < enemyNames.Length; i++)
            {
              enemyNames[i] = "";
            }
            baggageLabel.bonusPosCtrl.DeleteBonuses();
            BlowController blowController = other.gameObject.GetComponent<BlowController>();
            blowController.DestroyBoxes();
            hasMoneyLabel.text = blowController.HasMoney.ToString("f0");
            StartCoroutine(AddGoldForMoney(blowController.HasMoney, 2.5f));
            blowController.HasMoney = 0;
          }
        }
      }
    }
  }

  private IEnumerator AddGold(float time)
  {
    yield return new WaitForSeconds(time);
    selectCarController.Gold += price;
  }

  private IEnumerator AddGoldForMoney(int nums, float time)
  {
    yield return new WaitForSeconds(time);
    selectCarController.Gold += nums*100;//Цена за 1 монетку
    hasMoneyLabel.text = "0";
  }

  private void ExitFinishMenu()//Нажатие кнопки ОК
  {
    if (axisCarController != null)//только для нужного пути
    {
      var handler = Finish;//Уничтожение соперников в ButtonAddTrailer
      if (handler != null)
        handler();
      //Удаление прицепа
      CharacterJoint characterJoint = axisCarController.GetComponent<CharacterJoint>();
      if (characterJoint != null)
      {
        Destroy(characterJoint.connectedBody.gameObject.transform.parent.parent.gameObject);
        Destroy(characterJoint);
      }
      else Debug.LogWarning("CharacterJoint == null");
      finishPanel.animation.Play();
    }

    if (Activ)//выполняется для города-финиша, показать меню
    {
      Activ = false;
      if (axisCarController != null)
      axisCarController.InStation = true;
      else Debug.LogError("axisCarController == null");
      gamePanel.alpha = 0;
      gamePanel.enabled = false;
      SetPanelState(true);
      StartCoroutine(ShowStationMenu(finishPanel.animation.clip.length));
      UIButton[] enableButtons = stationPanel.GetComponentsInChildren<UIButton>();
      foreach (var eb in enableButtons)
      {
        eb.isEnabled = true;
      }
      baggageLabel.BaggageController = null;
    }
  }

  private IEnumerator ShowStationMenu(float time)
  {
    yield return new WaitForSeconds(time);
    stationPanel.transform.position = new Vector3(stationPanel.transform.position.x, 0, 0);
    finishPanel.enabled = false;
  }

  void OnDrawGizmos()
  {
    var points = gameObject.GetComponentsInChildren<Transform>();

    foreach (Transform point in points)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(point.position, 1.0f);
    }
  }
}
