using UnityEngine;
using System.Collections;
using CMNCMD;
public class DemiseWin : MonoBehaviour {
	//消息窗口positon TWEEN     
	public TweenPosition tween_position;
	public UIInput 	demise_member;
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
		demise_member.text = "";
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//禅让盟主  
	void DemiseLeader()
	{
		AllianceManager.ReqDemiseLeader(demise_member.text);
	}
}
