using System;
using UnityEngine;

public class SetPosition : MonoBehaviour 
{
  [Serializable] private class MovedObjs
  {
    public Transform obj = null;
    public Vector2 pos = Vector2.zero;
  }

  [SerializeField] private int id = 0;
  [SerializeField] private SetToggleControls setToggleControls = null;
  [SerializeField] private MovedObjs[] moveObjs = null;

  private void Start()
  {
    if (PlayerPrefs.GetInt("ControlsParams") == id)
      OnPress(false);
  }

  /*protected virtual*/public void OnPress(bool isPressed)
  {
    if (!isPressed)
    {
      foreach (var moveObj in moveObjs)
      {
        moveObj.obj.position = new Vector3(moveObj.pos.x / 400, moveObj.obj.position.y, 0);
      }
      PlayerPrefs.SetInt("ControlsParams", id);
      setToggleControls.currentControls = id;
    }
  }
}
