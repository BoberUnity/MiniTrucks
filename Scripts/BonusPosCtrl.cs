using System;
using UnityEngine;

public class BonusPosCtrl : MonoBehaviour 
{
  [SerializeField] private GameObject[] box = null;
  [SerializeField] private int boxNums = 2;
  [SerializeField] private GameObject money = null;
  [SerializeField] private int moneyNums = 2;
  [SerializeField] private GameObject nitro = null;
  [SerializeField] private int nitroNums = 2;
  [SerializeField] private Transform[] positions = null;
  [SerializeField] private bool[] fulls = null;
  [SerializeField] private GameObject[] createdObjs = null;

	public void CreateBonuses(int cargoId)
	{
    positions = GetComponentsInChildren<Transform>();
    Array.Resize(ref fulls, positions.Length);
    for (int i = 0; i < boxNums; i++)//Создаем коробки
    {
      var id = Mathf.Abs(UnityEngine.Random.Range(1, positions.Length));
      if (!fulls[id])
      {
        Array.Resize(ref createdObjs, createdObjs.Length+1);
        createdObjs[createdObjs.Length-1] = Instantiate(box[cargoId], positions[id].position, Quaternion.identity) as GameObject;
        fulls[id] = true;
      }
	  }

    for (int i = 0; i < moneyNums; i++)//Создаем money
    {
      var id = Mathf.Abs(UnityEngine.Random.Range(1, positions.Length));
      if (!fulls[id])
      {
        Array.Resize(ref createdObjs, createdObjs.Length + 1);
        createdObjs[createdObjs.Length - 1] = Instantiate(money, positions[id].position, Quaternion.identity) as GameObject;
        fulls[id] = true;
      }
    }

    for (int i = 0; i < nitroNums; i++)//Создаем nitro
    {
      var id = Mathf.Abs(UnityEngine.Random.Range(1, positions.Length));
      if (!fulls[id])
      {
        Array.Resize(ref createdObjs, createdObjs.Length + 1);
        createdObjs[createdObjs.Length - 1] = Instantiate(nitro, positions[id].position, Quaternion.identity) as GameObject;
        fulls[id] = true;
      }
    }
	}

  public void DeleteBonuses()
  {
    foreach (var obj in createdObjs)
    {
      Destroy(obj);
    }
  }
}
