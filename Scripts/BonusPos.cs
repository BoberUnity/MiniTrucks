using UnityEngine;

public class BonusPos : MonoBehaviour 
{
	void Start () 
  {
	
	}

  void OnDrawGizmos()
  {
    Gizmos.color = new Color(1, 1, 0, 1);
    Gizmos.DrawSphere(transform.position, 1.0f);
  }
}
