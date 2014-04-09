using System.Collections;
using UnityEngine;

public class ButtonChangePanel : MonoBehaviour 
{
  [SerializeField] private UIPanel disabledPanel = null;
  [SerializeField] private UIPanel enabledPanel = null;
  [SerializeField] private UIButton[] disableButtons = null;
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      disabledPanel.animation.Play();
      StartCoroutine(ChangePanel(disabledPanel.animation.clip.length));
      foreach (var db in disableButtons)
      {
        db.isEnabled = false;
      }
    }
	}

  private IEnumerator ChangePanel(float time)
  {
    yield return new WaitForSeconds(time);
    disabledPanel.enabled = false;
    enabledPanel.enabled = true;
  }
}

