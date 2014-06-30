using System;
using System.Collections;
using UnityEngine;

public class CarZona : MonoBehaviour
{
  [SerializeField] private GameObject carTrafik = null;
  [SerializeField] private Transform cameraTransform = null;
  [SerializeField] private GameObject[] enemies = null;
  private Transform[] enemiesTractors = new Transform[4];
  private Transform[] enemiesTrailers = new Transform[4];
  private TraficWay traficWay = null;
  private int id = 0;
  public int Id
  {
    set { id = value;}
  }

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
    traficWay = transform.parent.parent.parent.GetComponent<TraficWay>();
  }

  private void OnBecameInvisible()
  {
    if (enabled)
    {
      StopAllCoroutines();
      StartCoroutine(DisableCar(2));
    }
  }

  private void OnBecameVisible()
  {
    Debug.LogWarning("I wisible");
    StopAllCoroutines();
    if (Vector3.Distance(transform.position, cameraTransform.position) < 170)
      EnableCar();
    else
      StartCoroutine(CheckDistance(1));
    
  }

  private IEnumerator CheckDistance(float time)
  {
    yield return new WaitForSeconds(time);
    if (!enabled)
    {
      if (Vector3.Distance(transform.position, cameraTransform.position) < 170)
        EnableCar();
      else
        StartCoroutine(CheckDistance(1));
    }
  }

  private void EnableCar()
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
      float minDistoTraf = 100;
      i = 0;
      foreach (var ce in traficWay.CarsEnabled)
      {
        if (ce)
        {
          minDistoTraf = Mathf.Min(minDistoTraf, Vector3.Distance(transform.parent.position, traficWay.CarZones[i].transform.position));
        }
        i += 1;
      }
      if (minDistoTraf > 5)//Проверка до машинок трафика
      {
        traficWay.CarsEnabled[id] = true;
        carTrafik.SetActive(true);
        enabled = true;
      }
      else Debug.LogWarning("Car was not activate, because trafic is nearest");
    }
    else Debug.LogWarning("Car was not activate, because enemy is nearest");
  }

  private void Update()
  {
    transform.parent.position = carTrafik.transform.position;
    transform.parent.rotation = carTrafik.transform.rotation;
  }

  private IEnumerator DisableCar(float time)
  {
    yield return new WaitForSeconds(time);
    if (enabled)
    {
      if (carTrafik.transform.rotation.eulerAngles.z > 45 && carTrafik.transform.rotation.eulerAngles.z < 270)
        carTrafik.transform.eulerAngles = new Vector3(carTrafik.transform.eulerAngles.x, carTrafik.transform.eulerAngles.y, 0);
      carTrafik.SetActive(false);
      enabled = false;
      traficWay.CarsEnabled[id] = false;
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
