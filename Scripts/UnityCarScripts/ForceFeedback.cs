//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// based on the work of Cameron Bonde ('Vectrex' on the forums)
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using System.Runtime.InteropServices;
using UnityEngine;
public class ForceFeedback : MonoBehaviour
{
	[HideInInspector]
	public int force;
	float forceFeedback;
	private bool forceFeedbackEnabled=false;
	CarDynamics cardynamics;
	public int factor=1000;
	public float multiplier=0.5f;
	public float smoothingFactor=0.5f;
	public int clampValue=20;
	public bool invertForceFeedback=false;
	private int sign=1;
	float m_force;

	[DllImport("user32")]
	private static extern int GetForegroundWindow();

	// Import functions from DirectInput c++ wrapper dll
	[DllImport("UnityForceFeedback")]
	private static extern int InitDirectInput(int HWND);

	[DllImport("UnityForceFeedback")]
	private static extern void Aquire();

	[DllImport("UnityForceFeedback")]
	private static extern int SetDeviceForcesXY(int x, int y);

	[DllImport("UnityForceFeedback")]
	private static extern bool StartEffect();

	[DllImport("UnityForceFeedback")]
	private static extern bool StopEffect();

	[DllImport("UnityForceFeedback")]
	private static extern bool SetAutoCenter(bool autoCentre);

	[DllImport("UnityForceFeedback")]
	private static extern void FreeDirectInput();


	public void Start()
	{
		cardynamics = GetComponent<CarDynamics>();
		InitialiseForceFeedback();
		SetAutoCenter(false);
	}

	public void Update()
	{
		//veloFactor=(int)(thresholdVelo/3.6f + 1 - cardynamics.velo);
		//if (veloFactor<1) veloFactor=1;
		sign=1;
		if (invertForceFeedback==true) sign=-1;
		forceFeedback = cardynamics.forceFeedback;//*smoothingFactor + forceFeedback*(1.0f - smoothingFactor);
		if (Mathf.Abs(forceFeedback)>clampValue) forceFeedback=clampValue*Mathf.Sign(forceFeedback);
		//force=(int)(forceFeedback*multiplier)*factor*sign;
		force=(int)(forceFeedback*multiplier)*factor*sign;
		SetDeviceForcesXY(force, 0); // You might need to set force on the Y depending on your device. Also, be carefull with the update rate of new force data.
	}

	public void OnApplicationQuit()
	{
		ShutDownForceFeedback();
	}

	private void InitialiseForceFeedback()
	{
		if(forceFeedbackEnabled){
			Debug.LogWarning("UnityCar: Force feedback attempted to initialise but was aleady running!");
			return;
		}
		int hwnd = GetForegroundWindow();
		InitDirectInput(hwnd);
		Aquire();
		StartEffect();
		forceFeedbackEnabled = true;
	}

	private void ShutDownForceFeedback()
	{
		StopEffect();
		if(forceFeedbackEnabled)
			FreeDirectInput();
		else
			Debug.LogWarning("UnityCar:  Force feedback attempted to shutdown but wasn't running!");
	}
	
/* 	public void OnGUI()
	{
		float ffb=cardynamics.forceFeedback*multiplier;
		if (Mathf.Abs(m_force)<Mathf.Abs(ffb)) m_force=ffb;
		
		GUILayout.Space(50);
		GUILayout.Label("ffb = " + (int)ffb);
		GUILayout.Label("Max force = " + (int)m_force);
	}	 */
}
