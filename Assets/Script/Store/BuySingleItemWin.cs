using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class BuySingleItemWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UICheckbox		diamond_check;
	public UICheckbox		crystal_check;
	public UILabel			diamond_num;
	public UILabel			crystal_num;
	public UILabel			price_label;
	string price_str;
	
	public delegate void ReqBuy(uint money_type,int price);
	public static ReqBuy ReqBuyDelegate;
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
		price_str = price_label.text;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//弹出窗口    
	public void RevealPanel(int price)
	{
		diamond_num.text = CommonData.player_online_info.Diamond.ToString();
		crystal_num.text = CommonData.player_online_info.Crystal.ToString();
		price_label.text = price_str+price;
		tween_position.Play(true);
	}
	//关闭窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//购买 
	void BuyItem()
	{
		uint type = 0;
		int price = int.Parse(U3dCmn.CheckNullStr(price_label.text.Split(':')[1]));
		if(diamond_check.isChecked)
		{
			type = (uint)money_type.money_type_diamond;
			if(price > CommonData.player_online_info.Diamond)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.DIAMOND_NOT_ENOUGH));
			}
		}
		else
		{
			type = (uint)money_type.money_type_crystal;
			if(price > CommonData.player_online_info.Crystal)
			{
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CRYSTAL_NOT_ENOUGH));
			}
		}
		if(ReqBuyDelegate!=null)
		{
			ReqBuyDelegate(type,price);
		}
	}
	
}
