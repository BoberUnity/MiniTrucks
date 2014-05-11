using System;
using UnityEngine;

public class ButtonAddTrailer : MonoBehaviour
{
  [SerializeField] private GameObject trailer = null;
  [SerializeField] private Vector3 connectPosition = new Vector3(0, 0.41f, -1.7f);
  [SerializeField] private RaceStart raceStart = null;
  [SerializeField] private RaceStart raceFinish = null;
  //[SerializeField] private Transform characterPos = null;//Позиция на старте гонки
  [SerializeField] private Waypoint way = null;
  [SerializeField] private BaggageLabel baggageLabel = null;
  [SerializeField] private int price = 100;
  //[SerializeField] private UILabel fragilityLabel = null;

  private Transform truckCar = null;
  [SerializeField]
  private GameObject[] enemies = null;
  
  public Transform TruckCar//Трактор
  {
    set { truckCar = value; }
  }

  private void Start()
  {
    raceFinish.Finish += DestroyEnemies;
    //fragilityLabel.text = trailer.GetComponentInChildren<Trailer>().Frailty.ToString("f0");
  }

  private void OnDestroy()
  {
    raceFinish.Finish -= DestroyEnemies;
  }

	protected virtual void OnPress(bool isPressed)
	{
    if (!isPressed)
	  {
      truckCar.position = raceStart.CharPos.position;
      truckCar.rotation = raceStart.CharPos.rotation;
      
      baggageLabel.BaggageController = truckCar.GetComponentInChildren<BlowController>();
      GameObject t = Instantiate(trailer, Vector3.zero, Quaternion.identity) as GameObject;
      Trailer tr = t.GetComponentInChildren<Trailer>();  //Находим прицепа, 
      if (tr != null)
      {
        tr.transform.position = truckCar.position;
        tr.transform.rotation = truckCar.rotation;
        //baggageLabel.BaggageController = tr.GetComponentInChildren<BlowController>();
        truckCar.gameObject.AddComponent<CharacterJoint>();
        truckCar.GetComponent<CharacterJoint>().connectedBody = tr.GetComponentInChildren<Rigidbody>();
        truckCar.GetComponent<CharacterJoint>().anchor = connectPosition;
        SoftJointLimit softJointLimit = new SoftJointLimit();
        softJointLimit.limit = 25;
        truckCar.GetComponent<CharacterJoint>().highTwistLimit = softJointLimit;//вертикальный сустав
        softJointLimit.limit = 110;
        truckCar.GetComponent<CharacterJoint>().swing1Limit = softJointLimit;//горизонтальный сустав
        truckCar.GetComponent<BlowController>().Frailty = tr.Frailty;//Передаем хрупкость на тягач
        truckCar.GetComponent<BlowController>().Condition = 100;
      }
      Array.Resize(ref enemies, raceStart.enemiesPos.Length);
	    int i = 0;
      foreach (var e in raceStart.enemiesPos)//Создаем соперников
      {
        GameObject enemy = Instantiate(raceStart.enemiesPos[i].EnemyPref, e.EnemyPos.position, e.EnemyPos.rotation) as GameObject;
        if (enemy != null)
        {
          enemy.GetComponentInChildren<AxisCarController>().SetWay(way);
          enemies[i] = enemy;
          i += 1;
        }
        else Debug.LogWarning("opp == null");
      }
      raceFinish.Activ = true;
      raceStart.ExitStation();
      raceStart.ClockOn();
	    raceFinish.Price = price;
	  }
	}

  private void DestroyEnemies()
  {
    foreach (GameObject enemy in enemies)
    {
      Destroy(enemy);
    }
  }

  public void ExitRace()//Из меню паузы
  {
    Debug.LogWarning("ExitRace");
    if (raceFinish.Activ)
    {
      Debug.LogWarning("ExitRace active");
      DestroyEnemies();
      CharacterJoint characterJoint = truckCar.GetComponent<CharacterJoint>();
      if (characterJoint != null)
      {
        Destroy(characterJoint.connectedBody.gameObject.transform.parent.parent.gameObject);
        Destroy(characterJoint);
      }
      else Debug.LogWarning("CharacterJoint == null");
      baggageLabel.BaggageController = null;
    }
  }
}
