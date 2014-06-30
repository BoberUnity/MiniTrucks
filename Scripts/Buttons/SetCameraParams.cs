using UnityEngine;

public class SetCameraParams : MonoBehaviour
{
  [SerializeField] private CarCameras carCameras = null;
  [SerializeField] private float distance = 20;
  [SerializeField] private float height = 20;
  [SerializeField] private float yawAngle = -30;
  [SerializeField] private float pitchAngle = -16.8f;
  [SerializeField] private float farclipPlane = 180;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      carCameras.distance = distance;
      carCameras.height = height;
      carCameras.yawAngle = yawAngle;
      carCameras.pitchAngle = pitchAngle;
      carCameras.GetComponent<Camera>().farClipPlane = farclipPlane;
    }
  }
}
