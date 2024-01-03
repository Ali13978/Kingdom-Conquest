using UnityEngine;
using System.Collections;
using STCCMD;

public class ArenaRankItem : MonoBehaviour {
	
	public UILabel  lord_rank;
	public UILabel  lord_name;
	public UILabel  lord_level;
	public UILabel  guild_name;
	public UILabel  force_name;
	
	public ulong account_id = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//初始化数据 
	public void InitialRankData(RankListUnit rank_data)
	{
		account_id = rank_data.nAccountID2;
		if(rank_data.nAccountID2 !=0 )
		{
			lord_rank.text = rank_data.nRank1.ToString();
			lord_name.text= DataConvert.BytesToStr(rank_data.szName4);
			lord_level.text = rank_data.nLevel5.ToString();
			guild_name.text = DataConvert.BytesToStr(rank_data.szAllianceName9);
			force_name.text = rank_data.nTotalForce7.ToString();
		}
		else 
		{
			lord_rank.text = "";
			lord_name.text= "";
			lord_level.text = "";
			guild_name.text = "";
			force_name.text = "";
		}
	}
	//打开玩家信息窗口 
	void OpenPlayerInfoWin()
	{
		if(account_id != 0)
		{
			U3dCmn.OpenPlayerCardWin(account_id);
		}
	}
}
