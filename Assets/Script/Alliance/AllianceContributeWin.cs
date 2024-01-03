using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AllianceContributeWin : MonoBehaviour {
	public TweenPosition tween_position;
	public UILabel own_gold;
	public UILabel describe;
	public UISlider num_slider;
	public UIInput  num_input;
	uint 	max_gold_num;
	string describe_str; 
	void Awake()
	{
		describe_str = describe.text;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame 
	void Update () {
		
	}
	void RevealPanel()
	{
		max_gold_num = CommonData.player_online_info.Gold;
		num_input.text = "0";
		num_slider.sliderValue = 0;
		own_gold.text = max_gold_num.ToString();
		describe.text = string.Format(describe_str,CommonMB.AllianceContributeRate);
		tween_position.Play(true);
	}
	//关闭窗口  
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	
	//滑动条出发事件 调整训练数量 
	void OnSliderChange()
	{
		int num = (int)(num_slider.sliderValue*(float)max_gold_num);
		num = num/CommonMB.AllianceContributeRate*CommonMB.AllianceContributeRate;
		num_input.text = num.ToString();
	}
	//输出框内容改变  
	void InPutChange()
	{
		if(!U3dCmn.IsNumeric(num_input.text) || max_gold_num ==0)
		{
			num_input.text = "0";
			num_slider.sliderValue = 0;
		}
		else
		{
			uint num = uint.Parse(num_input.text);
			if(num>max_gold_num)
			{
				num = max_gold_num;
				
			}
			num = num/(uint)CommonMB.AllianceContributeRate*(uint)CommonMB.AllianceContributeRate;
			float slider_num = (float)num/(float)max_gold_num;
			num_slider.sliderValue = slider_num;
			num_input.text = num.ToString();
		}
		
	}
	//请求捐献金币  
	void ReqContributeGold()
	{
		CTS_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE;
		req.nGold3 = uint.Parse(num_input.text);
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T >(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE);
	}
	//请求捐献金币返回 
	void ReqContributeGoldRst(byte[] buff)
	{
		STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CONTRIBUTE_ALLIANCE_T.enum_rst.RST_OK)
		{
			AllianceManager.my_alliance_info.NowContribute += sub_msg.nDevelopment3;
			CommonData.player_online_info.Gold-=sub_msg.nGold2;
			max_gold_num = CommonData.player_online_info.Gold;
			own_gold.text = CommonData.player_online_info.Gold.ToString();
			PlayerInfoManager.RefreshPlayerDataUI();
			U3dCmn.SendMessage("AllianceWin","InitialContributeUI",null);
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CONTRIBUTE_OK));
			
			// 贡献度重新刷新 .....
			AllianceManager.ReqAllianceInfo();
		}
		num_input.text = "0";
		num_slider.sliderValue = 0;
		
	}
}
