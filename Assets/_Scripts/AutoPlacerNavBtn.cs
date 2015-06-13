using UnityEngine;
using System.Collections;

public class AutoPlacerNavBtn : MonoBehaviour {

	public Vector2 screenCoords;

	public Transform[] analogousSigns;

	// Use this for initialization
	void Start () {

		Vector3 relativeWorldPos = Camera.main.ViewportToWorldPoint (screenCoords);
		relativeWorldPos.z = transform.position.z;
		transform.position = relativeWorldPos;

		transform.Translate (0, -1121, 0);

		foreach (Transform sign in analogousSigns) {
			sign.localPosition = transform.localPosition;
		}
	}
}
