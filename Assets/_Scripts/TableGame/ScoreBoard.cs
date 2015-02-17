using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

	public GameObject starPrefab;
	double starStartX = -2;
	double starGapX = 0.364;
	int totalStars = 0;
	ArrayList stars;

	public int getTotalStars(){
		return totalStars;
	}

	void Start(){
		stars = new ArrayList ();
	}

	public bool addStar(){
		if (totalStars < 12) {
			GameObject star = Instantiate( starPrefab, Vector3.left*10, Quaternion.identity) as GameObject;
			stars.Add (star);
			star.transform.parent = transform;
//			Vector3 scale = star.transform.localScale;
			Vector3 scale = Vector3.one;
			star.transform.localScale = Vector3.zero;
			Vector3 pos = Vector3.zero;
			pos.x = (float)(starStartX + (totalStars++ * starGapX));
			star.transform.localPosition = pos;
			iTween.ScaleTo( star, iTween.Hash( "scale", scale, "time", 0.5f, "easetype", iTween.EaseType.easeOutElastic));

			return true;
		} else {
			return false;
		}
	}

	public IEnumerator makeStarsDance(){
		while (true) {
			yield return new WaitForSeconds( 0.15f);
			setAllStarsAlpha(1);
			for (int i = 0; i < 12; i++) {
				GameObject star = (GameObject)stars [i];
				star.renderer.material.color = new Color (1, 1, 1, 0);
				iTween.FadeTo (star, 1, 0.1f);
				yield return new WaitForSeconds( 0.1f);
			}

			for (int i = 11; i >= 0; i--) {
				GameObject star = (GameObject)stars [i];
				star.renderer.material.color = new Color (1, 1, 1, 0);
				iTween.FadeTo (star, 1, 0.1f);
				yield return new WaitForSeconds( 0.1f);
			}

			yield return new WaitForSeconds( 0.15f);
			setAllStarsAlpha(0);
			for( int i = 0; i < 24; i++ ){
				GameObject star = (GameObject)stars [Random.Range (0,12)];
				star.renderer.material.color = Color.white;
				iTween.FadeTo (star, 0, 0.5f);
				star = (GameObject)stars [Random.Range (0,12)];
				star.renderer.material.color = Color.white;
				iTween.FadeTo (star, 0, 0.5f);
				yield return new WaitForSeconds( 0.1f);
			}
		}
	}

	void setAllStarsAlpha( float alpha ){
		for (int i = 0; i < 12; i++) {
			GameObject star = (GameObject)stars [i];
			star.renderer.material.color = new Color (1, 1, 1, alpha);
		}
	}
}
