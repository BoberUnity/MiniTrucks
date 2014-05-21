using System.Collections;
using UnityEngine;

public class CarZona : MonoBehaviour
{
  [SerializeField] private GameObject carTrafik = null;
  private bool carActive = false;
  
  private void OnBecameInvisible()
  {
    if (carActive)
      StartCoroutine(DisableCar(3));
  }

  private void OnBecameVisible()
  {
    carTrafik.SetActive(true);
    carActive = true;
    StopAllCoroutines();
  }

  private void Update()
  {
    if (carActive)
    {
      transform.position = carTrafik.transform.position;
      transform.rotation = carTrafik.transform.rotation;
    }
  }

  private IEnumerator DisableCar(float time)
  {
    yield return new WaitForSeconds(time);
    if (carActive)
    {
      carTrafik.SetActive(false);
      carActive = false;
    }
  }

  private void OnApplicationQuit()
  {
    carActive = false;
  }
}
