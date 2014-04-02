using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class ShowFps : MonoBehaviour
{
  [SerializeField] private UILabel uILabel = null; 

  public float updateInterval = 0.5F;
  public bool Tormoz;
  public float LowFPS;
  public int Step = 30;
  private float accum   = 0; // FPS accumulated over the interval
  private int   frames  = 0; // Frames drawn over the interval
  private float timeleft; // Left time for current interval
  private float fps;

  private void Update()
  {
    timeleft -= Time.deltaTime;
    accum += Time.timeScale/Time.deltaTime;
    ++frames;
    Step += 1;
    if( timeleft <= 0.0f )
    {
      fps = accum/frames;
      
      //if (fps > LowFPS) {Step += 1;};
      //if (fps < LowFPS) {Step -= 1;};

      uILabel.text = "FPS: " + fps.ToString("f0");

      timeleft = updateInterval;
      accum = 0.0F;
      frames = 0;
    }
	
    if (Tormoz)
	  {
		  for(var i = 0; i < Step; i++)
			  {Debug.Log("Enable Tormoz");}
	  }

    if (Input.GetKeyDown(KeyCode.Escape))
      Application.Quit();
  }
}
 
