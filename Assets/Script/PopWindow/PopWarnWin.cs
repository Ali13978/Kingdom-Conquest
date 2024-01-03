using UnityEngine;
using System.Collections;
using CMNCMD;
using UnityEngine.UI;
using TMPro;

public class PopWarnWin : MonoBehaviour {
	public TweenPosition 	popwin_position;
	public TweenScale	 	popwin_scale;
	//public UILabel 			pop_text;
	public TMP_Text 			pop_text;
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
}
