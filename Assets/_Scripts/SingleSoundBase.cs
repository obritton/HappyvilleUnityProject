using UnityEngine;
using System.Collections;

public class SingleSoundBase : MonoBehaviour {

	AudioSource aSource = null;

	public void playSingleSound( string sfxName ){
		if (aSource != null)
			aSource.Stop ();

		print ("playSingleSound: " + sfxName);
		aSource = SoundManager.PlaySFX (sfxName);
	}
}
