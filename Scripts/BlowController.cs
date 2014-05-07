using UnityEngine;

public class BlowController : MonoBehaviour
{
  public float Frailty = 0;//0 - не хрупкоб 100 - стекло
  [SerializeField] private float ignoreCollision = 15;
  public float Condition = 100;
  
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contacts.Length > 0)
    {
      Vector3 colRelVel = collision.relativeVelocity;
      if (colRelVel.magnitude > ignoreCollision)
      {
        //Debug.LogWarning("Coll " + colRelVel.magnitude + " " + gameObject.name);
        Condition = Mathf.Max(0, Condition - colRelVel.magnitude * Frailty/1000);
      }
    }
  }
}
