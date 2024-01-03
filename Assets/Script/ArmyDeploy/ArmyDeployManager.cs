using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;


public class TroopData
{
	public int 		PosID;
	public ulong 	AccountID;
	public string	AccountName;
	public ulong 	HeroID;
	public int 		TroopId;
	public int 		TroopLevel;
	public int 		TroopNum;
	public int 		Force;
    public override string ToString()
    {
        return string.Format("TroopData: HeroID {0}\t\t\tRow {1}\t\t\tColumn {2}\t\t\tPosID {3}", this.HeroID, this.Row, this.Column, this.PosID);
    }
    public uint Row {
        get { return (uint)(this.PosID % 5 + 1); }
    }

    public uint Column {
        get { return (uint)(this.PosID / 5 + 1); }
    }
}

public class ArmyDeployManager : MonoBehaviour {
	public const int maxRows = 5;
	public const int maxColumns = 5; 
	static ArmyDeployManager mInst = null;
	//是否只是个人部队(用来判断显示君主名字还是兵种等级)  
	public static bool isIndividual = false;
	//当前的使用MANAGER的战斗模块 
	public static COMBAT_TYPE deploy_type;
	//是否正在拖拽ITEM 
	public static  bool is_drag = false;
    /// <summary>
    /// 存放队伍中所有将领的信息
    /// </summary>
    private List<GBLordTroopHero> _tmpHeros;
	
	public List<TroopData>TroopDataList = new List<TroopData>();
	public List<TroopData>EnemyTroopDataList = new List<TroopData>();
	
	public delegate void AutoArrangeFinishedDelegate();
	public static AutoArrangeFinishedDelegate AutoArrangeFinished;

