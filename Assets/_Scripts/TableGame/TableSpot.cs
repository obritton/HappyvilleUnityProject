using UnityEngine;
using Spine;
using System.Collections;

public class TableSpot : MonoBehaviour {

	public Plate plate;
	public CharacterNode characterNode;
	public ThoughtBubble thoughtBubble;
	public CharacterNode.CharacterType characterType = CharacterNode.CharacterType.None;
	public ThoughtBubble.ThoughtShape thoughtShape = ThoughtBubble.ThoughtShape.None;

	public float addNewCharacterOfType(CharacterNode.CharacterType characterTypeIn, ThoughtBubble.ThoughtShape thoughtShapeIn, GameObject newCharacter, string skinName = "blank"){
		characterType = characterTypeIn;
		thoughtShape = thoughtShapeIn;

		float time = animateNewCharacterUpToTable (newCharacter);
		StartCoroutine( configAndAnimateNewThoughtBubbleForTypeWithDelay(thoughtShapeIn,TGConsts.kThoughtPopupDelay));
		return time;
	}

	IEnumerator delayCharacterAlpha(GameObject newCharacter)
	{
		yield return new WaitForSeconds (0.01f);
		newCharacter.transform.localPosition = Vector3.zero;
		newCharacter.GetComponent<Renderer>().enabled = true;
	}

	float animateNewCharacterUpToTable( GameObject newCharacter){
		newCharacter.GetComponent<Renderer>().enabled = false;
		if (characterNode.transform.childCount > 0) {
			Destroy (characterNode.transform.GetChild (0).gameObject);
		}
		newCharacter.transform.parent = characterNode.transform;

		TrackEntry te = ((SkeletonAnimation)newCharacter.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "Popup", false );
		SoundManager.PlaySFX ("Character_Popup");
		((SkeletonAnimation)newCharacter.GetComponent<SkeletonAnimation> ()).state.AddAnimation(0, "Idle", true, 0 );

		StartCoroutine (delayCharacterAlpha (newCharacter));

		return te.animation.duration;
	}

	IEnumerator configAndAnimateNewThoughtBubbleForTypeWithDelay( ThoughtBubble.ThoughtShape thoughtShapeIn, float delay = 0 ){
		yield return new WaitForSeconds( delay );
		SoundManager.PlaySFX("Thought_PopUp");
//		SoundManager.PlaySFX("Character_Popup");
		((SkeletonAnimation)thoughtBubble.GetComponent<SkeletonAnimation> ()).state.SetAnimation (0, "PopUp-thought", false );
		((SkeletonAnimation)thoughtBubble.GetComponent<SkeletonAnimation> ()).state.AddAnimation (0, "IdleOne-thought", true, 0);
		yield return new WaitForSeconds( 0.01f );
		thoughtBubble.GetComponent<Renderer>().enabled = true;
		StartCoroutine (thoughtBubble.popupShape ());
	}
}
