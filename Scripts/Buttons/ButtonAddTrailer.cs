using System;
using UnityEngine;

public class ButtonAddTrailer : MonoBehaviour
{
  [SerializeField] private int id = 0;
  [SerializeField] private int cargoId = 0;
  [SerializeField] private GameObject trailer = null;
  [SerializeField] private Vector3 connectPosition = new Vector3(0, 0.41f, -1.7f);
  [SerializeField] private RaceStart raceStart = null;
  [SerializeField] private RaceStart raceFinish = null;
  [SerializeField] private Waypoint way = null;
  [SerializeField] private BaggageLabel baggageLabel = null;
  [SerializeField] private ButtonThrottle buttonThrottle = null;
  [SerializeField] private int price = 100;
  [SerializeField] private UISprite[] medals = null;

  private int medal = 10;
  public int Medal
  {
    get { return medal;}
    set
    {
      if (value < medal)
      {
        foreach (var m in medals)
        {
          m.gameObject.SetActive(false);
        }
        medal = value;
        medals[value].gameObject.SetActive(true);
        PlayerPrefs.SetInt(id.ToString("f0"), medal);
        //Debug.LogWarning("Key medal created");
      }
      if (value > 3)//Отключение медалей при начале новой игры
      {
        foreach (var m in medals)
        {
          m.gameObject.SetActive(false);
        }
        medal = value;
      }
    }
  }
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
    if (PlayerPrefs.HasKey(id.ToString("f0")))
      Medal = PlayerPrefs.GetInt(id.ToString("f0"));
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
        CharacterJoint characterJoint = truckCar.GetComponent<CharacterJoint>();
        characterJoint.connectedBody = tr.GetComponentInChildren<Rigidbody>();
        characterJoint.anchor = connectPosition;
        SoftJointLimit softJointLimit = new SoftJointLimit();
        //softJointLimit.limit = 0;
        //characterJoint.lowTwistLimit = softJointLimit;
        softJointLimit.limit = 25;
        characterJoint.highTwistLimit = softJointLimit;//вертикальный сустав
        softJointLimit.limit = 105;
        characterJoint.swing1Limit = softJointLimit;//горизонтальный сустав
        //softJointLimit.limit = 5;
        //truckCar.GetComponent<CharacterJoint>().swing2Limit = softJointLimit;//left/right
        truckCar.GetComponent<BlowController>().Frailty = tr.Frailty;//Передаем хрупкость на тягач
        truckCar.GetComponent<BlowController>().Condition = 100;
        truckCar.GetComponent<AxisCarController>().Trailer = tr.GetComponentInChildren<Rigidbody>();
        truckCar.GetComponent<AxisCarController>().SignalTrailerLeft = tr.SignalLeft;
        truckCar.GetComponent<AxisCarController>().SignalTrailerRight = tr.SignalRight;
        truckCar.GetComponentInChildren<BlowController>().TrailerTransform = tr.GetComponentInChildren<Rigidbody>().transform;
        truckCar.GetComponentInChildren<BlowController>().Box = tr.Box;
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
	    raceFinish.buttonAddTrailer = this;
	    buttonThrottle.NitroFuel = 100;
      //
	    CarZona[] carZones = FindObjectsOfType<CarZona>();
	    foreach (var carZone in carZones)
	    {
	      carZone.Enemies = enemies;
	    }
      //Создание бонусов
      baggageLabel.bonusPosCtrl.CreateBonuses(cargoId);
      raceStart.MapCamera.SetActive(true);
	  }
	}
  
  private void DestroyEnemies()
  {
    foreach (GameObject enemy in enemies)
    {
      Destroy(enemy);
    }
  }

  public void ExitRace()//Из меню паузы && Restart
  {
    Debug.LogWarning("ExitRace");
    if (raceFinish.Activ)//выполняется для всех кнопок с финишем в указанном городе
    {
      Debug.LogWarning("ExitRace active");
      if (enemies.Length > 1)//для 1-й кнопки
      {
        raceFinish.Activ = false;
        DestroyEnemies();
        CharacterJoint characterJoint = truckCar.GetComponent<CharacterJoint>();
        if (characterJoint != null)
        {
          Destroy(characterJoint.connectedBody.gameObject.transform.parent.parent.gameObject);
          Destroy(characterJoint);
        }
        else Debug.LogWarning("CharacterJoint == null");
        baggageLabel.bonusPosCtrl.DeleteBonuses();
        baggageLabel.BaggageController = null;
      }
    }
  }
}
