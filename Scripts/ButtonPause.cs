using UnityEngine;

public class ButtonPause : MonoBehaviour
{
  [SerializeField] private bool pause = false;
  [SerializeField] private GameObject pausePanel = null;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      pause = !pause;
      if (pause)
      {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
      }
      else
      {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
      }
    }
  }
}

