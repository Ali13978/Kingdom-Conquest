using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class MessageItem : MonoBehaviour {
	public UILabel send_char_name;
	public UILabel time;
	public UILabel content;
	public UIImageButton delete_btn;
	public UIImageButton receive_btn;
	public UISprite read_icon;
	MessageManager.MessageData mail_data;
	
	string alliance_title;
	string private_title;
	string sys_title;
	public int id;
	byte isread;
	void Awake()
	{
		string[] labels = content.text.Split(',');
		alliance_title = labels[0];
		private_title = labels[1];
	}
	// Use this for initialization
	void Start () {
		
	}
	
	void InitialData(MessageManager.MessageData unit)
	{
		mail_data = unit;
		
		if(unit.mail_id != 0)
		{
			if(unit.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_PRIVATE)
			{
				PrivateMailUnit data = (PrivateMailUnit)MessageManager.PersonalMsgMap[unit.mail_id];
				send_char_name.text = DataConvert.BytesToStr(data.szSender8);
				content.text = "[00FF00]"+private_title+"   [FFFFFF]"+DataConvert.BytesToStr(data.szText10);
				time.text = DataConvert.TimeIntToStr(data.nTime6);
				SetReadIcon(data.bReaded5);
			
			}
			else if(unit.msg_type ==(byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE)
			{
				PrivateMailUnit data = (PrivateMailUnit)MessageManager.PersonalMsgMap[unit.mail_id];
				send_char_name.text = DataConvert.BytesToStr(data.szSender8);
				content.text = "[00FFFF]"+alliance_title+"   [FFFFFF]"+DataConvert.BytesToStr(data.szText10);
				time.text = DataConvert.TimeIntToStr(data.nTime6);
				SetReadIcon(data.bReaded5);
			}
			else if(unit.msg_type ==(byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT)
			{
				PrivateMailUnit data = (PrivateMailUnit)MessageManager.AllianceEventMap[unit.mail_id];
				send_char_name.text = "";
				content.text = DataConvert.BytesToStr(data.szText10);
				time.text = DataConvert.TimeIntToStr(data.nTime6);
				read_icon.gameObject.SetActiveRecursively(false);
				receive_btn.gameObject.SetActiveRecursively(false);
				delete_btn.gameObject.SetActiveRecursively(true);
			}
			else if(unit.msg_type ==(byte)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT)
			{
				PrivateMailUnit data = (PrivateMailUnit)MessageManager.GoldEventMap[unit.mail_id];
				send_char_name.text = "";
				content.text = DataConvert.BytesToStr(data.szText10);
				time.text = DataConvert.TimeIntToStr(data.nTime6);
				read_icon.gameObject.SetActiveRecursively(false);
				receive_btn.gameObject.SetActiveRecursively(false);
				delete_btn.gameObject.SetActiveRecursively(true);
			}
			else if(unit.msg_type ==(byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
			{
				PrivateMailUnit data = (PrivateMailUnit)MessageManager.SysMsgMap[unit.mail_id];
				
				if(data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_NORMAL)
				{
					send_char_name.text = "";
					time.text = DataConvert.TimeIntToStr(data.nTime6);
					content.text = DataConvert.BytesToStr(data.szText10);
					receive_btn.gameObject.SetActiveRecursively(false);
					delete_btn.gameObject.SetActiveRecursively(true);
				}
				else if(data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_FIGHT)
				{
					string combat_log = "";
					CombatLog log = DataConvert.ByteToStruct<CombatLog>(data.szExtData12);		
					int head_len = U3dCmn.GetSizeofSimpleStructure<CombatLog>();
					head_len +=	U3dCmn.GetSizeofSimpleStructure<CombatUnit4Log>()*(log.n1Num7+log.n2Num8);
					int data_len = data.nExtDataLen11 - head_len;
					byte[] data_buff = new byte[data_len];
					Array.Copy(data.szExtData12,head_len,data_buff,0,data_len);
					//print ("tttttttttttttttttt"+log.nCombatType1);
					try
					{
						if(log.nCombatType1 == (int)COMBAT_TYPE.COMBAT_PVP_ATTACK )
						{
							if(log.nCombatRst2 == 1)
							{
								CombatLog_PVP pvp_log = DataConvert.ByteToStruct<CombatLog_PVP>(data_buff);
								if(pvp_log.nCup4 >0)
								{
									if(pvp_log.nAddPopulation3 == pvp_log.nPopulation2)
										combat_log =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVP_WIN_CUP),pvp_log.nCup4,pvp_log.nGold1,pvp_log.nPopulation2);
									else if(pvp_log.nAddPopulation3 < pvp_log.nPopulation2)
										combat_log =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVP_WIN_CUP_FULL),pvp_log.nCup4,pvp_log.nGold1,pvp_log.nPopulation2,pvp_log.nAddPopulation3);
								}
								else 
								{
									if(pvp_log.nAddPopulation3 == pvp_log.nPopulation2)
										combat_log =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVP_WIN_RESULT),pvp_log.nGold1,pvp_log.nPopulation2);
									else if(pvp_log.nAddPopulation3 < pvp_log.nPopulation2)
										combat_log =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVP_WIN_RESULT_NOT_FULL),pvp_log.nGold1,pvp_log.nPopulation2,pvp_log.nAddPopulation3);
								}
								
							}
							
						}
						else if(log.nCombatType1 == (int)COMBAT_TYPE.COMBAT_PVP_DEFENSE)
						{
							//print ("rrrrrrrrrr"+log.nCombatRst2);
							if(log.nCombatRst2 == 1)
							{
								CombatLog_PVP pvp_log = DataConvert.ByteToStruct<CombatLog_PVP>(data_buff);	
								if(pvp_log.nCup4 >0)
								{
									combat_log =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVP_LOSE_CUP),pvp_log.nCup4,pvp_log.nGold1,pvp_log.nPopulation2);
								}
								else
								{
									combat_log =string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVP_LOSE_RESULT),pvp_log.nGold1,pvp_log.nPopulation2);
								}
							}
						}
						else if(log.nCombatType1 == (int)COMBAT_TYPE.COMBAT_PVE)
						{
							CombatLog_PVE_Raid pve_log = DataConvert.ByteToStruct<CombatLog_PVE_Raid>(data_buff);	
							string item_str = U3dCmn.GetItemIconByExcelID((uint)pve_log.nExcelID1).Name;
							if(pve_log.nNum2>0)
								combat_log =". "+string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PVE_WIN_RESULT),item_str,pve_log.nNum2);
							
						}
						else if(log.nCombatType1 == (int)COMBAT_TYPE.COMBAT_GOLD)
						{
							
						}
					}
					catch(Exception e)
					{		
						combat_log = "";
						//return;
					}	
					
					send_char_name.text = "";
					time.text = DataConvert.TimeIntToStr(data.nTime6);
					content.text = DataConvert.BytesToStr(data.szText10)+combat_log;
					SetReadIcon(data.bReaded5);
					receive_btn.gameObject.SetActiveRecursively(false);
					delete_btn.gameObject.SetActiveRecursively(true);
				}
				else if(data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_REWARD)
				{
					send_char_name.text = "";
					time.text = DataConvert.TimeIntToStr(data.nTime6);
					content.text = DataConvert.BytesToStr(data.szText10);
					content.text += MessageManager.ExplainRewardMail(data.szExtData12);
					receive_btn.gameObject.SetActiveRecursively(true);
					delete_btn.gameObject.SetActiveRecursively(false);
					
				}
				else if(data.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_NEWFIGHT)
				{
					send_char_name.text = "";
					time.text = DataConvert.TimeIntToStr(data.nTime6);
					content.text = DataConvert.BytesToStr(data.szText10);
					receive_btn.gameObject.SetActiveRecursively(false);
					delete_btn.gameObject.SetActiveRecursively(true);	
				}
				SetReadIcon(data.bReaded5);
			}
		}
		else
		{
			read_icon.gameObject.SetActiveRecursively(false);
			send_char_name.text = "";
			time.text = "";
			content.text = "";
			delete_btn.gameObject.SetActiveRecursively(false);
			receive_btn.gameObject.SetActiveRecursively(false);
		}
	}
	//删除邮件 
	void DeleteMail()
	{
		if(mail_data.mail_id !=0)
		{
			string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SRUE_DELETE_THIS_MAIL);
			PopConfirmWin.instance.ShowPopWin(str,SrueDeleteMail);
		}
	}
	void SrueDeleteMail(GameObject obj)
	{
		if(mail_data.mail_id !=0)
		{
			MessageManager.ReqDeleteMail(mail_data.mail_id);
		}
	}
	//打开查看邮件窗口 
	void OpenReadWin()
	{
		if(mail_data.mail_id != 0)
		{
			if(mail_data.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_GOLDORE_EVENT)
			{
				PrivateMailUnit data = (PrivateMailUnit)MessageManager.GoldEventMap[mail_data.mail_id];
				SIMPLE_CHAR_INFO char_info;
				char_info.AccountId = data.nSenderID2;
				char_info.CharName = DataConvert.BytesToStr(data.szSender8);
				OthersJinKuangManager.OpenOthersJinKuangWin(char_info);	
			}
			else if(mail_data.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_ALLIANCE_EVENT)
			{
				return ;
			}
			else 
			{
				
				if(mail_data.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_SYSTEM)
				{
					PrivateMailUnit mail_unit = (PrivateMailUnit)MessageManager.SysMsgMap[mail_data.mail_id];
					if( mail_unit.nFlag4 == (byte)MAIL_FLAG.MAIL_FLAG_NEWFIGHT)
					{
						MessageManager.ReadMail(mail_data.mail_id);
						read_icon.gameObject.SetActiveRecursively(false);
						GameObject win = U3dCmn.GetObjFromPrefab("BattleLogWin");
				        if (win != null)
				        {
				            BattleLogWin log_win = win.GetComponent<BattleLogWin>();
				            log_win.InitialByMail(mail_data);
				        }
						return;
					}
				}
				
				GameObject obj  = U3dCmn.GetObjFromPrefab("ReadMessageWin");
				if (obj != null)
				{
					obj.SendMessage("RevealPanel",mail_data);
				}
				
				if(MessageManager.SysMsgMap.Contains(mail_data.mail_id))
				{
					//如果是带附件的邮件 不设置为已读 
					PrivateMailUnit data = (PrivateMailUnit)MessageManager.SysMsgMap[mail_data.mail_id];
					if(data.nFlag4 != (byte)MAIL_FLAG.MAIL_FLAG_REWARD)
					{
						MessageManager.ReadMail(mail_data.mail_id);
						read_icon.gameObject.SetActiveRecursively(false);
					}
				}
				else 
				{
					MessageManager.ReadMail(mail_data.mail_id);
					read_icon.gameObject.SetActiveRecursively(false);
				}
				
			}
		
			
		}
		
	}
	//设置邮件是否读过 
	void SetReadIcon(byte readed)
	{
		isread = readed;
		if(readed == 1)
		{
			read_icon.gameObject.SetActiveRecursively(false);
		}
		else 
		{
			read_icon.gameObject.SetActiveRecursively(true);
		}
	}
	//领取带附件的邮件 
	void ReceiveRewardMail()
	{
		MessageManager.ReceiveRewardMail(mail_data.mail_id);
		//print("jujianjujianjujian");
	}
}
