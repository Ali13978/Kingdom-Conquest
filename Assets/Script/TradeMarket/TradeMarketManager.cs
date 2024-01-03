using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class TradeMarketManager : MonoBehaviour {
	static TradeMarketManager mInst = null;
	List<GoldMarketDeal_T>TradeDataList = new List<GoldMarketDeal_T>();
	public uint total_num;
	static public TradeMarketManager instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(TradeMarketManager)) as TradeMarketManager;

				if (mInst == null)
				{
					GameObject go = new GameObject("TradeMarketManager");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<TradeMarketManager>();
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public List<GoldMarketDeal_T> GetTradeList()
	{
		return TradeDataList;
	}
	//获取金币交易数据列表 
	public static void ReqTradeList(uint page_num,uint page_size)
	{
		CTS_GAMECMD_GET_GOLD_DEAL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_GOLD_DEAL;
		req.nPage3 = page_num;
		req.nPageSize4 = page_size;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_GOLD_DEAL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_GOLD_DEAL);
	}
	//获取金币交易数据列表返回  
	public void ReqTradeListRst(byte[] buff)
	{
		STC_GAMECMD_GET_GOLD_DEAL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_GOLD_DEAL_T>(buff);
		if (sub_msg.nRst1 == (int)STC_GAMECMD_GET_GOLD_DEAL_T.enum_rst.RST_OK)
		{
			total_num = sub_msg.nTotalNum3;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_GOLD_DEAL_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GoldMarketDeal_T[] trade_array = (GoldMarketDeal_T[])DataConvert.ByteToStructArray<GoldMarketDeal_T>(data_buff,(int)sub_msg.nNum4);
			TradeDataList.Clear();
			for(int i=0;i<trade_array.Length;i++)
			{
				TradeDataList.Add(trade_array[i]);
			}
			U3dCmn.SendMessage("TradeMarketWin","InitialPanelData",null);
		}
	}
	//获取自己的金币交易数据列表 
	public static void ReqSelfTradeList()
	{
		CTS_GAMECMD_GET_SELF_GOLD_DEAL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_SELF_GOLD_DEAL;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_SELF_GOLD_DEAL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_SELF_GOLD_DEAL);
	}
	//获取自己的金币交易数据列表返回   
	public void ReqSelfTradeListRst(byte[] buff)
	{
		STC_GAMECMD_GET_SELF_GOLD_DEAL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_SELF_GOLD_DEAL_T>(buff);
		if (sub_msg.nRst1 == (int)STC_GAMECMD_GET_SELF_GOLD_DEAL_T.enum_rst.RST_OK)
		{
			total_num = 1;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_SELF_GOLD_DEAL_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GoldMarketDeal_T[] trade_array = (GoldMarketDeal_T[])DataConvert.ByteToStructArray<GoldMarketDeal_T>(data_buff,(int)sub_msg.nNum2);
			TradeDataList.Clear();
			for(int i=0;i<trade_array.Length;i++)
			{
				TradeDataList.Add(trade_array[i]);
			}
			U3dCmn.SendMessage("TradeMarketWin","InitialPanelData",null);
		}
	}
	//请求卖金币   
	public void ReqSellGold(uint gold_num,uint price)
	{
		CTS_GAMECMD_OPERATE_ADD_GOLD_DEAL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_ADD_GOLD_DEAL;
		req.nGold3 = gold_num;
		req.nPrice4 = price;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_ADD_GOLD_DEAL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_ADD_GOLD_DEAL);
	}
	//请求卖金币返回 
	public void ReqSellGoldRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ADD_GOLD_DEAL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ADD_GOLD_DEAL_T>(buff);
		if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ADD_GOLD_DEAL_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold -= sub_msg.nGold2;
			PlayerInfoManager.RefreshPlayerDataUI();
			U3dCmn.SendMessage("SellGoldWin","DismissPanel",null);
			U3dCmn.SendMessage("TradeMarketWin","GetTradeData",null);
		}
		else if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ADD_GOLD_DEAL_T.enum_rst.RST_LEVEL_LOW)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD_SELL_NUM_TOP));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ADD_GOLD_DEAL_T.enum_rst.RST_TOO_MANY)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ONLY_ONE_SALE_SAMETIME));
		}
	}
	//撤销黄金交易 
	public void ReqRevokeGoldSale(ulong transaction_id)
	{
		CTS_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CANCEL_GOLD_DEAL;
		req.nTransactionID3 = transaction_id;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL);
	}
	//撤销黄金交易返回  
	public void ReqRevokeGoldSaleRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T>(buff);
		if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold += sub_msg.nGold2;
			PlayerInfoManager.RefreshPlayerDataUI();
			U3dCmn.SendMessage("TradeMarketWin","GetTradeData",null);
		}
		else if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CANCEL_GOLD_DEAL_T.enum_rst.RST_CANCEL_INTERVAL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.TRADE_INTERVAL_IS_TOO_SHORT));
		}
	}
	//购买黄金 
	public void ReqBuyGoldSale(ulong transaction_id,uint money_type)
	{
		CTS_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET;
		req.nTransactionID3 = transaction_id;
		req.nMoneyType4 = money_type;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET);
	}
	//购买黄金返回  
	public void ReqBuyGoldSaleRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T>(buff);
		if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T.enum_rst.RST_OK)
		{
			if(sub_msg.nMoneyType2 == (uint)money_type.money_type_diamond)
			{
				CommonData.player_online_info.Diamond -= sub_msg.nTotalPrice3;
			}
			else if(sub_msg.nMoneyType2 == (uint)money_type.money_type_crystal)
			{
				CommonData.player_online_info.Crystal -= sub_msg.nTotalPrice3;
			}
			CommonData.player_online_info.Gold += sub_msg.nGold4;
			PlayerInfoManager.RefreshPlayerDataUI();
			U3dCmn.SendMessage("TradeMarketWin","GetTradeData",null);
			U3dCmn.SendMessage("TradeMarketBuyGoldWin","DismissPanel",null);
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.STORE_BUY_OK));
		}
		else if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T.enum_rst.RST_DEAL_NOT_EXISTS) 
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD_BOUGHT_BY_OTHERS));
		}
		else if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_BUY_GOLD_IN_MARKET_T.enum_rst.RST_LEVEL_TOO_LOW) 
		{
			string tip = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LORD_LEVEL_LIMIT_BUY_GOLD),U3dCmn.GetCmnDefNum((int)CMN_DEF.BUY_GOLD_LEVEL_LIMIT));
			U3dCmn.ShowWarnWindow(tip);
		}
	}
}
