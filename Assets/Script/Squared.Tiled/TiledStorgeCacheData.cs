using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;

// 地图区域块 (20x20)小块地图 ...
public class TiledCacheUnit
{	
	public enum UnitStatus
	{
		None = 0,
		Ready = 1,
		Loading = 2,
		ReadyCity = 3,
	};
	
	public int tx = 0;
	public int ty = 0;
	public UnitStatus st = UnitStatus.None;
	public UnitStatus st1 = UnitStatus.None;
	public TiledCacheEntry[] Tiles;
	public TiledCityEntry[] CityCards;
	public TiledResEntry[] ResCards;
}

// 地图的地理信息 ...
public class TiledCacheEntry
{
	public int g1 = 0;
	public int g2 = 0;
	public bool canBuild = true;
}

// 地图的城市信息 ...
public class TiledCityEntry
{
	public ulong nAccountID = 0; // 玩家帐号 ...
	public string name;				// 玩家名字 ...
	public int level = 0;			// 玩家等级 ...
	public int vipLevel = 0;		// VIP等级 ...
	public ulong nAllianceID = 0;	// 联盟ID ....
	public string allianceName;		// 联盟名字 ...
	public uint nPosX;				// 坐标 X ...
	public uint nPosY;				// 坐标 Y ...
	public uint nProtectTime;		// 保护时间 ...
}
// 世界资源的城市信息 ...
public class TiledResEntry
{
	public ulong		nID;	//唯一ID 
	public uint			nType;	//种类 
	public uint			nLevel;	//等级 
	public uint			nPosX;	//坐标X 
	public uint			nPosY; 	//坐标Y 
}
// 加载世界大地图的时候自动定位地图坐标 ...
static public class GlobeMapLocation
{
	static public byte byFastLocation; 		// 快速定位 ...
	static public int PosX; 				// 世界地图坐标 (X) ..
	static public int PosY; 				// 世界地图坐标 (Y) ..
}

public class TiledStorgeCacheData : MonoBehaviour {
		
	static TiledStorgeCacheData mInst = null;
	
	const int _CAPACITY = 20;	
	const int CACHE_CAPACITY = 16;
	
	public GameObject Button1 = null;		// 定位地图坐标 ...
	public GameObject Button2 = null;		// 定位我的城市 ...
	public GameObject Button3 = null;		// 签城 ...
	public GameObject Button4 = null;		// 圣杯查询 ...
	public GameObject _Indicator = null;
	public GameObject HourGlass = null;
	
	int mapFocusX = -1;
	int mapFocusY = -1;
	
	public TiledDraggablePanel GPS = null;		// 地图滑动组件 ...
	bool mIsAutoCityMovable = false;
	
	// 锁定哪块是我的地盘 ...
	TiledStorgeSlot mySlot1 = null;
	
	// 迁城响应处理回调 ...
	public delegate void ProcessCityMovableDelegate();
	static public ProcessCityMovableDelegate processCityMovableDelegate;
	
	// 名城列表 ...
	public delegate void ProcessHonorCityDelegate();
	static public ProcessHonorCityDelegate processHonorCityDelegate;
	public static ArrayList HonorCityList = new ArrayList();
	public static Hashtable HonorCityMap = new Hashtable();
	public static int HonorWarRestTime = 0;					// 离本次战争结束时间 ...
	public static int HonorCityRefreshTime = 0;				// 离下次城市刷新时间 ...
	
	// units for cache data ( 缓存临近的16块20x20格子区域图块 )
	TiledCacheUnit[] mCacheUnits = new TiledCacheUnit[CACHE_CAPACITY];
	bool mDelayLoading = false;
	
	// 请求 (20x20格子区域的列表 )
	SortedList<string, TiledCacheUnit> mLoadingUnits = new SortedList<string, TiledCacheUnit>();
	
	/// <summary>
	/// The instance of the TiledStorgeCacheData class. Will create it if one isn't already around.
	/// </summary>

