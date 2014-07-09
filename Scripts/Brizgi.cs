using UnityEngine;

public class Brizgi : MonoBehaviour
{
  [SerializeField] private ParticleEmitter brizgi = null;
  private Drivetrain drivetrain = null;

  private void Start()
  {
    drivetrain = GetComponent<Drivetrain>();
    enabled = false;
  }

  private void Update()
  {
    brizgi.emit = drivetrain.velo > 2;
    brizgi.localVelocity = drivetrain.gear > 0 ? Vector3.forward * drivetrain.velo * 0.9f + Vector3.up * 4 : -Vector3.forward * drivetrain.velo * 0.9f + Vector3.up * 4;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Water")
      enabled = true;
    
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.name == "Water")
    {
      enabled = false;
      brizgi.emit = false;
    }
  }
}
