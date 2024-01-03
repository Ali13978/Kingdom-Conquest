using UnityEngine;
using System.Collections;
using System;
using System.Timers;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class ZhuanPanAlliance : MonoBehaviour {
const float	maxVelocity			= 720f;
	const float minVelocity			= 360f;
	const int	maxRotateStrength	= 4;	// 旋转圈数 
	const int	minRotateStrength	= 1;
	const float	maxPressTime		= 5;	// 按5s达到最大速度	
	const string defaultItemIcon	= "choujianglibao";
	
	int		rotateStrength	= 0;			// 旋转圈数 
	bool	start			= false;
	float	rotate			= 0;
	float	totalRotate		= 0;
	float	curRotate		= 0;
	float	accelerateVal	= 0;
	float	leftTime		= 0;
	float	totalTime		= 0;
	float	beginPressTime	= 0;
	float	velocity		= 0;
	bool	canPress		= false;
	
	Transform zhuanpan_body;
	
	int	choice				= 0;
	uint	gettedLotteryType		= (uint)lottery_type.none;
	uint	gettedLotteryExcelID	= 0;
	public TweenPosition 	tweenPosition;
	
	UISprite[]	jiangLiIconArr	= new UISprite[10];
	UILabel[]	jiangLiLabelArr	= new UILabel[10];
	
	public UIButton		btnClose;
	public UIButton		btnDrawLottery;
	public UIButton		btnStart;
	public GameObject	gettedItem;
	public UISprite		gettedItemIcon;
	public UILabel		gettedItemNum;
	public UILabel		contributeNum;
	public UILabel		lotteryNum;
	public UILabel		priceNum;
	
	void SetAllJiangLiIconsLabelsToDefault()
	{
		for (int i=0; i<10; i++)
		{
			jiangLiIconArr[i].spriteName	= defaultItemIcon;
			jiangLiLabelArr[i].text			= "";
		}
	}
	
	void RefreshContributeLabels()
	{
		contributeNum.text		= AllianceManager.my_alliance_info.NowContribute.ToString();
		lotteryNum.text			= (AllianceManager.my_alliance_info.NowContribute/(uint)CommonMB.Lottery_Info.AllianceLotteryPrice).ToString();
		priceNum.text			= CommonMB.Lottery_Info.AllianceLotteryPrice.ToString();
	}
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = new Vector3(0.0025f,0.0025f,1.0f);
		}	
	}
	//弹出窗口   
	void RevealPanel()
	{
		//ReqLotteryData();
		btnStart.gameObject.SetActiveRecursively(false);
		btnClose.gameObject.SetActiveRecursively(true);
		btnDrawLottery.gameObject.SetActiveRecursively(true);
		gettedItem.SetActiveRecursively(false);
		
		RefreshContributeLabels();
		
		tweenPosition.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		if (start)
		{
			return;
		}
		tweenPosition.Play(false);
		SetAllJiangLiIconsLabelsToDefault();
		Destroy(gameObject);
	}
	//请求抽奖数据 
	void ReqCheckLotteryData()
	{
		if (start)
		{
			return;
		}
		//print ("ReqCheckLotteryData,Alliance");
		CTS_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T req;
		req.nCmd1		= (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2	= (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY;
		req.nType3		= (uint)money_type.money_type_alliance_contribute;
		
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T >(req);	
		LoadingManager.instance.ShowLoading();
	}
	//请求抽奖数据返回 
	void ReqLotteryDataRst(byte[] buff)
	{
		LoadingManager.instance.HideLoading();
		STC_GAMECMD_OPERATE_DRAW_LOTTERY_T	sub_msg	= DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_DRAW_LOTTERY_T>(buff);
		if (sub_msg.nRst1 == (byte)STC_GAMECMD_OPERATE_DRAW_LOTTERY_T.enum_rst.RST_OK)
		{
			if(CommonData.player_online_info.AllianceLotteryNum>0)
			{
				CommonData.player_online_info.AllianceLotteryNum--;
				
			}	
			if (AllianceManager.my_alliance_info.NowContribute > (uint)CommonMB.Lottery_Info.AllianceLotteryPrice)
			{
				AllianceManager.my_alliance_info.NowContribute	-= (uint)CommonMB.Lottery_Info.AllianceLotteryPrice;
			}
			else
			{
				AllianceManager.my_alliance_info.NowContribute	= 0;
			}
			RefreshContributeLabels();
			U3dCmn.SendMessage("AllianceWin","InitialContributeUI",null);
			//print ("choice:" + sub_msg.nChoice4 + ",num:" + sub_msg.nNum5 + ",type:" + sub_msg.nType2 + ",price:" + sub_msg.nPrice3);
			
			choice		= (int)sub_msg.nChoice4;
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_OPERATE_DRAW_LOTTERY_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			LotteryEntry[] lottery_array = (LotteryEntry[])DataConvert.ByteToStructArray<LotteryEntry>(data_buff,(int)sub_msg.nNum5);
			gettedLotteryType		= (uint)lottery_array[choice].nType1;
			gettedLotteryExcelID	= lottery_array[choice].nData2;
			for (int i=0; i<sub_msg.nNum5; i++)
			{
				switch ((int)lottery_array[i].nType1)
				{
					case (int)lottery_type.item:
					{
						jiangLiIconArr[i].spriteName	= U3dCmn.GetItemIconByExcelID(lottery_array[i].nData2).IconName;
						jiangLiLabelArr[i].text			= "1";
						//print (i + ",type:item,excel_id:" + lottery_array[i].nData2 + ",icon:" + jiangLiIconArr[i].spriteName);
					}
					break;
					case (int)lottery_type.crystal:
					{
						jiangLiIconArr[i].spriteName	= "shuijing";
						jiangLiLabelArr[i].text			= lottery_array[i].nData2.ToString();
						//print (i + ",type:crystal,num:" + lottery_array[i].nData2 + ",icon:" + jiangLiIconArr[i].spriteName);
					}
					break;
					case (int)lottery_type.gold:
					{
						jiangLiIconArr[i].spriteName	= "jinbi";
						jiangLiLabelArr[i].text			= lottery_array[i].nData2.ToString();
						//print (i + ",type:gold,num:" + lottery_array[i].nData2 + ",icon:" + jiangLiIconArr[i].spriteName);
					}
					break;
					default:
					{
						//print ("error lottery type:" + lottery_array[i].nType1);
					}
					break;
				}
			}
			
			btnStart.gameObject.SetActiveRecursively(true);
			btnClose.gameObject.SetActiveRecursively(false);
			btnDrawLottery.gameObject.SetActiveRecursively(false);
			gettedItem.SetActiveRecursively(false);
			
			canPress		= true;
		}
	}
	
	//
	void ReqCheckLotteryDataRst(byte[] buff)
	{
		LoadingManager.instance.HideLoading();
		STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T	sub_msg	= DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T.enum_rst.RST_ALLIANCE_DEV_NOT_ENOUGH)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CONTRIBUTE_NOT_ENOUGH));
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_CHECK_DRAW_LOTTERY_T.enum_rst.RST_ALLIANCE_DRAW_LOTTERY_NUM_0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ALLIANCE_LOTTERY_ISFULL));
		}
		//print ("check_draw_lottery,rst:" + sub_msg.nRst1 + ",type:" + sub_msg.nType2 + ",price:" + sub_msg.nPrice3);
	}
	
	//请求获取抽奖结果  
	void ReqFetchLottery()
	{
		LoadingManager.instance.ShowLoading();
		//print ("fetch_lottery");
		CTS_GAMECMD_OPERATE_FETCH_LOTTERY_T	req;
		req.nCmd1		= (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2	= (uint)SUB_CTS_MSG.CTS_GAMECMD_OPERATE_FETCH_LOTTERY;
		req.nMoneyType3	= (uint)money_type.money_type_alliance_contribute;
		TcpMsger.SendLogicData<CTS_GAMECMD_OPERATE_FETCH_LOTTERY_T >(req);
	}
	
	//请求获取抽奖结果返回  
	void ReqFetchLotteryRst(byte[] buff)
	{
		LoadingManager.instance.HideLoading();
		STC_GAMECMD_OPERATE_FETCH_LOTTERY_T	sub_msg	= DataConvert.ByteToStruct<STC_GAMECMD_OPERATE_FETCH_LOTTERY_T>(buff);
		if (sub_msg.nRst1 == (int)STC_GAMECMD_OPERATE_FETCH_LOTTERY_T.enum_rst.RST_OK)
		{
			switch (sub_msg.nType2)
			{
				case (uint)lottery_type.item:
				{
					CangKuManager.AddItem(sub_msg.nItemID4, (int)sub_msg.nData3, 1);
				}
				break;
				case (uint)lottery_type.crystal:
				{
					CommonData.player_online_info.Crystal	+= sub_msg.nData3;
					// 刷新一次角色数据  
					PlayerInfoManager.RefreshPlayerDataUI();
				}
				break;
				case (uint)lottery_type.gold:
				{
					CommonData.player_online_info.Gold		+= sub_msg.nData3;
					// 刷新一次角色数据  
					PlayerInfoManager.RefreshPlayerDataUI();
				}
				break;
			}
		}
		//print ("fetch_lottery,rst:" + sub_msg.nRst1 + ",type:" + sub_msg.nType2 + ",data:" + sub_msg.nData3 + ",item_id:" + sub_msg.nItemID4);
		
		// 还原到初始状态  
		SetAllJiangLiIconsLabelsToDefault();
		btnStart.gameObject.SetActiveRecursively(false);
		btnClose.gameObject.SetActiveRecursively(true);
		btnDrawLottery.gameObject.SetActiveRecursively(true);
		gettedItem.SetActiveRecursively(false);
	}
	
	// Use this for initialization
	void Start () 
	{
		zhuanpan_body	= transform.Find("ZhuanPan_Body");
		for (int i=0; i<10; i++)
		{
			string childName	= "s" + i;
			Transform	sX		= zhuanpan_body.Find(childName);
			Transform	iconT	= sX.Find("jiangli_icon");
			Transform	labelT	= sX.Find("num");
			/*
			Transform	iconT	= zhuanpan_body.FindChild(childName);
			if (iconT != null)
			{
				print (childName);
				iconT			= iconT.FindChild("jiangli_icon");
				if (iconT != null)
				{
					print ("jiangli_icon");
				}
			}
			*/
			jiangLiIconArr[i]	= iconT.gameObject.GetComponent<UISprite>();
			jiangLiLabelArr[i]	= labelT.gameObject.GetComponent<UILabel>();
		}
		SetAllJiangLiIconsLabelsToDefault();
		//DismissPanel();
	}
		
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (start)
		{	
			// 旋转角度计算,平缓的速度 
			float	rotateFrame	= velocity*Time.deltaTime - 0.5f*accelerateVal*Time.deltaTime*(2.0f*(totalTime-leftTime)+Time.deltaTime);
			leftTime	-= Time.deltaTime;

			if (leftTime > 0)
			{
				curRotate				+= rotateFrame;
				zhuanpan_body.RotateAround(zhuanpan_body.position, Vector3.back, rotateFrame);
			}
			else
			{
				zhuanpan_body.RotateAround(zhuanpan_body.position, Vector3.back, rotate-curRotate);
				start 	= false;
				
				btnStart.gameObject.SetActiveRecursively(false);
				btnClose.gameObject.SetActiveRecursively(false);
				btnDrawLottery.gameObject.SetActiveRecursively(false);
				gettedItem.SetActiveRecursively(true);
					
				gettedItemIcon.spriteName	= jiangLiIconArr[choice].spriteName;
				switch ((uint)gettedLotteryType)
				{
					case (uint)lottery_type.item:
					{
						gettedItemNum.text			= U3dCmn.GetItemIconByExcelID(gettedLotteryExcelID).Name + " x" + jiangLiLabelArr[choice].text;
					}
					break;
					default:
					{
						gettedItemNum.text			= jiangLiLabelArr[choice].text;
					}
					break;
				}
			}
		}
	}
	
	void BeginPress()
	{
		if (start)
		{
			return;
		}
		beginPressTime		= Time.time;
	}
	
	void EndPress()
	{
		if (start)
		{
			return;
		}
		if (!canPress)
		{
			return;
		}
		float deltaTime		= (Time.time-beginPressTime)>maxPressTime?maxPressTime:(Time.time-beginPressTime);
		velocity			= minVelocity + (maxVelocity-minVelocity)*deltaTime/maxPressTime;
		rotateStrength		= minRotateStrength + (int)(((float)(maxRotateStrength-minRotateStrength))*deltaTime/maxPressTime);
		canPress			= false;
		StartRotate();
	}
	
	void StartRotate()
	{
		if (start)
		{
			return;
		}
		btnStart.gameObject.SetActiveRecursively(false);
		
		start			= true;
		
		curRotate		= 0;
		rotate			= (360.0f - choice*36.0f) + rotateStrength*360.0f - totalRotate;
		if (rotate < 360.0f)
		{
			rotate		+= 360.0f;
		}
		totalRotate		+= rotate;
		totalRotate		%= 360.0f;
		if (totalRotate < 0)
			totalRotate	+= 360f;
		
		totalTime		= 2*rotate/velocity;
		leftTime		= totalTime;
		
		accelerateVal	= velocity/totalTime;
	}
}
