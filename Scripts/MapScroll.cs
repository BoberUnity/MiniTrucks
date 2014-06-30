using UnityEngine;

public class MapScroll : MonoBehaviour
{
  public Transform Truck = null;
  [SerializeField] private float scale = 0.2f;
  [SerializeField] private float offsetX = 0;
  [SerializeField] private float offsetY = 0;
  //private Material mapMaterial = null;

	void Start ()
	{
	  //mapMaterial = renderer.material;
	}
	
	void Update () 
  {
    if (Truck != null)
    {
      //Vector2 offset = new Vector2(Truck.position.x / scale + offsetX, Truck.position.z / scale + offsetY);
      //mapMaterial.SetTextureOffset("_MainTex", offset);
      transform.position = new Vector3(-Truck.position.x * scale + offsetX, -Truck.position.z * scale + offsetY, 0);
    }
	}
}
