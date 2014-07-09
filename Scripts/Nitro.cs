using UnityEngine;

public class Nitro : MonoBehaviour
{
  [SerializeField] private float mass = 30;
  [SerializeField] private bool destroyed = false;
  private ButtonThrottle buttonThrottle = null;
  
  private void Start()
  {
    buttonThrottle = GameObject.Find("ButtonNitro").GetComponent<ButtonThrottle>();
    gameObject.layer = 1 << 1;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name.Substring(0, 2) == "TE")
    {
      if (destroyed)
        Destroy(gameObject);
    }

    if (other.gameObject.name == "Traktor")
    {
      buttonThrottle.NitroFuel = Mathf.Min(100, buttonThrottle.NitroFuel + mass);
      if (destroyed) 
        Destroy(gameObject);
    }
  }
}
