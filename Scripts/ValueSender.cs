using UnityEngine;

public class ValueSender : MonoBehaviour
{
  [SerializeField] private SelectCarController selectCarController = null;
  [SerializeField] private int id = 0;//0-effects; 1-music
  private UISlider uISlider = null;

	private void Start()
	{
	  uISlider = GetComponent<UISlider>();
    if (id == 0)
    {
      if (PlayerPrefs.HasKey("VolumeEffects"))
        uISlider.value = PlayerPrefs.GetFloat("VolumeEffects");
    }
    if (id == 1)
    {
      if (PlayerPrefs.HasKey("VolumeMusic"))
        uISlider.value = PlayerPrefs.GetFloat("VolumeMusic");
    }
	}
	
	private void Update() 
  {
	  if (id == 0)
	    selectCarController.VolumeEffects = uISlider.value;
    if (id == 1)
      selectCarController.VolumeMusic = uISlider.value;
	}
  private void OnApplicationQuit()
  {
    if (id == 0)
    {
      PlayerPrefs.SetFloat("VolumeEffects", selectCarController.VolumeEffects);
    }
    if (id == 1)
    {
      PlayerPrefs.SetFloat("VolumeMusic", selectCarController.VolumeMusic);
    }
  }
}
