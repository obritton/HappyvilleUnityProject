using UnityEngine;
using System.Collections;

public class ForParentsPageManager : MonoBehaviour {

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	void OnGUI(){
		if (GUI.Button (new Rect (10, 10, Screen.width / 4, Screen.height / 6), "Erase All \n Progress")) {
			PlayerPrefs.DeleteAll();
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			GameObject pickedGO = mousePick();
			if( pickedGO != null ){
				switch( pickedGO.tag ){
				case "ExitBtn":
					gameObject.SetActive(false);
					break;
				case "FacebookBtn":
					openURL( "http://facebook.com" );
					break;
				case "TwitterBtn":
					openURL( "http://twitter.com" );
					break;
				case "HelpBtn":
					openURL( "http://help.com" );
					break;
				case "EmailBtn":
					openURL( "mailto:jasonledet@yahoo.com" );
					break;
				case "GiftBtn":
					openURL( "https://itunes.apple.com/us/app/sushi-monster/id512651258?mt=8" );
					break;
				}
			}
		}
	}

	void openURL( string url ){
		Application.OpenURL (url);
	}
}
