using UnityEngine;
using System.Collections;

public class TiledCoord : MonoBehaviour {

	public UILabel text1 = null;
	
	// Use this for initialization
	void Start () {
		// Set widgets alpha 0
		if (text1 != null) {
			NGUITools.SetActiveSelf(text1.gameObject, false);
		}
	}
	
	public void Set1(int X, int Y)
	{
		if (text1 != null)
		{
			text1.text = string.Format("( {0},{1} )",X,Y);
			NGUITools.SetActiveSelf(text1.gameObject, true);
		}
	}
	
	void OnJinChengDoor()
	{
		SceneManager.SetObjVisibleByTag("UnGlobal",true);
		U3dCmn.SendMessage("BuildingManager","RefreshMoreGame",null);
		U3dCmn.SendMessage("BuildingManager","CheckMainCityObjOrUI",null);
		GameObject go = GameObject.Find("GlobalWorld");
		go.SetActiveRecursively ( false);
		Destroy(go);
		
		Resources.UnloadUnusedAssets();
	}

}
