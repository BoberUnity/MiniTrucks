using UnityEngine;
using System.Collections;

public class Speeder : MonoBehaviour 
{
  [SerializeField] private float sp = 0;
  [SerializeField] private Rigidbody rig = null;
  [SerializeField] private UILabel l = null;
	

	void Update () 
  {
    sp = rig.velocity.magnitude;
	  l.text = sp.ToString("f0");
  }
}
