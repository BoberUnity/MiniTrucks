using System;
using UnityEngine;

public class TraficWay : MonoBehaviour
{
  public CarZona[] CarZones = null;
  public bool[] CarsEnabled = null;

	void Start () 
  {
    CarZones = GetComponentsInChildren<CarZona>();
    Array.Resize(ref CarsEnabled, CarZones.Length);
	  int id = 0;
    foreach (var cz in CarZones)
    {
      cz.Id = id;
      id += 1;
    }
	}
}
