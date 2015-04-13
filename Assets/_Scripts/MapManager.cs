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

	void Start(){
		if (firstTime) {
			DoorManager.immediateOpen();
			firstTime = false;
		}

		if (openPageIndex > 0) {
			startMusic();
			print ("openPageIndex: " + openPageIndex);
			DoorManager.openDoors();
		}

		startHappyvilleSignedAnimsHandle ();

		if (openPageIndex > 0) {
			StartCoroutine( navigateToPage(openPageIndex, false, false, true ));
		}
	}

	void startHappyvilleSignedAnimsHandle(){
		StartCoroutine (startPopupAnims ());
		StartCoroutine (fireRandomArrowAnims ());
	}

	public static int openPageIndex = 0;
	IEnumerator startPopupAnims(){
		yield return new WaitForSeconds (3);
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
		StartCoroutine( makeCharactersCheer ());
	}

	AudioSource music = null;
	IEnumerator makeCharactersCheer(){
		if( currentPage == 0 )
			SoundManager.PlaySFX ("GameStartCheer");
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
		startMusic ();
		((SkeletonAnimation)goSign.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Sway", true);
		StartCoroutine (fireRandomTouchRoutines ());
	}

	void startMusic()
	{
		if( music == null )
			music = SoundManager.PlaySFX ("HappyMusic", true, 0, 100);
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

	int currentPage = 0;
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
					}
					break;
				case "NavRightBtn":
					if(currentPage > 0 && currentPage < 6){
						StartCoroutine(navigateToPage(currentPage + 1, true, false));
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
					Table.level = buttonIndex;
					GameObject button = tableGameButtons[buttonIndex];
					((SkeletonAnimation)button.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Active_press", false);
					StartCoroutine(loadTableGame("Table Game", currentPage, true));
					break;
				case "CatchGameBtn":
					StartCoroutine(loadTableGame("CatchGame", 1));
					break;
				case "WhackGame":
					StartCoroutine(loadTableGame("Whack Game", 3));
					break;
				case "SlingshotGameBtn":
					StartCoroutine(loadTableGame("Slingshot Game", 5));
					break;
				case "PuzzleGame":
					StartCoroutine(loadTableGame("PuzzleGame", 2));
					break;
				case "MatchingGame":
					StartCoroutine(loadTableGame("MatchingGame", 6));
					break;
				case "PhotoBooth":
					StartCoroutine(loadTableGame("PhotoBooth", 4));
					break;
				default:
					break;
				}
			}
		}
	}


	IEnumerator loadTableGame( string gameName, int pageIndex, bool isTableButton = false){
		if (!isTableButton) {
			miniGameButtonsArr[pageIndex-1].state.SetAnimation(0,"Active_press",false);
		}
		openPageIndex = pageIndex;
		iTween.Stop ();
		yield return new WaitForSeconds (1);
		SoundManager.Stop();
		DoorManager.closeDoors ();

		yield return new WaitForSeconds (1);
		iTween.Stop ();
		print ("openPageIndex: " + openPageIndex);
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
		((SkeletonAnimation)goSign.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Click", false);
		((SkeletonAnimation)goSign.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "Sway", true, 0);
		yield return new WaitForSeconds (0.5f);
		StartCoroutine(navigateToPage(1));
	}

	AudioSource snoreAS = null;
	void toggleSnore( bool toggleOn ){
		if (snoreAS != null) {
			snoreAS.Stop ();
			if( music != null )
				music.volume = 100;
			}

		if (toggleOn) {
			if( music != null )
			music.volume = 0.2f;
			snoreAS = SoundManager.PlaySFX("Snoring", true, 0, 200, Random.Range(1.0f, 2.5f));

			                              }
	}

	IEnumerator navigateToPage(int pageIndex, bool pressedNavButton = false, bool isLeftNav = false, bool isInstantaneous = false){

		toggleSnore (pageIndex >= 2);

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
