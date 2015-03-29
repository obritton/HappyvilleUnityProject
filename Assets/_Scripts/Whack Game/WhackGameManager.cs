using UnityEngine;
using System.Collections;
using Spine;

public class WhackGameManager : FrenziableGame {

	public GameObject[] animalPrefabs;
	public string[] foodNamesArr;
	public GameObject foodPrefab;

	public GameObject balloonPrefab;
	public Transform[] leftNodes;
	public Transform[] centerNodes;
	public Transform[] rightNodes;
	ArrayList liveAnimalsIs;

	bool isGameStarted = false;
	public TimerAndMeter timerAndMeter;

	public SkeletonAnimation startBunny;
	void Start(){
		CatchGameManager.totalFruits = 0;
		CatchGameManager.totalCandies = 0;
		SoundManager.PlaySFX ("BariSaxMusic", true);
		liveAnimalsIs = new ArrayList ();
		StartCoroutine (showStartBunny ());
		Physics.gravity = Vector3.down * 400;

		GameObject doors = GameObject.Find ("Doors");
		DoorManager doorManager = doors.GetComponent <DoorManager>();
		if( doorManager )
		{
			StartCoroutine(doorManager.openDoors());
		}
	}

	public override void startFrenzy(){
		timerAndMeter.moveUp ();
		this.isFrenzy = true;
		StartCoroutine (stopFrenzy ());
	}

	IEnumerator stopFrenzy(){
		yield return new WaitForSeconds (12);
		this.isFrenzy = false;
		timerAndMeter.zerototalDots ();
		timerAndMeter.dropDown ();

	}

	IEnumerator loopBunnyPoint(){
		yield return new WaitForSeconds(3);
		while (!isGameStarted) {
			startBunny.state.SetAnimation(0,"Point",false);
			startBunny.state.AddAnimation(0, "Idle", true, 0 );
			yield return new WaitForSeconds(5.2f);
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
		if (startBunny != null) {
			startBunny.state.SetAnimation (0, "Game_Start", false);
			startBunny.state.AddAnimation (0, "Idle", true, 0);
			yield return new WaitForSeconds (.05f);
			startBunny.gameObject.GetComponent<Renderer> ().enabled = true;
			StartCoroutine (loopBunnyPoint ());
		}
	}

	public SkeletonAnimation startBtn;
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			GameObject touchedGO = mousePick ();
	
			if( touchedGO ){

				if ( touchedGO.tag == "CatchBtn") {
					TrackEntry te = startBtn.state.SetAnimation (0, "Tap", false);
					StartCoroutine (delayedButtonMove (te.animation.duration));
					te = startBunny.state.SetAnimation (0, "Start_Duck", true);
					StartCoroutine(delayedGameStart(te.animation.duration));
					isGameStarted = true;
				}
				else if( isGameStarted ){
					handleMashIndex(int.Parse( touchedGO.name ));
				}
			}
		}
	}

	void handleMashIndex( int mashIndex ){

		switch( mashIndex ){
		case 1:
			mashAtNode(leftNodes[0]);
			break;
		case 2:
			mashAtNode(leftNodes[1]);
			break;
		case 3:
			mashAtNode(leftNodes[2]);
			break;
		case 4:
			mashAtNode(centerNodes[1]);
			break;
		case 5:
			mashAtNode(centerNodes[0]);
			break;
		case 6:
			mashAtNode(rightNodes[0]);
			break;
		case 7:
			mashAtNode(rightNodes[1]);
			break;
		case 8:
			mashAtNode(rightNodes[2]);
			break;
		}
	}

	void mashAtNode( Transform node ){
		if (node.childCount > 0) {
			WhackAnimal animal = node.GetChild(0).GetComponent<WhackAnimal>();
			if( animal && !animal.whacked ){
				animal.whacked = true;
				SoundManager.PlaySFX ("WhackTap" + (1+Random.Range (0,2)),false,0,1,1+Random.Range(0.0f,1.0f));
				((SkeletonAnimation)node.GetChild(0).GetComponent<SkeletonAnimation>()).state.SetAnimation(0, "Tap", false);

				timerAndMeter.incrementScore( 5, isFrenzy );
			}
			else{
				SkeletonAnimation skelAnim = node.GetChild(0).GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					TrackEntry te = skelAnim.state.SetAnimation( 0, "Correct", false );
					skelAnim.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
					skelAnim.gameObject.GetComponent<Rigidbody>().useGravity = false;
					StartCoroutine(delayedDestroy(te.animation.duration, skelAnim.gameObject));

					timerAndMeter.incrementScore( 10, isFrenzy );
				}
			}
		}
	}

	IEnumerator delayedDestroy( float delay, GameObject toDieGO ){
		yield return new WaitForSeconds (delay);
		if (toDieGO != null) {
			Destroy(toDieGO);
		}
	}

	IEnumerator delayedGameStart( float delay ){
		SoundManager.PlaySFX ("SuperTransform");
		startBtn.transform.Translate (1000, 0, 0);
		yield return new WaitForSeconds(delay-0.05f);
		StartCoroutine (loopAnimalPopups());
		Destroy (startBunny.gameObject);
		timerAndMeter.dropDown ();
		StartCoroutine (timerAndMeter.delayedPieChartStart (1));
	}

	IEnumerator loopAnimalPopups(){
		while (true) {
			yield return new WaitForSeconds( isFrenzy ? 2 : 1);
			if( isFrenzy ){
				popupFrenzyFruit();
			}
			else{
				popupRandomItem();
			}
		}
	}

	void popupFrenzyFruit(){
		popupRandomItem (true);
		popupRandomItem (true);
		popupRandomItem (true);
	}

	void popupRandomItem( bool forceFruit = false ){
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

		if (Random.value <= 0.35f || forceFruit) {
			StartCoroutine(popupRandomFruitAtNode(node));
		} else {
			StartCoroutine (popupRandomAnimalAtNode (node));
		}
	}

	IEnumerator popupRandomAnimalAtNode( Transform node ){
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

	public int fruitForce = 15000;
	IEnumerator popupRandomFruitAtNode( Transform node ){

		int randomFruitIndex = Random.Range (0, foodNamesArr.Length);
		string fruitName = foodNamesArr [randomFruitIndex];
		GameObject fruit = Instantiate (foodPrefab, Vector3.down * 1000, Quaternion.identity) as GameObject;
		Vector3 localScale = fruit.transform.localScale;
		localScale *= 0.8f;
		fruit.transform.localScale = localScale;
		fruit.GetComponent<Collider> ().enabled = false;
		SkeletonAnimation skelAnim = fruit.GetComponent<SkeletonAnimation> ();
		skelAnim.skeleton.SetSkin(fruitName);
		fruit.transform.parent = node;
		fruit.transform.localPosition = Vector3.zero;
		fruit.GetComponent<Rigidbody> ().AddForce (Vector3.up * fruitForce);
		fruit.GetComponent<Rigidbody> ().AddTorque (Vector3.forward * fruitForce * Random.Range (-1, 2)*100); 

		yield return new WaitForSeconds(3);
		if (fruit != null) {
			Destroy (fruit);
		}
	}
}
