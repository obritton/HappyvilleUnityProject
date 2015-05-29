using UnityEngine;
using System.Collections;

public class SingleSoundBase : MonoBehaviour {

	AudioSource aSource = null;

	protected void playSingleSound( string sfxName ){
		if (aSource != null)
			aSource.Stop ();

		aSource = SoundManager.PlaySFX (sfxName);
	}
}
