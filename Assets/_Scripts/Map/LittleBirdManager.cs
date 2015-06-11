using UnityEngine;
using System.Collections;
using Spine;

public class LittleBirdManager : MonoBehaviour {

	public void startBirdAnim(){
		StartCoroutine (animateBirdToSign ());
	}

	public GameObject[] birdEnterPosArr;
	public SkeletonAnimation birdAnim;

	IEnumerator animateBirdToSign(){
//		TrackEntry te = birdAnim.state.SetAnimation (0, "Fly", true);
//		iTween.MoveTo (birdAnim.gameObject, iTween.Hash ("position", birdEnterPosArr [1].transform.position, "time", te.animation.duration, "easetype", iTween.EaseType.easeOutQuad));
//		yield return new WaitForSeconds(te.animation.duration);

		TrackEntry te = birdAnim.state.SetAnimation (0, "Idle_Sign", true);
		yield return new WaitForSeconds(te.animation.duration/2);

		te = birdAnim.state.SetAnimation (0, "Fly_Away", false);
//		iTween.MoveTo (birdAnim.gameObject, iTween.Hash ("position", birdEnterPosArr [2].transform.position, "time", te.animation.duration, "easetype", iTween.EaseType.easeInQuad));
	}
}
