using UnityEngine;
using System.Collections;

public class LODcam : MonoBehaviour {
	public float[] lodDistances = new float[32];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Camera.main.layerCullDistances = lodDistances;
	}
}
