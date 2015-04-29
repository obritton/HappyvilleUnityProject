using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;

public class SlingshotFingerFollower : MonoBehaviour {

	public Transform bandLeftHalf;
	public Transform bandRightHalf;
	public Transform fruit;
	Vector3 fruitStartPos;
	Vector3 jumpFromPos;
	public List<GameObject> targets;

	public static bool hasGameStarted = false;
	void Start(){
		fruitStartPos = fruit.localPosition;
		jumpFromPos = fruitStartPos;
		jumpFromPos.y -= 0.65f;
		hasGameStarted = false;
		ConvFuncs.setRandomSkin(fruit.GetComponent<SkeletonAnimation>());
		StartCoroutine (loopPopupFish());
	}

	bool loopFish = false;
	public IEnumerator loopPopupFish(){
		loopFish = true;
		float waitTime = 4;
		while (loopFish) {
			yield return new WaitForSeconds(waitTime);
			waitTime -= 0.05f;
			popupOneFish();
			if( waitTime < 2 )
				popupOneFish();
		}
	}

	void popupOneFish(){
		foreach (GameObject target in targets) {
			if( isFish( target )){
				float xPos = target.transform.localPosition.x;
				if( xPos < 1.275f && xPos > 0.85f && !target.GetComponent<Renderer>().enabled ){
					if( !target.GetComponent<Renderer>().enabled ){
						TrackEntry te = target.GetComponent<SkeletonAnimation>().state.SetAnimation(0,"Popup",false);
						target.GetComponent<Renderer>().enabled = true;
						target.GetComponent<Collider>().isTrigger = false;
						StartCoroutine(deactivateFishAfterDelay(te.animation.duration, target));
						return;
					}
				}
			}
		}
	}

	IEnumerator deactivateFishAfterDelay( float delay, GameObject fish){
		yield return new WaitForSeconds (delay);
		fish.GetComponent<Renderer> ().enabled = false;
		fish.GetComponent<Collider>().isTrigger = false;
	}

	bool isFish( GameObject potentialFish){
		return ( potentialFish.name.Split (" "[0])[0] == "Fish" );
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	public void startTargetsMoving(){
		foreach (GameObject target in targets) {
			target.GetComponent<SlingTarget>().startVelocity();
		}
	}

	public void resetFruit(){
//		print ("resetFruit");
		fruit.GetComponent<Renderer> ().enabled = true;
		fruit.GetComponent<Rigidbody> ().useGravity = false;
		fruit.localPosition = Vector3.zero;
		fruit.localRotation = Quaternion.identity;
		fruit.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		fruit.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		fruit.localPosition = jumpFromPos;
		fruit.GetComponent<Collider> ().enabled = true;
		iTween.Stop ();
//		fruit.transform.localScale = Vector3.one * 0.4f;
		ConvFuncs.setRandomSkin( fruit.GetComponent<SkeletonAnimation>());
		printFruitPos ();
		iTween.MoveTo (fruit.gameObject, iTween.Hash ("position", fruitStartPos, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack, "islocal", true,
		                                              "oncomplete", "printFruitPos", "oncompletetarget", gameObject));
	}

	void printFruitPos(){
//		print ("printFruitPos: " + fruit.localPosition);
	}

	// Update is called once per frame
	//(-40, -340)
	//(-40, -250)
	float halfSlingW = 172;
	bool isDragging = false;
	void Update () {

		//MOUSE DOWN
		if (hasGameStarted && Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick ();
			if (pickedGO != null && pickedGO.tag == "food") {
				isDragging = true;
				fruit.GetComponent<Rigidbody> ().isKinematic = true;
			}
		}

		//MOUSE MOVED
		if (hasGameStarted && isDragging && Input.GetMouseButton (0)) {
			Vector3 touchPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			if (touchPos.x < -100)
					touchPos.x = -100;
			if (touchPos.x > 100)
					touchPos.x = 100;
			if (touchPos.y < -500)
					touchPos.y = -500;
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

			touchPos.z = -11.5f;
			fruit.position = touchPos;
		}

		//MOUSE UP
		if (Input.GetMouseButtonUp (0)) {
			if (hasGameStarted && isDragging ) {
				float time = 1;
				iTween.EaseType easetype = iTween.EaseType.easeOutElastic;
				iTween.MoveTo (bandLeftHalf.gameObject, iTween.Hash ("position", new Vector3 (-81.85f, 337.157f, 0), "islocal", true, "time", time, "easetype", easetype));
				iTween.MoveTo (bandRightHalf.gameObject, iTween.Hash ("position", new Vector3 (86.146f, 337.157f, 0), "islocal", true, "time", time, "easetype", easetype));
				iTween.ScaleTo (bandLeftHalf.gameObject, iTween.Hash ("scale", new Vector3 (172, 91, 100), "islocal", true, "time", time, "easetype", easetype));
				iTween.ScaleTo (bandRightHalf.gameObject, iTween.Hash ("scale", new Vector3 (-172, 91, 100), "islocal", true, "time", time, "easetype", easetype));

				launchFruit ();
			}
		}
	}

	void launchFruit(){
		fruit.GetComponent<Rigidbody> ().isKinematic = false;
		Vector3 dir = fruit.transform.localPosition*100.0f - fruitStartPos*100.0f;
		if (dir.y > -5)
			dir.y = -5;
		dir.x *= -0.5f;
		dir.y *= -1;
		dir = dir.normalized * 5000;
//		print ("  - - - dir: " + dir);
		fruit.GetComponent<Rigidbody> ().AddForce (dir);
//		fruit.GetComponent<Rigidbody> ().AddTorque (Vector3.back * 1000000);
//		iTween.ScaleTo (fruit.gameObject, iTween.Hash ("scale", Vector3.one*0.25f, "time", 3, "easetype", iTween.EaseType.linear));
	}
}
