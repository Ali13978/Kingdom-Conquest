using UnityEngine;
using System.Collections;

public class GoldItem : MonoBehaviour {
	
	public UILabel 	name;
	public UILabel 	price;
	public UISlicedSprite select_bg;
	string name_str;
	
	void Awake()
	{
		name_str = name.text;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void InitialData()
	{
		name.text = CommonMB.DiamondToGold_Info.GoldNum+name_str;
		SetSelect(false);
		
	}
	void ShowDetail()
	{
		SetSelect(true);
		U3dCmn.SendMessage("StoreWin","ShowGoldDetail",this);
	}
	public void SetSelect(bool isselect)
	{
		if(isselect)
			select_bg.gameObject.SetActiveRecursively(true);
		else 
			select_bg.gameObject.SetActiveRecursively(false);
	}
}
