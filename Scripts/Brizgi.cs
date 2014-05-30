using UnityEngine;

public class Brizgi : MonoBehaviour
{
  [SerializeField] private ParticleEmitter brizgi = null;
  private Drivetrain drivetrain = null;
  private bool inWater = false;

  private void Start()
  {
    drivetrain = GetComponent<Drivetrain>();
  }

  private void Update()
  {
    if (inWater)
      brizgi.emit = drivetrain.velo > 5;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Water")
      inWater = true;
    
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.name == "Water")
    {
      inWater = false;
      brizgi.emit = false;
    }
  }
}
