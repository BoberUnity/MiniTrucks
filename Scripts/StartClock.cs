using UnityEngine;
using System.Collections;

public class StartClock : MonoBehaviour
{
  [SerializeField] private RaceStart raceStart = null;
  private UILabel uILabel = null;
	// Use this for initialization
	void Start ()
	{
	  uILabel = GetComponent<UILabel>();
	}
	
	public void ClockOn()
	{
	  uILabel.text = "3";
	  StartCoroutine(Show2(1));
	}

  private IEnumerator Show2(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "2";
    StartCoroutine(Show1(1));
  }

  private IEnumerator Show1(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "1";
    StartCoroutine(Show0(1));
  }

  private IEnumerator Show0(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "0";
    StartCoroutine(ClockOff(1));
  }

  private IEnumerator ClockOff(float time)
  {
    yield return new WaitForSeconds(time);
    uILabel.text = "";
  }
}
