using UnityEngine;
using System.Collections;
using Spine;
using TMPro;

public class CatchResults : MonoBehaviour {

	public SkeletonAnimation resultsAnim;

	public TextMeshPro fruitsTMP;
	public TextMeshPro candiesTMP;
	public TextMeshPro scoreTMP;

	public void showResults( int totalFruits, int totalCandies, int totalScore)
	{
		StartCoroutine (animateResults (totalFruits, totalCandies, totalScore));
	}

	IEnumerator animateResults(int totalFruits, int totalCandies, int totalScore){
		iTween.MoveTo (gameObject, iTween.Hash ("y", -175, "time", 2, "easetype", iTween.EaseType.easeOutBounce));
		yield return new WaitForSeconds (2);
		resultsAnim.state.SetAnimation (0, "Populate", false);
		resultsAnim.state.AddAnimation (0, "Idle", true, 0);

		yield return new WaitForSeconds (0.25f);
		fruitsTMP.text = ""+totalFruits;
		fruitsTMP.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds (0.25f);
		candiesTMP.text = ""+totalCandies;
		candiesTMP.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds (0.25f);
		scoreTMP.text = "" + totalScore;
		scoreTMP.GetComponent<Renderer>().enabled = true;
	}
}
