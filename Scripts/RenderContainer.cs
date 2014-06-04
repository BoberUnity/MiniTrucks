using UnityEngine;

public class RenderContainer : MonoBehaviour
{
  //[SerializeField] private CharacterJoint characterJoint = null;
  [SerializeField] private Rigidbody trailer = null;
  [SerializeField] private BoxCollider bigCollider = null;
  [SerializeField] private AxisCarController axisCarController = null;
  [SerializeField] private Vector3 connectPosition = new Vector3(0, 0.41f, -1.7f);
  [SerializeField] private Vector3 locPosMem = Vector3.zero;

  private void Start()
  {
    locPosMem = trailer.transform.localPosition;
  }
  
  private void OnBecameVisible()
  {
    //trailer.interpolation = RigidbodyInterpolation.Interpolate;
    axisCarController.IsVisible = true;
    transform.parent.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    bigCollider.enabled = false;
    trailer.gameObject.SetActive(true);
    trailer.transform.position = transform.parent.position - transform.forward * locPosMem.z + Vector3.up * locPosMem.y;
    trailer.transform.rotation = transform.parent.rotation;
    transform.parent.GetComponent<CharacterJoint>().connectedBody = trailer;
    transform.parent.GetComponent<CharacterJoint>().anchor = connectPosition;
  }

  private void OnBecameInvisible()
  {
    //trailer.interpolation = RigidbodyInterpolation.None;
    axisCarController.IsVisible = false;
    transform.parent.rigidbody.interpolation = RigidbodyInterpolation.None;
    trailer.gameObject.SetActive(false);
    bigCollider.enabled = true;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Z))
      OnBecameVisible();
    if (Input.GetKeyDown(KeyCode.X))
      OnBecameInvisible();
  }
}
