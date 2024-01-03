using UnityEngine;
using System.Collections;

public class RongYao : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//打开荣耀之战窗口 
	void OpenRongYaoWin()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("HonorInfoWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}	
	}
}
