using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class SoldierTrainWin : MonoBehaviour {
	SoldierUnit soldier_unit;
	public TweenPosition tween_position;
	public UILabel gold;
	public UILabel people;
	public UILabel all_sodiernum;
	public UILabel	describe;
	public UISlider num_slider;
	public UIInput train_num;
	
	int gold_per_soldier;
	int soldier_capacity;
	int soldier_num;
	int max_soldier_num;
	// Use this for initialization
	void Awake () {
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
	}
	

	//弹出窗口 
	void RevealPanel(SoldierUnit unit)
	{
		soldier_unit = unit;
		if(unit.nExcelID1 == (int)SoldierSort.QIBING)
		{
			describe.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.QIBING_DESCRIBE);
		}
		else if(unit.nExcelID1 == (int)SoldierSort.QIANGBING)
		{
			describe.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.QIANGBING_DESCRIBE);
		}
		else if(unit.nExcelID1 == (int)SoldierSort.JIANSHI)
		{
			describe.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.JIANSHI_DESCRIBE);
		}
		else if(unit.nExcelID1 == (int)SoldierSort.GONGBING)
		{
			describe.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.KONGBING_DESCRIBE);
		}
		else if(unit.nExcelID1 == (int)SoldierSort.QIXIE)
		{
			describe.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.QIXIE_DESCRIBE);
		}
		//兵营最大容量 
		soldier_capacity = SoldierManager.GetBingYingCapacity();
		//当前士兵总数 
		soldier_num =SoldierManager.GetSoldierNum();
		all_sodiernum.text = soldier_num+"/"+soldier_capacity;
		SoldierMBInfo soldier_mb_info = U3dCmn.GetSoldierInfoFromMb(unit.nExcelID1,unit.nLevel2);
		gold_per_soldier = soldier_mb_info.Money;
		gold.text = 0+"/"+CommonData.player_online_info.Gold;
		
		//居民
		people.text = 0+"/"+CommonData.player_online_info.Population;
		//计算最大训练数量 
		int max_num_by_money = (int)CommonData.player_online_info.Gold/gold_per_soldier;
		
		int max_num_by_people = (int)CommonData.player_online_info.Population;
		max_soldier_num = max_num_by_money>max_num_by_people?max_num_by_people:max_num_by_money;
		
		int capacity = soldier_capacity - soldier_num;
		max_soldier_num = max_soldier_num>capacity?capacity:max_soldier_num;
		max_soldier_num = max_soldier_num;// - soldier_num;
		
		
		if(max_soldier_num<0)
			 max_soldier_num = 0;
		num_slider.sliderValue = 0;
		tween_position.Play(true);
	}
	//滑动条出发事件 调整训练数量 
	void OnSliderChange()
	{
		int num = (int)(num_slider.sliderValue*(float)max_soldier_num);
		all_sodiernum.text = (soldier_num + num)+"/"+soldier_capacity;
		int money = gold_per_soldier*num;
		gold.text = money+"/"+CommonData.player_online_info.Gold;
		people.text =num+"/"+CommonData.player_online_info.Population;
		train_num.text = num.ToString();
	}
	//发送训练士兵指令 
	void ReqTrainSoldier()
	{
		if(!U3dCmn.IsNumeric(train_num.text) || uint.Parse(train_num.text)<=0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.INPUT_IS_WRONG));
			return ;
		}
		CTS_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CONSCRIPT_SOLDIER;
		req.nExcelID3 = soldier_unit.nExcelID1;
		req.nLevel4 = soldier_unit.nLevel2;
		req.nNum5  = uint.Parse(train_num.text);
		
		//print ("upgrade bingying"+req.nAutoID3 ); 
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CONSCRIPT_SOLDIER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONSCRIPT_SOLDIER);
	}
	//输出框内容改变 
	void InPutChange()
	{
		
		if(!U3dCmn.IsNumeric(train_num.text) || max_soldier_num ==0)
		{
			train_num.text = "0";
			num_slider.sliderValue = 0;
		}
		else
		{
			int num = int.Parse(train_num.text);
			if(num>max_soldier_num)
			{
				num = max_soldier_num;
				
			}
			train_num.text = num.ToString();
			float slider_num = (float)num/(float)max_soldier_num;
			num_slider.sliderValue = slider_num;
			
		}
		
	}
	// (新手引导初始值) 10 ...
	void TabInputChange()
	{
		train_num.text = "10";
		InPutChange();
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	
}
