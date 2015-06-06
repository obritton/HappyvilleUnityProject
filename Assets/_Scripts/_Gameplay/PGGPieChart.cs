using UnityEngine;
using System.Collections;

public class PGGPieChart : MonoBehaviour {

	public float currentChartAmount = 100;
	public bool isActive = false;
	public float multiplier = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive)
			currentChartAmount -= Time.deltaTime * multiplier;
	}
}
