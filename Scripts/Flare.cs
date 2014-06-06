using UnityEngine;

public class Flare : MonoBehaviour
{
  [SerializeField] private ParticleEmitter[] flares = null;
  [SerializeField] private ParticleEmitter[] smokes = null;
  private Drivetrain drivetrain = null;
  private AxisCarController axisCarController = null;

  private float velocityZ  = 0;
	
	void Start ()
	{
	  drivetrain = transform.parent.GetComponent<Drivetrain>();
    axisCarController = transform.parent.GetComponent<AxisCarController>();
    if (smokes.Length > 0) 
      velocityZ = smokes[0].localVelocity.z;
	}
	
	void Update ()
	{
	  foreach (var flare in flares)
	  {
	    flare.emit = axisCarController.NitroUsed;
	  }

    foreach (var smoke in smokes)
    {
      smoke.emit = (!axisCarController.BrakeUsed || drivetrain.gear == 0) && !axisCarController.NitroUsed;
      smoke.localVelocity = new Vector3(smoke.localVelocity.x, smoke.localVelocity.y, velocityZ - drivetrain.velo*0.1f);
    }
	}
}
