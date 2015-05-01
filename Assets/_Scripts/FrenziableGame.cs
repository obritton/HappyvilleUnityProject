using UnityEngine;
using System.Collections;

public abstract class FrenziableGame : GameManagerBase {

	public TimerAndMeter timerAndMeter;

	protected bool isFrenzy = false;
	public abstract void startFrenzy();
	public abstract void timerComplete();
	void doWin(){
		base.doWin ();
	}

	protected void startTimer(){
		timerAndMeter.dropDown ();
		StartCoroutine( timerAndMeter.delayedPieChartStart (2));
	}
}
