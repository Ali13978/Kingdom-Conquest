using UnityEngine;
using System.Collections;
using CMNCMD;
public class QuitWin : MonoBehaviour {
	public TweenPosition 	popwin_position;
	public TweenScale	 	popwin_scale;
	public UILabel 			pop_text;
	
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
	void RevealPanel(string content)
	{
		pop_text.text = content;
		popwin_position.Play(true);
		popwin_scale.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		popwin_position.Play(false);
		popwin_scale.Play(false);
	}
	//退出游戏  
	void QuitGame()
	{
		DismissPanel();
		Application.Quit();	
	}
}
