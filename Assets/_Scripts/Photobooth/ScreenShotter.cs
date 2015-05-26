using UnityEngine;
using System.Collections;

public class ScreenShotter : MonoBehaviour {

	public Transform origin;
	public Transform oppositeCorner;

	public Renderer picTester;

	public void doScreenShot(){
		StartCoroutine (initiateScreenShot ());
	}

	IEnumerator initiateScreenShot(){
		Vector3 worldOrigin = Camera.main.WorldToScreenPoint (origin.position);
		Vector3 worldOppCorner = Camera.main.WorldToScreenPoint (oppositeCorner.position);

		int width = (int)Mathf.Abs(worldOppCorner.x - worldOrigin.x);
		int height = (int)Mathf.Abs(worldOppCorner.y - worldOrigin.y);

		yield return new WaitForEndOfFrame ();
		Texture2D tex = new Texture2D(width, height);
		tex.ReadPixels(new Rect(worldOrigin.x,worldOrigin.y,width,height),0,0);
		tex.Apply();
		
		picTester.material.SetTexture ("_MainTex", tex);
		picTester.enabled = true;
	}
}
