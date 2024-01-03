using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CMNCMD;
public class ReadMessageWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UILabel send_char_name;
	public UILabel title;						// 消息标题 ...
	public UILabel content;						// 消息上下文 ...
	public UILabel enemy_title;					// 敌方战力 ...
	public UILabel my_title;					// 我方战力 ...
	public GameObject reply_btn;
	public GameObject view_battle_btn;
	public GameObject locate_btn;
	public GameObject lookover_btn;
	public GameObject battleobj;
	PrivateMailUnit mail_data;
	CombatLog  now_combat_log = new CombatLog();
	private string enemy_char_name;
	private bool IsAttacker = false;

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
		
		// 初始界面不显示 ...
		if (content != null) {
			content.enabled = false;
		}
		if (title != null) {
			title.enabled = false;
		}
		if (lookover_btn != null)
		{
			lookover_btn.SetActiveRecursively(false);
		}
		if (locate_btn != null)
		{
			locate_btn.SetActiveRecursively(false);
		}
		if (battleobj != null)
		{
			battleobj.SetActiveRecursively(false);
		}
		if (reply_btn != null)
		{
			reply_btn.SetActiveRecursively(false);
		}
		if (view_battle_btn != null)
		{
			view_battle_btn.SetActiveRecursively(false);
		}
	}
	
	// 初始化面板 ...
	void RevealPanel(MessageManager.MessageData unit)
	{
		if(unit.msg_type ==(byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
		{
			mail_data = (PrivateMailUnit)MessageManager.SysMsgMap[unit.mail_id];
			if(mail_data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_FIGHT)
			{
				bool isCombatPVP = false;
				CombatLog log = DataConvert.ByteToStruct<CombatLog>(mail_data.szExtData12);		
				now_combat_log = log;
				if((log.nCombatRst2 ==1) && (log.nCombatType1 == (int)COMBAT_TYPE.COMBAT_WANGZHEZHILU || log.nCombatType1 == (int)COMBAT_TYPE.COMBAT_ZHENGZHANTIANXIA))
				{
					int head_len = U3dCmn.GetSizeofSimpleStructure<CombatLog>();
					head_len +=	U3dCmn.GetSizeofSimpleStructure<CombatUnit4Log>()*(log.n1Num7+log.n2Num8);
					int data_len = mail_data.nExtDataLen11 - head_len;
					byte[] data_buff = new byte[data_len];
					Array.Copy(mail_data.szExtData12,head_len,data_buff,0,data_len);
					
					try
					{
						CombatLog_InstanceLoot combat_log = DataConvert.ByteToStruct<CombatLog_InstanceLoot>(data_buff);
						if(combat_log.nNum1 >0)
						{	
							head_len = U3dCmn.GetSizeofSimpleStructure<CombatLog_InstanceLoot>();
					 		data_len = data_buff.Length - head_len;
							byte[] buff = new byte[data_len];
							Array.Copy(data_buff,head_len,buff,0,data_len);
							LootDesc[] loot_list = (LootDesc[])DataConvert.ByteToStructArray<LootDesc>(buff,combat_log.nNum1);
							
							string cc = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GET_REWARD_LIST)+"\n";
							for(int i=0;i<loot_list.Length;i++)
							{
								ITEM_INFO d1 = U3dCmn.GetItemIconByExcelID((uint)loot_list[i].nExcelID1);
								cc = cc + (d1.Name + " x " + loot_list[i].nNum2 + "\n");
							}
							
							content.text = cc;
							content.enabled = true;
						}
					}
					catch(Exception e)
					{		
						return;
					}
					
					
				}
				else
				{
					IsAttacker = true;
					int my_army_num =0;
					int his_army_num =0;
					int my_army_prof = 0;
					int his_army_prof = 0;
					
					//print ("mail_data len: "+mail_data.szExtData12.Length);
					List<COMBAT_DEATH_INFO> log_array = CombatManager.AcceptMyCombatLogData(mail_data.szExtData12,out my_army_num,out his_army_num,out my_army_prof,out his_army_prof);
					my_title.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BATTLE_LOG_MY_TITLE),my_army_prof);
					enemy_title.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.BATTLE_LOG_ENEMY_TITLE),his_army_prof);
					
					// 战报显示 ...
					battleobj.SetActiveRecursively(true);
					int index = 0;
					int i=0;
					for(i=0;i<my_army_num;i++)
					{
						GameObject obj = U3dCmn.GetChildObjByName(battleobj,"myitem"+i);
						obj.SendMessage("SetDeathData",log_array[index]);
						index++;
						
					}
					for(;i<5;i++)
					{
						GameObject obj = U3dCmn.GetChildObjByName(battleobj,"myitem"+i);
						obj.SetActiveRecursively(false);
					}
					for(i=0;i<his_army_num;i++)
					{
						GameObject obj  = U3dCmn.GetChildObjByName(battleobj,"hisitem"+i);
						obj.SendMessage("SetDeathData",log_array[index]);
						index++;
						
					}
					for(;i<5;i++)
					{
						GameObject obj = U3dCmn.GetChildObjByName(battleobj,"hisitem"+i);
						obj.SetActiveRecursively(false);
					}
					// 当前战斗类型决定敌方名字 ...
					if (log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE ||
						log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE )
					{
						enemy_char_name = DataConvert.BytesToStr(log.szAttackName4);
						locate_btn.SetActiveRecursively(true);
						isCombatPVP = true;
						
					}
					else if (log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_ATTACK ||
						log.nCombatType1 == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_ATTACK )
					{
						enemy_char_name = DataConvert.BytesToStr(log.szDefenseName6);
						locate_btn.SetActiveRecursively(false);
						isCombatPVP = false;
					}
				}
				
				if ( isCombatPVP == true)
				{
					Transform m1 = view_battle_btn.transform;
					Vector3 p1 = m1.localPosition;
					m1.localPosition = new Vector3(-50f,p1.y,p1.z);
					
					Transform m2 = locate_btn.transform;
					Vector3 p2 = m2.localPosition;
					m2.localPosition = new Vector3(+50f,p2.y,p2.z);
				}

				view_battle_btn.SetActiveRecursively(true);
			}
			else if(mail_data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_REWARD)
			{
				//print("rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
				// 系统邮件 ...
				content.text = DataConvert.BytesToStr(mail_data.szText10);
				content.text += MessageManager.ExplainRewardMail(mail_data.szExtData12);
				content.enabled = true;				
				//content.text = content.text.Replace("[FF0000]","");
			}
			else if(mail_data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_NORMAL)
			{
				content.text = DataConvert.BytesToStr(mail_data.szText10);
				content.enabled = true;
				string send_charname =  DataConvert.BytesToStr(mail_data.szSender8);
				if(send_charname != "")
				{
					enemy_char_name = send_charname;
					lookover_btn.SetActiveRecursively(true);
					lookover_btn.transform.localPosition = new Vector3(0,lookover_btn.transform.localPosition.y,lookover_btn.transform.localPosition.z);
				}
				//	content.text = content.text.Replace("[FF0000]","");
				//print ("ccccccccccccc"+DataConvert.BytesToStr(mail_data.szText10));
			}
		
		}
		else if(unit.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE || unit.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE)
		{
			mail_data = (PrivateMailUnit)MessageManager.PersonalMsgMap[unit.mail_id];
			title.text = "From";
			title.enabled = true;
			
			send_char_name.text = DataConvert.BytesToStr(mail_data.szSender8);
			content.text = DataConvert.BytesToStr(mail_data.szText10);
			content.enabled = true;
			enemy_char_name = DataConvert.BytesToStr(mail_data.szSender8);
			lookover_btn.SetActiveRecursively(true);
			reply_btn.SetActiveRecursively(true);
			lookover_btn.transform.localPosition = new Vector3(70f,lookover_btn.transform.localPosition.y,lookover_btn.transform.localPosition.z);
			reply_btn.transform.localPosition = new Vector3(-70f,reply_btn.transform.localPosition.y,reply_btn.transform.localPosition.z);
		}
		else if(unit.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_ANNOUNCE)
		{
			Web_Notification_T announce_data = (Web_Notification_T)MessageManager.AnnounceMsgMap[(int)unit.mail_id];
			send_char_name.text = "";
			content.text = DataConvert.BytesToStr(announce_data.pszContent7);
			content.enabled = true;
			
			mail_data = new PrivateMailUnit();
		}
		else 
		{
			mail_data = new PrivateMailUnit();
		}
		
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		OtherPlayerInfoManager.UnRegisterCallBack(gameObject);
		Destroy(gameObject);
	}
	//查找敌人
	void SearchEnemyRst(STC_GAMECMD_GET_PLAYERCARD_T player_card)
	{
		//注销回调 
		OtherPlayerInfoManager.UnRegisterCallBack(gameObject);
		if(player_card.nRst1 == (int)STC_GAMECMD_GET_PLAYERCARD_T.enum_rst.RST_OK)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("PlayerCardWin"); 
			if(win != null)
			{
				PlayerCardWin.Counterattack = IsAttacker;
				win.GetComponent<PlayerCardWin>().RevealPanelByParam(player_card);
			}	
		}
		else 
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NO_THIS_PALYER));
		}
		//STC_GAMECMD_GET_PLAYERCARD_T player_card
	}
	// 查看敌方信息 ...
	void OpenEnemyCardWin()
	{
		OtherPlayerInfoManager.RegisterCallBack(gameObject, "SearchEnemyRst");
		OtherPlayerInfoManager.GetOtherPlayerCard(enemy_char_name);
	}
	// 定位敌方信息 ...
	void LocateEnemy()
	{
		U3dCmn.SendMessage("ReadMessageWin","DismissPanel",null);
		U3dCmn.SendMessage("MessageWin","DismissPanel",null);
	    SceneManager.OpenGlobalMapLocation((int)now_combat_log.nPosX12,(int)now_combat_log.nPosY13);
		//OtherPlayerInfoManager.RegisterCallBack(gameObject, "SearchEnemyRst");
		//OtherPlayerInfoManager.GetOtherPlayerCard(enemy_char_name);
	}
	//打开回复邮件窗口 ...
	void OpenReplyWin()
	{
		if(mail_data.nMailID1 != 0 && mail_data.nSenderID2!=0)
		{
			GameObject obj  = U3dCmn.GetObjFromPrefab("WriteMessageWin");
			if (obj != null)
			{
				WriteMessageWin win = obj.GetComponent<WriteMessageWin>(); 
				win.char_id = mail_data.nSenderID2;
				win.recv_charname.text = DataConvert.BytesToStr(mail_data.szSender8);
				win.MailType = (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE;
				obj.SendMessage("RevealPanel");
			}
		}
	
	}
	//观战  
	void ViewBattle()
	{
		if(mail_data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_FIGHT)
		{			
			DismissPanel();
			
			CombatFighting comFighting = CombatManager.AcceptMyCombatLogFighting(mail_data.szExtData12);
			if (comFighting != null) 
			{
				Globe.comFighting = comFighting;
				GameObject go = U3dCmn.GetObjFromPrefab("BaizBattlefieldWin");
				if ( go != null)
				{
					float depth = -BaizVariableScript.DEPTH_OFFSET * 3.0f;
					go.SendMessage("Depth", depth);
				}
			}
			
			//print ("view battle");
		}
	}
}
