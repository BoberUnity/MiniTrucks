using UnityEngine;

public class BaggageLabel : MonoBehaviour
{
  public BonusPosCtrl bonusPosCtrl = null;
  private BlowController baggageController = null;
  private UILabel uILabel = null;

  public BlowController BaggageController
  {
    set
    {
      baggageController = value;
      if (baggageController == null)
        uILabel.text = "baggage condition - 0 %";
    }
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
