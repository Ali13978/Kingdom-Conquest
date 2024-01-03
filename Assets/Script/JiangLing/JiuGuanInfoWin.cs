using UnityEngine;
using System;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class JiuGuanInfoWin : MonoBehaviour {
	
	public TweenPosition 	tween_position;
	//建筑信息面板  
	public UILabel			need_building;
	public UILabel 			need_gold;
	public UILabel 			need_people;
	public UILabel			need_time;
	public UILabel			hero_num_label;
	public UILabel 			yingxiongbang_label;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	string need_time_str;
	public UICheckbox 		zhaomu_tab;
	//招募武将面板 
	public GameObject JiangLingPage;
	public UILabel RefreshTime;
	//public UILabel JiangLingNum; 
	//酒馆武将缓存 
	//ArrayList JiangLingArray = new ArrayList();
	public static Hashtable JiangLingMap = new Hashtable();
	JiuGuanUnit now_unit;
	bool isstart;
	public struct JiuGuanUnit
	{
		public uint level;
		public uint build_state;
		public int 	build_end_time;
	}
	
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
		//记录一下初始格式化字符串   
		need_time_str = need_time.text;
	}
	
	// Use this for initialization
	void Start () {
		isstart = true;
		
	}
	
	// Update is called once per frame
	void Update () {
		//需要OBJ初始化在START函数之后执行的函数  
		if(isstart)
		{
			RevealPanel(now_unit);
			isstart =false;
		}
	}
	//弹出窗口   
	void RevealPanel(JiuGuanUnit unit)
	{
		
		now_unit = unit;
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JIUGUAN))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			if(unit.build_state == (uint)BuildingState.NORMAL)
			{
				accelerate_btn.SetActiveRecursively(false);
				upgrade_btn.SetActiveRecursively(true);
			}
			else if(unit.build_state == (uint)BuildingState.UPGRADING)
			{
				accelerate_btn.SetActiveRecursively(true);
				upgrade_btn.SetActiveRecursively(false);
			}
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JIUGUAN,(int)(now_unit.level+1));
			int sec = (int)info.BuildTime; 
			//print("jjjjjjjjjjjjjjj"+sec);
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			need_time.text= string.Format(need_time_str,hour,minute,second);
			
			bool btn_enable = true;
			uint need = now_unit.level+1;
			uint now = ChengBao.NowLevel;
			need_building.text = need + "/"+now;
			if(now<need)
			{
				btn_enable = false;
				need_building.color = new Color(1,0,0,1);
			}
			else 
			{
				need_building.color = new Color(0,0.38f,0,1);
			}
			
			need = (uint)info.Money;
			now = CommonData.player_online_info.Gold;
			need_gold.text = need + "/"+U3dCmn.GetNumStr(now);
			if(now<need)
			{
				btn_enable = false;
				need_gold.color = new Color(1,0,0,1);
			}
			else 
			{
				need_gold.color = new Color(0,0.38f,0,1);
			}
			
			need = (uint)info.Worker; 
			now = CommonData.player_online_info.Population;
			need_people.text = need + "/"+U3dCmn.GetNumStr(now);
			if(now<need)
			{
				btn_enable = false;
				need_people.color = new Color(1,0,0,1);
			}
			else 
			{
				need_people.color = new Color(0,0.38f,0,1);
			}
			
			if(!btn_enable)
			{
				upgrade_btn.GetComponent<Collider>().enabled = false;
				upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1_disable";
	
			}
			else
			{
				upgrade_btn.GetComponent<Collider>().enabled = true;
				upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1";
			}
			tween_position.Play(true);
		}
	}
	//切换到招募面板 
	void TabZhaoMu()
	{
		zhaomu_tab.isChecked = true;
		RefreshJiuGuan();
	}
	//刷新建造面板界面 
	void RefreshBuildPanel()
	{
		RevealPanel(now_unit);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//升级酒馆 
	void ReqUpgradeJiuGuan()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JIUGUAN))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JIUGUAN,1);
			CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_BUILDING;
			req.nAutoID3 = info.BeginID;
			//print ("upgrade bingying"+req.nAutoID3 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING);
		}
		DismissPanel();
	}
	//获取酒馆刷新状态（检查CD 则检查缓存 如果CD中 缓存也存在则显示界面 否则请求刷新将领） 
	void RefreshJiuGuan()
	{
		JiangLingPage.SetActiveRecursively(false);
		upgrade_btn.SetActiveRecursively(false);
		RefreshTime.text = "";
		int num = JiangLingManager.MyHeroMap.Count;
		int hero_max_num = U3dCmn.GetHeroMaxNumByCharLevel(CommonData.player_online_info.Level);
		hero_num_label.text = num+"/"+hero_max_num;
		yingxiongbang_label.text = CangKuManager.GetItemInfoByExcelID((int)ARTICLES.YINGXIONGBANG).nNum3.ToString();
		//JiangLingNum.text = "";
		
		CTS_GAMECMD_OPERATE_TAVERN_REFRESH_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_TAVERN_REFRESH;
		req.nUseItem3 = 0;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_TAVERN_REFRESH_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_TAVERN_REFRESH);
	}
	//更新当前招募的英雄数量 
	void RefreshHeroInfo()
	{
		int num = JiangLingManager.MyHeroMap.Count;
		int hero_max_num = U3dCmn.GetHeroMaxNumByCharLevel(CommonData.player_online_info.Level);
		hero_num_label.text = num+"/"+hero_max_num;
	}
	//PopConfirmWin.instance.ShowPopWin(str,ModifyLordName);
	//使用英雄榜立即刷新酒馆  
	void RefreshJiuGuanAtOnce()
	{
		
		if(CangKuManager.GetItemInfoByExcelID((int)ARTICLES.YINGXIONGBANG).nNum3 >0)
		{
			string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ZHAOMU_MINGJIANG);
			PopConfirmWin.instance.ShowPopWin(str,RefreshSeniorHero);
		}
		else 
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LACK_YINGXIONGBANG));
		}
	}
	void RefreshSeniorHero(GameObject obj)
	{
		CTS_GAMECMD_OPERATE_TAVERN_REFRESH_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_TAVERN_REFRESH;
		req.nUseItem3 = (int)ARTICLES.YINGXIONGBANG;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_TAVERN_REFRESH_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_TAVERN_REFRESH);
	}
	//酒馆刷新状态返回 
	void ProcessRefreshJiuGuanRst(byte[] buff)
	{
		
		STC_GAMECMD_OPERATE_TAVERN_REFRESH_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_TAVERN_REFRESH_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_TAVERN_REFRESH_T.enum_rst.RST_OK || sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_TAVERN_REFRESH_T.enum_rst.RST_OK_USEITEM)
		{
			if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_TAVERN_REFRESH_T.enum_rst.RST_OK_USEITEM)
			{
				ulong item_id = CangKuManager.GetItemInfoByExcelID((int)ARTICLES.YINGXIONGBANG).nItemID1;
				CangKuManager.SubItem(item_id,1);
				yingxiongbang_label.text = CangKuManager.GetItemInfoByExcelID((int)ARTICLES.YINGXIONGBANG).nNum3.ToString();
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.USE_HEROLIST_PROMPT));
			}
			
			//print ("CD over");
			
			//CD时间到 可以重新获取酒馆武将信息 
			RefreshJiangLing();
			StopCoroutine("Countdown");
			RefreshTime.text = "";
			int EndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sub_msg.nCD2;
			StartCoroutine("Countdown",EndTimeSec);
			
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_TAVERN_REFRESH_T.enum_rst.RST_CD)
		{
			StopCoroutine("Countdown");
			RefreshTime.text = "";
			int EndTimeSec = DataConvert.DateTimeToInt(DateTime.Now)+sub_msg.nCD2;
			StartCoroutine("Countdown",EndTimeSec);
			//CD中 检查缓存是否有武将信息 没有的话向服务器请求 
			if(JiangLingMap.Count > 0)
			{
				//print ("CD ing :" + JiangLingMap.Count);
				JiangLingPage.SetActiveRecursively(true);
				upgrade_btn.SetActiveRecursively(true);
				//初始化六个将领界面 
				int num =1;
				foreach(DictionaryEntry	de	in	JiangLingMap)  
  				{
					RefreshHero heroinfo =(RefreshHero)de.Value;
					GameObject obj = U3dCmn.GetChildObjByName(JiangLingPage,"JiangLing"+heroinfo.nSlotID1);
					obj.SetActiveRecursively(true);
					obj.SendMessage("InitialUI",heroinfo.nHeroID2);
					num++;
				}
			
			}
			else 
			{
				//print ("CD ing Refresh:");
				RefreshJiangLing();
			}
		}
		
		// < 新手指导 > 等待刷新武将 ....
		if (NewbieJiuGuan.processTavernRefreshRst != null)
		{
			NewbieJiuGuan.processTavernRefreshRst();
			NewbieJiuGuan.processTavernRefreshRst = null;
		}
	}

	//刷新时间倒计时  
	IEnumerator Countdown(int EndTimeSec)
	{
		int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		if(sec <0)
			sec = 0;
		while(sec!=0)
		{
			sec =(int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if(sec <0)
				sec = 0;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			if(hour>=100)
				RefreshTime.text =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			else
				RefreshTime.text =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
			yield return new WaitForSeconds(1);
		}
		RefreshJiuGuan();
	}
	//获取酒馆武将信息 
	void RefreshJiangLing()
	{
			
		CTS_GAMECMD_OPERATE_GET_HERO_REFRESH_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_GET_HERO_REFRESH;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_GET_HERO_REFRESH_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_GET_HERO_REFRESH);
	}
	//获取酒馆武将信息返回 
	void ProcessRefreshJiangLingRst(byte[] buff)
	{
	
		STC_GAMECMD_OPERATE_GET_HERO_REFRESH_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_GET_HERO_REFRESH_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_GET_HERO_REFRESH_T.enum_rst.RST_OK)
		{
			JiangLingMap.Clear();
			JiangLingPage.SetActiveRecursively(true);
			upgrade_btn.SetActiveRecursively(true);
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_GET_HERO_REFRESH_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			RefreshHero[] jiangling_list = (RefreshHero[])DataConvert.ByteToStructArray<RefreshHero>(data_buff,sub_msg.nNum2);

			for(int i=0;i<jiangling_list.Length;i++)
			{	
				//初始化六个将领界面 
				JiangLingMap.Add(jiangling_list[i].nHeroID2,jiangling_list[i]);
				if(i<6)
				{
					GameObject obj = U3dCmn.GetChildObjByName(JiangLingPage,"JiangLing"+jiangling_list[i].nSlotID1);
					obj.SetActiveRecursively(true);
					obj.SendMessage("InitialUI",jiangling_list[i].nHeroID2);
				}
				
			}
		}
	}
	//招募将领返回 
	void ProcessJiangLingZhaoMuRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_HIRE_HERO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_HIRE_HERO_T>(buff);
		//print ("zzzzzzzzzzzzzzzzzzz"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_HIRE_HERO_T.enum_rst.RST_OK)
		{
			JiangLingMap.Remove((ulong)sub_msg.nHeroID2);
			GameObject obj = U3dCmn.GetChildObjByName(JiangLingPage,"JiangLing"+sub_msg.nSlotID3);
			obj.SetActiveRecursively(false);
			//重新获取已招募将领列表 
			JiangLingManager.GetJiangLingInfo();
			
			// <新手引导> 成功返回 ... 
			if (NewbieJiuGuan.processZhaoMuRst != null)
			{
				NewbieJiuGuan.processZhaoMuRst();
				NewbieJiuGuan.processZhaoMuRst = null;
			}
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_HIRE_HERO_T.enum_rst.RST_HERO_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_IS_FULL));
		}
		
		// <新手引导> ... zhao mu
		NewbieJiuGuan.processZhaoMuRst = null;
	}
	//加速酒馆升级  
	void ReqAccelerateJiuGuan()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JIUGUAN,1);
		uint AutoID = info.BeginID;
		AccelerateWin.AccelerateUnit unit;
		unit.Type = (int)enum_accelerate_type.building;
		unit.BuildingType = (uint)te_type_building.te_subtype_building_upgrade; 
		unit.autoid = AutoID;
		unit.EndTimeSec = now_unit.build_end_time;
		GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
			AccelerateWin.MotherWin = gameObject;
			AccelerateWin.CloseCallBack = "DismissPanel";
		}
	}
}
