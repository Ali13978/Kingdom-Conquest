using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class LordItem : MonoBehaviour {
	public UIImageButton challenge_btn;
	public UISprite lord_icon;
	public UILabel  lord_rank;
	public UILabel  lord_name;
	public string   lord_rank_str;
	
	ChallengeListUnit now_unit;
	void Awake()
	{
		lord_rank_str = lord_rank.text;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void InitialLordData(ChallengeListUnit lord_info)
	{
		now_unit = lord_info;
		if(lord_info.nAccountID2 != 0)
		{
			lord_icon.spriteName = U3dCmn.GetCharIconName((int)lord_info.nHeadID6);
			lord_rank.text = lord_rank_str+ lord_info.nRank1;
			lord_name.text =  DataConvert.BytesToStr(lord_info.szName4);
			challenge_btn.gameObject.SetActiveRecursively(true);
		}
		else 
		{
			lord_icon.spriteName = U3dCmn.GetTransparentPic();
			lord_rank.text = "";
			lord_name.text =  "";
			challenge_btn.gameObject.SetActiveRecursively(false);
			
		}
	}
	//挑战 
	void Challenge()
	{
		if(now_unit.nAccountID2!=0)
		{
			if(now_unit.nRank1 > ArenaManager.my_rank)
			{
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CAN_NOT_CHALLENGE_LOW));
				//return ;
			}
			if(ArenaManager.try_num <=0)
			{
				//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ARENA_TRY_USEDUP));
				//return ;
			}
			ArenaManager.Instance.ReqChallenge(now_unit.nAccountID2);
			LoadingManager.instance.ShowLoading();
		}
	}
	//打开君主信息简板 
	void OpenPlayerInfoWin()
	{
		if(now_unit.nAccountID2!=0)
		{
			U3dCmn.OpenPlayerCardWin(now_unit.nAccountID2);
		}
	}
}
