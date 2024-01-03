using UnityEngine;
using System.Collections;
using CMNCMD;
public class PopLackDiamondWin : MonoBehaviour {

	public TweenPosition 	popwin_position;
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
		popwin_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		popwin_position.Play(false);
		Destroy(gameObject);
	}
	//打开充值界面 
	void OpenRechargeWin()
	{
		DismissPanel();
		if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("RechargeWinApple"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		}
		else if(CommonData.VERSION == VERSION_TYPE.VERSION_INNER)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("RechargeWinApple"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		}
	}
}
