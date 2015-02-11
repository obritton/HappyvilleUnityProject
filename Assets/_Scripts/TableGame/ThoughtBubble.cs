using UnityEngine;
using System.Collections;

public class ThoughtBubble : MonoBehaviour {

	public GameObject thoughtShape;

	public static int totalShapesUnlocked = 8;
	public enum ThoughtShape{
		Star, Circle, Triangle,
		Diamond, Square, HalfCircle,
		Rectangle, Oval, Hexagon,
		None
	};

	public void configWithShape( ThoughtBubble.ThoughtShape thoughtShapeIn){
		string skinName = "";
		switch (thoughtShapeIn) {
		case ThoughtBubble.ThoughtShape.Circle:
			skinName = "Circle-shape";
			break;
		case ThoughtBubble.ThoughtShape.Diamond:
			skinName = "Diamond-shape";
			break;
		case ThoughtBubble.ThoughtShape.HalfCircle:
			skinName = "Half-Circle-shape";
			break;
		case ThoughtBubble.ThoughtShape.Hexagon:
			skinName = "Hexagon-shape";
			break;
		case ThoughtBubble.ThoughtShape.Oval:
			skinName = "Oval-shape";
			break;
		case ThoughtBubble.ThoughtShape.Rectangle:
			skinName = "Rectangle-shape";
			break;
		case ThoughtBubble.ThoughtShape.Square:
			skinName = "Square-shape";
			break;
		case ThoughtBubble.ThoughtShape.Star:
			skinName = "Star-shape";
			break;
		case ThoughtBubble.ThoughtShape.Triangle:
			skinName = "Triangle-shape";
			break;
		default:
			break;
		}

		SkeletonAnimation skelAnim = ((SkeletonAnimation)thoughtShape.GetComponent<SkeletonAnimation> ());
		skelAnim.skeleton.SetSkin (skinName);
	}

	public IEnumerator popupShape()
	{
		yield return new WaitForSeconds(TGConsts.kThoughtShapeDelay);
		((SkeletonAnimation)thoughtShape.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-shape", false );
		yield return new WaitForSeconds( 0.01f );
		thoughtShape.renderer.enabled = true;
	}
}
