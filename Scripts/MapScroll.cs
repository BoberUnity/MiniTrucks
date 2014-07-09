using System;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
  public Transform Truck = null;
  [SerializeField] private Transform truckSprite = null;
  [SerializeField] private float scale = 0.2f;
  [SerializeField] private float offsetX = 0;
  [SerializeField] private float offsetY = 0;
  [SerializeField] private GameObject[] FinishObjs = null;
  [SerializeField] private int activFinis = -1;
  public bool Big = false;
  [SerializeField] private float cl = 0.2f;
  public Transform[] Enemies = null;
  [SerializeField] private Transform[] enemySprites;
  private Vector3[] finishPoses = null;

  public int ActivFinis
  {
    get { return activFinis;}
    set
    {
      
      if (value == -1)
      {
        foreach (var finishObj in FinishObjs)
        {
          finishObj.SetActive(false);
        }

        foreach (var sprite in enemySprites)
        {
          sprite.position = Vector3.right*2000;
        }
      }
      activFinis = value;
      
    }
  }

  private void Start()
  {
    Array.Resize(ref Enemies, 4);
  }

	void OnEnable ()
	{
	  int i = 0;
    foreach (var finishObj in FinishObjs)
	  {
      finishObj.SetActive(ActivFinis == i);
	    i++;
	  }
    Array.Resize(ref finishPoses, FinishObjs.Length);
	  i = 0;
    for (int j = 0; j < finishPoses.Length; j++)
	  {
      finishPoses[j] = FinishObjs[j].transform.localPosition;
	  }
	}
	
	void Update () 
  {
    if (Truck != null)
    {
      truckSprite.eulerAngles = new Vector3(0, 0, -Truck.eulerAngles.y);
      if (Big)
      {
        transform.position = Vector3.zero;
        truckSprite.position = new Vector3(Truck.position.x * scale - offsetX, Truck.position.z * scale - offsetY, 0);
        if (Enemies[0] != null)
        {
          int i = 0;
          foreach (var sprite in enemySprites)
          {
            sprite.position = new Vector3(Enemies[i].position.x*scale - offsetX, Enemies[i].position.z*scale - offsetY, 0);
            i += 1;
          }
        }
        if (ActivFinis > -1)
          FinishObjs[ActivFinis].transform.position = new Vector3(finishPoses[ActivFinis].x / 400 + transform.position.x, finishPoses[ActivFinis].y / 400 + transform.position.y, 0);
      }
      else
      {
        truckSprite.position = Vector3.zero;
        if (Enemies[0] != null)
        {
          int i = 0;
          foreach (var sprite in enemySprites)
          {
            sprite.position = new Vector3(transform.position.x + Enemies[i].position.x * scale - offsetX, transform.position.y + Enemies[i].position.z * scale - offsetY, 0);
            i += 1;
          }
        }
        transform.position = new Vector3(-Truck.position.x * scale + offsetX, -Truck.position.z * scale + offsetY, 0);
        if (ActivFinis > -1)
          FinishObjs[ActivFinis].transform.position = new Vector3(Mathf.Clamp(finishPoses[ActivFinis].x / 400 + transform.position.x, -cl, cl), Mathf.Clamp(finishPoses[ActivFinis].y / 400 + transform.position.y, -cl, cl), 0);
      }
    }
	}
}
