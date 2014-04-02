//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class SoundController : MonoBehaviour {
	public AudioClip engineThrottle;
	public float engineThrottleVolume=0.35f;
	public float engineThrottlePitchFactor=1f;
	public AudioClip engineNoThrottle;
	public float engineNoThrottleVolume=0.35f;
	public float engineNoThrottlePitchFactor=1f;
	public AudioClip startEngine;
	public float startEngineVolume=1f;
	public float startEnginePitch=1f;
	public GameObject enginePosition;
	public AudioClip transmission;
	public float transmissionVolume=0.5f;
	public float transmissionVolumeReverse=0.5f;
	public float transmissionSourcePitch=1;
	public AudioClip brakeNoise;
	public float brakeNoiseVolume=0.2f;
	public AudioClip skid;
	public float skidVolume=1;
	public float skidPitchFactor=1;
	public AudioClip crashHiSpeed;
	public float crashHighVolume=0.75f;
	public AudioClip crashLowSpeed;
	public float crashLowVolume=0.7f;
	public AudioClip scrapeNoise;
	public float scrapeNoiseVolume=1;
	public AudioClip ABSTrigger;
	public float ABSTriggerVolume=0.2f;
	public AudioClip shiftTrigger;
	public float shiftTriggerVolume=1;
	public AudioClip wind;
	public float windVolume=1;
	public AudioClip rollingNoiseGrass;
	public AudioClip rollingNoiseSand;
	public AudioClip rollingNoiseOffroad;
		
	
	AudioSource engineThrottleSource;
	AudioSource engineNoThrottleSource;
	AudioSource startEngineSource;
	AudioSource transmissionSource;
	AudioSource brakeNoiseSource;
	AudioSource[] skidSource;
	AudioSource crashHiSpeedSource;
	AudioSource crashLowSpeedSource;
	AudioSource scrapeNoiseSource;
	AudioSource ABSTriggerSource;
	AudioSource shiftTriggerSource;
	AudioSource windSource;
	AudioSource[] rollingNoiseSource;
	[HideInInspector]
	public CarController carController;
	CarDynamics cardynamics;
	Drivetrain drivetrain;
	PhysicMaterials physicMaterials;
	Axles axles;
	bool rimScraping=false;
	
	float volumeFactor;
	int i,k;	
	
	AudioSource CreateAudioSource(AudioClip clip, bool loop, bool playImmediately, Vector3 position) {
		GameObject go = new GameObject("audio");
		go.transform.parent = transform;
		go.transform.localPosition = position;
		go.transform.localRotation = Quaternion.identity;
		go.AddComponent(typeof(AudioSource));
		go.audio.clip = clip;
		go.audio.playOnAwake = false;
		if (loop==true){
			go.audio.volume = 0;
			go.audio.loop = true;
		}
		else 
			go.audio.loop = false;
		
		if (playImmediately) go.audio.Play();
		
		return go.audio;
	}
	
	void Start () {
		carController = GetComponent<CarController>();
		cardynamics = GetComponent<CarDynamics>();
		drivetrain= GetComponent<Drivetrain>();
		physicMaterials =GetComponent<PhysicMaterials>();
		axles=GetComponent<Axles>();
		
		Vector3 enginePositionV=Vector3.zero;
		if (enginePosition!=null) enginePositionV=enginePosition.transform.position;
		engineThrottleSource = CreateAudioSource(engineThrottle, true, true,enginePositionV);
		engineNoThrottleSource = CreateAudioSource(engineNoThrottle, true, true,enginePositionV);
		transmissionSource = CreateAudioSource(transmission, true, true,Vector3.zero);
		brakeNoiseSource = CreateAudioSource(brakeNoise, true, true,Vector3.zero);
		startEngineSource= CreateAudioSource(startEngine, true, false,enginePositionV);
		startEngineSource.volume=startEngineVolume;
		startEngineSource.pitch=startEnginePitch;
		
		System.Array.Resize(ref skidSource,axles.allWheels.Length);
		i=0;
		foreach(Wheel w in axles.allWheels){
			skidSource[i] = CreateAudioSource(skid, true, true,w.transform.localPosition);
			i++;
		}	

		crashHiSpeedSource = CreateAudioSource(crashHiSpeed, false, false,Vector3.zero);
		crashLowSpeedSource = CreateAudioSource(crashLowSpeed, false, false,Vector3.zero);
		scrapeNoiseSource = CreateAudioSource(scrapeNoise, false, false,Vector3.zero);
		ABSTriggerSource= CreateAudioSource(ABSTrigger, false, false,Vector3.zero);
		ABSTriggerSource.volume=ABSTriggerVolume;
		shiftTriggerSource= CreateAudioSource(shiftTrigger, false, false,Vector3.zero);
		shiftTriggerSource.volume=shiftTriggerVolume;
		windSource = CreateAudioSource(wind, true, true,Vector3.zero);
		
		if (physicMaterials){
			System.Array.Resize(ref rollingNoiseSource,axles.allWheels.Length);
			i=0;
			foreach(Wheel w in axles.allWheels){
				rollingNoiseSource[i]=CreateAudioSource(rollingNoiseGrass, true, false,w.transform.localPosition);
				i++;
			}	
		}
	}
	
	void Update(){
		if (carController!=null) {
			if (carController.ABSTriggered) ABSTriggerSource.PlayOneShot(ABSTrigger);			
			brakeNoiseSource.volume=Mathf.Clamp01(carController.brake*Mathf.Abs(AverageWheelVelo())*0.1f)*brakeNoiseVolume;
		}	
		if (drivetrain!=null) {		
			if (drivetrain.startEngine && drivetrain.rpm<drivetrain.minRPM) {if (!startEngineSource.isPlaying) startEngineSource.Play();}
			else startEngineSource.Stop();
		}
	}
	
	float AverageWheelVelo() {
		float val = 0f;
		foreach(Wheel w in axles.allWheels){
			val += w.wheelTireVelo;
		}
		return val/axles.allWheels.Length;
	}
	
	void FixedUpdate(){
		if (drivetrain!=null){
			if (drivetrain.shiftTriggered) {shiftTriggerSource.PlayOneShot(shiftTrigger);drivetrain.shiftTriggered=false;drivetrain.soundPlayed=true;}
		}

		if (Application.isEditor){
			if (shiftTriggerSource!=null) shiftTriggerSource.volume=shiftTriggerVolume;
			if (ABSTriggerSource!=null) ABSTriggerSource.volume=ABSTriggerVolume;
		}
		if (drivetrain){
			float factor=drivetrain.rpm/drivetrain.maxRPM; 
			engineNoThrottleSource.volume = Mathf.Clamp01((1 - drivetrain.throttle)*engineNoThrottleVolume*factor);
			engineNoThrottleSource.pitch = 0.5f + engineNoThrottlePitchFactor*factor;
			
			engineThrottleSource.volume = Mathf.Clamp01(drivetrain.throttle*engineThrottleVolume*factor + engineThrottleVolume*0.2f*factor);
			engineThrottleSource.pitch = 0.5f + engineThrottlePitchFactor*factor;
			
			if (drivetrain.clutch!=null){
				if (drivetrain.clutch.GetClutchPosition()!=0 && drivetrain.ratio != 0){
					float differentialSpeed = Mathf.Abs(drivetrain.differentialSpeed*0.01f);
					float factor1=Mathf.Abs(drivetrain.ratio/drivetrain.lastGearRatio);
					float mtransmissionVolume=transmissionVolume;
					if (drivetrain.ratio<0) {mtransmissionVolume=transmissionVolumeReverse;}
					transmissionSource.volume = Mathf.Clamp01((mtransmissionVolume - (1-drivetrain.throttle)*mtransmissionVolume*0.25f)/factor1);
					transmissionSource.pitch = differentialSpeed*transmissionSourcePitch*factor1;
							
					//transmissionSource.volume= mtransmissionVolume+ factor*drivetrain.throttle;
					//transmissionSource.pitch = 0.5f + transmissionPitchFactor*factor;
				}
				else{
					transmissionSource.volume=0;
				}
			}	
			if (drivetrain.shiftTriggered) {shiftTriggerSource.PlayOneShot(shiftTrigger);drivetrain.shiftTriggered=false;}
		}
		
		if (windSource!=null) windSource.volume=Mathf.Clamp01(Mathf.Abs(cardynamics.velo)*windVolume*0.006f);
		
		k=0;
		rimScraping=false;
		foreach(Wheel w in axles.allWheels){
			if (skidSource[k]!=null){
				skidSource[k].pitch=skidPitchFactor;
				skidSource[k].volume = Mathf.Clamp(Mathf.Abs(w.slipVelo)*0.00875f, 0f, skidVolume);
				if (skidSource[k].volume <=0.01f) skidSource[k].volume=0;
				
				if (w.rimScraping==true){
					rimScraping=true;
					scrapeNoiseSource.volume=Mathf.Clamp01(Mathf.Abs(w.angularVelocity)*0.01f + Mathf.Abs(w.slipVelo)*0.035f)*scrapeNoiseVolume;
					scrapeNoiseSource.loop=true;
					if (!scrapeNoiseSource.isPlaying) scrapeNoiseSource.Play();
				}
				else scrapeNoiseSource.loop=false;
				
			
				if (physicMaterials){
					if (w.physicMaterial==physicMaterials.track || w.physicMaterial==null) {
						rollingNoiseSource[k].volume=0;
					}
					else if (w.physicMaterial==physicMaterials.grass) {
						rollingNoiseSource[k].clip=rollingNoiseGrass;
						rollingNoiseSource[k].volume=Mathf.Clamp01(Mathf.Abs(w.angularVelocity)*0.01f + Mathf.Abs(w.slipVelo)*0.035f);
						if (!rollingNoiseSource[k].isPlaying) rollingNoiseSource[k].Play();
						if (rollingNoiseSource[k].volume <=0.01f || !w.onGroundDown) rollingNoiseSource[k].volume =0;
						skidSource[k].volume=0;
					}
					else if (w.physicMaterial ==physicMaterials.sand){
						rollingNoiseSource[k].clip=rollingNoiseSand;
						rollingNoiseSource[k].volume=Mathf.Clamp01(Mathf.Abs(w.angularVelocity)*0.01f + Mathf.Abs(w.slipVelo)*0.035f);
						if (!rollingNoiseSource[k].isPlaying) rollingNoiseSource[k].Play();
						if (rollingNoiseSource[k].volume <=0.01f || !w.onGroundDown) rollingNoiseSource[k].volume =0;
						skidSource[k].volume=0;
					}
					else if (w.physicMaterial ==physicMaterials.offRoad){
						rollingNoiseSource[k].clip=rollingNoiseOffroad;
						rollingNoiseSource[k].volume=Mathf.Clamp01(Mathf.Abs(w.angularVelocity)*0.01f + Mathf.Abs(w.slipVelo)*0.035f);
						if (!rollingNoiseSource[k].isPlaying) rollingNoiseSource[k].Play();
						if (rollingNoiseSource[k].volume <=0.01f || !w.onGroundDown) rollingNoiseSource[k].volume =0;
						skidSource[k].volume=0;
					} 
				}
			}
			k++;
		}
	}

	void OnCollisionEnter(Collision collInfo)
	{
		if (collInfo.contacts.Length > 0){
			if (collInfo.contacts[0].thisCollider.gameObject.layer != LayerMask.NameToLayer("Wheel") )
			{	
				volumeFactor = Mathf.Clamp01(collInfo.relativeVelocity.magnitude*0.1f);
				volumeFactor *= Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(collInfo.relativeVelocity.normalized, collInfo.contacts[0].normal)));
				//volumeFactor = volumeFactor*0.5f + 0.5f;
				if(volumeFactor > 0.9f && !crashHiSpeedSource.isPlaying){
					crashHiSpeedSource.volume=Mathf.Clamp01(volumeFactor*crashHighVolume);
					crashHiSpeedSource.Play();
				}
				if (!crashLowSpeedSource.isPlaying){
					crashLowSpeedSource.volume=Mathf.Clamp01(volumeFactor*crashLowVolume);
					crashLowSpeedSource.Play();
				}
				if (!scrapeNoiseSource.isPlaying){
					scrapeNoiseSource.volume=SetScrapeNoiseVolume(collInfo,1);
					scrapeNoiseSource.loop=false;
					scrapeNoiseSource.Play();
				}
			}	
		}
	}
	
	void OnCollisionExit(){
		if (rimScraping==false){	
			scrapeNoiseSource.volume=0;
			scrapeNoiseSource.loop=false;
		}
	}
	
		
	void OnCollisionStay(Collision collInfo){
		scrapeNoiseSource.volume=SetScrapeNoiseVolume(collInfo, 10);
		scrapeNoiseSource.loop=true;
		if (!scrapeNoiseSource.isPlaying) scrapeNoiseSource.Play();
	}
	
	float SetScrapeNoiseVolume(Collision collInfo, float factor=1){
		float magnitude=collInfo.relativeVelocity.magnitude/factor;
		//float angle=Vector3.Angle(collInfo.contacts[0].normal,collInfo.relativeVelocity);
		//float sin=Mathf.Abs(Mathf.Sin(angle*Mathf.Deg2Rad));
		float sin=0;
		if (collInfo.contacts.Length>0) sin=1 - Mathf.Abs(Vector3.Dot(collInfo.contacts[0].normal,collInfo.relativeVelocity.normalized));
		return Mathf.Clamp01(magnitude*sin*scrapeNoiseVolume);
	}
}
