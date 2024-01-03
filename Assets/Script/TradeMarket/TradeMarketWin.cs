using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class TradeMarketWin : MonoBehaviour {

	const int item_num_max = 10;
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 

	//排名面板 
	public GameObject sell_item;
	public UIDraggablePanel trade_panel;
	public UIGrid trade_grid;
	GameObject[] trade_item_array = new GameObject[item_num_max];
	public UILabel trade_desc_label;
	public UILabel page_num;
	int now_page_num;
	bool refresh = false;
	
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
	// Use this for initialization
	void Start () {
		for(int i=0;i<item_num_max;i++)
		{
			GameObject obj = NGUITools.AddChild(trade_grid.gameObject,sell_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			trade_item_array[i] = obj;
		}
		trade_grid.Reposition();
		now_page_num = 1;
		
		uint trade_lv_limit = (int)CMN_DEF.SELL_GOLD_LEVEL_LIMIT;
	    trade_desc_label.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD_TRADE_DESC),U3dCmn.GetCmnDefNum(trade_lv_limit));
	}
	
	// Update is called once per frame
	void Update () {
		if(refresh)
		{
			GetTradeData();
			refresh = false;
		}
	}
	void RevealPanel()
	{
		now_page_num = 1;
		refresh = true;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void GetTradeData()
	{
		trade_panel.gameObject.SetActiveRecursively(false);
		TradeMarketManager.ReqTradeList((uint)(now_page_num-1),(uint)item_num_max);
	}
	void InitialPanelData()
	{
		trade_panel.gameObject.SetActiveRecursively(true);
		if(now_page_num>GetPageNum((int)TradeMarketManager.instance.total_num))
			now_page_num = GetPageNum((int)TradeMarketManager.instance.total_num);
		
		int from_num = 0;
		int end_num = item_num_max;
		int i=0;
		//print ("nnnnnnnnnnnnnnnnnn"+RankManager.RankDataList.Count);
		List<GoldMarketDeal_T> TradeDataList = TradeMarketManager.instance.GetTradeList();
		for(int n= from_num;n<TradeDataList.Count;n++)
		{
			//print ("");
			//int index = RankManager.RankDataList.Count - n-1;
			GoldMarketDeal_T unit = (GoldMarketDeal_T)TradeDataList[n];
			
			trade_item_array[i].SetActiveRecursively(true);
			trade_item_array[i].SendMessage("InitialData",unit);
			i++;
		}
		for(;i<6;i++)
		{
			GoldMarketDeal_T unit =new GoldMarketDeal_T();
			trade_item_array[i].SetActiveRecursively(true);
			trade_item_array[i].SendMessage("InitialData",unit);
		}
		for(;i<item_num_max;i++)
		{
			trade_item_array[i].SetActiveRecursively(false);
		}
		trade_grid.Reposition();
		trade_panel.ResetPosition();
		page_num.text = now_page_num +"/"+GetPageNum((int)TradeMarketManager.instance.total_num);
		
	}
	//下一页 
	void NextPage()
	{
		
		if(now_page_num < GetPageNum((int)TradeMarketManager.instance.total_num))
		{
			now_page_num++;
		}	
		GetTradeData();
	}
	//上一页 
	void PrevPage()
	{
	
		if(now_page_num > 1)
		{
			now_page_num--;
		}	
		GetTradeData();
	}

	//获取页数 
	int GetPageNum(int num)
	{
		int pagenum = 1;
		if(num>0)
			pagenum = (num-1)/item_num_max+1;
			
		return pagenum;
	}
	//打开委托卖金币界面 
	void OpenSellGoldWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("SellGoldWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",null);
		}
	}
	//刷新出我的寄卖数据 
	void GetMyTradeData()
	{
		trade_panel.gameObject.SetActiveRecursively(false);
		TradeMarketManager.ReqSelfTradeList();
	}
		
}
