using UnityEngine;
using Spine;
using System.Collections;

public class Table : MonoBehaviour {

	public Food food;
	public Transform foodAnimOnStartPos;
	public Transform foodAnimOnTargetPos;
	public TableSpot[] tableSpotArr;
	public GameObject[] characterPrefabArr;
	public ScoreBoard scoreBoard;

	void Start(){
		foodStartPos = food.transform.localPosition;
		foodStartSize = food.transform.localScale;
		StartCoroutine (checkAndPlayRandomTapAnim ());
	}

	bool playing = true;
	IEnumerator checkAndPlayRandomTapAnim(){
		while (playing) {
			yield return new WaitForSeconds(Random.Range (5,10));
			if( !touchedRecently && !isFoodDragging )
			{
				playTouchForCharacter(Random.Range(0,3));
			}
			touchedRecently = false;
		}
	}

	Vector3 foodStartPos;
	Vector3 foodStartSize;
	bool isFoodDragging = false;
	Vector3 lastMousePos;

	bool touchedRecently = false;
	void Update(){
		if( Input.GetMouseButtonDown(0)){
			GameObject hitGO = mousePick();

			if( hitGO ){
				switch( hitGO.tag ){
				case "food":
					isFoodDragging = true;
					EyeFollow.registerFollowTransform (food.transform);
					iTween.Stop();
					lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					iTween.StopByName("idlepunch");
					iTween.StopByName("shadowpunch");
					iTween.FadeTo( foodShadow, 0, 0.25f );
					iTween.ScaleTo( food.gameObject, iTween.Hash( "time", 0.5f, "scale", 1.2f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
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
				}
			}
		}
		
		if( Input.GetMouseButton(0)){
			if( isFoodDragging ){
				Vector3 foodPos = food.transform.localPosition;
				Vector3 dragDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePos;
				lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//				dragDelta.x *= 0.002f * 0.5f;
//				dragDelta.y *= 0.003f * 0.5f;
//				foodPos += dragDelta;
//				Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				foodPos.x += dragDelta.x;
				foodPos.y += dragDelta.y;
				food.transform.localPosition = foodPos;

				int newArea = getNewAreaEntered();
				if( newArea != -1 )
				{
					GameObject plateGO = ((TableSpot)tableSpotArr[newArea-1]).plate.gameObject;
					((SkeletonAnimation)plateGO.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Over", false);
//					iTween.PunchPosition( plateGO, iTween.Hash ("time", 0.6f, "amount", Vector3.up * 0.5f, "easetype", iTween.EaseType.easeOutBounce));
				}
			}
		}
		
		if( Input.GetMouseButtonUp(0)){
			if( isFoodDragging ){
				food.gameObject.collider.enabled = false;
				GameObject pickedGO = mousePick();
				food.gameObject.collider.enabled = true;
				
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
			}
			touchedRecently = true;
			isFoodDragging = false;
		}
	}

	int currentArea = -1;
	int getNewAreaEntered(){
		float xPos = Input.mousePosition.x;
		if (Input.mousePosition.y > Screen.height * 0.25f) {
			for( int i = 0; i < 3; i++ ){
				if( xPos < Screen.width/3 * (i+1) ){
					if( currentArea != (i+1) ){
						currentArea = (i+1);
						return currentArea;
					}
					return -1;
				}
			}
		}

		return -1;
	}

	float setAnimForAll( string animName, bool immediate = true, bool loop = false, float delay = 0 ){
		TrackEntry te = null;
		for (int i = 0; i < 3; i++) {
			GameObject character = tableSpotArr[i].characterNode.transform.GetChild(0).gameObject;
			if( immediate ){
				te = ((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, animName, loop);
			}
			else{
				te = ((SkeletonAnimation)character.GetComponent<SkeletonAnimation> ()).state.AddAnimation(0, animName, loop, delay);
			}
		}
		
		return te.animation.duration;
	}

	void playTouchForCharacter( int characterIndex ){
		string touchAnimStr = "TouchOne";
		if( Random.value < 0.3f )
			touchAnimStr = "TouchTwo";
		if( Random.value < 0.3f )
			touchAnimStr = "TouchThree";

		((SkeletonAnimation)tableSpotArr[characterIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, touchAnimStr, false);
		((SkeletonAnimation)tableSpotArr[characterIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle", true, 0);
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
		EyeFollow.unregisterFollowTransform ();
		iTween.MoveTo (food.gameObject, iTween.Hash ("position", foodStartPos, "time", 1, "easetype", iTween.EaseType.easeOutElastic, "islocal", true));
		iTween.ScaleTo( food.gameObject, iTween.Hash( "time", 0.5f, "scale",  foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
		iTween.FadeTo( foodShadow, 1, 0.25f );
	}
	
	void moveFoodToPlate( int plateIndex )
	{
		Vector3 pos = tableSpotArr [plateIndex].transform.position;
		pos.z = food.transform.position.z;
		pos.y -= 175f;
		iTween.MoveTo (food.gameObject, pos, 0.5f);
		iTween.ScaleTo( food.gameObject, iTween.Hash( "time", 0.5f, "scale", 0.85f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic));
		StartCoroutine( performResponseForPosition( plateIndex, 0.5f ));
	}

	IEnumerator performResponseForPosition( int plateIndex, float delay = 0 ){
		yield return new WaitForSeconds (delay);
		if (food.shape == tableSpotArr [plateIndex].thoughtShape) {
			StartCoroutine(animateFoodMatch(plateIndex));
		} else {
			StartCoroutine(animateFoodMisMatch(plateIndex));
		}
	}

	int totalCorrects = 0;
	IEnumerator animateFoodMatch( int plateIndex ){
		totalCorrects++;
		TrackEntry te = ((SkeletonAnimation)tableSpotArr[plateIndex].thoughtBubble.thoughtShape.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Correct-shape", false);
		((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Grab", false);
//		((SkeletonAnimation)tableSpotArr[plateIndex].thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "IdleOne-thought", false);
		yield return new WaitForSeconds (te.animation.duration);
		for (int i = 0; i < 3; i++) {
			if( i == plateIndex || totalCorrects >= 12) {
				te = ((SkeletonAnimation)tableSpotArr[i].thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Leave-thought", false);
				((SkeletonAnimation)tableSpotArr[i].thoughtBubble.thoughtShape.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Leave-shape", false);
			}
		}
		yield return new WaitForSeconds (te.animation.duration);
		((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Eat", false);
		EyeFollow.unregisterFollowTransform ();
		te = ((SkeletonAnimation)tableSpotArr[plateIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Eat", false);
		((SkeletonAnimation)tableSpotArr[plateIndex].plate.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Bounce", false);
		float duration = te.animation.duration;
		te = ((SkeletonAnimation)tableSpotArr [plateIndex].characterNode.transform.GetChild (0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "ThankYou", false, 0);
		scoreBoard.addStar ();
		if (totalCorrects < 12) {
						duration += te.animation.duration;
						te = ((SkeletonAnimation)tableSpotArr [plateIndex].characterNode.transform.GetChild (0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Leave", false, 0);
						duration += te.animation.duration;
						yield return new WaitForSeconds (duration);
						StartCoroutine (resetTurnWithEmptyIndex (plateIndex));
				} else {
			StartCoroutine(doGameWin(te.animation.duration));
				}
	}

	IEnumerator doGameWin(float delay){
		yield return new WaitForSeconds (delay);
		playing = false;
		delay = makeEveryoneDance ();
		food.transform.Translate (10, 0, 0);
		StartCoroutine (scoreBoard.makeStarsDance ());
	}

	float makeEveryoneDance()
	{
		return setAnimForAll ("Dance", true, true);
	}

	public void addIdelForAll(){
		setAnimForAll ("Idle", false, true);
	}

	public float makeAllTap(){
		return setAnimForAll ("TapTable");
	}

	float jumpAllPlates(){
		TrackEntry te = null;
		for (int i = 0; i < 3; i++) {
			te = ((SkeletonAnimation)tableSpotArr[i].plate.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Bounce", false);
		}
		
		return te.animation.duration;
	}

	IEnumerator resetTurnWithEmptyIndex( int plateIndex ){
		food.transform.position = foodAnimOnStartPos.position;
		food.renderer.enabled = false;
		yield return new WaitForSeconds (0.5f);
		float delay = addNewCharacterAtSpot (plateIndex);
		yield return new WaitForSeconds (delay);
		delay = makeAllTap ();
		addIdelForAll ();
		jumpAllPlates ();
		yield return new WaitForSeconds (delay);
		isFoodDragging = false;
		animateMatchingFoodOn ();
	}

	IEnumerator animateFoodMisMatch( int plateIndex ){
		TrackEntry te = ((SkeletonAnimation)tableSpotArr[plateIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Wrong", false);
		((SkeletonAnimation)tableSpotArr[plateIndex].characterNode.transform.GetChild(0).GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle", true, 0);
		yield return new WaitForSeconds (te.animation.duration);
		snapFoodBack ();
	}

	public float addNewCharacterAtSpot( int spotIndex ){
		CharacterNode.CharacterType newCharacterType = createRandomCharacterTypeNotAtTable ();
		ThoughtBubble.ThoughtShape newThoughtshape = createRandomThoughtShapeNotAtTable ();

		GameObject newCharacter = createCharacterForType ( newCharacterType );
		float time = tableSpotArr [spotIndex].addNewCharacterOfType (newCharacterType, newThoughtshape, newCharacter);
		return time;
	}

	public GameObject foodPrefab;
	public GameObject foodShadow;
	public void animateMatchingFoodOn(){
		Destroy (food.gameObject);
		GameObject foodFromPrefab = Instantiate (foodPrefab, foodAnimOnStartPos.position, Quaternion.identity) as GameObject;
		foodShadow.renderer.enabled = true;
		foodFromPrefab.transform.parent = transform;
		foodFromPrefab.transform.localScale = foodStartSize;
		food = (Food)foodFromPrefab.GetComponent<Food>();
		food.renderer.enabled = false;
		int randSpotIndex = Random.Range (0, 3);
		food.shape = tableSpotArr [randSpotIndex].thoughtShape;

		string foodSkinName = "";

		switch (food.shape) {
		case ThoughtBubble.ThoughtShape.Circle:
				switch (Random.Range (0, 4)) {
				case 0:
					foodSkinName = "Cookie-circle";
					break;
				case 1:
					foodSkinName = "Donut-circle";
					break;
				case 2:
					foodSkinName = "GreenApple-circle";
					break;
				case 3:
					foodSkinName = "Orange-circle";
					break;
				}
				break;
		case ThoughtBubble.ThoughtShape.Diamond:
				switch (Random.Range (0, 4)) {
				case 0:
					foodSkinName = "Candy-green-diamond";
					break;
				case 1:
					foodSkinName = "Candy-pink-diamond";
					break;
				case 2:
					foodSkinName = "Chocolate-diamond";
					break;
				case 3:
					foodSkinName = "Cracker-diamond";
					break;
				}
				break;
		case ThoughtBubble.ThoughtShape.HalfCircle:
				switch (Random.Range (0, 4)) {
				case 0:
					foodSkinName = "Apple-half-circle";
					break;
				case 1:
					foodSkinName = "Canteloupe-half-circle";
					break;
				case 2:
					foodSkinName = "Orange-half-circle";
					break;
				case 3:
					foodSkinName = "Watermelon-half-circle";
					break;
				}
				break;
//		case ThoughtBubble.ThoughtShape.Hexagon:
//			switch(Random.Range(0,4)){
//			case 0:
//				foodSkinName = "";
//				break;
//			case 1:
//				foodSkinName = "";
//				break;
//			case 2:
//				foodSkinName = "";
//				break;
//			case 3:
//				foodSkinName = "";
//				break;
//			}
//			break;
		case ThoughtBubble.ThoughtShape.Oval:
				switch (Random.Range (0, 4)) {
				case 0:
					foodSkinName = "Bread-oval";
					break;
				case 1:
					foodSkinName = "Burger-oval";
					break;
				case 2:
					foodSkinName = "Pumpkin-oval";
					break;
				case 3:
					foodSkinName = "Watermelon-oval";
					break;
				}
				break;
		case ThoughtBubble.ThoughtShape.Rectangle:
				switch (Random.Range (0, 3)) {
				case 0:
					foodSkinName = "Chocolate-rectangle";
					break;
				case 1:
					foodSkinName = "Ice-cream-rectangle";
					break;
				case 2:
					foodSkinName = "Pastry-rectangle";
					break;
				}
				break;
		case ThoughtBubble.ThoughtShape.Square:
				switch (Random.Range (0, 4)) {
				case 0:
					foodSkinName = "Chocolate-square";
					break;
				case 1:
					foodSkinName = "Cracker-square";
					break;
				case 2:
					foodSkinName = "Pizza-square";
					break;
				case 3:
					foodSkinName = "Sandwhich-square";
					break;
				}
				break;
		case ThoughtBubble.ThoughtShape.Star:
				switch (Random.Range (0, 3)) {
				case 0:
					foodSkinName = "Candy-blue-star";
					break;
				case 1:
					foodSkinName = "Candy-pink-star";
					break;
				case 2:
					foodSkinName = "Cookie-star";
					break;
				}
				break;
		case ThoughtBubble.ThoughtShape.Triangle:
				switch (Random.Range (0, 4)) {
				case 0:
					foodSkinName = "Cheese-triangle";
					break;
				case 1:
					foodSkinName = "Pizza-triangle";
					break;
				case 2:
					foodSkinName = "Strawberry-triangle";
					break;
				case 3:
					foodSkinName = "Watermelon-triangle";
					break;
				}
				break;
		default:
				break;
		}

//		print ("shape: " + food.shape + ", \tfoodSkinName: " + foodSkinName);
		SkeletonAnimation skelAnim = ((SkeletonAnimation)food.GetComponent<SkeletonAnimation> ());
		skelAnim.skeleton.SetSkin (foodSkinName);

		StartCoroutine (animateFoodOn ());
	}
	
	IEnumerator animateFoodOn(){
		yield return new WaitForSeconds (0);
		food.transform.position = foodAnimOnStartPos.position;
		foodShadow.transform.position = foodAnimOnStartPos.position;
		Vector3 shadowPos = foodAnimOnTargetPos.position;
		shadowPos.z = -5.22f;
		iTween.MoveTo (food.gameObject, iTween.Hash ("time", 1, "position", foodAnimOnTargetPos, "islocal", false, "easetype", iTween.EaseType.easeOutElastic));
		iTween.MoveTo (foodShadow, iTween.Hash ("time", 1, "position", shadowPos, "islocal", false, "easetype", iTween.EaseType.easeOutElastic));
		food.renderer.enabled = true;
		iTween.FadeTo( foodShadow, 1, 0.25f );
		iTween.PunchScale( food.gameObject, iTween.Hash( "time", 2, "amount", 0.5f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic, "delay", 5, "looptype", iTween.LoopType.loop, "name", "idlepunch" ));
		iTween.PunchScale( foodShadow.gameObject, iTween.Hash( "time", 2, "amount", 0.5f * foodStartSize, "easetype", iTween.EaseType.easeOutElastic, "delay", 5, "looptype", iTween.LoopType.loop, "name", "shadowpunch" ));
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

	ThoughtBubble.ThoughtShape createRandomThoughtShapeNotAtTable(){
		ThoughtBubble.ThoughtShape newThoughtShape = RandomEnum<ThoughtBubble.ThoughtShape>(ThoughtBubble.totalShapesUnlocked);

		while(( newThoughtShape == tableSpotArr[0].thoughtShape ) ||
		      ( newThoughtShape == tableSpotArr[1].thoughtShape ) ||
		      ( newThoughtShape == tableSpotArr[2].thoughtShape )){
			newThoughtShape = RandomEnum<ThoughtBubble.ThoughtShape>(ThoughtBubble.totalShapesUnlocked);
		}

		return newThoughtShape;
	}

	public T RandomEnum<T>( int cap )
	{ 
		T[] values = (T[]) CharacterNode.CharacterType.GetValues(typeof(T));
		return values[Random.Range (0, cap)];
	}
}
