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
  [SerializeField] private LensFlare signalRearLeft = null;
  [SerializeField] private LensFlare signalRearRight = null;
  [SerializeField] private LensFlare signalTrailerLeft = null;//��� ��� ����������
  [SerializeField] private LensFlare signalTrailerRight = null;
  //private bool avar = false;
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
  [SerializeField] private float speedKoeff = 1;//��� ��������
  [SerializeField] private float trailerdragFree = 0.2f;//��� ��
  [SerializeField] private float trailerdragBrake = 1f;
  private float h = 0;
  [SerializeField] private Rigidbody trailerRigidbody = null;//��������������� � ��������� ��� ����������
  [SerializeField] private Waypoint waypoint = null;//��������������� ��� ����� �������
  private Transform firstWaypoint = null;//����� � ������� ������ ����. 
  [SerializeField] private bool trafic = false; //��������������� ������ ��� ����� �������
  [SerializeField] private float acselKoeff = 0.4f;//Only enemies
  [SerializeField]
  private bool rayCar = false;//��� �� ������������� ������ �������
  private float buksTime = 0;//����� ��������� ���� ��� ������� ����� � �������� ������ 1
  [SerializeField]
  private int maxSpeed = 0;
  private bool isVisible = true;
  private float roteteTime = 0;//����������
  //private bool isRotating = false;
  

  public bool BrakeUsed
  {
    get { return brakeUsed; }
    set
    {
      brakeUsed = value;
    }
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

  public LensFlare SignalTrailerLeft
  {
    set { signalTrailerLeft = value; }
  }

  public LensFlare SignalTrailerRight
  {
    set { signalTrailerRight = value; }
  }

  protected override void Start()
  {
    base.Start();
    
    if (waypoint != null)//��� ����� �������
      StartCoroutine(SetTrafikPath(0.01f));
    else
      StartCoroutine(ClockOff(1));//������ ���� 4 ���!!!!!!!!!! ����� ����������
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
    float minDis = 1000;//���� �� ���� ����� ����
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
          if (drivetrain.velo * 2.2f > maxSpeed && drivetrain.gear == 0 && !NitroUsed)
            brakeInput = 0;
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
              if (drivetrain.velo*2.2f < maxSpeed)//������������ ��������
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
    }
    else//AI
    {
      RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z));
      steerInput = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;//������� ����������
      if (Mathf.Abs(steering) > 0.4f && drivetrain.velo < 3 && !isVisible)
      {
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
      if (!oppWaitClock && !rayCar)
      {
        throttleInput = 0;
        NitroUsed = false;
        if (sp < speeds[currentWaypoint] * speedKoeff)
        {
          throttleInput = acselKoeff;//0.4f
        }
        if (sp < speeds[currentWaypoint] * speedKoeff + 5 && sp > 16 * speedKoeff && sp < 25 * speedKoeff && Mathf.Abs(steerInput) < 0.015f && !trafic)
        {
          throttleInput = 0.99f;
          NitroUsed = true;
        }
        BrakeUsed = false;//only for particles
      }
      else
      {
        throttleInput = 0;
        NitroUsed = false;
        BrakeUsed = true;//only for particles
      }


      if (sp > speeds[currentWaypoint] + 5 || (rayCar && drivetrain.gear > 1))
      {
        brakeInput = 0.5f;
        BrakeUsed = true;//only for particles
        if (trailerRigidbody != null)
          trailerRigidbody.drag = trailerdragBrake;
      }
      else
      {
        brakeInput = 0;
        if (trailerRigidbody != null) 
          trailerRigidbody.drag = trailerdragFree;
      }
      handbrakeInput = 0;
      clutchInput = 0;
      startEngineInput = Input.GetButton("Fire1");
      if (drivetrain == null)
        Debug.LogWarning("drivetrain == null" + gameObject.name);
      targetGear = drivetrain.gear;
    }
  }

  //������� ����� ���������
  protected override void Update()
  {
    base.Update();
    if (brakeInput > 0 || drivetrain.gear == 0)
    {
      signalRearLeft.enabled = true;
      signalRearRight.enabled = true;
      if (trailerRigidbody != null)
      {
        signalTrailerLeft.enabled = true;
        signalTrailerRight.enabled = true;
      }
      
      if (drivetrain.gear == 0 && !trafic)
        {
          signalRearLeft.color = Color.grey;
          signalRearRight.color = Color.grey;
          if (trailerRigidbody != null)
          {
            signalTrailerLeft.color = Color.grey;
            signalTrailerRight.color = Color.grey;
          }
        }

        if (brakeInput > 0)
        {
          signalRearLeft.color = Color.red;
          signalRearRight.color = Color.red;
          if (trailerRigidbody != null)
          {
            signalTrailerLeft.color = Color.red;
            signalTrailerRight.color = Color.red;
          }
        }
    }
    else
    {
      signalRearLeft.enabled = false;
      signalRearRight.enabled = false;
      if (trailerRigidbody != null)
      {
        signalTrailerLeft.enabled = false;
        signalTrailerRight.enabled = false;
      }
    }

    if ((transform.eulerAngles.z > 70 && transform.eulerAngles.z < 180) || (transform.eulerAngles.z > 180 && transform.eulerAngles.z < 310))
    {
      if (drivetrain.velo < 2)
      {
        roteteTime += Time.deltaTime;
        if (roteteTime > 5)
        {
          transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
          transform.position += Vector3.up*0.5f;
          Debug.LogWarning("ReturnOnGround");
        }
      }
    }
    else
      roteteTime = 0;
    
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
    if (waypoint == null) //������ ��� ����������
      rayCar = Physics.Raycast(transform.position/* + Vector3.up*/ + transform.forward*3, transform.forward, 10, 1 << 17); //layer Car1
    if (trafic)
    {
      rayCar = Physics.Raycast(transform.position/* + Vector3.up*/ + transform.forward*2, transform.forward, 15, 1 << 17); //layer Car1
      //avar = /*rayCar && */drivetrain.velo < 1;
    }
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
        if (trafic)//��� ����� �������
        {
          SetFirstPoint();
          int cw = 0;
          foreach (Transform wp in waypoints)
          {
            if (wp != firstWaypoint)
              cw += 1;
            else
              currentWaypoint = cw;
            
          }
        }
      }
      else
        Debug.LogWarning("��������� Waypoint �� ������ �� �������" + waypoint.gameObject.name);
    }
    else
      Debug.LogWarning("���� " + waypoint.gameObject.name + "�� ������");
  }

  void OnDrawGizmos ()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawLine(transform.position/* + Vector3.up*/ + transform.forward * 4, transform.position + Vector3.up + transform.forward * 15);
  }
}
