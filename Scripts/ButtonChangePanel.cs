using UnityEngine;

public class ButtonChangePanel : MonoBehaviour 
{
  [SerializeField] private UIPanel disabledPanel = null;
  [SerializeField] private UIPanel enabledPanel = null;
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      disabledPanel.enabled = false;
      enabledPanel.enabled = true;
    }
	}
}

