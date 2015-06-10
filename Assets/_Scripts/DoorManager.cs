using UnityEngine;
using Spine;
using System.Collections;

public class DoorManager : MonoBehaviour {

	public SkeletonAnimation doorAnim;

	public static void openDoors(float delay = 0){
		changeDoors (true);
	}

	public static void closeDoors(float delay = 0){
		changeDoors (false);
	}

	public static void immediateOpen(){
		changeDoors (true, true);
	}

	public static void changeDoors( bool openning, bool immediate = false ){
		GameObject doors = GameObject.Find ("Doors");
		if( doors ){
			DoorManager doorManager = doors.GetComponent<DoorManager>();
			if( doorManager ){
				if( openning ){
					doorManager.initiateOpenDoors( immediate );
				}
				else{
					doorManager.initiateCloseDoors();
				}
			}
		}
	}

	public void initiateCloseDoors(){
		StartCoroutine (animateCloseDoors ());
	}

	public void initiateOpenDoors( bool immediate = false ){
		StartCoroutine (animateOpenDoors (immediate));
	}

	public IEnumerator animateCloseDoors()
	{
		AudioSource aSource = SoundManager.PlaySFX ("Door_Close");

		int randomI = Random.Range (1, 10);
		string animName = "Close" + (randomI == 1 ? "" : "" + randomI);

		TrackEntry te = doorAnim.state.SetAnimation (0, animName, false);
		doorAnim.GetComponent<Renderer>().enabled = true;

		yield return new WaitForSeconds (te.animation.duration);
	}

	public IEnumerator animateOpenDoors( bool immediate = false )
	{
		if (immediate) {
			doorAnim.GetComponent<Renderer> ().enabled = false;
		} else {
			SoundManager.PlaySFX ("Door_Open");
		}
		TrackEntry te = doorAnim.state.SetAnimation (0, "Open", false);
		yield return new WaitForSeconds (te.animation.duration);
		doorAnim.GetComponent<Renderer>().enabled = false;
	}
}
