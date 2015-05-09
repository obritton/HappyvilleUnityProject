using UnityEngine;
using System.Collections;

public class HVButton : MonoBehaviour {

	public Texture2D normalTexture;
	public Texture2D pressedTexture;

	bool isActive = false;
	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if( mousePick() == gameObject ){
				isActive = true;
				GetComponent<Renderer>().material.SetTexture( "_MainTex", pressedTexture );
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if( isActive ){
				GetComponent<Renderer>().material.SetTexture( "_MainTex", normalTexture );
				isActive = false;
			}
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
