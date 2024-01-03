using UnityEngine;
using System.Collections;
using Squared.Tiled;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;

public enum CastleType
{
	None = 0,
	Large = 1,
	Medium = 2,
	Lower = 3,
}

public class TiledStorgeSlot : MonoBehaviour {
	
	public int mapCoordX = 0;
	public int mapCoordY = 0;
	
	public GameObject castle1 = null;
	public GameObject castle2 = null;
	public GameObject castle3 = null;
	
	public tk2dSprite idGroundCol = null;
	public tk2dSprite cityImgCol = null;
	//public UISprite idGroundCol = null;
	//public UISprite cityImgCol = null;
	
	public UISprite BoundCol = null;
	public tk2dSlicedSprite NameBgCol = null;
	
	// public UISprite NameBgCol = null;
	public UILabel cityNameCol = null;
	public tk2dSprite cityMoveable = null;
	public tk2dSprite cityProtect = null;
	
	public tk2dSprite cityCampCol = null;
	//public UISprite cityCampCol = null;
	public UILabel cityLevelCol = null;

	public tk2dSprite cityAllianceCol = null;
	public UILabel allianceCol = null;
	
	public tk2dSprite cityGrailCol = null;
	public UILabel cityGrailNum = null;
	
	bool hasChanged = false;
	bool hasCityChanged = false; // then
	
	int hasCastleType = 0;
	
	TiledStorge mCScene = null;

	// Use this for initialization
	void Awake ()
	{
		if (cityCampCol != null) {
			NGUITools.SetActiveSelf(cityCampCol.gameObject,false);
		}		
		if (cityLevelCol != null) {
			NGUITools.SetActiveSelf(cityLevelCol.gameObject,false);
		}
		if (NameBgCol != null) {
			NGUITools.SetActiveSelf(NameBgCol.gameObject,false);
		}
		if (cityImgCol != null) {
			NGUITools.SetActiveSelf(cityImgCol.gameObject,false);
		}
		if (cityNameCol != null) { 
			NGUITools.SetActiveSelf(cityNameCol.gameObject,false);
		}	
		if (BoundCol != null) {
			BoundCol.enabled = false;
		}
		if (cityAllianceCol != null) {
			NGUITools.SetActiveSelf(cityAllianceCol.gameObject,false);
		}
		if (allianceCol != null) {
			NGUITools.SetActiveSelf(allianceCol.gameObject,false);
		}
		if (cityMoveable != null) {
			NGUITools.SetActiveSelf(cityMoveable.gameObject, false);
		}
		if (cityProtect != null) {
			NGUITools.SetActiveSelf(cityProtect.gameObject, false);
		}
		if (cityGrailNum != null) {
			NGUITools.SetActiveSelf(cityGrailNum.gameObject, false);
		}
		if (cityGrailCol != null) {
			NGUITools.SetActiveSelf(cityGrailCol.gameObject, false);
		}
		if (castle1 != null) {
			NGUITools.SetActive(castle1, false);
		}
		if (castle2 != null) {
			NGUITools.SetActive(castle2, false);
		}
		if (castle3 != null) {
			NGUITools.SetActive(castle3, false);
		}
	}
	
	void OnDestroy()
	{
		TiledStorgeCacheData.processHonorCityDelegate -= OnProcessHonorCityDelegate;
	}
	
	public int mapX
	{
		get
		{
			return mapCoordX;
		}
		
		set
		{
			if (mapCoordX != value)
			{
				mapCoordX = value;
				hasChanged = true;
				hasCityChanged = false;
			}
		}
	}
		
	public int mapY
	{
		get {
			return mapCoordY;
		}
		
		set
		{
			if (mapCoordY != value)
			{
				mapCoordY = value;
				hasChanged = true;
				hasCityChanged = false;
			}
		}
	}
	
