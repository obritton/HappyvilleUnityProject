using UnityEngine;
using System.Collections;

public class SoundKeeper : MonoBehaviour {

	static bool loadedOnce = false;
	// Use this for initialization
	void Start () {
		if (!loadedOnce) {
			loadedOnce = true;
			DontDestroyOnLoad (this);
			Application.LoadLevel ("MainMenu Map");
		}
	}
}
