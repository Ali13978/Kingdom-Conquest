using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using STCCMD;
using CTSCMD;
public class LoadingManager : MonoBehaviour {
	bool loginover =  true;
	static LoadingManager mInst = null;
	GameObject LoadingSprite = null;
	static public int err = (int)MB_WARN_ERR.NET_REQ_TIMEOUT;
	// 超时时间 
	const int TimeOut = 30;
	// 命令列表 ...
	List<int>CommandList = new List<int>();
	static public LoadingManager instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(LoadingManager)) as LoadingManager;

				if (mInst == null)
				{
					GameObject go = new GameObject("LoadingManager");
					DontDestroyOnLoad(go);
					mInst = go.AddComponent<LoadingManager>();
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }
	
	void Start ()
	{
		
	}
	
	//LOADING  
	public void _ShowLoading(int t)
	{
		if(LoadingSprite == null)
		{
			LoadingSprite = U3dCmn.GetObjFromPrefab("LoadingBg");
		}
		
		if(LoadingSprite!=null)
		{
			LoadingSprite.SetActiveRecursively(true);
			LoadingSprite.transform.localPosition = new Vector3(0,0,-9.9f);
			StopCoroutine("Countdown");
			StartCoroutine("Countdown", t);
		}
		
	}
	public void ShowLoading()
	{
		
		LoadingManager.err = (int)MB_WARN_ERR.NET_REQ_TIMEOUT;
		_ShowLoading(TimeOut);
	}
	//隐藏LOADING 
	public void HideLoading()
	{
		if(LoadingSprite!=null)
		{
			LoadingSprite.SetActiveRecursively(false);
			LoadingSprite.transform.localPosition = new Vector3(-10.0f,-10.0f,0);
		}
		StopCoroutine("Countdown");
		if(!loginover)
		{
			//登陆过程结束
			loginover = true;
			U3dCmn.SendMessage("SceneManager","LoginOver",null);
		}
	}
	// 命令消息列表清空 ...
	public void CmdListClear()
	{
		//print ("CmdListClear");
		CommandList.Clear();
	}
	//超时倒计时  
	IEnumerator Countdown(int Secs)
	{
		if(Secs <0)
			Secs = 0;
		while(Secs!=0)
		{
			Secs--;
			if(Secs <0)
				Secs = 0;
			yield return new WaitForSeconds(1);
			//print ("hhhhhhhhhhhhhh");
		}
		
		HideLoading();
		CommandList.Clear();
		
		// 这条消息弹了 .... 1分种以内吧 ....
		U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB(err));
	}
	//登陆游戏需要初始化数据的指令集 
	public void LoginAddInitialData()
	{
		loginover = false;
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_ALLIANCE_JOIN_EVENT);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_BUILDING_LIST);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_BUILDING_TE_LIST);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_CHRISTMAS_TREE_INFO);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_ITEM);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_EQUIP);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_GEM);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_FRIEND_LIST);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_OTHERS_FRIEND_APPLY);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_GET_HERO_HIRE);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_SOLDIER);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_MAIL);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_PRODUCTION_EVENT);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_PRODUCTION_TE_LIST);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_MY_RANK);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_TECHNOLOGY);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_RESEARCH_TE);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_TRAINING_TE);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_GET_SERVER_TIME);
		CommandList.Add((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_GET_STATUS);
	}
	public void LoginRemoveInitialData(int cmd)
	{
		
		if(CommandList.Count>0)
		{
			CommandList.Remove(cmd);
			if(CommandList.Count == 0)
				HideLoading();
		}
	}
	//需要LOADING的地方加入需要返回的指令 
	public void AddLoadingCmd(int cmd)
	{
		CmdListClear();
		//bool need_show = (CommandList.Count == 0);
		//bool no_hide = false;
		
		// 未显示 ...
		//if (LoadingSprite != null) {
		//	no_hide = (false == LoadingSprite.active);
		//}
		
		if(false == CommandList.Contains(cmd))
		{
			CommandList.Add(cmd);
		}
		
		//if (need_show == true || no_hide == true)
		//{
			
			ShowLoading();
		//}
	
	}
	//检查所有指令是否到达是否消除LOADING 
	public void CheckCmdReturn(int cmd)
	{
		if(CommandList.Count>0)
		{
			CommandList.Remove(cmd);
			if(CommandList.Count == 0)
				HideLoading();
		}
	}
}
