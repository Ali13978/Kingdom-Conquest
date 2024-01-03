using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AnnounceItem : MonoBehaviour {
	public UILabel time;
	public UILabel content;
	
	MessageManager.MessageData mail_data;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void InitialData(MessageManager.MessageData unit)
	{
		mail_data = unit;
		
		if(unit.mail_id != 0)
		{
			if(unit.msg_type == (byte)MAIL_TYPE.MAIL_TYPE_ANNOUNCE)
			{
				Web_Notification_T data = (Web_Notification_T)MessageManager.AnnounceMsgMap[(int)unit.mail_id];
				content.text = DataConvert.BytesToStr(data.pszContent7);
				time.text = DataConvert.TimeIntToStr(data.nTime3);
			
			}
			
		}
		else
		{
			time.text = "";
			content.text = "";
			
		}
	}

	//打开查看邮件窗口 
	void OpenReadWin()
	{
		if(mail_data.mail_id != 0)
		{	
			GameObject obj  = U3dCmn.GetObjFromPrefab("ReadMessageWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",mail_data);
			}
		}
		
	}
}
