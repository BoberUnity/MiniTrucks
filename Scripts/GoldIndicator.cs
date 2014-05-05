using UnityEngine;

public class GoldIndicator : MonoBehaviour
{
  [SerializeField] private SelectCarController selectCarController = null;
  private UILabel uILabel = null;
  private bool changing = false;
  private float changTime = 0;
  private int currentGold = 0;
  private int oldGold = 0;

  private void Start()
  {
    selectCarController.ChangeGold += ChangeGold;
    uILabel = GetComponent<UILabel>();
    uILabel.text = selectCarController.Gold.ToString("f0");
    oldGold = selectCarController.Gold;
  }

  private void OnDestroy()
  {
    selectCarController.ChangeGold -= ChangeGold;
  }
	
  private void ChangeGold(int gold)
  {
    changing = true;
    currentGold = gold;
    changTime = 0;
  }

  void Update () 
  {
	  if (changing)
	  {
	    changTime += Time.deltaTime;
      uILabel.text = (currentGold + (oldGold - currentGold)*(1-changTime)).ToString("f0");
      if (changTime > 1)
      {
        uILabel.text = currentGold.ToString("f0");
        changing = false;
        oldGold = currentGold;
      }
	  }
	}
}
