/*using UnityEngine;
using System.Collections;

public class AITruck : MonoBehaviour
{
  [SerializeField] private AxisCarController axisCarController = null;
  private Vector3 RelativeWaypointPosition;
  [SerializeField] private Transform[] waypoints;
  private int currentWaypoint = 0;
  private bool nazad = false;
  private float steerInput = 0;
  
	//void Start ()
  //{
  //  axisCarController.throttleInput = 1;
  //}
	
  //// Update is called once per frame
  void Update () 
  {
  //  axisCarController.throttleInput = 1;
    axisCarController.throttleInput = 1;
  }

  void NavigateTowardsWaypoint()
  {

    if (nazad == false)
    {
      RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x,transform.position.y,waypoints[currentWaypoint].position.z));
      //Ðó÷íèê è ïÿòàê
      //if (speed2 < 0.05 || RelativeWaypointPosition.z < 0)
      //{
      //  if (inputSteer < -0.4) { transform.eulerAngles.y -= 1; };
      //  if (inputSteer > 0.4) { transform.eulerAngles.y += 1; };
      //};
    }
    //else
    //{
    //  RelativeWaypointPosition = transform.InverseTransformPoint(Vector3(
    //                         transform.position.x + Mathf.Sin((transform.eulerAngles.y - 180) / 57.3) * 100,
    //                         transform.position.y + 1,
    //                         transform.position.z + Mathf.Cos((transform.eulerAngles.y - 180) / 57.3) * 100));
    //};
    //temp_position = RelativeWaypointPosition;
    //b2 = RelativeWaypointPosition.z;

    steerInput = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

    //if (Mathf.Abs(steerInput) < 0.5 && RelativeWaypointPosition.z > 0)
    //{
    //  inputTorque = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs(steerInput);
    //}
    //else
    //{
    //  //inputTorque = 0;
    //  if (nazad == true)
    //  {
    //    inputTorque = -1;
    //    steerInput = -steerInput;
    //  };
    //}
    //if (STime < 3)
    //{
    //  inputTorque = 0;
    //  nazad = false;
    //  wait_revers = 0;
    //};
    ////bsp = RelativeWaypointPosition.magnitude;
    if (RelativeWaypointPosition.magnitude < 20)
    {
      currentWaypoint++;

      //if (currentWaypoint >= waypoints.length)
      //{
      //  currentWaypoint = 0;
      //  STime = -10000;

      //}
    }
  }
}*/
