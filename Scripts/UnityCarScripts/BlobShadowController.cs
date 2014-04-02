//========================================================================================================================
// UnityCar 2.1 Pro Vehicle Physics - (c) Michele Di Lena
// http://www.unitypackages.net/unitycar
//
// Any product developed using this version of UnityCar requires clearly readable UnityCar logo on splash screen or credits screen.
// See README.txt for more info.
//========================================================================================================================

using UnityEngine;

public class BlobShadowController : MonoBehaviour
{
	Transform mTranform;
	
	void Start(){
		mTranform=transform;
	}	
	
	void Update()
	{
		mTranform.position = mTranform.parent.position + Vector3.up * 8.246965f;
		mTranform.rotation = Quaternion.LookRotation(-Vector3.up, mTranform.parent.forward);
	}
}
