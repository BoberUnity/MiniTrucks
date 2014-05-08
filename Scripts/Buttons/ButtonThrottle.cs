using UnityEngine;

public class ButtonThrottle : MonoBehaviour 
{
  public AxisCarController axisCarController = null;
  [SerializeField] private float nitroTime = 10;//на сколько хватает баллона
  [SerializeField] private UILabel nitroLabel = null;
  [SerializeField] private float nitroFuel = 50;
  private bool nitroUsing = false;

  public float NitroFuel
  {
    set
    {
      nitroFuel = value;
      nitroLabel.text = nitroFuel.ToString("f0");
    }

    get { return nitroFuel; }
  }

  protected virtual void OnPress(bool isPressed)
  {
    //axisCarController.NitroUsed = isPressed;
    if (isPressed && nitroFuel > 0)
    {
      axisCarController.NitroUsed = true;
      nitroUsing = true;
    }

    if (!isPressed)
    {
      axisCarController.NitroUsed = false;
      StopAllCoroutines();
      nitroUsing = false;
    }
  }
  
  private void Update()
  {
    if (nitroUsing)
    {
      nitroFuel -= Time.deltaTime * 100/ nitroTime;
      nitroLabel.text = nitroFuel.ToString("f0");
      if (nitroFuel < 0)
      {
        axisCarController.NitroUsed = false;
        nitroUsing = false;
      }
    }
    
    if (Input.GetKeyDown("down"))
      axisCarController.BrakeUsed = true;
    if (Input.GetKeyUp("down"))
      axisCarController.BrakeUsed = false;

    if (Input.GetKeyDown("up"))
      axisCarController.NitroUsed = true;
    if (Input.GetKeyUp("up"))
      axisCarController.NitroUsed = false;
  }
}

