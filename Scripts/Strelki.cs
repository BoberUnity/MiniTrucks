using UnityEngine;

public class Strelki : MonoBehaviour 
{
  public Drivetrain drivetrain = null;
  [SerializeField] private UISprite speedoStrelka = null;
  [SerializeField] private UISprite tachoStrelka = null;
  [SerializeField] private UILabel speedIndicator = null;
  [SerializeField] private UILabel gearIndicator = null;
  //private Transform sStrelkaTransform = null;
  //private Transform tStrelkaTransform = null;
  	
  //private void Start()
  //{
  //  sStrelkaTransform = speedoStrelka.transform;
  //  tStrelkaTransform = tachoStrelka.transform;
  //}

	private void Update () 
  {
    if (drivetrain != null)
    {
      speedoStrelka.transform.eulerAngles = new Vector3(0, 0, 317 - drivetrain.velo * 6.7f);
	    speedIndicator.text = (drivetrain.velo*2.2f).ToString("f0");
      tachoStrelka.transform.eulerAngles = new Vector3(0, 0, 308 - drivetrain.rpm * 0.05f);
      if (drivetrain.gear > 1)
        gearIndicator.text = (drivetrain.gear - 1).ToString("f0");
      else
      {
        if (drivetrain.gear == 1)
          gearIndicator.text = "N";
        else 
          gearIndicator.text = "R";
      }
    }
  }
}
