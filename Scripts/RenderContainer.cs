using UnityEngine;

public class RenderContainer : MonoBehaviour
{
  //[SerializeField] private CharacterJoint characterJoint = null;
  [SerializeField] private Rigidbody trailer = null;
  //[SerializeField] private Vector3 connectPosition = new Vector3(0, 0.41f, -1.7f);

  private void OnBecameVisible()
  {
    trailer.interpolation = RigidbodyInterpolation.Interpolate;
    transform.parent.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
  }

  private void OnBecameInvisible()
  {
    trailer.interpolation = RigidbodyInterpolation.None;
    transform.parent.rigidbody.interpolation = RigidbodyInterpolation.None;
  }
}
