using UnityEngine;
using System.Collections;

public class TGConsts : MonoBehaviour {

	public float firstPopupDelay = 0;
	public float firstPopupsStaggar = 0.5f;
	public float thoughtPopupDelay = 1;
	public float thoughtShapeDelay = 1;

	public static float kFirstPopupDelay;
	public static float kFirstPopupsStaggar;
	public static float kThoughtPopupDelay;
	public static float kThoughtShapeDelay;

	void Start()
	{
		kFirstPopupDelay = firstPopupDelay;
		kFirstPopupsStaggar = firstPopupsStaggar;
		kThoughtPopupDelay = thoughtPopupDelay;
		kThoughtShapeDelay = thoughtShapeDelay;
	}
}
