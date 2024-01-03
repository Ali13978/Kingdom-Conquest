using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class DismissSoldierWin : MonoBehaviour {
	SoldierUnit soldier_now_unit;
	public TweenPosition tween_position;
	public UISlider num_slider;
	public UIInput dismiss_num_input;
	
	int max_soldier_num = 0;
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
		
		num_slider.sliderValue = 0;
		tween_position.Play(true);
	}
	//滑动条出发事件 调整训练数量 
	void OnSliderChange()
	{
		int num = (int)(num_slider.sliderValue*(float)soldier_now_unit.nNum3);
		dismiss_num_input.text = num.ToString();
	}
	//输出框内容改变 
	void InPutChange()
	{
		
		if(!U3dCmn.IsNumeric(dismiss_num_input.text) || soldier_now_unit.nNum3 ==0)
		{
			dismiss_num_input.text = "0";
			num_slider.sliderValue = 0;
		}
		else
		{
			int num = int.Parse(dismiss_num_input.text);
			if(num>soldier_now_unit.nNum3)
			{
				num = max_soldier_num;
				
			}
			float slider_num = (float)num/(float)soldier_now_unit.nNum3;
			num_slider.sliderValue = slider_num;
			dismiss_num_input.text = num.ToString();
		}
		
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//解散兵种 
	void DismissSoldier()
	{
		if(!U3dCmn.IsNumeric(dismiss_num_input.text) || uint.Parse(dismiss_num_input.text)<=0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.INPUT_IS_WRONG));
			return ;
		}
		string str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SURE_DISMISS_TROOPS);
		PopConfirmWin.instance.ShowPopWin(str,SrueDismissSoldier);
	}
	void SrueDismissSoldier(GameObject obj)
	{
		
		CTS_GAMECMD_OPERATE_DISMISS_SOLDIER_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_DISMISS_SOLDIER;
		req.nExcelID3 = soldier_now_unit.nExcelID1;
		req.nLevel4 = soldier_now_unit.nLevel2;
		req.nNum5 = uint.Parse(dismiss_num_input.text);;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_DISMISS_SOLDIER_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_DISMISS_SOLDIER);
	}
}
