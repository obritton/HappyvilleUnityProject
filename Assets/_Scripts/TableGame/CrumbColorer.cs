using UnityEngine;
using System.Collections;

public class CrumbColorer : MonoBehaviour {

	public TextAsset configTextFile;

	public Color getColorForFood( string foodName ){
		string colorStr = getValueForKey (linesArr, foodName);
		if (colorStr != "") {
			string[] cArr = colorStr.Split(","[0]);
			if( cArr.Length == 3 ){
				int r, g, b;
				if( int.TryParse( cArr[0], out r ) && int.TryParse( cArr[1], out g ) && int.TryParse( cArr[2], out b )){
					return new Color( r/255.0f,g/255.0f,b/255.0f);
				}
			}
		}
		return Color.black;
	}

	// Use this for initialization
	void Start () {
		if (configTextFile != null) {
			buildConfigObjects (configTextFile);
		}
	}

	string[] linesArr;
	public void buildConfigObjects( TextAsset configFile )
	{
		string text = configFile.text;
		linesArr = text.Split("\n"[0]);
	}

	protected string getValueForKey( string[] lines, string key )
	{
		string keyStr = search ( lines, key );
		string[] keyVal = keyStr.Split ( ":"[0] );
		
		if( keyVal.Length > 1 )
			return keyVal[1];
		
		return "";
	}
	
	string search( string[] stringArr, string searchTerm )
	{
		foreach( string str in stringArr )
			if( str.Contains(searchTerm))
				return str;
		
		return "";
	}
}
