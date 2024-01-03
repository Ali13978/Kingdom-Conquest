using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class MinJuInfoWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public TweenScale	 	tween_scale;
	public UILabel			need_building;
	public UILabel			now_building;
	public UILabel 			need_gold;
	public UILabel 			now_gold;
	public UILabel 			need_people;
	public UILabel 			now_people;
	public UILabel			need_time;
	public GameObject       upgrade_btn;
	public GameObject       accelerate_btn;
	string need_building_str;
	string need_time_str;
	MinJuUnit now_unit;
	public struct MinJuUnit
	{
		public uint level;
		public uint  autoid;
		public uint build_state;
		public int 	build_end_time;
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
	void RevealPanel(MinJuUnit unit)
	{
		now_unit = unit;
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			if(unit.build_state == (uint)BuildingState.NORMAL)
			{
				accelerate_btn.SetActiveRecursively(false);
				upgrade_btn.SetActiveRecursively(true);
			}
			else if(unit.build_state == (uint)BuildingState.UPGRADING)
			{
				accelerate_btn.SetActiveRecursively(true);
				upgrade_btn.SetActiveRecursively(false);
			}
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,(int)(now_unit.level+1));
			//AutoID = info.BeginID+unit.autoid;
			
			
			int sec = (int)info.BuildTime;
			int hour = sec/3600;
			int minute = sec/60%60;
			int second = sec%60;
			need_time.text= string.Format(need_time_str,hour,minute,second);
			
			bool btn_enable = true;
			uint need = now_unit.level+1;
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
			
			
			need = (uint)info.Money;
			now = CommonData.player_online_info.Gold;
			need_gold.text = need.ToString();
			now_gold.text = U3dCmn.GetNumStr(now);
			if(now<need)
			{
				btn_enable = false;
				now_gold.color = new Color(1,0,0,1);
			}
			else 
			{
				now_gold.color = new Color(0,0.38f,0,1);
			}
			
			need = (uint)info.Worker; 
			now = CommonData.player_online_info.Population;
			need_people.text = need.ToString();
			now_people.text = U3dCmn.GetNumStr(now);
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
				upgrade_btn.GetComponent<Collider>().enabled = false;
				upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1_disable";
			}
			else 
			{
				upgrade_btn.GetComponent<Collider>().enabled = true;
				upgrade_btn.GetComponent<UIImageButton>().target.spriteName = "button1";
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
	//升级民居   
	void ReqUpgradeMinJu()
	{
		
		if(!CommonMB.BuildingInfo_Map.Contains((int)BuildingSort.MINJU))
		{
			CommonMB.InitialBuilding();
		}
		else
		{
			BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,(int)(now_unit.level));
			uint AutoID = info.BeginID+now_unit.autoid;
			CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_BUILDING;
			req.nAutoID3 = AutoID;
			//print ("upgrade bingying"+req.nAutoID3 );
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_BUILDING_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_BUILDING);
		}
		// DismissPanel();
	}
	//加速  
	void ReqAccelerateMinJu()
	{
		BuildInfo info = (BuildInfo)U3dCmn.GetBuildingInfoFromMb((int)BuildingSort.MINJU,(int)(now_unit.level));
		uint AutoID = info.BeginID+now_unit.autoid;
		AccelerateWin.AccelerateUnit unit;
		unit.Type = (int)enum_accelerate_type.building;
		unit.BuildingType = (uint)te_type_building.te_subtype_building_upgrade; 
		unit.autoid = AutoID;
		unit.EndTimeSec = now_unit.build_end_time;
		GameObject win = U3dCmn.GetObjFromPrefab("AccelerateWin"); 
		if(win != null)
		{
			win.SendMessage("RevealPanel",unit);
			AccelerateWin.MotherWin = gameObject;
			AccelerateWin.CloseCallBack = "DismissPanel";
		}
	}
}
