using UnityEngine;

public class SetCameraParams : MonoBehaviour
{
  [SerializeField] private SetToggleCamera setToggleCamera = null;
  [SerializeField] private CarCameras carCameras = null;
  [SerializeField] private int id = 0;
  [SerializeField] private float distance = 20;
  [SerializeField] private float height = 20;
  [SerializeField] private float yawAngle = -30;
  [SerializeField] private float pitchAngle = -16.8f;
  [SerializeField] private float farclipPlane = 180;

  private void Start()
  {
    if (PlayerPrefs.GetInt("CameraParams") == id)
      OnPress(false);
  }

  /*protected virtual*/public void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      carCameras.distance = distance;
      carCameras.height = height;
      carCameras.yawAngle = yawAngle;
      carCameras.pitchAngle = pitchAngle;
      carCameras.GetComponent<Camera>().farClipPlane = farclipPlane;
      PlayerPrefs.SetInt("CameraParams", id);
      setToggleCamera.cameraType = id;
    }
  }
}
