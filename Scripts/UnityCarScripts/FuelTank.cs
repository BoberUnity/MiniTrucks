//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class FuelTank : MonoBehaviour {
	Rigidbody fuelTank;
	BoxCollider boxCollider;
	Drivetrain drivetrain;
	Transform myTransform;
	
	public float _tankCapacity = 50f; // in liters
	public float _currentFuel = 50f; // in liters
	public float _tankWeight=10; // in kg, when empty
	public float _fuelDensity = 0.73722f; // kg/liters. This value is for standard Gasoline (used to calc fuel weight). If density == 0 ---> Fuel has no weight
	
	public float tankWeight{
		get{return _tankWeight;}
		set{
			if (value <= 0f){
				_tankWeight = 0.001f; //to avoid zero mass fuel tank
			}
			else{
				_tankWeight = value;
			}			
		}
	}
	
	// tank capacity in liters. If tankCapacity == 0 ---> Infinite fuel
	public float tankCapacity{
		get{return _tankCapacity;}
		set{
			// First, set the new value for tankCapacity
			if (value < 0f){
				_tankCapacity = 0f;
			}
			else{
				_tankCapacity = value;
			}
			// After that, update (if necessary) the value of currentFuel
			if (_currentFuel > _tankCapacity){
				_currentFuel = _tankCapacity;
			}
		}
	}
	
	// Current fuel in liters
	public float currentFuel{
		get{return _currentFuel;}
		set{
			if (value < 0){
				_currentFuel = 0;
			}
			else if (value > tankCapacity){
				_currentFuel = tankCapacity;
			}
			else{
				_currentFuel = value;
			}
		}
	}
	
	// Fuel density in kg/l. If density == 0 ---> Fuel has no weight
	public float fuelDensity{
		get{return _fuelDensity;}
		set{
			if (value < 0f){
				_fuelDensity = 0f;
			}
			else{
				_fuelDensity = value;
			}
		}
	}
	
	void Start(){
		myTransform=transform;
		
		Transform trs = myTransform.parent;		
		while (trs.GetComponent<Drivetrain>() == null)  trs = trs.parent;
		drivetrain = trs.GetComponent<Drivetrain>();	
		
		// Make sure the property getters methods are called
		tankCapacity = tankCapacity;
		currentFuel = currentFuel;
		fuelDensity = fuelDensity;
				
		myTransform.gameObject.layer=drivetrain.transform.gameObject.layer;
		
		fuelTank=myTransform.gameObject.GetComponent<Rigidbody>();
		if (fuelTank == null){
			fuelTank = myTransform.gameObject.AddComponent<Rigidbody>();
		}
		
		boxCollider=myTransform.gameObject.GetComponent<BoxCollider>();
		if (boxCollider==null){
			boxCollider = myTransform.gameObject.AddComponent<BoxCollider>() as BoxCollider;
			boxCollider.size=new Vector3(0.7f,0.25f,0.6f);
		}
		
		// Set the fuel tank rigidbody parameters
		fuelTank.drag = 0f;
		fuelTank.angularDrag = 0f;
		fuelTank.useGravity = true;
		fuelTank.isKinematic = false;
		
		// Attach the fuel tank to the Car Chassis
		FixedJoint join = fuelTank.GetComponent<FixedJoint>();
		if (join == null){
			join = fuelTank.gameObject.AddComponent<FixedJoint>();
		}
		join.connectedBody = drivetrain.transform.rigidbody;
		
	}
	
	void FixedUpdate () {
		// If we don't have infinite fuel...
		if (tankCapacity>0){
			if (currentFuel>=0 && drivetrain.rpm>=20){
				currentFuel -= drivetrain.istantConsumption*Time.deltaTime*(1/(float)drivetrain.fuelTanks.Length);
				currentFuel = Mathf.Clamp(currentFuel,0,currentFuel);
			}
		}
		
		// update the weight of the fuel
		fuelTank.mass = currentFuel*fuelDensity + tankWeight;
	}
}
