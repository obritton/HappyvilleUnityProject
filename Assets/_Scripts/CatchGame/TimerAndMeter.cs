using UnityEngine;
using System.Collections;
using TMPro;

public class TimerAndMeter : MonoBehaviour {

	public TextMeshPro TMPScore;
	public Renderer[] MeterDotsArr;

	public CatchGameManager catchGame;

	int score = 0;
	int totalDots = 0;

	public void zerototalDots(){
		totalDots = 0;
		foreach (Renderer renderer in MeterDotsArr)
			renderer.enabled = false;
	}

	public void incrementScore( int amount, bool isFrenzyMode ){
		score += amount;
		TMPScore.SetText ("{0}", score);

		if (!isFrenzyMode) {
			if (totalDots < MeterDotsArr.Length) {
				MeterDotsArr [totalDots].renderer.enabled = true;
				totalDots++;
			}

			if (totalDots == 10) {
				catchGame.startFrenzy ();
			}
		}
	}

	public int getScore(){
		return score;
	}

	public void dropDown(){
		iTween.MoveTo (gameObject, iTween.Hash ("y", 484, "time", 1, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void moveUp(){
		iTween.MoveTo (gameObject, iTween.Hash ("y", 686, "time", 0.25, "easetype", iTween.EaseType.linear));
	}
}
