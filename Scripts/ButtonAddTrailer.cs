using UnityEngine;

public class ButtonAddTrailer : MonoBehaviour
{
  [SerializeField] private GameObject trailer = null;
  [SerializeField] private Vector3 connectPosition = new Vector3(0, 0.41f, -1.7f);
  [SerializeField] private RaceStart raceStart = null;
  [SerializeField] private Transform characterPos = null;//Позиция на старте гонки
  [SerializeField] private Transform truckCar = null;
  
  public Transform TruckCar
  {
    set { truckCar = value; }
  }

	protected virtual void OnPress(bool isPressed)
	{
	  if (!isPressed)
	  {
      truckCar.position = characterPos.position;
      truckCar.rotation = characterPos.rotation;
      GameObject t = Instantiate(trailer, Vector3.zero, Quaternion.identity) as GameObject;
      Trailer tr = t.GetComponentInChildren<Trailer>();  //Находим прицепа, 
      if (tr != null)
      {
        tr.transform.position = truckCar.position;
        tr.transform.rotation = truckCar.rotation;
        truckCar.gameObject.AddComponent<CharacterJoint>();
        truckCar.GetComponent<CharacterJoint>().connectedBody = tr.GetComponentInChildren<Rigidbody>();
        truckCar.GetComponent<CharacterJoint>().anchor = connectPosition;
      }
      raceStart.ExitStation();
	  }
	}
}
