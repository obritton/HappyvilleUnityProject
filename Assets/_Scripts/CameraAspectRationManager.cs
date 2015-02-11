using UnityEngine;
using System.Collections;

public class CameraAspectRationManager : MonoBehaviour {

	int iPadOrthoSize = 512;
	int iPhoneOrthoSize = 568;

	// Use this for initialization
	void Start () {
		float aspectRatio = (float)Screen.width / (float)Screen.height;
		double proximityToiPadAspectRatio = Mathf.Abs (aspectRatio - 768.0f / 1024.0f);

		int orthoSize = (proximityToiPadAspectRatio < 0.1 ? iPadOrthoSize : iPhoneOrthoSize);
		Camera.main.orthographicSize = orthoSize;
//		print ("aspectRatio: " + aspectRatio);
//		print ("proximityToiPadAspectRatio: " + proximityToiPadAspectRatio);
//		print ("orthoSize: " + orthoSize);
	}
}
