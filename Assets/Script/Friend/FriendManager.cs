using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class FriendManager : MonoBehaviour {
	//好友表 
	public static List<FriendUnit>FriendDataList = new List<FriendUnit>();
	//仇人表  
	public static List<EnemyUnit>EnemyDataList = new List<EnemyUnit>();
	//别人的请求列表  
	public static List<FriendApplyUnit>FriendApplyDataList = new List<FriendApplyUnit>();
	//public static Hashtable FriendMap = new Hashtable();
	//仇人表 
	public static Hashtable EnemyMap = new Hashtable();


	// Use this for initialization
	void Start () {
		ReqFriendData();
		//ReqMyApplyData();
		ReqOtherApplyData();
		ReqEnemyData();
	}
	
	void OpenFriendWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("FriendWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel");
		}
	}
	//获取好友列表 
	void ReqFriendData()
	{
		CTS_GAMECMD_GET_FRIEND_LIST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_FRIEND_LIST;
	
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_FRIEND_LIST_T>(req);
	}
	//获取好友列表返回  
	void ReqFriendDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_FRIEND_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_FRIEND_LIST_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_FRIEND_LIST_T.enum_rst.RST_OK)
		{
			//print ("ffffffffffff"+sub_msg.nNum2);
			FriendDataList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_FRIEND_LIST_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			FriendUnit[] friend_array = (FriendUnit[])DataConvert.ByteToStructArray<FriendUnit>(data_buff,(int)sub_msg.nNum2);
			for(int i=0;i<friend_array.Length;i++)
			{
				FriendDataList.Add(friend_array[i]);
			}
		}
	}
	//获取仇人列表  
	void ReqEnemyData()
	{
		CTS_GAMECMD_GET_ENEMY_LIST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ENEMY_LIST;
	
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ENEMY_LIST_T>(req);
	}
	//获取仇人列表返回   
	void ReqEnemyDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_ENEMY_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ENEMY_LIST_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ENEMY_LIST_T.enum_rst.RST_OK)
		{
			
			EnemyDataList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ENEMY_LIST_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			EnemyUnit[] enemy_array = (EnemyUnit[])DataConvert.ByteToStructArray<EnemyUnit>(data_buff,(int)sub_msg.nNum2);
			for(int i=0;i<enemy_array.Length;i++)
			{
				EnemyDataList.Add(enemy_array[i]);
			}
		}
	}
	//新的仇人（推送） 
	void NewEnemy(byte[] buff)
	{
		EnemyUnit sub_msg = DataConvert.ByteToStruct<EnemyUnit>(buff);
		if(!IsEnemyExit(sub_msg.nAccountID1))
			EnemyDataList.Add(sub_msg);
	}
	//获取邀请列表(申请加别人好友)  
	/*void ReqMyApplyData()
	{
		CTS_GAMECMD_GET_MY_FRIEND_APPLY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MY_FRIEND_APPLY;
	
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MY_FRIEND_APPLY_T>(req);
	}*/
	//获取请求列表(别人加我)   
	void ReqOtherApplyData()
	{
		CTS_GAMECMD_GET_OTHERS_FRIEND_APPLY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_OTHERS_FRIEND_APPLY;
	
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_OTHERS_FRIEND_APPLY_T>(req);
	}
	
	//获取请求列表返回(别人加我) 
	void ReqOtherApplyDataRst(byte[] buff)
	{
		
		STC_GAMECMD_GET_OTHERS_FRIEND_APPLY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_OTHERS_FRIEND_APPLY_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_OTHERS_FRIEND_APPLY_T.enum_rst.RST_OK)
		{
			FriendApplyDataList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_OTHERS_FRIEND_APPLY_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			FriendApplyUnit[] apply_array = (FriendApplyUnit[])DataConvert.ByteToStructArray<FriendApplyUnit>(data_buff,(int)sub_msg.nNum2);
			for(int i=0;i<apply_array.Length;i++)
			{
				FriendApplyDataList.Add(apply_array[i]);
				//print("HHHHHHHHHHHHHH"+apply_array[i].nHeadID5);
			}
		}
	}
	//申请加别人好友 
	public static void ReqAddFriend(ulong account_id)
	{
		CTS_GAMECMD_OPERATE_APPLY_FRIEND_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_APPLY_FRIEND;
		req.nPeerAccountID3 = account_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_APPLY_FRIEND_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_APPLY_FRIEND);
	}
	//申请加别人好友返回  
	void ReqAddFriendRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_APPLY_FRIEND_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_APPLY_FRIEND_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPLY_FRIEND_T.enum_rst.RST_OK || sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPLY_FRIEND_T.enum_rst.RST_ALREADY_EVENT)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ADD_FRIEND_OK));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPLY_FRIEND_T.enum_rst.RST_ALREADY_FRIEND)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.YOU_HAVE_FRIENDS));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPLY_FRIEND_T.enum_rst.RST_SELF_MAX_FRIEND)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.FRIEND_NUM_FULL));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPLY_FRIEND_T.enum_rst.RST_PEER_MAX_FRIEND)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.OTHER_FRIEND_NUM_FULL));
		}
		
	}
	//别人的好友申请推送   
	void  NewOtherFriendApply(byte[] buff)
	{	
		FriendApplyUnit sub_msg = DataConvert.ByteToStruct<FriendApplyUnit>(buff);
		for(int i=0;i<FriendApplyDataList.Count;i++)
		{
			if(FriendApplyDataList[i].nAccountID1 == sub_msg.nAccountID1)
				return;
		}
		FriendApplyDataList.Add(sub_msg);
		U3dCmn.SendMessage("FriendWin","InitialPanelData",null);
	}
	//同意成为好友 
	public static void AgreeFriendApply(FriendApplyUnit apply_unit)
	{
		CTS_GAMECMD_OPERATE_APPROVE_FRIEND_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_APPROVE_FRIEND;
		req.nPeerAccountID3 = apply_unit.nAccountID1;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_APPROVE_FRIEND_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_APPROVE_FRIEND);
	}
	//同意成为好友返回  
	public void AgreeFriendApplyRst(byte[] buff)
	{
		
		STC_GAMECMD_OPERATE_APPROVE_FRIEND_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_APPROVE_FRIEND_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPROVE_FRIEND_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_APPROVE_FRIEND_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
	
			FriendUnit friend_data = DataConvert.ByteToStruct<FriendUnit>(data_buff);
			DeleteApplyData(friend_data.nAccountID1);
			FriendDataList.Add(friend_data);
			U3dCmn.SendMessage("FriendWin","InitialPanelData",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPROVE_FRIEND_T.enum_rst.RST_MY_FRIEND_MAX)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.FRIEND_NUM_FULL));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_APPROVE_FRIEND_T.enum_rst.RST_PEER_FRIEND_MAX)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.OTHER_FRIEND_NUM_FULL));
		}
	}
	//拒绝成为好友 
	public static void RefuseFriendApply(FriendApplyUnit apply_unit)
	{
		CTS_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY;
		req.nPeerAccountID3 = apply_unit.nAccountID1;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_REFUSE_FRIEND_APPLY_T>(req);
		
		FriendApplyDataList.Remove(apply_unit);
		
	}
	//删除好友  
	public static void DeleteFriend(ulong account_id)
	{
		CTS_GAMECMD_OPERATE_DELETE_FRIEND_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_DELETE_FRIEND;
		req.nFriendID3 = account_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_DELETE_FRIEND_T>(req);
		
		//FriendApplyDataList.Remove(apply_unit);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DELETE_FRIEND);
	}
	//删除仇人   
	public static void DeleteEnemy(ulong account_id)
	{
		CTS_GAMECMD_OPERATE_DELETE_ENEMY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_DELETE_ENEMY;
		req.nEnemyID3 = account_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_DELETE_ENEMY_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DELETE_ENEMY);
		//FriendApplyDataList.Remove(apply_unit);
	}
	
	//删除好友返回  
	public void DeleteFriendRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_DELETE_FRIEND_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_DELETE_FRIEND_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_DELETE_FRIEND_T.enum_rst.RST_OK)
		{
			DeleteFriendData(sub_msg.nPeerAccountID2);
		}
		U3dCmn.SendMessage("FriendWin","InitialPanelData",null);
	}
	//删除仇人返回  
	public void DeleteEnemyRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_DELETE_ENEMY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_DELETE_ENEMY_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_DELETE_ENEMY_T.enum_rst.RST_OK)
		{
			DeleteEnemyData(sub_msg.nEnemyID2);
		}
		U3dCmn.SendMessage("FriendWin","InitialPanelData",null);
	}
	//从请求列表中删除数据  
	void DeleteApplyData(ulong account_id)
	{
		int index = -1;
		for(int i=0; i<FriendApplyDataList.Count;i++)
		{
			if(account_id == FriendApplyDataList[i].nAccountID1)
			{
				index = i;
				break;
			}
				
		}
		if(index != -1)
			FriendApplyDataList.RemoveAt(index);
	}
	//从好友列表中删除数据 
	void DeleteFriendData(ulong account_id)
	{
		int index = -1;
		for(int i=0; i<FriendDataList.Count;i++)
		{
			if(account_id == FriendDataList[i].nAccountID1)
			{
				index = i;
				break;
			}
				
		}
		if(index != -1)
			FriendDataList.RemoveAt(index);
	}
	//从仇人列表中删除数据 
	void DeleteEnemyData(ulong account_id)
	{
		int index = -1;
		for(int i=0; i<EnemyDataList.Count;i++)
		{
			if(account_id == EnemyDataList[i].nAccountID1)
			{
				index = i;
				break;
			}
				
		}
		if(index != -1)
			EnemyDataList.RemoveAt(index);
	}
	//根据好友ACCOUNTI_ID获取好友名字 
	public static FriendUnit GetFriendDataFromID(ulong account_id)
	{
		FriendUnit unit = new FriendUnit();
		for(int i=0; i<FriendDataList.Count;i++)
		{
			if(account_id == FriendDataList[i].nAccountID1)
			{
				unit =  FriendDataList[i];
				break;
			}
				
		}
		return unit;
	}
	//仇人列表中是否存在此玩家 
	bool IsEnemyExit(ulong account_id)
	{
		bool isexit = false;
		for(int i=0; i<EnemyDataList.Count;i++)
		{
			if(account_id == EnemyDataList[i].nAccountID1)
			{
				isexit = true;
				break;
			}
				
		}
		return isexit;
	}
}
