using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;

static public class AresRankInstance {
	
	public static int ARES_RANK_MAX_NUM = 0;
	static int RANK_MAX_NUM = 50;
	
	static List<AresRankCard> mAresRankCache = new List<AresRankCard>();
	
	public delegate void ProcessAresRankCacheCB();
	public static ProcessAresRankCacheCB processAresRankCacheCB;

	
	static public List<AresRankCard> GetAresRankList()
	{
		return mAresRankCache;
	}
	
	static public void UnpackAresRank(byte[] data)
	{
		STC_GAMECMD_GET_RANK_LIST_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_RANK_LIST_T>(data);
		mAresRankCache.Clear();
		
		if (msgCmd.nNum4 > 0)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_RANK_LIST_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			CharRankInfo[] card_array = (CharRankInfo[])DataConvert.ByteToStructArray<CharRankInfo>(data_buff,msgCmd.nNum4);
			for(int i=0, imax=card_array.Length;i<imax;i++)
			{
				CharRankInfo g = card_array[i];
				AresRankCard card = new AresRankCard();
				card.name = DataConvert.BytesToStr(g.szName3);
				card.nAccountID = g.nAccountID1;
				card.nData = g.nData7;
				card.nLevel = g.nLevel4;
				card.nRank = g.nRank6;
				mAresRankCache.Add(card);
			}
		}
		
		if (msgCmd.nTotalNum3> RANK_MAX_NUM)
		{
			ARES_RANK_MAX_NUM = RANK_MAX_NUM;
		}
		else {
			ARES_RANK_MAX_NUM = (int) msgCmd.nTotalNum3;
		}
		
		if (processAresRankCacheCB != null)
		{
			processAresRankCacheCB();
		}
		
		processAresRankCacheCB = null;
	}
}
