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
				playSound();
				StartCoroutine(doDeform());
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if( isActive ){
				GetComponent<Renderer>().material.SetTexture( "_MainTex", normalTexture );
				isActive = false;
			}
		}
	}

	public bool isThumb = false;
	void playSound(){

		if( isThumb )
			SoundManager.PlaySFX ("Thumb_Tap");
		else
			SoundManager.PlaySFX ("UI_Button_Tap");
	}

	IEnumerator doDeform(){
		iTween.PunchScale( gameObject, iTween.Hash( "time", 1, "amount", new Vector3( 30, 0, 0)));
		yield return new WaitForSeconds (0.15f);
		iTween.PunchScale( gameObject, iTween.Hash( "time", 1, "amount", new Vector3( 0, 10, 0)));
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
