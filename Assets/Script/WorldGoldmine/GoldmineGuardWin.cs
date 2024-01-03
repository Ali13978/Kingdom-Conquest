using UnityEngine;
using System.Collections;
using System;
using CMNCMD;
// 世界金矿查看状态 ...
public class GoldmineGuardWin : MonoBehaviour {
	
	public UILabel goldmineAdd = null;			// 金矿增加效果 ...
	public UILabel playerName = null;			// 是哪个玩家占领 ...
	public UILabel goldmineStatus = null; 		// 当前状态 ...
	public UIImageButton attack_btn;
	public UIImageButton lordinfo_btn;
	UIAnchor mDepth = null;
	
	string mPrettyName;
	string mPrettyStatus;
	string mPrettyAdd;
	
	GoldmineCardData mPrettyCard = null;
	
	void Awake() {
		if (playerName != null)
		{
			mPrettyName = playerName.text;
			playerName.text = "";
		}
		if (goldmineStatus != null)
		{
			mPrettyStatus = goldmineStatus.text;
			goldmineStatus.text = "";
		}
		if (goldmineAdd != null)
		{
			mPrettyAdd = goldmineAdd.text;
			goldmineAdd.text = "";
		}
		
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
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		GoldmineInstance.processDropGoldmineDelegate -= OnProcessDropMyGoldmineDelegate;
	}
	
	// Use this for initialization
	void Start () {

	}
	
	
	public void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	public float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	public void ApplyGoldmineData(GoldmineCardData newCard)
	{
		mPrettyCard = newCard;
		if (newCard.nAccountID != 0)
		{
			attack_btn.transform.localPosition = new Vector3(-53f,-54f,0);
			lordinfo_btn.gameObject.SetActiveRecursively(true);
		}
		else 
		{
			attack_btn.transform.localPosition = new Vector3(0,-54f,0);
			lordinfo_btn.gameObject.SetActiveRecursively(false);
		}
		if (playerName != null)
		{
			playerName.text = string.Format(mPrettyName,newCard.name);
		}
		else 
		{
			attack_btn.transform.localPosition = new Vector3(0,-54f,0);
		}
		if (goldmineAdd != null)
		{
			uint myLevel = (uint)CommonData.player_online_info.Level;
			Hashtable wgeMap = CommonMB.WorldGoldmineYieldMBInfo_Map;
			int d1 = newCard.n1Addition;
			if (true == wgeMap.ContainsKey(myLevel))
			{
				GoldmineYieldMBInfo card = (GoldmineYieldMBInfo) wgeMap[myLevel];
				d1 = Mathf.FloorToInt((float)d1 * card.exp);
			}
			
			goldmineAdd.text = string.Format(mPrettyAdd, newCard.n1Secs, d1);
		}
		if (goldmineStatus != null)
		{
			int nRestTime = newCard.n1SafeTime - newCard.nSafeTime;
			if (nRestTime <= 0)
			{
				goldmineStatus.text = string.Format(mPrettyStatus, "no");
			}
			else 
			{
				int endTimeSecs = nRestTime + DataConvert.DateTimeToInt(DateTime.Now);
				Begin(endTimeSecs);
			}
		}
	}
	
	void Begin(int endTimeSecs)
	{
		StopCoroutine("GoldmineCooldown");
		StartCoroutine("GoldmineCooldown", endTimeSecs);
	}
	
	IEnumerator GoldmineCooldown(int endTimeSec)
	{
		if (goldmineStatus != null)
		{
			int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if (nSecs <0) nSecs = 0;
			
			string RestTimeCol = "";
			while(nSecs > 0)
			{
				nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
				if(nSecs <0)
					nSecs = 0;
				
				int hour = nSecs/3600;
				int minute = nSecs/60%60;
				int second = nSecs%60;
				
				if(hour>=100)
				{
					RestTimeCol = string.Format("{0}",hour)+":"
							+ string.Format("{0:D2}",minute)+":" 
							+ string.Format("{0:D2}",second);
				}
				else
				{
					RestTimeCol = string.Format("{0:D2}", hour)+":"
							+ string.Format("{0:D2}", minute)+":"
							+ string.Format("{0:D2}", second);
				}
				
				goldmineStatus.text = string.Format(mPrettyStatus, RestTimeCol);
				
				yield return new WaitForSeconds(1);
			}
		
			mPrettyCard.nSafeTime = mPrettyCard.n1SafeTime;
			goldmineStatus.text = string.Format(mPrettyStatus, "no");
		}
	}
	
	public void OnGoldmineGuardClose()
	{		
		Destroy(gameObject);
	}
	
	void OnProcessDropAcceptDelegate(GameObject tween)
	{
		GoldmineInstance.processDropGoldmineDelegate += OnProcessDropMyGoldmineDelegate;
		GoldmineInstance.RequestDropMyGoldmine();
		PopTipDialog.instance.Dissband();
	}
	// 金矿出征 ...
	public void OnGoldmineChuZhen()
	{
		ulong nAccountID = mPrettyCard.nAccountID;
		if (nAccountID == CommonData.player_online_info.AccountID)
			return;
		
		if (nAccountID > 0)
		{
			int nRestTime = mPrettyCard.n1SafeTime - mPrettyCard.nSafeTime;
			if (nRestTime>0) 
			{
				int Tipset = BaizVariableScript.WORLDGOLDMINE_ROB_SAFETIME;
				PopTipDialog.instance.VoidSetText1(true,false,Tipset);
				return;
			}
		}
		
		if (GoldmineInstance.instance.MyGoldmine.nAreaID >0)
		{
			int Tipset = BaizVariableScript.WORLDGOLDMINE_GIVEUP;
			PopTipDialog.instance.Mode1(true,true,Tipset);
			PopTipDialog.instance.VoidButton1(OnProcessDropAcceptDelegate);
			PopTipDialog.instance.Popup();
		}
		else 
		{
			OnProcessDropMyGoldmineDelegate();
		}
	}
	
	void OnProcessDropMyGoldmineDelegate()
	{	
		GoldmineScene.UpdateGoldmineGuard();
		
		GameObject go = U3dCmn.GetObjFromPrefab("GoldmineChuZhenWin");
		if (go == null) return;
		
		GoldmineChuZhen win = go.GetComponent<GoldmineChuZhen>();
		if (win != null)
		{
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win.Depth(depth);
			win.ApplyGoldmineData(mPrettyCard);
		}
		
		// Delete this;
		Destroy(gameObject);
	}
	//打开对方信息面板 
	void OpenPlayerInfoWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",mPrettyCard.nAccountID);
		}
		OnGoldmineGuardClose();
	}
	
}