    void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	
	public static ArmyDeployManager instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(ArmyDeployManager)) as ArmyDeployManager;

				if (mInst == null)
				{
					GameObject go = new GameObject("ArmyDeployManager");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<ArmyDeployManager>();
				}
			}
			
			return mInst;
		}
	}
	
	void OnDestroy() { if (mInst == this) mInst = null; deploy_type = COMBAT_TYPE.NONE;}
	
    /// <summary>
    /// 初始化配兵数据 用于联盟副本中数据初始化 
    /// </summary>
	public void InitialArmyDeployData()
	{
		isIndividual = false;
        //先分配25个空格 
        int totalCount = 25;
        List<TroopData> troopList = new List<TroopData>(totalCount);
        List<GBTeamMemberItem> memberList = GBTeamManager.Instance.TeamMemberList;

        for (int i = 0; i < totalCount; ++i)
        {
            TroopData data = new TroopData
            {
                AccountID = 0,
				AccountName = "",
                Force = 0,
                HeroID = 0,
                PosID = i,
                TroopId = 0,
                TroopNum = 0
            };
            troopList.Add(data);
        }
        _tmpHeros = new List<GBLordTroopHero>();
        for (int i = 0; i < memberList.Count; ++i)
        {
            List<GBLordTroopHero> heroList = memberList[i].Heros;
            for (int j = 0; j < heroList.Count; ++j)
            {
                GBLordTroopHero hero = heroList[j];
                if (hero != null)
                {
                    //print(string.Format("id:{0}\t\t\tchanged:{1}\t\t\tposID:{2}", hero.HeroID, hero.PosChanged, hero.PosID));
                    TroopData troop = new TroopData
                    {
                        AccountID = hero.LordAccountID,
						AccountName = memberList[i].LordName,
                        Force = (int)hero.HeroForce,
                        HeroID = hero.HeroID,
                        TroopId = (int)hero.TroopID,
						TroopLevel = (int)hero.TroopLevel,
                        TroopNum = (int)hero.TroopAmount,
                        PosID = !hero.PosChanged ? i + j * 5 : (int)hero.PosID
                    };
                    troopList[troop.PosID] = troop;
                    _tmpHeros.Add(hero);
                }
            }
        }
        this.TroopDataList = troopList;
	}
	//竞技场初始化配兵数据 
	public void InitialArenaDeployData(ArenaHeroDeploy[] deploy_data)
	{
		isIndividual = true;
		 //先分配25个空格 
        int totalCount = maxRows*maxColumns;
        List<TroopData> troopList = new List<TroopData>(totalCount);

        for (int i = 0; i < totalCount; i++)
        {
            TroopData data = new TroopData
            {
                AccountID = 0,
				AccountName = "",
                Force = 0,
                HeroID = 0,
                PosID = i,
                TroopId = 0,
                TroopNum = 0
            };
            troopList.Add(data);
        }
	
		for (int i = 0; i < deploy_data.Length; i++)
        {
	        ArenaHeroDeploy hero_info = deploy_data[i];
				
			int pos_id = ((int)hero_info.nCol1-1)*maxRows+(int)hero_info.nRow2-1;
			if(pos_id <0 || pos_id >=totalCount)
				return ;
			TroopData troop_data = new TroopData();
	        troop_data.AccountID = CommonData.player_online_info.AccountID;
			troop_data.AccountName = CommonData.player_online_info.CharName;
	        troop_data.Force = hero_info.nProf7;
	        troop_data.HeroID = hero_info.nHeroID3;
	        troop_data.TroopId = (int)hero_info.nArmyType4;
			troop_data.TroopLevel = hero_info.nArmyLevel5;
	        troop_data.TroopNum = (int)hero_info.nArmyNum6;
			troop_data.PosID = pos_id;
       		troopList[pos_id] = troop_data;
        }
		this.TroopDataList = troopList;
	}
	//根据GBLordTroopHero初始化配兵数据(调用配兵场景返回)  
	public void InitialGBLordTroopHero(List<GBLordTroopHero> deploy_data)
	{
		 //先分配25个空格 
        int totalCount = maxRows*maxColumns;
        List<TroopData> troopList = new List<TroopData>(totalCount);

        for (int i = 0; i < totalCount; i++)
        {
            TroopData data = new TroopData
            {
                AccountID = 0,
				AccountName = "",
                Force = 0,
                HeroID = 0,
                PosID = i,
                TroopId = 0,
                TroopNum = 0
            };
            troopList.Add(data);
        }

		for (int i = 0; i < deploy_data.Count; i++)
		{
			GBLordTroopHero hero_info = deploy_data[i];
			int pos_id = i;
			TroopData troop_data = new TroopData();
	        troop_data.AccountID = CommonData.player_online_info.AccountID;
			troop_data.AccountName = CommonData.player_online_info.CharName;
	        troop_data.Force = (int)hero_info.HeroForce;
	        troop_data.HeroID = hero_info.HeroID;
	        troop_data.TroopId = (int)hero_info.TroopID;
			troop_data.TroopLevel = (int)hero_info.TroopLevel;
	        troop_data.TroopNum = (int)hero_info.TroopAmount;
			troop_data.PosID = i;
			troopList[i] = troop_data;
		}
		this.TroopDataList = troopList;
	}
	//将领数据刷新HeroSimpleData（一般是补给） 
	public void UpdateArmyData(HeroSimpleData[] hero_array,COMBAT_TYPE combat_type)
	{
		if(deploy_type == combat_type)
		{
			for(int i=0;i<hero_array.Length;i++)
			{
				HeroSimpleData hero_data = hero_array[i];
				int index = TroopDataList.FindIndex(delegate(TroopData  data){return   data.HeroID==hero_data.nHeroID1;});
				
				if(index < TroopDataList.Count && index >=0)
				{
					TroopData troop_data = TroopDataList[index];
					troop_data.Force = hero_data.nProf5;
					troop_data.TroopId = hero_data.nArmyType2;
					troop_data.TroopNum = hero_data.nArmyNum4;
					troop_data.TroopLevel = hero_data.nArmyLevel3;
					TroopDataList[index] = troop_data;
				}
				else 
				{
					print ("errrrrrrrrrrrrrrrrr"+index+" "+hero_data.nHeroID1);
				}
				
			}
			U3dCmn.SendMessage("ArmyDeployWin","InitialDeployScene",null);
		}
	}
	
	//根据位置得到兵的数据 
	public TroopData GetTroopUnitDataByID(int index)
	{
		TroopData troop_data = new TroopData();
		if(index <0 || index+1 > TroopDataList.Count)
		{
			troop_data.PosID = index;
			
		}
		else 
		{
			troop_data = TroopDataList[index];
		}
		return troop_data;
	}
	//交换单元排兵布阵顺序 
	public void ExchangeUnitTroopData(int from_id,int to_id)
	{
		if(from_id>=0&&from_id<TroopDataList.Count&&to_id>=0&&to_id<TroopDataList.Count)
		{
			TroopData from_data = TroopDataList[from_id];
			TroopData to_data = TroopDataList[to_id];
			from_data.PosID = to_id;
			to_data.PosID = from_id;
			TroopDataList[from_id] = to_data;
			TroopDataList[to_id] = from_data;
		}
		SaveTroopPos();
	}
	//根据列数得到一列兵的数据  
	public TroopData[] GetTroopColDataByID(int index,int row_num)
	{
		TroopData[] troop_data = new TroopData[row_num];
		if(index <0 || index+1 > TroopDataList.Count)
		{
			for(int i=0;i<row_num;i++)
			{
				troop_data[i].PosID =  index*row_num+i;
			}
		}
		else 
		{
			for(int i=0;i<row_num;i++)
			{
				troop_data[i] = TroopDataList[index*row_num+i];
			}
			
		}
		return troop_data;
	}
	//交换列派兵布阵顺序 
	public void ExchangeColumnTroopData(int from_col,int to_col,int row_num)
	{
		for(int i=0;i<row_num;i++)
		{
			TroopData tmp_data = TroopDataList[from_col*row_num+i];
			TroopDataList[from_col*row_num+i] = TroopDataList[to_col*row_num+i];
			TroopDataList[to_col*row_num+i] = tmp_data;
			TroopDataList[from_col*row_num+i].PosID =  from_col*row_num+i;
			TroopDataList[to_col*row_num+i].PosID = to_col*row_num+i;
		}
		SaveTroopPos();
	}
	//自动排兵布阵（按照兵种排序） 
	public void AutoArrange()
	{
		TroopDataList.Sort(CompareTroopData);
		for(int i =0,max = TroopDataList.Count;i<max;i++)
		{
			TroopDataList[i].PosID = i;
		}
		if(AutoArrangeFinished != null)
		{
			AutoArrangeFinished();
		}
		SaveTroopPos();
	}
	//排序的COMPARE 
	public int  CompareTroopData(TroopData data1,TroopData data2)
	{ 
		int rst = 0;
		if(data1.TroopId == 0)
		{
			rst = 1;
		}
		else if(data2.TroopId == 0)
		{
			rst = -1;
		}
		else if(data1.TroopId > data2.TroopId)
		{
			rst = 1;
		}
		else if(data1.TroopId < data2.TroopId)
		{
			rst = -1;
		}
		return rst;
	}
	//开始战斗 
	public void ReleaseTroop(ulong[] troopIDs)
	{
        for (int i = 0; i < this.TroopDataList.Count; ++i) {
            TroopData troop = this.TroopDataList[i];
            for (int j = 0; j < troopIDs.Length; ++j) {
                if (troopIDs[j] == troop.HeroID) {
                    troop.HeroID = 0;
                    troop.PosID = 0;
                    troop.Force = 0;
                    troop.TroopId = 0;
                    troop.TroopNum = 0;
                    troop.AccountID = 0;
                    troop.Force = 0;
                }
            }
        }
	}

    public void SaveTroopPos() {
		if(_tmpHeros != null)
		{
			for (int i = 0; i < this.TroopDataList.Count; ++i) {
            int posID = i;
            TroopData troop = this.TroopDataList[i];
            for (int j = 0; j < this._tmpHeros.Count; ++j) {
                GBLordTroopHero hero = this._tmpHeros[j];
                if (troop.HeroID == hero.HeroID) {
                    hero.PosID = (uint)troop.PosID;
                    hero.PosChanged = true;
					break ;
                }
            }
        	}
		}
       
    }
	//请求手动补给  
	public void ReqManualSupply(COMBAT_TYPE type)
	{
		CTS_GAMECMD_HERO_SUPPLY_T req;
        req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_HERO_SUPPLY;
        req.nCombatType3 = (uint)type;
        req.nNum4 = TroopDataList.Count;
		ulong[] heroid_array = new ulong[req.nNum4];
		int hero_num = 0;
		for(int i=0,max=TroopDataList.Count;i<max;i++)
		{
			if(TroopDataList[i].HeroID != 0)
			{
				heroid_array[i] = TroopDataList[i].HeroID;
				hero_num++;
			}
			
		}
		req.nNum4 = hero_num;
		req.HeroIDs5 = heroid_array;
        TcpMsger.SendLogicData<CTS_GAMECMD_HERO_SUPPLY_T>(req);
		LoadingManager.instance.ShowLoading();
	}
	//请求手动补给返回(只处理失败情况 成功则等待英雄数据的推送)  
	public void ReqManualSupplyRst(byte[] buff)
	{
		STC_GAMECMD_HERO_SUPPLY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_HERO_SUPPLY_T>(buff);
		if(sub_msg.nRst1 != (int)STC_GAMECMD_HERO_SUPPLY_T.enum_rst.RST_OK)
		{
			LoadingManager.instance.HideLoading();
		}
	}
	//请求地方的军队布阵信息 
	public void ReqEnemyDepoyData(COMBAT_TYPE type)
	{
		CTS_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
        req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY;
        req.nExcelID3 = (uint)type;
		req.nLevel4 = GBStatusManager.Instance.MyStatus.nCurLevel;
		TcpMsger.SendLogicData<CTS_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY);
		print ("55555555555555555555555555"+type+" "+ GBStatusManager.Instance.MyStatus.nCurLevel);
	}
	//请求地方的军队布阵信息返回  
	public void ReqEnemyDepoyDataRst(byte[] buff)
	{
		EnemyTroopDataList.Clear();
		STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T>(buff);
		print ("rrrrrrrr"+sub_msg.nRst1);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T.enum_rst.RST_OK)
		{
			
			InitialTroopDataList(EnemyTroopDataList);
			//AllianceApplyList.Clear();
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_ALLI_INSTANCE_GET_DEFENDER_DEPLOY_T>();
			ArmyUnit[] enemy_array = DataConvert.ByteToStructArrayByOffset<ArmyUnit>(buff,(int)sub_msg.nNum4,head_len);
			print ("88"+enemy_array.Length);
			for(int i=0;i<enemy_array.Length;i++)
			{
				ArmyUnit enemy_data = enemy_array[i];
			
				TroopData troop = new TroopData
                {
                    AccountID = 0,
					AccountName = "",
                    Force = (int)enemy_data.nForce2,
                    HeroID = 0,
                    TroopId = (int)enemy_data.nArmyType1,
					TroopLevel = 0,
                    TroopNum = 0,
                    PosID = ((int)enemy_data.nCol4-1)*maxRows+(int)enemy_data.nRow3-1
                };
                EnemyTroopDataList[troop.PosID] = troop;
			}
		}
		U3dCmn.SendMessage("ArmyDeployWin","ShowPanel",null);
	}
	//根据位置得到敌方派兵的数据  
	public TroopData GetEnemyTroopUnitDataByID(int index)
	{
		TroopData troop_data = new TroopData();
		if(index <0 || index+1 > EnemyTroopDataList.Count)
		{
			troop_data.PosID = index;
			
		}
		else 
		{
			troop_data = EnemyTroopDataList[index];
		}
		return troop_data;
	}
	//初始化TroopData的LIST 
	void InitialTroopDataList(List<TroopData> DataList) 
	{
		int totalCount = maxRows * maxColumns;
		for (int i = 0; i < totalCount; ++i)
        {
            TroopData data = new TroopData
            {
                AccountID = 0,
				AccountName = "",
                Force = 0,
                HeroID = 0,
                PosID = i,
                TroopId = 0,
                TroopNum = 0
            };
            DataList.Add(data);
        }
	}
}
