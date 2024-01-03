using UnityEngine;
using System.Collections;
using CMNCMD;
public class EnemyTroopItem : MonoBehaviour {
	public UISprite icon;
	public UISprite bg;
	public UILabel force_label;
	public UILabel account_name;
	public TroopData item_data = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//初始化数据 
	public void InitialData()
	{
		if(item_data != null)
		{
			if(item_data.TroopId!=0)
			{
				icon.spriteName = U3dCmn.GetTroopBigIconFromID(item_data.TroopId);
				force_label.text = item_data.Force.ToString();
			}
			else 
			{
				icon.spriteName = "transparent";
				force_label.text = "";
				account_name.text ="";
			}
		}
		else
		{
			icon.spriteName = "transparent";
			force_label.text = "";
			account_name.text = "";
		}
	}
}