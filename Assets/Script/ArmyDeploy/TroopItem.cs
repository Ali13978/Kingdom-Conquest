using UnityEngine;
using System.Collections;
using CMNCMD;
public class TroopItem : MonoBehaviour {
	public ArmyDeployWin deploy_win;
	public UISprite icon;
	public UISprite bg;
	public UILabel force_label;
	public UILabel account_name;
	public int pos_id = 0;
	public TroopData item_data = null;
	bool drag_item = false;
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
			if(item_data.HeroID!=0)
			{
				icon.spriteName = U3dCmn.GetTroopBigIconFromID(item_data.TroopId);
				force_label.text = item_data.Force.ToString();
				if(ArmyDeployManager.isIndividual)
				{
					account_name.text = "Lv"+item_data.TroopLevel+"."+item_data.TroopNum;
				}
				else
				{
					account_name.text = item_data.AccountName;
				}
				
				pos_id = item_data.PosID;
			}
			else 
			{
				icon.spriteName = "transparent";
				force_label.text = "";
				account_name.text ="";
				pos_id = item_data.PosID;
			}
		}
		else
		{
			icon.spriteName = "transparent";
			force_label.text = "";
			account_name.text = "";
		}
	}
	//按下 
	void OnPress(bool isDown)
	{
		if(isDown)
		{
			if(item_data != null && item_data.HeroID!=0)
				ArmyDeployManager.is_drag =true;
		}
	}
	//点击 
	void OnClick ()
	{
		if(item_data != null && item_data.HeroID!=0)
			ArmyDeployManager.is_drag =true;
		OnDrop(null);
	}
	//拖拽 
	void OnDrag (Vector2 delta)
	{
		if(deploy_win.drag_type == DRAG_TYPE.UNIT)
		{
			//拖拽单元 
			if(item_data != null && item_data.HeroID!=0)
			{
				if(deploy_win.drag_item == null)
				{
					//print("ddddddddddddddddddddddddddddddd");
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.Always;
					UpdateCursor();
					deploy_win.SetDragItem(this);
				}
			}
		}
		else if(deploy_win.drag_type == DRAG_TYPE.COLUMN)
		{
			//拖拽列 
			if(item_data != null)
			{
				if(deploy_win.drag_column == null)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.Always;
					UpdateCursor();
					deploy_win.SetDragColumn(this);
				}
			}
		}
	}
	
	//松开鼠标 
	void OnDrop (GameObject go)
	{
		if(deploy_win.drag_type == DRAG_TYPE.UNIT)
		{
			if(deploy_win.drag_item != null)
			{
				deploy_win.target_item = this;
				deploy_win.ExchangeItem();
				//drag_item = false;
				ClearCursor();
			}
		}
		else if(deploy_win.drag_type == DRAG_TYPE.COLUMN)
		{
			if(deploy_win.drag_column != null)
			{
				deploy_win.target_item = this;
				deploy_win.ExchangeColumn();
				ClearCursor();
			}
		}
		
	}
	//更新游标 
	void UpdateCursor ()
	{
		
		UICursor.Set(icon.atlas, icon.spriteName,icon.transform);
	
		
	}
	//清空游标 
	void ClearCursor()
	{
		UICursor.Set(null, null,null);
	}
}
