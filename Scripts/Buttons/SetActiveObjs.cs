using UnityEngine;

public class SetActiveObjs : MonoBehaviour 
{
  [SerializeField] private GameObject[] activateObjs = null;
  [SerializeField] private GameObject[] deactivateObjs = null;
  [SerializeField] private bool acsel = false;

  /*protected virtual*/public void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      foreach (var obj in activateObjs)
      {
        obj.SetActive(true);
        if (obj.GetComponent<Steer>() != null)
          obj.GetComponent<Steer>().Acsel = acsel;
      }

      foreach (var obj in deactivateObjs)
      {
        obj.SetActive(false);
      }
    }
  }
}
