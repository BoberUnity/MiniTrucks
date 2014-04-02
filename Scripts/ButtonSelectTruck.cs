using UnityEngine;

public class ButtonSelectTruck : MonoBehaviour
{
  [SerializeField] private CarCameras carcameras = null;
  [SerializeField] private Steer steer = null;
  [SerializeField] private GameObject[] trucks = null;
  [SerializeField] private int id = 0;

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      GameObject truck = Instantiate(trucks[id], Vector3.zero, Quaternion.identity) as GameObject;
      if (truck != null)
      {
        CameraTarget cts = truck.GetComponentInChildren<CameraTarget>();  //Находим трейлер, на который будет нацелена камера
        if (cts != null)
        {
          carcameras.target = cts.transform;
          steer.axisCarController = cts.GetComponent<AxisCarController>();
        }
        else
        {
          Debug.LogWarning("CameraTarget was not found");
        }
      }
      else
      {
        Debug.LogWarning("truck = NULL");
      }
    }
	}
}

