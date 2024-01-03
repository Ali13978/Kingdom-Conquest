using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CMNCMD;
using CTSCMD;
using STCCMD;


public class ArmyDeployWin : MonoBehaviour {
	
	
	//窗口positon TWEEN     
	public TweenPosition tween_position; 
	//底部按钮 
	public GameObject bottom_btn;
	public UICheckbox drag_unit_check;
	public UICheckbox drag_column_check;
	public UILabel total_force_label;
	public UILabel enemy_total_force_label;
	//联盟副本按钮总OBJ 
	public GameObject guildwar_btn_obj;
	//联盟副本自动排序按钮 
	/*public GameObject guildwar_autoarray_btn;
	//联盟副本踢人按钮 
	public GameObject guildwar_kick_btn;
	//联盟副本手动补给按钮 
	public GameObject guildwar_supply_btn;
	//联盟副本开始战斗按钮 
	public GameObject guildwar_startbattle_btn;*/
	
	//顶部按钮 
	public GameObject top_btn;
	//自动补给 
	public GameObject auto_supply_obj;
	public UICheckbox auto_supply_check;
	public UIInput    stop_level;
	
	//竞技场按钮总OBJ 
	public GameObject arena_btn_obj;
	
	//拖拽单元还是拖拽列 
	public DRAG_TYPE drag_type = DRAG_TYPE.UNIT;
	public GameObject troop_item;
	public int maxRows = 5;
	public int maxColumns = 5;
	
	public int spacing = 128;
	//自己的军队部署 
	public GameObject deploy_scene;
	//敌方军队部署  
	public GameObject enemy_scene;
	public GameObject enemy_item;
	public ArrowAnimation animation;
	bool  FirstOpen = false;
	public TroopItem drag_item;
	public TroopItem[] drag_column;
	public List<GameObject> troop_item_list;

