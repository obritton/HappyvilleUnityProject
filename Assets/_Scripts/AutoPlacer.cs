using UnityEngine;
using System.Collections;

public class AutoPlacer : MonoBehaviour {

	public Vector2 screenCoords;
	public bool relativeToParent = false;
	
	// Use this for initialization
	void Start () {
		if( relativeToParent )
			print ( "" + gameObject.name + " WorldToScreenPoint: " + Camera.main.WorldToScreenPoint( transform.localPosition ));
		else
			print ( "" + gameObject.name + " WorldToScreenPoint: " + Camera.main.WorldToScreenPoint( transform.position ));

		Vector3 relativeWorldPos = Camera.main.ScreenToWorldPoint (screenCoords);
		relativeWorldPos.z = transform.position.z;

		if(relativeToParent)
			transform.localPosition = relativeWorldPos;
		else
			transform.position = relativeWorldPos;
	}
}
