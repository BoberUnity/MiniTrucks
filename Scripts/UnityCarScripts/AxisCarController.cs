//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using System;
using System.Collections;
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
  [SerializeField] private float[] speeds;//Max speed points
  private int currentWaypoint = 0;
  private bool on = false;
  private bool oppWaitClock = true;
  public bool InStation = false;
  [SerializeField] private float speedKoeff = 1;
  [SerializeField]
  private float h = 0;

  private float buksTime = 0;//Время зависания авто при нажатом нитро и скорость меньше 1

  public bool BrakeUsed
  {
    set { brakeUsed = value;}
  }

  public bool NitroUsed
  {
    set { nitroUsed = value;}
  }

  public float SteerUsed
  {
    set { steerUsed = value;}
  }

  public bool On
  {
    set { on = value;}
  }

  protected override void Start()
  {
    base.Start();
    StartCoroutine(ClockOff(4));
  }

  private IEnumerator ClockOff(float time)
  {
    yield return new WaitForSeconds(time);
    oppWaitClock = false;
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
      
      if (InStation)
      {
        handbrakeInput = Mathf.Min(1, h + Time.deltaTime * 4);
        h = handbrakeInput;
        throttleInput = 0;
        brakeInput = 0;
      }
      else
      {
        handbrakeInput = 0;
        h = Mathf.Max(0, h - Time.deltaTime * 4);
        if (brakeUsed)
        {
          throttleInput = 0;
          brakeInput = 1;
        }
        else
        {
          if (on)
          {
            if (nitroUsed)
              throttleInput = 1;
            else
              throttleInput = 0.3f;
          }
          else
            throttleInput = 0;
          brakeInput = 0;
        }
      }
      steerInput = steerUsed * 0.0025f;//Input.GetAxisRaw(steerAxis); SteerUsed [-400;400]
      //handbrakeInput = Input.GetAxisRaw(handbrakeAxis);
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
    else
    {
      RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z));
      steerInput = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
      if (RelativeWaypointPosition.magnitude < 7)
      {
        currentWaypoint++;
        if (currentWaypoint == waypoints.Length)
          currentWaypoint = 0;
      }
      if (rigidbody.velocity.magnitude < speeds[currentWaypoint] * speedKoeff && !oppWaitClock)
        throttleInput = 0.5f;
      else
        throttleInput = 0;
      brakeInput = 0;
      if (rigidbody.velocity.magnitude > speeds[currentWaypoint]+5)
        brakeInput = 0.25f;
      //steerInput = 0;
      handbrakeInput = 0;
      clutchInput = 0;
      startEngineInput = Input.GetButton("Fire1");
      if (drivetrain == null)
        Debug.LogWarning("drivetrain == null" + gameObject.name);
      targetGear = drivetrain.gear;
    }
  }

  //Возврат после зависания
  protected override void Update()
  {
    base.Update();
    if (drivetrain != null)
    {
      if (nitroUsed && drivetrain.velo < 2)
      {
        buksTime += Time.deltaTime;
        {
          if (buksTime > 10)
          {
            transform.position += Vector3.up * 4 + Vector3.right * 3;
            buksTime = 0;
          }
        }
      }
      else 
        buksTime = 0;
    }
  }
  
  public void SetWay(Waypoint waypoint)
  {
    GameObject wayObject = waypoint.gameObject;
    if (wayObject != null)
    {
      Waypoint wayComponent = wayObject.GetComponent<Waypoint>();
      if (wayComponent != null)
      {
        waypoints = wayComponent.Waypoints;
        speeds = wayComponent.MaxSpeeds;
      }
      else
        Debug.LogWarning("Компонент Waypoint не найден на объекте" + waypoint.gameObject.name);
    }
    else
      Debug.LogWarning("Путь " + waypoint.gameObject.name + "не найден");
  }
}
