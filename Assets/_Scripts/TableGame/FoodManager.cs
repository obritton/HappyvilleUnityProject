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
//		print ("thoughtSkinName: " + thoughtSkinName + ", level: " + level);
		string foodSkinName = "";
		List<Skin> foodNameOptions = new List<Skin> ();
		Food foodComp = food.GetComponent<Food> ();
		SkeletonAnimation skelAnim = food.GetComponent<SkeletonAnimation> ();
		List<string> foodOptions = new List<string>();
		if (skelAnim != null) {
			foodNameOptions = skelAnim.state.Data.SkeletonData.Skins;
		}
//		print ("setRandomFood... 1");
		switch (level) {
		case 1://FOOD after last dash = THOUGHT before first dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[0];
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] skinTokenArr = skin.name.Split("-"[0]);
					string foodKey = skinTokenArr[1];
//					print ("thoughtKey: " + thoughtKey + ", foodKey: " + foodKey);
					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
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
		}}}}
			break;
		case 3:case 4:case 6:case 8:case 9:case 11:case 12://word = word
		{
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string foodKey = skin.name;
					if( string.Equals(thoughtSkinName,foodKey, System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
			break;
		case 10://FOOD first 2 letters = THOUGHT first two letters after dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr.Length > 1 ? thoughtTokenArr[1].Substring(0,2) : thoughtSkinName.Substring(0,2);
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string foodKey = skin.name.Substring(0,2);
					if( string.Equals(thoughtKey,foodKey, System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
			break;
		case 13://FOOD last 3 letters before dash = THOUGHT last 3 letters before dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[thoughtTokenArr.Length-2];
			thoughtKey = thoughtKey.Substring(thoughtKey.Length-3, 3);
			print ("13 thoughtKey: " + thoughtKey);
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
//					print ("  skin.name: " + skin.name);
					string[] foodTokenArr = skin.name.Split("-"[0]);
					string foodKey = foodTokenArr[foodTokenArr.Length-2];
					foodKey = foodKey.Substring(foodKey.Length-3, 3);
					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
			}}}}
			break;
		case 14:case 17://FOOD word = THOUGHT sum of words before & after + sign
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("+"[0]);
			int thoughtKey = int.Parse(thoughtTokenArr[0]) + int.Parse(thoughtTokenArr[1]);
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					int foodKey = int.Parse(skin.name);
					if( foodKey == thoughtKey){
//					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
			break;
		case 15://FOOD word before _ = THOUGHT word
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] foodTokenArr = skin.name.Split("-"[0]);
					string foodKey = foodTokenArr[0];
					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
			break;
		case 16://FOOD word after dash = THOUGHT word
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] foodTokenArr = skin.name.Split("-"[0]);
					string foodKey = foodTokenArr[1];
					if( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
			break;
		case 18://FOOD first letter = THOUGHT first letter
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
//					print ("skin.name.Substring(0,1): " + skin.name.Substring(0,1) + ", thoughtKey.Substring(0,1): " + thoughtKey.Substring(0,1));
					if( string.Equals(skin.name.Substring(0,1),thoughtKey.Substring(0,1),System.StringComparison.OrdinalIgnoreCase)){
						foodOptions.Add(skin.name);
		}}}}
			break;
		case 7://Custom
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					if( (skin.name == "Apple-red" && thoughtKey == "Red-color")
					   || (skin.name == "Candy-blue-diamond" && thoughtKey == "Blue-color")
					   || (skin.name == "Candy-blue-diamond" && thoughtKey == "Blue-color")
					   || (skin.name == "Candy-green-diamond" && thoughtKey == "Green-color")
					   || (skin.name == "Candy-pink-diamond" && thoughtKey == "Pink-color")
					   || (skin.name == "Candy-pink-star" && thoughtKey == "Pink-color")
					   || (skin.name == "Candy-purple-circle" && thoughtKey == "Purple-color")
					   || (skin.name == "Candy-purple-square" && thoughtKey == "Purple-color")
					   || (skin.name == "Candy-purple-square" && thoughtKey == "Purple-color")
					   || (skin.name == "Cheese-triangle" && thoughtKey == "Yellow-color")
					   || (skin.name == "GreenApple-circle" && thoughtKey == "Green-color")
					   || (skin.name == "Lemon" && thoughtKey == "Yellow-color")
					   || (skin.name == "Orange" && thoughtKey == "Orange-color")
					   || (skin.name == "Orange-half-circle" && thoughtKey == "Orange-color")
					   || (skin.name == "Pumpkin-oval" && thoughtKey == "Orange-color")
					   || (skin.name == "Strawberry-triangle" && thoughtKey == "Red-color")
					   || (skin.name == "Strawberry-triangle" && thoughtKey == "Red-color")
					   || (skin.name == "Watermelon-oval" && thoughtKey == "Green-color")
					   || (skin.name == "Watermelon-triangle" && thoughtKey == "Red-color"))
						foodOptions.Add(skin.name);
		}}}
			break;
		default:
			break;
		}

		int length = foodOptions.Count;
		foodSkinName = foodOptions[Random.Range(0,length)];
		skelAnim.skeleton.SetSkin (foodSkinName);
	}

	public static bool isMatch( int level, List<Skin> foodNameOptions, string thoughtSkinName ){
		switch (level) {
		case 1://FOOD after last dash = THOUGHT before first dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[0];
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] skinTokenArr = skin.name.Split("-"[0]);
					string foodKey = skinTokenArr[1];
					return ( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase));
		}}}
			break;
		case 2:case 5://FOOD word = THOUGHT word after last dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[thoughtTokenArr.Length-1];
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string foodKey = skin.name;
					return ( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase));
		}}}
			break;
		case 3:case 4:case 6:case 8:case 9:case 11:case 12://word = word
		{
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string foodKey = skin.name;
					return ( foodKey == thoughtSkinName);
		}}}
			break;
		case 10://FOOD first 2 letters = THOUGHT first two letters after dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr.Length > 1 ? thoughtTokenArr[1].Substring(0,2) : thoughtSkinName.Substring(0,2);
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string foodKey = skin.name.Substring(0,2);
					return ( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase));
		}}}
			break;
		case 13://FOOD last 3 letters before dash = THOUGHT last 3 letters before dash
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("-"[0]);
			string thoughtKey = thoughtTokenArr[thoughtTokenArr.Length-2];
			thoughtKey = thoughtKey.Substring(thoughtKey.Length-3, 3);
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] foodTokenArr = skin.name.Split("-"[0]);
					string foodKey = foodTokenArr[foodTokenArr.Length-2];
					foodKey = foodKey.Substring(foodKey.Length-3, 3);
					return ( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase));
			}}}
			break;
		case 14:case 17://FOOD word = THOUGHT sum of words before & after + sign
		{
			string[] thoughtTokenArr = thoughtSkinName.Split("+"[0]);
			int thoughtKey = int.Parse(thoughtTokenArr[0]) + int.Parse(thoughtTokenArr[1]);
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					int foodKey = int.Parse(skin.name);
					return ( foodKey == thoughtKey);
		}}}
			break;
		case 15://FOOD word before _ = THOUGHT word
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] foodTokenArr = skin.name.Split("-"[0]);
					string foodKey = foodTokenArr[0];
					return ( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase));
		}}}
			break;
		case 16://FOOD word after dash = THOUGHT word
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					string[] foodTokenArr = skin.name.Split("-"[0]);
					string foodKey = foodTokenArr[1];
					return ( string.Equals(thoughtKey,foodKey,System.StringComparison.OrdinalIgnoreCase));
		}}}
			break;
		case 18://FOOD first letter = THOUGHT first letter
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					print ("skin.name.Substring(0,1): " + skin.name.Substring(0,1) + ", thoughtKey.Substring(0,1): " + thoughtKey.Substring(0,1));
					return ( string.Equals(skin.name.Substring(0,1),thoughtKey.Substring(0,1),System.StringComparison.OrdinalIgnoreCase));
		}}}
			break;
		case 7://Custom
		{
			string thoughtKey = thoughtSkinName;
			foreach( Skin skin in foodNameOptions ){
				if( skin.name != "default" ){
					return ((skin.name == "Apple-red" && thoughtKey == "Red-color")
					   || (skin.name == "Candy-blue-diamond" && thoughtKey == "Blue-color")
					   || (skin.name == "Candy-blue-diamond" && thoughtKey == "Blue-color")
					   || (skin.name == "Candy-green-diamond" && thoughtKey == "Green-color")
					   || (skin.name == "Candy-pink-diamond" && thoughtKey == "Pink-color")
					   || (skin.name == "Candy-pink-star" && thoughtKey == "Pink-color")
					   || (skin.name == "Candy-purple-circle" && thoughtKey == "Purple-color")
					   || (skin.name == "Candy-purple-square" && thoughtKey == "Purple-color")
					   || (skin.name == "Candy-purple-square" && thoughtKey == "Purple-color")
					   || (skin.name == "Cheese-triangle" && thoughtKey == "Yellow-color")
					   || (skin.name == "GreenApple-circle" && thoughtKey == "Green-color")
					   || (skin.name == "Lemon" && thoughtKey == "Yellow-color")
					   || (skin.name == "Orange" && thoughtKey == "Orange-color")
					   || (skin.name == "Orange-half-circle" && thoughtKey == "Orange-color")
					   || (skin.name == "Pumpkin-oval" && thoughtKey == "Orange-color")
					   || (skin.name == "Strawberry-triangle" && thoughtKey == "Red-color")
					   || (skin.name == "Strawberry-triangle" && thoughtKey == "Red-color")
					   || (skin.name == "Watermelon-oval" && thoughtKey == "Green-color")
					   || (skin.name == "Watermelon-triangle" && thoughtKey == "Red-color"));
		}}}
			break;
		default:
			break;
		}

		return false;
	}

	public static GameObject createFoodForLevel( int level ){
		GameObject foodPrefab = getFoodPrefabForLevel (level);
		GameObject food = Instantiate( foodPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		return food;
	}

	public static GameObject createThoughtShapeForLevel( int level ){
		GameObject thoughtShapePrefab = getThoughtShapePrefabForLevel (level);
		GameObject thoughtShape = Instantiate( thoughtShapePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		return thoughtShape;
	}

	static GameObject getThoughtShapePrefabForLevel( int level ){
		string urlStr = "TableGame/Level_" + level + "/Thought-shapePrefab";
		GameObject thoughtShapePrefab = Resources.Load (urlStr) as GameObject;
//		print ("getThoughtShapePrefabForLevel " + level +": " + urlStr + (thoughtShapePrefab==null? " NULL":" good") + " thought");
		return thoughtShapePrefab;
	}

	static GameObject getFoodPrefabForLevel( int level ){
		string path = "TableGame/Level_" + level + "/FoodPrefab";
		GameObject foodPrefab = Resources.Load (path) as GameObject;
		return foodPrefab;
	}
}
