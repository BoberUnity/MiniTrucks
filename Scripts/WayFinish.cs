using System;
using UnityEngine;

public class WayFinish : MonoBehaviour
{
  [SerializeField] private UIPanel finishPanel = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UILabel resultLabel = null;
  [SerializeField] private ButtonHandler buttonOk = null;
  private AxisCarController axisCarController = null;
  public int prize = 1;
  private bool activ = false;//true, когда едет эту гонку
  public event Action Finish;
  
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

  private void OnTriggerEnter(Collider other)
  {
    if (activ)
    {
      if (other.gameObject.name == "TraktorEnemy")
      {
        prize += 1;
      }

      if (other.gameObject.name == "Traktor")
      {
        if (other.gameObject.GetComponent<CharacterJoint>() != null)
        {
          axisCarController = other.gameObject.GetComponent<AxisCarController>();
          axisCarController.InStation = true;
          finishPanel.transform.position = Vector3.zero;
          gamePanel.alpha = 0;
          activ = false;
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
          prize = 0;
        }
      }
    }
  }

  private void ExitFinishMenu()
  {
    var handler = Finish;//Уничтожение соперников в ButtonAddTrailer
    if (handler != null)
      Finish();
    if (axisCarController != null)//только для нужного пути
    {
      axisCarController.InStation = false;
      //Удаление прицепа
      CharacterJoint characterJoint = axisCarController.GetComponent<CharacterJoint>();
      if (characterJoint != null)
      {
        Destroy(characterJoint.connectedBody.gameObject);
        Destroy(characterJoint);
      }
      else Debug.LogWarning("CharacterJoint == null");
      finishPanel.animation.Play();
      axisCarController = null;
    }
  }

  //void OnDrawGizmos()
  //{
  //  var points = gameObject.GetComponentsInChildren<Transform>();

  //  foreach (Transform point in points)
  //  {
  //    Gizmos.color = Color.red;
  //    Gizmos.DrawSphere(point.position, 1.0f);
  //  }
  //}
}
