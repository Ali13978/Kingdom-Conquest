using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class StoreWin : MonoBehaviour {
	bool EnableInatialPanel = false;
	//商城面板positon TWEEN    
	public TweenPosition tween_position; 
	//装备grid管理器  
	public PagingStorage equip_paging_storage;
	//装备标签页按钮   
	public UICheckbox equip_tab;
	//装备面板capacity 
	int equip_capacity;
	//宝石grid管理器  
	public PagingStorage gem_paging_storage;
	//宝石标签页按钮    
	public UICheckbox gem_tab;
	//宝石面板capacity 
	int gem_capacity;
	//用品grid管理器  
	public PagingStorage articles_paging_storage;
	//用品标签页按钮    
	public UICheckbox articles_tab;
	//用品面板capacity 
	int articles_capacity;
	//金币面板 
	public UICheckbox gold_tab;
	public GameObject gold_item;
	
	//某买操作面板 
	public GameObject buy_obj;
	//当前拥有钻石数量 
	public UILabel diamon_num_have;
	//当前拥有水晶数量  
	public UILabel crystal_num_have;
	//选中商品title 
	public UILabel goods_name;
	//选中商品描述 
	public UILabel goods_describe;
	//钻石购买勾选  
	public UICheckbox diamond_checkbox;
	//水晶购买勾选   
	public UICheckbox crystal_checkbox;
	//购买总数量 
	public UIInput buy_num;
	//增加数量按钮 
	public UISprite add_num;
	//减少数量按钮  
	public UISprite sub_num;
	//购买总价格 
	public UILabel total_price;
	//购买按钮  
	public GameObject buy_btn;
	//购买单价 
	int goods_price;
	//商品excelid
	uint GoodsExcelID;
	public UILabel page_num;
	//各物品面板的页数 
	int equip_page_num;
	int gem_page_num;
	int article_page_num;
	
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
		buy_num.text = "1";
		UIEventListener.Get(add_num.gameObject).onPress -= ChangeNum;
		UIEventListener.Get(add_num.gameObject).onPress += ChangeNum;
		UIEventListener.Get(sub_num.gameObject).onPress -= ChangeNum;
		UIEventListener.Get(sub_num.gameObject).onPress += ChangeNum;
		
		PlayerInfoManager.RegisterCallBack(gameObject,"UpdateMoneyInfo");
	}
	
	// Update is called once per frame
	void Update () {
		//防止panel initial之后SetActiveRecursively(true)导致面板的部分空间隐藏不可控制 
		if(EnableInatialPanel)
		{
			InitialPanelData();
			EnableInatialPanel = false;
		}
	}
	void RevealPanel()
	{
		//InitialPanelData();
		//paging_storage.SetCapacity(12);
		//equip_paging_storage.gameObject.SetActiveRecursively(false);
		tween_position.Play(true);
		EnableInatialPanel = true;
		equip_tab.isChecked = true;
		
	}
	//隐藏窗口 
	void DismissPanel()
	{
		U3dCmn.SendMessage("HeroEquipWin","RefreshPanelData",null);
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//初始化面板数据 
	void InitialPanelData()
	{
		UpdateMoneyInfo();
		
		if(equip_tab.isChecked)
		{
			StoreManager.select_panel_sort = (uint)ItemSort.EQUIP;
			//初始化装备面板  
			equip_capacity = StoreManager.equip_id_list.Count;
			equip_paging_storage.SetCapacity(equip_capacity);
			List<GameObject> equip_item_list = equip_paging_storage.GetAvailableItems();
			
			for(int i=0;i<equip_item_list.Count;i++)
			{
				UIEventListener.Get(equip_item_list[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(equip_item_list[i].gameObject).onClick += ShowDetail;
			}
			equip_paging_storage.ResetAllSurfaces();
			equip_page_num = U3dCmn.GetPageNum(equip_capacity,6); 
			page_num.text = "1/"+equip_page_num;
			equip_paging_storage.onDragingPage = ShowPageNum;
		}
		else if(gem_tab.isChecked)
		{
			StoreManager.select_panel_sort = (uint)ItemSort.GEMSTONE;
			//初始化宝石面板   
			gem_capacity = StoreManager.gem_id_list.Count;
			gem_paging_storage.SetCapacity(gem_capacity);
			List<GameObject> gem_item_list = gem_paging_storage.GetAvailableItems();
			for(int i=0;i<gem_item_list.Count;i++)
			{
				UIEventListener.Get(gem_item_list[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(gem_item_list[i].gameObject).onClick += ShowDetail;
			}
			gem_paging_storage.ResetAllSurfaces();
			gem_page_num = U3dCmn.GetPageNum(gem_capacity,6); 
			page_num.text = "1/"+gem_page_num;
			gem_paging_storage.onDragingPage = ShowPageNum;
		}
		else if(articles_tab.isChecked)
		{
			StoreManager.select_panel_sort = (uint)ItemSort.ARTICLES;
			//初始化用品面板   
			articles_capacity = StoreManager.articles_id_list.Count;
			articles_paging_storage.SetCapacity(articles_capacity);
			List<GameObject> articles_item_list = articles_paging_storage.GetAvailableItems();
			for(int i=0;i<articles_item_list.Count;i++)
			{
				UIEventListener.Get(articles_item_list[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(articles_item_list[i].gameObject).onClick += ShowDetail;
			}
			articles_paging_storage.ResetAllSurfaces();
			article_page_num = U3dCmn.GetPageNum(articles_capacity,6); 
			page_num.text = "1/"+article_page_num;
			articles_paging_storage.onDragingPage = ShowPageNum;
		}
		else if(gold_tab.isChecked)
		{
			gold_item.SendMessage("InitialData");
		}
		ClearSelect();
	}
	
	void ClearSelect()
	{
		List<GameObject> equip_item_list = equip_paging_storage.GetAvailableItems();
		for(int i=0; i<equip_item_list.Count;i++)
		{
			StoreItem store_item = equip_item_list[i].GetComponent<StoreItem>();
			//store_item.icon.color = new Color(1,1,1,1);
			store_item.SetSelect(false);
		}
		List<GameObject> gem_item_list = gem_paging_storage.GetAvailableItems();
		for(int i=0; i<gem_item_list.Count;i++)
		{
			StoreItem store_item = gem_item_list[i].GetComponent<StoreItem>();
			//store_item.icon.color = new Color(1,1,1,1);
			store_item.SetSelect(false);
		}
		List<GameObject> articles_item_list = articles_paging_storage.GetAvailableItems();
		for(int i=0; i<articles_item_list.Count;i++)
		{
			StoreItem store_item = articles_item_list[i].GetComponent<StoreItem>();
			//store_item.icon.color = new Color(1,1,1,1);
			store_item.SetSelect(false);
		}
		GoodsExcelID = 0;
		goods_name.text = "";
		goods_describe.text = "";
		buy_num.text = "1";
		goods_price = 0;
		total_price.text = "0";
			
	}
	void ShowDetail(GameObject obj)
	{
		ClearSelect();
		StoreItem item = obj.GetComponent<StoreItem>();
		item.SetSelect(true);
		GoodsExcelID = item.excel_id;
		goods_name.text =item.name.text;
		goods_price = int.Parse(CommonMB.CheckNullStr(item.price.text));
		total_price.text = goods_price.ToString();
		if(item.describe == null)
			goods_describe.text ="";
		else
			goods_describe.text = item.describe;
		
		uint sort_id = (uint) CommonMB.ExcelToSort_Map[GoodsExcelID];
		if(sort_id == (uint)ItemSort.EQUIP)
		{
			goods_describe.text+=("\n"+U3dCmn.GetTipByExcelID(GoodsExcelID));
		}
		
		
		if(item.sell_by_crystal == 1)
		{
			buy_obj.gameObject.SetActiveRecursively(true);
			crystal_checkbox.gameObject.SetActiveRecursively(true);
			diamond_checkbox.gameObject.SetActiveRecursively(true);
			diamond_checkbox.isChecked =true;
			buy_btn.SetActiveRecursively(true);
			//crystal_checkbox.isChecked = true;
		}
		else if(item.sell_by_crystal == 0)
		{
			buy_obj.gameObject.SetActiveRecursively(true);
			crystal_checkbox.gameObject.SetActiveRecursively(false);
			diamond_checkbox.gameObject.SetActiveRecursively(true);
			diamond_checkbox.isChecked =true;
			buy_btn.SetActiveRecursively(true);
		}
		else if(item.sell_by_crystal == 2)
		{
			buy_obj.gameObject.SetActiveRecursively(false);
			crystal_checkbox.gameObject.SetActiveRecursively(false);
			diamond_checkbox.gameObject.SetActiveRecursively(false);
			buy_btn.SetActiveRecursively(false);
		}
	}
	//选中购买金币 
	void ShowGoldDetail(GoldItem item)
	{
		ArticlesInfo articles_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)ARTICLES.JINBI];
		GoodsExcelID = (uint)ARTICLES.JINBI;
		goods_price = 1;
		total_price.text = goods_price.ToString();
		goods_name.text = articles_info.Name;
		goods_describe.text = articles_info.Describe;
		crystal_checkbox.gameObject.SetActiveRecursively(false);
		diamond_checkbox.isChecked =true;
	}
	
	//更改数量 
	void ChangeNum(GameObject obj,bool ispressed)
	{
		int num =int.Parse(CommonMB.CheckNullStr(buy_num.text));
		if(ispressed)
		{
			if(obj.name == "AddNum")
				StartCoroutine("ChangeNumCoroutine",true);
			else if(obj.name == "SubNum")
				StartCoroutine("ChangeNumCoroutine",false);
		}
		else
		{
			StopCoroutine("ChangeNumCoroutine");
		}
	}
	//更改当前页数（根据回调） 
	void ShowPageNum(int num)
	{
		int total_page_num = 1;
		if(equip_tab.isChecked)
		{
			total_page_num = equip_page_num;
		}
		else if(gem_tab.isChecked)
		{
			total_page_num = gem_page_num;
		}
		else if(articles_tab.isChecked)
		{
			total_page_num = article_page_num;
		}
		page_num.text = (num+1)+"/"+total_page_num;
	}
	//刷新最新的钻石和水晶数据 
	void UpdateMoneyInfo()
	{
		diamon_num_have.text = CommonData.player_online_info.Diamond.ToString();
		crystal_num_have.text = CommonData.player_online_info.Crystal.ToString();
	}
	//改变购买数量 
	IEnumerator ChangeNumCoroutine(bool isadd)
	{
		int num =int.Parse(CommonMB.CheckNullStr(buy_num.text));
		while(true)
		{
			if(isadd)
			{
				buy_num.text = num<1000?(++num).ToString():"1000";
			}
			else
				buy_num.text = num>1?(--num).ToString():"1";
			ChangeTotalPrice();
			yield return new WaitForSeconds(0.2f);
		}
	}
	//根据数量更改总价 
	void ChangeTotalPrice()
	{
		if(U3dCmn.IsNumeric(buy_num.text))
		{
			int num =int.Parse(CommonMB.CheckNullStr(buy_num.text));
			total_price.text = (goods_price*num).ToString();
		}
		else
		{
			buy_num.text = "1";
			total_price.text = goods_price.ToString();
		}
	}
	//购买申请 
	void ReqBuy()
	{
		uint moneytype = 0;
		if(crystal_checkbox.isChecked)
		{
			moneytype =(uint)money_type.money_type_crystal;
			if(CommonData.player_online_info.Crystal<uint.Parse(CommonMB.CheckNullStr(total_price.text))) //检查水晶是否够用 
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH));
				return ;
			}
		}
		else if(diamond_checkbox.isChecked)
		{
			moneytype =(uint)money_type.money_type_diamond;
			if(CommonData.player_online_info.Diamond<uint.Parse(CommonMB.CheckNullStr(total_price.text))) //检查钻石是否够用 
			{
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
				U3dCmn.ShowRechargePromptWindow();
				return ;
			}
		}
		if(GoodsExcelID == (uint)ARTICLES.JINBI)
		{
			if(!U3dCmn.IsNumeric(buy_num.text))
				return;
			uint num =uint.Parse(CommonMB.CheckNullStr(buy_num.text));
			CTS_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD;
			req.nDiamond3 = num;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONVERT_DIAMOND_GOLD);
		}
		else if(GoodsExcelID !=0)
		{
			if(!U3dCmn.IsNumeric(buy_num.text))
				return;
			uint num =uint.Parse(CommonMB.CheckNullStr(buy_num.text));
			CTS_GAMECMD_OPERATE_BUY_ITEM_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BUY_ITEM;
			req.nExcelID3 = GoodsExcelID;
			req.nMoneyType5 = moneytype;
			req.nNum4 = num;
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUY_ITEM_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUY_ITEM);
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOODS_NOT_SELECT));
		}
	}
	//打开购买钻石窗口 
	void OpenBuyWin()
	{
		if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("RechargeWinApple"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		}
		else if(CommonData.VERSION == VERSION_TYPE.VERSION_INNER)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("RechargeWinApple"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",null);
			}
		}
	}
}
