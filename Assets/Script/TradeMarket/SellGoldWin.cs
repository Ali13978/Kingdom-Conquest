using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class SellGoldWin : MonoBehaviour {
	public TweenPosition tween_position;
	public UILabel own_gold;
	public UILabel sell_max;
	public UILabel sell_min;
	public UISlider num_slider;
	public UIInput  num_input;
	public UIInput  price_input;
	public UILabel  desc_label;
	uint 	max_gold_num;
	int 	min_gold_num;
	int    lord_lv_min;
	string describe_str;
	void Awake()
	{
		describe_str = desc_label.text;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame 
	void Update () {
		
	}
	void RevealPanel()
	{
		
		uint min_sell = 23;
		min_gold_num = U3dCmn.GetCmnDefNum(min_sell);
	
		uint lord_min_lv = 22;
		lord_lv_min = U3dCmn.GetCmnDefNum(lord_min_lv);
		
		num_input.text = "0";
		num_slider.sliderValue = 0;
		price_input.text = "10";
		own_gold.text = CommonData.player_online_info.Gold.ToString();
		
		uint max_by_level = (uint)U3dCmn.GetGoldSellNumByCharLevel(CommonData.player_online_info.Level);
		sell_min.text = U3dCmn.GetThousandNumStr((uint)min_gold_num);
		max_gold_num = CommonData.player_online_info.Gold>max_by_level?max_by_level:CommonData.player_online_info.Gold;
		sell_max.text = U3dCmn.GetThousandNumStr(max_gold_num);
		
		GoldMarketLevel unit = U3dCmn.GetNextLevelGoldSell(CommonData.player_online_info.Level);
		if(unit.LordLevel ==0)
		{
			desc_label.text = string.Format(describe_str,CommonData.player_online_info.Level,U3dCmn.GetThousandNumStr(max_by_level));
		}
		else
		{
			desc_label.text = string.Format(describe_str,unit.LordLevel,U3dCmn.GetThousandNumStr((uint)unit.GoldSellMax));
		}
	
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
		
		num = num/min_gold_num*min_gold_num;
	
		num_input.text = num.ToString();
	}
	//SELL数量输出框内容改变  
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
			num = num/(uint)min_gold_num*(uint)min_gold_num;
			float slider_num = (float)num/(float)max_gold_num;
			num_slider.sliderValue = slider_num;
			num_input.text = num.ToString();
		}
		
	}
	//单价输入框改变 
	void PriceInputChange()
	{
		if(!U3dCmn.IsNumeric(price_input.text))
		{
			price_input.text = "5";
		}
		else
		{
			uint num = uint.Parse(price_input.text);
			if(num>10)
			{
				num = 10;
				
			}
			if(num<5)
			{
				num = 5;
			}
			price_input.text = num.ToString();
		}
	}
	//请求寄卖金币 
	void ReqSellGold()
	{
		if(CommonData.player_online_info.Level < lord_lv_min)
		{
			string tip = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LORD_LV_LOW_FOR_SALE),lord_lv_min);
			U3dCmn.ShowWarnWindow(tip);
			return;
		}
		
		uint price = uint.Parse(U3dCmn.CheckNullStr(price_input.text));
		uint sell_num = uint.Parse(num_input.text);
		if(price <5 || price>10 )
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD_PRICE_NUM_LIMIT));
			return;
		}
		if(sell_num <= 0)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD_SELL_NUM_LOWER));
			return;
		}
		TradeMarketManager.instance.ReqSellGold(sell_num,price);
	}
}
