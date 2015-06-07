using UnityEngine;
using Spine;
using System.Collections;
using System.Collections.Generic;

public class Table : GameManagerBase {

	public Food food;
	public Transform foodAnimOnStartPos;
	public Transform foodAnimOnTargetPos;
	public TableSpot[] tableSpotArr;
	public GameObject[] characterPrefabArr;
	public ScoreBoard scoreBoard;
	FlySession flySession;
	public GameObject crumbsPrefab;
	public static int level = 0;//0-Based
	public GameObject starAndSpeakersPrefab;

	public GameObject bgSprite;
	public GameObject tableSprite;

	public SkeletonAnimation[] thoughtShapeArr;
	public Transform[] thoughShapeLocArr;

	void Start(){
//		StartCoroutine (doGameWin (6));
		if (level > -1)
			loadTableAndBG ();
		CharacterNode.totalCharactersUnlocked = 4 + (Table.level / 3);
		SoundManager.PlaySFX ("OLDWierdMusic", true);

		foodStartPos = food.transform.localPosition;
		foodStartSize = food.transform.localScale;
		StartCoroutine (checkAndPlayRandomTapAnim ());
	}

	void loadTableAndBG()
	{
		string bgTxtStr = "TableGame/Level_" + (level+1) + "/TableGame_Background";
		string tableTxtStr = "TableGame/Level_" + (level+1) + "/TableGame_Table";
		Texture2D bgSpriteFromRsc = Resources.Load (bgTxtStr) as Texture2D;
//		print ("bgSpriteFromRsc.height: " + bgSpriteFromRsc.height);
		Texture2D tableSpriteFromRsc = Resources.Load (tableTxtStr) as Texture2D;

		bgSprite.GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex", bgSpriteFromRsc);
		tableSprite.GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex", tableSpriteFromRsc);

		createThoughtShapesForThisLevel ();
	}

	void createThoughtShapesForThisLevel(){
		for (int i = 0; i < 3; i++) {
			GameObject thoughtShape = FoodManager.createThoughtShapeForLevel(level+1);
			thoughtShape.name = "thoughtShape";
			thoughtShape.transform.parent = thoughShapeLocArr[i];
			thoughtShape.transform.localPosition = Vector3.zero;
			SkeletonAnimation skelAnim = thoughtShape.GetComponent<SkeletonAnimation>();
			thoughtShapeArr[i] = skelAnim;
			tableSpotArr[i].thoughtBubble.thoughtShape = thoughtShape;
		}
	}

	bool playing = true;
	bool isFlyOut = false;
	IEnumerator checkAndPlayRandomTapAnim(){
		yield return new WaitForSeconds(3);
		while (playing) {
			if( !touchedRecently && !isFoodDragging && !isFlyOut && canTouchAnim )
			{
				playTouchForCharacter(Random.Range(0,3));
			}
			touchedRecently = false;
			yield return new WaitForSeconds(Random.Range (5,10));
		}
	}

	Vector3 foodStartPos;
	Vector3 foodStartSize;
	bool isFoodDragging = false;
	Vector3 lastMousePos;

	bool canTouchAnim = false;

