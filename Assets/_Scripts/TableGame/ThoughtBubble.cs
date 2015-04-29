using UnityEngine;
using System.Collections;

public class ThoughtBubble : MonoBehaviour {
	
	public GameObject thoughtShape;

	public static int totalShapesUnlocked = 8;
	public enum ThoughtShape{
		Star, Circle, Triangle,
		Diamond, Square, HalfCircle,
		Rectangle, Oval,
		None
	};

	public IEnumerator popupShape()
	{
		yield return new WaitForSeconds(TGConsts.kThoughtShapeDelay);
		((SkeletonAnimation)thoughtShape.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-shape", false );
		yield return new WaitForSeconds( 0.01f );
		thoughtShape.GetComponent<Renderer>().enabled = true;
	}
}
