using UnityEngine;

public class ButtonID : MonoBehaviour
{
  [SerializeField] private int id = 0;
  [SerializeField] private UIPanel optionsPanel = null;
  [SerializeField] private UIPanel pausePanel = null;
  [SerializeField] private SelectCarController selectCarController = null;
  [SerializeField] private GameObject[] delObjs = null;
  
  private void Start()
  {
    if (id == 0)
    {
      if (!PlayerPrefs.HasKey("StartCarPos"))
      {
        GetComponent<UIButton>().isEnabled = false;
      }
    }
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
        pausePanel.enabled = true;
        UIButton[] enableButtons = pausePanel.GetComponentsInChildren<UIButton>();
        foreach (var eb in enableButtons)
        {
          eb.isEnabled = true;
        }
      }

      if (id == 3)//
      {
        
      }

      if (id == 4)//ClearReg
      {
        PlayerPrefs.DeleteAll();
        //medals off
        foreach (var button in selectCarController.buttonsAddTrailer)
        {
          button.Medal = 10;
        }
      }

      if (id == 5)//ClearReg
      {
        foreach (var delObj in delObjs)
        {
          delObj.SetActive(!delObj.activeSelf);
        }
      }
    }
	}
}