	public int idCity
	{
		set 
		{
			if (cityImgCol != null)
			{
				if (value > 0)
				{
					int m = (value - 1);
					string s = "Decorate/" + m.ToString();
					cityImgCol.spriteId = cityImgCol.GetSpriteIdByName(s);
					
					//int m = value;
					//cityImgCol.spriteName = string.Format("Decorate_{0:D2}",m);
					NGUITools.SetActiveSelf(cityImgCol.gameObject,true);
				}
				else 
				{
					NGUITools.SetActiveSelf(cityImgCol.gameObject,false);
				}
			}
		}
	}
	
	public int idBackground
	{
		set
		{
			if (idGroundCol != null)
			{
				if (value > 0)
				{
					int m = (value - 1);
					string s = "Tiled/" + m.ToString();
					idGroundCol.spriteId = idGroundCol.GetSpriteIdByName(s);
					
					//int m = value;
					//idGroundCol.spriteName = string.Format("Tiled_{0:D2}",m);
				}
				else 
				{
					//idGroundCol.spriteName = "Tiled_01";
					idGroundCol.spriteId = idGroundCol.GetSpriteIdByName("Tiled/0");
				}
			}
		}
	}
	
	public void SetCScene(TiledStorge obj)
	{
		this.mCScene = obj;
	}
	
	public void ResetItem()
	{
		hasChanged = true;
		hasCityChanged = false;
	}
	
	// 查看名城列表 ....
	public void OnProcessHonorCityDelegate()
	{
		int ccOver = TiledStorgeCacheData.HonorWarRestTime;
		if (ccOver < 1) 
		{
			// 地图上的荣耀之城数据不新了, 更新下 ...
			mCScene.ReUnpack1();
			
			// 提示荣耀之战结束了 ...
			int Tipset = BaizVariableScript.COMBAT_HONOR_GAMEOVER;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			return;
		}
		
		ulong AutoID = ( ((ulong)mapCoordX << 32) + (ulong)mapCoordY); // ...
		Hashtable hcMap = TiledStorgeCacheData.HonorCityMap;
		if (false == hcMap.ContainsKey(AutoID))
		{
			// 地图上的荣耀之城数据不新了, 更新下 ...
			mCScene.ReUnpack1();
			
			// 提示选择其他城池 ...
			int Tipset = BaizVariableScript.COMBAT_HONOR_NO_GRAIL;
			PopTipDialog.instance.VoidSetText1(true,false,Tipset);
			return;
		}
		
		GameObject go = U3dCmn.GetObjFromPrefab("WorldCityGuardWin");
		if (go == null) return;
	
		WorldCityGuard win1 = go.GetComponent<WorldCityGuard>();
		if (win1 != null)
		{
			CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
			float d1 = (float) Mathf.Abs((int)info.PosX - mapCoordX);
			float d2 = (float) Mathf.Abs((int)info.PosY - mapCoordY);
				
			float sq1 = Mathf.Sqrt(d1*d1 + d2*d2);
			float sq2 = Mathf.Sqrt(400f*400f*2f);
			int secs = Mathf.CeilToInt((sq1/sq2) * 8f * 3600f); /// 最长时间为8小时 ...
			
			float depth = - BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
			win1.SetGoToTime(secs);
			win1.ApplySetHonorCity(AutoID, mapCoordX, mapCoordY);
		}
	}
	
	
	/// <summary>
	/// Raises the click event.
	/// </summary> 
	void OnClick()
	{
		TiledCityEntry cityCard = null;
		TiledStorgeCacheData.instance.GetCity(mapCoordX,mapCoordY,out cityCard);
		TiledResEntry ResCard = null;
		TiledStorgeCacheData.instance.GetResCity(mapCoordX,mapCoordY,out ResCard);
		if (mCScene != null) {
			mCScene.SetFocus1(mapCoordX, mapCoordY);
		}
		
		if (cityCard != null)
		{			
			TiledStorgeCacheData.instance.GPSFocus(this);
			GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
			if (obj == null) return;
			if(cityCard.nAccountID != CommonData.player_online_info.AccountID)
			{
				PlayerCardWin.Counterattack = true;
				PlayerCardWin.Favorite = true;
			}
			
			
			obj.SendMessage("RevealPanel",cityCard.nAccountID);
		}
		else if(ResCard != null)
		{
			TiledStorgeCacheData.instance.GPSFocus(this);
			TiledStorgeCacheData.instance.ReqResCityInfo(ResCard.nPosX,ResCard.nPosY,ResCard.nID);
		}
		else
		{
			TiledCacheEntry card = null;
			TiledStorgeCacheData.instance.GetEntry(mapCoordX,mapCoordY,out card);
			
			if (hasCastleType > 0)
			{
				// 生成 AutoID
				TiledStorgeCacheData.instance.GPSFocus(this);
				TiledStorgeCacheData.processHonorCityDelegate = OnProcessHonorCityDelegate;
				TiledStorgeCacheData.RequestHonorCity();
			}
			else
			{
				bool canBuild = true;
				if (card != null) {
					canBuild = card.canBuild;
				}
				
				// 不能建城 ...
				if (canBuild == false ) return;
				
				bool movable = TiledStorgeCacheData.instance.isCityMovable;
				if (true == movable)
				{
					GameObject go = U3dCmn.GetObjFromPrefab("WorldMapMovableWin");
					if (go == null) return;
					
					WorldMapCityMovable t = go.GetComponent<WorldMapCityMovable>();
					if (t != null)
					{
						float depth = -BaizVariableScript.DEPTH_OFFSET * 2.0f;
						t.Depth(depth);
						t.ApplyCityMoveable(mapCoordX, mapCoordY);
					}
				}
				else 
				{
					TiledStorgeCacheData.instance.GPSFocus(this);
				}
			}
		}
	}
	
