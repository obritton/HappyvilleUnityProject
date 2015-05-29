using UnityEngine;
using System.Collections;

public class MapUnlockSystem : MonoBehaviour {

	public enum GameType{
		None,
		TableGame,
		CatchGame,
		WhackGame,
		SlingshotGame,
		MatchGame,
		PuzzleGame,
		Photobooth
	}

	public static GameType lastGamePlayed = GameType.None;
	public static int lastTableGamePlayed = -1;
	public static bool wasLastGameCompleted = false;
	public static bool shouldNewButtonUnlock = false;

	static string tableGameUnlockedStr = "tableGameCompleted21";
	static string miniGameUnlockedStr = "miniGamePlayed21";
	static string hasPlayedFirstMiniGameStr = "hasPlayedFirstMiniGameStr21";

	public static bool shouldAutoUnlock = false;

	public static int tableGameCompleted(){
		return PlayerPrefs.GetInt(tableGameUnlockedStr);
	}

	public static int miniGamePlayed(){
		if (PlayerPrefs.GetInt (hasPlayedFirstMiniGameStr) == 0)
			return -1;

		return PlayerPrefs.GetInt(miniGameUnlockedStr);
	}
	
	public static void setTableGameComplete( int newHighTableGame ){
		PlayerPrefs.SetInt (tableGameUnlockedStr, newHighTableGame);
		PlayerPrefs.Save ();
	}
	
	public static void setMiniGamePlayed( int newHighMiniGame ){
		PlayerPrefs.SetInt (hasPlayedFirstMiniGameStr, 1);

		PlayerPrefs.SetInt (miniGameUnlockedStr, newHighMiniGame);
		PlayerPrefs.Save ();
	}
}
