using UnityEngine;
using System.Collections;
using CMNCMD;
public class RenewVersionWin : MonoBehaviour {
	public TweenPosition 	popwin_position;
	//public TweenScale	 	popwin_scale;
	//public UILabel 			pop_text;
	// Use this for initialization
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
	}
	
	//弹出窗口 
	void RevealPanel()
	{
		  
		//pop_text.text = content;
		popwin_position.Play(true);
		//popwin_scale.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		popwin_position.Play(false);
		//popwin_scale.Play(false);
	}
	//打开更新网址 
	void OpenRenewURL()
	{
		if(CommonData.APP_VERSION == APP_VERSION_TYPE.TINYKINGDOM)
			Application.OpenURL("http://www.nutgame.net/ageofempire/update/update.php?appid=" + CommonMB.AppID);
	}
}
