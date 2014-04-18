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
    //gamePanel.gameObject.SetActive(true);
    //gamePanel.enabled = true;
    axisCarController.InStation = false;
    //int i = 0;
    foreach (var enemyPos in enemyiesPos)
    {
      Instantiate(enemyPref, enemyPos.position, enemyPos.rotation);
      //i += 1;
    }
  }
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Traktor")
    {
      axisCarController = other.gameObject.GetComponent<AxisCarController>();
      axisCarController.InStation = true;
      //gamePanel.gameObject.SetActive(false);
      stationPanel.enabled = true;
    }
  }
}
