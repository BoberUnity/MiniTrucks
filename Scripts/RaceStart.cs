using UnityEngine;

public class RaceStart : MonoBehaviour
{
  [SerializeField] private UIPanel stationPanel = null;
  [SerializeField] private UIPanel gamePanel = null;
  private AxisCarController axisCarController = null;
  [SerializeField] private GameObject enemyPref = null;
  [SerializeField] private Transform[] enemyiesPos = null;

  public void ExitStation()
  {
    stationPanel.enabled = false;
    gamePanel.alpha = 1;
    axisCarController.InStation = false;
    foreach (var enemyPos in enemyiesPos)
    {
      Instantiate(enemyPref, enemyPos.position, enemyPos.rotation);
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
}
