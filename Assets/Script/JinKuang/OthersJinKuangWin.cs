using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class OthersJinKuangWin : MonoBehaviour {
	//金矿窗口 
	public TweenPosition 	tween_position;
	//角色头像 
	public UISprite head_icon;
	//角色名字 
	public UILabel	char_name_label;
	//角色等级 
	public UILabel	char_level_label;
	//摇钱树 
	public OthersYaoQianShu yaoqianshu;
	
	//金矿数量 
	const int JinKuangNum=8;
	//金矿哈希表 主键为AUTO_ID  
	Hashtable  JinKuang_Map = new Hashtable();
	//金矿tile哈希表 主键为AUTO_ID  
	Hashtable  JinKuangTile_Map = new Hashtable();
	//当前玩家的ID 
	public static SIMPLE_CHAR_INFO now_char_info;
	//偷取的金矿AUTO_ID 
	public static uint steal_jinkuang_id;
	
	
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
		KeepOut.instance.ShowKeepOut();
	}
	//public struct OtherJinKuangUnit
	//{
	//	public uint autoid;
	//	public uint level;
	//	public uint produce_state;
	//	public int 	produce_end_time;
	//}
	// Use this for initialization
	void Start () {
		U3dCmn.GetObjFromPrefab("JinKuangFriendListWin");
			
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
		JinKuangTile_Map.Clear();
		//初始化金矿地块信息 
		for(int i=0;i<JinKuangNum;i++)
		{
			string objname= "JinKuang"+(i+1);
			JinKuang_Map.Add((uint)(info.BeginID+i+1),U3dCmn.SetObjVisible(tween_position.gameObject,objname,false));
			objname= "JinKuangTile"+(i+1);
			
			// 金矿地标无点击 ....
			GameObject objTile = U3dCmn.SetObjVisible(tween_position.gameObject,objname,true);
			if  (objTile.GetComponent<Collider>() != null) { objTile.GetComponent<Collider>().enabled = false; }
			JinKuangTile_Map.Add((uint)(info.BeginID+i+1),objTile);
		}
		
		
		
	}
	
	void RevealPanel(SIMPLE_CHAR_INFO char_info)
	{
		char_name_label.text = char_info.CharName;
		
	
		CTS_GAMECMD_GET_OTHER_GOLDORE_INFO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_OTHER_GOLDORE_INFO;
		req.nAccountID3 = char_info.AccountId;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_OTHER_GOLDORE_INFO_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_OTHER_GOLDORE_INFO);
		tween_position.Play(true);
		
		now_char_info= char_info;
		foreach(DictionaryEntry de in JinKuang_Map)
		{
			GameObject obj = (GameObject)de.Value;
			obj.SetActiveRecursively(false);
		}
		foreach(DictionaryEntry de in JinKuangTile_Map)
		{
			GameObject obj = (GameObject)de.Value;
			obj.SetActiveRecursively(true);
		}
	}
	//刷新数据  
	void RefreshPanel()
	{
		RevealPanel(now_char_info);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		U3dCmn.SendMessage("JinKuangFriendListWin","DismissPanel",null);
		U3dCmn.SendMessage("BuildingManager","DismissJinKuangPanel",null);
		tween_position.Play(false);
		Destroy(gameObject);
		KeepOut.instance.HideKeepOut();
	}
	//初始化金矿数据 
	void InitialJinKuang(byte[] buff)
	{
		STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T.enum_rst.RST_OK)
		{
			head_icon.spriteName = U3dCmn.GetCharSmpIconName((int)sub_msg.nCharHeadID4);
			char_level_label.text = "Lv"+sub_msg.nCharLevel3.ToString();
			yaoqianshu.InitialData(sub_msg);
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_OTHER_GOLDORE_INFO_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GoldoreInfo[] gold_array = (GoldoreInfo[])DataConvert.ByteToStructArray<GoldoreInfo>(data_buff,(int)sub_msg.nNum9);
			for(int i=0;i<gold_array.Length;i++)
			{
				GameObject jinkuang_obj = (GameObject)JinKuang_Map[gold_array[i].nAutoID1];
				jinkuang_obj.SetActiveRecursively(true);
				jinkuang_obj.SendMessage("InitialJinKuang",gold_array[i]);
				GameObject tile_obj = (GameObject)JinKuangTile_Map[gold_array[i].nAutoID1];
				tile_obj.SetActiveRecursively(false);
			}
		}
	}
	//生产黄金时间事件  
	/*void ProcessProduceTERst(byte[] buff)
	{
		
		STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_TE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_TE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_TE_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_TE_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GoldoreProductionTimeEvent[] produce_array = (GoldoreProductionTimeEvent[])DataConvert.ByteToStructArray<GoldoreProductionTimeEvent>(data_buff,(int)sub_msg.nNum3);
			for(int i=0;i<produce_array.Length;i++)
			{
				GameObject jinkuang_obj = (GameObject)JinKuang_Map[produce_array[i].nAutoID1];
				jinkuang_obj.SetActiveRecursively(true);
				jinkuang_obj.SendMessage("ProcessProduceTERst",(int)(produce_array[i].nEndTime3));
				GameObject tile_obj = (GameObject)JinKuangTile_Map[produce_array[i].nAutoID1];
				tile_obj.SetActiveRecursively(false);
			}
		}
	}
	//黄金成熟 
	/*void ProcessGoldResult(byte[] buff)
	{
		STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_EVENT_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_EVENT_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_EVENT_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_OTHER_GOLDORE_PRODUCTION_EVENT_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GoldoreProductionEvent[] result_array = (GoldoreProductionEvent[])DataConvert.ByteToStructArray<GoldoreProductionEvent>(data_buff,(int)sub_msg.nNum3);
			for(int i=0;i<result_array.Length;i++)
			{
				GameObject jinkuang_obj = (GameObject)JinKuang_Map[result_array[i].nAutoID1];
				jinkuang_obj.SetActiveRecursively(true);
				jinkuang_obj.SendMessage("ProcessGoldResult",result_array[i].nProduction2);
				GameObject tile_obj = (GameObject)JinKuangTile_Map[result_array[i].nAutoID1];
				tile_obj.SetActiveRecursively(false);
			}
		}
	}
	//金矿可以偷窃 
	void GoldCanBeStealed(byte[] buff)
	{
		//print ("oooooooooooooooooooooooooooooo");
		STC_GAMECMD_GET_OTHER_GOLDORE_FETCH_INFO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_OTHER_GOLDORE_FETCH_INFO_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_OTHER_GOLDORE_FETCH_INFO_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_OTHER_GOLDORE_FETCH_INFO_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			FetchUnit[] steal_array = (FetchUnit[])DataConvert.ByteToStructArray<FetchUnit>(data_buff,(int)sub_msg.nNum3);
			for(int i=0;i<steal_array.Length;i++)
			{
				GameObject jinkuang_obj = (GameObject)JinKuang_Map[steal_array[i].nAutoID1];
				jinkuang_obj.SetActiveRecursively(true);
				jinkuang_obj.SendMessage("GoldCanBeStealed",steal_array[i].bCanFetch2);
				//print ("oooooooooooooooooooooooooooooo"+steal_array[i].bCanFetch2);
				GameObject tile_obj = (GameObject)JinKuangTile_Map[steal_array[i].nAutoID1];
				tile_obj.SetActiveRecursively(false);
			}
		}
	}*/
	//请求偷窃金矿返回 
	void ReqStealGoldRst(byte[] buff)
	{
		
		STC_GAMECMD_OPERATE_STEAL_GOLD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_STEAL_GOLD_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_STEAL_GOLD_T.enum_rst.RST_OK)
		{
			if(steal_jinkuang_id >0)
			{
				GameObject jinkuang_obj = (GameObject)JinKuang_Map[steal_jinkuang_id];
				jinkuang_obj.SetActiveRecursively(true);
				jinkuang_obj.SendMessage("ReqStealGoldRst",sub_msg.nGold2);
				GameObject tile_obj = (GameObject)JinKuangTile_Map[steal_jinkuang_id];
				tile_obj.SetActiveRecursively(false);
			}
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_STEAL_GOLD_T.enum_rst.RST_GOLD_PROTECT)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD_VIP_PROTECT));
		}
		steal_jinkuang_id = 0;
	}
	//摇钱树数据（别人）  
	/*void InitialYaoQianShu(byte[] buff)
	{
		STC_GAMECMD_GET_OTHER_CHRISTMAS_TREE_INFO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_OTHER_CHRISTMAS_TREE_INFO_T>(buff);
		yaoqianshu.InitialData(sub_msg);
	}*/
	//摇钱树浇水返回(别人)  
	void WaterTreeRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_WATERING_CHRISTMAS_TREE_T.enum_rst.RST_OK)
		{
			yaoqianshu.WaterTreeRst(sub_msg);
		}
	}
	
}
