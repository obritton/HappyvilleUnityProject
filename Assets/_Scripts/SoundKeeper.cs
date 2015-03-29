using UnityEngine;
using System.Collections;

public class SoundKeeper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
		Application.LoadLevel ("MainMenu Map");
	}
}
