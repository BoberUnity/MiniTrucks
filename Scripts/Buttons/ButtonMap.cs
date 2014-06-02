using UnityEngine;

public class ButtonMap : MonoBehaviour
{
  [SerializeField] private UIButton[] disableButtons = null;
  [SerializeField] private ButtonHandler map = null;
  
  private void Start()
  {
    map.Pressed += MapPressed;
  }

  private void OnDestroy()
  {
    map.Pressed -= MapPressed;
  }

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      foreach (var button in disableButtons)
      {
        button.isEnabled = false;
      }
      map.gameObject.SetActive(true);
    }
	}

  private void MapPressed()
  {
    foreach (var button in disableButtons)
    {
      button.isEnabled = true;
    }
    map.gameObject.SetActive(false);
  }
}

