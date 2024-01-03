using UnityEngine;
using System.Collections;

public class JinKuangTouch : MonoBehaviour {
	
	public GameObject objJinKuangSpace;
	
	// Use this for initialization
	void Start () {
	
		if (objJinKuangSpace != null) 
		{
			UIEventListener.Get(objJinKuangSpace).onClick = clickTouchWinDelegate;
		}
	}
	
	// 点击其他地方会隐藏好友列表 ....
	void clickTouchWinDelegate(GameObject tween)
	{
		// 如果有好友列表显示则隐藏 ....
		U3dCmn.SendMessage("JinKuangFriendListWin", "DisFriendList", null);
	}
}
