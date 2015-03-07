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

	void Start(){
		liveAnimalsIs = new ArrayList ();
		StartCoroutine(loopAnimalPopups());

		GameObject doors = GameObject.Find ("Doors");
		DoorManager doorManager = doors.GetComponent<DoorManager>();
		if( doorManager )
		{
			StartCoroutine(doorManager.openDoors());
		}
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
										int randomNodeI = Random.Range (0, 2);
										node = leftNodes [randomNodeI];
								}
								break;
						case 1:
								{
										int randomNodeI = Random.Range (0, 1);
										node = centerNodes [randomNodeI];
								}
								break;
						case 2:
								{
										int randomNodeI = Random.Range (0, 2);
										node = centerNodes [randomNodeI];
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
