using UnityEngine;

public class GoldIndicator : MonoBehaviour
{
  [SerializeField] private SelectCarController selectCarController = null;
  private UILabel uILabel = null;

  private void Start()
  {
    selectCarController.ChangeGold += ChangeGold;
    uILabel = GetComponent<UILabel>();
    uILabel.text = selectCarController.Gold.ToString("f0");
  }

  private void OnDestroy()
  {
    selectCarController.ChangeGold -= ChangeGold;
  }
	
  private void ChangeGold(int gold)
  {
    uILabel.text = gold.ToString("f0");
  }
	// Update is called once per frame
	void Update () {
	
	}
}
