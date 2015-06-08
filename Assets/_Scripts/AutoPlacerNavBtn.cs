using UnityEngine;
using System.Collections;

public class AutoPlacerNavBtn : MonoBehaviour {

	public Vector2 screenCoords;
	public bool relativeToParent = false;
	public bool doesPrint = false;

	// Use this for initialization
	void Start () {
		if (doesPrint) {
			if (relativeToParent)
				print ("" + gameObject.name + " WorldToScreenPoint: " + Camera.main.WorldToViewportPoint (transform.localPosition));
			else
				print ("" + gameObject.name + " WorldToScreenPoint: " + Camera.main.WorldToViewportPoint (transform.position));
		}

		Vector3 relativeWorldPos = Camera.main.ViewportToWorldPoint (screenCoords);
		relativeWorldPos.z = transform.position.z;

//		if(relativeToParent)
//			transform.localPosition = relativeWorldPos;
//		else
			transform.position = relativeWorldPos;
	}
}
