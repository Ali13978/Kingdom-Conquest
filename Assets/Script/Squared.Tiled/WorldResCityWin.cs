using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class WorldResCityWin : MonoBehaviour {
	public TweenPosition 	tween_position;
	public UISprite res_icon;
	public UILabel  res_desc;
	public UILabel level_label;
	public UILabel force_label;
	public UILabel res_label;
	public UILabel position_label;
	public WorldRes city_info;
	string level_str;
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
		level_str = level_label.text;
	}
	// Use this for initialization
	void Start () {
	
	}
	public float GetDepth()
	{
		return gameObject.transform.localPosition.z;
	}
	// Update is called once per frame
	void Update () {
	
	}
	void RevealPanel()
	{
		if(city_info.nID1 == 0)
		{
			return;
		}
		res_desc.text = U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.WORLD_RES_DESC);
		if(city_info.nType2 == (uint)WORLD_RESOURCE.GOLDMINE)
		{
			res_icon.spriteName = "world_jinkuang";
			res_label.text = city_info.nGold6.ToString()+" "+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GOLD);
		}
		else if(city_info.nType2 == (uint)WORLD_RESOURCE.COTTAGE)
		{
			res_icon.spriteName = "world_minju";
			res_label.text = city_info.nPop7.ToString()+" "+U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.POP);
		}
		
		force_label.text = city_info.nForce9.ToString();
		//print ("fffffffffffffffffffff"+ResCityInfo.nForce9);
		position_label.text = city_info.nPosX4 +","+city_info.nPosY5;
		level_label.text = string.Format(level_str,city_info.nLevel3);
		position_label.text = city_info.nPosX4+","+city_info.nPosY5;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//攻打 
	void AttackRecCity()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("PopGeneralCrushWin");
		if (go == null) return;
		PopGeneralCrush win1 = go.GetComponent<PopGeneralCrush>();
		if (win1 != null)
		{
			CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
			float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			
			float d1 = (float) Mathf.Abs((int)info.PosX - city_info.nPosX4);
			float d2 = (float) Mathf.Abs((int)info.PosY - city_info.nPosY5);
			
			float sq1 = Mathf.Sqrt(d1*d1 + d2*d2);
			float sq2 = Mathf.Sqrt(400f*400f*2f);
			Hashtable cgtMap = CommonMB.CombatGoToTimeMBInfo_Map;
			uint combatTypeID = (uint)CombatTypeEnum.COMBAT_PVP_ATTACK;
			int secs =0;
			if (true == cgtMap.ContainsKey(combatTypeID))
			{
				CombatGoToTimeMBInfo timeinfo = (CombatGoToTimeMBInfo) cgtMap[combatTypeID];
				secs = Mathf.CeilToInt((sq1/sq2) * timeinfo.GoToTime);
	
			}
			win1.ApplyResCityID(city_info.nID1,city_info.nType2);
			win1.ApplyGoToTime(secs);
			DismissPanel();
		}
	}
	
}
