using UnityEngine;

public class Waypoint : MonoBehaviour 
{
  private Transform[] waypoints;

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
