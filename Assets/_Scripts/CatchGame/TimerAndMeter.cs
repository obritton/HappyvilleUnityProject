using UnityEngine;
using System.Collections;
using TMPro;

public class TimerAndMeter : MonoBehaviour {

	public TextMeshPro TMPScore;
	public Renderer[] MeterDotsArr;

	public FrenziableGame frenziableGame;
//	public PieChartMeshController_NN pieChart;
	public PGGPieChart pieChart;

	int score = 0;
	int totalDots = 0;

	void Update(){
		if (pieChart.currentChartAmount <= 0) {
			frenziableGame.timerComplete();
		}
	}

	public void fillPieChart(){
		pieChart.currentChartAmount = 360;
	}

	public IEnumerator delayedPieChartStart( float delay )
	{
		yield return new WaitForSeconds (delay);
		pieChart.currentChartAmount = 360;
		pieChart.isActive = true;//64 bit crash was here
	}

	public void pausePieChart(){
		pieChart.isActive = false;
	}

	public void unpausePieChart(){
		pieChart.isActive = true;
	}

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
				MeterDotsArr [totalDots].GetComponent<Renderer>().enabled = true;
				totalDots++;
			}

			if (totalDots == 10) {
				frenziableGame.startFrenzy ();
			}
		}
	}

	public void zeroOutScore(){
		score = 0;
		TMPScore.SetText ("{0}", score);
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
