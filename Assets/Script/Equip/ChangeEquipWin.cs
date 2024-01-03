using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CMNCMD;
public class ChangeEquipWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public ulong original_equip_id;
	public ulong change_equip_id;
	public UISprite now_equip_icon;
	public UILabel  now_equip_name;
	public UILabel now_equip_describe;
	public UILabel now_equip_property;
	public UILabel now_gem_num_label;
	public UILabel now_gem_info;
	public UISprite change_equip_icon;
	public UILabel  change_equip_name;
	public UILabel change_equip_describe;
	public UILabel change_equip_property;
	public UILabel change_gem_num_label;
	public UILabel change_gem_info;
	
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
		gem_num_str = now_gem_num_label.text;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	//弹出窗口   
	void RevealPanel()
	{
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

		ItemDesc item_info = (ItemDesc)CangKuManager.CangKuItemMap[change_equip_id];
		EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
	
		change_equip_icon.spriteName = equip_info.IconName;
		change_equip_name.text = equip_info.EquipName;
		change_equip_describe.text = equip_info.EquipDescribe;
		change_equip_property.text = U3dCmn.GetTipByExcelID((uint)item_info.nExcelID2);
		List<uint>change_gem_list = EquipManager.GetEmbedGem(change_equip_id);
		change_gem_num_label.text = gem_num_str+" "+change_gem_list.Count+"/4";
		for(int i=0;i<change_gem_list.Count;i++)
		{
			change_gem_info.text +=U3dCmn.GetTipByExcelID(change_gem_list[i])+"\n";
		}
		
		//change_equip_property.text = equip_info.Attack + "\n"+equip_info.Defense+ "\n"+equip_info.Life+ "\n"+equip_info.Leader+"\n"+"gem"+"\n"+GemEmbedGemNum(change_equip_id)+"/4";
		
		item_info = (ItemDesc)EquipManager.EquipItemMap[original_equip_id];
		equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)item_info.nExcelID2];
		
		now_equip_icon.spriteName = equip_info.IconName;
		now_equip_name.text = equip_info.EquipName;
		now_equip_describe.text = equip_info.EquipDescribe;
		now_equip_property.text = U3dCmn.GetTipByExcelID((uint)item_info.nExcelID2);
		List<uint>now_gem_list = EquipManager.GetEmbedGem(original_equip_id);
		now_gem_num_label.text = gem_num_str+" "+now_gem_list.Count+"/4";
		for(int i=0;i<now_gem_list.Count;i++)
		{
			now_gem_info.text +=U3dCmn.GetTipByExcelID(now_gem_list[i])+"\n";
		}
		
		//now_equip_property.text = equip_info.Attack + "\n"+equip_info.Defense+ "\n"+equip_info.Life+ "\n"+equip_info.Leader+"\n"+"gem"+"\n"+GemEmbedGemNum(original_equip_id)+"/4";
	}
	int GemEmbedGemNum(ulong equip_id)
	{
		int num =0;
		if(EquipManager.EquipToGemMap.Contains(equip_id))
		{
			EquipManager.EquipGemUnit unit =  (EquipManager.EquipGemUnit)EquipManager.EquipToGemMap[equip_id];
			if(unit.Slot1!=0)
				num++;
			if(unit.Slot2!=0)
				num++;
			if(unit.Slot3!=0)
				num++;
			if(unit.Slot4!=0)
				num++;
		}
		return num;	
	}
	void ChangeEquip()
	{
		DismissPanel();
		GameObject.Find("HeroEquipWin").SendMessage("ReqChangeEquip",change_equip_id);
	}
}
