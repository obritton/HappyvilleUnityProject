using UnityEngine;
using System.Collections;

public class FrenzySquareFollower : MonoBehaviour {

	public GameObject[] floatingCharacters;

	Vector3[] locations;

	void Start()
	{
		locations = new Vector3[4];
		for( int i = 0; i < 4; i++ ){
			locations[i] = floatingCharacters[i].transform.localPosition;
		}

		StartCoroutine(moveCharactersAroundSquare ());
	}

	int indexOffset = 0;
	public bool isActive = true;
	IEnumerator moveCharactersAroundSquare(){
		while (isActive) {
			indexOffset = (indexOffset + 1) % 4;
			for (int i = 0; i < 4; i++) {
				iTween.MoveTo (floatingCharacters [i], iTween.Hash ("position", locations [(i + indexOffset) % 4], "time", 4, "easetype", iTween.EaseType.easeInOutQuad, "islocal", true));
			}
			yield return new WaitForSeconds(4);
		}
	}
}
