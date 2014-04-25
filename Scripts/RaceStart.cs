using UnityEngine;

public class RaceStart : MonoBehaviour
{
  [SerializeField] private UIPanel stationPanel = null;
  [SerializeField] private UIPanel gamePanel = null;
  private AxisCarController axisCarController = null;
  public GameObject EnemyPref = null;
  public Transform[] EnemyiesPos = null;

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
