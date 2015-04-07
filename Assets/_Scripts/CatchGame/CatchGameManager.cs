using UnityEngine;
using System.Collections;
using Spine;

public class CatchGameManager : FrenziableGame {

	public SkeletonAnimation startBtn;
	public SkeletonAnimation lion;
	public FoodDropper foodDropper;
	public TimerAndMeter timerAndMeter;
	public GameObject basketCollision;
	public FruitKiller fruitKiller;

	public GameObject background;
	public GameObject clouds;

	public CatchResults catchResults;

	public enum CatchGameMode{
		WaitForStart, NormalGameplay, Frenzy, Results
	};
	
	public CatchGameMode gameMode = CatchGameMode.WaitForStart;
	// Use this for initialization
	void Start () {
		StartCoroutine (delayedLionPoint(10));
		StartCoroutine (openDoors ());
	}

	public DoorManager doorManager;
	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		if( doorManager ){
			StartCoroutine(doorManager.openDoors());
		}
	}

	public override void timerComplete ()
	{
		if( gameMode != CatchGameMode.Results )
			initiateResults ();
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	//---------- ---------- ---------- ---------- ---------- WAIT FOR START ----------
	IEnumerator delayedLionPoint( float delay){
		yield return new WaitForSeconds( 3 );
		while (gameMode == CatchGameMode.WaitForStart) {
			makeLionPoint();
			yield return new WaitForSeconds( 8 );
		}
	}

	void makeLionPoint(){
		lion.state.SetAnimation (0, "Point", false);
		lion.state.AddAnimation (0, "Wait", true, 0);
	}

	AudioSource music = null;
	void waitForStart_mouseButtonDown(){
		GameObject touchedGO = mousePick ();

		if ( touchedGO && touchedGO.tag == "CatchBtn") {
			gameMode = CatchGameMode.NormalGameplay;
			TrackEntry te = startBtn.state.SetAnimation (0, "Tap", false);
			StartCoroutine (delayedButtonMove (te.animation.duration));
			te = lion.state.SetAnimation (0, "Start", false);
			lion.state.AddAnimation (0, "Walk", true, 0);
			StartCoroutine (delayedGameStart (te.animation.duration));
		}
		if (touchedGO && touchedGO.tag == "CatchLion") {
			makeLionPoint();
		}
	}

	IEnumerator delayedButtonMove( float delay )
	{
		yield return new WaitForSeconds (delay);
		startBtn.transform.Translate (10000, 0, 0);
	}

	//---------- ---------- ---------- ---------- --------- NORMAL GAMEPLAY ----------

	IEnumerator returnToNormalMode(){
		if (gameMode != CatchGameMode.Results) {
			yield return new WaitForSeconds (15);
			timerAndMeter.unpausePieChart();
			fruitKiller.currentPitch = 1;
			timerAndMeter.zerototalDots();
			foodDropper.startFruitDrops ();
			StartCoroutine (animateReturnToNormal ());
			gameMode = CatchGameMode.NormalGameplay;
		}
	}

	IEnumerator animateReturnToNormal(){
		SoundManager.PlaySFX("CatchSuperDone", false, 0);
		float duration = 0;
		canLionAnimate = false;
		duration = lion.state.SetAnimation (0, "Change_From_Super", false).animation.duration;
		animateBackground (false, duration-0.5f);
		timerAndMeter.dropDown ();
		fruitKiller.explodeAllLiveFoodAway ();
		yield return new WaitForSeconds (duration);
		lion.skeleton.SetSkin ("Lion");
		basketCollision.transform.localPosition = Vector3.zero;
		duration = lion.state.AddAnimation (0, "Change_To_Lion", false, 0).animation.duration;
		lion.state.AddAnimation (0, "Walk", true, 0);
		yield return new WaitForSeconds (duration);
		canLionAnimate = true;
	}

	public void playCatch()
	{
		if (!canLionAnimate) return;
		string catchStr = gameMode == CatchGameMode.NormalGameplay ? "Catch" : "Catch_Fly";
		string walkStr =  gameMode == CatchGameMode.NormalGameplay ? "Walk" : "Fly";

		lion.state.SetAnimation (0, catchStr, false);
		lion.state.AddAnimation (0, walkStr, true, 0);
	}

	IEnumerator delayedGameStart( float delay )
	{
		yield return new WaitForSeconds (delay);
		music = SoundManager.PlaySFX("FastMusic", true);
		timerAndMeter.dropDown ();
		foodDropper.startFruitDrops ();
		StartCoroutine (timerAndMeter.delayedPieChartStart (1));
//		StartCoroutine (delayedInitiateResults (60));
	}

	bool isLionTouched = false;
	float lastTouchX = 0;
	void normalGameplay_mouseButtonDown(){
		frenzyAndGameplay_mouseButtonDown ();
	}

	void normalGameplay_mouseButtonMoved(){
		frenzyAndGameplay_mouseButtonMoved ();
	}

	void normalGameplay_mouseButtonUp(){
		frenzyAndGamePlay_mouseButtonUp ();
	}

	//---------- ---------- ---------- ---------- ---------- -- FRENZY MODE ----------
	public GameObject vertCloudPrefab;

	public override void startFrenzy(){
		gameMode = CatchGameMode.Frenzy;
		StartCoroutine (animateFrenzyStart ());
		foodDropper.startFrenzyMode ();
		StartCoroutine (returnToNormalMode ());
		timerAndMeter.pausePieChart ();
	}

	bool canLionAnimate = true;
	IEnumerator animateFrenzyStart(){
		SoundManager.PlaySFX("CatchBecomeSuperman", false, 0);
		float duration = 0;
		canLionAnimate = false;
		duration = lion.state.SetAnimation (0, "Change_From_Lion", false).animation.duration;
		yield return new WaitForSeconds (duration);
		basketCollision.transform.localPosition = new Vector3 (65, 65, 0);
		duration = lion.state.AddAnimation (0, "Change_To_Super", false, 0).animation.duration;
		lion.skeleton.SetSkin ("Lion_Super");
		yield return new WaitForSeconds (0.2f);
		animateBackground (true, duration);
		timerAndMeter.moveUp ();
		fruitKiller.explodeAllLiveFoodAway ();
		lion.state.AddAnimation (0, "Fly", true, 0);
		yield return new WaitForSeconds (duration);
		StartCoroutine (dropShrinkClouds ());
		canLionAnimate = true;
	}

	IEnumerator dropShrinkClouds()
	{
		for (int i = 0; i < 24; ++i) {
			float randomX = Random.Range (-270, 270);
			Vector3 pos = new Vector3( randomX, 660, 8 );
			GameObject cloud = Instantiate( vertCloudPrefab, pos, Quaternion.identity ) as GameObject;
			pos.y = -650;
			iTween.MoveTo( cloud, iTween.Hash( "position", pos, "time", 5, "easetype", iTween.EaseType.easeOutExpo));
			iTween.ScaleBy( cloud, iTween.Hash( "amount", Vector3.one*0.25f, "time", 5, "easetype", iTween.EaseType.easeOutExpo));
			yield return new WaitForSeconds( Random.Range ( 0.4f, 0.6f ));
			StartCoroutine( delayedDestroy(cloud, 5.5f ));
		}
	}

	IEnumerator delayedDestroy( GameObject go, float delay)
	{
		yield return new WaitForSeconds (delay);
		Destroy (go);
	}

	void animateBackground( bool moveDown, float duration ){
		Vector3 position = moveDown ? new Vector3( 0, -500, 10 ) : new Vector3( 0, 0, 10 );
		iTween.MoveTo (background, iTween.Hash( "position", position, "time", duration*2 , "islocal", true));

		Vector3 cloudPosition = moveDown ? new Vector3 (0, -1440, 8) : new Vector3 (0, -144, 8);
		iTween.MoveTo (clouds, iTween.Hash( "position", cloudPosition, "time", duration*2 , "islocal", true));
	}

	void frenzyMode_mouseButtonDown(){
		frenzyAndGameplay_mouseButtonDown ();
	}

	float lastDelta = 0;
	void frenzyMode_mouseButtonMoved(){
		float delta = frenzyAndGameplay_mouseButtonMoved ();

		if (delta != lastDelta && canLionAnimate) {
			string animName = "Fly_Swipe_" + (delta < 0 ? "Left" : "Right");
			lion.state.SetAnimation (0, animName, false);
			lion.state.AddAnimation (0, "Fly", true, 0);
			lastDelta = delta;
		}
	}
	
	void frenzyMode_mouseButtonUp(){
		frenzyAndGamePlay_mouseButtonUp ();
	}

	public static int totalFruits = 0;
	public static int totalCandies = 0;
	//---------- ---------- ---------- ---------- ---------- ------ RESULTS ----------
	IEnumerator delayedInitiateResults( float delay ){
		yield return new WaitForSeconds(60);
		initiateResults ();
	}

	void initiateResults(){
		gameMode = CatchGameMode.Results;
		if( music != null )
			music.Stop ();
		SoundManager.PlaySFX("EndWin", false, 0);
		foodDropper.dropperMode = FoodDropper.DropperMode.Inactive;
		fruitKiller.explodeAllLiveFoodAway ();
		catchResults.showResults ( totalFruits, totalCandies, timerAndMeter.getScore());
		timerAndMeter.moveUp ();
		canLionAnimate = false;
		fireLionEnd ();
	}

	void fireLionEnd( )
	{
		lion.state.SetAnimation (0, "End", false);
		lion.state.AddAnimation (0, "Wait", true, 0);
	}

	void results_mouseButtonDown(){

	}

	//---------- ---------- ---------- ---------- ---------- ---- STRUCTURE ----------
	void frenzyAndGameplay_mouseButtonDown(){
		GameObject touchedGO = mousePick ();
		if (touchedGO && touchedGO.tag == "CatchLion") {
			isLionTouched = true;
			lastTouchX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
		}
	}

	float frenzyAndGameplay_mouseButtonMoved(){
		if (isLionTouched && canLionAnimate ) {
			float currentX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
			float delta = currentX - lastTouchX;
			lastTouchX = currentX;
			Vector3 pos = lion.transform.localPosition;
			pos.x += delta;
			lion.transform.localPosition = pos;
			bookendLionPosition();
			return (delta < 0 ? -1 : 1);
		} else {
			GameObject touchedGO = mousePick ();
			if (touchedGO && touchedGO.tag == "CatchLion"){
				isLionTouched = true;
				lastTouchX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
			}
			return 0;
		}
	}

	void frenzyAndGamePlay_mouseButtonUp(){
		isLionTouched = false;
	}

	void bookendLionPosition(){
		Vector3 pos = lion.transform.localPosition;
		if( pos.x < -Screen.width/2 )
			pos.x = -Screen.width/2;
		if( pos.x > Screen.width/2 )
			pos.x = Screen.width/2;
		
		lion.transform.localPosition = pos;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			switch( gameMode ){
			case CatchGameMode.WaitForStart:
				waitForStart_mouseButtonDown();
				break;
			case CatchGameMode.NormalGameplay:
				normalGameplay_mouseButtonDown();
				break;
			case CatchGameMode.Frenzy:
				frenzyMode_mouseButtonDown();
				break;
			case CatchGameMode.Results:
				results_mouseButtonDown();
				break;
			default:
				break;
			}
		}
		
		if (Input.GetMouseButton (0)) {
			switch( gameMode ){
			case CatchGameMode.NormalGameplay:
				normalGameplay_mouseButtonMoved();
				break;
			case CatchGameMode.Frenzy:
				frenzyMode_mouseButtonMoved();
				break;
			default:
				break;
			}
		}
		
		if (Input.GetMouseButtonUp (0)) {
			switch( gameMode ){
			case CatchGameMode.NormalGameplay:
				normalGameplay_mouseButtonUp();
				break;
			case CatchGameMode.Frenzy:
				frenzyMode_mouseButtonUp();
				break;
			default:
				break;
			}
		}
	}
}
