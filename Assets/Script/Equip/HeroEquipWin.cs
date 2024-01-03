using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class HeroEquipWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	HireHero now_hero;
	//将领基本信息 
	public UISprite hero_icon;
	public UILabel hero_name;
	public UILabel attack_label;
	public UILabel defense_label;
	public UILabel life_label;
	public UILabel leader_label;
	//将领五件装备 
	public HeroEquipItem equip_head;
	public HeroEquipItem equip_armor;
	public HeroEquipItem equip_shoe;
	public HeroEquipItem equip_arm;
	public HeroEquipItem equip_jewelery;
	//选中的装备 
	HeroEquipItem select_equip_item;
	//三个按钮  
	public UIImageButton ViewBtn;
	public UIImageButton RemovalBtn;
	public UIImageButton EmbedBtn;
	//可用的装备grid管理器  
	public PagingStorage equip_paging_storage;
	public PagingDraggablePanel equip_panel;
	//没有装备时显露的商城按钮 
	public UIImageButton store_btn;
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
	void RevealPanel(HireHero hero_unit)
	{
		select_equip_item =null;
		InitialPanelData(hero_unit);
		tween_position.Play(true);
	}
	//初始化数据 
	void InitialPanelData(HireHero hero_unit)
	{
		//备份数据 
		
		now_hero = hero_unit;
		hero_name.text = DataConvert.BytesToStr(hero_unit.szName3);
		if(hero_unit.nAttackAdd15>0)
			attack_label.text = (hero_unit.nAttack5).ToString()+"[009600](+"+hero_unit.nAttackAdd15+")";
		else 
			attack_label.text = (hero_unit.nAttack5).ToString();
		if(hero_unit.nDefenseAdd16>0)
			defense_label.text = (hero_unit.nDefense6).ToString()+"[009600](+"+hero_unit.nDefenseAdd16+")";
		else
			defense_label.text = (hero_unit.nDefense6).ToString();
		if(hero_unit.nHealthAdd17>0)
			life_label.text = (hero_unit.nHealth7).ToString()+"[009600](+"+hero_unit.nHealthAdd17+")";
		else
			life_label.text = (hero_unit.nHealth7).ToString();
		if(hero_unit.nLeaderAdd18>0)
			leader_label.text = (hero_unit.nLeader13).ToString()+"[009600](+"+hero_unit.nLeaderAdd18+")";
		else 
			leader_label.text = (hero_unit.nLeader13).ToString();
		hero_icon.spriteName = U3dCmn.GetHeroIconName(hero_unit.nModel12);
		//初始化五件装备的信息 
		if(EquipManager.HeroToEquipMap.Contains((ulong)hero_unit.nHeroID1))
		{
			EquipManager.HeroEquipUnit unit = (EquipManager.HeroEquipUnit)EquipManager.HeroToEquipMap[(ulong)hero_unit.nHeroID1];
			equip_head.equip_id = unit.HEAD;
			equip_armor.equip_id = unit.ARMOR;
			equip_shoe.equip_id = unit.SHOE;
			equip_arm.equip_id = unit.ARM;
			equip_jewelery.equip_id = unit.JEWELERY;
			
		}
		else
		{
			equip_head.equip_id = 0;
			equip_armor.equip_id = 0;
			equip_shoe.equip_id = 0;
			equip_arm.equip_id = 0;
			equip_jewelery.equip_id = 0;
		}
		equip_head.InitialData();
		UIEventListener.Get(equip_head.gameObject).onClick -= SelectMyEquip;
		UIEventListener.Get(equip_head.gameObject).onClick += SelectMyEquip;
		
		equip_armor.InitialData();
		UIEventListener.Get(equip_armor.gameObject).onClick -= SelectMyEquip;
		UIEventListener.Get(equip_armor.gameObject).onClick += SelectMyEquip;
		
		equip_shoe.InitialData();
		UIEventListener.Get(equip_shoe.gameObject).onClick -= SelectMyEquip;
		UIEventListener.Get(equip_shoe.gameObject).onClick += SelectMyEquip;
		
		equip_arm.InitialData();
		UIEventListener.Get(equip_arm.gameObject).onClick -= SelectMyEquip;
		UIEventListener.Get(equip_arm.gameObject).onClick += SelectMyEquip;
		
		equip_jewelery.InitialData();
		UIEventListener.Get(equip_jewelery.gameObject).onClick -= SelectMyEquip;
		UIEventListener.Get(equip_jewelery.gameObject).onClick += SelectMyEquip;
		
		//ViewBtn.target.spriteName = "transparent";
		//RemovalBtn.target.spriteName = "transparent";
		//EmbedBtn.target.spriteName = "transparent";
		//ViewBtn.collider.enabled = false;
		//RemovalBtn.collider.enabled = false;
 		//EmbedBtn.collider.enabled = false;
		ViewBtn.gameObject.SetActiveRecursively(false);
		RemovalBtn.gameObject.SetActiveRecursively(false);
		EmbedBtn.gameObject.SetActiveRecursively(false);
		
		RefreshPanelData(); 
	}
	//只是刷新武将四围基本信息 
	void RefreshHeroBasicData(HireHero hero_unit)
	{
		hero_name.text = DataConvert.BytesToStr(hero_unit.szName3);
		if(hero_unit.nAttackAdd15>0)
			attack_label.text = hero_unit.nAttack5+"[009600](+"+hero_unit.nAttackAdd15+")";
		else 
			attack_label.text = hero_unit.nAttack5.ToString();
		if(hero_unit.nDefenseAdd16>0)
			defense_label.text = hero_unit.nDefense6+"[009600](+"+hero_unit.nDefenseAdd16+")";
		else
			defense_label.text = hero_unit.nDefense6.ToString();
		if(hero_unit.nHealthAdd17>0)
			life_label.text = hero_unit.nHealth7+"[009600](+"+hero_unit.nHealthAdd17+")";
		else
			life_label.text = hero_unit.nHealth7.ToString();
		if(hero_unit.nLeaderAdd18>0)
			leader_label.text = hero_unit.nLeader13+"[009600](+"+hero_unit.nLeaderAdd18+")";
		else 
			leader_label.text = hero_unit.nLeader13.ToString();
		
		hero_icon.spriteName = U3dCmn.GetHeroIconName(hero_unit.nModel12);
		//刷新将领列表页 
		GameObject obj = GameObject.Find("JiangLingInfoWin");
		if(obj!=null)
		{
			obj.SendMessage("SetJiangLingInfoPanelData",hero_unit);
		}
	}
	//刷新可用装备面板数据 
	void RefreshPanelData() 
	{
		EquipManager.equip_id_list.Clear();
		//初始化仓库里可用的装备列表 
		for(int i=0;i<CangKuManager.Item_Id_List.Count;i++)
		{
			
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.Item_Id_List[i]];
			if((uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2] == (uint)ItemSort.EQUIP)
			{
				EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
				if(select_equip_item!=null && select_equip_item.slot_id != 0)
				{
					
					if(equip_info.EuipSlot == select_equip_item.slot_id)
					{
						EquipManager.equip_id_list.Add(item_info.nItemID1);
					}
				}
				else
				{
					EquipManager.equip_id_list.Add(item_info.nItemID1);
				
				}
			}
		}
		//初始化装备面板   
		int capacity = EquipManager.equip_id_list.Count;
		if(capacity == 0)
		{
			store_btn.gameObject.SetActiveRecursively(true);
		}
		else 
		{
			store_btn.gameObject.SetActiveRecursively(false);
		}
		equip_paging_storage.SetCapacity(capacity);
		List<GameObject> equip_item_list = equip_paging_storage.GetAvailableItems();
		for(int i=0;i<equip_item_list.Count;i++)
		{
			UIEventListener.Get(equip_item_list[i].gameObject).onClick -= DressEquip;
			UIEventListener.Get(equip_item_list[i].gameObject).onClick += DressEquip;
		}
	
		equip_paging_storage.ResetAllSurfaces1(0);
		equip_panel.RestrictVisibleWithinBounds();
	}
	
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//下一个武将 
	void NextHero()
	{
		HireHero[] herolist = new HireHero[JiangLingManager.MyHeroMap.Count];
		JiangLingManager.MyHeroMap.Values.CopyTo(herolist,0);
		for(int i=0;i<herolist.Length;i++)
		{
			if(now_hero.nHeroID1 == herolist[i].nHeroID1)
			{
				int index = (i+1)%herolist.Length;
				now_hero = herolist[index];
				InitialPanelData(now_hero);
				return;
			}
		}
		
	}
	//上一个武将 
	void PrevHero()
	{
		HireHero[] herolist = new HireHero[JiangLingManager.MyHeroMap.Count];
		JiangLingManager.MyHeroMap.Values.CopyTo(herolist,0);
		for(int i=0;i<herolist.Length;i++)
		{
			if(now_hero.nHeroID1 == herolist[i].nHeroID1)
			{
				int index =0;
				if(i==0)
					index = herolist.Length-1;
				else 
					index= i-1;
				now_hero = herolist[index];
				InitialPanelData(now_hero);
				return;
			}
		}
	}
	void ClearSelect()
	{
		equip_head.SetSelect(false);
		equip_armor.SetSelect(false);
		equip_shoe.SetSelect(false);
		equip_arm.SetSelect(false);
		equip_jewelery.SetSelect(false);
	}
	//选中我的装备  
	void SelectMyEquip(GameObject obj)
	{
		ClearSelect();
		HeroEquipItem item = obj.GetComponent<HeroEquipItem>();
		if(item.equip_id != 0)
		{
			//ViewBtn.target.spriteName = "select_normal";
			//RemovalBtn.target.spriteName = "select_normal";
			//EmbedBtn.target.spriteName = "select_normal";
			ViewBtn.gameObject.SetActiveRecursively(true);
			RemovalBtn.gameObject.SetActiveRecursively(true);
			EmbedBtn.gameObject.SetActiveRecursively(true);
			//ViewBtn.collider.enabled = true;
			//RemovalBtn.collider.enabled = true;
	 		//EmbedBtn.collider.enabled = true;
		}
		else
		{
			ViewBtn.gameObject.SetActiveRecursively(false);
			RemovalBtn.gameObject.SetActiveRecursively(false);
			EmbedBtn.gameObject.SetActiveRecursively(false);
		}
		item.SetSelect(true);
		select_equip_item = item;
		RefreshPanelData();
	}
	//穿着或者更换装备  
	void DressEquip(GameObject obj)
	{
		CangKuEquipItem item = obj.GetComponent<CangKuEquipItem>();
		if(item.equip_id != 0)
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[item.equip_id];
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
			if(CommonData.APP_VERSION == APP_VERSION_TYPE.TINYKINGDOM)
			{
				if(equip_info.NeedLevel > now_hero.nLevel19)
				{
					U3dCmn.ShowWarnWindow(string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.EQUIP_LIMIT_HERO_LEVEL),equip_info.NeedLevel)); 
					return;
				}
			}
			
			ulong select_equip_id = 0;
			if(equip_info.EuipSlot == (int)EquipSlot.HEAD)
			{
				select_equip_id = equip_head.equip_id;
				select_equip_item = equip_head;
			}
			else if(equip_info.EuipSlot == (int)EquipSlot.ARMOR)
			{
				select_equip_id = equip_armor.equip_id;
				select_equip_item = equip_armor;
			}
			else if(equip_info.EuipSlot == (int)EquipSlot.SHOE)
			{
				select_equip_id = equip_shoe.equip_id;
				select_equip_item = equip_shoe;
			}
			else if(equip_info.EuipSlot == (int)EquipSlot.ARM)
			{
				select_equip_id = equip_arm.equip_id;
				select_equip_item = equip_arm;
			}
			else if(equip_info.EuipSlot == (int)EquipSlot.JEWELERY)
			{
				select_equip_id = equip_jewelery.equip_id;
				select_equip_item = equip_jewelery;
			}
			if(select_equip_id ==0)//没有穿着此装备 
			{
				MyEquipInfoWin infowin = U3dCmn.GetObjFromPrefab("MyEquipInfoWin").GetComponent<MyEquipInfoWin>();
				if(infowin != null)
				{
					infowin.only_show = false;
					infowin.my_equip_id = item.equip_id;
					infowin.SendMessage("RevealPanel");
				}
			}
			else 
			{
				ChangeEquipWin infowin = U3dCmn.GetObjFromPrefab("ChangeEquipWin").GetComponent<ChangeEquipWin>();
				if(infowin != null)
				{
					infowin.change_equip_id = item.equip_id;
					infowin.original_equip_id = select_equip_id;
					infowin.SendMessage("RevealPanel");
				}
			}
		}
			
	}
	//请求更换装备 
	void ReqChangeEquip(ulong equip_id)
	{
		if(now_hero.nStatus14 != (int)HeroState.NORMAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
			return ;
		}
		ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[equip_id];
		EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		
		CTS_GAMECMD_REPLACE_EQUIP_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_REPLACE_EQUIP;
		req.nHeroID3 = (ulong)now_hero.nHeroID1;
		req.nEquipType4 = equip_info.EuipSlot;
		req.nItemID5 = equip_id;
		//req.
		TcpMsger.SendLogicData<CTS_GAMECMD_REPLACE_EQUIP_T >(req);
		EquipManager.hero_id = now_hero.nHeroID1;
		EquipManager.change_equip_id = equip_id;
		EquipManager.original_equip_id = select_equip_item.equip_id;
		EquipManager.RegisterCallBack(gameObject,"ReqChangeEquipRst");
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_REPLACE_EQUIP);
	}
	//更换装备刷新界面 
	void ReqChangeEquipRst()
	{	
		
		ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[EquipManager.change_equip_id];
		EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		//添加到武将的装备表 
		EquipManager.AddItem(EquipManager.change_equip_id,(int)equip_info.ExcelID,1);
		//把更换下来的放回仓库的装备表  
		CangKuManager.AddItem(select_equip_item.equip_id,select_equip_item.excel_id,1);
	
		//在武将的装备表中删除 
		EquipManager.SubItem(select_equip_item.equip_id,1);
		//把更换上的在仓库的装备表中删除  
		CangKuManager.SubItem(EquipManager.change_equip_id,1);
		
		//改变武将和装备的关联表 
		EquipManager.ChangeHeroToEquipMap((ulong)now_hero.nHeroID1,equip_info.EuipSlot,EquipManager.change_equip_id);
		
		//刷新可用的装备列表 
		RefreshPanelData();
		
		now_hero = (HireHero)JiangLingManager.MyHeroMap[now_hero.nHeroID1];
		select_equip_item = null;
		InitialPanelData(now_hero);
		
		EquipManager.UnRegisterCallBack(gameObject);
		EquipManager.change_equip_id = 0;
		EquipManager.original_equip_id = 0;
		
		// 穿装备的界面隐藏 ....
		U3dCmn.SendMessage("MyEquipInfoWin","DismissPanel",null);
		
		// <新手教程> ... 
		if (NewbieJiangLing.processSetEquipRst != null)
		{
			NewbieJiangLing.processSetEquipRst();
			NewbieJiangLing.processSetEquipRst = null;
		}
	}
	//查看装备 
	void ShowEquipInfo()
	{
		if(select_equip_item!=null && select_equip_item.equip_id!= 0)
		{
			MyEquipInfoWin infowin = U3dCmn.GetObjFromPrefab("MyEquipInfoWin").GetComponent<MyEquipInfoWin>();
			if(infowin != null)
			{
				infowin.only_show = true;
				infowin.my_equip_id = select_equip_item.equip_id;
				infowin.SendMessage("RevealPanel");
			}
		}
	}
	//卸载装备 
	void ReqRemovalEquip()
	{
		if(now_hero.nStatus14 != (int)HeroState.NORMAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
			return ;
		}
		if(select_equip_item!=null && select_equip_item.equip_id!= 0)
		{
			ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[select_equip_item.equip_id];
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
			
			CTS_GAMECMD_REPLACE_EQUIP_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_REPLACE_EQUIP;
			req.nHeroID3 = (ulong)now_hero.nHeroID1;
			req.nEquipType4 = equip_info.EuipSlot;
			req.nItemID5 = 0;
			
			TcpMsger.SendLogicData<CTS_GAMECMD_REPLACE_EQUIP_T >(req);
			EquipManager.hero_id = now_hero.nHeroID1;
			EquipManager.change_equip_id = 0;
			EquipManager.original_equip_id = select_equip_item.equip_id;
			EquipManager.RegisterCallBack(gameObject,"ReqRemovalEquipRst");
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_REPLACE_EQUIP);
		}
	}
	//卸载装备刷新界面 
	void ReqRemovalEquipRst()
	{
		ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[select_equip_item.equip_id];
		EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		
		//把更换下来的放回仓库的装备表  
		CangKuManager.AddItem(select_equip_item.equip_id,select_equip_item.excel_id,1);
		//在武将的装备表中删除 
		EquipManager.SubItem(select_equip_item.equip_id,1);
		
		//改变武将和装备的关联表 
		EquipManager.ChangeHeroToEquipMap((ulong)now_hero.nHeroID1,equip_info.EuipSlot,0);
		select_equip_item.equip_id = 0;
		select_equip_item = null;
		//刷新可用的装备列表 
		RefreshPanelData();
		
		now_hero = (HireHero)JiangLingManager.MyHeroMap[now_hero.nHeroID1];
		
		//select_equip_item.InitialData();
		InitialPanelData(now_hero);
		
		EquipManager.UnRegisterCallBack(gameObject);
		EquipManager.change_equip_id = 0;
		EquipManager.original_equip_id = 0;
	}
	//镶嵌宝石  
	void OpenEmbedGemWin()
	{
		EmbedGemWin infowin = U3dCmn.GetObjFromPrefab("EmbedGemWin").GetComponent<EmbedGemWin>();
		if(infowin != null)
		{
			infowin.hero_info = now_hero;
			infowin.equip_id = select_equip_item.equip_id;
			infowin.SendMessage("RevealPanel");
		}
	}
	//打开商城界面 
	void OpenStore()
	{
		GameObject win = U3dCmn.GetObjFromPrefab("StoreWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel");
		}	
	}
	
	// (新手引导) ...
	void DressEqiupForNewbie()
	{
		List<GameObject> equip_item_list = equip_paging_storage.GetAvailableItems();
		if (equip_item_list.Count == 0) return;
		GameObject go1 = equip_item_list[0];
		DressEquip(go1);
	}
}
