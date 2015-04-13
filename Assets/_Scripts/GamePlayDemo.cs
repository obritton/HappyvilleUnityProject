using UnityEngine;
using Spine;
using System.Collections;

public class GamePlayDemo : MonoBehaviour {

	public GameObject[] characters;
	public GameObject[] thoughtBubbles;
	public GameObject[] toughtShapes;
	public GameObject[] platesArr;

	public GameObject[] foods;

	Vector3 foodStartPos;
	Vector3 foodStartSize;
	// Use this for initialization
	void Start () {
		foreach (GameObject character in characters) {
			character.GetComponent<Renderer>().enabled = false;
		}

		foreach (GameObject thoughtBubble in thoughtBubbles) {
			thoughtBubble.GetComponent<Renderer>().enabled = false;
		}

		foreach (GameObject toughtShape in toughtShapes) {
			toughtShape.GetComponent<Renderer>().enabled = false;
		}

		foreach (GameObject food in foods) {
			food.GetComponent<Renderer>().enabled = false;
		}

		foodStartPos = (foods [0]).transform.localPosition;
		foodStartSize = (foods [0]).transform.localScale;

		StartCoroutine(openDoors ());
	}

//	void OnGUI(){
//		if( GUI.Button( new Rect( 0, 0, Screen.width * 0.1f, Screen.height * 0.05f), "Back")){
//			MapManager.openPageIndex = 1;
//			iTween.Stop ();
//			Application.LoadLevel("MainMenuMap");
//		}
//	}

	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		DoorManager.openDoors ();

		StartCoroutine (startGame ());
	}

	bool hasStarted = false;
	bool isFoodDragging = false;
	Vector3 lastMousePos;

	void Update(){
		if (!hasStarted) {
			if (Input.GetMouseButtonUp (0)) {
				StartCoroutine (startGame ());
			}
		}else {


			if( Input.GetMouseButtonDown(0)){
				GameObject hitGO = mousePick();

				if( hitGO && hitGO.tag == "food" ){
					isFoodDragging = true;
					iTween.Stop();
					lastMousePos= Input.mousePosition;
					iTween.ScaleTo( foods[0], iTween.Hash( "time", 0.5f, "scale", 1.2f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
				}
			}

			if( Input.GetMouseButton(0)){
				if( isFoodDragging ){
					Vector3 foodPos = foods[0].transform.localPosition;
					Vector3 dragDelta = Input.mousePosition - lastMousePos;
					lastMousePos = Input.mousePosition;
					dragDelta.x *= 0.002f * 0.5f;
					dragDelta.y *= 0.003f * 0.5f;
					foodPos += dragDelta;
					foods[0].transform.localPosition = foodPos;
				}
			}

			if( Input.GetMouseButtonUp(0)){
				if( isFoodDragging ){
					isFoodDragging = false;
					((GameObject)foods[0]).GetComponent<Collider>().enabled = false;
					GameObject pickedGO = mousePick();
					((GameObject)foods[0]).GetComponent<Collider>().enabled = true;

					if( pickedGO == null ){
						snapFoodBack();
					}
					else{
						for( int i = 1; i <= 3; i++ ){
							string tag = "Plate" + i + "Btn";
							if( pickedGO.tag == tag ){
								moveFoodToPlate(i-1);
							}
						}
					}
				}
			}
		}
	}

	void snapFoodBack()
	{
		iTween.MoveTo (foods [0], iTween.Hash ("position", foodStartPos, "time", 1, "easetype", iTween.EaseType.easeOutElastic, "islocal", true));
		iTween.ScaleTo( foods[0], iTween.Hash( "time", 0.5f, "scale",  foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
	}

	void moveFoodToPlate( int plateIndex )
	{
		Vector3 pos = ((GameObject)platesArr [plateIndex]).transform.position;
		pos.z = ((GameObject)foods [0]).transform.position.z;
		pos.y -= 0.2f;
		iTween.MoveTo ((GameObject)foods [0], pos, 0.5f);
		iTween.ScaleTo( foods[0], iTween.Hash( "time", 0.5f, "scale", 0.85f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
		StartCoroutine( performResponseForPosition( plateIndex, 0.5f ));
	}

	IEnumerator performResponseForPosition( int plateIndex, float delay = 0 ){
		yield return new WaitForSeconds (delay);
		
		if (plateIndex == 2) {
			string eatName = "Eat";
			GameObject character = (GameObject)characters [2];
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, eatName, false);
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "LeaveDown", false, 0);
			GameObject food = (GameObject)foods[0];
			((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Eat-mid-food", false);

		} else {
			string wrongName = "Wrong";
			string idleName = "Idle";
			GameObject character = (GameObject)characters [plateIndex];
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, wrongName, false);
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, idleName, true, 0);
			yield return new WaitForSeconds(1);
			snapFoodBack();
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
		{
			if( hit.collider ){
				return hit.collider.gameObject;
			}
		}

		return null;
	}

	IEnumerator startGame()
	{
		hasStarted = true;
		foreach (GameObject character in characters) {
			string popupName = "Popup";
//			string idleName = "Idle";
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, popupName, false );
//			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, idleName, true, 0 );
			yield return new WaitForSeconds (0.01f);
			character.GetComponent<Renderer>().enabled = true;
			yield return new WaitForSeconds (0.2f);
		}

		yield return new WaitForSeconds (0.35f);

		for( int i = 0; i < thoughtBubbles.Length; i++ ){
			GameObject thoughtBubble = thoughtBubbles[i];
			((SkeletonAnimation)thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "PopUp-thought", false);
			((SkeletonAnimation)thoughtBubble.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "IdleOne-thought", true, 0);
			yield return new WaitForSeconds (0.01f);
			thoughtBubble.GetComponent<Renderer>().enabled = true;
			yield return new WaitForSeconds (0.15f);
			((SkeletonAnimation)toughtShapes[i].GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-shape", false);
			yield return new WaitForSeconds (0.01f);
			toughtShapes[i].GetComponent<Renderer>().enabled = true;
		}

		StartCoroutine (tapTableAll ());

		yield return new WaitForSeconds (1.8f);
		((SkeletonAnimation)foods[0].GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Enter-food", false);
		yield return new WaitForSeconds (0.01f);
		foods[0].GetComponent<Renderer>().enabled = true;
	}

	IEnumerator tapTableAll()
	{
		yield return new WaitForSeconds (1);

		foreach (GameObject character in characters) {
			string tapName = "TapTable";
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, tapName, false);
			string idleName = "Idle";
			((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, idleName, true, 0 );
		}

		foreach (GameObject plate in platesArr) {
			((SkeletonAnimation)plate.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Eat-lft", false);
		}
	}
}
