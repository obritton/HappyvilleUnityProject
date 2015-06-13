using UnityEngine;
using System.Collections;

public class PGGPieChart : MonoBehaviour {

	public float currentChartAmount = 360;
	public bool isActive = false;
	public float multiplier = 1;

	public Transform leftBackPivot;
	public Transform rightBackPivot;
	public Transform leftFillPivot;
	public Transform rightFillPivot;

	// Use this for initialization
	void Start () {
//		StartCoroutine (testCountDown ());
	}

	IEnumerator testCountDown(){
		iTween.RotateBy (leftFillPivot.gameObject, iTween.Hash ("time", 7, "z", 0.5f, "easetype", iTween.EaseType.linear));
		yield return new WaitForSeconds (7);
		leftFillPivot.Translate (0, 0, 2);
		leftBackPivot.Translate (0, 0, -3);
		iTween.RotateBy (rightFillPivot.gameObject, iTween.Hash ("time", 7, "z", 0.5f, "easetype", iTween.EaseType.linear));
	}

	// Update is called once per frame
	void Update () {
		if (isActive)
		{
			currentChartAmount -= Time.deltaTime * multiplier;
//			if( currentChartAmount > 180 ){
//				Quaternion rot = leftFillPivot.rotation;
//				rot.z = currentChartAmount/10.0f;
//				print ("LEFT currentChartAmount: " + currentChartAmount +  ", rot.z: " + rot.z);
//				leftFillPivot.rotation = rot;
//			}
//			else
//			{
//				if( leftFillPivot != null )
//					Destroy( leftFillPivot.gameObject );
//				Quaternion rot = rightFillPivot.rotation;
//				rot.z = 180.0f * (100.0f-currentChartAmount)/10000.0f;
//				print ("RIGHT currentChartAmount: " + currentChartAmount +  ", rot.z: " + rot.z);
//				rightFillPivot.rotation = rot;
//			}
		}
	}
}
