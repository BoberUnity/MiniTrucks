using UnityEngine;

public class ButtonSelectButtons : MonoBehaviour 
{
  [SerializeField] private UIButton[] disableButtons = null;
  [SerializeField] private UIButton[] enableButtons = null;
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      foreach (var db in disableButtons)
      {
        db.gameObject.SetActive(false);
        db.isEnabled = true;
      }
      foreach (var eb in enableButtons)
      {
        eb.gameObject.SetActive(true);
        eb.isEnabled = true;
      }
    }
	}
}

