using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class ResearchSoldierWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UISprite soldier_icon;
	public UILabel  soldier_name;
	public UILabel	title;
	public UILabel	pre_need;
	public UILabel  need_money;
	public UILabel 	need_time;
	public UIImageButton research_btn;
	string title_str;
	string pre_need_str;
	uint excel_id;
	
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
		title_str = title.text;
		pre_need_str = pre_need.text;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	void RevealPanel(SoldierUnit soldier_unit)
	{
		bool enable_btn = true;
		excel_id = soldier_unit.nExcelID1;
		SoldierMBInfo soldier_info = U3dCmn.GetSoldierInfoFromMb(soldier_unit.nExcelID1,soldier_unit.nLevel2);
		if(!CommonMB.Technology_Map.Contains(soldier_unit.nLevel2+1))
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.TECH_IS_TOP));
			return;
		}
		soldier_icon.spriteName = U3dCmn.GetIconFromSoldierID(soldier_unit.nExcelID1);
		TechnologyInfo tech_info = (TechnologyInfo)CommonMB.Technology_Map[soldier_unit.nLevel2+1];
		soldier_name.text = soldier_info.Name+"Lv"+soldier_unit.nLevel2;
		title.text = string.Format(title_str,soldier_unit.nLevel2+1,soldier_info.Name);
		pre_need.text = string.Format(pre_need_str,tech_info.InsituteLevel);
		if(tech_info.InsituteLevel>Institute.NowLevel)
		{
			enable_btn = false;
			pre_need.color = new Color(1,0,0,1);
		}
		else 
		{
			pre_need.color = new Color(0.38f,0,0,1);
		}
		need_money.text = tech_info.Money+"/"+CommonData.player_online_info.Gold;
		if(tech_info.Money>CommonData.player_online_info.Gold)
		{
			enable_btn = false;
			need_money.color = new Color(1,0,0,1);
		}
		else
		{
			need_money.color = new Color(0.38f,0,0,1);
		}
		need_time.text = U3dCmn.GetTimeStrFromSec((int)(tech_info.ResearchTime*3600));
		if(!enable_btn)
		{
			research_btn.target.spriteName = "button1_disable"; 
			research_btn.gameObject.GetComponent<Collider>().enabled = false;
		}
		else 
		{
			research_btn.target.spriteName = "button1"; 
			research_btn.gameObject.GetComponent<Collider>().enabled = true;
		}
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//研究科技 
	void ResearchTech()
	{
		TechnologyManager.ReqResearchTech(excel_id);
	}
}
