using UnityEngine;

public class ButtonExitRace : MonoBehaviour
{
  [SerializeField] private UIPanel gamePanel = null;
  [SerializeField] private UIPanel startPanel = null;
  [SerializeField] private SelectCarController selectCarController = null;
  [SerializeField] private ButtonPause buttonPause = null;
  [SerializeField] private GameObject cameraLevel = null;
  [SerializeField] private GameObject cameraGarage = null;
  [SerializeField] private GameObject cameraMap = null;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      buttonPause.Pause = false;
      cameraLevel.SetActive(false);
      cameraGarage.SetActive(true);
      gamePanel.transform.position = new Vector3(0, -800, 0);
      startPanel.transform.position = Vector3.zero;
      startPanel.enabled = true;
      UIButton[] enableButtons = startPanel.GetComponentsInChildren<UIButton>();
      foreach (var eb in enableButtons)
      {
        eb.isEnabled = true;
      }
      selectCarController.DestroyCharacter();
      foreach (var button in selectCarController.buttonsAddTrailer)
      {
        button.ExitRace();
      }
      cameraMap.SetActive(false);
    }
	}
}

