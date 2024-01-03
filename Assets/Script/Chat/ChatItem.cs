using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class ChatItem : MonoBehaviour {
	public UILabel title;
	public UILabel charname;
	public UILabel time;
	public UILabel content;
	public UISprite vip_icon;
	public UISlicedSprite select_bg;
	string world_title;
	string alliance_title;
	string private_title;
	string sys_title;
	string team_title;
	public ChatWin chat_win;
	CHAT_DATA now_unit;
	// Use this for initialization
	void Awake()
	{
		string[] labels = title.text.Split(',');
		world_title = labels[0];
		alliance_title = labels[1];
		private_title = labels[2];
		sys_title = labels[3];
		team_title = labels[4];
	}
	void Start () {
		
	}
	
	void InitialData(CHAT_DATA unit)
	{
		now_unit = unit;
		if(unit.ChatType != 0)
		{
			content.text = "";
			if(unit.ChatType == (uint)GAME_SUB_RST.STC_GAMECMD_CHAT_WORLD)
			{
				title.text = "[00FFFF]"+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CHAT_WORLD);
				//title.color = new Color(0,1,1,1);
			}
				
			else if(unit.ChatType == (uint)GAME_SUB_RST.STC_GAMECMD_CHAT_ALLIANCE)
			{
				title.text = "[00FF00]"+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CHAT_ALLIANCE);	
				//title.color = new Color(0,1,0,1);
			}
			else if(unit.ChatType == (uint)GAME_SUB_RST.STC_GAMECMD_CHAT_PRIVATE)
			{
				title.text = "[FF00FF]"+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CHAT_PRIVATE);	
				//title.color = new Color(1,0,0,1);
				content.text = "to "+unit.szTo+":";
			}
			else if(unit.ChatType == (uint)GAME_SUB_RST.STC_GAMECMD_SYS_REPORT)
			{
				title.text = "[FF0000]"+sys_title;	
			}
			else if(unit.ChatType == (uint)GAME_SUB_RST.STC_GAMECMD_CHAT_GROUP)
			{
				title.text = "[FFFF00]"+team_title;
			}
			
			charname.color = new Color(1,1,1,1); 
			charname.text = unit.szFrom;
			content.text += unit.szText;
			time.text = DataConvert.TimeIntToStr(unit.ChatTime);
			Bounds	bounds1	= NGUIMath.CalculateRelativeWidgetBounds(time.transform);
			Bounds	bounds2	= NGUIMath.CalculateRelativeWidgetBounds(content.transform);
			
			float len = Mathf.Abs(bounds1.extents.y-bounds1.center.y)*time.transform.localScale.y + Mathf.Abs(bounds2.extents.y-bounds2.center.y)*content.transform.localScale.y +12.0f;
			select_bg.transform.localScale	= new Vector3(select_bg.transform.localScale.x,len,1);
			//UIFont mFont;
			float text_len = charname.font.CalculatePrintedSize(charname.text, true,UIFont.SymbolStyle.Uncolored).x;
			float vip_x = text_len*charname.transform.localScale.x+charname.transform.localPosition.x+1;
		
			if(unit.Vip >0)
				vip_icon.spriteName = "vip"+unit.Vip;
			else
				vip_icon.spriteName = "transparent";
			vip_icon.transform.localPosition = new Vector3(vip_x,vip_icon.transform.localPosition.y,vip_icon.transform.localPosition.z);
			select_bg.spriteName = "sliced_bg2";
		}
		else
		{
			title.text= "";
			charname.text = "";
			content.text = "";
			time.text = "";
			select_bg.spriteName = "transparent";
			vip_icon.spriteName = "transparent";
			//select_bg.color = new Color(select_bg.color.r,select_bg.color.g,select_bg.color.b,20.0f);
		}
		
	}
	
	void SelectItem()
	{
		//if(now_unit.nCharID!=0 && now_unit.nCharID != CommonData.player_online_info.AccountID)
		if(now_unit.nCharID!=0)
		{
			chat_win.RefreshSelectBg(select_bg);
			select_bg.spriteName = "sliced_bg";
			GameObject obj  = U3dCmn.GetObjFromPrefab("SelectCharWin");
			if (obj != null)
			{
				obj.SendMessage("RevealPanel",now_unit);
			}
			
		}
		
		//select_bg.color = new Color(select_bg.color.r,select_bg.color.g,select_bg.color.b,255.0f);
	}
}
