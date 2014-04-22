using System;
using UnityEngine;

public class WayFinish : MonoBehaviour
{
  [SerializeField] private UIPanel finishPanel = null;
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UILabel resultLabel = null;
  private AxisCarController axisCarController = null;
  public int prize = 1;
  public event Action Finish;
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "TraktorEnemy")
    {
      prize += 1;
    }

    if (other.gameObject.name == "Traktor")
    {
      //axisCarController = other.gameObject.GetComponent<AxisCarController>();
      //axisCarController.InStation = true;
      finishPanel.enabled = true;
      gamePanel.alpha = 0;
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
      var handler = Finish;
      if (handler != null)
        Finish();
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
