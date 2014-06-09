//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

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
  [SerializeField] private float speedKoeff = 1;//для сопёрника
  [SerializeField] private float trailerdragFree = 0.2f;//для гг
  [SerializeField] private float trailerdragBrake = 1f;
  private float h = 0;
  [SerializeField] private Rigidbody trailerRigidbody = null;//Устанавливается в редакторе для соперников
  [SerializeField] private Waypoint waypoint = null;//устанавливается для машин трафика
  private Transform firstWaypoint = null;//точка с которой начать путь. 
  [SerializeField] private bool trafic = false; //Устанавливается только для машин трафика
  private bool rayCar = false;//луч до впередиидущей машины трафика
  private float buksTime = 0;//Время зависания авто при нажатом нитро и скорость меньше 1
  [SerializeField] private int maxSpeed = 0;
  private bool isVisible = true;
  private float acselKoeff = 0.4f;

  public bool BrakeUsed
  {
    get { return brakeUsed; }
    set { brakeUsed = value;}
  }

  public bool NitroUsed
  {
    get { return nitroUsed; }
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

  public Rigidbody Trailer
  {
    set { trailerRigidbody = value; }
  }

  public int MaxSpeed
  {
    get { return maxSpeed; }//Tunning Button
    set { maxSpeed = value; }
  }

  public bool IsVisible
  {
    set { isVisible = value; }
  }
   public float AcselKoeff
   {
     set { acselKoeff = value; }
   }
  protected override void Start()
  {
    base.Start();
    
    if (waypoint != null)//для машин трафика
      StartCoroutine(SetTrafikPath(0.1f));
    else
      StartCoroutine(ClockOff(4));
  }

  private IEnumerator SetTrafikPath(float time)
  {
    yield return new WaitForSeconds(time);
    SetWay(waypoint);
    ai = true;
    oppWaitClock = false;
  }

  private void SetFirstPoint()
  {
    float minDis = 1000;//дист до ближ точки пути
    foreach (Transform wp in waypoints)
    {
      float disToWp = Vector3.Distance(transform.position, wp.position);
      if (disToWp < minDis)
      {
        firstWaypoint = wp;
        minDis = disToWp;
      }
    }
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
          if (trailerRigidbody != null)
            trailerRigidbody.drag = trailerdragBrake;
        }
        else
        {
          if (on)
          {
            if (nitroUsed)
              throttleInput = 1;
            else
            {
              if (drivetrain.velo*2.2f < maxSpeed)//Ограничитель скорости
                throttleInput = acselKoeff;
              else 
                throttleInput = 0;
            }
          }
          else
            throttleInput = 0;
          brakeInput = 0;
          if (trailerRigidbody != null)
            trailerRigidbody.drag = trailerdragFree;
        }
      }
      steerInput = steerUsed; /* 0.0025f;*///Input.GetAxisRaw(steerAxis); SteerUsed [-400;400]
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
    else//AI
    {
      RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z));
      steerInput = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;//Поворот сбившегося
      if (Mathf.Abs(steering) > 0.4f && drivetrain.velo < 3 && !isVisible)
      {
        //transform.Rotate(Vector3.up * steering * Time.deltaTime * 40);
        transform.localEulerAngles += new Vector3(0, steering*Time.deltaTime*40, 0);
        //Debug.LogWarning("Rotating");
      }

      if (RelativeWaypointPosition.magnitude < 7)
      {
        currentWaypoint++;
        if (currentWaypoint == waypoints.Length)
          currentWaypoint = 0;
      }

      float sp = rigidbody.velocity.magnitude;
      if (sp < speeds[currentWaypoint] * speedKoeff && !oppWaitClock && !rayCar)
        throttleInput = 0.95f;
      else
        throttleInput = 0;


      if (sp > speeds[currentWaypoint] + 5 || (rayCar && drivetrain.gear > 1))
      {
        brakeInput = 0.5f;
        if (trailerRigidbody != null)
          trailerRigidbody.drag = trailerdragBrake;
      }
      else
      {
        brakeInput = 0;
        if (trailerRigidbody != null) 
          trailerRigidbody.drag = trailerdragFree;
      }
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
    //if (drivetrain != null)
    //{
    //  if (nitroUsed && drivetrain.velo < 2)
    //  {
    //    buksTime += Time.deltaTime;
    //    {
    //      if (buksTime > 10)
    //      {
    //        transform.position += Vector3.up * 4 + Vector3.right * 3;
    //        buksTime = 0;
    //      }
    //    }
    //  }
    //  else 
    //    buksTime = 0;
    //}
  }

  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    if (waypoint == null) //только для соперников
      rayCar = Physics.Raycast(transform.position + Vector3.up + transform.forward, transform.forward, 10, 1 << 17); //layer Car1
    if (trafic)
      rayCar = Physics.Raycast(transform.position + Vector3.up + transform.forward, transform.forward, 15, 1 << 17); //layer Car1
  }

  public void SetWay(Waypoint wpoint)
  {
    GameObject wayObject = wpoint.gameObject;
    if (wayObject != null)
    {
      Waypoint wayComponent = wayObject.GetComponent<Waypoint>();
      if (wayComponent != null)
      {
        waypoints = wayComponent.Waypoints;
        speeds = wayComponent.MaxSpeeds;
        if (trafic)//Для машин трафика
        {
          SetFirstPoint();
          int cw = 0;
          foreach (Transform wp in waypoints)
          {
            if (wp != firstWaypoint)
              cw += 1;
            else
            {
              //Debug.LogWarning("CW= " + cw);
              currentWaypoint = cw;
            }
          }
        }
      }
      else
        Debug.LogWarning("Компонент Waypoint не найден на объекте" + waypoint.gameObject.name);
    }
    else
      Debug.LogWarning("Путь " + waypoint.gameObject.name + "не найден");
  }

  void OnDrawGizmos ()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + transform.forward * 10);
  }
}
