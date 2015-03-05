using UnityEngine;
using System.Collections;

public class GroundAndBasketCollider : MonoBehaviour {

	public FruitKiller fruitKiller;

	void OnCollisionEnter( Collision collision ){
		fruitKiller.killFruitFrom ( collision.gameObject, gameObject.tag);
	}
}
