using UnityEngine;
using Spine;
using System.Collections;

public class DoorManager : MonoBehaviour {

	public SkeletonAnimation doorAnim;

	public IEnumerator closeDoors()
	{
		yield return new WaitForSeconds (0);
		doorAnim.state.SetAnimation (0, "Close", false);
		doorAnim.GetComponent<Renderer>().enabled = true;
	}

	public IEnumerator openDoors()
	{
		SoundManager.PlaySFX("DoorOpen", false, 0);
		TrackEntry te = doorAnim.state.SetAnimation (0, "Open", false);
		yield return new WaitForSeconds (te.animation.duration);
		doorAnim.GetComponent<Renderer>().enabled = false;
	}
}
