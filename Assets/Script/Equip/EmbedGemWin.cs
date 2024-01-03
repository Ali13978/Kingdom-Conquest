using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class EmbedGemWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public HireHero 		hero_info;
	public ulong 			equip_id;
	//装备信息 
	public UISprite equip_icon;
	public UILabel equip_name;
	public UILabel equip_property;
	public UILabel gem_num_label;
	public UILabel gem_info;
	public EquipGemItem Gem1;
	public EquipGemItem Gem2;
	public EquipGemItem Gem3;
	public EquipGemItem Gem4;
	//选中的宝石插槽 
	public static int gem_slot;
	//选择镶嵌的宝石id 
	ulong select_gem_id =0;
	string gem_num_str;
	
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
		gem_num_str = gem_num_label.text;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	//弹出窗口   
	void RevealPanel()
	{
		InitialPanelData();
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//初始化数据 
	void InitialPanelData()
	{
		ItemDesc item_info = new ItemDesc();
		if(EquipManager.EquipItemMap.Contains(equip_id))
		{
			item_info = (ItemDesc)EquipManager.EquipItemMap[equip_id];
		}
		else if(CangKuManager.CangKuItemMap.Contains(equip_id))
		{
			item_info = (ItemDesc)CangKuManager.CangKuItemMap[equip_id];
		}
		else 
			return;
		EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		equip_icon.spriteName = equip_info.IconName;
		equip_name.text = equip_info.EquipName;
		equip_property.text = U3dCmn.GetTipByExcelID((uint)item_info.nExcelID2);
		List<uint>gem_list = EquipManager.GetEmbedGem(equip_id);
		gem_num_label.text = gem_num_str+" "+gem_list.Count+"/4";
		gem_info.text = "";
		for(int i=0;i<gem_list.Count;i++)
		{
			gem_info.text +=U3dCmn.GetTipByExcelID(gem_list[i])+"\n";
		}
		if(EquipManager.EquipToGemMap.Contains(equip_id))
		{
			EquipManager.EquipGemUnit unit = (EquipManager.EquipGemUnit)EquipManager.EquipToGemMap[equip_id];
			Gem1.gem_id = unit.Slot1;
			Gem2.gem_id = unit.Slot2;
			Gem3.gem_id = unit.Slot3;
			Gem4.gem_id = unit.Slot4;
		}
		else
		{
			Gem1.gem_id = 0;
			Gem2.gem_id = 0;
			Gem3.gem_id = 0;
			Gem4.gem_id = 0;
		}
		Gem1.InitialData();
		Gem2.InitialData();
		Gem3.InitialData();
		Gem4.InitialData();
	}

	
	
	//请求镶嵌宝石  
	void ReqEmbedGem(ulong gem_id)
	{
		if(hero_info.nStatus14 != (int)HeroState.NORMAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
			return ;
		}
		if(gem_id!=0)
		{
			if(EquipManager.CheckEmbed(equip_id,gem_id))
			{
				ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[gem_id];
				GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
				CTS_GAMECMD_MOUNT_ITEM_T req;
				req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
				req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_MOUNT_ITEM;
				req.nHeroID3 = hero_info.nHeroID1;
				req.nEquipID4 = equip_id;
				req.nSlotIdx5 = gem_slot;
				req.nExcelID6 = (int)gem_info.ExcelID;
				TcpMsger.SendLogicData<CTS_GAMECMD_MOUNT_ITEM_T >(req);
				LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_MOUNT_ITEM);
				select_gem_id = gem_id;
			}
			else 
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HAVE_EMBED_THISGEM));
			}
		}
		
	}
	//请求镶嵌宝石返回 
	void ReqEmbedGemRst(byte[] buff)
	{
		
		STC_GAMECMD_MOUNT_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_MOUNT_ITEM_T>(buff);
		
		if(sub_msg.nRst1 == (int)STC_GAMECMD_MOUNT_ITEM_T.enum_rst.RST_OK)
		{	
			
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_MOUNT_ITEM_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			
			HeroDesc msg = DataConvert.ByteToStruct<HeroDesc>(data_buff);
			if(JiangLingManager.MyHeroMap.Contains(hero_info.nHeroID1))
			{
				HireHero hero_unit = JiangLingManager.UpdateHeroInfo(hero_info.nHeroID1,msg);
				
				//刷新将领装备页面  
				GameObject obj = GameObject.Find("HeroEquipWin");
				if(obj!=null)
				{
					obj.SendMessage("RefreshHeroBasicData",hero_unit);
				}
			
			}
			
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[select_gem_id];
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
		
			//添加到装备的宝石表  
			EquipManager.AddItem(select_gem_id,(int)gem_info.ExcelID,1);
			//把更换上的宝石在仓库中删除  
			CangKuManager.SubItem(select_gem_id,1);
			//改变武将和装备的关联表 
			EquipManager.ChangeEquipToGemMap(equip_id,gem_slot,select_gem_id);
			gem_slot = 0;
			select_gem_id = 0;
			//刷新界面 
			InitialPanelData();
			U3dCmn.SendMessage("CangKuWin","RefreshPanelData",null);
		}
		else 
		{
			U3dCmn.ShowWarnWindow("unknown err");
		}
		
	}
	//请求摘除宝石   
	void ReqRemovalGem(ulong gem_id)
	{
		if(hero_info.nStatus14 != (int)HeroState.NORMAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_NOT_FREE)); 
			return ;
		}
		ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[gem_id];
		GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
		CTS_GAMECMD_UNMOUNT_ITEM_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_UNMOUNT_ITEM;
		req.nHeroID3 = hero_info.nHeroID1;
		req.nEquipID4 = equip_id;
		req.nSlotIdx5 = gem_slot;
		TcpMsger.SendLogicData<CTS_GAMECMD_UNMOUNT_ITEM_T >(req);
		select_gem_id = gem_id;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_UNMOUNT_ITEM);
	}
	//请求摘除宝石返回    
	void ReqRemovalGemRst(byte[] buff)
	{
		STC_GAMECMD_UNMOUNT_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_UNMOUNT_ITEM_T>(buff);
		
		if(sub_msg.nRst1 == (int)STC_GAMECMD_UNMOUNT_ITEM_T.enum_rst.RST_OK)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_UNMOUNT_ITEM_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			
			HeroDesc msg = DataConvert.ByteToStruct<HeroDesc>(data_buff);
			if(JiangLingManager.MyHeroMap.Contains(hero_info.nHeroID1))
			{
				HireHero hero_unit = JiangLingManager.UpdateHeroInfo(hero_info.nHeroID1,msg);
				//刷新将领装备页面  
				GameObject obj = GameObject.Find("HeroEquipWin");
				if(obj!=null)
				{
					obj.SendMessage("RefreshHeroBasicData",hero_unit);
				}
			
			}
			else 
			{
				
			}
			ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[select_gem_id];
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
		
			//从装备的宝石表中删除   
			EquipManager.SubItem(select_gem_id,1);
			//返回仓库中   
			CangKuManager.AddItem(select_gem_id,(int)gem_info.ExcelID,1);
			//改变武将和装备的关联表 
			EquipManager.ChangeEquipToGemMap(equip_id,gem_slot,0);
			gem_slot = 0;
			select_gem_id = 0;
			//刷新界面 
			InitialPanelData();
			U3dCmn.SendMessage("CangKuWin","RefreshPanelData",null);
		}
		else 
		{
			U3dCmn.ShowWarnWindow("unknown err");
		}
	}
}
