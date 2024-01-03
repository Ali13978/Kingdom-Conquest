using UnityEngine;
using System.Collections;
using CMNCMD;
public class PopWriteChatMsg : MonoBehaviour {
	
	public UIInput chatInput = null;
	public GameObject ButtonTo = null;
	public GameObject ButtonCancel = null;
	
	UIAnchor mDepth = null;
	
	void Awake()
	{
		// IPAD 适配尺寸 ...
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			if (root != null) { root.manualHeight = 360; }
		}
	}
	
	// Use this for initialization
	void Start () {
	
		if (ButtonTo != null)
		{
			UIEventListener.Get(ButtonTo).onClick = clickButtonToDelegate;
		}
		
		if (ButtonCancel != null)
		{
			UIEventListener.Get(ButtonCancel).onClick = clickButtonCloseDelegate;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	public void Popup1(float depth)
	{
		if (chatInput != null)
		{
			int Tipset = BaizVariableScript.CHAT_MSG_QUEST_GUIDE_NEWPLAYER;
			chatInput.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
		}
		
		Depth(depth);
	}
	
	void clickButtonToDelegate(GameObject tween)
	{
		if (chatInput != null)
		{
			string chatMsg = chatInput.text;
			ChatManager.SendWorldData(chatMsg);
		}
		
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
		//弹出免费抽奖 
		if(CommonData.player_online_info.FreeDrawLotteryNum >0)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPan"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel");
			}
		}
	}
	
	void clickButtonCloseDelegate(GameObject tween)
	{
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
		//弹出免费抽奖 
		if(CommonData.player_online_info.FreeDrawLotteryNum >0)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPan"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel");
			}
		}
	}
}
