using System.Collections;
using UnityEngine;

public class Cargo : MonoBehaviour
{
  private bool bonus = false;
  private float toBonusTime = 2.5f;
  [HideInInspector] public float AddCondition = 5;//Изменяется из BlowController
  private bool rayCar = false;

  void Start ()
	{
    StartCoroutine(CheckChangeToBonus(toBonusTime));
	}
	
  private IEnumerator CheckChangeToBonus(float time)
  {
    yield return new WaitForSeconds(time);
    if (Physics.Raycast(transform.position, -Vector3.up, 3, ~(1 << 17))) //layer Car1)
      ChangeToBonus();
    else
      StartCoroutine(CheckChangeToBonus(0.5f));
  }

  private void ChangeToBonus()
  {
    bonus = true;
    GetComponent<BoxCollider>().isTrigger = true;
    Destroy(rigidbody);
    gameObject.layer = 1 << 1;
  }

  private void OnBecameVisible()//On
  {
    if (bonus)
      StopAllCoroutines();
  }
  
  private void OnBecameInvisible()//Off
  {
    if (bonus && gameObject.activeSelf)
    {
      StopAllCoroutines();
      StartCoroutine(DestroyMe(5));
    }
  }

  private IEnumerator DestroyMe(float time)
  {
    yield return new WaitForSeconds(time);
    Destroy(gameObject);
  }
}
