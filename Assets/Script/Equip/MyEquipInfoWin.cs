using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CMNCMD;
public class MyEquipInfoWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public ulong my_equip_id;
	public bool only_show =true;
	public UISprite my_equip_icon;
	public UILabel  my_equip_name;
	public UILabel my_equip_describe;
	public UILabel my_equip_property;
	public UILabel gem_num_label;
	public UILabel gem_info;
	public UIImageButton dress_btn;
	string gem_num_str;
	
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
		gem_num_str = gem_num_label.text;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	//弹出窗口   
	void RevealPanel()
	{
		if(only_show)
			dress_btn.gameObject.SetActiveRecursively(false);
		else
			dress_btn.gameObject.SetActiveRecursively(true);
		
		InitialData();
		tween_position.Play(true);
	}
	//关闭窗口   
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void InitialData()
	{
		ItemDesc item_info;
		EquipInfo equip_info;
		if(only_show)
		{
			item_info = (ItemDesc)EquipManager.EquipItemMap[my_equip_id];
			equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		}
		else
		{
			item_info = (ItemDesc)CangKuManager.CangKuItemMap[my_equip_id];
			equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		}
		
	
		my_equip_icon.spriteName = equip_info.IconName;
		my_equip_name.text = equip_info.EquipName;
		my_equip_describe.text = equip_info.EquipDescribe;
		my_equip_property.text = U3dCmn.GetTipByExcelID((uint)item_info.nExcelID2);
		List<uint>gem_list = EquipManager.GetEmbedGem(my_equip_id);
		gem_num_label.text = gem_num_str+" "+gem_list.Count+"/4";
		for(int i=0;i<gem_list.Count;i++)
		{
			gem_info.text +=U3dCmn.GetTipByExcelID(gem_list[i])+"\n";
		}
		
		//gem_info.text = 
		
	}
	
	
	void ChangeEquip()
	{
		// DismissPanel();
		GameObject.Find("HeroEquipWin").SendMessage("ReqChangeEquip",my_equip_id);
	}
}
