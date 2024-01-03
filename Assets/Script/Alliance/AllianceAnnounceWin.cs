using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AllianceAnnounceWin : MonoBehaviour {
	//消息窗口positon TWEEN     
	public TweenPosition tween_position;  
	public UIInput 	announce_text; 
	
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
	// Use this for initialization
	void Start () {
	
	}
	

	//弹出窗口   
	void RevealPanel(string text)
	{
		announce_text.text = text;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//U3dCmn.SendMessage("AllianceWin","InitialPanelData",null);
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//修改联盟简介 
	void ModifyAnnounce()
	{
		AllianceManager.ReqModifyAnnounce(announce_text.text);
	}
}
