using UnityEngine;

public class TimeLive : MonoBehaviour
{
  [SerializeField] private float timeLive = 1;

	void Start () 
  {
	  Destroy(gameObject, timeLive);
	}
}
