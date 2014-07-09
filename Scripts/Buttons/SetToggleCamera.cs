using System.Collections;
using UnityEngine;

public class SetToggleCamera : MonoBehaviour
{
  [SerializeField] private ButtonHandler buttonStart = null;
  [SerializeField] private UIToggle[] toggles = null;
  [SerializeField] private SetCameraParams[] setCameraParams = null;
  public int cameraType = 0;
  private bool pause = false;

	private void Awake ()
	{
    if (PlayerPrefs.HasKey("CameraParams"))
    {
      SetCameraType(PlayerPrefs.GetInt("CameraParams"));
      cameraType = PlayerPrefs.GetInt("CameraParams");
    }
    
    buttonStart.Pressed += NewGame;
	}

  private void OnDestroy()
  {
    buttonStart.Pressed -= NewGame;
  }

  private void OnEnable()
  {
    if (Time.timeScale < 0.5f)
    {
      Time.timeScale = 1;
      pause = true;
    }
    StartCoroutine(SetActivateButton(0.01f));
  }

  private void NewGame()
  {
    PlayerPrefs.SetInt("CameraParams", cameraType);
  }

  private void SetCameraType(int type)
  {
    toggles[type].value = true;
    setCameraParams[type].OnPress(false);
  }

  private IEnumerator SetActivateButton(float time)
  {
    yield return new WaitForSeconds(time);
    if (pause)
    {
      Time.timeScale = 0;
      pause = false;
    }
    SetCameraType(cameraType);
  }
}
