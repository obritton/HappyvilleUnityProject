using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

	public SkeletonAnimation tutorial;

	bool hasShowntutorial = false;
	public void doTutorial( string tutName){
		if (!hasShowntutorial) {
			print ("doTutorial");
			hasShowntutorial = true;
			tutorial.state.SetAnimation( 0, tutName, false );
			tutorial.GetComponent<Renderer>().enabled = true;
		}
	}
}
