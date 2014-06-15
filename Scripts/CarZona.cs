using System.Collections;
using UnityEngine;

public class CarZona : MonoBehaviour
{
  [SerializeField] private GameObject carTrafik = null;
  [SerializeField] private GameObject[] enemies = null;
  private Transform[] enemiesTractors = new Transform[4];
  private Transform[] enemiesTrailers = new Transform[4];
  [SerializeField] private float vel;


  public GameObject[] Enemies
  {
    set
    {
      enemies = value;
      int i = 0;
      foreach (var e in enemies)
      {
        enemiesTractors[i] = e.GetComponentInChildren<AxisCarController>().transform;
        enemiesTrailers[i] = enemiesTractors[i].GetComponent<CharacterJoint>().connectedBody.transform;
        i += 1;
      }
    }
  }
  
  private void Start()
  {
    carTrafik.SetActive(false);
    enabled = false;
  }

  private void OnBecameInvisible()
  {
    if (enabled)
      StartCoroutine(DisableCar(2));
  }

  private void OnBecameVisible()
  {
    int i = 0;
    float minDis = 100;//Дистанция до ближайшего соперника/прицепа
    if (enemiesTractors[0] != null)
    {
      foreach (var e in enemies)
      {
        minDis = Mathf.Min(minDis, Vector3.Distance(transform.position, enemiesTractors[i].position));
        minDis = Mathf.Min(minDis, Vector3.Distance(transform.position, enemiesTrailers[i].position));
        i += 1;
      }
    }
    
    if (minDis > 5)
    {
      carTrafik.SetActive(true);
      enabled = true;
      StopAllCoroutines();
    }
    else Debug.LogWarning("Car was not activate, because enemy is nearest");
  }

  private void Update()
  {
    vel = Mathf.Max(vel, carTrafik.rigidbody.velocity.magnitude);
    transform.parent.position = carTrafik.transform.position;
    transform.parent.rotation = carTrafik.transform.rotation;
  }

  private IEnumerator DisableCar(float time)
  {
    yield return new WaitForSeconds(time);
    if (enabled)
    {
      carTrafik.SetActive(false);
      enabled = false;
    }
  }

  private void OnApplicationQuit()
  {
    enabled = false;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = new Color(1, 1, 0, 0.5f);
    Gizmos.DrawSphere(transform.parent.position, 2.0f);
  }
}
