using UnityEngine;
using System.Collections;
using Spine;

public class SlingshotGameManager : FrenziableGame {

	public SlingshotFingerFollower fingerFollower;
	public GameObject pointsPrefab;
	public GameObject[] frenzyGroups;
	public Transform frenzyGroupParent;

	public SingleSoundBase music;
	void Start () {
		StartCoroutine (openDoors ());
		StartCoroutine (makeBearPoint ());
		music.playSingleSound ("Menu_Ambient_Background_Loop", true);
	}

	bool isBearWaiting = true;
	IEnumerator makeBearPoint()
	{
		while (isBearWaiting) {
			yield return new WaitForSeconds(3.667f);
			bearAnim.state.SetAnimation(0, "Point", false);
			bearAnim.state.AddAnimation(0, "Idle", true, 0);
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		DoorManager.openDoors ();
	}

	public SkeletonAnimation bearAnim;
	public GameObject backgroundNode;
	public GameObject boat;

	public TutorialManager tutorial;

	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick ();
			if( pickedGO )
			switch (pickedGO.tag) {
			case "StartBtn":
				SoundManager.PlaySFX ("StartButton_Tap");
				StartCoroutine (delayedTutorial ());
				TrackEntry te = pickedGO.GetComponent<SkeletonAnimation> ().state.SetAnimation (0, "Tap", false);
				StartCoroutine (delayedButtonMove (te.animation.duration, pickedGO));
				isBearWaiting = false;
				iTween.MoveTo( backgroundNode, iTween.Hash("x", -768, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
				iTween.MoveTo( boat, iTween.Hash("x", 0, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
				StartCoroutine(delayedGameStartSignal(2));
				fingerFollower.startTargetsMoving();
				startTimer();
				music.playSingleSound ("SlingGame_Music", true);
				break;

			case "ReplayBtn":

				break;
			}
		}
	}

	IEnumerator panToFrenzy(){
		iTween.MoveTo( backgroundNode, iTween.Hash("x", -2304, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
		timerAndMeter.pausePieChart ();
		timerAndMeter.moveUp ();
		GameObject frenzyGroup = Instantiate (frenzyGroups [Random.Range (0, 2)], frenzyGroupParent.position, Quaternion.identity) as GameObject;
		frenzyGroup.transform.parent = frenzyGroupParent.transform;
		music.playSingleSound ("SlingGame_FrenzyMusic", true);
		yield return new WaitForSeconds(20);
		panToNormalMode ();
		yield return new WaitForSeconds (2);
		Destroy (frenzyGroup);
	}

	void panToNormalMode(){
		music.playSingleSound ("SlingGame_Music", true);
		iTween.MoveTo( backgroundNode, iTween.Hash("x", -768, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
		timerAndMeter.zerototalDots ();
		timerAndMeter.unpausePieChart ();
		timerAndMeter.dropDown ();
	}

	IEnumerator delayedTutorial(){
		yield return new WaitForSeconds (2);
		print ("delayedTutorial");
		tutorial.doTutorial("Sling_Tutorial");
	}

	public void hitFish( Vector3 location ){
		timerAndMeter.incrementScore (10, false);
		StartCoroutine( triggerScoreAtPosition(10, location));
	}
	
	public void hitTarget( Vector3 location  ){
		timerAndMeter.incrementScore (5, false);
		StartCoroutine( triggerScoreAtPosition(5, location));
	}
	
	public void hitAnimal( Vector3 location  ){
		timerAndMeter.incrementScore (10, true);
		StartCoroutine( triggerScoreAtPosition(10, location));
	}

	IEnumerator triggerScoreAtPosition( int score, Vector3 position ){
		position.z -= 0.2f;
		position.y += 100;
		GameObject numbers = Instantiate (pointsPrefab, position, Quaternion.identity) as GameObject;
		string skinName = (score == 5 ? "Five" : "Ten" );
		numbers.GetComponent<SkeletonAnimation> ().skeleton.SetSkin (skinName);
		yield return new WaitForSeconds (5);
		Destroy (numbers);
	}

	IEnumerator delayedGameStartSignal( float delay ){
		yield return new WaitForSeconds (delay);
		SlingshotFingerFollower.hasGameStarted = true;
	}

	IEnumerator delayedButtonMove( float delay, GameObject startButton ){
		yield return new WaitForSeconds(delay);
		startButton.transform.Translate (Vector3.left * 1000);
	}

	public override void startFrenzy(){
		fingerFollower.startFrenzy ();
		StartCoroutine(panToFrenzy ());
	}

	bool areResultsInitiated = false;
	public ScoreboardManager catchResults;
	public override void timerComplete ()
	{
		if (!areResultsInitiated) {
			areResultsInitiated = true;
			fingerFollower.initiateResults ();
			catchResults.showResults (10, 10, 85);
			music.playSingleSound ("Scoreboard_Music");

			iTween.MoveTo( backgroundNode, iTween.Hash("x", 0, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
			iTween.MoveTo( boat, iTween.Hash("x", 768, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
			iTween.MoveTo( bearAnim.gameObject, iTween.Hash("y", -525, "time", 0.1f, "easetype", iTween.EaseType.easeInOutQuad));
		}
	}
}
