using UnityEngine;
using System.Collections;

public class BasketBumperCollider : MonoBehaviour {

	void playBounce(){
		string sfxName = "Food_Bounce_0" + (1 + Random.Range (0,3));
		SoundManager.PlaySFX(sfxName);
	}

	void OnCollisionEnter( Collision collision ){
		playBounce ();
	}
}
