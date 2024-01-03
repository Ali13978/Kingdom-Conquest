using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AllianceInfoWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UILabel			alliance_name;
	public UILabel			leader_label;
	public UILabel			level_label;
	public UILabel			rank_label;
	public UILabel			member_num_label;
	public UILabel			development_label;
	public UILabel			announce_label;
	AllianceCard now_unit;
	
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
	
	}
	//打开界面请求联盟数据 
	void RevealPanel(ulong AllianceID)
	{
		CTS_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_ALLIANCE_CARD_BY_ID;
		req.nAllianceID3 = AllianceID;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID);
		tween_position.Play(true);
	}
	//传入联盟数据直接打开界面 
	void RevealPanelByParam(AllianceCard unit)
	{
		InitialData(unit);
		tween_position.Play(true);
	}
	//请求联盟数据返回 
	void AllianceDataRst(byte[] buff)
	{
		STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_ALLIANCE_CARD_BY_ID_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			AllianceCard unit =  DataConvert.ByteToStruct<AllianceCard>(data_buff);
			InitialData(unit);
		}
	}
	//根绝以后初始化数据 
	void InitialData(AllianceCard unit)
	{
		now_unit = unit;
		alliance_name.text = DataConvert.BytesToStr(unit.szAllianceName3);
		leader_label.text = DataConvert.BytesToStr(unit.szLeaderName6);
		level_label.text = unit.nLevel10.ToString();
		rank_label.text = unit.nRank9.ToString();
		member_num_label.text = unit.nMemberNum11+"/"+unit.nMaxMemberNum12;
		development_label.text = unit.nDevelopment7.ToString();
		announce_label.text = DataConvert.BytesToStr(unit.szIntroduction14);
		//tween_position.Play(true);
		
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//申请加入联盟 
	void ApplyAlliance()
	{
		AllianceManager.ReqJoinAllianceByID(now_unit.nAllianceID1);
		AllianceManager.apply_alliance_name =DataConvert.BytesToStr(now_unit.szAllianceName3);
	}
}
