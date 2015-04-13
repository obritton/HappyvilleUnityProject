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
		yield return new WaitForSeconds (0);
		doorAnim.state.SetAnimation (0, "Close", false);
		doorAnim.GetComponent<Renderer>().enabled = true;
	}

	public IEnumerator animateOpenDoors( bool immediate = false )
	{
		if (immediate) {
			doorAnim.GetComponent<Renderer> ().enabled = false;
		} else {
				SoundManager.PlaySFX ("DoorOpen", false, 0);
		}
		TrackEntry te = doorAnim.state.SetAnimation (0, "Open", false);
		yield return new WaitForSeconds (te.animation.duration);
		doorAnim.GetComponent<Renderer>().enabled = false;
	}
}
