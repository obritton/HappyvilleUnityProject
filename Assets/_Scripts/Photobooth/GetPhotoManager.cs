using UnityEngine;
using System.Collections;

public class GetPhotoManager : MonoBehaviour {

	private WebCamTexture camTexture;
	
	void Start()
	{
//		camTexture = new WebCamTexture();
//		GetComponent<Renderer>().material.mainTexture = camTexture;
//		camTexture.Play();
	}

	public SkeletonAnimation[] skelAnim;
	void Update(){
		if( Input.GetMouseButtonDown(0)){
			GameObject pickedGO = mousePick ();
			if( pickedGO != null ){
				switch( pickedGO.tag ){
				case "PhotoboothSign":
					break;
				case "PhotoboothGallery":
					break;
				case "PhotoboothAlbum":
					break;
				case "PhotoboothCamera":
					break;
				}
			}
		}
	}

	void takeSnapshot(){
		Texture2D snap = new Texture2D(camTexture.width, camTexture.height);
		snap.SetPixels(camTexture.GetPixels());
		snap.Apply();

		GetComponent<Renderer>().material.mainTexture = snap;
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
