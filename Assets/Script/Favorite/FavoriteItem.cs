using UnityEngine;
using System.Collections;
using CMNCMD;
using STCCMD;
using CTSCMD;
public class FavoriteItem : MonoBehaviour {
	public UILabel  char_name;
	public UISprite head_icon;
	public UILabel  pos_label;
	public UISprite pos_bg;
	public UIImageButton delete_btn;
	
	PositionMark now_unit;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void InitialData(PositionMark unit)
	{
		now_unit = unit;
		if(unit.szName5 != null)
		{
			head_icon.spriteName = U3dCmn.GetCharIconName((int)unit.nHeadID3);
			char_name.text = DataConvert.BytesToStr(unit.szName5);
			pos_label.text = unit.nPosX1 + ","+unit.nPosY2;
			pos_bg.gameObject.SetActiveRecursively(true);
			delete_btn.gameObject.SetActiveRecursively(true);
		}
		else 
		{
			head_icon.spriteName = "transparent";
			char_name.text = "";
			pos_label.text = "";
			pos_bg.gameObject.SetActiveRecursively(false);
			delete_btn.gameObject.SetActiveRecursively(false);
		}
	}
	//删除数据 
	void DeleteFavoriteData()
	{
		FavoriteManager.Instance.DeleteFavoriteData(now_unit);
	}
	//定位 
	void Locate()
	{
		SceneManager.OpenGlobalMapLocation((int)now_unit.nPosX1,(int)now_unit.nPosY2);
		U3dCmn.SendMessage("FavoriteWin","DismissPanel",null);
	}
	
}
