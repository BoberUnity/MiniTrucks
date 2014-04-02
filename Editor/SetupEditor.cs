//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Setup))]
public class SetupEditor : Editor {
	
	Setup setupTarget;
	URLClass urlClass;
	
	void OnEnable(){
		setupTarget = (Setup)target;
	}	
		
	public override void OnInspectorGUI(){
		if (urlClass==null) urlClass=new URLClass();

		string className="Setup"; 
	  
		if (GUILayout.Button("Open Wiki Page")) Application.OpenURL (urlClass.URL+className);
			
		EditorGUILayout.Space();
		base.OnInspectorGUI();
		
		if(GUILayout.Button("Load Setup")){
			setupTarget.LoadSetup();
		}

		if(GUILayout.Button("Save Setup")){
			setupTarget.SaveSetup();
		}
	}
}