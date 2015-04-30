using UnityEngine;
using System.Collections;

public class FrenzyCircleTurner : MonoBehaviour {

	public Transform[] characters;
	// Update is called once per frame
	void Update () {
		transform.Rotate (0, 0, Time.deltaTime * 50);

		foreach (Transform characterTrans in characters) {
			characterTrans.rotation = Quaternion.identity;
		}
	}
}
