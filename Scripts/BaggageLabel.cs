using UnityEngine;

public class BaggageLabel : MonoBehaviour 
{
  private BlowController baggageController = null;
  private UILabel uILabel = null;

  public BlowController BaggageController
  {
    set { baggageController = value; }
  }
	
  void Start()
  {
    uILabel = GetComponent<UILabel>();
  }

	void Update ()
	{
    if (baggageController != null)
      uILabel.text = "baggage condition - " + baggageController.Condition.ToString("f0") + "%";
	}
}
