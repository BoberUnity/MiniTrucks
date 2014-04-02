using UnityEngine;
using System.Collections;

public class Fence : MonoBehaviour {
	
//	[RequireComponent(Rigidbody)]
	
	public float MinImpactForce = 1.0f;
	public float MassOfFinalObject = 200.0f;
	
	private float force;
	private Rigidbody body;

	void OnCollisionEnter(Collision collision) {
		force = collision.relativeVelocity.magnitude * collision.gameObject.rigidbody.mass;
		if (force > MinImpactForce) {
			body = gameObject.AddComponent<Rigidbody>();
			body.mass = MassOfFinalObject;
			Destroy(this);
		}
	}
}
