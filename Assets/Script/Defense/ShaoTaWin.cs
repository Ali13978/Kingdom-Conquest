using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CTSCMD;
using CMNCMD;
public class ShaoTaWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public DefenseHero 		hero1;
	public DefenseHero 		hero2;
	public DefenseHero 		hero3;
	public DefenseHero 		hero4;
	public DefenseHero 		hero5;
	public DefenseHero[]  hero_array = new DefenseHero[5];
	public GameObject hero_page;
	public GameObject save_btn;
	public GameObject supply_btn;
	public UICheckbox auto_supply_btn;
	public UILabel	  auto_supply_info;
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
		hero_array[0] = hero1;
		hero_array[1] = hero2;
		hero_array[2] = hero3;
		hero_array[3] = hero4;
		hero_array[4] = hero5;
	}

	void OnDestroy()
	{
		BaizGeneralRoadWin.applyBaizCampDataDelegate = null;
	}
	// Use this for initialization
	void Start () {
		auto_supply_btn.gameObject.SetActiveRecursively(false);
		auto_supply_info.gameObject.SetActiveRecursively(false);
	}
	
	//弹出窗口   
	void RevealPanel()
	{
		ReqDefenseData();
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{		
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//请求城防数据 
	void ReqDefenseData()
	{
		CTS_GAMECMD_GET_CITYDEFENSE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_CITYDEFENSE;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_CITYDEFENSE_T >(req);	
		hero_page.SetActiveRecursively(false);
		save_btn.gameObject.SetActiveRecursively(false);
		supply_btn.SetActiveRecursively(false);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_CITYDEFENSE);
	}
	//请求城防数据返回 
	void ReqDefenseDataRst(byte[] buff)
	{
		
		STC_GAMECMD_GET_CITYDEFENSE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_CITYDEFENSE_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_GET_CITYDEFENSE_T.enum_rst.RST_OK)
		{
			hero_page.SetActiveRecursively(true);
			save_btn.SetActiveRecursively(true);
			supply_btn.gameObject.SetActiveRecursively(true);
			SetHeroInfo(0,sub_msg.n1Hero2);
			SetHeroInfo(1,sub_msg.n2Hero3);
			SetHeroInfo(2,sub_msg.n3Hero4);
			SetHeroInfo(3,sub_msg.n4Hero5);
			SetHeroInfo(4,sub_msg.n5Hero6);
		}
		//如果是VIP 开启自动补给按钮 
		if(CommonData.player_online_info.Vip >0)
		{
			auto_supply_info.gameObject.SetActiveRecursively(false);
			auto_supply_btn.gameObject.SetActiveRecursively(true);
			if(sub_msg.nAutoSupply7 == (int)CITY_DEFENSE_AUTOSUPPLY.OPEN)
			{
				auto_supply_btn.isChecked = true;
			}
			else
			{
				auto_supply_btn.isChecked = false;
			}
		}
		else
		{
			auto_supply_btn.gameObject.SetActiveRecursively(false);
			auto_supply_info.gameObject.SetActiveRecursively(true);
		}
		
	}
	//打开配置武将列表界面  
	void OpenHeroListWin()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("BaizGeneralRoadWin");
		if (go == null) return;
		BaizGeneralRoadWin t = go.GetComponent<BaizGeneralRoadWin>();
		if (t != null)
		{	
			t.AssignRoadGeneral();
			BaizGeneralRoadWin.applyBaizCampDataDelegate  = OnApplyDefenseHeroData;
		}
	}
	//配置武将回调 
	void OnApplyDefenseHeroData(List<PickingGeneral> data)
	{
		//print ("dddddddddddddddddddddddddd");
		int i= 0;
		for(;i<data.Count;i++)
		{
			PickingGeneral unit = data[i];
			if (unit != null)
			{
				SetHeroInfo(i,unit.nHeroID);
			}
			else
			{
				SetHeroInfo(i,0);
			}
		}
		for(;i<5;i++)
		{
			SetHeroInfo(i,0);
		}
	}
	//刷新城防将领UI   
	void SetHeroInfo(int index,ulong hero_id)
	{
	//	print ("hhhhhhhhhh"+hero_id);
		hero_array[index].SendMessage("InitialData",hero_id);
	}
	//保存城防守将信息 
	void SaveDefenseHeroInfo()
	{
		CTS_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CONFIG_CITYDEFENSE;
		req.n1Hero3 = hero1.heroID;
		req.n2Hero4 = hero2.heroID;
		req.n3Hero5 = hero3.heroID;
		req.n4Hero6 = hero4.heroID;
		req.n5Hero7 = hero5.heroID;
		int auto_supply = (int)CITY_DEFENSE_AUTOSUPPLY.CLOSE;
		if(CommonData.player_online_info.Vip >0)
		{
			if(auto_supply_btn.isChecked)
			{
				auto_supply = (int)CITY_DEFENSE_AUTOSUPPLY.OPEN;
			}
		}
		req.nAutoSupply8 = auto_supply;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T >(req);	
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE);
	}
	//保存城防守将信息返回 
	void SaveDefenseHeroInfoRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T>(buff);
		//print ("rrrrrrrrrrrrr"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_CONFIG_CITYDEFENSE_T.enum_rst.RST_OK)
		{
			DismissPanel();
		}
	}
	//补给城防   
	void SupplyDefence()
	{
		CTS_GAMECMD_MANUAL_SUPPLY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_MANUAL_SUPPLY;
		req.nInstanceID3 = 0;
		req.nCombatType4 = 0;
		req.n1Hero5 = 0;
		req.n2Hero6 = 0;
		req.n3Hero7 = 0;
		req.n4Hero8 = 0;
		req.n5Heor9 = 0;
		
		if (hero_array[0].heroID != 0) { req.n1Hero5 = hero_array[0].heroID; }
		if (hero_array[1].heroID != 0) { req.n2Hero6 = hero_array[1].heroID; }
		if (hero_array[2].heroID != null) { req.n3Hero7 = hero_array[2].heroID; }
		if (hero_array[3].heroID != null) { req.n4Hero8 = hero_array[3].heroID; }
		if (hero_array[4].heroID != null) { req.n5Heor9 = hero_array[4].heroID; }
		
		LoadingManager.instance.ShowLoading();
		CombatManager.processManualSupplyDelegate = OnProcessManualSupplyDelegate;
		TcpMsger.SendLogicData<CTS_GAMECMD_MANUAL_SUPPLY_T>(req);
	}
	//补给城防数据返回  
	void OnProcessManualSupplyDelegate()
	{
		SetHeroInfo(0,hero_array[0].heroID);
		SetHeroInfo(1,hero_array[1].heroID);
		SetHeroInfo(2,hero_array[2].heroID);
		SetHeroInfo(3,hero_array[3].heroID);
		SetHeroInfo(4,hero_array[4].heroID);
		LoadingManager.instance.HideLoading();
	}
}