	bool touchedRecently = false;
	AudioSource plateOverAS = null;
	void Update(){
		if( Input.GetMouseButtonDown(0)){
			GameObject hitGO = mousePick();

			if( hitGO ){
//				print ("hitGO.tag: " + hitGO.tag);
				switch( hitGO.tag ){
				case "food":
					if( hitGO.GetComponent<Food>().isSetOnPlate )
						break;
					setAnimForAll( "Pant", true, true );
					isFoodDragging = true;
					foodTouched = true;
					Transform foodBone = food.transform.Find("SkeletonUtility-Root/root/Food");
					EyeFollow.registerFollowTransform (foodBone);
					iTween.Stop();
					lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Grab", false);
					SoundManager.PlaySFX ("Food_Grab");
					((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Drag", true, 0);
					break;
				case "Character1":
					playTouchForCharacter(0);
					break;
				case "Character2":
					playTouchForCharacter(1);
					break;
				case "Character3":
					playTouchForCharacter(2);
					break;
				case "Plate1Btn":
					playTouchForPlate(0);
					break;
				case "Plate2Btn":
					playTouchForPlate(1);
					break;
				case "Plate3Btn":
					playTouchForPlate(2);
					break;
				case "Thought1Btn":
					playTouchForThought(0);
					break;
				case "Thought2Btn":
					playTouchForThought(1);
					break;
				case "Thought3Btn":
					playTouchForThought(2);
					break;
				case "Fly":
					if( !touchedFly )
					StartCoroutine(endFlySession());
					break;
				}
			}
		}
		
		if( Input.GetMouseButton(0)){
			if( isFoodDragging ){
				Vector3 foodPos = food.transform.localPosition;
				Vector3 dragDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePos;
				lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				foodPos.x += dragDelta.x;
				foodPos.y += dragDelta.y;
				foodPos.z = -11;
				food.transform.localPosition = foodPos;

				int newArea = getNewAreaEntered();
				if( newArea != -1 )
				{
//					print ("newArea-1: " + (newArea-1));
					GameObject plateGO = ((TableSpot)tableSpotArr[newArea-1]).plate.gameObject;
					((SkeletonAnimation)plateGO.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Over", false);
//					if( plateOverAS == null ){
//						plateOverAS = SoundManager.PlaySFX("TablePlate_Over");
//					}
//					else if( !plateOverAS.isPlaying ){
//						plateOverAS = SoundManager.PlaySFX("TablePlate_Over");
//					}
//					iTween.PunchPosition( plateGO, iTween.Hash ("time", 0.6f, "amount", Vector3.up * 0.5f, "easetype", iTween.EaseType.easeOutBounce));
				}
			}
		}
		
		if( Input.GetMouseButtonUp(0)){
			if( isFoodDragging ){
				food.gameObject.GetComponent<Collider>().enabled = false;
				GameObject pickedGO = mousePick();
				food.gameObject.GetComponent<Collider>().enabled = true;



				if( pickedGO == null ){
					snapFoodBack();
				}
				else{
					for( int i = 1; i <= 3; i++ ){
						string plateTag = "Plate" + i + "Btn";
						string characterTag = "Character" + i;
						string thoughtTag = "Thought" + i + "Btn";
						if( pickedGO.tag == plateTag ||
						    pickedGO.tag == characterTag ||
						    pickedGO.tag == thoughtTag ){
							moveFoodToPlate(i-1);
						}
					}
				}
				touchedRecently = true;
				setAnimForAll("Idle", true, true );
				isFoodDragging = false;
			}
		}
	}

//	int currentArea = -1;
//	int currentNarrowArea = -1;
//	int lastPlateOver = -1;
//	int lastTallAreaEntered = -1;
	int getNewAreaEntered(){
		int plateOver = -1;
		GameObject hitGO = mousePick();
		if (hitGO != null) {
			switch (hitGO.tag) {
				case "Plate1Btn":
						plateOver = 1;
						break;
				case "Plate2Btn":
						plateOver = 2;
						break;
				case "Plate3Btn":
						plateOver = 3;
						break;
				}
			}
		return plateOver;
	}

	float setAnimForAll( string animName, bool immediate = true, bool loop = false, float delay = 0 ){
		TrackEntry te = null;
		for (int i = 0; i < 3; i++) {
//			GameObject character = tableSpotArr[i].characterNode.transform.GetChild(0).gameObject;
			TableSpot spot = tableSpotArr[i];
			if( spot != null ){
				CharacterNode characterNode = spot.characterNode;
				if( characterNode != null ){
					Transform cnTransform = characterNode.transform;
					if( cnTransform != null ){
						Transform child = cnTransform.GetChild(0);
						if( child != null )
						{
							GameObject character = child.gameObject;

							if( immediate ){
								te = ((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, animName, loop);
							}
							else{
								te = ((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.AddAnimation(0, animName, loop, delay);
							}
						}}}}
		}
		
		return te.animation.duration;
	}

	void playTouchForCharacter( int characterIndex ){
		if (!canTouchAnim || isFlyOut)
						return;

		SoundManager.PlaySFX ("OLDTableWrong" + Random.Range (0, 6));
		string touchAnimStr = "TouchOne";
		if( Random.value < 0.3f )
			touchAnimStr = "TouchTwo";
		if( Random.value < 0.3f )
			touchAnimStr = "TouchThree";

		((SkeletonAnimation)tableSpotArr[characterIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, touchAnimStr, false);
		((SkeletonAnimation)tableSpotArr[characterIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle", true, 0);
		string animalName = tableSpotArr [characterIndex].characterNode.transform.GetChild (0).name;
		StartCoroutine(playSoundForAnimal (touchAnimStr, animalName));
	}

	public SkeletonAnimation[] plateArr;
	void playTouchForPlate( int p ){
		if (canTouchAnim) {
			plateArr [p].state.SetAnimation (0, "Bounce", false);
			SoundManager.PlaySFX("TablePlate_Bounce");
		}
	}
	
	void playTouchForThought( int t ){
		if( canTouchAnim && !isFlyOut )
		thoughtShapeArr[t].state.SetAnimation (0, "Tap-shape", false);
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 300))
		{
			if( hit.collider ){
				return hit.collider.gameObject;
			}
		}
		return null;
	}
	
	void snapFoodBack()
	{
		food.GetComponent<Food> ().isSetOnPlate = false;
		EyeFollow.unregisterFollowTransform ();
		iTween.MoveTo (food.gameObject, iTween.Hash ("position", foodStartPos, "time", 1, "easetype", iTween.EaseType.easeOutElastic, "islocal", true));
		iTween.ScaleTo( food.gameObject, iTween.Hash( "time", 0.5f, "scale",  foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
		((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "NoBurst", false);
	}
	
	void moveFoodToPlate( int plateIndex )
	{
		food.GetComponent<Food> ().isSetOnPlate = true;
		Vector3 pos = tableSpotArr [plateIndex].transform.position;
		pos.z = food.transform.position.z;
		pos.y -= 175f;
		iTween.MoveTo (food.gameObject, pos, 0.5f);
		iTween.ScaleTo( food.gameObject, iTween.Hash( "time", 0.5f, "scale", 0.85f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
		pos.z += 0.1f;
		((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "NoShadow", false);
		StartCoroutine( performResponseForPosition( plateIndex, 0.5f ));
	}

	IEnumerator performResponseForPosition( int plateIndex, float delay = 0 ){
		yield return new WaitForSeconds (delay);
//		if (food.shape == tableSpotArr [plateIndex].thoughtShape) {
		List<Skin> skins = new List<Skin> ();
		skins.Add (food.gameObject.GetComponent<SkeletonAnimation>().skeleton.Skin);

		GameObject thoughtShape = tableSpotArr [plateIndex].transform.FindChild ("ThoughtShapeLoc").GetChild(0).gameObject;
		SkeletonAnimation anim = thoughtShape.GetComponent<SkeletonAnimation> ();
		string thoughtSkinName = thoughtShape.GetComponent<SkeletonAnimation> ().skeleton.Skin.name;

		if (FoodManager.isMatch(level+1, skins, thoughtSkinName)) {
			StartCoroutine(animateFoodMatch(plateIndex));
		} else {
			StartCoroutine(animateFoodMisMatch(plateIndex));
			EyeFollow.unregisterFollowTransform ();
		}
	}

	int totalCorrects = 0;
	IEnumerator animateFoodMatch( int plateIndex ){
		canTouchAnim = false;
		totalCorrects++;
		TrackEntry te = ((SkeletonAnimation)tableSpotArr[plateIndex].thoughtBubble.thoughtShape.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Correct-shape", false);
		((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Correct", false);
		SoundManager.PlaySFX ("Thought_Correct");
		yield return new WaitForSeconds (te.animation.duration);

		//Add VO here
		string skinName = ((SkeletonAnimation)tableSpotArr[plateIndex].thoughtBubble.thoughtShape.GetComponent<SkeletonAnimation> ()).skeleton.Skin.name;
		skinName = skinName.Split ("-" [0]) [0];
//		print ("animateFoodMatch: " + skinName);
		float voDuration = playVoiceOver (skinName);
		yield return new WaitForSeconds (voDuration);

		for (int i = 0; i < 3; i++) {
			if( i == plateIndex || totalCorrects >= 10) {
				te = ((SkeletonAnimation)tableSpotArr[i].thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Leave-thought", false);
				((SkeletonAnimation)tableSpotArr[i].thoughtBubble.thoughtShape.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Leave-shape", false);
				SoundManager.PlaySFX("Thought_Leave");
			}
		}
		yield return new WaitForSeconds (te.animation.duration);
 		((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Eat", false);
		SoundManager.PlaySFX ("TablePlate_Eat");
		delayedEyeUnregister(1);
		StartCoroutine(fireCrumbsForCharacterIndex ( plateIndex ));
		TrackEntry eatTe = ((SkeletonAnimation)tableSpotArr[plateIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Eat", false);
		//Put per character eat anim here
		string animalName = tableSpotArr[plateIndex].characterNode.transform.GetChild(0).name;
		StartCoroutine(playSoundForAnimal ("Eat", animalName));
		canTouchAnim = false;
		StartCoroutine (delayedReactivateTouchAnims (te.animation.duration));
		((SkeletonAnimation)tableSpotArr[plateIndex].plate.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Bounce", false);
		float duration = eatTe.animation.duration;
		StartCoroutine (delayedEyeUnregister (duration));

		yield return new WaitForSeconds(0.5f);	//Little extra time Jason asked for 5/26/2015

		te = ((SkeletonAnimation)tableSpotArr [plateIndex].characterNode.transform.GetChild (0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "ThankYou", false, 0);
		animalName = tableSpotArr [plateIndex].characterNode.transform.GetChild (0).name;
		StartCoroutine (playSoundForAnimal ("ThankYou", animalName, eatTe.animation.duration));
		SoundManager.PlaySFX ("OLDTableRight" + Random.Range (0, 5),false,2,1,1+Random.Range (0.0f,0.5f));
		scoreBoard.addStar ();
		if (totalCorrects < 10) {
			duration += te.animation.duration;
			yield return new WaitForSeconds (duration);
			SoundManager.PlaySFX("Character_Leave");
			te = ((SkeletonAnimation)tableSpotArr [plateIndex].characterNode.transform.GetChild (0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Leave", false, 0);
			duration = te.animation.duration;
			yield return new WaitForSeconds (duration);
			StartCoroutine (resetTurnWithEmptyIndex (plateIndex));
		} else {
			duration += te.animation.duration;
			yield return new WaitForSeconds(duration);
			EyeFollow.unregisterFollowTransform();
			StartCoroutine(doGameWin(te.animation.duration));
		}
	}

	float playVoiceOver( string voStr ){
//		print ("playVoiceOver: " + voStr);
		SoundManager.PlaySFX (voStr);
		return 1;
	}

	IEnumerator playSoundForAnimal( string soundName, string animalName, float delay = 0 ){
		yield return new WaitForSeconds (delay);
		animalName = animalName.Split (" "[0])[0];

		string soundStr = "Table" + animalName + "_" + soundName;
//		print ("soundStr: " + soundStr);
		SoundManager.PlaySFX (soundStr);
	}

	IEnumerator fireCrumbsForCharacterIndex( int i ){
		yield return new WaitForSeconds(1.25f);
		Vector3 pos = new Vector3( -206 + i * 206, 0, -3 );
		Quaternion rot = Quaternion.Euler (new Vector3 (270, 0, 0));
		GameObject crumbs = Instantiate (crumbsPrefab, pos, rot) as GameObject;
		crumbs.GetComponent<Renderer>().material.color = foodColor;
	}

	IEnumerator delayedReactivateTouchAnims( float delay ){
		yield return new WaitForSeconds (delay);
		canTouchAnim = true;
	}

	IEnumerator delayedEyeUnregister( float delay )
	{
		yield return new WaitForSeconds (delay);
		EyeFollow.unregisterFollowTransform ();
	}

	IEnumerator doGameWin(float delay){
		MapUnlockSystem.wasLastGameCompleted = true;
		base.doWin ();
		incrementMapUnlock ();
		yield return new WaitForSeconds (delay);
		playing = false;
		delay = makeEveryoneDance ();
		food.transform.Translate (1000, 0, 0);
		StartCoroutine (scoreBoard.makeStarsDance ());
		GameObject starAndSpeakers = Instantiate (starAndSpeakersPrefab, Vector3.forward * 5, Quaternion.identity) as GameObject;
		SoundManager.PlaySFX ("Dance_Music_01");
		SoundManager.PlaySFX ("TableGame_EndDance");
		iTween.MoveBy (scoreBoard.gameObject, iTween.Hash ("y", 300, "time", 0.5f, "easetype", iTween.EaseType.easeInBounce));
		StartCoroutine (delayedGameExit (delay));
	}

	void incrementMapUnlock(){
		int tableGameCompleted = MapUnlockSystem.tableGameCompleted();
		if (level >= tableGameCompleted) {
			MapUnlockSystem.setTableGameComplete(level);
			MapUnlockSystem.shouldNewButtonUnlock = true;
		}
	}

	IEnumerator delayedGameExit( float delay ){
		iTween.Stop ();
		yield return new WaitForSeconds (delay);
		SoundManager.Stop ();
		DoorManager.closeDoors ();

		yield return new WaitForSeconds (2);
		iTween.Stop ();
		Application.LoadLevel("MainMenu Map");
	}

	float makeEveryoneDance()
	{
		return setAnimForAll ("Dance", true, true);
	}

	public void addIdelForAll(){
		setAnimForAll ("Idle", false, true);
	}

	public float makeAllTap(){
		SoundManager.PlaySFX ("TablePlate_TableTap");
		float delay = setAnimForAll ("TapTable");
		setAnimForAll ("Idle", false, true);
		return delay;
	}

	public float jumpAllPlates( bool playSound = true ){
		TrackEntry te = null;
		for (int i = 0; i < 3; i++) {
			te = ((SkeletonAnimation)tableSpotArr[i].plate.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Bounce", false);
			if( playSound )
				SoundManager.PlaySFX("TablePlate_Bounce");
		}
		
		return te.animation.duration;
	}

	IEnumerator resetTurnWithEmptyIndex( int plateIndex ){
		food.transform.position = foodAnimOnStartPos.position;
		food.GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds (0.5f);
		float delay = addNewCharacterAtSpot (plateIndex);

		float flyRandom = Random.value;
		if (scoreBoard.getTotalStars() > 0 && flyRandom < 0.2f ){
			StartCoroutine(startFlySession (delay));
		} else {
			yield return new WaitForSeconds (delay);
			delay = makeAllTap ();
//			addIdelForAll ();
			jumpAllPlates (false);
			yield return new WaitForSeconds (delay);
			isFoodDragging = false;
			createMatchingFood ();
			StartCoroutine (animateFoodOn ());
			canTouchAnim = true;
		}
	}

	public GameObject prefabFly;

	AudioSource flyAS = null;

	Vector3 flyPosLeft = new Vector3( -472, 242, -11 );
	Vector3 flyPosRight = new Vector3( 489, 242, -11 );
	static bool firstTimeFlyOut = true;
	IEnumerator startFlySession(float delay){
		isFlyOut = true;
		touchedFly = false;

		GameObject flyS = Instantiate (prefabFly, Vector3.left * 1000, Quaternion.identity) as GameObject;
		flySession = flyS.GetComponent<FlySession> ();
		flySession.transform.parent = transform.parent;
		flyAS = SoundManager.PlaySFX ("TableFly_Fly", true);
		flySession.transform.localPosition = flyPosLeft;
		yield return new WaitForSeconds (delay);
		Transform fly = flySession.getFly ();
		fly.GetComponent<Renderer>().enabled = true;
		((SkeletonAnimation)fly.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Fly", true, 0);
		if (firstTimeFlyOut) {
			firstTimeFlyOut = false;
			((SkeletonAnimation)fly.GetComponent<SkeletonAnimation> ()).skeleton.SetSkin("Fly_1");
		} else {
			((SkeletonAnimation)fly.GetComponent<SkeletonAnimation> ()).skeleton.SetSkin("Fly_" + Random.Range(1,18));
		}
		iTween.MoveTo( flySession.gameObject, iTween.Hash( "position", flyPosRight, "time", 10, "islocal", true, "easetype", iTween.EaseType.linear, "name", "flypass", "oncomplete", "turnFlyAround", "oncompletetarget", gameObject));
		StartCoroutine (collapseThoughts ());
		EyeFollow.registerFollowTransform (fly.Find ("SkeletonUtility-Root/root/Body/Mouth-fly"));
		setAnimForAll ("Duck");

	}

	void turnFlyAround(){
		Vector3 position = flyPosRight;
		Vector3 scale = Vector3.one * 0.9f;

		if (flySession.transform.localPosition.x > 0) {
			scale.x *= -1;
			position = flyPosLeft;
		}
		flySession.getFly ().localScale = scale;
		iTween.MoveTo( flySession.gameObject, iTween.Hash( "position", position, "time", 10, "islocal", true, "easetype", iTween.EaseType.linear, "name", "flypass", "oncomplete", "turnFlyAround", "oncompletetarget", gameObject));
	}

	IEnumerator collapseThoughts()
	{
		yield return new WaitForSeconds (1);
		foreach( SkeletonAnimation anim in thoughtShapeArr ){
			anim.state.SetAnimation(0, "Leave-shape", false);
		}
		foreach( TableSpot spot in tableSpotArr ){
			((SkeletonAnimation)spot.thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Leave-thought", false);
			SoundManager.PlaySFX("Thought_Leave");
		}
	}

	bool touchedFly = false;
	IEnumerator endFlySession(){
		if( flyAS != null )
			flyAS.Stop ();
		touchedFly = true;
		TrackEntry te1 = ((SkeletonAnimation)flySession.getFly().GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Touch", false);
		TrackEntry te2 = ((SkeletonAnimation)flySession.getFly().GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Leave", false, 0);
		EyeFollow.unregisterFollowTransform ();
		iTween.StopByName ("flypass");

		SoundManager.PlaySFX("TableFly_Touch");
		SoundManager.PlaySFX ("TableFly_Leave", false, te1.animation.duration);

		yield return new WaitForSeconds (te1.animation.duration + te2.animation.duration);
		Destroy (flySession.gameObject);
		setAnimForAll ("unDuck");
		setAnimForAll ("Idle", false, true);
		StartCoroutine(reopenThoughts ());
	}

	IEnumerator reopenThoughts()
	{
		yield return new WaitForSeconds (1);
		foreach( SkeletonAnimation anim in thoughtShapeArr ){
			anim.state.SetAnimation(0, "Popup-shape", false);
		}

		float delay = 0;
		foreach( TableSpot spot in tableSpotArr ){
			TrackEntry te = ((SkeletonAnimation)spot.thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "PopUp-thought", false);
			SoundManager.PlaySFX("Thought_PopUp");
			((SkeletonAnimation)spot.thoughtBubble.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "IdleOne-thought", false, 0);

			delay = te.animation.duration;
		}

		yield return new WaitForSeconds (delay + 0.5f);
		isFlyOut = false;
		createMatchingFood ();
		StartCoroutine (animateFoodOn ());
	}

	IEnumerator animateFoodMisMatch( int plateIndex ){
		TrackEntry te = ((SkeletonAnimation)tableSpotArr[plateIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Wrong", false);
		string animalName = tableSpotArr [plateIndex].characterNode.transform.GetChild (0).name;
		StartCoroutine(playSoundForAnimal ("Wrong", animalName));
		((SkeletonAnimation)tableSpotArr[plateIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle", true, 0);
		yield return new WaitForSeconds (te.animation.duration/2.0f);
		SoundManager.PlaySFX ("OLDTableWrong" + Random.Range (0, 6),false,1.5f,1,1+Random.Range (0.0f,0.5f));
		yield return new WaitForSeconds (te.animation.duration/2.0f);
		snapFoodBack ();
	}

	public float addNewCharacterAtSpot( int spotIndex ){
		CharacterNode.CharacterType newCharacterType = createRandomCharacterTypeNotAtTable ();
		string newThoughtStr = createRandomThoughtShapeNotAtTable ();
		GameObject thoughtShape = tableSpotArr [spotIndex].transform.FindChild ("ThoughtShapeLoc").GetChild(0).gameObject;
		thoughtShape.transform.localScale = thoughtShape.transform.localScale;
		thoughtShape.GetComponent<SkeletonAnimation> ().skeleton.SetSkin (newThoughtStr);
		GameObject newCharacter = createCharacterForType ( newCharacterType );
		float time = tableSpotArr [spotIndex].addNewCharacterOfType (newCharacterType, ThoughtBubble.ThoughtShape.None, newCharacter, newThoughtStr);

		canTouchAnim = false;
		StartCoroutine (delayedReactivateTouchAnims(time));
		return time;
	}

	public CrumbColorer crumbColorer;
	public GameObject foodPrefab;
	Color foodColor = Color.white;
	public void createMatchingFood(){
		iTween.Stop (food.gameObject);
		Destroy (food.gameObject);
		GameObject foodFromPrefab = FoodManager.createFoodForLevel (level + 1);
		foodFromPrefab.transform.parent = transform;
		foodFromPrefab.transform.localScale = foodStartSize;
		food = (Food)foodFromPrefab.GetComponent<Food>();
		food.GetComponent<Renderer>().enabled = false;
		int randSpotIndex = Random.Range (0, 3);
		food.shape = tableSpotArr [randSpotIndex].thoughtShape;
		TableSpot spot = tableSpotArr [randSpotIndex];
		if (spot != null) {

			GameObject thoughtShape = spot.transform.FindChild ("ThoughtShapeLoc").GetChild(0).gameObject;
			SkeletonAnimation anim = thoughtShape.GetComponent<SkeletonAnimation> ();
			if (anim != null) {
				string skinName = thoughtShape.GetComponent<SkeletonAnimation> ().skeleton.Skin.name;
				FoodManager.setRandomFoodSkinForLevel (foodFromPrefab, level + 1, skinName);
			}
		}

//		foodColor = crumbColorer.getColorForFood (foodSkinName);

	}
	
	public IEnumerator animateFoodOn(){
		yield return new WaitForSeconds (0);
		food.transform.position = foodAnimOnStartPos.position;
		iTween.MoveTo (food.gameObject, iTween.Hash ("time", 1, "position", foodAnimOnTargetPos, "islocal", false, "easetype", iTween.EaseType.easeOutElastic));
		SoundManager.PlaySFX ("Food_Enter");
		food.GetComponent<Renderer>().enabled = true;
		StartCoroutine (delayedConditionalFoodPunch ());
	}

	bool foodTouched = false;
	IEnumerator delayedConditionalFoodPunch(){
		foodTouched = false;
		yield return new WaitForSeconds (5);
		if (!foodTouched) {
				iTween.PunchScale (food.gameObject, iTween.Hash ("time", 2, "amount", 0.5f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
		}
	}

	GameObject createCharacterForType( CharacterNode.CharacterType characterType ){
		GameObject prefab = characterPrefabArr [(int)characterType];

		return Instantiate( prefab, Vector3.down*10, Quaternion.identity ) as GameObject;
	}

	CharacterNode.CharacterType createRandomCharacterTypeNotAtTable(){
		CharacterNode.CharacterType newCharacterType = RandomEnum<CharacterNode.CharacterType>(CharacterNode.totalCharactersUnlocked);

		while(( newCharacterType == tableSpotArr[0].characterType ) ||
		      ( newCharacterType == tableSpotArr[1].characterType ) ||
		      ( newCharacterType == tableSpotArr[2].characterType )){
			newCharacterType = RandomEnum<CharacterNode.CharacterType>(CharacterNode.totalCharactersUnlocked);
		}

		return newCharacterType;
	}

	string createRandomThoughtShapeNotAtTable(){
		SkeletonAnimation thoughtAnim = tableSpotArr [0].transform.FindChild ("ThoughtShapeLoc").GetChild (0).GetComponent<SkeletonAnimation> ();
		AnimationStateData data = thoughtAnim.state.Data;
		List<Skin> skins = data.SkeletonData.Skins;

		Skin randomSkin = skins [Random.Range(1, skins.Count)];

		while(( randomSkin.name == getThoughtShapeSkinNameAtSpot(0) ) ||
		      ( randomSkin.name == getThoughtShapeSkinNameAtSpot(1) ) ||
		      ( randomSkin.name == getThoughtShapeSkinNameAtSpot(2) ) ||
		      ( randomSkin.name == "Hexagon-shape" )){
			randomSkin = skins [Random.Range(0, skins.Count)];
		}

		return randomSkin.name;
	}

	string getThoughtShapeSkinNameAtSpot( int spotI ){
		SkeletonAnimation thoughtAnim = tableSpotArr [spotI].transform.FindChild ("ThoughtShapeLoc").GetChild (0).GetComponent<SkeletonAnimation> ();
		if (thoughtAnim == null)
			return "";
		else{
			if( thoughtAnim.skeleton.Skin == null )
				return "";
			else
			return thoughtAnim.skeleton.Skin.name;
		}
	}

	public T RandomEnum<T>( int cap )
	{ 
		T[] values = (T[]) CharacterNode.CharacterType.GetValues(typeof(T));
		return values[Random.Range (0, cap)];
	}
}
