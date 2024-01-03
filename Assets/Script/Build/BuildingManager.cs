using UnityEngine;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class BuildingManager : MonoBehaviour {
	//当前建造或者升级时间事件表 
	public static Hashtable BuildingTeMap = new Hashtable();
	//兵营数量 
	public static int BingYingNum= 8;
	//民居数量 
	public static int MinJuNum= 8;
	//金矿数量 
	public static int JinKuangNum=8;
	//兵营容量哈希表 主键为AUTO_ID  
	public static Hashtable  BingYing_Capacity_Map = new Hashtable();
	//民居容量哈希表 主键为AUTO_ID  
	public static Hashtable  MinJu_Capacity_Map = new Hashtable();
	//民居生产率哈希表 主键为AUTO_ID  
	public static Hashtable  MinJu_Product_Map = new Hashtable();
	
	//城堡 
	public GameObject ChengBaoObj;
	//研究院  
	public GameObject YanJiuYuanObj;
	//酒馆  
	public GameObject JiuGuanObj;
	//修炼馆  
	public GameObject XiuLianObj;
	//大使馆   
	public GameObject DashiGuanObj;
	//医馆   
	public GameObject HospitalObj;
	//摇钱树 
	public YaoQianShu YaoQianShuObj;
	//兵营哈希表 主键为AUTO_ID  
	public static Hashtable  BingYing_Map = new Hashtable();
	//兵营tile哈希表 主键为AUTO_ID  
	Hashtable  BingYingTile_Map = new Hashtable();
	//民居哈希表 主键为AUTO_ID  
	public static Hashtable  MinJu_Map = new Hashtable();
	//民居tile哈希表 主键为AUTO_ID  
	Hashtable  MinJuTile_Map = new Hashtable();
	//金矿哈希表 主键为AUTO_ID  
	public static Hashtable  JinKuang_Map = new Hashtable();
	//金矿tile哈希表 主键为AUTO_ID  
	Hashtable  JinKuangTile_Map = new Hashtable();

	
	//兵营面板TWEEN  
	public TweenPosition BingYingTween; 
	//民居面板TWEEN  
	public TweenPosition MinJuTween; 
	//金矿面板TWEEN  
	public TweenPosition JinKuangTween; 
	public UILabel JinKuangNumLabel;
	string jinkuangnum_str;
	//民居的信息面板 
	public UILabel MinJuProductLabel;
	public UILabel MinJuCapacityLabel;
	public UILabel MinJuNumLabel;
	string minjunum_str;
	//兵营的信息面板 
	public UILabel 	BingYingCapacityLabel;
	public UILabel	BingYingNumLabel;
	string bingyingnum_str;
	//世界钟表 
	public UILabel WorldTime;
	public static long WorldTimeModify = 0;
	//moregame 按钮 
	public GameObject MoreGameBtn;
	public static int UnreadNum = 1;
	public static bool IsOpenMoreGame = false;
	//金币交易市场按钮 
	public GameObject TradeMarketBtn;
	//竞技场按钮 
	public GameObject ArenaBtn;
	//建筑或者升级时间队列结构 
	public struct TimeTeUnit
	{
		//建造或者升级 
		public uint 	BuildState;
		//剩余时间 
		public uint time;
	}
	
	void Awake()
	{
		jinkuangnum_str = JinKuangNumLabel.text;
		bingyingnum_str = BingYingNumLabel.text;
		minjunum_str = MinJuNumLabel.text;
		JinKuangNumLabel.text = "";
		BingYingNumLabel.text = "";
		MinJuNumLabel.text = "";
		
		Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
		uint ShowMoreGame = 16;
		//出一个1-5的随机值 
		UnreadNum = UnityEngine.Random.Range(1,5);
		RefreshMoreGame();
		CheckMainCityObjOrUI();
		/*if (true == sMap.ContainsKey(ShowMoreGame))
		{
			CmnDefineMBInfo cmndef = (CmnDefineMBInfo) sMap[ShowMoreGame];
			if (cmndef.num == 1)
				MoreGameBtn.SetActiveRecursively(true);
			else
				MoreGameBtn.SetActiveRecursively(false);
		}*/
		
	}
	// Use this for initialization
	void Start () {
		InitialBingYing();
		InitialMinJu();
		InitialJinKuang();
		InitialOtherBuilding();
		GetBuildingList();
		GetBuildingTEList();
		GetYaoQianShuInfo();
		GetWorldTime();
		
		//aa.SetActiveRecursively(true);
	}
	//获取建筑列表 
	public  void GetBuildingList()
	{
		CTS_GAMECMD_GET_BUILDING_LIST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_BUILDING_LIST;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_BUILDING_LIST_T >(req);
	}
	//获取建筑时间队列  
	public  void GetBuildingTEList()
	{
		CTS_GAMECMD_GET_BUILDING_TE_LIST_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_BUILDING_TE_LIST;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_BUILDING_TE_LIST_T >(req);
	}
	//获取摇钱树当前信息 
	public void GetYaoQianShuInfo()
	{
		CTS_GAMECMD_GET_CHRISTMAS_TREE_INFO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_CHRISTMAS_TREE_INFO;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_CHRISTMAS_TREE_INFO_T >(req);
	}
	//获取摇钱树当前信息返回  
	public void GetYaoQianShuInfoRst(byte[] buff)
	{
		STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_GET_CHRISTMAS_TREE_INFO_T.enum_rst.RST_OK)
		{
			YaoQianShuObj.InitialData(sub_msg);
		}
		
	}
	//初始化其他建筑 
	void InitialOtherBuilding()
	{
		ChengBao.BuildState = (uint)BuildingState.NORMAL; // 新手引导时要用 ....
		YanJiuYuanObj.GetComponent<Institute>().BuildState = (uint)BuildingState.NORMAL;
		JiuGuanObj.GetComponent<JiuGuan>().BuildState = (uint)BuildingState.NORMAL;
		XiuLianObj.GetComponent<TrainGround>().BuildState = (uint)BuildingState.NORMAL;
		DashiGuanObj.GetComponent<DashiGuan>().BuildState = (uint)BuildingState.NORMAL;
		HospitalObj.GetComponent<Hospital>().BuildState = (uint)BuildingState.NORMAL;
	}
	//初始化兵营界面 
	void InitialBingYing()
	{
		BingYing_Map.Clear(); 
		BingYingTile_Map.Clear();
		BingYing_Capacity_Map.Clear();
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,1);
			//初始化兵营地块信息 
			for(int i=0;i<BingYingNum;i++)
			{
				string objname= "BingYing"+(i+1);
				GameObject obj = U3dCmn.SetObjVisible(BingYingTween.gameObject,objname,false);
				obj.GetComponent<BingYing>().BuildState = (uint)BuildingState.NORMAL;
				BingYing_Map.Add((int)info.BeginID+i+1,obj);
				objname= "BingYingTile"+(i+1);
				BingYingTile_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(BingYingTween.gameObject,objname,true));
			}
			//for(int i=tile_num;i<BingYingNum;i++)
			//{
			//	string objname= "BingYing"+(i+1);
			//	BingYing_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(objname,false));
			//	objname= "BingYingTile"+(i+1);
			//	BingYingTile_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(objname,false));
			//}
			//初始化兵营容量哈希表  
			for(int i=0;i<BingYingNum;i++)
			{
				BingYing_Capacity_Map.Add((uint)(info.BeginID+i+1),0);
			}
		}
		
	}
	//初始化民居界面 
	void InitialMinJu()
	{
		MinJu_Map.Clear();
		MinJuTile_Map.Clear();
		MinJu_Capacity_Map.Clear();
		MinJu_Product_Map.Clear();
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,1);
			//初始化民居地块信息 
			for(int i=0;i<MinJuNum;i++)
			{
				string objname= "MinJu"+(i+1);
				GameObject obj = U3dCmn.SetObjVisible(MinJuTween.gameObject,objname,false);
				obj.GetComponent<MinJu>().BuildState = (uint)BuildingState.NORMAL;
				MinJu_Map.Add((int)info.BeginID+i+1,obj);
				objname= "MinJuTile"+(i+1);
				MinJuTile_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(MinJuTween.gameObject,objname,true));
			}
			//for(int i=0;i<MinJuNum;i++)
			//{
			//	string objname= "MinJu"+(i+1);
			//	MinJu_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(objname,false));
			//	objname= "MinJuTile"+(i+1);
			//	MinJuTile_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(objname,false));
			//}
			//初始化民居容量哈希表  
			for(int i=0;i<MinJuNum;i++)
			{
				MinJu_Capacity_Map.Add((int)info.BeginID+i+1,0);
				MinJu_Product_Map.Add((int)info.BeginID+i+1,0);
			}
		}
	}
	//初始化金矿界面 
	void InitialJinKuang()
	{
		JinKuang_Map.Clear();
		JinKuangTile_Map.Clear();
		if(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
			//初始化金矿地块信息 
			for(int i=0;i<JinKuangNum;i++)
			{
				string objname= "JinKuang"+(i+1);
				GameObject obj = U3dCmn.SetObjVisible(JinKuangTween.gameObject,objname,false);
				obj.GetComponent<JinKuang>().BuildState = (uint)BuildingState.NORMAL;
				obj.GetComponent<JinKuang>().GoldState = (uint)ProduceState.NORMAL;
				JinKuang_Map.Add((int)info.BeginID+i+1,obj);
				objname= "JinKuangTile"+(i+1);
				JinKuangTile_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(JinKuangTween.gameObject,objname,true));
			}
			//for(int i=tile_num;i<JinKuangNum;i++)
			//{
			//	string objname= "JinKuang"+(i+1);
			//	JinKuang_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(objname,false));
			//	objname= "JinKuangTile"+(i+1);
			//	JinKuangTile_Map.Add((int)info.BeginID+i+1,U3dCmn.SetObjVisible(objname,false));
			//}
		}
	}
	//计算已经建造的兵营、民居、金矿的数量 
	public static int GetBuildingNum(int building_sort)
	{
		if (building_sort <1 || 
			building_sort> (int)BuildingSort.XUYUANSHU)
		{
			// 无效建筑 ...
			return 0;
		}
		
		int num = 0;
		if(building_sort == (int)BuildingSort.BINGYING)
		{
			foreach(DictionaryEntry de in BingYing_Map)
			{
				GameObject obj = (GameObject)de.Value;
				BingYing bingying = obj.GetComponent<BingYing>();
				if(bingying.NowLevel>0)
					num++;
			}
		}
		else if(building_sort == (int)BuildingSort.MINJU)
		{
			foreach(DictionaryEntry de in MinJu_Map)
			{
				GameObject obj = (GameObject)de.Value;
				MinJu minju = obj.GetComponent<MinJu>();
				if(minju.NowLevel>0)
					num++;
			}
		}
		else if(building_sort == (int)BuildingSort.JINKUANG)
		{
			foreach(DictionaryEntry de in JinKuang_Map)
			{
				GameObject obj = (GameObject)de.Value;
				JinKuang jinkuang = obj.GetComponent<JinKuang>();
				if(jinkuang.NowLevel>0)
					num++;
			}
		}
		else 
		{
			num = 1;
		}
		
		// Fini
		return num;
	}
	//计算已经建造的兵营、民居、金矿的数量 
	public static int GetRipeJinKuangNum()
	{
		int num = 0;
		foreach(DictionaryEntry de in JinKuang_Map)
		{
			GameObject obj = (GameObject)de.Value;
			JinKuang jinkuang = obj.GetComponent<JinKuang>();
			if(jinkuang.GoldState == (uint)ProduceState.RESUIT)
				num++;
		}
		return num;
	}
	
	//弹出兵营子界面 
	void RevealBingYingPanel()
	{
		RefreshBingYingInfo();
		BingYingTween.GetComponent<UIPanel>().enabled  = true;
		BingYingTween.Play(true);
		KeepOut.instance.ShowKeepOut();
	}
	//刷新兵营界面基本信息 
	void RefreshBingYingInfo()
	{
		BingYingCapacityLabel.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BINGYING_CAPACITY),SoldierManager.GetSoldierNum(),SoldierManager.GetBingYingCapacity());
		
		BuildingNum building_num_info = (BuildingNum)U3dCmn.GetBuildingNumByCharLevel(CommonData.player_online_info.Level);
		BuildingNum next_building_num_info = U3dCmn.GetNextCharLevelForBuilding((int)BuildingSort.BINGYING,building_num_info.CampNum);
		string str =  bingyingnum_str+ GetBuildingNum((int)BuildingSort.BINGYING)+"/"+building_num_info.CampNum;
		if(next_building_num_info.CampNum >0)
			str += "\n"+string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_DESCRIBE),next_building_num_info.LordLevel,next_building_num_info.CampNum);
		
		BingYingNumLabel.text = str;
		
		//BingYingNumLabel.text = GetBuildingNum((int)BuildingSort.BINGYING)+"/"+building_num_info.CampNum;
	}
	//关闭兵营子界面 
	void DismissBingYingPanel()
	{
		//给新手引导一个通知 ... 
		BingYingTween.Play(false);
		BingYingTween.GetComponent<UIPanel>().enabled  = false;
		KeepOut.instance.HideKeepOut();
	}
	//弹出民居子界面 ....
	void RevealMinJuPanel()
	{
		RefreshMinJuInfo();	
		MinJuTween.GetComponent<UIPanel>().enabled  = true;
		MinJuTween.Play(true);
		KeepOut.instance.ShowKeepOut();
	}
	//刷新民居界面基本信息 ...
	void RefreshMinJuInfo()
	{
		MinJuCapacityLabel.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MINJU_CAPACITY),CommonData.player_online_info.Population,U3dCmn.GetMinJuCapacity());
		MinJuProductLabel.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.JUMIN_PRODUCT),U3dCmn.GetMinJuProduct());
		BuildingNum building_num_info = (BuildingNum)U3dCmn.GetBuildingNumByCharLevel(CommonData.player_online_info.Level);
		BuildingNum next_building_num_info = U3dCmn.GetNextCharLevelForBuilding((int)BuildingSort.MINJU,building_num_info.HouseNum);
	
		string str =  minjunum_str+ GetBuildingNum((int)BuildingSort.MINJU)+"/"+building_num_info.HouseNum;
		if(next_building_num_info.HouseNum >0)
			str += "\n"+string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_DESCRIBE),next_building_num_info.LordLevel,next_building_num_info.HouseNum);
		else 
			str = "          "+str;
		MinJuNumLabel.text = str;
	}
	//关闭民居子界面 
	void DismissMinJuPanel()
	{
		MinJuTween.Play(false);
		MinJuTween.GetComponent<UIPanel>().enabled  = false;
		KeepOut.instance.HideKeepOut();
	}
	//弹出金矿子界面 
	void RevealJinKuangPanel()
	{
		RefreshJinKuangInfo();
		U3dCmn.GetObjFromPrefab("JinKuangFriendListWin");
		GetYaoQianShuInfo();
		JinKuangTween.GetComponent<UIPanel>().enabled  = true;
		JinKuangTween.Play(true);
		KeepOut.instance.ShowKeepOut();
	}
	//刷新金矿界面基本信息 
	void RefreshJinKuangInfo()
	{
		BuildingNum building_num_info = (BuildingNum)U3dCmn.GetBuildingNumByCharLevel(CommonData.player_online_info.Level);
		BuildingNum next_building_num_info = U3dCmn.GetNextCharLevelForBuilding((int)BuildingSort.JINKUANG,building_num_info.GoldNum);
		string str =  jinkuangnum_str+ GetBuildingNum((int)BuildingSort.JINKUANG)+"/"+building_num_info.GoldNum;
		if(next_building_num_info.GoldNum >0)
			str += "\n"+string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_DESCRIBE),next_building_num_info.LordLevel,next_building_num_info.GoldNum);
		
		JinKuangNumLabel.text = str;
	}
	//关闭金矿子界面 
	void DismissJinKuangPanel()
	{
		U3dCmn.SendMessage("JinKuangFriendListWin","DismissPanel",null);
		JinKuangTween.Play(false);
		JinKuangTween.GetComponent<UIPanel>().enabled  = false;
		KeepOut.instance.HideKeepOut();
		//JinKuangTween.gameObject.SetActiveRecursively(true);
	}
	//打开哨塔界面  
	void OpenShaoTaWin()
	{
        GameObject infowin = U3dCmn.GetObjFromPrefab("ShaoTaWin");
        if (infowin != null)
        {
            infowin.SendMessage("RevealPanel");
        }
	}
	//打开军情界面 ...
	void OnProcessListCombatDataDelegate()
	{
		LoadingManager.instance.HideLoading();
		GameObject obj = U3dCmn.GetObjFromPrefab("WarSituationWin");
		if(obj == null) return;
		
		WarSituationWin win = obj.GetComponent<WarSituationWin>();
		if (win != null) { 
			//win.WaitingMarchListCombat();
			win.ApplyWarSituation(); 
		}
	}
	
	void OpenWarSituationWin()
	{
		//print ("pppppppppppppppppppppppp");
		LoadingManager.instance.ShowLoading();
		CombatManager.processListCombatDataDelegate = OnProcessListCombatDataDelegate;
		CombatManager.ReqReinforceListData();
		CombatManager.RequestCombatListData();
		
	}
	//打开南征北战副本 
	void OpenNanZheng()
	{
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processListInstanceStatusDelegate = OnProcessOpenNanBeizhanDelegate;
		BaizInstanceManager.RequestListInstanceStatusData();
	}
	
	void OnProcessOpenNanBeizhanDelegate()
	{
		LoadingManager.instance.HideLoading();
		GameObject go = U3dCmn.GetObjFromPrefab("NanBeizhanLaunchWin");
		if (go == null) return;
		
		NanBeizhanLaunchWin t = go.GetComponent<NanBeizhanLaunchWin>();
		if (t != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyCDiTuLevel(1);
			t.ApplyBaizInstanceStatus();
		}
	}
		
	//打开世界金矿 
	void OpenWorldGold()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("WorldGoldmineScene");
		if (go == null) return;
		
		GoldmineScene t = go.GetComponent<GoldmineScene>();
		if (t != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.GetGoldmineArea(1);
			t.GetMyGoldmine();
			t.AssignComm();
		}
	}
	
	//打开百战副本 
	void OpenBaiZhan()
	{
		LoadingManager.instance.ShowLoading();
		BaizInstanceManager.processListInstanceStatusDelegate = OnProcessOpenBaizhanDelegate;
		BaizInstanceManager.RequestListInstanceStatusData();
	}
	
	void OnProcessOpenBaizhanDelegate()
	{
		LoadingManager.instance.HideLoading();
		GameObject go = U3dCmn.GetObjFromPrefab("BaizhanLaunchWin");
		if (go == null ) return;
		
		BaizhanLaunchWin t = go.GetComponent<BaizhanLaunchWin>();
		if (t != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyBaizInstanceStatus();
		}
	}
	
	//处理建筑列表信息 
	void ProcessBuidingList(byte[] buff)
	{
		STC_GAMECMD_GET_BUILDING_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_BUILDING_LIST_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_GET_BUILDING_LIST_T.enum_rst.RST_OK)
		{	
			//string msg = "building num:"+sub_msg.nNum2;
			//U3dCmn.ShowWarnWindow(msg);
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_BUILDING_LIST_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			BuildingUnit[] building_array = (BuildingUnit[])DataConvert.ByteToStructArray<BuildingUnit>(data_buff,sub_msg.nNum2);
			for(int i=0;i<building_array.Length;i++)
			{
				switch(building_array[i].nExcelID2)
				{
					case (uint)BuildingSort.CHENGBAO: //城堡  
					{
						//ChengBaoObj.SetActiveRecursively(true);
						ChengBaoObj.SendMessage("InitialChengBao",building_array[i].nLevel3);
						//ChengBao.NowLevel = building_array[i].nLevel3;
					}
					break;
					case (uint)BuildingSort.YANJIUYUAN: //研究院 
					{
						//YanJiuYuanObj.SetActiveRecursively(true);
						YanJiuYuanObj.SendMessage("InitialInstitute",building_array[i].nLevel3);
					}
					break;
					case (uint)BuildingSort.JIUGUAN: 	//酒馆 
					{
						//JiuGuanObj.SetActiveRecursively(true);
						JiuGuanObj.SendMessage("InitialJiuGuan",building_array[i].nLevel3);
					}
					break;
					case (uint)BuildingSort.XIULIAN: //修炼馆 
					{
						//XiuLianObj.SetActiveRecursively(true);
						XiuLianObj.SendMessage("InitialTrainGround",building_array[i].nLevel3);
					}
					break;
					case (uint)BuildingSort.DASHIGUAN: //大使馆 
					{
						//DashiGuanObj.SetActiveRecursively(true);
						DashiGuanObj.SendMessage("InitialDashiGuan",building_array[i].nLevel3);
					}
					break;
					case (uint)BuildingSort.YIGUAN: //医馆  
					{
						//HospitalObj.SetActiveRecursively(true);
						HospitalObj.SendMessage("InitialHospital",building_array[i].nLevel3);
					}
					break;
					case (uint)BuildingSort.BINGYING: //兵营 
					{
						GameObject obj= (GameObject)BingYing_Map[(int)building_array[i].nAutoID1];
						if(obj!=null)
						{
							obj.SetActiveRecursively(true);
							obj.SendMessage("InitialBingYing",building_array[i].nLevel3);
						}
						obj= (GameObject)BingYingTile_Map[(int)building_array[i].nAutoID1];
						if(obj!=null)
							obj.SetActiveRecursively(false);
					}
					break;
					case (uint)BuildingSort.MINJU: //民居 
					{
						GameObject obj= (GameObject)MinJu_Map[(int)building_array[i].nAutoID1];
						if(obj!=null)
						{
							obj.SetActiveRecursively(true);
							obj.SendMessage("InitialMinJu",building_array[i].nLevel3);
						}
						obj= (GameObject)MinJuTile_Map[(int)building_array[i].nAutoID1];
						if(obj!=null)
							obj.SetActiveRecursively(false);
					}
					break;
					case (uint)BuildingSort.JINKUANG: //金矿  
					{
						GameObject obj= (GameObject)JinKuang_Map[(int)building_array[i].nAutoID1];
						if(obj!=null)
						{
							obj.SetActiveRecursively(true);
							obj.SendMessage("InitialJinKuang",building_array[i].nLevel3);
						}
						obj= (GameObject)JinKuangTile_Map[(int)building_array[i].nAutoID1];
						if(obj!=null)
							obj.SetActiveRecursively(false);
					}
					break;
					default:
						break;
				
				}
			}

		}
		
	}
	//处理建筑时间队列 
	void ProcessBuidTeList(byte[] buff)
	{
		STC_GAMECMD_GET_BUILDING_TE_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_BUILDING_TE_LIST_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_GET_BUILDING_TE_LIST_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_BUILDING_TE_LIST_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			BuildingTEUnit[] build_te_list = (BuildingTEUnit[])DataConvert.ByteToStructArray<BuildingTEUnit>(data_buff,sub_msg.nNum3);
			BuildingTeMap.Clear();
			for(int i=0;i<build_te_list.Length;i++)
			{
				switch(build_te_list[i].nExcelID4)
				{
					case (uint)BuildingSort.CHENGBAO: //城堡  
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =build_te_list[i].nType5;
						timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
						ChengBaoObj.SendMessage("InitialTimeTe",timeunit);
					}
					break;
					case (uint)BuildingSort.YANJIUYUAN: //研究院 
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =build_te_list[i].nType5;
						timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
						YanJiuYuanObj.SendMessage("InitialTimeTe",timeunit);
						
					}
					break;
					case (uint)BuildingSort.JIUGUAN: 	//酒馆 
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =build_te_list[i].nType5;
						timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
						JiuGuanObj.SendMessage("InitialTimeTe",timeunit);
						//JiuGuanObj.SetActiveRecursively(true);
					}
					break;
					case (uint)BuildingSort.XIULIAN: //修炼馆  
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =build_te_list[i].nType5;
						timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
						XiuLianObj.SendMessage("InitialTimeTe",timeunit);
						//TieJiangPuObj.SetActiveRecursively(true);
					}
					break;
					case (uint)BuildingSort.DASHIGUAN: //大使馆 
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =build_te_list[i].nType5;
						timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
						DashiGuanObj.SendMessage("InitialTimeTe",timeunit);
					}
					break;
					case (uint)BuildingSort.YIGUAN: //医馆  
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =build_te_list[i].nType5;
						timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
						HospitalObj.SendMessage("InitialTimeTe",timeunit);
					}
					break;
					case (uint)BuildingSort.BINGYING: //兵营 
					{
						GameObject obj= (GameObject)BingYing_Map[(int)build_te_list[i].nAutoID3];
						GameObject tileobj= (GameObject)BingYingTile_Map[(int)build_te_list[i].nAutoID3];
						if(obj!=null)
						{
							obj.SetActiveRecursively(true);
							tileobj.SetActiveRecursively(false);
							TimeTeUnit timeunit;
							timeunit.BuildState =build_te_list[i].nType5;
							timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
							obj.SendMessage("InitialTimeTe",timeunit);
						}
					}
					break;
					case (uint)BuildingSort.MINJU: //民居 
					{
						GameObject obj= (GameObject)MinJu_Map[(int)build_te_list[i].nAutoID3];
						GameObject tileobj= (GameObject)MinJuTile_Map[(int)build_te_list[i].nAutoID3];
						if(obj!=null)
						{
							obj.SetActiveRecursively(true);
							tileobj.SetActiveRecursively(false);
							TimeTeUnit timeunit;
							timeunit.BuildState =build_te_list[i].nType5;
							timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
							obj.SendMessage("InitialTimeTe",timeunit);
						}
					}
					break;
					case (uint)BuildingSort.JINKUANG:  //金矿    
					{
						GameObject obj= (GameObject)JinKuang_Map[(int)build_te_list[i].nAutoID3];
						GameObject tileobj= (GameObject)JinKuangTile_Map[(int)build_te_list[i].nAutoID3];
						if(obj!=null)
						{
							obj.SetActiveRecursively(true);
							tileobj.SetActiveRecursively(false);
							TimeTeUnit timeunit;
							timeunit.BuildState =build_te_list[i].nType5;
							timeunit.time=build_te_list[i].nEndTime2-sub_msg.nTimeNow2;
							obj.SendMessage("InitialTimeTe",timeunit);
						}
					}
					break;
					default:
						break;
				
				}
				BuildingTEUnit TeUnit = build_te_list[i];
				TeUnit.nEndTime2 = (uint)(build_te_list[i].nEndTime2-sub_msg.nTimeNow2+DataConvert.DateTimeToInt(DateTime.Now));
				BuildingTeMap.Add(build_te_list[i].nAutoID3,TeUnit);
			}
		}
	}
	
	//请求建造返回信息  
	void ProcessBuidRst(byte[] buff)
	{
	
		//更新玩家基本信息 
		PlayerInfoManager.GetPlayerSmpInfo();
		STC_GAMECMD_OPERATE_BUILD_BUILDING_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_BUILD_BUILDING_T>(buff);
		CommonData.player_online_info.Development = sub_msg.nAddedCharDev5;
		switch(sub_msg.nRst1)
		{
			case (int)STC_GAMECMD_OPERATE_BUILD_BUILDING_T.enum_rst.RST_OK:
			{
					
				if(sub_msg.nExcelID2 == (uint)BuildingSort.BINGYING)
				{
					GameObject obj= (GameObject)BingYing_Map[(int)(sub_msg.nAutoID3)];
					if(obj!=null)
					{
						obj.SetActiveRecursively(true);
						obj.SendMessage("BuildBeginCountdown");
					}
					obj= (GameObject)BingYingTile_Map[(int)(sub_msg.nAutoID3)];
					if(obj!=null)
						obj.SetActiveRecursively(false);
					
					
				}
				else if(sub_msg.nExcelID2 == (uint)BuildingSort.MINJU)
				{
					GameObject obj= (GameObject)MinJu_Map[(int)(sub_msg.nAutoID3)];
					if(obj!=null)
					{
						obj.SetActiveRecursively(true);
						obj.SendMessage("BuildBeginCountdown");
					}
					obj= (GameObject)MinJuTile_Map[(int)(sub_msg.nAutoID3)];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				else if(sub_msg.nExcelID2 == (uint)BuildingSort.JINKUANG)
				{
					GameObject obj= (GameObject)JinKuang_Map[(int)(sub_msg.nAutoID3)];
					if(obj!=null)
					{
						
						obj.SetActiveRecursively(true);
						obj.SendMessage("BuildBeginCountdown");
					}
					obj= (GameObject)JinKuangTile_Map[(int)(sub_msg.nAutoID3)];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				//U3dCmn.ShowWarnWindow("build success");
			
				// <新手引导> 建筑建设返回处理 ....
				if ( NewbieHouse.processNewbieBuildingRst != null)
				{
					NewbieHouse.processNewbieBuildingRst();
					NewbieHouse.processNewbieBuildingRst = null;
				}
			}
			break;
			case (int)STC_GAMECMD_OPERATE_BUILD_BUILDING_T.enum_rst.RST_GOLD_NOT_ENOUGH:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MONEY_NOT_ENOUGH));
			}
			break;
			case (int)STC_GAMECMD_OPERATE_BUILD_BUILDING_T.enum_rst.RST_POPULATION_NOT_ENOUGH:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PEOPLE_NOT_ENOUGH));
			}
			break;
			case (int)STC_GAMECMD_OPERATE_BUILD_BUILDING_T.enum_rst.RST_BUILDING_QUEUE_ENOUGH:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILD_QUE_ENOUGH));
			}
			break;
			case (int)STC_GAMECMD_OPERATE_BUILD_BUILDING_T.enum_rst.RST_BUILDING_NUM_MAX:
			{
				if(sub_msg.nExcelID2 == (uint)BuildingSort.BINGYING)
				{
					BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,1);
					BuildingNum building_num_info = (BuildingNum)U3dCmn.GetBuildingNumByCharLevel(CommonData.player_online_info.Level);
					BuildingNum next_building_num_info = U3dCmn.GetNextCharLevelForBuilding((int)BuildingSort.BINGYING,building_num_info.CampNum);
					string warn_str = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_MAX),info.Name,next_building_num_info.LordLevel);
					U3dCmn.ShowWarnWindow(warn_str);
				}
				else if(sub_msg.nExcelID2 == (uint)BuildingSort.MINJU)
				{
					BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,1);
					BuildingNum building_num_info = (BuildingNum)U3dCmn.GetBuildingNumByCharLevel(CommonData.player_online_info.Level);
					BuildingNum next_building_num_info = U3dCmn.GetNextCharLevelForBuilding((int)BuildingSort.MINJU,building_num_info.HouseNum);
					string warn_str = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_MAX),info.Name,next_building_num_info.LordLevel);
					U3dCmn.ShowWarnWindow(warn_str);
				}
				else if(sub_msg.nExcelID2 == (uint)BuildingSort.JINKUANG)
				{
					BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
					BuildingNum building_num_info = (BuildingNum)U3dCmn.GetBuildingNumByCharLevel(CommonData.player_online_info.Level);
					BuildingNum next_building_num_info = U3dCmn.GetNextCharLevelForBuilding((int)BuildingSort.JINKUANG,building_num_info.GoldNum);
					string warn_str = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_MAX),info.Name,next_building_num_info.LordLevel);
					U3dCmn.ShowWarnWindow(warn_str);
				}
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILDING_NUM_MAX));
			}
			break;
			default:
			 	//U3dCmn.ShowWarnWindow("build err"+sub_msg.nRst1);
			break;
			
		}
	}
	//请求升级返回信息  
	void ProcessUpgradeRst(byte[] buff)
	{
		//更新玩家基本信息 
		PlayerInfoManager.GetPlayerSmpInfo();
		STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T>(buff);
		//CommonData.player_online_info.Exp = sub_msg.nAddedCharDev5;
		
		switch(sub_msg.nRst1)
		{
			case (int)STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T.enum_rst.RST_OK:
			{
				switch(sub_msg.nExcelID3)
				{
					case (uint)BuildingSort.CHENGBAO: //城堡  
					{
						ChengBaoObj.SendMessage("UpgradeBeginCountdown",null);
						// 关闭请求的窗口 ...
						U3dCmn.SendMessage("ChengBaoInfoWin","DismissPanel", null);
					}
					break;
					case (uint)BuildingSort.JIUGUAN: //酒馆  
					{
						JiuGuanObj.SendMessage("UpgradeBeginCountdown",null);
					}
					break;
					case (uint)BuildingSort.YANJIUYUAN: //研究院   
					{
						YanJiuYuanObj.SendMessage("UpgradeBeginCountdown",null);
					}
					break;
					case (uint)BuildingSort.XIULIAN: //修炼馆    
					{
						XiuLianObj.SendMessage("UpgradeBeginCountdown",null);
					}
					break;
					case (uint)BuildingSort.DASHIGUAN: //大使馆    
					{
						DashiGuanObj.SendMessage("UpgradeBeginCountdown",null);
					}
					break;
					case (uint)BuildingSort.YIGUAN: //医馆      
					{
						HospitalObj.SendMessage("UpgradeBeginCountdown",null);
					}
					break;
					case (uint)BuildingSort.BINGYING: //兵营 
					{
						GameObject obj= (GameObject)BingYing_Map[(int)(sub_msg.nAutoID2)];
						obj.SendMessage("UpgradeBeginCountdown");
					}
					break;
					case (uint)BuildingSort.MINJU: //民居  
					{
						GameObject obj= (GameObject)MinJu_Map[(int)(sub_msg.nAutoID2)];
						obj.SendMessage("UpgradeBeginCountdown");
						U3dCmn.SendMessage("MinJuInfoWin", "DismissPanel", null);
					}
					break;
					case (uint)BuildingSort.JINKUANG: //金矿   
					{
						GameObject obj= (GameObject)JinKuang_Map[(int)(sub_msg.nAutoID2)];
						obj.SendMessage("UpgradeBeginCountdown");
					}
					break;
				}
			}
			break;
			case (int)STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T.enum_rst.RST_GOLD_NOT_ENOUGH:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MONEY_NOT_ENOUGH));
			}
			break;
			case (int)STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T.enum_rst.RST_POPULATION_NOT_ENOUGH:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PEOPLE_NOT_ENOUGH));
			}
			break; 
			case (int)STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T.enum_rst.RST_BUILDING_LEVEL_MAX:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILD_LEVEL_LIMIT));
			}
			break;
			case (int)STC_GAMECMD_OPERATE_UPGRADE_BUILDING_T.enum_rst.RST_BUILDING_QUEUE_ENOUGH:
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BUILD_QUE_ENOUGH));
			}
			break;
			default:
			 	U3dCmn.ShowWarnWindow("upgrade err"+sub_msg.nRst1);
			break;
		}
			
	}
	//建造或者升级时间事件完成结果处理 
	void ProcessBuidEventRst(byte[] buff)
	{
		//更新玩家基本信息 
		PlayerInfoManager.GetPlayerSmpInfo();
		STC_GAMECMD_BUILDING_TE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_BUILDING_TE_T>(buff);
		BuildingTeMap.Remove(sub_msg.nAutoID1);
		if(sub_msg.nType3 == (uint)BuildingState.BUILDING)
		{
			switch(sub_msg.nExcelID2)
			{
				case (uint)BuildingSort.BINGYING: //兵营 
				{
					//U3dCmn.ShowWarnWindow(String.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BINGYING_BUILD_OK),sub_msg.nAutoID1));
					GameObject obj= (GameObject)BingYing_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
					{
									
						obj.SetActiveRecursively(true);
						obj.SendMessage("ProcessBuildRst");
					}
					obj= (GameObject)BingYingTile_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				break;
				case (uint)BuildingSort.MINJU: //民居 
				{
					//U3dCmn.ShowWarnWindow(String.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MINJU_BUILD_OK),sub_msg.nAutoID1));
					GameObject obj= (GameObject)MinJu_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
					{
									
						obj.SetActiveRecursively(true);
						obj.SendMessage("ProcessBuildRst");
					}
					obj= (GameObject)MinJuTile_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
						obj.SetActiveRecursively(false);
					
				}
				break;
				case (uint)BuildingSort.JINKUANG: //金矿  
				{
				
					GameObject obj= (GameObject)JinKuang_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
					{
									
						obj.SetActiveRecursively(true);
						obj.SendMessage("ProcessBuildRst");
					}
					obj= (GameObject)JinKuangTile_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				break;
				default:
					break;
			}
		}
		else if(sub_msg.nType3 == (uint)BuildingState.UPGRADING)
		{
			switch(sub_msg.nExcelID2)
			{
				case (uint)BuildingSort.CHENGBAO: //城堡  
				{
					//ChengBaoObj.SetActiveRecursively(true);
					ChengBaoObj.SendMessage("ProcessUpgradeRst",null);
				}
				break;
				case (uint)BuildingSort.YANJIUYUAN: //研究院 
				{
					//YanJiuYuanObj.SetActiveRecursively(true);
					YanJiuYuanObj.SendMessage("ProcessUpgradeRst",null);
				}
				break;
				case (uint)BuildingSort.JIUGUAN: 	//酒馆 
				{
					//JiuGuanObj.SetActiveRecursively(true);
					JiuGuanObj.SendMessage("ProcessUpgradeRst",null);
				}
				break;
				case (uint)BuildingSort.XIULIAN: //修炼馆  
				{
					// XiuLianObj.SetActiveRecursively(true);
					XiuLianObj.SendMessage("ProcessUpgradeRst",null);
				}
				break;
				case (uint)BuildingSort.DASHIGUAN: //大使馆  
				{
					//DashiGuanObj.SetActiveRecursively(true);
					DashiGuanObj.SendMessage("ProcessUpgradeRst",null);
				}
				break;
				case (uint)BuildingSort.YIGUAN: //医馆 
				{
					//HospitalObj.SetActiveRecursively(true);
					HospitalObj.SendMessage("ProcessUpgradeRst",null);
				}
				break;
				case (uint)BuildingSort.BINGYING: //兵营 
				{
					//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BINGYING_UPGRADE_OK));
					GameObject obj= (GameObject)BingYing_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
					{
									
						obj.SetActiveRecursively(true);
						obj.SendMessage("ProcessUpgradeRst");
					}
					obj= (GameObject)BingYingTile_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				break;
				case (uint)BuildingSort.MINJU: //民居 
				{
					//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MINJU_UPGRADE_OK));
					GameObject obj= (GameObject)MinJu_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
					{
									
						obj.SetActiveRecursively(true);
						obj.SendMessage("ProcessUpgradeRst");
					}
					obj= (GameObject)MinJuTile_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				break;
				case (uint)BuildingSort.JINKUANG: //金矿  
				{
					GameObject obj= (GameObject)JinKuang_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
					{
									
						obj.SetActiveRecursively(true);
						obj.SendMessage("ProcessUpgradeRst");
					}
					obj= (GameObject)JinKuangTile_Map[(int)sub_msg.nAutoID1];
					if(obj!=null)
						obj.SetActiveRecursively(false);
				}
				break;
				default:
					break;
			}
		}
			
	}
	//建筑加速 返回  
	void ProcessAcceRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_ACCE_BUILDING_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_ACCE_BUILDING_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_ACCE_BUILDING_T.enum_rst.RST_OK)
		{
			if(sub_msg.nMoneyType5 == (uint)money_type.money_type_crystal)
			{
				CommonData.player_online_info.Crystal -= sub_msg.nMoneyNum4;
			}
			else if(sub_msg.nMoneyType5 == (uint)money_type.money_type_diamond)
			{
				CommonData.player_online_info.Diamond -= sub_msg.nMoneyNum4;
			}
			PlayerInfoManager.RefreshPlayerDataUI();
			if(BuildingManager.BuildingTeMap.Contains(sub_msg.nAutoID2))
			{
				BuildingTEUnit teunit =  (BuildingTEUnit)BuildingManager.BuildingTeMap[sub_msg.nAutoID2];
				teunit.nAutoID3 = sub_msg.nAutoID2;
				teunit.nEndTime2 = (uint)DataConvert.DateTimeToInt(DateTime.Now)+ sub_msg.nTime3;
				teunit.nExcelID4 = (uint)sub_msg.nExcelID6;
				teunit.nType5 = (uint)BuildingState.BUILDING;
				BuildingManager.BuildingTeMap[sub_msg.nAutoID2] = teunit; 
			}
			switch(sub_msg.nExcelID6)
			{
				case (uint)BuildingSort.CHENGBAO: //城堡  
				{
					//ChengBaoObj.SetActiveRecursively(true);	
					if(sub_msg.nTime3 >0)
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =sub_msg.nType7;
						timeunit.time=sub_msg.nTime3;
						ChengBaoObj.SendMessage("InitialTimeTe",timeunit);
					}
					GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
					if(win != null)
					{
						win.SendMessage("CompleteAccelerate");
					}
					
				}
				break;
				case (uint)BuildingSort.YANJIUYUAN: //研究院 
				{
					if(sub_msg.nTime3 >0)
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =sub_msg.nType7;
						timeunit.time=sub_msg.nTime3;
						YanJiuYuanObj.SendMessage("InitialTimeTe",timeunit);
					}
					GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
					if(win != null)
					{
						win.SendMessage("CompleteAccelerate");
					}
				}
				break;
				case (uint)BuildingSort.JIUGUAN: 	//酒馆 
				{
					//JiuGuanObj.SetActiveRecursively(true);	
					if(sub_msg.nTime3 >0)
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =sub_msg.nType7;
						timeunit.time=sub_msg.nTime3;
						JiuGuanObj.SendMessage("InitialTimeTe",timeunit);
					}
					GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
					if(win != null)
					{
						win.SendMessage("CompleteAccelerate");
					}
				}
				break;
				case (uint)BuildingSort.XIULIAN: 	//修炼馆  
				{
					//XiuLianObj.SetActiveRecursively(true);	
					if(sub_msg.nTime3 >0)
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =sub_msg.nType7;
						timeunit.time=sub_msg.nTime3;
						XiuLianObj.SendMessage("InitialTimeTe",timeunit);
					}
					GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
					if(win != null)
					{
						win.SendMessage("CompleteAccelerate");
					}
				}
				break;
				case (uint)BuildingSort.DASHIGUAN: 	//大使馆  
				{
					//DashiGuanObj.SetActiveRecursively(true);	
					if(sub_msg.nTime3 >0)
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =sub_msg.nType7;
						timeunit.time=sub_msg.nTime3;
						DashiGuanObj.SendMessage("InitialTimeTe",timeunit);
					}
					GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
					if(win != null)
					{
						win.SendMessage("CompleteAccelerate");
					}
				}
				break;
				case (uint)BuildingSort.YIGUAN: 	//医馆   
				{
					//HospitalObj.SetActiveRecursively(true);	
					if(sub_msg.nTime3 >0)
					{
						TimeTeUnit timeunit;
						timeunit.BuildState =sub_msg.nType7;
						timeunit.time=sub_msg.nTime3;
						HospitalObj.SendMessage("InitialTimeTe",timeunit);
					}
					GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
					if(win != null)
					{
						win.SendMessage("CompleteAccelerate");
					}
				}
				break;
				case (uint)BuildingSort.BINGYING: //兵营 
				{
					GameObject obj= (GameObject)BingYing_Map[(int)sub_msg.nAutoID2];
					if(obj!=null)
					{			
						if(sub_msg.nTime3 >0)
						{
							TimeTeUnit timeunit;
							timeunit.BuildState =sub_msg.nType7;
							timeunit.time=sub_msg.nTime3;
							obj.SendMessage("InitialTimeTe",timeunit);
						}
						GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
						if(win != null)
						{
							win.SendMessage("CompleteAccelerate");
						}
					}
					
				}
				break;
				case (uint)BuildingSort.MINJU: //民居 
				{
					
					GameObject obj= (GameObject)MinJu_Map[(int)sub_msg.nAutoID2];
					if(obj!=null)
					{
						if(obj!=null)
						{			
							if(sub_msg.nTime3 >0)
							{
								TimeTeUnit timeunit;
								timeunit.BuildState =sub_msg.nType7;
								timeunit.time=sub_msg.nTime3;
								obj.SendMessage("InitialTimeTe",timeunit);
							}
							GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
							if(win != null)
							{
								win.SendMessage("CompleteAccelerate");
							}
						}		
					}
					
				}
				break;
				case (uint)BuildingSort.JINKUANG: //金矿 
				{
					
					GameObject obj= (GameObject)JinKuang_Map[(int)sub_msg.nAutoID2];
					if(obj!=null)
					{
						if(obj!=null)
						{			
							if(sub_msg.nTime3 >0)
							{
								TimeTeUnit timeunit;
								timeunit.BuildState =sub_msg.nType7;
								timeunit.time=sub_msg.nTime3;
								obj.SendMessage("InitialTimeTe",timeunit);
							}
							GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
							if(win != null)
							{
								win.SendMessage("CompleteAccelerate");
							}
						}		
					}
					
				}
				break;
			}
		}
		else
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.UNKNOWN_ERR)+sub_msg.nRst1);
			GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
			if(win != null)
			{
				win.SendMessage("CompleteAccelerate");
			}
		}
	}
	//请求建造金矿 (提供给新手引导使用) 
	public static void BuildJinKuang(uint AutoID)
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
			CTS_GAMECMD_OPERATE_BUILD_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BUILD_BUILDING;
			req.nExcelID3 = (uint)info.ID;
			req.nAutoID4  = info.BeginID+AutoID;
			//print ("bingying"+req.nExcelID3 +" "+req.nAutoID4 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUILD_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING);
		}
	} 
	//请求建造民居  (提供给新手引导使用)
	public static void BuildMinJu(uint AutoID)
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,1);
			CTS_GAMECMD_OPERATE_BUILD_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BUILD_BUILDING;
			req.nExcelID3 = (uint)info.ID;
			req.nAutoID4  = info.BeginID+AutoID;
			//print ("bingying"+req.nExcelID3 +" "+req.nAutoID4 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUILD_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING);
		}
	} 
	//请求建造兵营  (提供给新手引导使用)
	public static void BuildBingYing(uint AutoID)
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,1);
			CTS_GAMECMD_OPERATE_BUILD_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BUILD_BUILDING;
			req.nExcelID3 = (uint)info.ID;
			req.nAutoID4  = info.BeginID+AutoID;
			//print ("bingying"+req.nExcelID3 +" "+req.nAutoID4 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUILD_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING);
		}
	} 
	//打开相应兵营界面  (提供给新手引导使用) ...
	public static void OpenBingYingWin(int AutoID)
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,1);
		
		GameObject obj= (GameObject)BingYing_Map[(int)info.BeginID+AutoID];
		if (obj != null) { 
			obj.SendMessage("OpenBingYingInfoWin");
		}
	} 
	//打开相应的民居界面 (提供给新手引导使用) ...
	public static void OpenMinJuWin(int AutoID)
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,1);
		GameObject obj = (GameObject)MinJu_Map[(int)info.BeginID+AutoID];
		if (obj != null) {
			obj.SendMessage("OpenMinJuInfoWin");
		}
	}
	//打开相应的金矿界面 (提供给新手引使用) ....
	public static void OpenJinKuangWin(int AutoID)
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.JINKUANG,1);
		GameObject obj = (GameObject)JinKuang_Map[(int)info.BeginID+AutoID];
		if (obj != null) {
			obj.SendMessage("OpenJinKuangInfoWin");
		}
	}
	
	//工匠之书到期 
	public void AddBuildingListOut(byte[]  buff)
	{
		STC_GAMECMD_BUILD_ITEM_USE_OUT_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_BUILD_ITEM_USE_OUT_T>(buff);
		CommonData.player_online_info.TotalBuildNum = (uint)sub_msg.nCurrentBuildNum1;
	}
	//打开设置面板 
	public void OpenSettingWin()
	{
		GameObject buildwin = U3dCmn.GetObjFromPrefab("SettingWin");
		if(buildwin != null)
		{
			buildwin.SendMessage("RevealPanel");
		}
	}
	//打开金矿的说明 
	public void OpenJinKuangExplainWin()
	{
		GameObject infowin = U3dCmn.GetObjFromPrefab("BulletinWin");
		if(infowin != null)
		{
			InstanceMBInfo info = (InstanceMBInfo)CommonMB.InstanceMBInfo_Map[(uint)HELP_TYPE.GOLD_HELP];
			infowin.GetComponent<BulletinWin>().title_label.text = info.mode;
			infowin.GetComponent<BulletinWin>().text_label.text = info.Rule1;
			infowin.SendMessage("RevealPanel");
		}
	}
	//获取世界时间 
	void GetWorldTime()
	{
		CTS_GAMECMD_GET_SERVER_TIME_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_SERVER_TIME;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_SERVER_TIME_T >(req);
	}
		
	//世界钟表开始倒计时 
	void BeginWorldTimeCutDown(byte[] buff)
	{
		STC_GAMECMD_GET_SERVER_TIME_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_SERVER_TIME_T>(buff);
		WorldTimeModify = (long)DataConvert.DateTimeToInt(DateTime.Now) - (long)sub_msg.nTimeNow1;
		StopCoroutine("WorldTimeCutDown");
		//int NowTime = (int)(DataConvert.DateTimeToInt(DateTime.Now)+time);
		WorldTime.gameObject.SetActiveRecursively(true);
		StartCoroutine("WorldTimeCutDown");
	}
	//世界钟表开始倒计时  
	IEnumerator WorldTimeCutDown()
	{
		uint sec = 0;
		while(true)
		{
			//sec++;
			uint world_time = (uint)(DataConvert.DateTimeToInt(DateTime.Now)-WorldTimeModify);
			WorldTime.text = DataConvert.WorldTimeIntStr(world_time);
			//print (DataConvert.WorldTimeIntStr(world_time+sec));
			yield return new WaitForSeconds(1);
		}
	}
	//打开MOREGAME 
	void OpenMoreGame()
	{
	
	}
	//设置MOREGAME按钮是否可见 
	void RefreshMoreGame()
	{
		Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
		uint ShowMoreGame = 16;
		
		if (true == sMap.ContainsKey(ShowMoreGame))
		{
			CmnDefineMBInfo cmndef = (CmnDefineMBInfo) sMap[ShowMoreGame];
			if (cmndef.num == 1)
			{
				MoreGameBtn.SetActiveRecursively(true);
				if(IsOpenMoreGame)
					U3dCmn.GetChildObjByName(MoreGameBtn,"UnRead").SetActiveRecursively(false);	
				else 
				{	
					MoreGameBtn.GetComponentInChildren<UILabel>().text = UnreadNum.ToString();
				}
			}
			else
				MoreGameBtn.SetActiveRecursively(false);
		}
	}
	//检查是否有需要关闭的主场景或主UI 
	void CheckMainCityObjOrUI()
	{
		if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.GOLD_TRADE_MARKET) == 0)
		{
			TradeMarketBtn.SetActiveRecursively(false);
		}
		else
		{
			TradeMarketBtn.SetActiveRecursively(true);
		}
		if(U3dCmn.GetCmnDefNum((uint)CMN_DEF.HERO_ARENA) == 0)
		{
			ArenaBtn.SetActiveRecursively(false);
		}
		else
		{
			ArenaBtn.SetActiveRecursively(true);
		}
	}
	//打开金币交易市场 
	void OpenTradeMarketWin()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("TradeMarketWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",null);
		}
	}
	//打开竞技场 
	void OpenArenaWin()
	{
		if(CommonData.player_online_info.Level < CommonMB.Arena_Info.LevelLimit)
		{
			U3dCmn.ShowWarnWindow(string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ARENA_LORD_LEVEL_LIMIT),CommonMB.Arena_Info.LevelLimit));
			return ;
		}
		GameObject obj  = U3dCmn.GetObjFromPrefab("ArenaWin");
		if (obj != null)
		{
			obj.SendMessage("RevealPanel",null);
		}	
	}
}
