using UnityEngine;
using System.Collections;
using CMNCMD;
using STCCMD;
using CTSCMD;
public class SellGoldItem : MonoBehaviour {
	public UILabel	gold_label;
	public UILabel 	unit_price_label;
	public UILabel  total_price_name;
	public GameObject gold_icon;
	public GameObject unit_price_icon;
	public GameObject total_price_icon;
	public GameObject buy_btn;
	public GameObject revoke_btn;

	
	GoldMarketDeal_T now_unit;
	void Awake()
	{
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void InitialData(GoldMarketDeal_T unit)
	{
		now_unit = unit;
		if(unit.nTransactionID1 !=0)
		{
			if(unit.nAccountID5 != CommonData.player_online_info.AccountID)
			{
				gold_label.text = unit.nGold2.ToString();
				unit_price_label.text = unit.nPrice3.ToString();
				total_price_name.text = unit.nTotalPrice4.ToString();
				buy_btn.SetActiveRecursively(true);
				revoke_btn.SetActiveRecursively(false);
			}
			else 
			{
				gold_label.text = unit.nGold2.ToString();
				unit_price_label.text = unit.nPrice3.ToString();
				total_price_name.text = unit.nTotalPrice4.ToString();
				buy_btn.SetActiveRecursively(false);
				revoke_btn.SetActiveRecursively(true);
			}
		}
		else
		{
			gold_label.text = "";
			unit_price_label.text = "";
			total_price_name.text = "";
			buy_btn.SetActiveRecursively(false);
			gold_icon.SetActiveRecursively(false);
			unit_price_icon.SetActiveRecursively(false);
			total_price_icon.SetActiveRecursively(false);
			revoke_btn.SetActiveRecursively(false);
			
		}
		
	}
	//购买黄金 
	void OpenBuyGoldWin()
	{
		int limit_level = U3dCmn.GetCmnDefNum((int)CMN_DEF.BUY_GOLD_LEVEL_LIMIT);
		if(limit_level > CommonData.player_online_info.Level)
		{
			string tip = string.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LORD_LEVEL_LIMIT_BUY_GOLD),limit_level);
			U3dCmn.ShowWarnWindow(tip);
			return;
		}
			
		GameObject obj  = U3dCmn.GetObjFromPrefab("TradeMarketBuyGoldWin");
		if (obj != null)
		{
			obj.GetComponent<TradeMarketBuyGoldWin>().RevealPanel(now_unit.nTotalPrice4,now_unit.nPrice3,now_unit.nTransactionID1);
		}
		//TradeMarketManager.instance.ReqBuyGoldSale(now_unit.nTransactionID1,);
	}
	//撤销交易 
	void RevokeTrade()
	{
		TradeMarketManager.instance.ReqRevokeGoldSale(now_unit.nTransactionID1);
	}
}	
