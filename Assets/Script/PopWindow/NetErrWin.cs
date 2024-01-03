using UnityEngine;
using System.Collections;
using CMNCMD;
public class NetErrWin : MonoBehaviour {
	public TweenPosition 	popwin_position;
	public TweenScale	 	popwin_scale;
	
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
	
	// Update is called once per frame
	void Update () {
	
	}
	//弹出窗口 
	void RevealPanel()
	{
		LoadingManager.instance.HideLoading();
		popwin_position.Play(true);
		popwin_scale.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		popwin_position.Play(false);
		popwin_scale.Play(false);
	}
	//重新连接 
	void Reconnect()
	{
		DismissPanel();
		U3dCmn.SendMessage("ReLoginManager","StartLogin",null);
		
	}
	//退出游戏  
	void QuitGame()
	{
		DismissPanel();
		Application.Quit();	
	}
}
