using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ComposeGemWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	
	public UILabel now_gem_name;
	public UILabel now_gem_property;
	public UILabel senior_gem_name;
	public UILabel senior_gem_property;
	public UIInput compose_num_input;
	public UILabel compose_need;
	public UILabel now_have;
	public UIImageButton compose_btn;
	ulong  compose_gem_id =0;
	int compose_num =0;
	int excel_id =0;
	string format_str;
	// Use this for initialization
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
		format_str = now_have.text;
	}
			
	void Start () {
		
	}
	
	//弹出窗口   
	void RevealPanel(ulong gem_id)
	{
		compose_btn.gameObject.SetActiveRecursively(false);
		compose_gem_id = gem_id;
		InitialPanelData(gem_id);
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void InitialPanelData(ulong gem_id)
	{
		if(CangKuManager.CangKuItemMap.Contains(gem_id))
		{
			ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[gem_id];
			uint sort_id = (uint)CommonMB.ExcelToSort_Map[(uint)item_info.nExcelID2];
		
			if(sort_id == (uint)ItemSort.GEMSTONE)
			{
				excel_id = item_info.nExcelID2;
				GemInfo now_gem_info = (GemInfo)CommonMB.Gem_Map[(uint)item_info.nExcelID2];
				now_gem_name.text = now_gem_info.GemName;
				now_gem_property.text = now_gem_info.GemDescribe;
				if(!CommonMB.Gem_Map.Contains((uint)(item_info.nExcelID2+10)))
				{
					senior_gem_name.text = "";
					senior_gem_property.text = ""; 
					compose_need.text = "";
					compose_btn.gameObject.SetActiveRecursively(false);
					now_have.text =U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GEM_LEVEL_TOP);
					return ;
				}
				else
				{
					GemInfo senior_gem_info = (GemInfo)CommonMB.Gem_Map[(uint)(item_info.nExcelID2+10)];
					senior_gem_name.text = senior_gem_info.GemName;
					senior_gem_property.text = senior_gem_info.GemDescribe;
					compose_need.text = now_gem_info.GemName+"*4";
					
				
					compose_num = item_info.nNum3/4;
					
					now_have.text= string.Format(format_str,item_info.nNum3,compose_num);
					compose_num_input.text = compose_num.ToString();
					if(compose_num ==0)
					{
						now_have.color = new Color(1,0,0,1);
					}
					else
					{
						now_have.color = new Color(0,1,0,1);
						compose_btn.gameObject.SetActiveRecursively(true);
					}
				}
				
			}
		}
		
	}
	void AddComposeNum()
	{
		if(U3dCmn.IsNumeric(compose_num_input.text))
		{
			int num = int.Parse(compose_num_input.text);
			if(num+1 <compose_num && num+1>=0)
			{
				compose_num_input.text = (num+1).ToString();
			}
			else 
			{
				compose_num_input.text = compose_num.ToString();
			}
		}
		
	}
	void SubComposeNum()
	{
		if(U3dCmn.IsNumeric(compose_num_input.text))
		{
			int num = int.Parse(compose_num_input.text);
			if(num-1 >=0 && num<=compose_num)
			{
				compose_num_input.text = (num-1).ToString();
			}
			else 
			{
				compose_num_input.text = "0";
			}
		}
	}
	void InPutChange()
	{
		if(U3dCmn.IsNumeric(compose_num_input.text))
		{
			int num = int.Parse(compose_num_input.text);
			if(num >compose_num)
			{
				compose_num_input.text = compose_num.ToString();
			}
			else if(num<0)
			{
				compose_num_input.text = "0";
			}
		}
		else 
		{
			compose_num_input.text = "0";
		}
	}
	//请求合成 
	void ReqCompose()
	{
		if(U3dCmn.IsNumeric(compose_num_input.text))
		{
			int num = int.Parse(compose_num_input.text);
			if(num >0)
			{
				CTS_GAMECMD_COMPOS_ITEM_T req;
				req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
				req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_COMPOS_ITEM;
				req.nExcelID3 = excel_id+10;
				req.nNum4 = num;
				TcpMsger.SendLogicData<CTS_GAMECMD_COMPOS_ITEM_T >(req);
				LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_COMPOS_ITEM);
			}
		}
	}
	//请求合成返回  
	void ReqComposeRst(byte[] buff)
	{ 
		STC_GAMECMD_COMPOS_ITEM_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_COMPOS_ITEM_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_COMPOS_ITEM_T.enum_rst.RST_OK)
		{
			CangKuManager.SubItem(compose_gem_id,compose_num*4);
			CangKuManager.AddItem(sub_msg.nItemID2,excel_id+10,compose_num);
			GameObject.Find("CangKuWin").SendMessage("RefreshPanelData");
		}
		else 
		{
			U3dCmn.ShowWarnWindow("unknown err");
		}
		DismissPanel();
	}
}
