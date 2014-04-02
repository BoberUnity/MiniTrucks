//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class AxisCarController : CarController
{
  private bool brakeUsed = false;
  private bool nitroUsed = false;
  private float steerUsed = 0.5f;
  [SerializeField] private bool ai = false;
	public string throttleAxis="Throttle";
	public string brakeAxis="Brake";
	public string steerAxis="Horizontal";
	public string handbrakeAxis="Handbrake";
	public string clutchAxis="Clutch";
	public string shiftUpButton="ShiftUp";
	public string shiftDownButton="ShiftDown";
	public string startEngineButton="StartEngine";
  private Vector3 RelativeWaypointPosition;
  [SerializeField] private Transform[] waypoints;
  private int currentWaypoint = 0;

  public bool BrakeUsed
  {
    set { brakeUsed = value;}
  }

  public bool NitroUsed
  {
    set { nitroUsed = value; }
  }

  public float SteerUsed
  {
    set { steerUsed = value;}
  }



  protected override void GetInput(out float throttleInput,
                  out float brakeInput,
                  out float steerInput,
                  out float handbrakeInput,
                  out float clutchInput,
                  out bool startEngineInput,
                  out int targetGear)
  {
    if (!ai)
    {
      if (brakeUsed)
      {
        throttleInput = 0;
        brakeInput = 1;
      }
      else
      {
        if (nitroUsed)
          throttleInput = 1;
        else
          throttleInput = 0.3f;
        brakeInput = 0;
      }
      steerInput = steerUsed * 0.01f;//Input.GetAxisRaw(steerAxis);
      handbrakeInput = Input.GetAxisRaw(handbrakeAxis);
      clutchInput = Input.GetAxisRaw(clutchAxis);
      startEngineInput = Input.GetButton(startEngineButton);


      // Gear shift
      targetGear = drivetrain.gear;
      if (Input.GetButtonDown(shiftUpButton))
      {
        ++targetGear;
      }
      if (Input.GetButtonDown(shiftDownButton))
      {
        --targetGear;
      }

      if (drivetrain.shifter == true)
      {
        if (Input.GetButton("reverse"))
        {
          targetGear = 0;
        }

        else if (Input.GetButton("neutral"))
        {
          targetGear = 1;
        }

        else if (Input.GetButton("first"))
        {
          targetGear = 2;
        }

        else if (Input.GetButton("second"))
        {
          targetGear = 3;
        }

        else if (Input.GetButton("third"))
        {
          targetGear = 4;
        }

        else if (Input.GetButton("fourth"))
        {
          targetGear = 5;
        }

        else if (Input.GetButton("fifth"))
        {
          targetGear = 6;
        }

        else if (Input.GetButton("sixth"))
        {
          targetGear = 7;
        }

        else
        {
          targetGear = 1;
        }
      }
    }
    else//AI
    {
      RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z));
      steerInput = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
      if (RelativeWaypointPosition.magnitude < 5)
      {
        currentWaypoint++;
        if (currentWaypoint == waypoints.Length)
          currentWaypoint = 0;
      }
      throttleInput = 0.5f;
      brakeInput = 0;
      //steerInput = 0;
      handbrakeInput = 0;
      clutchInput = 0;
      startEngineInput = Input.GetButton("Fire1");
      targetGear = drivetrain.gear;
    }
  }
}
