using UnityEngine;
using System.Collections;

public class SlingshotFingerFollower : MonoBehaviour {

	public Transform bandLeftHalf;
	public Transform bandRightHalf;

	// Update is called once per frame
	//(-170, -210)
	//(1,-277)
	//(170,-210)
	float halfSlingW = 172;
	void Update () {
		if (Input.GetMouseButton (0)) {
			Vector3 touchPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if (touchPos.x < -halfSlingW + 30)
					touchPos.x = -halfSlingW + 30;
			if (touchPos.x > halfSlingW - 30)
					touchPos.x = halfSlingW - 30;
			if (touchPos.y < -370)
					touchPos.y = -370;
			if (touchPos.y > -250)
					touchPos.y = -250;

			float xWidth = touchPos.x + halfSlingW;
			float yHeight = touchPos.y + 210;

			//Left side of band lateral stretch
			Vector3 leftScale = bandLeftHalf.localScale;
			leftScale.x = xWidth;
			bandLeftHalf.localScale = leftScale;
			Vector3 leftPos = bandLeftHalf.position;
			leftPos.x = -halfSlingW + xWidth / 2 + 2;
			bandLeftHalf.position = leftPos;

			//Right side of band lateral stretch
			Vector3 rightScale = bandRightHalf.localScale;
			rightScale.x = xWidth - 2 * halfSlingW;
			bandRightHalf.localScale = rightScale;
			Vector3 rightPos = bandRightHalf.position;
			rightPos.x = halfSlingW + rightScale.x / 2 - 2;
			bandRightHalf.position = rightPos;

			//Longitudinal stretch of band
			leftScale = bandLeftHalf.localScale;
			leftScale.y = -yHeight;
			bandLeftHalf.localScale = leftScale;
			leftPos = bandLeftHalf.position;
			leftPos.y = -210 - leftScale.y / 2 + 10;
			bandLeftHalf.position = leftPos;

			rightScale = bandRightHalf.localScale;
			rightScale.y = -yHeight;
			bandRightHalf.localScale = rightScale;
			rightPos = bandRightHalf.position;
			rightPos.y = -210 - rightScale.y / 2 + 10;
				bandRightHalf.position = rightPos;
		}

		if (Input.GetMouseButtonUp (0)) {
			float time = 1;
			iTween.EaseType easetype = iTween.EaseType.easeOutElastic;
			iTween.MoveTo( bandLeftHalf.gameObject, iTween.Hash( "position", new Vector3(-81.85f, 337.157f, 0 ), "islocal", true, "time", time, "easetype", easetype ));
			iTween.MoveTo( bandRightHalf.gameObject, iTween.Hash( "position", new Vector3(86.146f, 337.157f, 0 ), "islocal", true, "time", time, "easetype", easetype ));
			iTween.ScaleTo( bandLeftHalf.gameObject, iTween.Hash( "scale", new Vector3(172, 91, 100 ), "islocal", true, "time", time, "easetype", easetype ));
			iTween.ScaleTo( bandRightHalf.gameObject, iTween.Hash( "scale", new Vector3(-172, 91, 100 ), "islocal", true, "time", time, "easetype", easetype ));
		}
	}
}
