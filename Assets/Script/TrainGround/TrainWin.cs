using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class TrainWin : MonoBehaviour {
	public static int max_hour = 10;
	public TweenPosition 	tween_position;
	public UIImageButton	train_btn;
	public UISprite 		head_icon;
	public UISprite 		add_hour;
	public UISprite 		sub_hour;
	public UILabel			hero_name_label;
	public UILabel			need_money_label;
	public UILabel			exp_label;
	public UILabel			level_warning_label;
	public UILabel			money_warning_label;
	public UIInput			train_hour_input;
	string level_warning_str;
	string money_warning_str;
	int train_hour = 0;
	int money_per_hour = 0;
	int exp_per_hour = 0;
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
		level_warning_str = level_warning_label.text;
		money_warning_str = money_warning_label.text;
	}

	// Use this for initialization
	void Start () {
		UIEventListener.Get(add_hour.gameObject).onPress -= ChangeHour;
		UIEventListener.Get(add_hour.gameObject).onPress += ChangeHour;
		UIEventListener.Get(sub_hour.gameObject).onPress -= ChangeHour;
		UIEventListener.Get(sub_hour.gameObject).onPress += ChangeHour;
	}
	
	//弹出窗口   
	void RevealPanel()
	{
		tween_position.Play(true);
		train_hour = 1;
		train_hour_input.text = "1";
		level_warning_label.text = "";
		money_warning_label.text = "";
		need_money_label.text = "";
		exp_label.text = "";
		TrainingGround train_ground = (TrainingGround)CommonMB.TrainGround_Map[TrainGroundWin.now_unit.level];
		money_per_hour = 0;
		exp_per_hour = 0;
		//显示第一个将领的信息 
		SetHeroIcon(TrainGroundManager.SelectHeroList[0]);
		for(int i=0; i<TrainGroundManager.SelectHeroList.Count;i++)
		{
			ulong hero_id = TrainGroundManager.SelectHeroList[i];
			HireHero hero_info = (HireHero)JiangLingManager.MyHeroMap[hero_id];
			if(train_ground.HeroTopLevel< hero_info.nLevel19)
			{
				//训练馆等级不够满足将领训练 
				SetHeroIcon(hero_id);
				level_warning_label.text = string.Format(level_warning_str,DataConvert.BytesToStr(hero_info.szName3));
				train_btn.gameObject.GetComponent<Collider>().enabled = false;
				train_btn.target.spriteName = "button1_disable"; 
				return;
			}
			
			HeroExpGoldInfo exp_gold_info =  (HeroExpGoldInfo)CommonMB.HeroExpGold_Map[hero_info.nLevel19];
			exp_per_hour += exp_gold_info.TrainExpPerHour;
			money_per_hour += exp_gold_info.TrainMoneyPerHour *train_hour;
		}
		if(money_per_hour>CommonData.player_online_info.Gold)
		{
			//金钱不足 
			money_warning_label.text = string.Format(money_warning_str,money_per_hour); 
			need_money_label.text = money_per_hour.ToString();
			train_btn.gameObject.GetComponent<Collider>().enabled = false;
			train_btn.target.spriteName = "button1_disable"; 
			return ;
		}
		
		
		need_money_label.text = money_per_hour.ToString();
		exp_label.text = exp_per_hour.ToString();
		train_btn.gameObject.GetComponent<Collider>().enabled = true;
		train_btn.target.spriteName = "button1";
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//显示相应的将领信息 
	void SetHeroIcon(ulong hero_id)
	{ 
		HireHero hero_info = (HireHero)JiangLingManager.MyHeroMap[hero_id];
		string hero_name = DataConvert.BytesToStr(hero_info.szName3);
		head_icon.spriteName = U3dCmn.GetHeroIconName(hero_info.nModel12);
		hero_name_label.text = hero_name+" Lv"+hero_info.nLevel19;
	}

	//更改数量 
	void ChangeHour(GameObject obj,bool ispressed)
	{
		int num =int.Parse(CommonMB.CheckNullStr(train_hour_input.text));
		if(ispressed)
		{
			if(obj.name == "AddHour")
				StartCoroutine("ChangeNumCoroutine",true);
			else if(obj.name == "SubHour")
				StartCoroutine("ChangeNumCoroutine",false);
		}
		else
		{
			StopCoroutine("ChangeNumCoroutine");
		}
	}
	//改变修炼的小时数 
	IEnumerator ChangeNumCoroutine(bool isadd)
	{
		int num =int.Parse(CommonMB.CheckNullStr(train_hour_input.text));
		while(true)
		{
			if(isadd)
			{
				if(money_per_hour*(num+1)<=CommonData.player_online_info.Gold && (num+1)<=max_hour)
				{
					num++;
				}
				train_hour_input.text = num.ToString();
			}
			else
				train_hour_input.text = num>1?(--num).ToString():"1";
			ChangeMoney();
			yield return new WaitForSeconds(0.1f);
		}
	}
	//输入修炼时间  
	void HourInput()
	{
		if(U3dCmn.IsNumeric(train_hour_input.text))
		{
			int num =int.Parse(CommonMB.CheckNullStr(train_hour_input.text));
			if(money_per_hour*num>CommonData.player_online_info.Gold || num>max_hour)
			{
				int max_by_money = (int)CommonData.player_online_info.Gold/money_per_hour;
				num = max_by_money>max_hour?max_hour:max_by_money;
				train_hour_input.text = num.ToString();
			}
		}
		else
		{
			train_hour_input.text = "1";
		}
		ChangeMoney();
	}
	
	//根据小时更改总价和经验   
	void ChangeMoney()
	{
		int num =int.Parse(CommonMB.CheckNullStr(train_hour_input.text));
		need_money_label.text = (money_per_hour*num).ToString();
		exp_label.text = (exp_per_hour*num).ToString();
	}
	//请求修炼  
	void TrainHero()
	{
		TrainGroundManager.ReqTrainHero(uint.Parse(CommonMB.CheckNullStr(train_hour_input.text)));
	}
}
