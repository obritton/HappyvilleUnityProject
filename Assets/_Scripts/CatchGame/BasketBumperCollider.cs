using UnityEngine;
using System.Collections;

public class BasketBumperCollider : MonoBehaviour {

	void OnCollisionEnter( Collision collision ){
		SoundManager.PlaySFX("Catch BasketBounce", false, 0, 100, Random.Range(1.0f,4.0f));
	}
}