	static public TiledStorgeCacheData instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(TiledStorgeCacheData)) as TiledStorgeCacheData;

				if (mInst == null)
				{
					GameObject go = new GameObject("_TiledStorgeCacheData");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<TiledStorgeCacheData>();
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	
	// Use this for initialization
	void Start () {
		
		if (Button1 != null) {
			UIEventListener.Get(Button1).onClick = VoidButton1Delegate;
		}
		if (Button2 != null) {
			UIEventListener.Get(Button2).onClick = VoidButton2Delegate;
		}
		if (Button3 != null) {
			UIEventListener.Get(Button3).onClick = VoidButton3Delegate;
		}
		if (Button4 != null) {
			UIEventListener.Get(Button4).onClick = VoidButton4Delegate;
		}
		if (_Indicator != null) {
			NGUITools.SetActive(_Indicator, false);
		}
		if (HourGlass != null) {
			NGUITools.SetActive(HourGlass, false);
		}
		
		// 获取世界名城信息 ...
		
		TiledStorgeCacheData.RequestHonorCity();
		LoadingManager.instance.HideLoading();
		
	}
	// 断线重连 ...
	void DisbandForUnpack1()
	{
		if (GPS != null) 
		{			
			GPS.ResetPacket1();
		}
	}
	// 解除焦点 ...
	public void GPSDisbandFocus(TiledStorgeSlot con1)
	{	
		if (_Indicator == null ) return;
		
		int X = con1.mapX;
		int Y = con1.mapY;
		
		Transform t = _Indicator.transform;
		if (t.parent == con1.transform)
		{
			bool c = (mapFocusX == X && mapFocusY == Y);
			NGUITools.SetActive(_Indicator, c);
		}
	}
	// 焦点 ... 
	public void GPSFocus(TiledStorgeSlot con1)
	{
		bool FCS = false;
		int mapx = con1.mapX;
		int mapy = con1.mapY;
		if (mapx == mapFocusX && mapy == mapFocusY) 
		{
			mapFocusX = -1;
			mapFocusY = -1;
			FCS = false;
		}
		else 
		{
			mapFocusX = mapx;
			mapFocusY = mapy;
			FCS = true;
		}
		
		if (_Indicator != null) 
		{
			if (FCS == true) 
			{ 
				Transform t = _Indicator.transform;
				_Indicator.transform.parent = con1.transform;
				_Indicator.transform.localPosition = Vector3.zero;
			}
			
			NGUITools.SetActive(_Indicator, FCS);
		}
	}
	// 重新加载地盘的数据块 ...
	public void GPSDisbandUnit(int nPosX, int nPosY)
	{
		if (GPS == null) return;
		if (nPosX<0 || nPosX>399 || nPosY<0 || nPosY>399) return;
		
		int tile_x = nPosX/_CAPACITY;
		int tile_y = nPosY/_CAPACITY;
		
		int tx1 = tile_x-1;
		int tx2 = tile_x+1;
		int ty1 = tile_y-1;
		int ty2 = tile_y+1;
		
		// search can be use
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			TiledCacheUnit t = mCacheUnits[i];
			if (t == null) continue;
			
			if ((t.tx == tile_x && t.ty == tile_y) ||
				(t.tx == tx1 && t.ty == tile_y) ||
				(t.tx == tx2 && t.ty == tile_y) ||
				(t.tx == tile_x && t.ty == ty1) ||
				(t.tx == tile_x && t.ty == ty2) )
			{
				mCacheUnits[i] = null;
			}
		}
		
		GPS.ResetPacket1();
	}
	
	// 迁城指示标 ...
	void VoidButton3Delegate(GameObject tween)
	{
		bool movable = mIsAutoCityMovable;
		mIsAutoCityMovable = (movable==false);
		
		if (GPS != null) {
			GPS.ResetPacket1();
		}
	}
	
	// 搜索定义玩家位置 ...
	void VoidButton1Delegate(GameObject tween)
	{
		// 搜索 ...
		GameObject go = U3dCmn.GetObjFromPrefab("WorldMapLocateWin");
		if (go == null) return;
		
		WorldMapLocate t = go.GetComponent<WorldMapLocate>();
		if (t != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET * 3.0f;
			t.Depth(depth);
		}
	}
	
	// 我的城市坐标 ...
	void VoidButton2Delegate(GameObject go)
	{
		// 我的城市 ...
		if (GPS != null)
		{
			CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
			GPS.SetLocation((int)info.PosX, (int) info.PosY);
		}
	}
	
	void VoidButton4Delegate(GameObject tween)
	{
		/*GameObject go = U3dCmn.GetObjFromPrefab("HonorInfoWin");
		if (go == null) return;
		
		HonorInfoWin win1 = go.GetComponent<HonorInfoWin>();
		if ( win1 != null ) 
		{
			win1.SendMessage("RevealPanel");
		}*/
		
		GameObject win = U3dCmn.GetObjFromPrefab("FavoriteWin");
		if (win == null) return;
		win.SendMessage("RevealPanel");
	}
	
	// 请求圣杯城市, HonorCity
	static public void RequestHonorCity()
	{
		CTS_GAMECMD_WORLDCITY_GET_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_WORLDCITY_GET;
		TcpMsger.SendLogicData<CTS_GAMECMD_WORLDCITY_GET_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_WORLDCITY_GET);
	}
	
	static public void ProcessHonorCityMap(byte[] data)
	{
		STC_GAMECMD_WORLDCITY_GET_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_WORLDCITY_GET_T>(data);
		if(msgCmd.nRst1 == (int)STC_GAMECMD_WORLDCITY_GET_T.enum_rst.RST_OK)
		{
			HonorCityRefreshTime = msgCmd.nRefreshTime3;
			HonorWarRestTime = msgCmd.nEndTime4;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_WORLDCITY_GET_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(data,head_len,data_buff,0,data_len);
			WorldCity[] city_list = (WorldCity[])DataConvert.ByteToStructArray<WorldCity>(data_buff,msgCmd.nNum2);
			HonorCityMap.Clear();
			HonorCityList.Clear();
			for(int i=0,imax=city_list.Length;i<imax;i++)
			{
				WorldCity card = city_list[i];
				HonorCityMap.Add(card.nID1, card);
				HonorCityList.Add( card);
			}
			
			// 名城回调函数 ...
			if (processHonorCityDelegate != null)
			{
				processHonorCityDelegate();
			}
		}
		
		// 取空吧 ...
		processHonorCityDelegate = null;
	}
	
	// 我的地盘 ...
	static public void GPSMySlot1(TiledStorgeSlot my_slot1)
	{
		if (mInst == null) return;
		mInst.mySlot1 = my_slot1;
	}
	// 消除保护圈子 ....
	static public void GPSRemoveProtect()
	{
		if (mInst == null) return;
		if (mInst.mySlot1 == null) return;
		
		// 清除保护时间 ...
		CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
		TiledCityEntry objEntry = null;
		int PosX = mInst.mySlot1.mapX;
		int PosY = mInst.mySlot1.mapY;
		if (PosX == (int)info.PosX && PosY == (int)info.PosY)
		{
			mInst.GetCity((int)info.PosX, (int)info.PosY, out objEntry);
			if (objEntry != null) objEntry.nProtectTime = 0;
			mInst.mySlot1.ResetItem();
		}
	}
	
	// 地图定位接口 ...
	public void GPSSetLocation(int x, int y)
	{
		if (GPS == null) return;		
		if (x<0 || x>399 || y<0 || y>399) return;
		GPS.SetLocation(x, y);
	}
	
	public bool isCityMovable
	{
		get {
			return mIsAutoCityMovable;
		}
		set {
			mIsAutoCityMovable = value;
		}
	}
	
	// -- 延时请求 ...
	public bool isAutoDelay
	{
		get {
			return this.mDelayLoading;
		}
		set {
			
			if (true == value)
			{
				StopCoroutine("AutoCooldown");
				this.mDelayLoading = value;
			}
			else 
			{
				StartCoroutine("AutoCooldown");
			}
		}
	}
	
	// 当手指滑动消失后 0.3f 允许加载地图信息 ...
	IEnumerator AutoCooldown()
	{
		yield return new WaitForSeconds(0.3f);
		this.mDelayLoading = false;
	}
	
	// Process data from server
	public void ProcessTiledCacheData(byte[] data)
	{
		STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN_T>(data);
		
		int tile_x = (int)(msgCmd.nFromX1/_CAPACITY);
		int tile_y = (int)(msgCmd.nFromY2/_CAPACITY);
		
		TiledCacheUnit go = null;
		string s = string.Format("{0},{1}",tile_x,tile_y);
		if (false == mLoadingUnits.TryGetValue(s,out go))
		{
			return;
		}
		
		mLoadingUnits.Remove(s);
		
		// -- 当前标记置为可用 ...
		go.st = TiledCacheUnit.UnitStatus.Ready;
		if (go.Tiles != null)
		{
			System.Array.Clear(go.Tiles,0,_CAPACITY*_CAPACITY);
		}
		
		if (msgCmd.nNum5>0)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			TerrainUnit[] tile_array = (TerrainUnit[])DataConvert.ByteToStructArray<TerrainUnit>(data_buff,(int)msgCmd.nNum5);	
			for (int i=0; i<msgCmd.nNum5; ++ i)
			{
				TerrainUnit tile = tile_array[i];
				int x = (int)(tile.nPosX1 - msgCmd.nFromX1);
				int y = (int)(tile.nPosY2 - msgCmd.nFromY2);
				if (x<0 || y<0 || y>=_CAPACITY || x>=_CAPACITY) continue;
				
				TiledCacheEntry t = new TiledCacheEntry();
				t.g1 = tile.nFloor4;
				t.g2 = tile.nFloor5;
				t.canBuild = (tile.canBuild3 == 1);
				go.Tiles[y*_CAPACITY + x] = t;
			}
		}
		
		// 沙漏显示 关闭 ....
		//print ("STC_GAMECMD_GET_WORLD_AREA_INFO_TERRAIN_T:" + msgCmd.nNum5);
	}
	
	// Process city data from server
	public void ProcessTiledCacheCity(byte[] data)
	{
		STC_GAMECMD_GET_WORLD_AREA_INFO_CITY_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_WORLD_AREA_INFO_CITY_T>(data);
		int tile_x = (int)(msgCmd.nFromX1/_CAPACITY);
		int tile_y = (int)(msgCmd.nFromY2/_CAPACITY);
		
		TiledCacheUnit go = null;
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			go = mCacheUnits[i];
			if (go == null) continue;
			if (go.tx == tile_x && go.ty == tile_y) 
			{
				break;
			}
		}
		
		if (go == null) return;
		
		// -- 当前标记置为可用 ...
		go.st1 = TiledCacheUnit.UnitStatus.ReadyCity;
		if (go.CityCards != null)
		{
			int cityMax = _CAPACITY*_CAPACITY;
			System.Array.Clear(go.CityCards,0,cityMax);
		}
		
		if (msgCmd.nNum5 > 0)
		{	
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_WORLD_AREA_INFO_CITY_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			CityCard[] card_array = (CityCard[])DataConvert.ByteToStructArray<CityCard>(data_buff,(int)msgCmd.nNum5);
			for (int i=0; i<msgCmd.nNum5; ++ i)
			{
				CityCard city = card_array[i];
				int x = (int)(city.nPosX9 - msgCmd.nFromX1);
				int y = (int)(city.nPosY10 - msgCmd.nFromY2);
				if (x<0 || y<0 || y>=_CAPACITY || x>=_CAPACITY) continue;
				
				TiledCityEntry t = new TiledCityEntry();
				t.nAccountID = city.nAccountID1;
				t.name = DataConvert.BytesToStr(city.szName3);
				t.nPosX = city.nPosX9;
				t.nPosY = city.nPosY10;
				t.level = (int) city.nLevel4;
				t.vipLevel = (int) city.nVip5;
				t.nAllianceID = city.nAllianceID6;
				t.nProtectTime = city.nProtectTime11;
				t.allianceName = DataConvert.BytesToStr(city.szAllianceName8);
				
				go.CityCards[y*_CAPACITY + x] = t;
			}
		}
		
		//print ("STC_GAMECMD_GET_WORLD_AREA_INFO_CITY:" + msgCmd.nNum5);
	}
	// 处理世界资源列表数据 
	public void ProcessWorldResCityList(byte[] data)
	{
		STC_GAMECMD_GET_WOLRD_RES_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_WOLRD_RES_T>(data);
		int tile_x = (int)(msgCmd.nFromX1/_CAPACITY);
		int tile_y = (int)(msgCmd.nFromY2/_CAPACITY);
	
		TiledCacheUnit go = null;
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			go = mCacheUnits[i];
			if (go == null) continue;
			if (go.tx == tile_x && go.ty == tile_y) 
			{
				break;
			}
		}
		
		if (go == null)return;
		
		// -- 当前标记置为可用 ...
		go.st1 = TiledCacheUnit.UnitStatus.ReadyCity;
		if (go.ResCards != null)
		{
			int cityMax = _CAPACITY*_CAPACITY;
			System.Array.Clear(go.ResCards,0,cityMax);
		}
		if (msgCmd.nNum5 > 0)
		{	
			
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_WOLRD_RES_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			WorldResSimpleInfo[] res_array = (WorldResSimpleInfo[])DataConvert.ByteToStructArray<WorldResSimpleInfo>(data_buff,(int)msgCmd.nNum5);
			for (int i=0; i<msgCmd.nNum5; ++ i)
			{
				
				WorldResSimpleInfo rescity = res_array[i];
				int x = (int)(rescity.nPosX4 - msgCmd.nFromX1);
				int y = (int)(rescity.nPosY5 - msgCmd.nFromY2);
				if (x<0 || y<0 || y>=_CAPACITY || x>=_CAPACITY) continue;
				TiledResEntry t = new TiledResEntry();
				t.nID = rescity.nID1;
				t.nLevel = rescity.nLevel3;
				t.nPosX = rescity.nPosX4;
				t.nPosY = rescity.nPosY5;
				t.nType = rescity.nType2;
				// = rescity.;
				//t.g2 = rescity.nFloor5;
				//t.canBuild = (tile.canBuild3 == 1);
				go.ResCards[y*_CAPACITY + x] = t;
				//print ("uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu"+y*_CAPACITY+ x);
			}
			if(GPS !=null)
				GPS.ResetPacket1(); // 立即更新
		}
		
		//print ("STC_GAMECMD_GET_WORLD_AREA_INFO_CITY:" + msgCmd.nNum5);
	}
	// Request tile data from x,y 
	static public void RequestTiledCacheData(int tile_x, int tile_y)
	{
		CTS_GAMECMD_GET_WORLD_AREA_INFO_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 	= (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_WORLD_AREA_INFO;
		req.nPosX3 		= (uint)(tile_x * _CAPACITY);
		req.nPosY4 		= (uint)(tile_y * _CAPACITY);
		req.nToPosX5 	= (uint)(tile_x * _CAPACITY + _CAPACITY - 1);
		req.nToPosY6 	= (uint)(tile_y * _CAPACITY + _CAPACITY - 1);
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_WORLD_AREA_INFO_T>(req);
	}
	
	static bool isValidUnit(int tile_x, int tile_y)
	{
		return (tile_x>=0 && tile_x<20 && tile_y>=0 && tile_y<20);
	}
	
	// 根据坐标获取区域缓存块 ...
	public TiledCacheUnit LoadUnit(int tile_x, int tile_y, out TiledCacheUnit.UnitStatus state)
	{
		// search can be use
		int firstgid = CACHE_CAPACITY;
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			TiledCacheUnit t = mCacheUnits[i];
			if (t != null && t.tx == tile_x && t.ty == tile_y)
			{
				// Find result direct return;
				state = TiledCacheUnit.UnitStatus.Loading;
				return t;
			}
			else if (t == null && i<firstgid)
			{
				// Find the rest result
				firstgid = i;
				break;
			}
		}
		
		// network don't working...
		if (TcpMsger.tcpstate == TcpSate.nothing) 
		{
			state = TiledCacheUnit.UnitStatus.None;
			return null;
		}
		
		// Delay loading ...
		if (mDelayLoading == true)
		{
			state = TiledCacheUnit.UnitStatus.Loading;
			return null;
		}
		
		// ok we need to find a place in the cache
		if (firstgid == CACHE_CAPACITY)
		{
			int score, maxscore = 0, maxgid = 0;
			// oh shit we need to throw away a tile
			for (int j=0; j<CACHE_CAPACITY; ++ j)
			{
				TiledCacheUnit t = mCacheUnits[j];
				if (t == null) continue;
				
				score = Mathf.Abs(t.tx-tile_x) + Mathf.Abs(t.ty-tile_y);
				if (score>maxscore)
				{
					maxscore = score;
					maxgid = j;
				}
			}
			
			// maxidx is the winner (loser)
			firstgid = maxgid;
		}
		
		TiledCacheUnit gUnit = mCacheUnits[firstgid];
		if (gUnit == null)
		{
			gUnit = new TiledCacheUnit();
			gUnit.CityCards = new TiledCityEntry[_CAPACITY*_CAPACITY];
			gUnit.Tiles = new TiledCacheEntry[_CAPACITY*_CAPACITY];
			gUnit.ResCards = new  TiledResEntry[_CAPACITY*_CAPACITY];
			mCacheUnits[firstgid] = gUnit;
		}
		
		gUnit.tx = tile_x;
		gUnit.ty = tile_y;
		gUnit.st = TiledCacheUnit.UnitStatus.Loading;
		gUnit.st1 = TiledCacheUnit.UnitStatus.Loading;
		state = TiledCacheUnit.UnitStatus.Loading;
		
		// Request
		string s = string.Format("{0},{1}",tile_x,tile_y);
		if (false == mLoadingUnits.ContainsKey(s))
		{
			mLoadingUnits.Add(s, gUnit);
			
			// Commit request
			RequestTiledCacheData(tile_x, tile_y);
			
			// 沙漏显示 ....
		}
		
		// Fini
		return gUnit;
	}
	
	// 获取单元格子的地理缓存信息 ... 
	public TiledCacheUnit.UnitStatus GetEntry(int X, int Y, out TiledCacheEntry floorCard)
	{
		floorCard = null;
		
		int tile_x = X/_CAPACITY;
		int offset_x = X - tile_x*_CAPACITY;
		int tile_y = Y/_CAPACITY;
		int offset_y = Y - tile_y*_CAPACITY;
		
		// Choose unit for loading
		TiledCacheUnit.UnitStatus state = TiledCacheUnit.UnitStatus.None;
		TiledCacheUnit go = LoadUnit(tile_x,tile_y,out state);
		if (go == null) return  state; // Delay loading... or net break;
		if (go.st == TiledCacheUnit.UnitStatus.Ready)
		{
			int gid = offset_y*_CAPACITY + offset_x;
			floorCard = go.Tiles[gid];
		}

		// Fini
		return (go.st);
	}
	
	// 获取单元格子上的城市信息 ...
	public TiledCacheUnit.UnitStatus GetCity(int X, int Y, out TiledCityEntry cityCard)
	{
		cityCard = null;
		
		int tile_x = X/_CAPACITY;
		int offset_x = X - tile_x*_CAPACITY;
		int tile_y = Y/_CAPACITY;
		int offset_y = Y - tile_y*_CAPACITY;
		
		TiledCacheUnit go = null;
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			go = this.mCacheUnits[i];
			if (go == null) continue;
			if (go.tx == tile_x && go.ty == tile_y)
			{
				break;
			}
		}
		
		if (go == null) return TiledCacheUnit.UnitStatus.None;
		if (go.st1 == TiledCacheUnit.UnitStatus.ReadyCity)
		{
			if (go.CityCards != null)
			{
				int gid = offset_y*_CAPACITY + offset_x;
				cityCard =  go.CityCards[gid];
			}
		}

		// Fini
		return (go.st1);
	}
	// 获取单元格子上的世界资源信息 ...
	public TiledCacheUnit.UnitStatus GetResCity(int X, int Y, out TiledResEntry ResCard)
	{
		ResCard = null;
		
		int tile_x = X/_CAPACITY;
		int offset_x = X - tile_x*_CAPACITY;
		int tile_y = Y/_CAPACITY;
		int offset_y = Y - tile_y*_CAPACITY;
		TiledCacheUnit go = null;
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			go = this.mCacheUnits[i];
			if (go == null) continue;
			if (go.tx == tile_x && go.ty == tile_y)
			{
				break;
			}
		}
		
		if (go == null) return TiledCacheUnit.UnitStatus.None;
		if (go.st1 == TiledCacheUnit.UnitStatus.ReadyCity)
		{
			if (go.ResCards != null)
			{
				int gid = offset_y*_CAPACITY + offset_x;
				ResCard =  go.ResCards[gid];
			}
		}

		// Fini
		return (go.st1);
	}
	//删除某一个世界资源 
	public bool DeleteWorldResCity(int X, int Y)
	{
		TiledResEntry ResCard= null;
		bool rst  =false;
		int tile_x = X/_CAPACITY;
		int offset_x = X - tile_x*_CAPACITY;
		int tile_y = Y/_CAPACITY;
		int offset_y = Y - tile_y*_CAPACITY;
		TiledCacheUnit go = null;
		for (int i=0; i<CACHE_CAPACITY; ++ i)
		{
			go = this.mCacheUnits[i];
			if (go == null) continue;
			if (go.tx == tile_x && go.ty == tile_y)
			{
				break;
			}
		}
		
		if (go == null) return false;
		if (go.st1 == TiledCacheUnit.UnitStatus.ReadyCity)
		{
			if (go.ResCards != null)
			{
				int gid = offset_y*_CAPACITY + offset_x;
				ResCard =  go.ResCards[gid];
				if(ResCard!=null)
				{
					go.ResCards[gid] = null;
					rst = true;
				}
			}
		}
		return rst;
		
	}
	// 发起迁城指令 ...
	static public void RequestCityMoveTo(int nPosX, int nPosY)
	{
		CTS_GAMECMD_OPERATE_MOVE_CITY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 	= (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_MOVE_CITY;
		req.nPosX3 = (uint) nPosX;
		req.nPosY4 = (uint) nPosY;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_MOVE_CITY_T>(req);
	}
	
	// 处理迁城指令回执 ...
	public void ProcessCityMovableData(byte[] data)
	{
		STC_GAMECMD_OPERATE_MOVE_CITY_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_MOVE_CITY_T>(data);
		if (msgCmd.nRst1 == (int) STC_GAMECMD_OPERATE_MOVE_CITY_T.enum_rst.RST_OK)
		{
			// 迁城道具减1
			int ExcelID = 2004; // 道具列表 ID
			ItemDesc desc = CangKuManager.GetItemInfoByExcelID(ExcelID);
			if (desc.nExcelID2 == ExcelID)
			{
				CangKuManager.SubItem(desc.nItemID1,1); // 迁城都得花钱 ...
			}
			if (processCityMovableDelegate != null)
			{
				processCityMovableDelegate();
			}
		}
		else 
		{
			LoadingManager.instance.HideLoading();
			int Tipset = 0;
			switch (msgCmd.nRst1)
			{
			case (int)STC_GAMECMD_OPERATE_MOVE_CITY_T.enum_rst.RST_ALREADY_PLAYER:
				{
					Tipset = BaizVariableScript.CITY_MOVABLE_RST_ALREADY_PLAYER;
				} break;
			case (int)STC_GAMECMD_OPERATE_MOVE_CITY_T.enum_rst.RST_CANNOT_BUILD_CITY:
				{
					Tipset = BaizVariableScript.CITY_MOVABLE_RST_CANNOT_BUILD;
				} break;
			case (int)STC_GAMECMD_OPERATE_MOVE_CITY_T.enum_rst.RST_NO_ITEM:
				{
					Tipset = BaizVariableScript.CITY_MOVABLE_RST_ITEM_IS_NULL;
				} break;
			case (int)STC_GAMECMD_OPERATE_MOVE_CITY_T.enum_rst.RST_CANNOT_MOVE_CITY:
				{
					Tipset = BaizVariableScript.CITY_MOVABLE_RST_CANNOT_MOVE;
				} break;
			}
			
			PopTipDialog.instance.VoidSetText1(true, false, Tipset);
		}
		
		processCityMovableDelegate = null;
		//print ("STC_GAMECMD_OPERATE_MOVE_CITY:" + msgCmd.nRst1);
	}
	//请求世界资源详细信息 
	public void ReqResCityInfo(uint PosX,uint PosY,ulong CityID)
	{
		CTS_GAMECMD_GET_WORLD_RES_CARD_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 	= (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_WORLD_RES_CARD;
		req.nID3 = CityID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_WORLD_RES_CARD_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_WORLD_RES_CARD);
	}
	
	// 处理世界资源详细信息回执 ... 
	public void ProcessWorldResCityInfo(byte[] data)
	{
		STC_GAMECMD_GET_WORLD_RES_CARD_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_GET_WORLD_RES_CARD_T>(data);
		if(msgCmd.nRst1 == (int)STC_GAMECMD_GET_WORLD_RES_CARD_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_WORLD_RES_CARD_T>();
			int data_len = data.Length - head_len;
			byte[] data_buff = new byte[data_len];
			System.Array.Copy(data,head_len,data_buff,0,data_len);
			WorldRes ResInfo = DataConvert.ByteToStruct<WorldRes>(data_buff);
			GameObject obj  = U3dCmn.GetObjFromPrefab("WorldResCityWin");
			if (obj != null)
			{
				obj.GetComponent<WorldResCityWin>().city_info = ResInfo;
				obj.SendMessage("RevealPanel");
			}
			
		}
		/*else if(msgCmd.nRst1 == (int)STC_GAMECMD_GET_WORLD_RES_CARD_T.enum_rst.RST_NOT_EXIST)
		{
			if(ResCityX >0 && ResCityY>0)
			{
				DeleteWorldResCity(ResCityX,ResCityY);
				GPS.ResetPacket1();
			}
		}
		ResCityX = -1;
		ResCityY= -1;*/
	}
	// 处理世界资源被攻占信息  
	public void ProcessWorldResAttacked(byte[] data)
	{
		STC_GAMECMD_WORLD_RES_CLEAR_T msgCmd = DataConvert.ByteToStruct<STC_GAMECMD_WORLD_RES_CLEAR_T>(data);
		DeleteWorldResCity((int)msgCmd.nPosX1,(int)msgCmd.nPosY2);
		if(GPS!=null)
			GPS.ResetPacket1();
	}
}
