using UnityEngine;
using System.Collections;

public class ChouJiangManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//打开抽奖页面 
	void OpenChouJiangWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPan"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}
	}
}
