using UnityEngine;
using Spine;
using System.Collections;

public class MapManager : MonoBehaviour {

	public GameObject levelHolder;
	public GameObject bird;
	public GameObject cat;
	public GameObject fox;
	public GameObject frog;
	public GameObject monkey;
	public GameObject goSign;

	public GameObject[] leftArrows;
	public GameObject[] rightArrows;

	static bool firstTime = true;
	public SkeletonAnimation[] mapDenizenArr;

	bool isPlaying = false;
	IEnumerator loopRandomAnims(){
		while (isPlaying) {
			yield return new WaitForSeconds(6);
			randomAnimOnAll();
		}
	}

	void randomAnimOnAll(){
		string animStr = "Touch" + (Random.value < 0.5f ? "One" : "Two");
		int highestTableGameUnlocked = MapUnlockSystem.tableGameCompleted () + 1;

		for( int i = 0; i < mapDenizenArr.Length; i++ ){
			if(i < highestTableGameUnlocked%3){
				mapDenizenArr[i].state.SetAnimation(0, animStr, false);
				mapDenizenArr[i].state.AddAnimation(0, "Idle", true, 0);
			}
			else{
//				if( i > 0 )
//					mapDenizenArr[i].state.SetAnimation( 0, "Sleep", true );
			}
		}
	}

	//LIGHTPOSTS
	public SkeletonAnimation[] lightpostsArr;

	IEnumerator loopRandomLightpostAnims(){
		yield return new WaitForSeconds(3);
		while (isPlaying) {
			yield return new WaitForSeconds(12);
			randomLighpostAnimOnAll();
		};
	}
	
	void randomLighpostAnimOnAll(){
		if( currentPage >=1 && currentPage <= 3 )
			SoundManager.PlaySFX("Lightpole_Touch4");

		string animStr = "TouchOne2";
		foreach( SkeletonAnimation anim in lightpostsArr ){
			anim.state.SetAnimation(0,animStr, false);
			anim.state.AddAnimation(0,"Idle", true, 0);
		}
	}
	//

	void Start(){

		if (firstTime) {
			DoorManager.immediateOpen();
			firstTime = false;
		}

		startHappyvilleSignedAnimsHandle ();

		if (openPageIndex > 0) {
			startMusic();
			DoorManager.openDoors();
			StartCoroutine( navigateToPage(openPageIndex, false, false, true ));
		}
		setProperButtonStates ();
		SoundManager.PlaySFX ("Menu_Ambient_Background_Loop", true, 0, .05f);
	}

	void setProperButtonStates(){
		int highestTableGameUnlocked = MapUnlockSystem.tableGameCompleted ();
		int highestMiniGameUnlocked = MapUnlockSystem.miniGamePlayed ();
//		print ("highestTableGameUnlocked: " + highestTableGameUnlocked + " highestMiniGameUnlocked: " + highestMiniGameUnlocked);

		//set animals asleep and awake
		for (int i = 1; i < mapDenizenArr.Length; i++) {
			string animalAnimName = (i <= (highestTableGameUnlocked/3)) ? "Idle" : "Sleep";
			mapDenizenArr[i].state.SetAnimation( 0, animalAnimName, true );
		}

		//set table buttons locked and idle
		for( int i = 1; i < tableGameButtons.Length; i++){
			SkeletonAnimation skelAnim = tableGameButtons[i].GetComponent<SkeletonAnimation>();
			string animName = (i <= highestTableGameUnlocked) ? "Active_Idle" : "Locked_Idle";
			if( i > 9 ){
				animName = (i <= highestTableGameUnlocked) ? "Active_Float_Idle" : "Locked_Float_Idle";
			}
			skelAnim.state.SetAnimation(0, animName, true);
		}

		//set minigame buttons locked and idle
		for( int i = 0; i < miniGameButtonsArr.Length; i++){
			string animName = (i <= highestMiniGameUnlocked) ? "Active_Idle" : "Locked_Idle";
			if( i > 3 ){
				animName = (i < highestMiniGameUnlocked) ? "Active_Float_Idle" : "Locked_Idle";//"Locked_Float_Idle";
			}
			miniGameButtonsArr[i].state.SetAnimation(0, animName, true);
		}

		if (MapUnlockSystem.lastGamePlayed != MapUnlockSystem.GameType.TableGame) {
			determineIfMiniGameWasPlayedForTheFirstTime();
		}

		//Set last button immediately or with animation
		string lastAnimName = (highestTableGameUnlocked < 9 ) ? "Active_Idle" : "Active_Float_Idle" ;
		SkeletonAnimation lastButtonAnim = tableGameButtons[highestTableGameUnlocked].GetComponent<SkeletonAnimation>();
		if (MapUnlockSystem.shouldNewButtonUnlock) {
			StartCoroutine(unlockNextLevel());
		} else {
			lastButtonAnim.state.SetAnimation(0, lastAnimName, true);
		}
	}

