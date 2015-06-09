using UnityEngine;
using System.Collections;

public class AutoPlacerViewPort : MonoBehaviour {

	public Vector2 screenCoords;
	public bool relativeToParent = false;
	
	// Use this for initialization
	void Start () {
		if( relativeToParent )
			print ( "" + gameObject.name + " WorldToScreenPoint: " + Camera.main.WorldToViewportPoint( transform.localPosition ));
		else
			print ( "" + gameObject.name + " WorldToScreenPoint: " + Camera.main.WorldToViewportPoint( transform.position ));

		Vector3 relativeWorldPos = Camera.main.ViewportToWorldPoint (screenCoords);
		relativeWorldPos.z = transform.position.z;

		if(relativeToParent)
			transform.localPosition = relativeWorldPos;
		else
			transform.position = relativeWorldPos;
	}
}
