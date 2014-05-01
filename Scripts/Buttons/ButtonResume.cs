using UnityEngine;

public class ButtonResume : MonoBehaviour
{
  [SerializeField] private ButtonPause buttonPause = null;
  
  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      buttonPause.Pause = false;
    }
  }
}

