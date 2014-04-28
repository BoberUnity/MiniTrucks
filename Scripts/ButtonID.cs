using UnityEngine;

public class ButtonID : MonoBehaviour
{
  [SerializeField] private int id = 0;
  [SerializeField] private UIPanel optionsPanel = null;
  [SerializeField] private UIPanel pausePanel = null;
  //[SerializeField] private ButtonAddTrailer[] buttonsAddTrailer = null;
  
  private void Start()
  {
    if (id == 0)
      GetComponent<UIButton>().isEnabled = false;
  }

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      if (id == 1)
        Application.Quit();
      if (id == 2)//Back in options menu to pause menu
      {
        optionsPanel.transform.position = -Vector3.up*800;
        pausePanel.alpha = 1;
        UIButton[] enableButtons = pausePanel.GetComponentsInChildren<UIButton>();
        foreach (var eb in enableButtons)
        {
          eb.isEnabled = true;
        }
      }

      if (id == 3)//Exit Race
      {
        ButtonAddTrailer[] buttonsAddTrailer = FindObjectsOfType(typeof(ButtonAddTrailer)) as ButtonAddTrailer[];
        foreach (var button in buttonsAddTrailer)
        {
          button.ExitRace();
        }
      }
    }
	}
}

