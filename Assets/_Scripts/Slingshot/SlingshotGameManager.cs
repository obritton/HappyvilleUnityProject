using UnityEngine;
using System.Collections;

public class SlingshotGameManager : MonoBehaviour {

	void Start () {
		StartCoroutine (openDoors ());
	}

	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		DoorManager.openDoors ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
