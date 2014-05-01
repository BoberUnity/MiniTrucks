using UnityEngine;

public class ButtonPause : MonoBehaviour
{
  private bool pause = false;
  [SerializeField] private GameObject pausePanel = null;
  [SerializeField] private GameObject gamePanel = null;

  public bool Pause
  {
    set 
    {
      pause = value;
      pausePanel.SetActive(false);
      gamePanel.SetActive(true);
      Time.timeScale = 1;
    }
  }

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      pause = !pause;
      if (pause)
      {
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0;
      }
    }
  }
}

