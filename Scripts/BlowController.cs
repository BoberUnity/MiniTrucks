using UnityEngine;

public class BlowController : MonoBehaviour
{
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contacts.Length > 0)
    {
      Vector3 colRelVel = collision.relativeVelocity;
      Debug.LogWarning("Udar " + colRelVel.magnitude);
    }
  }
}
