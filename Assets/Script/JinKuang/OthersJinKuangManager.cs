using UnityEngine;
using System.Collections;

public class OthersJinKuangManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	//打开别人的金矿窗口 
	public static void OpenOthersJinKuangWin(SIMPLE_CHAR_INFO char_info)
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("OthersJinKuangWin");
		if(infowin != null)
		{
			infowin.SendMessage("RevealPanel",char_info);
		}
	}
}
