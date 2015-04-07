using UnityEngine;
using System.Collections;

public abstract class FrenziableGame : MonoBehaviour {

	protected bool isFrenzy = false;
	public abstract void startFrenzy();
	public abstract void timerComplete();
}
