using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class UpdateSoldierWin : MonoBehaviour {
	SoldierUnit soldier_advance_unit;
	SoldierUnit soldier_now_unit;
	public TweenPosition tween_position;
	public UILabel gold;
	public UILabel sodiernum;
	public UILabel describe;
	public UISlider num_slider;
	public UIInput update_num_input;
	
	int gold_per_soldier;
	
	int update_soldier_num;
	int now_soldier_num;
	int max_soldier_num;
	string describe_str;
	
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
		describe_str = describe.text;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//弹出窗口 
	void RevealPanel(SoldierUnit unit)
	{
	 
		soldier_now_unit = (SoldierUnit)SoldierManager.SoldierMap[unit.nExcelID1+"_"+unit.nLevel2];
		soldier_advance_unit = (SoldierUnit)SoldierManager.GetTopSoldierInfo(unit.nExcelID1);
		now_soldier_num = unit.nNum3;
		sodiernum.text = update_soldier_num+"/"+unit.nNum3;
	
		SoldierMBInfo soldier_now_info = U3dCmn.GetSoldierInfoFromMb(unit.nExcelID1,unit.nLevel2);
		SoldierMBInfo soldier_advance_info = U3dCmn.GetSoldierInfoFromMb(soldier_advance_unit.nExcelID1,soldier_advance_unit.nLevel2);
		describe.text = string.Format(describe_str,soldier_advance_unit.nLevel2,soldier_advance_info.Name);
		gold_per_soldier = soldier_advance_info.Money - soldier_now_info.Money;
		gold.text = 0+"/"+CommonData.player_online_info.Gold;
		
		
		//计算最大训练数量 
		if(gold_per_soldier <=0)
			return;
		int max_num_by_money = (int)CommonData.player_online_info.Gold/gold_per_soldier;
		
		int max_num_by_soldier = unit.nNum3;
		max_soldier_num = max_num_by_money>max_num_by_soldier?max_num_by_soldier:max_num_by_money;
		
		
		
		if(max_soldier_num<0)
			 max_soldier_num = 0;
		num_slider.sliderValue = 0;
		tween_position.Play(true);
	}
	//滑动条出发事件 调整训练数量 
	void OnSliderChange()
	{
		int num = (int)(num_slider.sliderValue*(float)max_soldier_num);
		sodiernum.text = num+"/"+now_soldier_num;
		int money = gold_per_soldier*num;
		gold.text = money+"/"+CommonData.player_online_info.Gold;
		update_num_input.text = num.ToString();
	}
	//发送升级士兵指令 
	void ReqUpdateSoldier()
	{
		if(!U3dCmn.IsNumeric(update_num_input.text) || uint.Parse(update_num_input.text)<=0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.INPUT_IS_WRONG));
			return ;
		}
		CTS_GAMECMD_OPERATE_UPGRADE_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_UPGRADE_SOLDIER;
		req.nExcelID3 = soldier_advance_unit.nExcelID1;
		req.nFromLevel4 = soldier_now_unit.nLevel2;
		req.nToLevel5 = soldier_advance_unit.nLevel2;
		req.nNum6  = uint.Parse(update_num_input.text);
		
		//print ("upgrade bingying"+req.nAutoID3 ); 
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_UPGRADE_SOLDIER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_UPGRADE_SOLDIER);
	}
	//输出框内容改变 
	void InPutChange()
	{
		
		if(!U3dCmn.IsNumeric(update_num_input.text) || max_soldier_num ==0)
		{
			update_num_input.text = "0";
			num_slider.sliderValue = 0;
		}
		else
		{
			int num = int.Parse(update_num_input.text);
			if(num>max_soldier_num)
			{
				num = max_soldier_num;
				
			}
			float slider_num = (float)num/(float)max_soldier_num;
			num_slider.sliderValue = slider_num;
			update_num_input.text = num.ToString();
		}
		
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	
}
