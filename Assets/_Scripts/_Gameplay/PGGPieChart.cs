using UnityEngine;
using System.Collections;

public class PGGPieChart : MonoBehaviour {

	public float currentChartAmount = 100;
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
			if( currentChartAmount > 50 ){
//				Quaternion rot = leftFillPivot.rotation;
//				rot.z = 180 * currentChartAmount/50;
//				
//				leftFillPivot.rotation = rot;
			}
			else
			{

			}
		}
	}
}
