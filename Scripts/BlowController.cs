using System;
using UnityEngine;

public class BlowController : MonoBehaviour
{
  public float Frailty = 0;//0 - не хрупкоe 100 - стекло
  [SerializeField] private float ignoreCollision = 15;
  [SerializeField] private GameObject iskri = null;
  [SerializeField] private Transform trailerTransform = null;
  [SerializeField] private GameObject box = null;
  [SerializeField] private GameObject[] createdObjs = null;
  [SerializeField] private int hasMoney = 0;
  

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

  public int HasMoney
  {
    get { return hasMoney; }
    set { hasMoney = value; }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.contacts.Length > 0)
    {
      Vector3 colRelVel = collision.relativeVelocity;
      if (colRelVel.magnitude > ignoreCollision)
      {
        //GameObject boxObj = null;
        if (Condition > 0 && Box != null && TrailerTransform != null)
        {
          Array.Resize(ref createdObjs, createdObjs.Length + 1);
          createdObjs[createdObjs.Length - 1] = Instantiate(Box, TrailerTransform.position + Vector3.up * 4, Quaternion.identity) as GameObject;
          createdObjs[createdObjs.Length - 1].transform.parent = transform.parent;//b
          if (createdObjs[createdObjs.Length - 1] != null)
          {
            Cargo cargo = createdObjs[createdObjs.Length - 1].GetComponent<Cargo>();
            if (cargo != null)
              cargo.AddCondition = colRelVel.magnitude * Frailty / 100;
          }
        }
        Condition = Mathf.Max(0, Condition - colRelVel.magnitude * Frailty/100);
      }

      foreach (ContactPoint contact in collision.contacts)
      {
        Instantiate(iskri, contact.point, Quaternion.identity);
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name.Substring(0, 3) == "Box")
    {
      Condition = Mathf.Min(100, Condition + other.GetComponent<Cargo>().AddCondition);
      Destroy(other.gameObject);
    }

    if (other.gameObject.name.Substring(0, 5) == "Money")
    {
      hasMoney += 1;
      Destroy(other.gameObject);
    }
  }

  private void OnDestroy()
  {
    DestroyBoxes();
  }

  public void DestroyBoxes()
  {
    foreach (var obj in createdObjs)
    {
      Destroy(obj);
    }
  }
}
