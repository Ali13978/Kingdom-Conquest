using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ModifyNameWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UIInput new_name;
	public ulong hero_id;
	ChangeNameInfo now_change_info;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//弹出窗口   
	void RevealPanel(int type)
	{
		if(CommonMB.ChangeNameInfo_Map.Contains(type))
		{
			now_change_info = (ChangeNameInfo)CommonMB.ChangeNameInfo_Map[type];
		}
		else 
		{
			return ;
		}
		//InitialPanelData();
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//修改名字 
	void ModifyName()
	{
		if(new_name.text.Length > 10)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NAME_TOO_LONG));
			return;
		}
		if(U3dCmn.IsContainBadName(new_name.text))
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BAD_NAME));
			return;
		}
		string str =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MODIFY_NAME_SPEND),now_change_info.Price);
		if(now_change_info.Type == (int)MODIFY_NAME_TYPE.LORD_NAME)
		{
			PopConfirmWin.instance.ShowPopWin(str,ModifyLordName);
		}
		else if(now_change_info.Type == (int)MODIFY_NAME_TYPE.HERO_NAME)
		{
			PopConfirmWin.instance.ShowPopWin(str,ModifyHeroName);
		}
		
	}
	//修改君主名字 
	void ModifyLordName(GameObject obj)
	{
		if(now_change_info.Price >CommonData.player_online_info.Diamond)
		{
			U3dCmn.ShowRechargePromptWindow();
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
			return ;
		}
		CTS_GAMECMD_OPERATE_CHANGE_NAME_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CHANGE_NAME;
		
		req.szNewName4 = DataConvert.StrToBytes(new_name.text);
	
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CHANGE_NAME_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CHANGE_NAME);
	}
	//修改君主名字返回 
	void ModifyLordNameRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CHANGE_NAME_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CHANGE_NAME_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CHANGE_NAME_T.enum_rst.RST_OK)
		{
			CommonData.player_online_info.Diamond -= sub_msg.nDiamond2;
			CommonData.player_online_info.CharName =  new_name.text;
			PlayerInfoManager.RefreshPlayerDataUI();
			U3dCmn.SendMessage("PlayerInfoWin","UpdatePlayerInfo",null);
			DismissPanel();
		}
	}
	//修改将领名字 
	void ModifyHeroName(GameObject obj)
	{
		if(now_change_info.Price >CommonData.player_online_info.Diamond)
		{
			U3dCmn.ShowRechargePromptWindow();
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
			return ;
		}
		if(hero_id >0)
		{
			CTS_GAMECMD_OPERATE_CHANGE_HERO_NAME_T req;
			req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CHANGE_HERO_NAME;
			req.nHeroID3 = hero_id;
			req.szNewName5 = DataConvert.StrToBytes(new_name.text);
		
			TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CHANGE_HERO_NAME_T >(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CHANGE_HERO_NAME);
		}
		
		
	}
	//修改将领名字返回 
	void ModifyHeroNameRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CHANGE_HERO_NAME_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CHANGE_HERO_NAME_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CHANGE_HERO_NAME_T.enum_rst.RST_OK)
		{
		//	print ("ccccccccccccccccccccccccccc");
			CommonData.player_online_info.Diamond -= sub_msg.nDiamond2;
			PlayerInfoManager.RefreshPlayerDataUI();
			if(JiangLingManager.MyHeroMap.Contains(hero_id))
			{
				HireHero heroinfo = (HireHero)JiangLingManager.MyHeroMap[hero_id];
				heroinfo.szName3 = DataConvert.StrToBytes(new_name.text);
				JiangLingManager.MyHeroMap[hero_id] = heroinfo;
				
				//刷新将领界面 
				U3dCmn.SendMessage("JiangLingInfoWin","SetJiangLingInfoPanelData",heroinfo);
			}
			
			DismissPanel();
		}
		hero_id = 0;
	}
}
