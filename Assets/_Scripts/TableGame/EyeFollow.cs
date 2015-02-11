using UnityEngine;
using System.Collections;

public class EyeFollow : MonoBehaviour {
	
	ArrayList eyes;
	float constrainDist = 0.09f;
	public Transform followTransform;

	public static void registerFollowTransform( Transform newFollowTransform ){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.regFollowTransform( newFollowTransform );
		}
	}

	public static void unregisterFollowTransform(){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.unregFollowTransform();
		}
	}

	public static void registerEyeball( Transform newEyeBall ){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.regEyeball( newEyeBall );
		}
	}

	public static void unregisterEyeBall( Transform newEyeBall ){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.unregEyeball( newEyeBall );
		}
	}

	public static EyeFollow getEyeFollow(){
		GameObject eyeballManager = GameObject.Find ("NewFollowTransform");
		if( eyeballManager ){
			EyeFollow eyeFollow = (EyeFollow)eyeballManager.GetComponent<EyeFollow> ();
			return eyeFollow;
		}
		return null;
	}

	public void regFollowTransform( Transform newFollowTransform ){
		followTransform = newFollowTransform;
		setEyesToOverride ();
	}

	public void unregFollowTransform(){
		setEyesToOverride (false);
		followTransform = null;
	}

	void setEyesToOverride( bool isOverride = true ){
		SkeletonUtilityBone.Mode mode = isOverride ? SkeletonUtilityBone.Mode.Override : SkeletonUtilityBone.Mode.Follow;
		foreach (Transform eye in eyes) {
			SkeletonUtilityBone utilBone = (SkeletonUtilityBone)eye.GetComponent<SkeletonUtilityBone>();
			if( utilBone ){
				utilBone.mode = mode;
			}
		}
	}

	public void regEyeball( Transform newEyeBall ){
		if (!eyes.Contains (newEyeBall)) {
			eyes.Add (newEyeBall);
		}
	}

	public void unregEyeball( Transform eyeBall ){
		if (eyes.Contains (eyeBall)) {
			eyes.Remove(eyeBall);
		}
	}

	void Start(){
		eyes = new ArrayList ();
	}

	void Update () {
		if (followTransform) {
			foreach (Transform eye in eyes) {
				Vector3 angle = angleFromTransform (eye, followTransform);
				eye.localPosition = angle;
			}
		}
	}

	Vector3 angleFromTransform( Transform eye, Transform dragged )
	{
		Vector3 direction = dragged.position - eye.position;
		direction.Normalize ();
		direction *= constrainDist;
		float x = direction.x;
		direction.x = direction.y;
		direction.y = -x;
		return direction;
	}
}

/*Using Offsets 
 * using UnityEngine;
using System.Collections;

public class EyeFollow : MonoBehaviour {
	
	ArrayList eyes;
	float constrainDist = 0.09f;
	public Transform followTransform;

	public static void registerFollowTransform( Transform newFollowTransform ){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.regFollowTransform( newFollowTransform );
		}
	}

	public static void unregisterFollowTransform(){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.unregFollowTransform();
		}
	}

	public static void registerEyeball( Transform newEyeBall ){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.regEyeball( newEyeBall );
		}
	}

	public static void unregisterEyeBall( Transform newEyeBall ){
		EyeFollow eyeFollow = getEyeFollow ();
		if (eyeFollow) {
			eyeFollow.unregEyeball( newEyeBall );
		}
	}

	public static EyeFollow getEyeFollow(){
		GameObject eyeballManager = GameObject.Find ("NewFollowTransform");
		if( eyeballManager ){
			EyeFollow eyeFollow = (EyeFollow)eyeballManager.GetComponent<EyeFollow> ();
			return eyeFollow;
		}
		return null;
	}

	public void regFollowTransform( Transform newFollowTransform ){
		followTransform = newFollowTransform;
		setEyesToOverride ();
	}

	public void unregFollowTransform(){
		setEyesToOverride (false);
		followTransform = null;
	}

	void setEyesToOverride( bool isOverride = true ){
		SkeletonUtilityBone.Mode mode = isOverride ? SkeletonUtilityBone.Mode.Override : SkeletonUtilityBone.Mode.Follow;
		foreach (Eye eye in eyes) {
			Transform eyeTans = eye.eye;

			SkeletonUtilityBone utilBone = (SkeletonUtilityBone)eyeTans.GetComponent<SkeletonUtilityBone>();
			if( utilBone ){
				utilBone.mode = mode;
			}
		}
	}

	public void regEyeball( Transform newEyeBall ){
//		if (!eyes.Contains (newEyeBall)) {
//			eyes.Add (newEyeBall);
//		}
		Eye eye = new Eye ();
		eye.eye = newEyeBall;
		eye.centerPos = newEyeBall.localPosition;
		eyes.Add (eye);
	}

	public void unregEyeball( Transform eyeBall ){

		Eye eyeToRemove = null;

		foreach (Eye eye in eyes) {
			if( eye.eye == eyeBall ){
				eyeToRemove = eye;
				break;
			}
		}

		if (eyeToRemove != null) {
			eyes.Remove (eyeToRemove);
		}
	}

	void Start(){
		eyes = new ArrayList ();
	}

	void Update () {
		if (followTransform) {
			foreach (Eye eye in eyes) {
				Transform eyeTrans = eye.eye;
				Vector3 centerPos = eye.centerPos;
				Vector3 angle = angleFromTransform (centerPos, followTransform);
				eyeTrans.localPosition = centerPos + angle;
			}
		}
	}

	Vector3 angleFromTransform( Vector3 eyePosition, Transform dragged )
	{
		Vector3 direction = dragged.position - eyePosition;
		direction.Normalize ();
		direction *= constrainDist;
		float x = direction.x;
		direction.x = direction.y;
		direction.y = -x;
		return direction;
	}
}

*/