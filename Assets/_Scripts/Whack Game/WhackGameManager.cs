using UnityEngine;
using System.Collections;
using Spine;

public class WhackGameManager : MonoBehaviour {

	public GameObject[] animalPrefabs;
	public GameObject balloonPrefab;
	public Transform[] leftNodes;
	public Transform[] centerNodes;
	public Transform[] rightNodes;
	ArrayList liveAnimalsIs;

	public SkeletonAnimation startBunny;
	void Start(){
		liveAnimalsIs = new ArrayList ();
		StartCoroutine (showStartBunny ());

		GameObject doors = GameObject.Find ("Doors");
		DoorManager doorManager = doors.GetComponent<DoorManager>();
		if( doorManager )
		{
			StartCoroutine(doorManager.openDoors());
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	IEnumerator delayedButtonMove( float delay )
	{
		yield return new WaitForSeconds (delay);
		startBtn.transform.Translate (10000, 0, 0);
	}

	IEnumerator showStartBunny(){
		yield return new WaitForSeconds (1);
		startBunny.state.SetAnimation (0, "Game_Start", false);
		startBunny.state.AddAnimation (0, "Idle", true, 0);
		yield return new WaitForSeconds (.05f);
		startBunny.gameObject.GetComponent<Renderer>().enabled = true;
	}

	public SkeletonAnimation startBtn;
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			GameObject touchedGO = mousePick ();
	
			if (touchedGO && touchedGO.tag == "CatchBtn") {
				TrackEntry te = startBtn.state.SetAnimation (0, "Tap", false);
				StartCoroutine (delayedButtonMove (te.animation.duration));
				te = startBunny.state.SetAnimation (0, "Start_Duck", true);
				StartCoroutine(delayedGameStart(te.animation.duration));
			}
		}
	}

	IEnumerator delayedGameStart( float delay ){
		yield return new WaitForSeconds(delay-0.05f);
		StartCoroutine (loopAnimalPopups());
		Destroy (startBunny.gameObject);
	}

	IEnumerator loopAnimalPopups(){
		while (true) {
			yield return new WaitForSeconds(1);
			StartCoroutine(popupRandomCharacter());
		}
	}
	
	IEnumerator popupRandomCharacter(){
		Transform node = null;

		do {
						switch (Random.Range (0, 3)) {
						case 0:
								{
										int randomNodeI = Random.Range (0, 3);
										node = leftNodes [randomNodeI];
								}
								break;
						case 1:
								{
										int randomNodeI = Random.Range (0, 2);
										node = centerNodes [randomNodeI];
								}
								break;
						case 2:
								{
										int randomNodeI = Random.Range (0, 3);
										node = rightNodes [randomNodeI];
								}
								break;
						}
				} while(node.childCount > 0);

		int randomAnimalI = -1;
		do {
						randomAnimalI = Random.Range (0, 6);
				} while(liveAnimalsIs.Contains(randomAnimalI));
		GameObject animalPrefab = animalPrefabs [randomAnimalI];
		GameObject animal = Instantiate (animalPrefab, node.position, Quaternion.identity) as GameObject;
		liveAnimalsIs.Add (randomAnimalI);
		animal.transform.parent = node;
		animal.transform.localPosition = Vector3.zero;

		TrackEntry te = animal.GetComponent<SkeletonAnimation> ().state.SetAnimation (0, randomAnimalI == 3 ? "PopUp" : "Popup", false);
		yield return new WaitForSeconds(te.animation.duration);
		liveAnimalsIs.Remove (randomAnimalI);
		Destroy (animal);
	}
}
