using UnityEngine;
using System.Collections;
using CMNCMD;
using STCCMD;
public class SupplyDrugWin : MonoBehaviour {
	public TweenPosition tween_position; 
	//药水包数量 
	public UILabel 	drug_num_label;
	//增加数量按钮 
	public UISprite add_num;
	//减少数量按钮  
	public UISprite sub_num;
	//使用数量  
	public UIInput  use_num;
	//使用按钮 
	public UIImageButton use_btn;
	 //ulong 	now_hero_id;
	int drug_num =0;
	
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
		
		UIEventListener.Get(add_num.gameObject).onPress -= ChangeNum;
		UIEventListener.Get(add_num.gameObject).onPress += ChangeNum;
		UIEventListener.Get(sub_num.gameObject).onPress -= ChangeNum;
		UIEventListener.Get(sub_num.gameObject).onPress += ChangeNum;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void RevealPanel()
	{
		
		drug_num = CangKuManager.GetItemInfoByExcelID((int)ARTICLES.CAOYAOBAO).nNum3;
		if(drug_num>0)
		{
			use_num.text = "1";
			use_btn.GetComponent<Collider>().enabled = true;
			use_btn.target.spriteName = "button1";
			//use_btn.collider
		}
		else 
		{
			use_num.text = "0";
			use_btn.GetComponent<Collider>().enabled = false;
			use_btn.target.spriteName = "button1_disable";
			
		}
		drug_num_label.text = drug_num.ToString();
		tween_position.Play(true);
	
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
	}
	//更改数量 
	void ChangeNum(GameObject obj,bool ispressed)
	{
		int num =int.Parse(CommonMB.CheckNullStr(use_num.text));
		if(ispressed)
		{
			if(obj.name == "AddNum")
				StartCoroutine("ChangeNumCoroutine",true);
			else if(obj.name == "SubNum")
				StartCoroutine("ChangeNumCoroutine",false);
		}
		else
		{
			StopCoroutine("ChangeNumCoroutine");
		}
	}
	//输入改变数量 
	void InputChangeNum()
	{
		if(!U3dCmn.IsNumeric(use_num.text))
		{
			use_num.text = "1";
		}
		else 
		{
			int num =int.Parse(CommonMB.CheckNullStr(use_num.text));
			if(num > drug_num)
				use_num.text = drug_num.ToString();
		}
	}
	//改变购买数量 
	IEnumerator ChangeNumCoroutine(bool isadd)
	{
		int num =int.Parse(CommonMB.CheckNullStr(use_num.text));
		while(true)
		{
			if(isadd)
			{
				use_num.text = num<drug_num?(++num).ToString():drug_num.ToString();
			}
			else
				use_num.text = num>1?(--num).ToString():"1";
		
			yield return new WaitForSeconds(0.2f);
		}
	}
	//使用药水包  
	void SupplyDrug()
	{
		if(U3dCmn.IsNumeric(use_num.text) && drug_num>0)
		{
			//print ("11111111111111");
			int num =int.Parse(CommonMB.CheckNullStr(use_num.text));
			ArticlesInfo articles_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)ARTICLES.CAOYAOBAO];
			if(num <= drug_num && articles_info.DescribeStr != null && articles_info.DescribeStr != "")
			{
				//print ("2222222222222222222");
				string[] str = articles_info.DescribeStr.Split('*');
				if(U3dCmn.IsNumeric(str[1]))
				{
					int caoyao_num = int.Parse(str[1])*num;
					if(TreatManager.GetHospitalCapacity()<(int)CommonData.player_online_info.CaoYao+caoyao_num)
					{
						string warn_str = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.YIGUAN_LACK_SPACE);
						PopConfirmWin.instance.ShowPopWin(warn_str,SureUseArticlesItem);
						
						return ;
					}
					else 
					{
						SureUseArticlesItem(gameObject);
					}
				}
			}
		}
		
	}
	public void SureUseArticlesItem(GameObject obj)
	{
		int num =int.Parse(CommonMB.CheckNullStr(use_num.text));
		CangKuManager.UseArticlesItem((int)ARTICLES.CAOYAOBAO,CangKuManager.GetItemInfoByExcelID((int)ARTICLES.CAOYAOBAO).nItemID1,0,num);
		DismissPanel();
	}
}
