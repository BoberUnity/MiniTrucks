using UnityEngine;
using System.Collections;

public class StartClock : MonoBehaviour
{
  //[SerializeField] private RaceStart raceStart = null;
  private UILabel uILabel = null;
	// Use this for initialization
	private void Start ()
	{
	  uILabel = GetComponent<UILabel>();
	}

  private void OnEnable()
  {
    if (uILabel != null)
    {
      //Если была нажата пауза
      uILabel.text = "";
    }
  }

	
	public void ClockOn()
	{
	  uILabel.text = "3";
    StartCoroutine(Show2(0.25f));//должно быль 1 и RaceStart 4 s
	}

  private IEnumerator Show2(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "2";
    StartCoroutine(Show1(0.25f));//должно быль 1 и RaceStart 4 s
  }

  private IEnumerator Show1(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "1";
    StartCoroutine(Show0(0.25f));//должно быль 1 и RaceStart 4 s
  }

  private IEnumerator Show0(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "0";
    StartCoroutine(ClockOff(0.25f));//должно быль 1 и RaceStart 4 s
  }

  private IEnumerator ClockOff(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "";
  }
}
