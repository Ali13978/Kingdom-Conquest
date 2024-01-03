using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class RechargeWinApple : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UIImageButton	free_diamond_btn;
	public UIImageButton	buy_btn;
	const int option_max = 6;
	public GameObject option_root;
	public UILabel VipDescribe;
	GameObject[] option_array =  new GameObject[option_max];
	bool reveal = false;
	
	// Use this for initialization
	void Start () {
		for(int i=0;i<option_max;i++)
		{
			option_array[i] = U3dCmn.GetChildObjByName(option_root,"Goods"+(i+1));
		}
		Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
		uint ShowPopad = 18;
		if (true == sMap.ContainsKey(ShowPopad))
		{
			CmnDefineMBInfo cmndef = (CmnDefineMBInfo) sMap[ShowPopad];
			if (cmndef.num == 0)
			{
				free_diamond_btn.gameObject.SetActiveRecursively(false);
				buy_btn.transform.localPosition += new Vector3(-60.61f,0,0);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(reveal)
		{
			InitialData();
			reveal = false;
		}
	}
	void RevealPanel()
	{
		CalculateVIPDiamond();
		reveal = true;
		
			free_diamond_btn.GetComponent<Collider>().enabled = false;
			free_diamond_btn.target.spriteName = "button1_disable";
			//free_diamond_btn.
	
	}
	//计算一下升级VIP需要的充值数 
	void CalculateVIPDiamond()
	{
		int total_diamond = (int)CommonData.player_online_info.TotalDiamond;
		VipRightInfo vip_info = (VipRightInfo)U3dCmn.GetNextVipInfo(total_diamond);
		if(vip_info.VipLevel == 0)
		{
			VipDescribe.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.VIP_IS_TOP);
		}
		else 
		{
			VipDescribe.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RECHARGE_VIP_DESCRIBE),total_diamond,vip_info.NeedDiamond-total_diamond,vip_info.VipLevel);
		}
		
	}
	void InitialData()
	{
		ArrayList list = new ArrayList(CommonMB.RechargeDiamond_Map.Keys);
      	list.Sort();
		for(int i=0;i<list.Count;i++)
		{
			RechargeDiamond recharge_info = (RechargeDiamond)CommonMB.RechargeDiamond_Map[(float)list[i]];
			float price = recharge_info.Money;
			int diamond = recharge_info.DiamondNum;
			int send_rate = recharge_info.SendDiamondNum*100/recharge_info.DiamondNum;
			UILabel[] lables = option_array[i].GetComponentsInChildren<UILabel>();
			for(int n=0;n<lables.Length;n++)
			{
				
				if(lables[n].name == "Describe")
				{
					if(send_rate > 0)
					{
						lables[n].text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RECHARGE_DESCRIBE),diamond,send_rate);
					}
					else 
					{
						lables[n].text =diamond.ToString();
					}
					
				}
				else if(lables[n].name == "Money")
				{
					lables[n].text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.RECHARGE_PRICE_APPLE),price);
				}
			}
		}
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//调用相应的充值接口 
	void StartRecharge()
	{
		int index = 0;
		for(int i=0;i<option_max;i++)
		{
			UICheckbox chek = option_array[i].GetComponentInChildren<UICheckbox>();
			if(chek.isChecked)
			{
				index = i;
			}
		}
		switch (index)
		{
			case 0: IAPManager.Instance.BuyDiamon1();break;
			case 1: IAPManager.Instance.BuyDiamon2();break;
			case 2: IAPManager.Instance.BuyDiamon3();break;
			case 3: IAPManager.Instance.BuyDiamon4();break;
			case 4: IAPManager.Instance.BuyDiamon5();break;
			case 5: IAPManager.Instance.BuyDiamon6();break;
			default: Debug.Log("cantfind");break;
		}
		/*ArrayList list = new ArrayList(CommonMB.RechargeDiamond_Map.Keys);
      	list.Sort();
		RechargeDiamond recharge_info = (RechargeDiamond)CommonMB.RechargeDiamond_Map[(float)list[index]];
		//记录购买请求 
		CTS_GAMECMD_RECHARGE_TRY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_RECHARGE_TRY;
		req.nDiamond3 = recharge_info.DiamondNum;
		TcpMsger.SendLogicData<CTS_GAMECMD_RECHARGE_TRY_T >(req);
		U3dCmn.SendMessage("91Manager","BuyDiamond",recharge_info.DiamondNum);
		
		//开启阻塞 ( 50secs ) ....
		//print ("Recharge:50 secs ...");
		LoadingManager.err = (int)MB_WARN_ERR.RECHARGE_TIMEOUT;
		LoadingManager.instance._ShowLoading(5);
		
		//API91.recharge_diamond = recharge_info.DiamondNum;
		//recharge_info.Money*/
	}
	
	//打开VIP说明 
	void OpenVipExplainWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin != null)
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[(uint)HELP_TYPE.VIP_HELP];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
	//打开adcolony广告 
	void OpenAdWin()
	{
		
		//U3dCmn.SendMessage("AdColonyManager","PlayV4VC",null);
	}
}
