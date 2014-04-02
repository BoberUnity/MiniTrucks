//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AerodynamicResistance))]
public class AerodynamicResistanceWiki : Editor{

	URLClass urlClass;
		
	public override void OnInspectorGUI() {
		
		if (urlClass==null) urlClass=new URLClass();

		string className="Aerodynamic_Resistance"; 
	  
		if (GUILayout.Button("Open Wiki Page")) Application.OpenURL (urlClass.URL+className);
			
		EditorGUILayout.Space();
		base.OnInspectorGUI();
	}
}