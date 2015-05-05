using UnityEngine;
using System.Collections;

public class StickerTrayManager : MonoBehaviour {

	public SkeletonAnimation tray;

	public GameObject hairTray;
	public GameObject faceTray;
	public GameObject maskTray;
	public GameObject hatTray;
	GameObject currenTray;
	
	enum ButtonType{	Head, Face, Glasses, Hat	};

	void Start(){
		currenTray = hairTray;
	}

	int currentPosition = 0;
	// Update is called once per frame
	void Update () {
		if( Input.GetMouseButtonDown(0)){
			GameObject pickedGO = mousePick ();
			if( pickedGO != null ){
				ButtonType buttonType = ButtonType.Head;
				switch( pickedGO.tag ){
				case "HairBtn":
					activateTab(ButtonType.Head);
					break;
				case "FaceBtn":
					activateTab(ButtonType.Face);
					break;
				case "MaskBtn":
					activateTab(ButtonType.Glasses);
					break;
				case "HatBtn":
					activateTab(ButtonType.Hat);
					break;
				case "LeftArrow":
					pressedTrayArrow( true );
					break;
				case "RightArrow":
					pressedTrayArrow( false );
					break;
				}
			}
		}
	}

	void pressedTrayArrow( bool isLeft ){
		int direction = isLeft ? 1 : -1;
		currentPosition += direction;
		animateToNewPosition (currentPosition);
	}

	void animateToNewPosition( int pos ){
		iTween.MoveTo (currenTray.gameObject, iTween.Hash ("x", pos * 500, "time", 0.5f, "easetype", iTween.EaseType.easeOutElastic));
	}

	void activateTab( ButtonType buttonType ){
		string animationName = "";

		switch( buttonType ){
		case ButtonType.Face:
			animationName = "Tap_Face";
			break;
		case ButtonType.Head:
			animationName = "Tap_Head";
			break;
		case ButtonType.Hat:
			animationName = "Tap_Hat";
			break;
		case ButtonType.Glasses:
			animationName = "Tap_Glasses";
			break;
		}

		currentPosition = 0;
		animateToNewPosition (currentPosition);
		tray.state.SetAnimation(0, animationName, false);
		setActiveTray (buttonType);
	}

	void setActiveTray(ButtonType buttonType){
		Vector3 pos = new Vector3 (0, 148, 0);
		hairTray.transform.localPosition = pos;
		faceTray.transform.localPosition = pos;
		maskTray.transform.localPosition = pos;
		hatTray.transform.localPosition = pos;

		hairTray.gameObject.SetActive (false);
		faceTray.gameObject.SetActive (false);
		maskTray.gameObject.SetActive (false);
		hatTray.gameObject.SetActive (false);

		switch( buttonType ){
		case ButtonType.Face:
			currenTray = faceTray;
			faceTray.gameObject.SetActive (true);
			break;
		case ButtonType.Head:
			currenTray = hairTray;
			hairTray.gameObject.SetActive (true);
			break;
		case ButtonType.Hat:
			currenTray = hatTray;
			hatTray.gameObject.SetActive (true);
			break;
		case ButtonType.Glasses:
			currenTray = maskTray;
			maskTray.gameObject.SetActive (true);
			break;
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
