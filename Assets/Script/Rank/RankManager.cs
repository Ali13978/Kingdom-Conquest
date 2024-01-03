using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class RankManager : MonoBehaviour {
	//好友表 
	public static List<CharRankInfo>RankDataList = new List<CharRankInfo>();
	public static uint rank_type;
	public static uint total_num;
	public static uint my_rank;
	const int max_num = 200;
	// Use this for initialization
	void Start () {
		GetMyRankInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//请求自己的排名信息  
	void GetMyRankInfo()
	{
		CTS_GAMECMD_GET_MY_RANK_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_MY_RANK;
		req.nType3 = (uint)RANK_TYPE.rank_type_char_level;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_MY_RANK_T >(req);
	}
	//请求自己的排名信息返回   
	void GetMyRankInfoRst(byte[] buff)
	{
		STC_GAMECMD_GET_MY_RANK_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_MY_RANK_T>(buff);
		CommonData.player_online_info.LevelRank = sub_msg.nRank2;
		//sub_msg.
	}
	//请求排名信息列表   
	public static void GetRankListInfo(uint type,uint from_num,uint num)
	{
		rank_type = type;
		CTS_GAMECMD_GET_RANK_LIST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_RANK_LIST;
		req.nType3 = type;
		req.nFrom4 = from_num;
		req.nNum5 = num;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_RANK_LIST_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_RANK_LIST);
	}
	//请求排名信息列表返回    
	void GetRankListInfoRst(byte[] buff)
	{
		STC_GAMECMD_GET_RANK_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_RANK_LIST_T>(buff);
		//if(sub_msg.nType1  == (int)STC_GAMECMD_GET_RANK_LIST_T.enum_rst.RST_OK)
		//{	
		if(sub_msg.nType1 == (uint)RANK_TYPE.rank_type_instance_wangzhe)
		{
			AresRankInstance.UnpackAresRank(buff);
			return ;
		}
		if(sub_msg.nMyRank2 == 0)
			my_rank = sub_msg.nTotalNum3+1;
		else 
			my_rank = sub_msg.nMyRank2;
		if(sub_msg.nType1 == (uint)RANK_TYPE.rank_type_char_level)
		{
			CommonData.player_online_info.LevelRank = my_rank;
		}
		// = 	sub_msg.nMyRank2;
		RankDataList.Clear();
		if(sub_msg.nTotalNum3>max_num)
			total_num = max_num;
		else 
			total_num = sub_msg.nTotalNum3;
		int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_RANK_LIST_T>();
		int data_len = buff.Length - head_len;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,head_len,data_buff,0,data_len);
		CharRankInfo[] rank_array = (CharRankInfo[])DataConvert.ByteToStructArray<CharRankInfo>(data_buff,sub_msg.nNum4);
		for(int i=0;i<rank_array.Length;i++)
		{
			RankDataList.Add(rank_array[i]);
		}
		U3dCmn.SendMessage("RankWin","InitialPanelData",null);
		//}
	}
}
