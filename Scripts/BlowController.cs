using UnityEngine;

public class BlowController : MonoBehaviour
{
  public float Frailty = 0;//0 - не хрупкоб 100 - стекло
  [SerializeField] private float ignoreCollision = 15;
  public float Condition = 100;
  [SerializeField] private float ray = 0;
  //[SerializeField] private bool rayCar = false;
  
  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contacts.Length > 0)
    {
      Vector3 colRelVel = collision.relativeVelocity;
      if (colRelVel.magnitude > ignoreCollision)
      {
        //Debug.LogWarning("Coll " + colRelVel.magnitude + " " + gameObject.name);
        Condition = Mathf.Max(0, Condition - colRelVel.magnitude * Frailty/400);
      }
    }
  }

  private void FixedUpdate()
  {
    Vector3 fwd = transform.TransformDirection(Vector3.forward);
    //rayCar = Physics.Raycast(transform.position, fwd, 20, 17);
    RaycastHit[] hits;
    hits = Physics.RaycastAll(transform.position, fwd, 100.0F);
    int i = 0;
    ray = 100;
    while (i < hits.Length)
    {
      RaycastHit hit = hits[i];
      if (hit.transform.gameObject.layer == 17)//layer Car1
        ray = Mathf.Min(ray, hit.distance);
      i++;
    }
  }
}
