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
	public GameObject crumbsPrefab;

	bool isGameStarted = false;
	public TimerAndMeter timerAndMeter;

	public ScoreboardManager sbManager;

	public SkeletonAnimation startBunny;
	void Start(){
		configNewGame ();
		StartCoroutine (showStartBunny ());
		Physics.gravity = Vector3.down * 400;

		DoorManager.openDoors ();
	}

	void configNewGame(){
		CatchGameManager.totalFruits = 0;
		CatchGameManager.totalCandies = 0;
		totalAnimals = 0;
		totalFruits = 0;
		timerAndMeter.pausePieChart ();
		timerAndMeter.fillPieChart ();
		timerAndMeter.zerototalDots ();
		timerAndMeter.zeroOutScore ();
		liveAnimalsIs = new ArrayList ();
	}

	public override void timerComplete ()
	{
		print ("timerComplete");
		if (!gameEnded)
			endGame ();
	}

	bool gameEnded = false;
	void endGame()
	{
		gameEnded = true;
		timerAndMeter.moveUp ();
		sbManager.showResults ( totalAnimals, totalFruits, timerAndMeter.getScore());
		configNewGame ();
	}

	public override void startFrenzy(){
		timerAndMeter.moveUp ();
		this.isFrenzy = true;
		timerAndMeter.pausePieChart ();
		StartCoroutine (stopFrenzy ());
	}

	IEnumerator stopFrenzy(){
		print ("stopFrenzy before WaitForSeconds (12)");
		yield return new WaitForSeconds (12);
		print ("stopFrenzy after Wait");
		timerAndMeter.unpausePieChart ();
		this.isFrenzy = false;
		timerAndMeter.zerototalDots ();
		timerAndMeter.dropDown ();

	}

	IEnumerator loopBunnyPoint(){
		yield return new WaitForSeconds(3);
		while (!isGameStarted) {
			startBunny.state.SetAnimation(0,"Point",false);
			SoundManager.PlaySFX ("Bunny_Point");
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
			AudioSource gameStartAS = SoundManager.PlaySFX("Bunny_Game_Start");
			startBunny.state.AddAnimation (0, "Idle", true, 0);
			yield return new WaitForSeconds (.05f);
			startBunny.gameObject.GetComponent<Renderer> ().enabled = true;
			StartCoroutine (loopBunnyPoint ());
		}
	}

	bool wasStartBtnPressed = false;
	public SkeletonAnimation startBtn;
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			GameObject touchedGO = mousePick ();
	
			if( touchedGO ){
				if ( touchedGO.tag == "CatchBtn" ) {
					if( !wasStartBtnPressed )
					{
						wasStartBtnPressed = true;
						TrackEntry te = startBtn.state.SetAnimation (0, "Tap", false);
						print ("te.animation.duration: " + te.animation.duration);
						StartCoroutine (delayedButtonMove (te.animation.duration));
						te = startBunny.state.SetAnimation (0, "Start_Duck", true);
						SoundManager.PlaySFX("Bunny_StartDuck");
						StartCoroutine(delayedGameStart(te.animation.duration));
						isGameStarted = true;
						base.startTimer ();
					}
				}
				else if( touchedGO.tag == "ExitBtn" )
				{

				}
				else if( touchedGO.tag == "ReplayBtn" )
				{
					doReplay();
				}
				else if( isGameStarted ){
					handleMashIndex(int.Parse( touchedGO.name ));
				}
			}
		}
	}

	void getGameGoing(){
		StartCoroutine(delayedGameStart(1));
		isGameStarted = true;
		base.startTimer ();
	}

	void doReplay(){
		sbManager.hideResults ();
		getGameGoing ();
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

	int totalAnimals = 0;
	int totalFruits = 0;
	public CrumbColorer crumbColorer;
	void mashAtNode( Transform node ){
		if (node.childCount > 0) {
			WhackAnimal animal = node.GetChild(0).GetComponent<WhackAnimal>();
			if( animal && !animal.whacked ){
				animal.whacked = true;
				string animalName = animal.gameObject.name.Split (" " [0]) [0];
				string soundName = animalName + "_" + "Tap";
//				SoundManager.PlaySFX (soundName);
				animal.GetComponent<SingleSoundBase> ().playSingleSound (soundName);
//				SoundManager.PlaySFX ("OLDWhackTap" + (1+Random.Range (0,2)),false,0,1,1+Random.Range(0.0f,1.0f));
				((SkeletonAnimation)node.GetChild(0).GetComponent<SkeletonAnimation>()).state.SetAnimation(0, "Tap", false);

				timerAndMeter.incrementScore( 5, isFrenzy );
				totalAnimals++;
				Vector3 numbersPos = animal.transform.position + new Vector3( 100, 350, 0 );
				showScore(numbersPos, 5 );
			}
			else{
				SkeletonAnimation skelAnim = node.GetChild(0).GetComponent<SkeletonAnimation>();
				if( skelAnim != null && skelAnim.gameObject.tag == "food"){
					TrackEntry te = skelAnim.state.SetAnimation( 0, "Correct", false );
//					skelAnim.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
//					skelAnim.gameObject.GetComponent<Rigidbody>().useGravity = false;
//					StartCoroutine(delayedDestroy(te.animation.duration, skelAnim.gameObject));

					Color crumbColor = crumbColorer.getColorForFood(skelAnim.gameObject.name);
					Vector3 numbersPos = skelAnim.transform.position + new Vector3( 100, 100, 0 );
					showScore(numbersPos, 10 );
					totalFruits++;
					timerAndMeter.incrementScore( 10, isFrenzy );
					GameObject crumbs = Instantiate( crumbsPrefab, skelAnim.transform.position, Quaternion.identity ) as GameObject;
					crumbs.GetComponent<Renderer>().material.color = crumbColor;
					Destroy ( skelAnim.gameObject );
					StartCoroutine(delayedDestroy( 5, crumbs ));

				}
				else if( skelAnim != null && skelAnim.gameObject.tag == "Balloon"){
					StartCoroutine(popBalloon(skelAnim));
				}
			}
		}
	}

	void showScore( Vector3 numbersPos, int amount ){
		ScoreShower shower = GetComponent<ScoreShower>();
		if( shower != null )
			shower.showScoreAtPosition( numbersPos, amount );
	}

	void duckAllAnimals()
	{
		TrackEntry te = null;

		string animStr = "Duck";

		foreach (Transform node in leftNodes) {
			if( node.childCount > 0 && node.GetChild(0).tag == "WhackAnimal" ){
				if( Time.time - node.GetChild(0).GetComponent<WhackAnimal>().animStartTime > 0.5f )
				{
					SkeletonAnimation skelAnim = node.GetChild(0).GetComponent<SkeletonAnimation>();
					te = skelAnim.state.SetAnimation(0,animStr,false);
					StartCoroutine(delayedDestroy( te.animation.duration, node.GetChild(0).gameObject, true ));
					node.GetChild(0).GetComponent<WhackAnimal>().whacked = true;
				}
				else
				{
					StartCoroutine(delayedDestroy( 0, node.GetChild(0).gameObject, true ));
				}
			}
		}
		foreach (Transform node in centerNodes) {
			if( node.childCount > 0 && node.GetChild(0).tag == "WhackAnimal" ){
				if( Time.time - node.GetChild(0).GetComponent<WhackAnimal>().animStartTime > 0.5f )
				{
					SkeletonAnimation skelAnim = node.GetChild(0).GetComponent<SkeletonAnimation>();
					te = skelAnim.state.SetAnimation(0,animStr,false);
					StartCoroutine(delayedDestroy( te.animation.duration, node.GetChild(0).gameObject, true ));
					node.GetChild(0).GetComponent<WhackAnimal>().whacked = true;
				}
				else
				{
					StartCoroutine(delayedDestroy( 0, node.GetChild(0).gameObject, true ));
				}
			}
		}
		foreach (Transform node in rightNodes) {
			if( node.childCount > 0 && node.GetChild(0).tag == "WhackAnimal" ){
				if( Time.time - node.GetChild(0).GetComponent<WhackAnimal>().animStartTime > 0.5f )
				{
					SkeletonAnimation skelAnim = node.GetChild(0).GetComponent<SkeletonAnimation>();
					te = skelAnim.state.SetAnimation(0,animStr,false);
					StartCoroutine(delayedDestroy( te.animation.duration, node.GetChild(0).gameObject, true ));
					node.GetChild(0).GetComponent<WhackAnimal>().whacked = true;
				}
				else
				{
					StartCoroutine(delayedDestroy( 0, node.GetChild(0).gameObject, true ));
				}
			}
		}
	}
	
	IEnumerator delayedDestroy( float delay, GameObject toDieGO, bool removeFromLiveList = false ){
		yield return new WaitForSeconds (delay);
		if (toDieGO != null) {;
			Destroy(toDieGO);
			if( removeFromLiveList ){
				int index = toDieGO.GetComponent<WhackAnimal>().index;
				if( liveAnimalsIs.Contains(index))
					liveAnimalsIs.Remove(index);
			}
		}
	}

	IEnumerator delayedGameStart( float delay ){
		yield return new WaitForSeconds(delay-0.05f);
		startBtn.transform.Translate (1000, 0, 0);
		gameEnded = false;
		StartCoroutine (loopAnimalPopups());
		if( startBunny != null )
			Destroy (startBunny.gameObject);
		timerAndMeter.dropDown ();
		StartCoroutine (timerAndMeter.delayedPieChartStart (1));
	}

	IEnumerator loopAnimalPopups(){
		while (!gameEnded) {
			yield return new WaitForSeconds( isFrenzy ? 1 : 1);
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
//		popupRandomItem (true);
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

		if (forceFruit) {
			StartCoroutine(popupRandomFruitAtNode (node));
		} else {
			if (shouldSendBalloon()) {
				StartCoroutine (popupBalloonAtNode (node));
			} else {
				popupRandomAnimalAtNode(node);
			}
		}
	}

	void popupRandomAnimalAtNode( Transform node ){
		if (isBalloonPopping) 
			return;
		int randomAnimalI = -1;
		do {
			randomAnimalI = Random.Range (0, 6);
		} while(liveAnimalsIs.Contains(randomAnimalI));
		GameObject animalPrefab = animalPrefabs [randomAnimalI];
		GameObject animal = Instantiate (animalPrefab, node.position, Quaternion.identity) as GameObject;

		StartCoroutine(popupAnimalAtNode (animal, randomAnimalI, node));
	}

	IEnumerator popupAnimalAtNode( GameObject animal, int index, Transform node ){

		liveAnimalsIs.Add (index);
		animal.GetComponent<WhackAnimal> ().index = index;
		animal.GetComponent<WhackAnimal> ().animStartTime = Time.time;
		animal.transform.parent = node;
		animal.transform.localPosition = Vector3.zero;

		TrackEntry te = animal.GetComponent<SkeletonAnimation> ().state.SetAnimation ( 0, "Popup", false);
		string animalName = animal.name.Split (" " [0]) [0];
		string soundName = animalName + "_" + "Popup";
//		SoundManager.PlaySFX (soundName);
		animal.GetComponent<SingleSoundBase> ().playSingleSound (soundName);
		yield return new WaitForSeconds (te.animation.duration+1);
		liveAnimalsIs.Remove (index);
		if (!isBalloonPopping)
			Destroy (animal);
	}

	public int fruitForce = 15000;
	IEnumerator popupRandomFruitAtNode( Transform node ){
		int randomFruitIndex = Random.Range (0, foodNamesArr.Length);
		string fruitName = foodNamesArr [randomFruitIndex];
		GameObject fruit = Instantiate (foodPrefab, Vector3.down * 1000, Quaternion.identity) as GameObject;
		Vector3 localScale = fruit.transform.localScale;
//		localScale *= 0.8f;
		localScale *= 1.6f;
		fruit.transform.localScale = localScale;
		fruit.GetComponent<Collider> ().enabled = false;
		SkeletonAnimation skelAnim = fruit.GetComponent<SkeletonAnimation> ();
		skelAnim.skeleton.SetSkin(fruitName);
		skelAnim.gameObject.name = fruitName;
		fruit.transform.parent = node;
		fruit.transform.localPosition = Vector3.zero;
		fruit.GetComponent<Rigidbody> ().AddForce (Vector3.up * fruitForce);
		fruit.GetComponent<Rigidbody> ().AddTorque (Vector3.forward * fruitForce * Random.Range (-1, 2)*100); 

		yield return new WaitForSeconds(3);
		if (fruit != null) {
			Destroy (fruit);
		}
	}

	//-------------------------------------------------- BALLOON ----------
	int animalsSinceLastBalloon = 0;
	bool shouldSendBalloon(){
		++animalsSinceLastBalloon;
		if (animalsSinceLastBalloon >= 8) {
			if( Random.value < 1.0/6.0 || animalsSinceLastBalloon >= 16)
			{
				animalsSinceLastBalloon = 0;
				return true;
			}
		}
		
		return false;
	}

	IEnumerator popupBalloonAtNode( Transform node ){
		GameObject balloon = Instantiate (balloonPrefab, Vector3.down * 1000, Quaternion.identity) as GameObject;
		Vector3 localScale = balloon.transform.localScale;
		balloon.transform.localScale = localScale;
		
		SkeletonAnimation skelAnim = balloon.GetComponent<SkeletonAnimation> ();
		TrackEntry te = skelAnim.state.SetAnimation (0, "Popup", false);
		SoundManager.PlaySFX ("Balloon_Popup");
		balloon.transform.parent = node;
		Vector3 pos = Vector3.zero;
		pos.x += 30;
		pos.x -= 50;
		balloon.transform.localPosition = pos;
		
		yield return new WaitForSeconds(te.animation.duration);
		if (balloon != null) {
			Destroy (balloon);
		}
	}

	bool isBalloonPopping = false;
	IEnumerator popBalloon(SkeletonAnimation skelAnim)
	{
		isBalloonPopping = true;
		TrackEntry te = skelAnim.state.SetAnimation( 0, "Tap", false );
		SoundManager.PlaySFX ("Balloon_Tap");
		StartCoroutine(delayedDestroy(te.animation.duration, skelAnim.gameObject));
		duckAllAnimals ();
		yield return new WaitForSeconds (te.animation.duration+2);
		isBalloonPopping = false;
	}
}
