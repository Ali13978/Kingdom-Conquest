using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class CangKuWin : MonoBehaviour {
	bool EnableInatialPanel = false;
	//商城面板positon TWEEN    
	public TweenPosition tween_position; 
	//全部grid管理器   
	public PagingStorage allitem_paging_storage;
	//全部标签页按钮   
	public UICheckbox allitem_tab;
	//全部商品item_page
	public GameObject allitem_page;
	//装备grid管理器  
	public PagingStorage equip_paging_storage;
	//装备标签页按钮   
	public UICheckbox equip_tab;
	//宝石grid管理器   
	public PagingStorage gem_paging_storage;
	//宝石标签页按钮    
	public UICheckbox gem_tab;
	//用品grid管理器   
	public PagingStorage article_paging_storage;
	//用品标签页按钮    
	public UICheckbox article_tab;
	//任务道具grid管理器   
	public PagingStorage task_paging_storage;
	//任务道具标签页按钮    
	public UICheckbox task_tab;
	
	//物品名称  
	public UILabel item_name;
	//物品描述 
	public UILabel item_describe;
	//物品数量 
	public UIInput item_num;
	//增加数量按钮 
	public UISprite add_num;
	//减少数量按钮  
	public UISprite sub_num;
	//出售价格 
	public UILabel sell_price_label;
	//合成按钮 
	public UIImageButton compose_btn;
	//镶嵌按钮   
	public UIImageButton embed_btn;
	//使用按钮  
	public UIImageButton use_btn;
	//售出按钮  
	public UIImageButton sell_btn;
	
	//选中物品售价 
	uint sell_unit_price;
	//选中物品ID 
	GameObject select_obj;
	ulong select_item_id;
	int select_excel_id;
	public UILabel page_num;
	//各物品面板的页数 
	int all_page_num;
	int equip_page_num;
	int gem_page_num;
	int article_page_num;
	int task_page_num;
	//public int select_item_num;
	//全部面板ITEM列表 
	//public  List<ulong> all_id_list = new List<ulong>();
	//装备面板ITEM列表  
//	public  List<ulong> equip_id_list = new List<ulong>();
	//宝石面板ITEM列表  
	//public  List<ulong> gem_id_list = new List<ulong>();
	
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
	/// Use this for initialization
	void Start () {
		item_num.text = "1";
		
		UIEventListener.Get(add_num.gameObject).onPress -= ChangeNum;
		UIEventListener.Get(add_num.gameObject).onPress += ChangeNum;
		UIEventListener.Get(sub_num.gameObject).onPress -= ChangeNum;
		UIEventListener.Get(sub_num.gameObject).onPress += ChangeNum;
		
	}
	
	// Update is called once per frame
	void Update () {
		//防止panel initial之后SetActiveRecursively(true)导致面板的部分空间隐藏不可控制 
		if(EnableInatialPanel)
		{
			CangKuManager.RegisterCallBack(gameObject,"InitialData");
			CangKuManager.ReqCangKuData();
			//InitialData();
			EnableInatialPanel = false;
			//allitem_paging_storage.gameObject.SetActiveRecursively(false);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_ITEM);
		}
	}
	void RevealPanel()
	{
		tween_position.Play(true);
		EnableInatialPanel = true;
		allitem_tab.isChecked = true;
		embed_btn.gameObject.SetActiveRecursively(false);
		compose_btn.gameObject.SetActiveRecursively(false);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	
	//初始化面板数据  
	void InitialData()
	{
		//注销掉回调函数 
		CangKuManager.UnRegisterCallBack(gameObject);
		
		//List<uint> all_id_list = new List<uint>();
		//List<uint> equip_id_list = new List<uint>();
		
		RefreshPanelData();
		//ClearSelect();
	}
	//刷新面板数据 
	void RefreshPanelData()
	{
		CangKuManager.all_id_list.Clear();
		CangKuManager.equip_id_list.Clear();
		CangKuManager.gem_id_list.Clear();
		CangKuManager.article_id_list.Clear();
		CangKuManager.task_id_list.Clear();
		//print ("kkkkkkkkkkkkk");
		//foreach(DictionaryEntry de in CangKuManager.CangKuItemMap)
		for(int i=0;i<CangKuManager.Item_Id_List.Count;i++)
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[CangKuManager.Item_Id_List[i]];
			
			if((uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2] == (uint)ItemSort.EQUIP)
			{
				CangKuManager.equip_id_list.Add(item_info.nItemID1);
			}
			else if((uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2] == (uint)ItemSort.GEMSTONE)
			{
				CangKuManager.gem_id_list.Add(item_info.nItemID1);
			}
			else if((uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2] == (uint)ItemSort.ARTICLES)
			{
				CangKuManager.article_id_list.Add(item_info.nItemID1);
			}
			else if((uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2] == (uint)ItemSort.TASKITEM)
			{
				CangKuManager.task_id_list.Add(item_info.nItemID1);
			}
			CangKuManager.all_id_list.Add(CangKuManager.Item_Id_List[i]);
		}
		//if(allitem_tab.isChecked)
		//	allitem_paging_storage.gameObject.SetActiveRecursively(true);
		
		
		if(allitem_tab.isChecked)
		{
			CangKuManager.select_panel_sort = (uint)ItemSort.ALLITEM;
			//初始化全部面板  
			int capacity = CangKuManager.all_id_list.Count;
			
			allitem_paging_storage.SetCapacity(capacity);
			allitem_paging_storage.ResetAllSurfaces();
			List<GameObject> objs = allitem_paging_storage.GetAvailableItems();
			for(int i=0;i<objs.Count;i++)
			{
				UIEventListener.Get(objs[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(objs[i].gameObject).onClick += ShowDetail;
			}
			all_page_num = U3dCmn.GetPageNum(capacity,6); 
			page_num.text = (allitem_paging_storage.pagedDragID+1) +"/"+all_page_num;
			allitem_paging_storage.onDragingPage = ShowPageNum;
			ShowOperateUI((uint)ItemSort.ARTICLES);
		}
		else if(equip_tab.isChecked)
		{
			CangKuManager.select_panel_sort = (uint)ItemSort.EQUIP;
			//初始化装备面板   
			int capacity = CangKuManager.equip_id_list.Count;
			equip_paging_storage.SetCapacity(capacity);
			equip_paging_storage.ResetAllSurfaces();
			List<GameObject> objs = equip_paging_storage.GetAvailableItems();
			for(int i=0;i<objs.Count;i++)
			{
				UIEventListener.Get(objs[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(objs[i].gameObject).onClick += ShowDetail;
			}
			equip_page_num = U3dCmn.GetPageNum(capacity,6); 
			page_num.text = (equip_paging_storage.pagedDragID+1) +"/"+equip_page_num;
			equip_paging_storage.onDragingPage = ShowPageNum;
			ShowOperateUI((uint)ItemSort.EQUIP);

		}
		else if(gem_tab.isChecked)
		{
			CangKuManager.select_panel_sort = (uint)ItemSort.GEMSTONE;
			//初始化装备面板   
			int capacity = CangKuManager.gem_id_list.Count;
			gem_paging_storage.SetCapacity(capacity);
			gem_paging_storage.ResetAllSurfaces();
			List<GameObject> objs = gem_paging_storage.GetAvailableItems();
			for(int i=0;i<objs.Count;i++)
			{
				UIEventListener.Get(objs[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(objs[i].gameObject).onClick += ShowDetail;
			}
			gem_page_num = U3dCmn.GetPageNum(capacity,6); 
			page_num.text = (gem_paging_storage.pagedDragID+1)+"/"+gem_page_num;
			gem_paging_storage.onDragingPage = ShowPageNum;
			ShowOperateUI((uint)ItemSort.GEMSTONE);
		}
		else if(article_tab.isChecked)
		{
			CangKuManager.select_panel_sort = (uint)ItemSort.ARTICLES;
			//初始化装备面板   
			int capacity = CangKuManager.article_id_list.Count;
			article_paging_storage.SetCapacity(capacity);
			article_paging_storage.ResetAllSurfaces();
			List<GameObject> objs = article_paging_storage.GetAvailableItems();
			for(int i=0;i<objs.Count;i++)
			{
				UIEventListener.Get(objs[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(objs[i].gameObject).onClick += ShowDetail;
			}
			article_page_num = U3dCmn.GetPageNum(capacity,6); 
			page_num.text = (article_paging_storage.pagedDragID+1)+"/"+article_page_num;
			article_paging_storage.onDragingPage = ShowPageNum;
			ShowOperateUI((uint)ItemSort.ARTICLES);
		}
		else if(task_tab.isChecked)
		{
			CangKuManager.select_panel_sort = (uint)ItemSort.TASKITEM;
			//初始化装备面板   
			int capacity = CangKuManager.task_id_list.Count;
			task_paging_storage.SetCapacity(capacity);
			task_paging_storage.ResetAllSurfaces();
			List<GameObject> objs = task_paging_storage.GetAvailableItems();
			for(int i=0;i<objs.Count;i++)
			{
				UIEventListener.Get(objs[i].gameObject).onClick -= ShowDetail;
				UIEventListener.Get(objs[i].gameObject).onClick += ShowDetail;
			}
			task_page_num = U3dCmn.GetPageNum(capacity,6); 
			page_num.text = (task_paging_storage.pagedDragID+1)+"/"+task_page_num;
			task_paging_storage.onDragingPage = ShowPageNum;
			ShowOperateUI((uint)ItemSort.TASKITEM);
		}
		ClearSelect();
	}
	void ShowOperateUI(uint item_sort)
	{
		if(item_sort == (uint)ItemSort.EQUIP)
		{
			embed_btn.gameObject.SetActiveRecursively(true);
			compose_btn.gameObject.SetActiveRecursively(false);
			use_btn.gameObject.SetActiveRecursively(false);
		}
		else if(item_sort == (uint)ItemSort.GEMSTONE)
		{
			compose_btn.gameObject.SetActiveRecursively(true);
			use_btn.gameObject.SetActiveRecursively(false);
			embed_btn.gameObject.SetActiveRecursively(false);
		}
		else if(item_sort == (uint)ItemSort.ARTICLES)
		{
			compose_btn.gameObject.SetActiveRecursively(false);
			embed_btn.gameObject.SetActiveRecursively(false);
			if(select_excel_id == (int)ARTICLES.YINGXIONGCHENGZHANG || select_excel_id == (int)ARTICLES.YINGXIONGBANG || select_excel_id == (int)ARTICLES.YINGXIONGJINGYAN
				 || select_excel_id == (int)ARTICLES.QIANCHENGJUANZHOU || select_excel_id == (int)ARTICLES.ZHENGZHANJUANZHOU)
				use_btn.gameObject.SetActiveRecursively(false);
			else 
				use_btn.gameObject.SetActiveRecursively(true);
			
		}
		else if(item_sort == (uint)ItemSort.TASKITEM)
		{
			use_btn.gameObject.SetActiveRecursively(false);
			compose_btn.gameObject.SetActiveRecursively(false);
			embed_btn.gameObject.SetActiveRecursively(false);
		}
	}
	void ClearSelect()
	{
		List<GameObject> all_item_list = allitem_paging_storage.GetAvailableItems();
		for(int i=0; i<all_item_list.Count;i++)
		{
			CangKuItem item = all_item_list[i].GetComponent<CangKuItem>();
			
			item.SetSelect(false);
		}
		List<GameObject> equip_item_list = equip_paging_storage.GetAvailableItems();
		for(int i=0; i<equip_item_list.Count;i++)
		{
			CangKuItem item = equip_item_list[i].GetComponent<CangKuItem>();
			item.SetSelect(false);
		}
		List<GameObject> gem_item_list = gem_paging_storage.GetAvailableItems();
		for(int i=0; i<gem_item_list.Count;i++)
		{
			CangKuItem item = gem_item_list[i].GetComponent<CangKuItem>();
			item.SetSelect(false);
		}
		List<GameObject> article_item_list = article_paging_storage.GetAvailableItems();
		for(int i=0; i<article_item_list.Count;i++)
		{
			CangKuItem item = article_item_list[i].GetComponent<CangKuItem>();
			item.SetSelect(false);
		}
		List<GameObject> task_item_list = task_paging_storage.GetAvailableItems();
		for(int i=0; i<task_item_list.Count;i++)
		{
			CangKuItem item = task_item_list[i].GetComponent<CangKuItem>();
			item.SetSelect(false);
		}
		select_item_id = 0;
		item_name.text = "";
		item_describe.text = "";
		item_num.text = "1";
		sell_price_label.text = "";
		sell_unit_price = 0;
		//select_obj = null;
	}
	//选中显示 
	void ShowDetail(GameObject obj)
	{
		ClearSelect();
		select_obj = obj;
		CangKuItem item = obj.GetComponent<CangKuItem>();
		if(item.item_id!=0 && item.item_num >0)
		{
			
			item.SetSelect(true);
			select_item_id = item.item_id;
			item_name.text =item.name_label.text.Split('\n')[0];
			sell_unit_price = item.sell_price;
			sell_price_label.text = sell_unit_price.ToString();
			//total_price.text = goods_price.ToString();
			if(item.describe == null)
				item_describe.text ="";
			else
				item_describe.text = item.describe;
			
			uint sort = (uint) CommonMB.ExcelToSort_Map[(uint)item.excel_id];
			if(sort == (uint)ItemSort.EQUIP)
			{
				item_describe.text+=("\n"+U3dCmn.GetTipByExcelID((uint)item.excel_id));
			}
			
			//uint sort = (uint)CommonMB.ExcelToSort_Map[(uint)item.excel_id];
			
			select_excel_id = item.excel_id;
			ShowOperateUI(sort);
		}
		
		
	}
	//选中第一个ITEM 给引导系统使用 
	void SelectFirstItem()
	{
		List<GameObject> all_item_list = allitem_paging_storage.GetAvailableItems();
		ShowDetail(all_item_list[0]);
	}
	//刷新选中
	void RefreshSelect()
	{
		RefreshPanelData();
		select_obj = null;
		//ShowDetail(select_obj);
	}
	//更改当前页数（根据回调） 
	void ShowPageNum(int num)
	{
		int total_page_num = 1;
		if(allitem_tab.isChecked)
		{
			total_page_num = all_page_num;
		}
		else if(equip_tab.isChecked)
		{
			total_page_num = equip_page_num;
		}
		else if(gem_tab.isChecked)
		{
			total_page_num = gem_page_num;
		}
		else if(article_tab.isChecked)
		{
			total_page_num = article_page_num;
		}
		else if(task_tab.isChecked)
		{
			total_page_num = task_page_num;
		}
		page_num.text = (num+1)+"/"+total_page_num;
	}
	//更改数量 
	void ChangeNum(GameObject obj,bool ispressed)
	{
		int num =int.Parse(CommonMB.CheckNullStr(item_num.text));
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
	//改变数量 
	IEnumerator ChangeNumCoroutine(bool isadd)
	{
		int num =int.Parse(CommonMB.CheckNullStr(item_num.text));
		int select_item_num = 1;
		if(select_obj!=null)
		{
			CangKuItem item = select_obj.GetComponent<CangKuItem>();
			select_item_num = item.item_num;
		}
			
		
		while(true)
		{
			if(isadd)
			{
				num = num<select_item_num?(++num):select_item_num;
				item_num.text = num<1000?num.ToString():"1000";
			}
			else
			{
				item_num.text = num>1?(--num).ToString():"1";
			}
				
			ChangeSellPrice();
			yield return new WaitForSeconds(0.1f);
		}
	}
	//根据数量更改总价 
	void ChangeSellPrice()
	{
		if(U3dCmn.IsNumeric(item_num.text))
		{
			int num =int.Parse(CommonMB.CheckNullStr(item_num.text));
			sell_price_label.text = (sell_unit_price*num).ToString();
		}
		else
		{
			item_num.text = "1";
			sell_price_label.text = sell_unit_price.ToString();
		}
		
	}
	//卖掉物品 
	void SellItem()
	{
		if(select_item_id !=0)
		{
			string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_SELL_THIS_ITEM);
			PopConfirmWin.instance.ShowPopWin(str,SrueSellItem);
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ITEM_NOT_SELECT));
		}
	}
	void SrueSellItem(GameObject obj)
	{
		if(select_item_id !=0)
		{
			if(CangKuManager.GetSortIDByItemID(select_item_id) == (uint)ItemSort.EQUIP)
			{
				if(EquipManager.GetEmbedGemNum(select_item_id)>0)
				{
					U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.EMBED_EQUIP_NOT_SELL));
					return ;
				}
			}
			uint num =uint.Parse(CommonMB.CheckNullStr(item_num.text));
			CTS_GAMECMD_OPERATE_SELL_ITEM_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_SELL_ITEM;
			req.nItemID3 = select_item_id;
			req.nNum4 = num;
		
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_SELL_ITEM_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_SELL_ITEM);
		}
	}
	//卖掉物品返回  
	void SellItemRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_SELL_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_SELL_ITEM_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_SELL_ITEM_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Gold += sub_msg.nGold5;		
			PlayerInfoManager.RefreshPlayerDataUI();
			int num =int.Parse(CommonMB.CheckNullStr(item_num.text)); 
			CangKuManager.SubItem(select_item_id,num);
			InitialData();
			//RefreshPanelData();
		}
		else
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);
		}
	}
	// 合成宝石 
	void ComposeGem()
	{
		if(select_item_id !=0)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("ComposeGemWin"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel",select_item_id);
			}	
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ITEM_NOT_SELECT));
		}
	}
	//镶嵌宝石 
	void EmbedGem()
	{
		if(select_item_id !=0)
		{
			EmbedGemWin infowin = U3dCmn.GetObjFromPrefab("EmbedGemWin").GetComponent<EmbedGemWin>();
			if(infowin != null)
			{ 
				HireHero hero_unit = new HireHero();
				infowin.hero_info = hero_unit;
				infowin.equip_id = select_item_id;
				infowin.SendMessage("RevealPanel");
			}
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ITEM_NOT_SELECT));
		}
	}
	//使用道具 
	public void UseArticlesItem()
	{
		int num =int.Parse(CommonMB.CheckNullStr(item_num.text));
		//未选中道具 
		if(select_obj==null)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ITEM_NOT_SELECT));
			return ;
		}
			
		//数量不足 
		if(num>select_obj.GetComponent<CangKuItem>().item_num)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NUM_NOT_ENOUGH));
			return ;
		}
		
		if(select_item_id>0 && num >0 && select_excel_id>0)
		{
			//如果是草药包检查一下医馆的容量 
			if(select_excel_id == (int)ARTICLES.CAOYAOBAO)
			{
				ArticlesInfo articles_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)select_excel_id];
				if(articles_info.DescribeStr != null && articles_info.DescribeStr != "")
				{
					string[] str = articles_info.DescribeStr.Split('*');
					if(U3dCmn.IsNumeric(str[1]))
					{
						int caoyao_num = int.Parse(str[1])*num;
						if(TreatManager.GetHospitalCapacity()<(int)CommonData.player_online_info.CaoYao+caoyao_num)
						{
							string warn_str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.YIGUAN_LACK_SPACE);
							PopConfirmWin.instance.ShowPopWin(warn_str,SureUseArticlesItem);
							
							return ;
						}
					}
				}
	
			}
			//如果是人口卷轴检查一下民居的容量 
			/*if(select_excel_id == (int)ARTICLES.RENKOUJUANZHOU)
			{			
				int capacity = U3dCmn.GetMinJuCapacity() - (int)CommonData.player_online_info.Population;
				//print ("rrrrrrrrrrr"+U3dCmn.GetMinJuCapacity()+"  "+CommonData.player_online_info.Population);
				if(capacity <0)
					capacity = 0;
				ArticlesInfo equip_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)ARTICLES.RENKOUJUANZHOU];
				
				if(equip_info.EffectNum>capacity)
				{
					string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_USE_RENKOUJUANZHOU);
					PopConfirmWin.instance.ShowPopWin(str,SureUseArticlesItem);
					return;
				}
				//if(U3dCmn.get)
				
			}*/
			//如果是君主经验丹检查一下君主是否满级 
			if(select_excel_id == (int)ARTICLES.JUNZHUJINGYAN)
			{
				CharLevelExp char_level_exp = (CharLevelExp)CommonMB.CharLevelExp_Map[CommonData.player_online_info.Level];
				if(0== CommonData.player_online_info.Exp)
				{
					if(!CommonMB.CharLevelExp_Map.Contains(CommonData.player_online_info.Level+1))
					{
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.JUNZHU_TOP_LEVEL_NOT_ADD_EXP));
						return;
					}
				}
				//if(CommonMB.CharLevelExp_Map[utin])
			}
			//如果是免战检查一下是否处于荣耀之战  
			if(select_excel_id == (int)ARTICLES.MIANZHANPAI || select_excel_id == (int)ARTICLES.HOUR24_TRUCETOKEN || select_excel_id == (int)ARTICLES.HOUR10_TRUCETOKEN)
			{
			
				if(CommonData.player_online_info.HonorCup >0)
				{
					U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MIANZHAN_DISABLE_FOR_HONOR));
					return;
				}
				if((int)CommonData.player_online_info.ProtectEndTime - DataConvert.DateTimeToInt(DateTime.Now) > 0)
				{
					U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.IN_PROTECT_NO_TRUCE_TOKEN));
					return;
				}
				//if(CommonMB.CharLevelExp_Map[utin])
			}
			//如果是工匠之书 检查是否已经使用过  
			if(select_excel_id == (int)ARTICLES.SMITHBOOK || select_excel_id == (int)ARTICLES.DAY7_SMITHBOOK ||select_excel_id == (int)ARTICLES.HOUR24_SMITHBOOK || select_excel_id == (int)ARTICLES.HOUR48_SMITHBOOK)
			{
				
				if(CommonData.player_online_info.AddBuildNumTime > DataConvert.DateTimeToInt(DateTime.Now))
				{
					U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SMITHBOOK_STACK));
					return;
				}
			}
			CangKuManager.UseArticlesItem(select_excel_id,select_item_id,0,num);
		}
			
	}
	
	public void SureUseArticlesItem(GameObject obj)
	{
		int num =int.Parse(CommonMB.CheckNullStr(item_num.text));
		CangKuManager.UseArticlesItem(select_excel_id,select_item_id,0,num);
	}
	//根据物品的种类 隐藏掉使用按钮 
	void DisableUseBtn()
	{
		use_btn.gameObject.SetActiveRecursively(false);
	}
}
