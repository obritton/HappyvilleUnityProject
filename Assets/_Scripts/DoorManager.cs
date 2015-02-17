using UnityEngine;
using System.Collections;

public class DoorManager : MonoBehaviour {

	public GameObject leftHinge;
	public GameObject rightHinge;
	
	public IEnumerator closeDoors( bool isImmediate = false )
	{
		yield return new WaitForSeconds (0.25f);
		iTween.RotateTo (leftHinge, Vector3.zero, isImmediate ? 0 : 1);
//		iTween.RotateTo (rightHinge, Vector3.zero, isImmediate ? 0 : 0.5f);
	}

	public IEnumerator openDoors( bool isImmediate = false )
	{
		yield return new WaitForSeconds (0);
		iTween.RotateTo (leftHinge, Vector3.up*90, isImmediate ? 0 : 1);
//		iTween.RotateTo (rightHinge, Vector3.up*-90, isImmediate ? 0 : 0.5f);
	}
}
