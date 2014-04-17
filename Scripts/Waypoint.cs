using System;
using UnityEngine;

public class Waypoint : MonoBehaviour 
{
  public Transform[] Waypoints;
  public float[] MaxSpeeds;

  private void Awake()
  {
    int i = 0;
    var points = gameObject.GetComponentsInChildren<Transform>();
    Array.Resize(ref Waypoints, points.Length-1);
    Array.Resize(ref MaxSpeeds, points.Length - 1);
    foreach (Transform point in points)
    {
      if (point != transform)//чтобы не брал себя
      {
        Waypoints[i] = point;
        MaxSpeeds[i] = point.GetComponent<MaxSpeed>().Speed/2;
        i += 1;
      }
    }
  }

	void OnDrawGizmos () 
  {
	  var points = gameObject.GetComponentsInChildren<Transform>();

    foreach (Transform point in points)
    {
      Gizmos.color = Color.white; 
      Gizmos.DrawSphere(point.position, 1.0f );
    }
  }
}
