using UnityEngine;
using System.Collections;

public class GestureInputResponder : MonoBehaviour {

	bool isDragging = false;
	bool isPinching = false;
	bool isTwisting = false;

	bool isGestureSelecting( ContinuousGesture gesture ){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (gesture.Position);
		if (Physics.Raycast (ray, out hit, 2000)) {
			return hit.collider.gameObject == gameObject;
		}

		return false;
	}

	void OnDrag( DragGesture gesture ) 
	{
		switch (gesture.Phase) {
		case ContinuousGesturePhase.Started:
			isDragging = isGestureSelecting(gesture);
			break;
		case ContinuousGesturePhase.Updated:
			if( !isDragging )
				return;
			Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
			pos.x += gesture.DeltaMove.x;
			pos.y += gesture.DeltaMove.y;
			transform.position = Camera.main.ScreenToWorldPoint(pos);
			break;
		case ContinuousGesturePhase.Ended:
			isDragging = false;
			break;
		default:
		case ContinuousGesturePhase.None:
			break;
		}
	}

	void OnPinch( PinchGesture gesture ) 
	{
		switch (gesture.Phase) {
		case ContinuousGesturePhase.Started:
			isPinching = isGestureSelecting(gesture);
			break;
		case ContinuousGesturePhase.Updated:
			if( !isPinching )
				return;
			ContinuousGesturePhase phase = gesture.Phase;
			Vector3 scale = Camera.main.WorldToScreenPoint(transform.localScale);
			scale.x += gesture.Delta;
			scale.y += gesture.Delta;
			transform.localScale = Camera.main.ScreenToWorldPoint(scale);
			break;
		case ContinuousGesturePhase.Ended:
			isPinching = false;
			break;
		default:
		case ContinuousGesturePhase.None:
			break;
		}
	}

	void OnTwist( TwistGesture gesture ) 
	{
		switch (gesture.Phase) {
		case ContinuousGesturePhase.Started:
			isTwisting = isGestureSelecting(gesture);
			break;
		case ContinuousGesturePhase.Updated:
			if( !isTwisting )
				return;
			ContinuousGesturePhase phase = gesture.Phase;
			float delta = gesture.DeltaRotation;
			float total = gesture.TotalRotation;
			transform.Rotate (0, 0, delta);
			break;
		case ContinuousGesturePhase.Ended:
			isTwisting = false;
			break;
		default:
		case ContinuousGesturePhase.None:
			break;
		}
	}
}
