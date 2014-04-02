using UnityEngine;
using System.Collections;

public class Truck : MonoBehaviour
{
  [SerializeField] private WheelCollider FrontLeftWheel = null;
  [SerializeField] private WheelCollider FrontRightWheel = null;
  [SerializeField] private WheelCollider RearLeftWheel = null;
  [SerializeField] private WheelCollider RearRightWheel = null;
  [SerializeField] private Transform CenterMass = null;
  [SerializeField] private float SilaRula =25;
  [SerializeField] private float[] GearRatio;
  [SerializeField] private float[] GearRatioTorque;//reality sila tolchka
  //[SerializeField] private int CurrentGear = 2;
  //[SerializeField] private float Friction = 1;
  private float throttle = 0;
  private float steer = 0;
  //private float sp = 0;
  private double engineRPM = 0;
  //private double MinEngineRPM = 1000;
  //private double MaxEngineRPM = 3000;
  [SerializeField] private float EngineTorque = 10000;
  private Rigidbody ThisRigidbody = null;
	// Use this for initialization
	void Start () 
  {
    ThisRigidbody = rigidbody;
    ThisRigidbody.centerOfMass = CenterMass.localPosition;
	}
	
	// Update is called once per frame
  void Update()
  {
    throttle = Input.GetAxis("Vertical");
    steer = Input.GetAxis("Horizontal");
    ThisRigidbody.drag = ThisRigidbody.velocity.magnitude/250;
    //sp = ThisRigidbody.velocity.magnitude*3;

    engineRPM = (FrontLeftWheel.rpm + FrontRightWheel.rpm)/2*3.7f;
    engineRPM = Mathf.Max(Mathf.Min(Mathf.Abs((float) engineRPM), 8000), 900);

    FrontLeftWheel.steerAngle = SilaRula * steer;
    FrontRightWheel.steerAngle = SilaRula * steer;
    RearLeftWheel.motorTorque = throttle * EngineTorque;
    RearRightWheel.motorTorque = throttle * EngineTorque;
    
  }
}
