using UnityEngine;
using System.Collections;
using Spine;

public class TGResTestManager : MonoBehaviour {

	public SkeletonAnimation[] Lions;
	public GUIStyle invisStyle;

	public string[] animNames;
	int currentAnimIndex = 0;

	void OnGUI(){
		if (GUI.Button (new Rect (0, 0, Screen.width / 10, Screen.height / 10), "Back")) {
			Application.LoadLevel("ResTestMenu");
		}

		if( GUI.Button( new Rect(0, 0, Screen.width, Screen.height), "", invisStyle )){
			currentAnimIndex = (currentAnimIndex + 1) % animNames.Length;
			string animName = animNames[currentAnimIndex];
			foreach( SkeletonAnimation anim in Lions ){
				anim.state.SetAnimation( 0, animName, true );
			}
		}
	}
}