	// 更新奖杯图标 ....
	void UpdateGrailData()
	{
		if (cityGrailCol != null) {
			NGUITools.SetActiveSelf(cityGrailCol.gameObject,true);
		}
		if (cityGrailNum != null) 
		{	
			Hashtable hcMap = TiledStorgeCacheData.HonorCityMap;
			ulong AutoID = ( ((ulong)mapCoordX << 32) + (ulong)mapCoordY); // ...
			if (true == hcMap.ContainsKey(AutoID))
			{
				WorldCity card = (WorldCity) hcMap[AutoID];
				cityGrailNum.text = string.Format("{0}", card.nCup6);
				NGUITools.SetActiveSelf(cityGrailNum.gameObject,true);
			}
		}
	}
	
	void LateUpdate()
	{		
		if (true == hasChanged )
		{	
			StopCoroutine("ProtectCooldown");
			hasCityChanged = false;
			hasCastleType = 0;

			// 我的地盘我做主 ...
			TiledStorgeCacheData.instance.GPSDisbandFocus(this);
			int nPosX = (int) CommonData.player_online_info.PosX;
			int nPosY = (int) CommonData.player_online_info.PosY;
			if (nPosX == mapCoordX && nPosY == mapCoordY)
			{
				TiledStorgeCacheData.GPSMySlot1(this);
			}
			
			// network don't work... , oh my god
			if (cityCampCol != null) {
				NGUITools.SetActiveSelf(cityCampCol.gameObject,false);
			}
			if (cityNameCol != null) {
				NGUITools.SetActiveSelf(cityNameCol.gameObject,false);
			}
			if (cityLevelCol != null) {
				NGUITools.SetActiveSelf(cityLevelCol.gameObject,false);
			}
			if (NameBgCol != null) {
				NGUITools.SetActiveSelf(NameBgCol.gameObject,false);
			}
			if (cityAllianceCol != null) {
				NGUITools.SetActiveSelf(cityAllianceCol.gameObject,false);
			}
			if (allianceCol != null) {
				NGUITools.SetActiveSelf(allianceCol.gameObject,false);
			}
			if (cityGrailCol != null) {
				NGUITools.SetActiveSelf(cityGrailCol.gameObject, false);
			}
			if (cityGrailNum != null) {
				NGUITools.SetActiveSelf(cityGrailNum.gameObject, false);
			}
			if (cityMoveable != null) {
				NGUITools.SetActiveSelf(cityMoveable.gameObject, false);
			}
			if (cityProtect != null) {
				NGUITools.SetActiveSelf(cityProtect.gameObject, false);
			}
			if (castle1 != null) {
				NGUITools.SetActive(castle1, false);
			}
			if (castle2 != null) {
				NGUITools.SetActive(castle2, false);
			}
			if (castle3 != null) {
				NGUITools.SetActive(castle3, false);
			}
			
			TiledCacheEntry entry = null;
			TiledCacheUnit.UnitStatus st = TiledStorgeCacheData.instance.GetEntry(mapCoordX,mapCoordY,out entry);
			
			switch (st)
			{
			case TiledCacheUnit.UnitStatus.None:
				{
					idBackground = 0;
					idCity = 0;
					hasChanged = false;
				} break;
				
			case TiledCacheUnit.UnitStatus.Loading:
				{
					idBackground = 0;
					idCity = 0;
				} break;
				
			case TiledCacheUnit.UnitStatus.Ready:
				{
					hasChanged = false;
					if (entry == null)
					{
						idBackground = 0;
						idCity = 0;
						hasCityChanged = true; /// 暂时做为名城用吧 ...
						
					}
					else 
					{
						idBackground = entry.g1;
						idCity = entry.g2;
						hasCityChanged = entry.canBuild;
						if (entry.g2 > 100 && entry.g2 <200) // 该地方会有名城 ...
						{
							ulong AutoID = ( ((ulong)mapCoordX << 32) + (ulong)mapCoordY); // ...
							Hashtable hcMap = TiledStorgeCacheData.HonorCityMap;
							if (true == hcMap.ContainsKey(AutoID)) 
							{
								idCity = 0;
								int idRand = entry.g2 - 100;
								if (idRand>2) 
								{
									// 低级名城 ...
									hasCastleType = (int) CastleType.Lower;
									if (castle1 != null) { NGUITools.SetActive(castle1,true); }
								}
								else if (idRand>1) 
								{
									// 中级名城 ...
									hasCastleType = (int) CastleType.Medium;
									if (castle2 != null) { NGUITools.SetActive(castle2,true); }
								}
								else if (idRand>0) 
								{
									// 高级名城 ...
									hasCastleType = (int) CastleType.Large;
									if (castle3 != null) { NGUITools.SetActive(castle3,true); }
								}
								
								// 更新奖杯 ...
								// UpdateGrailData();
							}
						}
						if (entry.g2 > 200) // 该地方会有世界资源 ...
						{
								
							TiledResEntry ResCard = null;
							TiledCacheUnit.UnitStatus tile_state = TiledStorgeCacheData.instance.GetResCity(mapCoordX,mapCoordY,out ResCard);
							if (tile_state == TiledCacheUnit.UnitStatus.None) 
							{
								return;
							}
							if (tile_state == TiledCacheUnit.UnitStatus.ReadyCity)
							{
								if (ResCard != null)
								{
									if(ResCard.nType == (uint)WORLD_RESOURCE.COTTAGE)
									{
										cityImgCol.spriteId = cityAllianceCol.GetSpriteIdByName("Cottage");
									}
									else if(ResCard.nType == (uint)WORLD_RESOURCE.GOLDMINE)
									{
										cityImgCol.spriteId = cityAllianceCol.GetSpriteIdByName("Mine/0");
									}
								    NGUITools.SetActiveSelf(cityImgCol.gameObject, true);
								
									this.cityLevelCol.text = ResCard.nLevel.ToString();
									NGUITools.SetActiveSelf(cityLevelCol.gameObject,true);
									cityCampCol.spriteId = cityCampCol.GetSpriteIdByName("cityCamp/0");
									NGUITools.SetActiveSelf(cityCampCol.gameObject,true);
									
								}
							}
							
							//NGUITools.SetActiveSelf(cityImgCol.gameObject, true);
						}
					}
						
				
				} break;
			}
		}
				
		// Get city icon
		if (true == hasCityChanged)
		{
			TiledCityEntry cityCard = null;
			TiledCacheUnit.UnitStatus st = TiledStorgeCacheData.instance.GetCity(mapCoordX,mapCoordY,out cityCard);
			if (st == TiledCacheUnit.UnitStatus.None) 
			{
				return;
			}
			
			// 城市数据已获取完成 ...
			if (st == TiledCacheUnit.UnitStatus.ReadyCity)
			{
				hasCityChanged = false;
				if (cityCard != null)
				{
					cityImgCol.spriteId = cityImgCol.GetSpriteIdByName("cityCard/0");
					CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
					if (cityCard.nAccountID == info.AccountID) {
						cityCampCol.spriteId = cityCampCol.GetSpriteIdByName("cityCamp/1");
					}
					else if (cityCard.nAllianceID == 0 || cityCard.nAllianceID != info.AllianceID) {
						cityCampCol.spriteId = cityCampCol.GetSpriteIdByName("cityCamp/0");
					}
					else 
					{
						cityCampCol.spriteId = cityCampCol.GetSpriteIdByName("cityCamp/1");
					}
					
					// 是否有加入联盟 ...
					if (cityCard.nAllianceID != 0)
					{
						if (cityCard.nAllianceID == info.AllianceID) {
							cityAllianceCol.spriteId = cityAllianceCol.GetSpriteIdByName("cityAlliance/0");
						}
						else 
						{
							cityAllianceCol.spriteId = cityAllianceCol.GetSpriteIdByName("cityAlliance/1");
						}
						
						NGUITools.SetActiveSelf(cityAllianceCol.gameObject,true);
						this.allianceCol.text = cityCard.allianceName;
						NGUITools.SetActiveSelf(allianceCol.gameObject, true);
					}
					
					// 是否有保护光圈 ...
					if (cityCard.nProtectTime>0) 
					{
						int endTimeSec = (int) cityCard.nProtectTime + DataConvert.DateTimeToInt(DateTime.Now);
						Begin(endTimeSec);
					}
					
					NGUITools.SetActiveSelf(cityCampCol.gameObject, true);
					NGUITools.SetActiveSelf(cityImgCol.gameObject, true);
					this.cityNameCol.text = cityCard.name;
					NGUITools.SetActiveSelf(cityNameCol.gameObject,true);
					this.cityLevelCol.text = cityCard.level.ToString();
					NGUITools.SetActiveSelf(cityLevelCol.gameObject,true);
					
					if (!string.IsNullOrEmpty(cityCard.name))
					{
						NGUITools.SetActiveSelf(NameBgCol.gameObject, true);
						float MaxLen = Mathf.Min(90f, 14f*cityCard.name.Length);
						NameBgCol.dimensions = new Vector2(MaxLen, 20f);
					}

					NGUITools.SetActiveSelf(cityProtect.gameObject, cityCard.nProtectTime>0);
				}
				else 
				{
					// 是否是可迁城状态 ...
					bool movable = TiledStorgeCacheData.instance.isCityMovable;
					NGUITools.SetActiveSelf(cityMoveable.gameObject, movable);
				}
			}
		}
	}
	
	void Begin(int endTimeSec)
	{
		StopCoroutine("ProtectCooldown");
		StartCoroutine("ProtectCooldown", endTimeSec);
	}
	
	IEnumerator ProtectCooldown(int endTimeSec)
	{
		NGUITools.SetActiveSelf(cityProtect.gameObject, true);
		int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		if (nSecs <0) nSecs = 0;
		
		while(nSecs > 0)
		{
			nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
			if(nSecs <0)
				nSecs = 0;
			
			yield return new WaitForSeconds(1);
		}
		
		// 解除光圈 ...
		NGUITools.SetActiveSelf(cityProtect.gameObject, false);
		
		TiledCityEntry cityCard = null;
		TiledStorgeCacheData.instance.GetCity(mapCoordX,mapCoordY,out cityCard);
		if (cityCard != null) 
		{
			cityCard.nProtectTime = 0;
		}
	}
}
