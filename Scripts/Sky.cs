using UnityEngine;

public class Sky : MonoBehaviour
{
  [SerializeField] private Transform levelCamera = null;
  private Transform thistransform = null;
  private float y = 0;

  private void Start()
  {
    thistransform = transform;
    y = transform.position.y;
  }

  private void Update()
	{
	  thistransform.position = new Vector3(levelCamera.position.x, y, levelCamera.position.z);
	}
}
