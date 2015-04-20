using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;

public class FoodManager : MonoBehaviour {

	public int testLevel = 1;
	public bool testing = false;
	public bool printVerbose = false;

	GameObject food = null;

	void OnGUI(){
		if (testing && GUI.Button (new Rect (0, 0, Screen.width, Screen.height), "Test")) {
			GameObject tempFood = runTest();
			setRandomFoodSkinForLevel( tempFood, 1, "Circle-shape" );
		}
	}

	GameObject runTest(){
		if (food != null)
				Destroy (food);

		GameObject foodPrefab = getFoodPrefabForLevel (testLevel);
		if (foodPrefab != null) {
			food = Instantiate (foodPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			SkeletonAnimation foodAnim = food.GetComponent<SkeletonAnimation> ();
			AnimationStateData data = foodAnim.state.Data;
			List<Skin> skins = data.SkeletonData.Skins;
			if( printVerbose )
			{
				foreach (Skin skin in skins) {
					print ("Skin name: " + skin.Name);
				}
			}
		}
		
		return food;
	}
	
	public static ThoughtBubble.ThoughtShape getAnUnusedShape( ThoughtBubble.ThoughtShape[] usedShapes){
		return ThoughtBubble.ThoughtShape.None;
	}

	public static void setRandomFoodSkinForLevel( GameObject food, int level, string thoughtSkinName ){
		print ("thoughtSkinName: " + thoughtSkinName + ", level: " + level);
		string foodSkinName = "";
		List<Skin> foodNameOptions = new List<Skin> ();
		Food foodComp = food.GetComponent<Food> ();
		SkeletonAnimation skelAnim = food.GetComponent<SkeletonAnimation> ();
		List<string> foodOptions = new List<string>();
		if (skelAnim != null) {
			foodNameOptions = skelAnim.state.Data.SkeletonData.Skins;
		}

		switch (level) {
		case 1://FOOD after last dash = THOUGHT before first dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[0];
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] skinTokenArr = skin.name.Split("-"[0]);
					string foodKey = skinTokenArr[1];
					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
					}
				}
			}
		}
			break;
		case 2:case 5://FOOD word = THOUGHT word after last dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[thoughtTokenArr.Length-1];
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string foodKey = skin.name;
					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
					}
				}
			}
		}
			break;
		case 3:case 4:case 6:case 8:case 9:case 11:case 12://word = word
			break;
		case 10://FOOD first 2 letters = THOUGHT first two letters after dash
			break;
		case 13://FOOD last 3 letters before dash = THOUGHT last 3 letters before dash
			break;
		case 14:case 17://FOOD word = THOUGHT sum of words before & after + sign
			break;
		case 15://FOOD word before _ = THOUGHT word
			break;
		case 16://FOOD word after dash = THOUGHT word
			break;
		case 18://FOOD first letter = THOUGHT first letter
			break;
		case 7://Custom
			break;
		default:
			break;
		}

		int length = foodOptions.Count;
		foodSkinName = foodOptions[Random.Range(0,length)];
		skelAnim.skeleton.SetSkin (foodSkinName);
	}
	
	public static GameObject createFoodForLevel( int level ){
		GameObject foodPrefab = getFoodPrefabForLevel (level);
		GameObject food = Instantiate( foodPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		return food;
	}

	static GameObject getThoughtShapePrefabForLevel( int level ){
		string urlStr = "TableGame/Level_" + level + "/Thought-shape";
		GameObject thoughtShapePrefab = Resources.Load (urlStr) as GameObject;
		return thoughtShapePrefab;
	}

	static GameObject getFoodPrefabForLevel( int level ){
		string path = "TableGame/Level_" + level + "/FoodPrefab";
		GameObject foodPrefab = Resources.Load (path) as GameObject;
		return foodPrefab;
	}
}
