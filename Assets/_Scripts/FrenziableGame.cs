using UnityEngine;
using System.Collections;

public abstract class FrenziableGame : GameManagerBase {

	protected bool isFrenzy = false;
	public abstract void startFrenzy();
	public abstract void timerComplete();
	void doWin(){
		base.doWin ();
	}
}
