﻿using UnityEngine;
using System.Collections;

public class PhotoboothManager : GameManagerBase {

	private WebCamTexture camTexture;

	public GameObject cameraBooth;
	public GameObject album;
	public GameObject gallery;

	public Renderer webcamRenderer;

	public SkeletonAnimation tray;
	float trayStartY = 0;
	
	void Start () {
		DoorManager.openDoors ();

		setupCameraAccess ();

		trayStartY = tray.transform.localPosition.y;
		tray.transform.Translate (Vector3.down*325);
	}

	void setupCameraAccess(){
		WebCamDevice[] devices = WebCamTexture.devices;
	
		string deviceStr = devices [0].name;
		foreach (WebCamDevice device in devices) {
			print ("" + device.name + " isFrontFacing: " + device.isFrontFacing);
			if( device.isFrontFacing )
				deviceStr = device.name;
		}
		print ("deviceStr " + deviceStr);

//		string deviceStr = devices.Length > 1 ? devices[1].name : devices[0].name;
		camTexture = new WebCamTexture (deviceStr);
		webcamRenderer.material.mainTexture = camTexture;
		camTexture.Play ();
	}

	public SkeletonAnimation[] skelAnim;
	void Update(){
		if( Input.GetMouseButtonDown(0)){
			GameObject pickedGO = mousePick ();
			if( pickedGO != null ){
				int objectIndex = -1;
				switch( pickedGO.tag ){
				case "PhotoboothSign":
					tapTableButton(0);
					break;
				case "PhotoboothGallery":
					cameraBooth.transform.localPosition = Vector3.right*1000;
					album.transform.localPosition = Vector3.right*1000;
					gallery.transform.localPosition = Vector3.zero;
					tapTableButton(1);
					break;
				case "PhotoboothAlbum":
					cameraBooth.transform.localPosition = Vector3.right*1000;
					album.transform.localPosition = Vector3.zero;
					gallery.transform.localPosition = Vector3.right*1000;
					tapTableButton(2);
					break;
				case "PhotoboothCamera":
					cameraBooth.transform.localPosition = Vector3.zero;
					album.transform.localPosition = Vector3.right*1000;
					gallery.transform.localPosition = Vector3.right*1000;
					tapTableButton(3);
					break;
				case "BackBtn":
					pressBack();
					break;
				case "CamShutterBtn":
					pressedShutter();
					break;
				case "PhotoRejectBtn":
					pressedPhotoReject();
					break;
				case "PhotoConfirmBtn":
					pressedPhotoConfirm();
					setShutterBtnActive( false, false);
					break;
				case "HairBtn":
					tray.state.SetAnimation(0, "Tap_Head", false);
					break;
				case "FaceBtn":
					tray.state.SetAnimation(0, "Tap_Face", false);
					break;
				case "MaskBtn":
					tray.state.SetAnimation(0, "Tap_Glasses", false);
					break;
				case "HatBtn":
					tray.state.SetAnimation(0, "Tap_Hat", false);
					break;
				case "PhotoDone":
//					shouldSnapShot = true;
//					StartCoroutine(pressedDoneBtn ());
					screenShotter.doScreenShot();
					break;
			}
			}
		}
	}

	public ScreenShotter screenShotter;

//	public Renderer picTester;
//
//	IEnumerator pressedDoneBtn(){
//		yield return new WaitForEndOfFrame ();
//		Texture2D tex = new Texture2D(Screen.width, Screen.height);
//		tex.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);
//		tex.Apply();
//
//		picTester.material.SetTexture ("_MainTex", tex);
//		picTester.enabled = true;
//	}

//	bool shouldSnapShot = false;
//	void LateUpdate(){
//		if (shouldSnapShot) {
//			pressedDoneBtn();
//			shouldSnapShot = false;
//		}
//	}

	public GameObject shutterBtn;
	public GameObject confirmPhotoBtn;
	public GameObject rejectPhotoBtn;
	void pressedShutter(){
		takeSnapshot ();
		setShutterBtnActive (false);
	}

	public Renderer trayMaskRenderer;
	void pressedPhotoConfirm(){
		iTween.MoveTo (tray.gameObject, iTween.Hash ("y", trayStartY, "time", 0.5f));
		trayMaskRenderer.enabled = true;
	}

	void pressedPhotoReject(){
		setShutterBtnActive (true);
		unsnap ();
	}

	void setShutterBtnActive( bool shutterActive, bool leaveOneAlive = true ){
		shutterBtn.SetActive (shutterActive && leaveOneAlive);
		confirmPhotoBtn.SetActive (!shutterActive && leaveOneAlive);
		rejectPhotoBtn.SetActive (!shutterActive && leaveOneAlive);
	}

	public Collider[] backBtnColliderArr;
	public Collider[] tableBtnArr;
	void tapTableButton(int objectIndex){
		if( objectIndex > -1 )
			skelAnim[objectIndex].state.SetAnimation(0, "Tap", false );
		
		if( objectIndex > 0 ){
			for( int i = 0; i < skelAnim.Length; i++ ){
				if( i != objectIndex ){
					skelAnim[i].state.SetAnimation(0, "Open", false);
				}
			}
			foreach( Collider col in backBtnColliderArr )
				col.enabled = true;

			foreach( Collider col in tableBtnArr )
				col.enabled = false;
		}
	}

	Texture2D snap;
	void takeSnapshot(){
		snap = new Texture2D(camTexture.width, camTexture.height);
//		print ("camTexture.w: " + camTexture.width + ", .h: " + camTexture.height);
		snap.SetPixels(camTexture.GetPixels());
		snap.Apply();
		
		webcamRenderer.material.mainTexture = snap;
	}

	void unsnap(){
		webcamRenderer.material.mainTexture = camTexture;

		if( snap != null)
			Destroy (snap);
	}

	void pressBack(){
		foreach (SkeletonAnimation anim in skelAnim) {
			anim.state.SetAnimation(0, "Close", false);
			foreach( Collider col in backBtnColliderArr )
				col.enabled = false;
			foreach( Collider col in tableBtnArr )
				col.enabled = true;
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	void doWin(){
		base.doWin ();
	}
}
