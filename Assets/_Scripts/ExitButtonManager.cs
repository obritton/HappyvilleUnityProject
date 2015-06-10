﻿using UnityEngine;
using System.Collections;

public class ExitButtonManager : MonoBehaviour {

	public Texture2D activeTexture;
	public Texture2D touchedTexture;

	IEnumerator delayedMenuLoad()
	{
		yield return new WaitForSeconds (2.5f);
		Application.LoadLevel("MainMenu Map");
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 300))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	bool touchDownInside = false;
	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if( mousePick() == gameObject ){
				touchDownInside = true;
				changeTexture(true);
				SoundManager.PlaySFX("StartButton_Tap");
			}
		}
		
		if (Input.GetMouseButtonUp (0)) {
			if( touchDownInside ){
				iTween.Stop ();
				SoundManager.Stop();
				DoorManager.closeDoors();
				StartCoroutine(delayedMenuLoad ());
				changeTexture(false);
			}
			touchDownInside = false;
		}
	}

	void changeTexture( bool pressed ){
		Texture2D texture = pressed ? touchedTexture : activeTexture;
		GetComponent<Renderer> ().sharedMaterial.SetTexture ("_MainTex", texture);
	}
}
