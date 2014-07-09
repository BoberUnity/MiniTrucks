using System.Collections;
using UnityEngine;

public class SetToggleControls : MonoBehaviour
{
  [SerializeField] private ButtonHandler buttonStart = null;
  [SerializeField] private UIToggle[] toggles = null;
  [SerializeField] private SetPosition[] setPositions = null;
  [SerializeField] private SetActiveObjs[] setActivesObjs = null;
  public int currentControls = 0;

	private void Awake ()
	{
    if (PlayerPrefs.HasKey("ControlsParams"))
      SetControlType(PlayerPrefs.GetInt("ControlsParams"));

	  buttonStart.Pressed += NewGame;
	}

  private  void OnDestroy()
  {
    buttonStart.Pressed -= NewGame;
  }

  private void OnEnable()
  {
    SetControlType(currentControls);
  }

  private void NewGame()
  {
    PlayerPrefs.SetInt("ControlsParams", currentControls);
  }

  private void SetControlType(int type)
  {
    toggles[type].value = true;
    setPositions[type].OnPress(false);
    setActivesObjs[type].OnPress(false);
    currentControls = type;
  }
}
