using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

	public GameObject starPrefab;
	double starStartX = -2;
	double starGapX = 0.372;//1.64(current/bad) vs 1.72
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
			Vector3 scale = Vector3.one*2;
			star.transform.localScale = Vector3.zero;
			Vector3 pos = Vector3.zero;
			pos.z -= 0.1f;
			pos.x = (float)(starStartX + (totalStars++ * starGapX));
			star.transform.localPosition = pos;
			iTween.ScaleTo( star, iTween.Hash( "scale", scale, "time", 2, "easetype", iTween.EaseType.easeOutExpo,
			                                  "oncompletetarget", gameObject, "oncompleteparams", star, "oncomplete", "shrinkLastStar"));
			iTween.RotateBy( star.gameObject, iTween.Hash( "amount", Vector3.forward, "time", 4, "easetype",  iTween.EaseType.easeOutElastic));

			return true;
		} else {
			return false;
		}
	}

	void shrinkLastStar( Object param){
		GameObject star = param as GameObject;
		iTween.ScaleTo (star, iTween.Hash ("scale", Vector3.one, "time", 2, "easetype", iTween.EaseType.easeOutBounce));
	}

	public IEnumerator makeStarsDance(){
		while (true) {
			yield return new WaitForSeconds( 0.15f);
			setAllStarsAlpha(1);
			for (int i = 0; i < stars.Count; i++) {
				GameObject star = (GameObject)stars [i];
				star.GetComponent<Renderer>().material.color = new Color (1, 1, 1, 0);
				iTween.FadeTo (star, 1, 0.1f);
				yield return new WaitForSeconds( 0.1f);
			}

			for (int i = stars.Count-1; i >= 0; i--) {
				GameObject star = (GameObject)stars [i];
				star.GetComponent<Renderer>().material.color = new Color (1, 1, 1, 0);
				iTween.FadeTo (star, 1, 0.1f);
				yield return new WaitForSeconds( 0.1f);
			}

			yield return new WaitForSeconds( 0.15f);
			setAllStarsAlpha(0);
			for( int i = 0; i < 24; i++ ){
				GameObject star = (GameObject)stars [Random.Range (0,stars.Count)];
				star.GetComponent<Renderer>().material.color = Color.white;
				iTween.FadeTo (star, 0, 0.5f);
				star = (GameObject)stars [Random.Range (0,stars.Count)];
				star.GetComponent<Renderer>().material.color = Color.white;
				iTween.FadeTo (star, 0, 0.5f);
				yield return new WaitForSeconds( 0.1f);
			}
		}
	}

	void setAllStarsAlpha( float alpha ){
		for (int i = 0; i < stars.Count; i++) {
			GameObject star = (GameObject)stars [i];
			star.GetComponent<Renderer>().material.color = new Color (1, 1, 1, alpha);
		}
	}
}
