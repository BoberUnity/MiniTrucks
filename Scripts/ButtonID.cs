using UnityEngine;

public class ButtonID : MonoBehaviour
{
  [SerializeField] private int id = 0;
  
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
    }
	}
}

