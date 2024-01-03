using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class ArenaManager : MonoBehaviour {
	public static int challenge_num = 5;
	public static int item_num_max = 10;
	public static uint my_rank;
	public static  int try_num;
	public static uint total_rank_num;
	//是否部署过兵力 
	public static bool is_deploy = false;
	static ArenaManager _instance;
    private ArenaManager() { }
    static ArenaManager()
    {
        _instance = new ArenaManager();
    }
    public static ArenaManager Instance
    {
        get { return _instance; }
    }
	public delegate void ReqArenaDataFinishDelegate(STC_GAMECMD_ARENA_GET_STATUS_T status,ChallengeListUnit[] challenge_array);
	public static ReqArenaDataFinishDelegate ReqArenaDataFinish;
	public delegate void ReqArenaRankDataFinishDelegate(STC_GAMECMD_ARENA_GET_RANK_LIST_T rank_data,RankListUnit[] rank_array);
	public static ReqArenaRankDataFinishDelegate ReqArenaRankDataFinish;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//请求单签排名信息和可挑战次数等基本信息 
	public void ReqArenaData()
	{
		CTS_GAMECMD_ARENA_GET_STATUS_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARENA_GET_STATUS;
		req.nNum3 = (uint)challenge_num;
		TcpMsger.SendLogicData<CTS_GAMECMD_ARENA_GET_STATUS_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ARENA_GET_STATUS);
	}
	//请求单签排名信息和可挑战次数等基本信息返回  
	public void ReqArenaDataRst(byte[] buff)
	{
		STC_GAMECMD_ARENA_GET_STATUS_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ARENA_GET_STATUS_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_GET_STATUS_T.enum_rst.RST_OK)
		{
			//AllianceApplyList.Clear();
			my_rank = sub_msg.nRank5;
			try_num = (int)sub_msg.nChallengeTimesLeft2;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ARENA_GET_STATUS_T>();
			ChallengeListUnit[] challenge_array = DataConvert.ByteToStructArrayByOffset<ChallengeListUnit>(buff,sub_msg.nNum6,head_len);
			if(ReqArenaDataFinish != null)
			{
				ReqArenaDataFinish(sub_msg,challenge_array);
				ReqArenaDataFinish = null;
			}
		
		}
	}
	//请求竞技场排名数据 
	public void ReqArenaRankData(uint page_size,uint page_num)
	{
		CTS_GAMECMD_ARENA_GET_RANK_LIST_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARENA_GET_RANK_LIST;
		req.nPageSize3 = page_size;
		req.nPage4 = page_num;
		TcpMsger.SendLogicData<CTS_GAMECMD_ARENA_GET_RANK_LIST_T>(req);
		print (req.nPageSize3+" "+req.nPage4);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ARENA_GET_RANK_LIST);
	}
	//请求竞技场排名数据返回 
	public void ReqArenaRankDataRst(byte[] buff)
	{
		STC_GAMECMD_ARENA_GET_RANK_LIST_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ARENA_GET_RANK_LIST_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_GET_RANK_LIST_T.enum_rst.RST_OK)
		{
			total_rank_num = sub_msg.nTotalNum2;
			//AllianceApplyList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ARENA_GET_RANK_LIST_T>();
			RankListUnit[] rank_array = DataConvert.ByteToStructArrayByOffset<RankListUnit>(buff,sub_msg.nNum4,head_len);
			if(ReqArenaRankDataFinish != null)
			{
				ReqArenaRankDataFinish(sub_msg,rank_array);
			}
			
		}
		ReqArenaRankDataFinish = null;
	}
	//请求竞技场自己的镜像数据 
	public void ReqDeployData()
	{
		CTS_GAMECMD_ARENA_GET_DEPLOY_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARENA_GET_DEPLOY;
		TcpMsger.SendLogicData<CTS_GAMECMD_ARENA_GET_DEPLOY_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ARENA_GET_DEPLOY);
	}
	//请求竞技场自己的镜像数据返回 
	public void ReqDeployDataRst(byte[] buff)
	{
		 
		STC_GAMECMD_ARENA_GET_DEPLOY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ARENA_GET_DEPLOY_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_GET_DEPLOY_T.enum_rst.RST_OK || sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_GET_DEPLOY_T.enum_rst.RST_NOT_UPLOAD)
		{
			//AllianceApplyList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ARENA_GET_DEPLOY_T>();
			ArenaHeroDeploy[] hero_array = DataConvert.ByteToStructArrayByOffset<ArenaHeroDeploy>(buff,(int)sub_msg.nNum2,head_len);
			ArmyDeployManager.instance.InitialArenaDeployData(hero_array);
			GameObject obj  = U3dCmn.GetObjFromPrefab("ArmyDeployWin");
	        if (obj != null)
	        {
	            obj.SendMessage("RevealPanel",COMBAT_TYPE.COMBAT_ARENA);
	        }
		}
	}
	//上传镜像数据 
	public void UploadDeployData()
	{
		if(is_deploy)
		{
			CTS_GAMECMD_ARENA_UPLOAD_DATA_T req;
			req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
			req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ARENA_UPLOAD_DATA;
			List<HeroDeploy> deploys = new List<HeroDeploy>();
			
	    
	        List<TroopData> heros = ArmyDeployManager.instance.TroopDataList;
			int total_force = 0;
	        for (int i = 0; i < heros.Count; i++) {
				if(heros[i].Force != 0 && heros[i].HeroID != 0)
				{
					HeroDeploy dep = new HeroDeploy { 
	                nCol1 = heros[i].Column,
	                nRow2 = heros[i].Row,
	                nHeroID3 = heros[i].HeroID
		            };
					total_force  = heros[i].Force;
		            deploys.Add(dep);
				}
	           
	        }
			if(total_force<=0)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MUST_DEPLOY_FIRST));
				return ;
			}
	        byte[] data = DataConvert.StructArrayToByte<HeroDeploy>(deploys.ToArray());
	        req.nSize3 = data.Length;
	        req.Data4 = data;
			TcpMsger.SendLogicData<CTS_GAMECMD_ARENA_UPLOAD_DATA_T>(req);
			LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ARENA_UPLOAD_DATA);
		}
		else 
		{
			U3dCmn.SendMessage("ArmyDeployWin","DismissPanel",null);
		}
	}
	//上传镜像数据返回  
	public void UploadDeployDataRst(byte[] buff)
	{
		STC_GAMECMD_ARENA_UPLOAD_DATA_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ARENA_UPLOAD_DATA_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_UPLOAD_DATA_T.enum_rst.RST_OK)
		{
			is_deploy = false;
			U3dCmn.SendMessage("ArmyDeployWin","DismissPanel",null);
			U3dCmn.SendMessage("ArenaWin","ReqArenaData",null);
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_UPLOAD_DATA_T.enum_rst.RST_UPLOAD_CD)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ARENA_UPLOAD_CD));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_ARENA_UPLOAD_DATA_T.enum_rst.RST_NO_TROOP)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MUST_DEPLOY_FIRST));
		}
	}
	//发起挑战 
	public void ReqChallenge(ulong account_id)
	{
		BattleManager.CombatInfo info = new BattleManager.CombatInfo();
		info.CombatType = (uint)COMBAT_TYPE.COMBAT_ARENA;
		info.ObjectID = account_id;
		BattleManager.Instance.StartCombat(info);
	}
	
}
