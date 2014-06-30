using UnityEngine;

public class BlowController : MonoBehaviour
{
  public float Frailty = 0;//0 - не хрупкоe 100 - стекло
  [SerializeField] private float ignoreCollision = 15;
  [SerializeField] private Transform trailerTransform = null;
  [SerializeField] private GameObject box = null;
  public float Condition = 100;

  public Transform TrailerTransform
  {
    set { trailerTransform = value; }
    get { return trailerTransform; }
  }

  public GameObject Box
  {
    get { return box; }
    set { box = value; }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contacts.Length > 0)
    {
      Vector3 colRelVel = collision.relativeVelocity;
      if (colRelVel.magnitude > ignoreCollision)
      {
        GameObject boxObj = null;
        if (Condition > 0 && Box != null)
        {
          boxObj = Instantiate(Box, TrailerTransform.position + Vector3.up * 4, Quaternion.identity) as GameObject;
          if (boxObj != null)
            boxObj.GetComponent<Cargo>().AddCondition = colRelVel.magnitude*Frailty/100;
        }
        Condition = Mathf.Max(0, Condition - colRelVel.magnitude * Frailty/100);
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Box(Clone)")
    {
      Condition = Mathf.Min(100, Condition + other.GetComponent<Cargo>().AddCondition);
      Destroy(other.gameObject);
    }
  }
}
