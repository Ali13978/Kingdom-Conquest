using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class TradeMarketBuyGoldWin : MonoBehaviour {
  
	public TweenPosition 	tween_position;
	public UILabel		 	unit_price_label;
	public UILabel		 	total_price_label;
	public UILabel			DiamondNum;
	public UILabel 			desc_label;
	uint total_price;
	uint unit_price;
	uint moneytype;
	ulong transaction_id;
	public void Awake()
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
	
	//弹出窗口 
	public void RevealPanel(uint TotalPrice,uint UnitPrice,ulong TransactionId)
	{
		total_price = TotalPrice;
		unit_price = UnitPrice;
		transaction_id = TransactionId;
		total_price_label.text = total_price.ToString();
		unit_price_label.text = unit_price.ToString();
		DiamondNum.text = CommonData.player_online_info.Diamond.ToString();
		desc_label.text = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LORD_LEVEL_LIMIT_BUY_GOLD),U3dCmn.GetCmnDefNum((int)CMN_DEF.BUY_GOLD_LEVEL_LIMIT));
		tween_position.Play(true);
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//购买金币 
	void BuyGold()
	{
		//if(DiamandCheck.isChecked)
		//{
		//检查钻石是否够用  
		if(CommonData.player_online_info.Diamond<total_price)
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
			U3dCmn.ShowRechargePromptWindow();
			return ;
		}
		moneytype = (uint)money_type.money_type_diamond;
		//}
		/*else if(CrystalCheck.isChecked)
		{
			//检查水晶是否够用  
			if(CommonData.player_online_info.Crystal<total_price)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH));
				return ;
			}
			moneytype = (uint)money_type.money_type_crystal;
		}*/
		TradeMarketManager.instance.ReqBuyGoldSale(transaction_id,moneytype);
	}
}
