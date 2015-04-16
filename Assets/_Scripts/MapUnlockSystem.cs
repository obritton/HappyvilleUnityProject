using UnityEngine;
using System.Collections;

public class MapUnlockSystem : MonoBehaviour {

	static string tableGameUnlockedStr = "tableGameUnlocked";
	static string miniGameUnlockedStr = "miniGameUnlocked";

	public static int tableGameUnlocked(){
		return PlayerPrefs.GetInt(tableGameUnlockedStr);
	}

	public static int miniGameUnlocked(){
		return PlayerPrefs.GetInt(miniGameUnlockedStr);
	}

	public static void unlockTableGame( int newHighTableGame ){
		PlayerPrefs.SetInt (tableGameUnlockedStr, newHighTableGame);
		PlayerPrefs.Save ();
	}

	public static void unlockMiniGame( int newHighMiniGame ){
		PlayerPrefs.SetInt (miniGameUnlockedStr, newHighMiniGame);
		PlayerPrefs.Save ();
	}
}
