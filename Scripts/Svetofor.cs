using UnityEngine;
using System.Collections;

public class Svetofor : MonoBehaviour
{
  [SerializeField] private LensFlare green1 = null;
  [SerializeField] private LensFlare green2 = null;
  [SerializeField] private LensFlare red1 = null;
  [SerializeField] private LensFlare red2 = null;
  [SerializeField] private LensFlare yellow1 = null;
  [SerializeField] private LensFlare yellow2 = null;
  [SerializeField] private float updateInterval = 8;
  private int colorLight = 0;
	
	void Start ()
	{
	  Off();
	  StartCoroutine(ChangeColor(0.1f));
	}
	
	private IEnumerator ChangeColor(float time)
	{
	  yield return new WaitForSeconds(time);
    Off();
    if (colorLight == 0)
    {
      green1.enabled = true;
      green2.enabled = true;
    }
    if (colorLight == 1 || colorLight == 3)
    {
      yellow1.enabled = true;
      yellow2.enabled = true;
    }
    if (colorLight == 2)
    {
      red1.enabled = true;
      red2.enabled = true;
    }
	  colorLight += 1;
    if (colorLight > 3)
      colorLight = 0;
    StartCoroutine(ChangeColor(updateInterval));
	}

  private void Off()
  {
    green1.enabled = false;
    green2.enabled = false;
    red1.enabled = false;
    red2.enabled = false;
    yellow1.enabled = false;
    yellow2.enabled = false;
  }
}
