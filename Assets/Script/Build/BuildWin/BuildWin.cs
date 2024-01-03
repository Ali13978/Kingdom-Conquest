using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class BuildWin : MonoBehaviour {
	uint 	AutoID;
	byte	Sort;
	public TweenPosition 	tween_position;
	public UILabel			need_building;
	public UILabel			now_building;
	public UILabel 			need_gold;
	public UILabel 			now_gold;
	public UILabel 			need_people;
	public UILabel 			now_people;
	public UILabel			need_time;
	public GameObject		build_btn;
	string need_building_str;
	string need_time_str;
	//
	public struct BuildUnit
	{
		public byte sort;
		public uint autoid;
	}
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
		//记录一下初始格式化字符串  
		need_building_str = need_building.text;
		need_time_str = need_time.text;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	//弹出窗口  
	void RevealPanel(BuildUnit unit)
	{

		if(!(CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING) && CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU) 
			&& CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.JINKUANG)))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			Sort = (byte)unit.sort;
			AutoID = unit.autoid;
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)unit.sort,1);
			
			int sec = (int)(info.BuildTime);
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			need_time.text= string.Format(need_time_str,hour,minute,second);
			
			bool btn_enable = true;
			uint need = 1;
			uint now = ChengBao.NowLevel;
			need_building.text = string.Format(need_building_str,need);
			now_building.text = string.Format(need_building_str,now);
			if(now<need)
			{
				btn_enable = false;
				now_building.color = new Color(1,0,0,1);
			}
			else
			{
				now_building.color = new Color(0,0.38f,0,1);
			}
			
			need = (uint)(info.Money);
			now = CommonData.player_online_info.Gold;
			need_gold.text = need.ToString();
			now_gold.text = now.ToString();
			if(now<need)
			{
				btn_enable = false;
				now_gold.color = new Color(1,0,0,1);
			}
			else
			{
				now_gold.color = new Color(0,0.38f,0,1);
			}
			
			
			need = (uint)(info.Worker);
			now = CommonData.player_online_info.Population;
			need_people.text = need.ToString();
			now_people.text = now.ToString();
			if(now<need)
			{
				btn_enable = false;
				now_people.color = new Color(1,0,0,1);
			}
			else
			{
				now_people.color = new Color(0,0.38f,0,1);
			}
			
			if(!btn_enable)
			{
				build_btn.GetComponent<Collider>().enabled = false;
				build_btn.GetComponent<UIImageButton>().target.spriteName = "button1_disable";
			}
			else
			{
				build_btn.GetComponent<Collider>().enabled = true;
				build_btn.GetComponent<UIImageButton>().target.spriteName = "button1";
			}
			tween_position.Play(true);
		}
	
	}
	//隐藏窗口  
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//请求建造  
	void ReqBuild()
	{
		
		if(Sort == (byte)BuildingSort.BINGYING)
		{
			ReqBuildBingYing();
		}
		else if(Sort == (byte)BuildingSort.MINJU)
		{
			ReqBuildMinJu();
		}
		else if(Sort == (byte)BuildingSort.JINKUANG)
		{
			ReqBuildJinKuang();
		}
		
	}
	//请求建造兵营   
	void ReqBuildBingYing()
	{
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.BINGYING))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)(BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.BINGYING,1);
			CTS_GAMECMD_OPERATE_BUILD_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_BUILD_BUILDING;
			req.nExcelID3 = (uint)info.ID;
			req.nAutoID4  = info.BeginID+(uint)AutoID;
			//print ("bingying"+req.nExcelID3 +" "+req.nAutoID4 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUILD_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING);
		}
		DismissPanel();
	} 
	//请求建造民居  
	void ReqBuildMinJu()
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
			req.nAutoID4  = info.BeginID+(uint)AutoID;
			//print ("bingying"+req.nExcelID3 +" "+req.nAutoID4 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUILD_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING);
		}
		DismissPanel();
	} 
	//请求建造金矿  
	void ReqBuildJinKuang()
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
			req.nAutoID4  = info.BeginID+(uint)AutoID;
			//print ("bingying"+req.nExcelID3 +" "+req.nAutoID4 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_BUILD_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_BUILD_BUILDING);
		}
		DismissPanel();
	} 
}
