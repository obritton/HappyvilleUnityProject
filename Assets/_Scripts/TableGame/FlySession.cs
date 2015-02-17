using UnityEngine;
using System.Collections;

public class FlySession : MonoBehaviour {

	public Fly fly;

	public Transform getFly(){
		return fly.transform;
	}

	static bool isFlyInSession = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
