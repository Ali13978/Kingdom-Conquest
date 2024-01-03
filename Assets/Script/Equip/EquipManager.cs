using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class EquipManager : MonoBehaviour {
	//更新装备（更换装备 镶嵌石头）的回调注册表 里面包含需要回调的object和回调函数名   
	public static Hashtable CallBackObjMap = new Hashtable();
	//已装备的ITEM列表(包括已经镶嵌的宝石 武将身上的装备)  
	public static Hashtable EquipItemMap = new Hashtable();
	//武将的装备 
	public struct HeroEquipUnit
	{
		//武将ID 
		public ulong HeroID;
		//武器 
		public ulong ARM;
		//头盔 
		public ulong HEAD;
		//护甲 
		public ulong ARMOR;
		//饰品  
		public ulong JEWELERY;
		//鞋子  
		public ulong SHOE;
	}
	//武将和装备的关联表  
	public static Hashtable HeroToEquipMap = new Hashtable();
	//装备上宝石  
	public struct EquipGemUnit
	{
		//装备ID 
		public ulong EquipID;
		//四个插槽 
		public ulong Slot1;
		public ulong Slot2;
		public ulong Slot3;
		public ulong Slot4;
	}
	//装备和宝石的关联表   
	public static Hashtable EquipToGemMap = new Hashtable();
	
	//当前操作的HEROID 
	public static ulong hero_id;
	//记录更换的装备ID 
	public static ulong original_equip_id;
	public static ulong change_equip_id;
	//装备面板ITEM列表  
	public static List<ulong> equip_id_list = new List<ulong>();
	//装备面板宝石ITEM列表  
	public static List<ulong> gem_id_list = new List<ulong>();
	// Use this for initialization
	void Start () {
		ReqDressedEquip();
		ReqEquipedGem();
	}
	
	//注册装备更新回调   
	public static void RegisterCallBack(GameObject obj,string func_name)
	{
		if(!CallBackObjMap.Contains(obj))
		{
			CallBackObjMap.Add(obj,func_name);
		}
		else
		{
			CallBackObjMap[obj] = func_name;
		}
	} 
	//注销装备更新回调  
	public static void UnRegisterCallBack(GameObject obj)
	{
		if(CallBackObjMap.Contains(obj))
		{
			CallBackObjMap.Remove(obj);
		}
	} 
	//请求已经装备到武将的装备信息
	void ReqDressedEquip()
	{
		CTS_GAMECMD_GET_EQUIP_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_EQUIP;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_EQUIP_T >(req);
	}
	//请求已经镶嵌到装备的宝石信息  
	void ReqEquipedGem()
	{
		CTS_GAMECMD_GET_GEM_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_GEM;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_GEM_T >(req);
	}
	//请求已经装备到武将的装备信息返回 
	void ReqDressedEquipRst(byte[] buff)
	{
		STC_GAMECMD_GET_EQUIP_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_EQUIP_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_EQUIP_T.enum_rst.RST_OK)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_EQUIP_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			EquipDesc[] equip_array = (EquipDesc[])DataConvert.ByteToStructArray<EquipDesc>(data_buff,sub_msg.nNum2);
			for(int i=0;i<equip_array.Length;i++)
			{
				ItemDesc item_info;
				item_info.nItemID1 = equip_array[i].nEquipID1;
				item_info.nExcelID2 = equip_array[i].nExcelID3;
				item_info.nNum3 = 1;
				if(!EquipItemMap.Contains(equip_array[i].nEquipID1))
					EquipItemMap.Add(equip_array[i].nEquipID1,item_info);
				//建立将领和装备的关联 
				if(HeroToEquipMap.Contains(equip_array[i].nHeroID4))
				{
					HeroEquipUnit unit = (HeroEquipUnit)HeroToEquipMap[equip_array[i].nHeroID4];
					SetEquipData(ref unit,equip_array[i]);
					HeroToEquipMap[equip_array[i].nHeroID4] = unit;
				}
				else 
				{
					HeroEquipUnit unit = new HeroEquipUnit(); 
					SetEquipData(ref unit,equip_array[i]);
					HeroToEquipMap.Add(equip_array[i].nHeroID4,unit);
				}
			}
		}
		//print ();
	}
	void SetEquipData(ref HeroEquipUnit unit,EquipDesc item_info)
	{
		if(item_info.nEquipType2 == (int)EquipSlot.HEAD)
		{
			unit.HEAD = item_info.nEquipID1;
		}
		else if(item_info.nEquipType2 == (int)EquipSlot.ARMOR)
		{
			unit.ARMOR = item_info.nEquipID1;
		}
		else if(item_info.nEquipType2 == (int)EquipSlot.SHOE)
		{
			unit.SHOE = item_info.nEquipID1;
		}
		else if(item_info.nEquipType2 == (int)EquipSlot.ARM)
		{
			unit.ARM = item_info.nEquipID1;
		}
		else if(item_info.nEquipType2 == (int)EquipSlot.JEWELERY)
		{
			unit.JEWELERY = item_info.nEquipID1;
		}

	}
	//请求已经镶嵌到装备的宝石信息返回   
	void ReqEquipedGemRst(byte[] buff)
	{
		STC_GAMECMD_GET_GEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_GEM_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_GEM_T.enum_rst.RST_OK)
		{	
		
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_GEM_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GemDesc[] gem_array = (GemDesc[])DataConvert.ByteToStructArray<GemDesc>(data_buff,sub_msg.nNum2);
			
			for(int i=0;i<gem_array.Length;i++)
			{
				if(EquipItemMap.Contains(gem_array[i].nGemID5))
				{
					ItemDesc item_info = (ItemDesc)EquipItemMap[gem_array[i].nGemID5];
					item_info.nNum3++;
					EquipItemMap[item_info.nItemID1] = item_info;
				}
				else
				{
					ItemDesc item_info;
					item_info.nItemID1 = gem_array[i].nGemID5;
					item_info.nExcelID2 = gem_array[i].nExcelID4;
					item_info.nNum3 = 1;
					EquipItemMap.Add(gem_array[i].nGemID5,item_info);
				}
				
			
				//建立装备和宝石的关联 
				if(EquipToGemMap.Contains(gem_array[i].nEquipID1))
				{
					EquipGemUnit unit = (EquipGemUnit)EquipToGemMap[gem_array[i].nEquipID1];
					SetGemData(ref unit,gem_array[i]);
					EquipToGemMap[gem_array[i].nEquipID1] = unit;
				}
				else 
				{
					EquipGemUnit unit = new EquipGemUnit(); 
					SetGemData(ref unit,gem_array[i]);
					EquipToGemMap.Add(gem_array[i].nEquipID1,unit);
				}
			}
		}
	}
	void SetGemData(ref EquipGemUnit unit,GemDesc item_info)
	{
		if(item_info.nSlotIdx2 == (int)GemSlot.SLOT1)
		{
			unit.Slot1 = item_info.nGemID5;
		}
		else if(item_info.nSlotIdx2 == (int)GemSlot.SLOT2)
		{
			unit.Slot2 = item_info.nGemID5;
		}
		else if(item_info.nSlotIdx2 == (int)GemSlot.SLOT3)
		{
			unit.Slot3 = item_info.nGemID5;
		}
		else if(item_info.nSlotIdx2 == (int)GemSlot.SLOT4)
		{
			unit.Slot4 = item_info.nGemID5;
		}

	}
	//增加物品 这里是穿着装备 或者镶嵌宝石(同步操作缓存)
	public static void AddItem(ulong item_id,int excel_id,int num)
	{
		if(item_id!=0)
		{
			if(EquipItemMap.Contains(item_id))
			{
				ItemDesc item_info = (ItemDesc)EquipItemMap[item_id];
				item_info.nNum3+=num;
				EquipItemMap[item_id] =item_info;
			}
			else
			{
				ItemDesc item_info ;
				item_info.nItemID1 = item_id;
				item_info.nExcelID2 = excel_id;
				item_info.nNum3 = num;
				EquipItemMap.Add(item_info.nItemID1,item_info);
			}
		}
	}
	//减少物品 这里是卸掉装备或者摘除宝石(同步操作缓存) 
	public static void SubItem(ulong item_id,int num)
	{
	
		if(EquipItemMap.Contains(item_id))
		{
			ItemDesc item_info = (ItemDesc)EquipItemMap[item_id];
			if(item_info.nNum3 > num)
			{
				item_info.nNum3-=num;
				EquipItemMap[item_id] =item_info;
			}
			else
			{
				EquipItemMap.Remove(item_id);
			}
		}
	}
	//改变武将和装备的关联表   
	public static void ChangeHeroToEquipMap(ulong hero_id,int equip_type,ulong equip_id)
	{
		if(HeroToEquipMap.Contains(hero_id))
		{
			
			HeroEquipUnit unit = (HeroEquipUnit)HeroToEquipMap[hero_id];
			if(equip_type == (int)EquipSlot.HEAD)
			{
				unit.HEAD = equip_id;
			}
			else if(equip_type == (int)EquipSlot.ARMOR)
			{
				unit.ARMOR = equip_id;
			}
			else if(equip_type == (int)EquipSlot.SHOE)
			{
				unit.SHOE = equip_id;
			}
			else if(equip_type == (int)EquipSlot.ARM)
			{
				unit.ARM = equip_id;
			}
			else if(equip_type == (int)EquipSlot.JEWELERY)
			{
				unit.JEWELERY = equip_id;
			}
			HeroToEquipMap[hero_id] = unit;
		
		}
		else
		{
			HeroEquipUnit unit = new HeroEquipUnit(); 
			if(equip_type == (int)EquipSlot.HEAD)
			{
				unit.HEAD = equip_id;
			}
			else if(equip_type == (int)EquipSlot.ARMOR)
			{
				unit.ARMOR = equip_id;
			}
			else if(equip_type == (int)EquipSlot.SHOE)
			{
				unit.SHOE = equip_id;
			}
			else if(equip_type == (int)EquipSlot.ARM)
			{
				unit.ARM = equip_id;
			}
			else if(equip_type == (int)EquipSlot.JEWELERY)
			{
				unit.JEWELERY = equip_id;
			}
			HeroToEquipMap.Add(hero_id,unit);
		}
		
	}
	//改变装备和宝石的关联表   
	public static void ChangeEquipToGemMap(ulong equip_id,int slot_id,ulong gem_id)
	{
		
		if(EquipToGemMap.Contains(equip_id))
		{
			
			EquipGemUnit unit = (EquipGemUnit)EquipToGemMap[equip_id];
			if(slot_id == (int)GemSlot.SLOT1)
			{
				unit.Slot1 = gem_id;
			}
			else if(slot_id == (int)GemSlot.SLOT2)
			{
				unit.Slot2 = gem_id;
			}
			else if(slot_id == (int)GemSlot.SLOT3)
			{
				unit.Slot3 = gem_id;
			}
			else if(slot_id == (int)GemSlot.SLOT4)
			{
				unit.Slot4 = gem_id;
			}
			EquipToGemMap[equip_id] = unit;
		
		}
		else
		{
			EquipGemUnit unit = new EquipGemUnit(); 
			if(slot_id == (int)GemSlot.SLOT1)
			{
				unit.Slot1 = gem_id;
			}
			else if(slot_id == (int)GemSlot.SLOT2)
			{
				unit.Slot2 = gem_id;
			}
			else if(slot_id == (int)GemSlot.SLOT3)
			{
				unit.Slot3 = gem_id;
			}
			else if(slot_id == (int)GemSlot.SLOT4)
			{
				unit.Slot4= gem_id;
			}
			EquipToGemMap.Add(equip_id,unit);
		}
		
	}
	//检查同类宝石是否已镶嵌 
	public static  bool CheckEmbed(ulong equip_id,ulong gem_id)
	{
		bool rst = true;
		if(EquipToGemMap.Contains(equip_id))
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[gem_id];
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
			int embed_sort = gem_info.GemSort;
			
			EquipGemUnit unit = (EquipGemUnit)EquipToGemMap[equip_id];
			if(embed_sort == GetEmbedGemSort(unit.Slot1))
				rst = false;
			else if(embed_sort == GetEmbedGemSort(unit.Slot2))
				rst = false;
			else if(embed_sort == GetEmbedGemSort(unit.Slot3))
				rst = false;
			else if(embed_sort == GetEmbedGemSort(unit.Slot4))
				rst = false;
		}
	
		return rst;
	}
	public static  int GetEmbedGemSort(ulong gem_id)
	{
		int gem_sort = 0;
		if(EquipItemMap.Contains(gem_id))
		{
			ItemDesc item_info = (ItemDesc)EquipItemMap[gem_id];
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
			gem_sort = gem_info.GemSort;
		}
		
		return gem_sort;
	}
	public void ReqChangeEquipRst(byte[] buff)
	{
		STC_GAMECMD_REPLACE_EQUIP_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_REPLACE_EQUIP_T>(buff);
		
		if(sub_msg.nRst1 == (int)STC_GAMECMD_REPLACE_EQUIP_T.enum_rst.RST_OK)
		{	
			
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_REPLACE_EQUIP_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			
			HeroDesc msg = DataConvert.ByteToStruct<HeroDesc>(data_buff);
			if(JiangLingManager.MyHeroMap.Contains(hero_id))
			{
				HireHero hero_unit = JiangLingManager.UpdateHeroInfo(hero_id,msg);
				//刷新将领列表页 
				GameObject obj = GameObject.Find("JiangLingInfoWin");
				if(obj!=null)
				{
					obj.SendMessage("SetJiangLingInfoPanelData",hero_unit);
				}
			}
			
			//执行各OBJ的回调函数刷新数据 
			Hashtable callback_map = (Hashtable)CallBackObjMap.Clone();
			foreach(DictionaryEntry	de	in	callback_map)  
	  		{
				GameObject obj = (GameObject)de.Key;
				string func_name = (string)de.Value;
				if(obj!=null && func_name!="")
				{
					obj.SendMessage(func_name);
				}
			}
		}
		
	}
	//根据装备ID获取镶嵌在上面的宝石列表 
	public static List<uint> GetEmbedGem(ulong equip_id)
	{
		List<uint> gem_list = new List<uint>();
		if(EquipManager.EquipToGemMap.Contains(equip_id))
		{
			EquipManager.EquipGemUnit unit =  (EquipManager.EquipGemUnit)EquipManager.EquipToGemMap[equip_id];
			if(unit.Slot1!=0)
			{
				ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[unit.Slot1];
				gem_list.Add((uint)item_info.nExcelID2);
			}
			if(unit.Slot2!=0)
			{
				ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[unit.Slot2];
				gem_list.Add((uint)item_info.nExcelID2);
			}
			if(unit.Slot3!=0)
			{
				ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[unit.Slot3];
				gem_list.Add((uint)item_info.nExcelID2);
			}
			if(unit.Slot4!=0)
			{
				ItemDesc item_info = (ItemDesc)EquipManager.EquipItemMap[unit.Slot4];
				gem_list.Add((uint)item_info.nExcelID2);
			}
		}
		return gem_list;	
	}
	//根据装备ID获取镶嵌在上面的宝石数量  
	public static int GetEmbedGemNum(ulong equip_id)
	{
		
		int num =0;
		if(EquipManager.EquipToGemMap.Contains(equip_id))
		{
			EquipManager.EquipGemUnit unit =  (EquipManager.EquipGemUnit)EquipManager.EquipToGemMap[equip_id];
			if(unit.Slot1!=0)
				num++;
			if(unit.Slot2!=0)
				num++;
			if(unit.Slot3!=0)
				num++;
			if(unit.Slot4!=0)
				num++;
		}
		return num;	
	}
}
