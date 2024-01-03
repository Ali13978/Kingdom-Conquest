using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ReinforceSoldierWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	// 兵种列表 
	public PagingStorage soldier_storage;
	//配兵面板 
	public GameObject reinforcesoldierpanel;
	public UILabel  reinforce_num;
	public UISlider num_slider;
	public UIInput  with_num;
	public UILabel  guild_reinforce_label;
	public UILabel  lord_level_limit_label;
	//public UILabel  soldier_label;
	//可以配置的士兵种类列表 
	public static ArrayList soldier_key_array = new ArrayList();
	
	//当前配置的兵种 
	SoldierUnit select_soldier;
	int max_soldier_num; 
	int max_reinforce_num_left;
	int max_reinforce_num;
	string soldier_str;
	//增援的玩家ID 
	ulong AllyID = 0;
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
		//soldier_str = soldier_label.text;
		//soldier_label.text = "";
	}
	
	// Use this for initialization
	void Start () {
		
	}
	 
	//弹出窗口 
	void RevealPanel(ulong AccountID)
	{
		AllyID = AccountID;
		CTS_GAMECMD_GET_LEFT_DONATE_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_LEFT_DONATE_SOLDIER;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_LEFT_DONATE_SOLDIER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_LEFT_DONATE_SOLDIER);
	}
	void InitialData(byte[] buff)
	{
		STC_GAMECMD_GET_LEFT_DONATE_SOLDIER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_LEFT_DONATE_SOLDIER_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_LEFT_DONATE_SOLDIER_T.enum_rst.RST_OK)
		{
			GuildReinforceInfo info =  U3dCmn.GetNextGuildReinforceInfo((int)sub_msg.nMaxNum3);
			guild_reinforce_label.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NEXT_GUILD_REINFORCE_NUM),info.GuildLevel,info.ReinforceMax);
			//print ("hhhhhhhhhh"+info.GuildLevel);
			reinforce_num.text = (sub_msg.nMaxNum3 - sub_msg.nLeftNum2)+"/"+sub_msg.nMaxNum3;
			max_reinforce_num_left = (int)sub_msg.nLeftNum2;
			max_reinforce_num = (int)sub_msg.nMaxNum3;
			reinforcesoldierpanel.SetActiveRecursively(false);
			InitialSoldierList();
			tween_position.Play(true);
			with_num.text = "0";
			num_slider.sliderValue =0;
			max_soldier_num = 0;
			//AllyID = AccountID;
		}
	}
	//关闭窗口   
	void DismissPanel()
	{
		
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//滑动条出发事件 调整训练数量 
	void OnSliderChange()
	{
		int max_num = max_soldier_num>max_reinforce_num_left?max_reinforce_num_left:max_soldier_num;//>(select_hero.nLeader13+select_hero.nLeaderAdd18)?(select_hero.nLeader13+select_hero.nLeaderAdd18):max_soldier_num;
		int num = (int)(num_slider.sliderValue*max_num);
		
		with_num.text = num.ToString();
		reinforce_num.text = (max_reinforce_num-max_reinforce_num_left+num)+"/"+max_reinforce_num;
	}
	//输出框内容改变 
	void InPutChange()
	{
		int max_num = max_soldier_num>max_reinforce_num_left?max_reinforce_num_left:max_soldier_num;//>(select_hero.nLeader13+select_hero.nLeaderAdd18)?(select_hero.nLeader13+select_hero.nLeaderAdd18):max_soldier_num;
		if(!U3dCmn.IsNumeric(with_num.text) || max_num ==0)
		{
			with_num.text = "0";
			num_slider.sliderValue = 0;
		}
		else
		{
			int num = int.Parse(with_num.text);
			if(num>max_num)
			{
				num = max_num;
				
			}
			
			float slider_num = (float)num/max_num;
			num_slider.sliderValue = slider_num;
			with_num.text = num.ToString();
			reinforce_num.text = (max_reinforce_num-max_reinforce_num_left+num)+"/"+max_reinforce_num;
		}
		
	}
	// 新手引导设置兵数
	void TabInputChange()
	{
		with_num.text = "10";
		InPutChange();
	}
	//初始化可配置的士兵列表 
	void InitialSoldierList()
	{
		int capacity = 0;
		//foreach(DictionaryEntry	de	in	SoldierManager.SoldierMap)  
  		//{
			//if()
		////	SoldierUnit unit = (SoldierUnit)de.Value;
		//	if(unit.nNum3 >0 || select_hero.nArmyNum11 >0)
		//		capacity++;
		//}
		soldier_key_array.Clear();
		
		foreach(DictionaryEntry	de	in	SoldierManager.SoldierMap)  
  		{
			SoldierUnit unit = (SoldierUnit)de.Value;
			//print (unit.nExcelID1+" "+unit.nLevel2+" " +unit.nNum3);
			if((unit.nExcelID1>0 && unit.nNum3 >0))
			{
				soldier_key_array.Add((string)de.Key);
			}
				
		}
		soldier_key_array.Sort();
		capacity = soldier_key_array.Count;
		soldier_storage.SetCapacity(capacity);
		soldier_storage.ResetAllSurfaces();
		List<GameObject> objs = soldier_storage.GetAvailableItems();
		for(int i=0;i<objs.Count;i++)
		{
			UIEventListener.Get(objs[i].gameObject).onClick -= ShowDetail;
			UIEventListener.Get(objs[i].gameObject).onClick += ShowDetail;
			
		}
		int lord_level_limit = U3dCmn.GetCmnDefNum((uint)CMN_DEF.REINFORCE_LEVEL_LIMIT);
		if(CommonData.player_online_info.Level >= lord_level_limit)
		{
			lord_level_limit_label.text = "";
			if(capacity>0)
				reinforcesoldierpanel.SetActiveRecursively(true);
		}
		else
		{
			lord_level_limit_label.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.REINFORCE_LEVEL_LIMIT),lord_level_limit); 
		}
		//soldiergrid.Reposition();
	}
	//选中显示 
	void ShowDetail(GameObject obj)
	{
		
		//select_obj = obj;
		ReinforceSoldierItem item = obj.GetComponent<ReinforceSoldierItem>();
		if(item.excel_id!=0 && item.excel_id >0)
		{
			ClearSelect();
			item.SetSelect(true);
			SetWithSoldierPanelData(item.nowinfo);
		}
		
		if (NewbieJiangLing.NewbiePeiBing == true)
		{
			NewbieJiangLing.NewbiePeiBing = false;
			TabInputChange();
		}
	}
	void ClearSelect()
	{
		List<GameObject> all_item_list = soldier_storage.GetAvailableItems();
		for(int i=0; i<all_item_list.Count;i++)
		{
			ReinforceSoldierItem item = all_item_list[i].GetComponent<ReinforceSoldierItem>();
			
			item.SetSelect(false);
		}
	}
	//显示当前选择士兵详细信息面板 
	void SetWithSoldierPanelData(SoldierUnit soldierinfo)
	{
		select_soldier = soldierinfo;
		
		max_soldier_num = (int)soldierinfo.nNum3;
		num_slider.sliderValue =0;
		with_num.text = "0";
		
		
		
		SoldierMBInfo soldierMB_info =  U3dCmn.GetSoldierInfoFromMb(soldierinfo.nExcelID1,soldierinfo.nLevel2);
		//soldier_label.text = string.Format(soldier_str,soldierinfo.nLevel2,soldierMB_info.Name);
		//soldier_num.text = max_soldier_num.ToString();
		//leader_num.text = (select_hero.nLeader13+select_hero.nLeaderAdd18).ToString();
		
		
	}
	//请求增援 
	void ReqReinforce()
	{
		//print(select_soldier.nExcelID1+" "+select_soldier.nLevel2+" "+int.Parse(with_num.text));
		
		CTS_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER;
		req.nObjID3 = AllyID;
		req.nExcelID4 = select_soldier.nExcelID1;
		req.nLevel5 = select_soldier.nLevel2;
		req.nNum6 = uint.Parse(with_num.text);
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER);
		//select_soldier.
	}
	//请求增援返回  
	void ReqReinforceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T.enum_rst.RST_OK)
		{
			string soldier_key = sub_msg.nExcelID3+"_"+sub_msg.nLevel4;
			if(SoldierManager.SoldierMap.Contains(soldier_key))
			{
				SoldierUnit unit = (SoldierUnit)SoldierManager.SoldierMap[soldier_key];
				if(unit.nNum3 <= sub_msg.nNum5)
				{
					SoldierManager.SoldierMap.Remove(soldier_key);
				}
				else 
				{
					unit.nNum3 -= (int) sub_msg.nNum5;
					SoldierManager.SoldierMap[soldier_key] = unit;
				}
				
			}
			
			//CombatManager.ReqReinforceListData();
			GameObject go = U3dCmn.GetObjFromPrefab("WarSituationWin");
			if (go != null ) {
				
				WarSituationWin win = go.GetComponent<WarSituationWin>();
				if (win != null) 
				{
					win.GetComponentInChildren<UIAnchor>().depthOffset = -1.5f;
					win.WaitingReinforceList();
				}
			}
			
			soldier_key_array.Clear();
			foreach(DictionaryEntry	de	in	SoldierManager.SoldierMap)  
	  		{
				SoldierUnit unit = (SoldierUnit)de.Value;
				//print (unit.nExcelID1+" "+unit.nLevel2+" " +unit.nNum3);
				if((unit.nExcelID1>0 && unit.nNum3 >0))
				{
					//print (unit.nLevel2+" "+unit.nExcelID1+" "+unit.nNum3);
					soldier_key_array.Add((string)de.Key);
				}
				
			}
			DismissPanel();
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T.enum_rst.RST_QUEUE_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.REINFORCE_QUEUE_MAX));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T.enum_rst.RST_TODAY_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.REINFORCE_MAX_NUM));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ALLI_DONATE_SOLDIER_T.enum_rst.RST_ALLY_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLY_BARRACK_CAP_FULL));
		}
	}
}
