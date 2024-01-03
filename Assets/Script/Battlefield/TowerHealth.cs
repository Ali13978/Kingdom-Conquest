using UnityEngine;
using System.Collections;
using CMNCMD;

public class TowerHealth : MonoBehaviour {
	
	public UISlider HealthBar = null;
	public UILabel TowerName = null;
	public UISprite Faceid = null;
	
	public int PowerAfter = 0;
	public int Health = 0;
	float SPercent = 0.72f;
	
	public int Victory = 0;
	TowerHealth mCTarget = null;
	
	// Use this for initialization
	void Start () {
		_onSetSCardPercent();
	}
	
	void _onSetSCardPercent()
	{
		uint ExcelID = 9;
		Hashtable gDefMap = CommonMB.CmnDefineMBInfo_Map;
		if (true == gDefMap.ContainsKey(ExcelID))
		{
			CmnDefineMBInfo card = (CmnDefineMBInfo)gDefMap[ExcelID];
			SPercent = card.num * 0.01f;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ApplyTowerData(string name, int faceID, int power, int bVictory)
	{
		if (TowerName != null) {
			TowerName.text = name;
		}
		
		if (Faceid != null) {
			Faceid.spriteName = U3dCmn.GetCharSmpIconName(faceID);
		}
		
		PowerAfter = power;
		Health = power;
		Victory = bVictory;
	}
	
	public void RoundAttack(int nDamage)
	{
		if (Victory == 1)
		{
			float d = (float)Mathf.Max(0,(Health - mCTarget.Health));
			nDamage = Mathf.Min(nDamage, Mathf.CeilToInt(d * SPercent));
		}
		else if (Victory == -1)
		{
			// 败方减吧 ...
		}
		
		Health = Mathf.Max(0, Health-nDamage);
	
		float per = (float)Health / (float)PowerAfter;
		if (HealthBar != null) {
			HealthBar.sliderValue = per;
		}
	}
	
	public void SetCTarget(TowerHealth objTarget)
	{
		mCTarget = objTarget;
	}
	
	
}
