using UnityEngine;

public class Nitro : MonoBehaviour
{
  [SerializeField] private float mass = 30;
  private ButtonThrottle buttonThrottle = null;
  
  private void Start()
  {
    buttonThrottle = GameObject.Find("ButtonNitro").GetComponent<ButtonThrottle>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "TraktorEnemy")
    {

    }

    if (other.gameObject.name == "Traktor")
    {
      buttonThrottle.NitroFuel = Mathf.Min(100, buttonThrottle.NitroFuel + mass);
      Destroy(gameObject);
    }
  }
}