	void determineIfMiniGameWasPlayedForTheFirstTime(){
		int lastGameIndex = -1;
		switch (MapUnlockSystem.lastGamePlayed)
		{
		case MapUnlockSystem.GameType.CatchGame:{		lastGameIndex = 0;		}	break;
		case MapUnlockSystem.GameType.PuzzleGame:{		lastGameIndex = 1;		}	break;	
		case MapUnlockSystem.GameType.WhackGame:{		lastGameIndex = 2;		}	break;
		case MapUnlockSystem.GameType.Photobooth:{		lastGameIndex = 3;		}	break;
		case MapUnlockSystem.GameType.SlingshotGame:{	lastGameIndex = 4;		}	break;
		case MapUnlockSystem.GameType.MatchGame:{		lastGameIndex = 5;		}	break;
		}

		if (lastGameIndex >= MapUnlockSystem.miniGamePlayed()) {
			MapUnlockSystem.setMiniGamePlayed(lastGameIndex);
			MapUnlockSystem.shouldNewButtonUnlock = true;
		}
	}

	IEnumerator unlockNextLevel()
	{
		print ("unlockNextLevel with lastGamePlayed: " + MapUnlockSystem.lastGamePlayed);
		switch (MapUnlockSystem.lastGamePlayed) {
		case MapUnlockSystem.GameType.TableGame:
		{
			int levelMod = Table.level % 3;
			print ("levelMod: " + levelMod);
			if (levelMod == 0 || levelMod == 1) {
				StartCoroutine(unlockTableButton (Table.level + 1));
			} else if (levelMod == 2) {
				unlockMinigame (Table.level / 3);
			}
		}
			break;
		
		case MapUnlockSystem.GameType.CatchGame:
		{
			StartCoroutine(unlockTableButton(3));
		}
			break;

		case MapUnlockSystem.GameType.PuzzleGame:
		{
			StartCoroutine(unlockTableButton(6));
		}
			break;

		case MapUnlockSystem.GameType.WhackGame:
		{
			StartCoroutine(unlockTableButton(9));
		}
			break;

		case MapUnlockSystem.GameType.Photobooth:
		{
			StartCoroutine(unlockTableButton(12));
		}
			break;

		case MapUnlockSystem.GameType.SlingshotGame:
		{
			StartCoroutine(unlockTableButton(15));
		}
			break;

		case MapUnlockSystem.GameType.MatchGame:
		{
			//Nothing left to unlock
		}
			break;
		}

		yield return new WaitForSeconds (0);
	}

