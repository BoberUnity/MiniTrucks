using System;
using System.Collections;
using UnityEngine;

public class ButtonGoToGarage : MonoBehaviour 
{
  [SerializeField] private UIWidget disabledPanel = null;
  [SerializeField] private UIPanel enabledPanel = null;
  [SerializeField] private UIPanel infoPanel = null;
  [SerializeField] private SelectCarController selectCarController = null;
  public event Action Pressed;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      var handler = Pressed;//SelectCarController
      if (handler != null)
        Pressed();
      
      if (disabledPanel.animation != null)
      {
        disabledPanel.animation.Play();
        StartCoroutine(ChangePanel(disabledPanel.animation.clip.length));
      }
      
      UIButton[] disableButtons = disabledPanel.GetComponentsInChildren<UIButton>();
      foreach (var db in disableButtons)
      {
        db.isEnabled = false;
      }
      selectCarController.raceStart.stationPanel = transform.parent.GetComponent<UIWidget>();
    }
	}

  private IEnumerator ChangePanel(float time)
  {
    yield return new WaitForSeconds(time);
    enabledPanel.transform.position = Vector3.zero;
    enabledPanel.alpha = 1;
    enabledPanel.enabled = true;
    UIButton[] enableButtons = enabledPanel.GetComponentsInChildren<UIButton>();
    foreach (var eb in enableButtons)
    {
      eb.isEnabled = true;
    }
    infoPanel.transform.position = Vector3.zero;
    infoPanel.enabled = true;
  }
}

