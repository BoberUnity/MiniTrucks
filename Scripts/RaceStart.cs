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
  //[SerializeField] private UIPanel stationFinishPanel = null;
  [SerializeField] private int id = 0;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UIPanel finishPanel = null;
  [SerializeField] private UILabel resultLabel = null;
  [SerializeField] private UILabel addGoldLabel = null;
  [SerializeField] private SelectCarController selectCarController = null;
  [SerializeField] private ButtonHandler buttonOk = null;
  [SerializeField] private StartClock startClock = null;
  [SerializeField] private Transform truckPos = null;

  public AxisCarController axisCarController = null;
  public EnemiesPosition[] enemiesPos = null;
  public Transform CharPos = null;
  public int prize = 1;
  public event Action Finish;
  private bool activ = false;//true, когда едет эту гонку
  private int price = 0;
  
  public int Price
  {
    set { price = value; }
  }
  
  public bool Activ
  {
    set { activ = value; }
    get { return activ; }
  }

  public AxisCarController TractorAxisCarController
  {
    set { axisCarController = value; }
  }

  private void Start()
  {
    buttonOk.Pressed += ExitFinishMenu;
  }

  private void OnDestroy()
  {
    buttonOk.Pressed -= ExitFinishMenu;
  }

  public void ExitStation()
  {
    stationPanel.animation.Play();
    UIButton[] disableButtons = stationPanel.GetComponentsInChildren<UIButton>();
    foreach (var db in disableButtons)
    {
      db.isEnabled = false;
    }
    gamePanel.alpha = 1;
  }

  public void StartRace()
  {
    axisCarController.InStation = false;
  }

  public void ClockOn()
  {
    startClock.ClockOn();
    StartCoroutine(ClockOff(4));
  }

  private IEnumerator ClockOff(float time)//Часы убрали 0
  {
    yield return new WaitForSeconds(time);
    StartRace();
  }
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "TraktorEnemy")
    {
      if (activ)//finish
      {
        prize += 1;
      }
    }
    
    if (other.gameObject.name == "Traktor")//+ при переходе из гаража
    {
      PlayerPrefs.SetInt("StartCarPos", id);
      selectCarController.raceStart = this;
      truckPos.position = other.transform.position;
      if (other.GetComponent<CharacterJoint>() == null)
      {
        axisCarController = other.gameObject.GetComponent<AxisCarController>();
        axisCarController.InStation = true;
        gamePanel.alpha = 0;
        //stationPanel.transform.position = new Vector3(stationPanel.transform.position.x, 0, 0);
        //Debug.LogWarning("StPan");
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
            axisCarController = other.gameObject.GetComponent<AxisCarController>();
            axisCarController.InStation = true;
            finishPanel.transform.position = Vector3.zero;
            gamePanel.alpha = 0;
            price = (int)(price*other.gameObject.GetComponent<BlowController>().Condition/100);
            if (prize == 1)
            {
              resultLabel.text = "1-st";
            }
            if (prize == 2)
            {
              resultLabel.text = "2-nd";
              price = (int)(price*0.7f);
            }
            if (prize == 3)
            {
              resultLabel.text = "3-rd";
              price = (int)(price * 0.6f);
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

  private void ExitFinishMenu()//Нажатие кнопки ОК
  {
    if (axisCarController != null)//только для нужного пути
    {
      var handler = Finish;//Уничтожение соперников в ButtonAddTrailer
      if (handler != null)
        Finish();
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

    if (activ)//выполняется для города-финиша, показать меню
    {
      activ = false;
      if (axisCarController != null)
      axisCarController.InStation = true;
      else Debug.LogError("axisCarController == null");
      gamePanel.alpha = 0;
      StartCoroutine(ShowStationMenu(finishPanel.animation.clip.length));
      UIButton[] enableButtons = stationPanel.GetComponentsInChildren<UIButton>();
      foreach (var eb in enableButtons)
      {
        eb.isEnabled = true;
      }
    }
  }

  private IEnumerator ShowStationMenu(float time)
  {
    yield return new WaitForSeconds(time);
    stationPanel.transform.position = new Vector3(stationPanel.transform.position.x, 0, 0);
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