	IEnumerator unlockTableButton( int l ){
		if (l % 3 == 0) {
			StartCoroutine (navigateToPage (l / 3 + 1, false, false, true));
		}
		yield return new WaitForSeconds(1);
		SoundManager.PlaySFX ("Path_Active_Unlock");
		MapUnlockSystem.setTableGameComplete (MapUnlockSystem.tableGameCompleted () + 1);
		tableGameButtons [l].GetComponent<SkeletonAnimation> ().state.SetAnimation (0, "Unlock_Button", false);
		string idleAnimName = (l < 9 ? "Active_Idle" : "Active_Float_Idle");
		tableGameButtons [l].GetComponent<SkeletonAnimation> ().state.AddAnimation(0, idleAnimName, true, 0);
		mapDenizenArr [l / 3].state.SetAnimation (0, "Unlock", false);
		mapDenizenArr [l / 3].state.AddAnimation (0, "Idle", false, 0);

		if (l >= 6 && l <= 8) {
			SoundManager.PlaySFX("MenuBunny_Unlock");
		}
	}

	void unlockMinigame( int m ){
		print ("unlockMinigame: " + m);
		MapUnlockSystem.setMiniGamePlayed (MapUnlockSystem.miniGamePlayed () + 1);
		miniGameButtonsArr [m].state.SetAnimation (0, "Unlock_Button", false);
		string idleAnimName = (m < 9 ? "Active_Idle" : "Active_Float_Idle");
		miniGameButtonsArr [m].state.AddAnimation (0, idleAnimName, true, 0);
	}

	void startHappyvilleSignedAnimsHandle(){
		StartCoroutine (startPopupAnims ());
		StartCoroutine (fireRandomArrowAnims ());
	}

