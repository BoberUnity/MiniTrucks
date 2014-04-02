//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {
	public CarCamerasController carCamerasController;
	public CarCamerasController mapCameraController;
	GameObject StartTimer;
	SettingsMenu settingsMenu;
	public GameObject[] cars;
	GameObject selectedCar;
	bool altNormalForce;
	public Skidmarks skidmarks;
	public float fixedTimeStep=0.02f;
	int index=0;
	int lastIndex;
	CarCameras carCameras;
	Light mlight;
	DashBoard dashBoard;
	
	GameObject unityCar;
	
	void Awake () {
		//Application.targetFrameRate = 30;
		if (carCamerasController==null) carCamerasController = Camera.main.GetComponent<CarCamerasController>();
		settingsMenu = GetComponent<SettingsMenu>();
		carCameras = Camera.main.GetComponent<CarCameras>();
		if (mapCameraController==null){
			if (GameObject.FindWithTag("MapCamera")) mapCameraController = GameObject.FindWithTag("MapCamera").GetComponent<CarCamerasController>();
			else if (GameObject.Find("MapCamera")) mapCameraController = GameObject.Find("MapCamera").GetComponent<CarCamerasController>();
		}
		
		object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (object o in obj){
			GameObject g = (GameObject) o;
			if (g.GetComponent<Light>()!=null) mlight=g.GetComponent<Light>();
		}
		
		Time.fixedDeltaTime=fixedTimeStep;
		
		if (cars.Length==0) cars=GameObject.FindGameObjectsWithTag("Car");
		
		foreach (GameObject car in cars) {
			if (car!=null) {
				car.SetActive/*Recursively*/(true);
				if (car.transform.GetComponent<CarDebug>()!=null) car.transform.GetComponent<CarDebug>().enabled=false;
				if (car.transform.GetComponent<Setup>()!=null) car.transform.GetComponent<Setup>().enabled=false;
				if (car.transform.GetComponent<CarDynamics>().skidmarks==null){
					Skidmarks skidclone;
					if (skidmarks) {
						skidclone=Instantiate(skidmarks, Vector3.zero, Quaternion.identity) as Skidmarks;
						car.transform.GetComponent<CarDynamics>().skidmarks=skidclone;
					}
				}
			}
		}
	}
	
	void Start(){
		foreach (GameObject car in cars) {
			if (car!=null) DisableObject(car);
		}
		if (cars.Length!=0 && cars[0]!=null) {
			lastIndex=-1;
			selectedCar=ChangeCar(0,lastIndex);
		}
	}	
	
	void DisableObjects(GameObject selectedCar){
		foreach (GameObject car in cars){
			if (car !=selectedCar) DisableObject(car); //car.SetActiveRecursively(false);
		}
	}
	
	void DisableObject(GameObject car){
		if (settingsMenu!=null){ if (settingsMenu.stressTest==false) car.transform.GetComponent<CarDynamics>().SetController("external");}
		else car.transform.GetComponent<CarDynamics>().SetController("external");
/* 		foreach(Wheel w in car.transform.GetComponent<CarDynamics>().allWheels){
			w.tirePressureEnabled=false;
		}
 */		dashBoard=car.GetComponentInChildren<DashBoard>();
		if (dashBoard!=null) dashBoard.gameObject.SetActive(false);
		if (car.transform.GetComponent<CarDebug>()!=null) car.transform.GetComponent<CarDebug>().enabled=false;
		if (settingsMenu!=null) settingsMenu.ChangeCar(null);
	}
	
	void EnableObject(GameObject car){
		car.SetActive/*Recursively*/(true);
		if (car.transform.GetComponent<Setup>()!=null) car.transform.GetComponent<Setup>().enabled=true;
		if (settingsMenu!=null){ if (settingsMenu.stressTest==false) car.transform.GetComponent<CarDynamics>().SetController("axis");}
		else car.transform.GetComponent<CarDynamics>().SetController("axis");
/* 		if (Time.fixedDeltaTime<=0.02f) {
			foreach(Wheel w in car.transform.GetComponent<CarDynamics>().allWheels){
				w.tirePressureEnabled=true;
				w.SetTireStiffness();
			}
		} */
		dashBoard=car.GetComponentInChildren<DashBoard>();
		if (dashBoard!=null) dashBoard.gameObject.SetActive(true);
		if (settingsMenu!=null) StartCoroutine(settingsMenu.ChangeCar(car));
	}	
	
	void SelectNextCar(bool next){
		lastIndex=index;
		if (next==true) index++;
		else index--;
		if (index>cars.Length-1) index=0;
		if (index<0) index=cars.Length-1;
		selectedCar=ChangeCar(index, lastIndex);	
	}
	
	void Update(){
		if (cars.Length>=1){
			if (Input.GetKeyUp (KeyCode.PageUp)) {
				SelectNextCar(true);
			}
			else if (Input.GetKeyUp (KeyCode.PageDown)) {
				SelectNextCar(false);
			}			
		}
		
		if (Input.GetKeyUp (KeyCode.M)){
      if (mapCameraController != null) mapCameraController.gameObject.SetActive(!mapCameraController.gameObject.activeSelf);
		}
		
 		if (Input.GetKeyDown (KeyCode.F10) && settingsMenu.stressTest==true){
			StartCoroutine(CreateCar());
		}

		if (Input.GetKeyDown (KeyCode.F12) && mlight!=null){
			if (mlight.shadows == LightShadows.None) mlight.shadows = LightShadows.Soft;
			else mlight.shadows = LightShadows.None;
		}
		
		if (dashBoard!=null){
			if (carCameras.driverView==true) dashBoard.showGUIDashboard=false;
			else dashBoard.showGUIDashboard=true;
		}
	}
	
	IEnumerator CreateCar(){
		unityCar=UnityCar.CreateNewCar();
		int sign=(Random.value<0.5f?-1:1);
		unityCar.transform.position=Camera.main.transform.TransformPoint(Vector3.forward*(10+Random.value*10) + Vector3.up*3 + sign*Vector3.right*Random.value*10);
		unityCar.transform.eulerAngles=new Vector3(0, Camera.main.transform.eulerAngles.y,0);
		Resize(ref cars,cars.Length+1);
		cars[cars.Length-1]=unityCar;
		settingsMenu.carsNumber=cars.Length;
		
		if (carCameras.target==null) {
			carCameras.target=unityCar.transform;
			while (unityCar.GetComponent<CarDynamics>()==null) yield return new WaitForSeconds(0.02f);
			SelectNextCar(true);	
		}
	}
	
	GameObject ChangeCar(int index, int lastIndex){
		if (lastIndex!=-1) DisableObject(cars[lastIndex]);
		selectedCar=cars[index];
		EnableObject(selectedCar);
		carCamerasController.externalSizex=carCamerasController.externalSizey=carCamerasController.externalSizez=0;
		if (selectedCar.transform.tag=="Truck" && selectedCar.transform.GetComponent<CharacterJoint>()!=null) carCamerasController.externalSizez=6f;
		carCamerasController.SetCamera(0,selectedCar.transform, true);
		if (mapCameraController!=null) {
			mapCameraController.SetCamera(7,selectedCar.transform, true);
		}
		return selectedCar;
	}
	
	public static void Resize<T>(ref T[] array, int newSize){
		T[] sourceArray = array;
		if (sourceArray == null){
			array = new T[newSize];
		}
		else if (sourceArray.Length != newSize){
			T[] destinationArray = new T[newSize];
			sourceArray.CopyTo(destinationArray, 0);
			array = destinationArray;
		}
	}	
}