using System;
using UnityEngine;

public class RaceStart : MonoBehaviour
{
  [SerializeField] private UIPanel stationPanel = null;
  //[SerializeField] private UIPanel stationFinishPanel = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UIPanel finishPanel = null;
  [SerializeField] private UILabel resultLabel = null;
  [SerializeField] private ButtonHandler buttonOk = null;
  private AxisCarController axisCarController = null;
  public GameObject EnemyPref = null;
  public Transform[] EnemyiesPos = null;
  public int prize = 1;
  public event Action Finish;
  private bool activ = false;//true, когда едет эту гонку
  
  public bool Activ
  {
    set { activ = value; }
    get { return activ; }
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
    axisCarController.InStation = false;
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
    
    if (other.gameObject.name == "Traktor")
    {
      if (other.GetComponent<CharacterJoint>() == null)
      {
        axisCarController = other.gameObject.GetComponent<AxisCarController>();
        axisCarController.InStation = true;
        gamePanel.alpha = 0;
        stationPanel.transform.position = Vector3.zero;
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
          if (other.gameObject.name == "Traktor")
          {
            if (other.gameObject.GetComponent<CharacterJoint>() != null)
            {
              axisCarController = other.gameObject.GetComponent<AxisCarController>();
              axisCarController.InStation = true;
              finishPanel.transform.position = Vector3.zero;
              gamePanel.alpha = 0;
              //activ = false;
              if (prize == 1)
                resultLabel.text = "1-st";
              if (prize == 2)
                resultLabel.text = "2-nd";
              if (prize == 3)
                resultLabel.text = "3-rd";
              if (prize == 4)
                resultLabel.text = "4-th";
              if (prize == 5)
                resultLabel.text = "5-th";
              prize = 1;
              buttonOk.GetComponent<UIButton>().isEnabled = true;
            }
          }
        }
      }
    }
  }

  private void ExitFinishMenu()//Нажатие кнопки ОК
  {
    if (axisCarController != null)//только для нужного пути
    {
      var handler = Finish;//Уничтожение соперников в ButtonAddTrailer
      if (handler != null)
        Finish();
      //axisCarController.InStation = false;
      //Удаление прицепа
      CharacterJoint characterJoint = axisCarController.GetComponent<CharacterJoint>();
      if (characterJoint != null)
      {
        Destroy(characterJoint.connectedBody.gameObject);
        Destroy(characterJoint);
      }
      else Debug.LogWarning("CharacterJoint == null");
      finishPanel.animation.Play();
    }

    if (activ)//выполняется для города-финиша, показать меню
    {
      activ = false;
      axisCarController.InStation = true;
      gamePanel.alpha = 0;
      stationPanel.transform.position = Vector3.zero;
      UIButton[] enableButtons = stationPanel.GetComponentsInChildren<UIButton>();
      foreach (var eb in enableButtons)
      {
        eb.isEnabled = true;
      }
    }
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