	public static int openPageIndex = 0;
	AudioSource popupAS = null;
	IEnumerator startPopupAnims(){
		yield return new WaitForSeconds (3);
		popupAS = SoundManager.PlaySFX ("Title_CharacterPopup", false, 0, (currentPage == 0) ? 100 : 0);
		((SkeletonAnimation)bird.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-brd", false);
		((SkeletonAnimation)fox.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-fox", false);
		((SkeletonAnimation)cat.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-cat", false);
		((SkeletonAnimation)monkey.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-monk", false);
		((SkeletonAnimation)frog.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup-frg", false);
		yield return new WaitForSeconds(0.01f);
		bird.GetComponent<Renderer>().enabled = true;
		cat.GetComponent<Renderer>().enabled = true;
		fox.GetComponent<Renderer>().enabled = true;
		frog.GetComponent<Renderer>().enabled = true;
		monkey.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds (2);
		startMusic ();
		StartCoroutine( makeCharactersCheer ());
	}

	AudioSource music = null;
//	AudioSource swaySound = null;
	IEnumerator makeCharactersCheer(){
		if (currentPage == 0) {
//						SoundManager.PlaySFX ("OLDGameStartCheer");
//			swaySound = SoundManager.PlaySFX ("GoSign_Sway", true);
				}
		TrackEntry trackEntry = ((SkeletonAnimation)bird.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Cheer-brd", false);
		((SkeletonAnimation)bird.GetComponent<SkeletonAnimation> ()).state.AddAnimation(0, "Idle-brd", true, 0);
		((SkeletonAnimation)cat.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Cheer-cat", false);
		((SkeletonAnimation)cat.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle-cat", true, 0);
		((SkeletonAnimation)fox.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Cheer-fox", false);
		((SkeletonAnimation)fox.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle-fox", true, 0);
		((SkeletonAnimation)frog.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Cheer-frg", false);
		((SkeletonAnimation)frog.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle-frg", true, 0);
		((SkeletonAnimation)monkey.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Cheer-monk", false);
		((SkeletonAnimation)monkey.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Idle-monk", true, 0);
		yield return new WaitForSeconds (trackEntry.animation.duration);
		((SkeletonAnimation)goSign.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Sway", true);
		StartCoroutine (fireRandomTouchRoutines ());
	}

	void startMusic()
	{
		if( music == null )
			music = SoundManager.PlaySFX ("HappyMusic", true, 0, 0);
	}

	bool isAnimatingTouch = false;
	IEnumerator fireRandomTouchRoutines(){
		while (true) {
			yield return new WaitForSeconds (3);
			if( !isAnimatingTouch){
				switch(Random.Range(0, 5)){
				case 0:
					StartCoroutine(fireBirdTouchAnim());
					break;
				case 1:
					StartCoroutine(fireCatTouchAnim());
					break;
				case 2:
					StartCoroutine(fireFoxTouchAnim());
					break;
				case 3:
					StartCoroutine(fireFrogTouchAnim());
					break;
				case 4:
					StartCoroutine(fireMonkeyTouchAnim());
					break;
				}
			}
		}
	}

	public static int currentPage = 0;
	float kPageLength = 768f;

	int totalBirdClicks = 0;
	int totalCatClicks = 0;
	int totalFoxClicks = 0;
	int totalFrogClicks = 0;
	int totalMonkeyClicks = 0;

	public GameObject[] tableGameButtons;
	public SkeletonAnimation[] miniGameButtonsArr;

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			if( Physics.Raycast( Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100 )){
				string tag = hit.collider.gameObject.tag;
				switch(tag){
				case "HomeBtn":
					if(currentPage > 0){
						StartCoroutine( navigateToPage(0));
					}
					break;
				case "NavLeftBtn":
					if(currentPage > 0){
						StartCoroutine(navigateToPage(currentPage - 1, true, true));
						SoundManager.PlaySFX ("NavSign_Click");
					}
					break;
				case "NavRightBtn":
					if(currentPage > 0 && currentPage < 6){
						StartCoroutine(navigateToPage(currentPage + 1, true, false));
						SoundManager.PlaySFX ("NavSign_Click");
					}
					break;
				case "ForParentsBtn":
					
					break;
				case "StartBtn":
					StartCoroutine(clickGoSign());
					break;
				case "BirdTitleBtn":
					isAnimatingTouch = true;
					StartCoroutine(fireBirdTouchAnim());
					StartCoroutine(delayedTouchOff());
					break;
				case "CatTitleBtn":
					isAnimatingTouch = true;
					StartCoroutine(fireCatTouchAnim());
					StartCoroutine(delayedTouchOff());
					break;
				case "FrogTitleBtn":
					isAnimatingTouch = true;
					StartCoroutine(fireFrogTouchAnim());
					StartCoroutine(delayedTouchOff());
					break;
				case "FoxTitleBtn":
					isAnimatingTouch = true;
					StartCoroutine(fireFoxTouchAnim());
					StartCoroutine(delayedTouchOff());
					break;
				case "MonkeyTitleBtn":
					isAnimatingTouch = true;
					StartCoroutine(fireMonkeyTouchAnim());
					StartCoroutine(delayedTouchOff());
					break;
				case "TableGameBtn":
					int buttonIndex = int.Parse( hit.collider.gameObject.name);
					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.TableGame;
					MapUnlockSystem.lastTableGamePlayed = buttonIndex;
//					print ("buttonIndex: " + buttonIndex + ", MapUnlockSystem.tableGameCompleted(): " + MapUnlockSystem.tableGameCompleted());
					if( buttonIndex <= MapUnlockSystem.tableGameCompleted() || MapUnlockSystem.shouldAutoUnlock)
					{
						Table.level = buttonIndex;
						GameObject button = tableGameButtons[buttonIndex];
						((SkeletonAnimation)button.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Active_press", false);
						StartCoroutine(loadTableGame("Table Game", currentPage, true));
					}
					break;
				case "CatchGameBtn":
					if( MapUnlockSystem.miniGamePlayed() >= 0 || MapUnlockSystem.shouldAutoUnlock )
					{
					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.CatchGame;
					StartCoroutine(loadTableGame("CatchGame", 1));
					}
					break;
				case "WhackGame":
					if( MapUnlockSystem.miniGamePlayed() >= 2 || MapUnlockSystem.shouldAutoUnlock )
					{
					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.WhackGame;
					StartCoroutine(loadTableGame("Whack Game", 3));
					}
					break;
				case "SlingshotGameBtn":
					if( MapUnlockSystem.miniGamePlayed() >= 4 || MapUnlockSystem.shouldAutoUnlock )
					{
					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.SlingshotGame;
					StartCoroutine(loadTableGame("Slingshot Game", 5));
					}
					break;
				case "PuzzleGame":
					if( MapUnlockSystem.miniGamePlayed() >= 1 || MapUnlockSystem.shouldAutoUnlock )
					{
					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.PuzzleGame;
					StartCoroutine(loadTableGame("PuzzleGame", 2));
					}
					break;
				case "MatchingGame":
					if( MapUnlockSystem.miniGamePlayed() >= 5 || MapUnlockSystem.shouldAutoUnlock )
					{
					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.MatchGame;
					StartCoroutine(loadTableGame("MatchingGame", 6));
					}
					break;
				case "PhotoBooth":
//					if( MapUnlockSystem.miniGamePlayed() >= 3 || MapUnlockSystem.shouldAutoUnlock )
//					{
//					MapUnlockSystem.lastGamePlayed = MapUnlockSystem.GameType.Photobooth;
//					StartCoroutine(loadTableGame("PhotoBooth", 4));
//					}
					break;
				default:
					break;
				}
			}
		}
	}

	static string lastGame;
	IEnumerator loadTableGame( string gameName, int pageIndex, bool isTableButton = false){
		SoundManager.PlaySFX ("Path_Active_Press");
		MapUnlockSystem.wasLastGameCompleted = false;
		MapUnlockSystem.shouldNewButtonUnlock = false;
		lastGame = gameName;
		isPlaying = false;
		if (!isTableButton) {
			miniGameButtonsArr[pageIndex-1].state.SetAnimation(0,"Active_press",false);
		}
		openPageIndex = pageIndex;
//		print ("openPageIndex: " + openPageIndex);
		iTween.Stop ();
		yield return new WaitForSeconds (1);
		SoundManager.Stop();
		DoorManager.closeDoors ();

		yield return new WaitForSeconds (2.5f);
		iTween.Stop ();
		Application.LoadLevel (gameName);
	}

	IEnumerator fireBirdTouchAnim(){
		totalBirdClicks= (totalBirdClicks + 1) % 2;
		string birdAnimName = (totalBirdClicks == 0 ? "TouchOne-brd" : "TouchTwo-brd");
		TrackEntry te = ((SkeletonAnimation)bird.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, birdAnimName, false);
		yield return new WaitForSeconds (te.animation.duration);
		((SkeletonAnimation)bird.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Idle-brd", true);
	}

	IEnumerator fireFrogTouchAnim(){
		string frogAnimName = calcAnimName( "frg", ++totalFrogClicks );
		TrackEntry te = ((SkeletonAnimation)frog.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, frogAnimName, false);
		yield return new WaitForSeconds (te.animation.duration);
		((SkeletonAnimation)frog.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Idle-frg", true);
	}

	IEnumerator fireMonkeyTouchAnim(){
		string monkeyAnimName = calcAnimName( "monk", ++totalMonkeyClicks );
		TrackEntry te = ((SkeletonAnimation)monkey.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, monkeyAnimName, false);
		yield return new WaitForSeconds (te.animation.duration);
		((SkeletonAnimation)monkey.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Idle-monk", true);
	}

	IEnumerator fireCatTouchAnim(){
		string catAnimName = calcAnimName( "cat", ++totalCatClicks );
		TrackEntry te = ((SkeletonAnimation)cat.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, catAnimName, false);
		yield return new WaitForSeconds (te.animation.duration);
		((SkeletonAnimation)cat.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Idle-cat", true);
	}

	IEnumerator fireFoxTouchAnim(){
		string foxAnimName = calcAnimName( "fox", ++totalFoxClicks );
		TrackEntry te = ((SkeletonAnimation)fox.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, foxAnimName, false);
		yield return new WaitForSeconds (te.animation.duration);
		((SkeletonAnimation)fox.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Idle-fox", true);
	}

	IEnumerator delayedTouchOff(){
		yield return new WaitForSeconds (2);
		isAnimatingTouch = false;
	}

	string calcAnimName( string animalStr, int totalClicks )
	{
		string indexStr = "One";
		if (totalClicks % 3 == 1) {
			indexStr = "Two";
		}
		else if (totalClicks % 3 == 2) {
			indexStr = "Three";
		}

		return "Touch" + indexStr + "-" + animalStr;
	}

	bool fireLeftArrows;
	IEnumerator fireRandomArrowAnims()
	{
		while (true) {
			yield return new WaitForSeconds(8);
			fireLeftArrows = !fireLeftArrows;
			GameObject[] arrowArr = fireLeftArrows ? leftArrows : rightArrows;

			for( int i = 0; i < arrowArr.Length; i++ ){
				GameObject arrow = arrowArr[i];
				string animName = "Sway_Right_Nav_Sign";
				if( i > 3 || (i == 3 && !fireLeftArrows)){
					animName = "Idle_Right_Water_Nav_Sign";
				}
//				print ("" + i + ": " + animName);
				((SkeletonAnimation)arrow.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, animName, true);
			}
		}
	}

	IEnumerator clickGoSign()
	{
		SoundManager.PlaySFX ("GoSign_Click");
		isPlaying = true;
		((SkeletonAnimation)goSign.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Click", false);
		((SkeletonAnimation)goSign.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Sway", true, 0);
		yield return new WaitForSeconds (0.5f);
		StartCoroutine(navigateToPage(1));

		StartCoroutine (loopRandomLightpostAnims());
		StartCoroutine (loopRandomAnims());
	}

	AudioSource snoreAS = null;
	void toggleSnore( bool toggleOn ){
		if (snoreAS != null) {
			snoreAS.Stop ();
			if( music != null ){
//				music.volume = 100;
			}
			}

		if (toggleOn) {
			if( music != null )
			{
//			music.volume = 0.2f;
			}
			snoreAS = SoundManager.PlaySFX("OLDSnoring", true, 0, 200, Random.Range(1.0f, 2.5f));

			                              }
	}

	IEnumerator navigateToPage(int pageIndex, bool pressedNavButton = false, bool isLeftNav = false, bool isInstantaneous = false){

//		print ("navigateToPage: pageIndex: " + pageIndex);

//		toggleSnore (pageIndex >= 2);

//		if (swaySound != null) {
//			float volume = (pageIndex == 0) ? 100 : 0;
//						swaySound.volume = volume;
//				}

		if (popupAS != null)
			popupAS.volume = (currentPage == 0) ? 100 : 0;

		if (pressedNavButton) {
			GameObject[] arrowArr = isLeftNav ? leftArrows : rightArrows;

			for (int i = 0; i < arrowArr.Length; i++) {
					GameObject arrow = arrowArr [i];
					string setAnimName = "Click_Right_Nav_Sign";
					string addAnimName = "Sway_Right_Nav_Sign";
					if (i > 3 || (i == 3 && !isLeftNav)) {
						setAnimName = "Click_Right_Water_Nav_Sign";
						addAnimName = "Idle_Right_Water_Nav_Sign";
					}

				((SkeletonAnimation)arrow.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, setAnimName, false);
				((SkeletonAnimation)arrow.GetComponent<SkeletonAnimation> ()).state.AddAnimation( 0, addAnimName, true, 0 );
			}
		}

		yield return new WaitForSeconds ( pressedNavButton ? 0.3f : 0 );

		Vector3 position = levelHolder.transform.position;
		position.x = -pageIndex * kPageLength;
//		string oncompleteCallback = ((pageIndex == 0) ? "startHappyvilleSignedAnimsHandle" : "hideCharacters");
		float time = isInstantaneous ? 0 : 0.5f * Mathf.Abs (pageIndex - currentPage);
		iTween.MoveTo (levelHolder, iTween.Hash ("time", time, "position", position));//,
//		                                        "oncompletetarget", gameObject, "oncomplete", oncompleteCallback));
		currentPage = pageIndex;
	}
}
