using UnityEngine;

public class ButtonMap : MonoBehaviour
{
  [SerializeField] private UIButton[] disableButtons = null;
  [SerializeField] private ButtonHandler map = null;
  [SerializeField] private GameObject mapCamera = null;
  
  private void Start()
  {
    map.Pressed += MapPressed;
  }

  private void OnDestroy()
  {
    map.Pressed -= MapPressed;
  }

  protected virtual void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      foreach (var button in disableButtons)
      {
        button.isEnabled = false;
      }
      //map.gameObject.SetActive(true);
      Transform[] mapObjs = mapCamera.GetComponentsInChildren<Transform>();
      foreach (var mapObj in mapObjs)
      {
        mapObj.gameObject.layer = 28;
      }
      
      mapCamera.SetActive(true);

    }
	}

  private void MapPressed()//При нажатии на развернутую карту
  {
    foreach (var button in disableButtons)
    {
      button.isEnabled = true;
    }
    //map.gameObject.SetActive(false);
    Transform[] mapObjs = mapCamera.GetComponentsInChildren<Transform>();
    foreach (var mapObj in mapObjs)
    {
      mapObj.gameObject.layer = 30;
    }
    //mapCamera.SetActive(false);
  }
}

