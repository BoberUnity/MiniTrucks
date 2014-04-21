using UnityEngine;

public class RaceStart : MonoBehaviour
{
  [SerializeField] private UIPanel stationPanel = null;
  [SerializeField] private UIPanel gamePanel = null;
  private AxisCarController axisCarController = null;
  [SerializeField] private string way = "Way0";
  [SerializeField] private GameObject enemyPref = null;
  [SerializeField] private Transform[] enemyiesPos = null;

  public void ExitStation()
  {
    stationPanel.enabled = false;
    gamePanel.alpha = 1;
    axisCarController.InStation = false;
    foreach (var enemyPos in enemyiesPos)//Создаем соперников
    {
      GameObject opp = Instantiate(enemyPref, enemyPos.position, enemyPos.rotation) as GameObject;
      if (opp != null)
        opp.GetComponentInChildren<AxisCarController>().SetWay(way);
      else Debug.LogWarning("opp == null");
    }
  }
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Traktor")
    {
      axisCarController = other.gameObject.GetComponent<AxisCarController>();
      axisCarController.InStation = true;
      gamePanel.alpha = 0;
      stationPanel.enabled = true;
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