	public TroopItem target_item;
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
		drag_column = null;
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			bottom_btn.transform.localPosition = bottom_btn.transform.localPosition+new Vector3(0,-20,0);
			top_btn.transform.localPosition = top_btn.transform.localPosition+new Vector3(0,20,0);
		}
	}
	// Use this for initialization
	void Start () {
		if(drag_unit_check.isChecked)
		{
			drag_type = DRAG_TYPE.UNIT;
		}
		else if(drag_column_check.isChecked)
		{
			drag_type = DRAG_TYPE.COLUMN;
		}
		// iphone5 遮挡 ...
		KeepOut.instance.ShowKeepOut();
	}

	void RevealPanel(COMBAT_TYPE type)
	{
		ArmyDeployManager.deploy_type = type;
		if(type == COMBAT_TYPE.COMBAT_INSTANCE_GUILD)
		{
			arena_btn_obj.SetActiveRecursively(false);
			if(GBStatusManager.Instance.MyStatus.nCreatorID != CommonData.player_online_info.AccountID)
			{
				//如果不是队长 只能看阵容 
				guildwar_btn_obj.SetActiveRecursively(false);
				drag_unit_check.gameObject.SetActiveRecursively(false);
				drag_column_check.gameObject.SetActiveRecursively(false);
				drag_type = DRAG_TYPE.NONE;
			}
			if(GBStatusManager.Instance.MyStatus.bAutoCombat == (byte)1)
			{
				auto_supply_obj.SetActiveRecursively(true);
			}
			else 
			{
				auto_supply_obj.SetActiveRecursively(false);
			}
			stop_level.text = GBStatusManager.Instance.MyStatus.MaxLevel.ToString();
	        GBTeamManager.Instance.OnKickMemberComplete += this.OnKickComplete;
	        GBTeamManager.Instance.OnQuitTeamComplete += this.OnMemberQuit;
	        ArmyDeployManager.AutoArrangeFinished += AutoArrangeRst;
			ArmyDeployManager.instance.ReqEnemyDepoyData(type);
			return ;
		}
		else if(type == COMBAT_TYPE.COMBAT_ARENA)
		{
			guildwar_btn_obj.SetActiveRecursively(false);
			auto_supply_obj.SetActiveRecursively(false);
			guildwar_btn_obj.SetActiveRecursively(false);
			ArmyDeployManager.AutoArrangeFinished += AutoArrangeRst;
		}
		
		ShowPanel();
	}
	//弹出窗口 
	void ShowPanel()
	{
		FirstOpen = true;
		InitialDeployScene();
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		
		if(ArmyDeployManager.deploy_type == COMBAT_TYPE.COMBAT_INSTANCE_GUILD)
		{
			GBTeamManager.Instance.OnKickMemberComplete -= this.OnKickComplete;
			GBTeamManager.Instance.OnQuitTeamComplete -= this.OnMemberQuit;
	        ArmyDeployManager.AutoArrangeFinished = null;
		}
       	if(ArmyDeployManager.deploy_type == COMBAT_TYPE.COMBAT_ARENA)
		{
			ArmyDeployManager.AutoArrangeFinished = null;
		}
		ArmyDeployManager.deploy_type = COMBAT_TYPE.NONE;
		tween_position.Play(false);
		Destroy(gameObject);
		ArenaManager.is_deploy = false;
		// iphone5 遮挡挪开 ...
		KeepOut.instance.HideKeepOut();
	}
	//军队部署场景初始化 
	void InitialDeployScene()
	{
		foreach(Transform obj in deploy_scene.gameObject.transform)
		{
			Destroy(obj.gameObject);
		}
		troop_item_list.Clear();
		int total_force = 0;
		if (troop_item != null)
		{
			int count = 0;
			for (int x = 0; x < maxColumns; ++x)
			{
				for (int y = 0; y < maxRows; ++y)
				{
					
					GameObject go = NGUITools.AddChild(deploy_scene, troop_item);
					Transform t = go.transform;
					t.localPosition = new Vector3((0.5f -x ) * spacing,  ( 0.5f-y) * spacing, 0f);
					t.name = "troop"+count;
					TroopItem item = t.GetComponent<TroopItem>();
					item.item_data = ArmyDeployManager.instance.GetTroopUnitDataByID(count);
					total_force += item.item_data.Force;
					item.InitialData();
					troop_item_list.Add(go);
					count++;
				}
			}
		}
		total_force_label.text = total_force.ToString();
		if(ArmyDeployManager.deploy_type == COMBAT_TYPE.COMBAT_INSTANCE_GUILD)
		{
			InitialEnemyDeployScene();
		}
		
	}
	//显示敌人的军队部署 
	void InitialEnemyDeployScene()
	{
		foreach(Transform obj in enemy_scene.gameObject.transform)
		{
			Destroy(obj.gameObject);
		}
		int enemy_total_force = 0;
		if (enemy_item != null)
		{
			int count = 0;
			for (int x = 0; x < maxColumns; ++x)
			{
				for (int y = 0; y < maxRows; ++y)
				{
					
					GameObject go = NGUITools.AddChild(enemy_scene, enemy_item);
					Transform t = go.transform;
					t.localPosition = new Vector3((x-0.5f ) * spacing,  (0.5f-y) * spacing, 0f);
					t.name = "troop"+count;
					EnemyTroopItem item = t.GetComponent<EnemyTroopItem>();
					item.item_data = ArmyDeployManager.instance.GetEnemyTroopUnitDataByID(count);
					enemy_total_force += item.item_data.Force;
					item.InitialData();
					count++;
				}
			}
		}
		enemy_total_force_label.text = enemy_total_force.ToString();
		if(FirstOpen)
		{
			animation.StartPlay();
			FirstOpen = false;
		}
		
	}
	//自动排兵布阵（按照兵种）  
	public void AutoArrange()
	{
		ArmyDeployManager.instance.AutoArrange();
	}
	//自动排兵回调 刷新界面  
	public void AutoArrangeRst()
	{
		InitialDeployScene();
	}
	//拖拽单元还是拖拽列 
	public void SetDragType()
	{
		if(drag_unit_check.isChecked)
		{
			drag_type = DRAG_TYPE.UNIT;
		}
		else if(drag_column_check.isChecked)
		{
			drag_type = DRAG_TYPE.COLUMN;
		}
	}
	//设置拖拽的ITEM 
	public void SetDragItem(TroopItem item)
	{
		ArmyDeployManager.is_drag = true;
		//target_item = item.;
		drag_item = item;
		drag_item.icon.spriteName = "transparent";
		drag_item.force_label.text = "";
		drag_item.account_name.text = "";
	}
	//归还拖拽的ITEM 
	public void RevertItem()
	{
		if(drag_type == DRAG_TYPE.UNIT)
		{
			if(drag_item != null)
			{
				ArmyDeployManager.is_drag = false;;
				drag_item.InitialData();
				drag_item = null;
				target_item = null;
				UICursor.Clear();
			}
		}
		else if(drag_type == DRAG_TYPE.COLUMN)
		{
			if(drag_column != null)
			{
				ArmyDeployManager.is_drag = false;
				for(int i=0,max = drag_column.Length;i<max;i++)
				{
					drag_column[i].InitialData();
				}
				drag_column = null;
				target_item = null;
				UICursor.Clear();
			}
		}	
	}
	//交换两个ITEM 
	public void ExchangeItem()
	{
		if(drag_item != null && target_item != null)
		{
			int from_id = drag_item.item_data.PosID;
			int to_id = target_item.item_data.PosID;
			//先交换manager里面的布阵数据 
			ArmyDeployManager.instance.ExchangeUnitTroopData(from_id,to_id);
			//然后更新界面 
			drag_item.item_data = ArmyDeployManager.instance.GetTroopUnitDataByID(from_id);
			drag_item.InitialData();
			target_item.item_data = ArmyDeployManager.instance.GetTroopUnitDataByID(to_id);
			target_item.InitialData();
			drag_item = null;
			target_item = null;
			ArmyDeployManager.is_drag = false;
		}
	}

	//设置拖拽的column  
	public void SetDragColumn(TroopItem item)
	{
		ArmyDeployManager.is_drag = true;
		drag_item = item;
		drag_column = GetColumnByPosID(item.pos_id);
		for(int i=0,max = drag_column.Length;i<max;i++)
		{
			drag_column[i].icon.spriteName = "transparent";
			drag_column[i].force_label.text = "";
			drag_column[i].account_name.text = "";
		}
	}
	//根据位置ID获取同一列的ITEM 
	public TroopItem[] GetColumnByPosID(int pos_id)
	{
		TroopItem[] data_array = new TroopItem[maxRows];
		int col_num = pos_id/maxRows;
		int begin_num = col_num*maxRows;
		for(int i=0;i<maxRows;i++)
		{
			data_array[i] = troop_item_list[begin_num+i].GetComponent<TroopItem>();
		}
		//troop_item_list 
		return data_array;
	}
	//交换两列 
	public void ExchangeColumn()
	{
		if(drag_item != null && target_item != null)
		{
			
			int from_col = drag_item.item_data.PosID/maxRows;
			int to_col = target_item.item_data.PosID/maxRows;
			if(from_col== to_col)
			{
				RevertItem();
			}
			else 
			{
				ArmyDeployManager.instance.ExchangeColumnTroopData(from_col,to_col,maxRows);
				TroopData[] from_data_list = ArmyDeployManager.instance.GetTroopColDataByID(from_col,maxRows);
				TroopData[] to_data_list = ArmyDeployManager.instance.GetTroopColDataByID(to_col,maxRows);
				for(int i=0;i<maxRows;i++)
				{
					TroopItem item = troop_item_list[from_col*maxRows+i].GetComponent<TroopItem>();
					item.item_data = from_data_list[i];
					item.InitialData();
					
					item = troop_item_list[to_col*maxRows+i].GetComponent<TroopItem>();
					item.item_data = to_data_list[i];
					item.InitialData();
				}
			}
			drag_column = null;
			drag_item = null;
			target_item = null;
			ArmyDeployManager.is_drag = false;
		}
	}

    void OnKickComplete(STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_KICK_T.enum_rst.RST_OK) {
			ArmyDeployManager.instance.InitialArmyDeployData();
            InitialDeployScene();
        }
    }
	
	//停止关卡输出框内容改变 
	void InPutChange()
	{
		if(!U3dCmn.IsNumeric(stop_level.text))
		{
			stop_level.text = GBStatusManager.Instance.MyStatus.MaxLevel.ToString();
		}
		else
		{
			uint num = uint.Parse(stop_level.text);
			if(num<= GBStatusManager.Instance.MyStatus.nCurLevel)
			{
				num = GBStatusManager.Instance.MyStatus.nCurLevel;
			}
			else if(num> GBStatusManager.Instance.MyStatus.MaxLevel)
			{
				num = (uint)GBStatusManager.Instance.MyStatus.MaxLevel;
			}
			stop_level.text = num.ToString();
		}
	}
    void StartGuildBattle() {
        this.DismissPanel();
		if(GBStatusManager.Instance.MyStatus.bAutoCombat == (byte)1)
		{
			if(auto_supply_check.isChecked)
			{
				GBStatusManager.Instance.MyStatus.bAutoSupply = 1;
			}
			GBStatusManager.Instance.MyStatus.nStopLevel =uint.Parse(stop_level.text);
		}
        GameObject _levelWinObj = GameObject.Find(GBWindowPath.GBLevelWin);
        if (_levelWinObj != null)
        {
            GBLevelWin win = _levelWinObj.GetComponent<GBLevelWin>();
            if (win.IsManual)
            {
                GBStatusManager.Instance.MyStatus.bAutoCombat = 0;
            }
            else {
                GBStatusManager.Instance.MyStatus.bAutoCombat = 1;
            }
        }
        //保存当前位置信息  
        ArmyDeployManager.instance.SaveTroopPos();
        U3dCmn.SendMessage(GBWindowPath.GBLevelWin, "StartCombat", null);
    }

    void OnMemberQuit(STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst result)
    {
        if (result == STC_GAMECMD_ALLI_INSTANCE_EXIT_T.enum_rst.RST_OK) {
            ArmyDeployManager.instance.InitialArmyDeployData();
            InitialDeployScene();
        }
    }

    void CloseWin() {
		if(ArmyDeployManager.deploy_type == COMBAT_TYPE.COMBAT_INSTANCE_GUILD)
		{
			if (GBStatusManager.Instance.MyStatus.bAutoCombat == 1) { 
                GBStatusManager.Instance.MyStatus.bAutoCombat = 0;
	        }
	        GameObject _levelWinObj = GameObject.Find(GBWindowPath.GBLevelWin);
	        if (_levelWinObj != null) {
	            GBLevelWin win = _levelWinObj.GetComponent<GBLevelWin>();
                if (win.IsManual)
                {
                    win.IsManual = false;
                }
	        }
		}
       
        this.DismissPanel();
    }
	/// <summary>
    /// 手动补给 
	/// </summary>
	void GuildWarManualSupply()
	{
        GBTeamManager.Instance.ManualSupplyForce();
        LoadingManager.instance.ShowLoading();
	}
    //竞技场手动补给 
	void ArenaManualSupply()
	{

		if(ArenaManager.is_deploy)
		{
			ArmyDeployManager.instance.ReqManualSupply(COMBAT_TYPE.COMBAT_ARENA);
		}
		else
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MUST_DEPLOY_FIRST));
		}
		//BattleManager.Instance.ManualSupply((uint)COMBAT_TYPE.COMBAT_ARENA,0);
        //LoadingManager.instance.ShowLoading();
	}
   
	//踢人 
	void KickPlayer()
	{
        GameObject obj = U3dCmn.GetObjFromPrefab(GBWindowPath.GBKickWin);
        GBKickWin win = obj.GetComponent<GBKickWin>();
        win.MemberList = GBTeamManager.Instance.TeamMemberList;
	}
	//////////////////////////////////////
	//////////////竞技场模块///////////////
	/////////////////////////////////////
	//部署兵力 
	public void ArenaDeploy()
	{
		GameObject go = U3dCmn.GetObjFromPrefab(GBWindowPath.GBLordDeployWin);
        GBLordDeployWin win = go.GetComponent<GBLordDeployWin>();
		win.transform.localPosition = new Vector3(win.transform.localPosition.x,win.transform.localPosition.y,transform.localPosition.z-1);
		win.MaxHeroCapacity = maxRows+maxColumns;
		win.OnApplyHeroList += ArenaDeployFinish;
		win.ShowCheckAllBtn = true;
		win.MaxHeroCapacity = maxRows * maxColumns;
        if (win != null)
        {
            win.AssignHero(HeroState.NORMAL);
        }
	}
	//部署兵力的回调 
	public void ArenaDeployFinish(List<GBLordTroopHero> list)
	{
		ArmyDeployManager.instance.InitialGBLordTroopHero(list);
		U3dCmn.SendMessage("ArmyDeployWin","InitialDeployScene",null);
		ArenaManager.is_deploy = true;
	}
	//保存上传兵力 
	public void ArenaSaveTroop()
	{
		if(!ArenaManager.is_deploy)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MUST_DEPLOY_FIRST));
			return ;
		}
		ArenaManager.Instance.UploadDeployData();
	}
}
