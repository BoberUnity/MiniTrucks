using UnityEngine;

public class SetActiveObjs : MonoBehaviour 
{
  [SerializeField] private GameObject[] activateObjs = null;
  [SerializeField] private GameObject[] deactivateObjs = null;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      foreach (var obj in activateObjs)
      {
        obj.SetActive(true);
      }

      foreach (var obj in deactivateObjs)
      {
        obj.SetActive(false);
      }
    }
  }
}
