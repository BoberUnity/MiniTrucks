using UnityEngine;

public class BlowController : MonoBehaviour
{
  [SerializeField] private float frailty = 1;
  [SerializeField] private float ignoreCollision = 15;
  public float Condition = 100;
  
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contacts.Length > 0)
    {
      Vector3 colRelVel = collision.relativeVelocity;
      if (colRelVel.magnitude > ignoreCollision)
        Condition -= colRelVel.magnitude * frailty;
    }
  }
}
