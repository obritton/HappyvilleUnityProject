using UnityEngine;
using System.Collections;

public class EyeExposer : MonoBehaviour {

	public Transform leftEye, rightEye;

	// Use this for initialization
	void Start () {
		EyeFollow.registerEyeball (leftEye);
		EyeFollow.registerEyeball (rightEye);
	}

	void OnDestroy(){
		EyeFollow.unregisterEyeBall (leftEye);
		EyeFollow.unregisterEyeBall (rightEye);
	}
}
