using UnityEngine;

public class ButtonPause : MonoBehaviour
{
  [SerializeField] private bool pause = false;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      pause = !pause;
      if (pause)
        Time.timeScale = 0;
      else
        Time.timeScale = 1;
    }
  }
}

