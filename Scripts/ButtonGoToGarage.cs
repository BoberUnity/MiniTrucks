using System.Collections;
using UnityEngine;

public class ButtonGoToGarage : MonoBehaviour 
{
  [SerializeField] private UIWidget disabledPanel = null;
  [SerializeField] private UIPanel enabledPanel = null;
  [SerializeField] private SelectCarController selectCarController = null;
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      if (disabledPanel.animation != null)
      {
        disabledPanel.animation.Play();
        StartCoroutine(ChangePanel(disabledPanel.animation.clip.length));
      }
      else//Панель паузы
      {
        StartCoroutine(ChangePanel(0));
        disabledPanel.alpha = 0;
      }
      UIButton[] disableButtons = disabledPanel.GetComponentsInChildren<UIButton>();
      foreach (var db in disableButtons)
      {
        db.isEnabled = false;
      }
      selectCarController.StationPanel = transform.parent.GetComponent<UIWidget>();
    }
	}

  private IEnumerator ChangePanel(float time)
  {
    yield return new WaitForSeconds(time);
    //disabledPanel.enabled = false;
    //enabledPanel.enabled = true;
    //enabledPanel.alpha = 1;
    enabledPanel.transform.position = Vector3.zero;
    enabledPanel.alpha = 1;
    UIButton[] enableButtons = enabledPanel.GetComponentsInChildren<UIButton>();
    foreach (var eb in enableButtons)
    {
      eb.isEnabled = true;
    }
  }
}

