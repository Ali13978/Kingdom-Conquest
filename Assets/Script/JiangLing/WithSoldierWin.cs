using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class WithSoldierWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	//将领配兵 兵种列表 
	public PagingStorage soldier_storage;
	//将领配兵面板 
	public GameObject withsoldierpanel;
	public UILabel  soldier_num;
	public UILabel  leader_num;
	public UISlider num_slider;
	public UIInput  with_num;
	public UILabel  soldier_label;
	//可以配置的士兵种类列表 
	public static ArrayList soldier_key_array = new ArrayList();
	
	//当前选择的将领 
	public static HireHero select_hero;
	//当前配置的兵种 
	SoldierUnit select_soldier;
	int max_soldier_num; 
	string soldier_str;
	
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
		soldier_str = soldier_label.text;
		soldier_label.text = "";
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	//弹出窗口 
	void RevealPanel(HireHero heroinfo)
	{
		select_hero = heroinfo;
		withsoldierpanel.SetActiveRecursively(false);
		InitialSoldierList();
		tween_position.Play(true);
		with_num.text = "0";
		num_slider.sliderValue =0;
		max_soldier_num = 0;
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
		int max_num = max_soldier_num>(select_hero.nLeader13+select_hero.nLeaderAdd18)?(select_hero.nLeader13+select_hero.nLeaderAdd18):max_soldier_num;
		int num = (int)(num_slider.sliderValue*max_num);
		
		with_num.text = num.ToString();
	}
	//输出框内容改变 
	void InPutChange()
	{
		int max_num = max_soldier_num>(select_hero.nLeader13+select_hero.nLeaderAdd18)?(select_hero.nLeader13+select_hero.nLeaderAdd18):max_soldier_num;
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
		
		if(select_hero.nArmyNum11 >0)
		{
			string key = select_hero.nArmyType9+"_"+select_hero.nArmyLevel10;
			if(!SoldierManager.SoldierMap.Contains(key))
			{
				
				SoldierUnit unit =  new SoldierUnit();
				unit.nExcelID1 = (uint)select_hero.nArmyType9;
				unit.nLevel2 = (uint)select_hero.nArmyLevel10;
				unit.nNum3 = 0;
				SoldierManager.SoldierMap.Add(key,unit);
			}
		}
		foreach(DictionaryEntry	de	in	SoldierManager.SoldierMap)  
  		{
			SoldierUnit unit = (SoldierUnit)de.Value;
			//print (unit.nExcelID1+" "+unit.nLevel2+" " +unit.nNum3);
			if((unit.nExcelID1>0 && unit.nNum3 >0) || ((uint)select_hero.nArmyType9 == unit.nExcelID1 && (uint)select_hero.nArmyLevel10 == unit.nLevel2 && select_hero.nArmyNum11>0))
			{
			//	print (unit.nLevel2+" "+unit.nExcelID1);
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
		if(capacity>0)
			withsoldierpanel.SetActiveRecursively(true);
		//soldiergrid.Reposition();
	}
	//选中显示 
	void ShowDetail(GameObject obj)
	{
		
		//select_obj = obj;
		SoldierIcon item = obj.GetComponent<SoldierIcon>();
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
			SoldierIcon item = all_item_list[i].GetComponent<SoldierIcon>();
			
			item.SetSelect(false);
		}
	}
	//显示当前选择士兵详细信息面板 
	void SetWithSoldierPanelData(SoldierUnit soldierinfo)
	{
		select_soldier = soldierinfo;
		if(select_hero.nArmyType9==soldierinfo.nExcelID1 && select_hero.nArmyLevel10 == soldierinfo.nLevel2)
		{
			
			max_soldier_num = (int)soldierinfo.nNum3+ select_hero.nArmyNum11;
			int max_num = max_soldier_num>(select_hero.nLeader13+select_hero.nLeaderAdd18)?(select_hero.nLeader13+select_hero.nLeaderAdd18):max_soldier_num;
			float slider_num = (float)select_hero.nArmyNum11/(float)max_num;
			num_slider.sliderValue = slider_num;
			with_num.text = select_hero.nArmyNum11.ToString();
		}
		else 
		{
			max_soldier_num = (int)soldierinfo.nNum3;
			num_slider.sliderValue =0;
			with_num.text = "0";
		}
		
		
		SoldierMBInfo soldierMB_info =  U3dCmn.GetSoldierInfoFromMb(soldierinfo.nExcelID1,soldierinfo.nLevel2);
		soldier_label.text = string.Format(soldier_str,soldierinfo.nLevel2,soldierMB_info.Name);
		soldier_num.text = max_soldier_num.ToString();
		leader_num.text = (select_hero.nLeader13+select_hero.nLeaderAdd18).ToString();
		
		
	}
	//请求保存配置士兵数据 
	void ReqSaveWithSoldierData()
	{
		JiangLingManager.CallBackObj = gameObject;
		CTS_GAMECMD_OPERATE_CONFIG_HERO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CONFIG_HERO;
		req.nHeroID3 = (ulong)select_hero.nHeroID1;
		req.nArmyType4 = (int)select_soldier.nExcelID1;
		req.nArmyLevel5 = (int)select_soldier.nLevel2;
		req.nArmyNum6 = int.Parse(with_num.text);
		//req.nSlotID3
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CONFIG_HERO_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONFIG_HERO);
	}
	//解散士兵 
	void ReqDismissSoldier()
	{
		JiangLingManager.CallBackObj = gameObject;
		CTS_GAMECMD_OPERATE_CONFIG_HERO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CONFIG_HERO;
		req.nHeroID3 = (ulong)select_hero.nHeroID1;
		req.nArmyType4 = 0;
		req.nArmyLevel5 = 0;
		req.nArmyNum6 = 0;
		//req.nSlotID3
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CONFIG_HERO_T >(req);
		select_soldier = new SoldierUnit();
		with_num.text = "0";
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONFIG_HERO);
	}
	//保存配置士兵数据返回 
	void SaveWithSoldierDataRst(STC_GAMECMD_OPERATE_CONFIG_HERO_T sub_msg)
	{
		JiangLingManager.CallBackObj = null;
		if(sub_msg.nRst1 ==(int)STC_GAMECMD_OPERATE_CONFIG_HERO_T.enum_rst.RST_OK)
		{
			if(SoldierManager.SoldierMap.Contains(select_hero.nArmyType9+"_"+select_hero.nArmyLevel10))
			{
				SoldierUnit soldierinfo = (SoldierUnit)SoldierManager.SoldierMap[select_hero.nArmyType9+"_"+select_hero.nArmyLevel10];
				soldierinfo.nNum3 += select_hero.nArmyNum11;
				SoldierManager.SoldierMap[select_hero.nArmyType9+"_"+select_hero.nArmyLevel10] = soldierinfo;
			}
			else if(select_hero.nArmyType9>0 && select_hero.nArmyLevel10>0 && select_hero.nArmyNum11>0)
			{
				SoldierUnit soldierinfo;
				soldierinfo.nExcelID1 = (uint)select_hero.nArmyType9;
				soldierinfo.nLevel2 = (uint)select_hero.nArmyLevel10;
				soldierinfo.nNum3 = select_hero.nArmyNum11;
		
				SoldierManager.SoldierMap.Add(select_hero.nArmyType9+"_"+select_hero.nArmyLevel10,soldierinfo);
			}
			if(select_soldier.nExcelID1>0 && SoldierManager.SoldierMap.Contains(select_soldier.nExcelID1+"_"+select_soldier.nLevel2))
			{
				SoldierUnit soldierinfo = (SoldierUnit)SoldierManager.SoldierMap[select_soldier.nExcelID1+"_"+select_soldier.nLevel2];
				soldierinfo.nNum3 -= int.Parse(with_num.text);
				SoldierManager.SoldierMap[select_soldier.nExcelID1+"_"+select_soldier.nLevel2] = soldierinfo;
			}
			HireHero heroinfo = (HireHero)JiangLingManager.MyHeroMap[select_hero.nHeroID1];
			heroinfo.nArmyType9 = (int)select_soldier.nExcelID1;
			heroinfo.nArmyLevel10 =(int)select_soldier.nLevel2;
			heroinfo.nArmyNum11 = int.Parse(with_num.text);
			heroinfo.nProf20 = sub_msg.nProf2; 
			JiangLingManager.MyHeroMap[select_hero.nHeroID1] = heroinfo;
			
			
			//刷新将领界面 
			GameObject jianglingobj =GameObject.Find("JiangLingInfoWin");
			if(jianglingobj != null)
				jianglingobj.SendMessage("SetJiangLingInfoPanelData",heroinfo);
			DismissPanel();
			
			//InitialSoldierList();
			// < 新手引导 > ... 配置士兵成功返回 ....
			if (NewbieJiangLing.processSaveSoldierRst != null)
			{
				NewbieJiangLing.processSaveSoldierRst();
				NewbieJiangLing.processSaveSoldierRst = null;
			}
		}
		else
		{
			DismissPanel();
		}
		
		
		// <新手引导> ... 清空 ....
		NewbieJiangLing.processSaveSoldierRst = null;
	}
}
