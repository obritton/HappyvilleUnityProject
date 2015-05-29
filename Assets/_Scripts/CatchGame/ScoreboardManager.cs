using UnityEngine;
using System.Collections;
using Spine;
using TMPro;

public class ScoreboardManager : MonoBehaviour {

	public SkeletonAnimation resultsAnim;

	public TextMeshPro leftScoreTMP;
	public TextMeshPro rightScoreTMP;
	public TextMeshPro totalScoreTMP;
	
	public void showResults( int leftScore, int rightScore, int totalScore)
	{
		StartCoroutine (animateResults (leftScore, rightScore, totalScore));
	}

	float countUpLength = 1.5f;
	IEnumerator animateResults(int leftScore, int rightScore, int totalScore){
		iTween.MoveTo (gameObject, iTween.Hash ("y", -175, "time", 2, "easetype", iTween.EaseType.easeOutBounce));
		yield return new WaitForSeconds (2);
		resultsAnim.state.SetAnimation (0, "Populate", false);
		resultsAnim.state.AddAnimation (0, "Idle", true, 0);

		yield return new WaitForSeconds (0.25f);
		StartCoroutine( countUpScore( leftScore, leftScoreTMP ));
		leftScoreTMP.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds (countUpLength);
		StartCoroutine( countUpScore( rightScore, rightScoreTMP ));
		rightScoreTMP.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds (countUpLength + 0.2f);
//		StartCoroutine( countUpScore( totalScore, totalScoreTMP ));
		totalScoreTMP.text = "" + totalScore;
		totalScoreTMP.GetComponent<Renderer>().enabled = true;
	}

	IEnumerator countUpScore( int targetScore, TextMeshPro scoreTMP ){
		int currentScore = 0;
		int incrementAmount = targetScore <= 10 ? 1 : targetScore / 10;
		while (currentScore < targetScore) {
			currentScore += incrementAmount;
			if( currentScore > targetScore )
				currentScore = targetScore;
			scoreTMP.text = "" + currentScore;
			yield return new WaitForSeconds(0.05f);
		}
	}
}
