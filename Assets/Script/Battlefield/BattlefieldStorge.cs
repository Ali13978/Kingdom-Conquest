using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class BattlefieldStorge : IgnoreTimeScale {

	public GameObject BombPrefab = null;
	public GameObject ArrowPrefab = null;
	
	public BaizhanPoster PosterCol = null;
	public TowerHealth Tower1 = null;
	public TowerHealth Tower2 = null;
	public GameObject template = null;
	
	int A1PowerProf = 0;
	int A2PowerProf = 0;
	
	int Tower1PowerAfter = 0;
	int Tower2PowerAfter = 0;

	public float spacing = 10f;
	BattlefieldSlot[] mSpriteBatcher = new BattlefieldSlot[10];
	
	UILabel RestTimeCol = null;
	public GameObject RestTimeBar = null;

	string mPrettyRestTime;
	public BattlefieldWin CoolBar = null;
	bool BeDefensePoster = false;
	
	int mHeroNum = 0;
	int mEnemyNum = 0;
	int mCombatType = 0;
	int mCombatR = 0;
	
	enum BattleStatus
	{
		Fighting = 0,
		Waiting = 1,
		End,
	};
	
	BattleStatus mStatus = BattleStatus.Fighting;
	
	void Awake() {
		
		if (PosterCol != null) {
			NGUITools.SetActive(PosterCol.gameObject, false);
		}
		if (RestTimeBar != null)
		{
			RestTimeCol = RestTimeBar.GetComponentInChildren<UILabel>();
			mPrettyRestTime = RestTimeCol.text;
			RestTimeBar.SetActiveRecursively(false);
		}
	}
	
	void RoundCombat(int nCombatType)
	{		
		for (int i=0; i<5; ++ i)
		{
			BattlefieldSlot c1 = mSpriteBatcher[i];
			BattlefieldSlot c2 = mSpriteBatcher[i+5];
			
			if (c1 != null && c2 != null)
			{
				CombatCalc_Raid_Round(nCombatType, c1, c2);
				c1.ArmyDamagePerRound = (int)((float)c1.ArmyRaidDamage/(float)c1.ArmyRaidRound);
				c2.ArmyDamagePerRound = (int)((float)c2.ArmyRaidDamage/(float)c2.ArmyRaidRound);
				
				//print("Round:" + c1.ArmyRaidRound + "," + c1.ArmyDamagePerRound + "," + c2.ArmyDamagePerRound + "," + c1.ArmyRaidDamage + "," + c2.ArmyRaidDamage);
			}
		}
	}
	
	void CombatCalc_Raid_PowerAfter(CombatFighting comFighting)
	{
		// 计算剩余战斗力  power	
		Tower1PowerAfter = 0;
		Tower2PowerAfter = 0;
		
		// Five roads
		for (int i=0; i<5; ++i)
		{
			BattlefieldSlot c1 = mSpriteBatcher[i];
			BattlefieldSlot c2 = mSpriteBatcher[i+5];
			
			if (c1 != null) {
				Tower1PowerAfter += c1.ArmyPowerAfter;
			}
		
			if (c2 != null) {
				Tower2PowerAfter += c2.ArmyPowerAfter;
			}
		}
		
		float rate = 0;
		if(Tower1PowerAfter > Tower2PowerAfter)
			rate = (float)Tower2PowerAfter/(float)Tower1PowerAfter;
		else
			rate = (float)Tower1PowerAfter/(float)Tower2PowerAfter;
		if (Tower1 != null)
		{
			int cc1 = 0;
			if (Tower1PowerAfter>Tower2PowerAfter) cc1 = 1;
			else if (Tower1PowerAfter<Tower2PowerAfter) cc1 = -1;
			Tower1.SetCTarget(Tower2);
			int defense = (int)(rate*5*Tower1PowerAfter);
			if(defense<=0)
				defense = 1;
			Tower1.ApplyTowerData(comFighting.szAttackName, comFighting.nAttackModel,defense, cc1);
		}
				
		if (Tower2 != null) 
		{
			int cc2 = 0;
			if (Tower2PowerAfter>Tower1PowerAfter) cc2 = 1;
			else if (Tower2PowerAfter<Tower1PowerAfter) cc2 = -1;
			Tower2.SetCTarget(Tower1);
			int defense =(int)(rate*5*Tower2PowerAfter);
			if(defense<=0)
				defense = 1;
			Tower2.ApplyTowerData(comFighting.szDefenseName,comFighting.nDefenseModel,defense, cc2);
		}
	}
	
	int RoundOrSolider(int type1)
	{
		int d1 = 0;
		if (type1 == (int)SoldierSort.QIANGBING) 		d1 = 0;
		else if (type1 == (int)SoldierSort.QIBING) 		d1 = 2;
		else if (type1 == (int)SoldierSort.JIANSHI) 	d1 = 0;
		else if (type1 == (int)SoldierSort.GONGBING) 	d1 = 5;
		else if (type1 == (int)SoldierSort.QIXIE) 		d1 = 6;
		return d1;
	}
	
	void CombatCalc_Diff_Round(int type1, int type2, out int d1, out int d2)
	{
		d1 = 0;
		d2 = 0;
		
		// 如果有远程兵种 ...
		if (type1 == (int)SoldierSort.QIXIE || type2 == (int)SoldierSort.QIXIE ||
			type1 == (int)SoldierSort.GONGBING || type2 == (int)SoldierSort.GONGBING)
		{
			d1 = RoundOrSolider(type1);
			d2 = RoundOrSolider(type2);
			
			if (d1>d2) {
				d1 = d1 - d2;
				d2 = 0;
			}
			else if (d2>d1) {
				d2 = d2 - d1;
				d1 = 0;
			}
			else 
			{
				d1 = 0;
				d2 = 0;
			}
		}
	}
	
	void CombatCalc_Raid_Round(int nCombatType, BattlefieldSlot c1, BattlefieldSlot c2)
	{
		int nHealthA = c1.health;
		int nHealthD = c2.health;
		int nCombatRound = 1;
		float nRecover = 80f;
		
		Hashtable cmnDefMap = CommonMB.CmnDefineMBInfo_Map;
		Hashtable crushArmyMap = CommonMB.ArmyDesc_Map;
		
		// 有兵种克制吗 ...
		float nMA = 1f;
		float nMD = 1f;
		
		uint army1_ID = (uint) c1.armyID;
		if (true == crushArmyMap.ContainsKey(army1_ID))
		{
			ArmyDescMBInfo info1 = (ArmyDescMBInfo) crushArmyMap[army1_ID];
			if ((int)c2.armyID== info1.aCrush[0] || (int)c2.armyID == info1.aCrush[1])
			{
				// 攻击加成 fBonus
				nMA += info1.fBonus;
			}
		}
		
		uint army2_ID = (uint) c2.armyID;
		if (true == crushArmyMap.ContainsKey(army2_ID))
		{
			ArmyDescMBInfo info2 = (ArmyDescMBInfo) crushArmyMap[army2_ID];
			if ((int)c1.armyID == info2.aCrush[0] || (int)c1.armyID == info2.aCrush[1])
			{
				// 攻击加成 fBonus
				nMD += info2.fBonus;
			}
		}
		
		uint cmn5_ID = 5;
		if (true == cmnDefMap.ContainsKey(cmn5_ID))
		{
			CmnDefineMBInfo cmnCard = (CmnDefineMBInfo) cmnDefMap[cmn5_ID];
			nRecover = cmnCard.num;
		}
		
		/*
		while (c1.health > 0 && c2.health > 0)
		{
			//攻方回合 Round
			int nDamageA = (int) ((float)c1.attackPower*nMA - (float)c2.armor);
			if (nDamageA>0)
			{
				c2.health -= nDamageA;
				c1.ArmyRaidDamage += nDamageA;
			}
			else 
			{
				// 如果双方攻防正好抵消，那么还是要算上一些伤害的 damaged
				if (nDamageA==0)
				{
					c2.health -= 10;
					c1.ArmyRaidDamage += 10;
				}
			}
			
			//守方回合 Round
			int nDamageD = (int) ((float)c2.attackPower*nMD - (float)c1.armor);
			if (nDamageD>0)
			{
				c1.health -= nDamageD;
				c2.ArmyRaidDamage += nDamageD;
			}
			else 
			{
				// 如果双方攻防正好抵消，那么还是要算上一些伤害的 damaged
				if (nDamageD==0)
				{
					c1.health -= 10;
					c2.ArmyRaidDamage += 10;
				}
			}
			
			if (nDamageA<0 && nDamageD<0)
			{
				// 双方攻击都不能造成伤害时，还是要比较一下 damaged
				if (nDamageA>=nDamageD)
				{
					c2.health -= 10;
					c1.ArmyRaidDamage += 10;
				}
				else
				{
					c1.health -= 10;
					c2.ArmyRaidDamage += 10;
				}
			}
			
			// 可以退出了吗 health
			if (c2.health <= 0)
			{
				c2.health = 0;
				if (c1.health <= 0)
				{
					c1.health += nDamageD;
					c2.ArmyRaidDamage -= nDamageD;
				}
				
				goto Round_GoOver;
			}
			if (c1.health <= 0)
			{
				c1.health = 0;
				if (c2.health <= 0)
				{
					c2.health += nDamageA;
					c2.ArmyRaidDamage -= nDamageA;
				}
					
				goto Round_GoOver;
			}
			
			// 回合数 calc
			nCombatRound ++;
		}
	
	Round_GoOver:
		
		// 计算部队剩余数量 death
		float fA = (nHealthA>0)? (float)c1.health/(float)nHealthA: 0;
		float fD = (nHealthD>0)? (float)c2.health/(float)nHealthD: 0;
		c1.health = nHealthA;
		c2.health = nHealthD;
		c1.ArmyPowerAfter = (int)(fA*c1.attackPower);
		c2.ArmyPowerAfter = (int)(fD*c2.attackPower);
		c1.ArmyNumAfter = (int)(fA*c1.ArmyNumBefore);
		c2.ArmyNumAfter = (int)(fD*c2.ArmyNumBefore);
		c1.ArmyRaidRound = nCombatRound;
		c2.ArmyRaidRound = nCombatRound;
		*/
		
		int nProfA	= (int)((c1.attackPower*nMA+c1.armor*0.9+c1.health*0.8)/3);
		if (nProfA==0 && c1.ArmyNumBefore>=0) nProfA=1;
		int nProfD	= (int)((c2.attackPower*nMD+c2.armor*0.9+c2.health*0.8)/3);
		if (nProfD==0 && c2.ArmyNumBefore>=0) nProfD=1;
		int nProfMin	= nProfA>nProfD? nProfD: nProfA;
		int nProfMax	= nProfA>nProfD? nProfA: nProfD;
		float fA = (nProfA>0)? (float)nProfMin/(float)nProfA: 0;
		float fD = (nProfD>0)? (float)nProfMin/(float)nProfD: 0;
		fA = (float)Math.Round(fA, 3);
		fD = (float)Math.Round(fD, 3);
		fA = 1-fA;
		fD = 1-fD;
		nHealthA = (int)(fA*c1.health);
		nHealthD = (int)(fD*c2.health);
		c1.ArmyNumAfter = (int)(fA*c1.ArmyNumBefore);
		c2.ArmyNumAfter = (int)(fD*c2.ArmyNumBefore);
		c1.ArmyPowerAfter = (int)(fA*nProfA);
		c2.ArmyPowerAfter = (int)(fD*nProfD);

		if (nProfMin > 0)
		{
		float fDiff = nProfMax/nProfMin;
		if (fDiff>=10)
			c1.ArmyRaidRound = 1;
		else if (fDiff>=8)
			c1.ArmyRaidRound = 2;
		else if (fDiff>=6)
			c1.ArmyRaidRound = 3;
		else if (fDiff>=4)
			c1.ArmyRaidRound = 4;
		else if (fDiff>=2)
			c1.ArmyRaidRound = 5;
		else
			c1.ArmyRaidRound = 6;
		}
		else
		{
			c1.ArmyRaidRound = 1;
		}
		
		c2.ArmyRaidRound = c1.ArmyRaidRound;
		c1.ArmyRaidDamage = c2.health - nHealthD;
		c2.ArmyRaidDamage = c1.health - nHealthA;
		// 添加回合数 ...
		int d1 = 0, d2 = 0;
		CombatCalc_Diff_Round((int)c1.armyID, (int)c2.armyID, out d1, out d2);
		c1.ArmyRaidRound = c1.ArmyRaidRound + d1;
		c2.ArmyRaidRound = c2.ArmyRaidRound + d2;
		
		if (c1.ArmyRaidDamage/c1.ArmyRaidRound*c1.ArmyRaidRound < c1.ArmyRaidDamage)
		{
			c1.ArmyRaidDamage = (c1.ArmyRaidDamage/c1.ArmyRaidRound+1)*c1.ArmyRaidRound;
		}
		if (c2.ArmyRaidDamage/c2.ArmyRaidRound*c2.ArmyRaidRound < c2.ArmyRaidDamage)
		{
			c2.ArmyRaidDamage = (c2.ArmyRaidDamage/c2.ArmyRaidRound+1)*c2.ArmyRaidRound;
		}

		c1.ArmyNumRecover = 0;
		c2.ArmyNumRecover = 0;
		if (nCombatType != (int)CombatTypeEnum.COMBAT_PVP_ATTACK)
		{
			float perf = nRecover * 0.01f;
			c1.ArmyNumRecover = (int)((c1.ArmyNumBefore-c1.ArmyNumAfter)*perf);
			c2.ArmyNumRecover = (int)((c2.ArmyNumBefore-c2.ArmyNumAfter)*perf);
		}
	}
	
	// Use this for initialization
	void Start ()
	{		
		if (template != null)
		{
			Vector3 offset = new Vector3(-140f,80f,0f);
			float rowZ = -1f;
			
			A1PowerProf = 0;
			A2PowerProf = 0;
			
			System.Array.Clear(mSpriteBatcher,0,10);
			
			CombatFighting comFighting = Globe.comFighting;
			if (comFighting != null)
			{
				mCombatType = comFighting.nCombatType;	
				BeDefensePoster = (mCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE || 
					mCombatType == (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE);
								
				int numRedCard = comFighting.nNumA;
				int numBlueCard = comFighting.nNumB;
				
				if (numRedCard>0)
				{
					for (int i=0; i<numRedCard; ++ i)
					{
						CombatCacheUnit newCard = comFighting.combatCards[i];
						//if (newCard.nArmyType < 1) continue; // 没有配置兵力, 直接过 ...
						
						GameObject go = NGUITools.AddChild(gameObject, template);
						Transform t = go.transform;
						rowZ = -1.0f - (newCard.nSlot-1)*0.1f;
						t.localPosition = new Vector3(offset.x, offset.y - (newCard.nSlot) * spacing, rowZ);
						//print ("offset"+offset.y +" "+newCard.nSlot+" "+spacing);
						BattlefieldSlot tk = go.GetComponent<BattlefieldSlot>();
						tk.armyID = (ArmyCategory) newCard.nArmyType;
						tk.campID = ArmyCampColor.Red;
						tk.toward = BattlefieldSlot.Direction.Right;
						tk.health 			= Mathf.Max(1,newCard.nHealth);
						tk.heroHealth		= Mathf.Max(1,newCard.nHealth);
						tk.ArmyRaidRound	= 1;
						tk.attackPower 		= newCard.nAttack;
						tk.ArmyPowerAfter	= newCard.nAttack;
						tk.armor  			= newCard.nDefense;
						tk.heroID 			= newCard.nHeroID;
						tk.ArmyNumBefore 	= newCard.nArmyNum;
						tk.ArmyNumAfter		= newCard.nArmyNum;
						tk.heroArmyType 	= newCard.nArmyType;
						tk.heroArmyLevel 	= newCard.nArmyLevel;
						tk.heroLevel		= newCard.nLevel;
						tk.HeroName 		= newCard.HeroName;
						
						// 出征战力 ...
						tk.SetCScene(this);
						tk.init2();
						tk.SetHealthAS(-8.0f);
						
						// 计算战力 ...
						float nProfA	= ((float)tk.attackPower+(float)tk.armor*0.9f+(float)tk.health*0.8f)/3f;
						A1PowerProf = A1PowerProf + Mathf.CeilToInt(nProfA);
						
						// 计算战力格子 ...						
						mSpriteBatcher[newCard.nSlot-1] = tk;
						
						
					}
					
					mHeroNum = numRedCard;
				}
				
				if (numBlueCard>0)
				{
					offset.x = 120f;
					rowZ = -1f;
					
					for (int i=0; i<numBlueCard; ++ i)
					{
						CombatCacheUnit newCard = comFighting.combatCards[i+numRedCard];
						//if (newCard.nArmyType < 1) continue; // 没有配置兵力, 直接过 ...
						
						GameObject go = NGUITools.AddChild(gameObject, template);
						Transform t = go.transform;
						rowZ = -1.0f - (newCard.nSlot-1)*0.1f;	
						t.localPosition = new Vector3(offset.x, offset.y - (newCard.nSlot) * spacing, rowZ);
						
						BattlefieldSlot tk = go.GetComponent<BattlefieldSlot>();
						tk.armyID = (ArmyCategory) newCard.nArmyType;
						tk.campID = ArmyCampColor.Blue;
						tk.toward = BattlefieldSlot.Direction.Left;
						tk.health 			= Mathf.Max(1,newCard.nHealth);
						tk.heroHealth		= Mathf.Max(1,newCard.nHealth);
						tk.ArmyRaidRound	= 1;
						tk.attackPower 		= newCard.nAttack;
						tk.ArmyPowerAfter	= newCard.nAttack;
						tk.armor 			= newCard.nDefense;
						tk.heroID 			= newCard.nHeroID;
						tk.ArmyNumBefore 	= newCard.nArmyNum;
						tk.ArmyNumAfter		= newCard.nArmyNum;
						tk.heroArmyType 	= newCard.nArmyType;
						tk.heroArmyLevel 	= newCard.nArmyLevel;
						tk.heroLevel		= newCard.nLevel;
						tk.HeroName 		= newCard.HeroName;
						// 设置初始状态 ...
						tk.SetCScene(this);
						tk.sprite.FlipX();
						tk.init2();
						tk.SetHealthAS(8.0f);
						
						// 计算战力 ...
						float nProfD	= ((float)tk.attackPower+(float)tk.armor*0.9f+(float)tk.health*0.8f)/3f;
						A2PowerProf = A2PowerProf + Mathf.CeilToInt(nProfD);
						
						// 战力格子 ....
						mSpriteBatcher[newCard.nSlot+4] = tk;
						
						//rowZ -= 0.1f;
					}
					
					mEnemyNum = numBlueCard;
				}
				
				// 回合战斗 combat
				RoundCombat(comFighting.nCombatType);
				
				// 计算最后战力作为血条 ...
				CombatCalc_Raid_PowerAfter(comFighting);
				
				// 开启战斗音乐 ...
				if (SoundManager.palying_music != CMNCMD.BG_MUSIC.BATTLE_MUSIC)
				{
					SoundManager.PlayBattleSound();
				}
			}
		}
	}
	
	public void BombGen(Vector3 cc, ArmyCampColor campID)
	{
		GameObject go = NGUITools.AddChild(gameObject, BombPrefab);
		if (go == null) return;
		
		BaizhanBomb t = go.GetComponent<BaizhanBomb>();
		if (t != null)
		{
			t.transform.localPosition = cc;
			t.Set(campID);
		}
	}
	
	public void ArrowGen(Transform myTrans, ArmyCampColor campID)
	{
		GameObject go = NGUITools.AddChild(gameObject, ArrowPrefab);
		if (go == null) return;
		
		BaizhanArrow t = go.GetComponent<BaizhanArrow>();
		if (t != null)
		{
			Vector3 offset = new Vector3(0f,28f,0f);
			t.transform.localPosition = myTrans.localPosition + offset;
			t.Set(campID);
		}
	}
	
	public void Begin(int endTimeSec)
	{
		StopCoroutine("BattleCooldown");
		StartCoroutine("BattleCooldown", endTimeSec);
	}
	
	// 战斗Cooldown 
	IEnumerator BattleCooldown(int endTimeSec)
	{
		if (RestTimeCol != null)
		{
			int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
			if (nSecs <0) nSecs = 0;
			
			while(nSecs > 0)
			{
				nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
				if(nSecs <0)
					nSecs = 0;

				RestTimeCol.text = string.Format(mPrettyRestTime,nSecs);
				yield return new WaitForSeconds(1);
			}
		}
		
		mStatus = BattleStatus.End;
	} 
	
	void LogicFighting()
	{
		switch (mStatus)
		{
		case BattleStatus.Fighting:
			{
				bool canOver = false;
				int c2Num = 0;
				int c1Num = 0;
				// Five roads
				for (int i=0; i<5; ++ i)
				{
					BattlefieldSlot c1 = mSpriteBatcher[i];
					BattlefieldSlot c2 = mSpriteBatcher[i+5];
					if (c1 == null && c2 == null) 
					{
						c1Num = c1Num + 1;
						c2Num = c2Num + 1;
						continue;
					}
				
					if (c2 == null) 
					{
						Transform t1 = c1.gameObject.transform;
						float dist = Mathf.Abs(t1.localPosition.x - 220);
						if (dist<c1.attackRange)
						{
							c1.EnmeryTower(Tower2);
						}
					
						c2Num = c2Num + 1;
					}
					else if (c1 == null)
					{
						Transform t2 = c2.gameObject.transform;
						float dist = Mathf.Abs(-220 - t2.localPosition.x);
						if (dist<c2.attackRange)
						{
							c2.EnmeryTower(Tower1);
						}
					
						c1Num = c1Num + 1;
					}
					else 
					{
						Transform t1 = c1.gameObject.transform;
						Transform t2 = c2.gameObject.transform;
					
						if (c1.health <= 0) 
						{
							float dist = Mathf.Abs(-220 - t2.localPosition.x);
							if (dist<c2.attackRange)
							{
								c2.EnmeryTower(Tower1);
							}
						
							c1Num = c1Num + 1;
						}
						else if (c2.health <= 0)
						{
							float dist = Mathf.Abs(t1.localPosition.x - 220);
							if (dist<c1.attackRange)
							{
								c1.EnmeryTower(Tower2);
							}
						
							c2Num = c2Num + 1;
						}
						else
						{
							float dist = Mathf.Abs(t1.localPosition.x - t2.localPosition.x);
							if (dist < c1.attackRange)
							{
								c1.EnmeryDamage(c2);
							}
							if (dist < c2.attackRange)
							{
								c2.EnmeryDamage(c1);
							}
						}
					}
				}
			
				if (Tower1.Health <= 0 || Tower2.Health <= 0 || (c1Num==5) || (c2Num==5) ) 
				{
					canOver = true;
				}
				
				if (canOver == true)
				{
					if (Tower1PowerAfter > Tower2PowerAfter) {
						//胜利了 
						if (true == BeDefensePoster) mCombatR = -1; else mCombatR = 1;
					}
					else if (Tower1PowerAfter < Tower2PowerAfter)
					{
						if (true == BeDefensePoster) mCombatR = 1; else mCombatR = -1; 
					}
					else
					{
						mCombatR = 0;
					}			
				
					if (PosterCol != null) 
					{
						if (mCombatR ==1) {
							PosterCol.Play("anim-victory");
						}
						else if (mCombatR == -1)
						{
							PosterCol.Play("anim-failure");
						}
					}
				
					mStatus = BattleStatus.Waiting;
					
					// 到计时关闭 
					if (RestTimeBar != null)
					{
						RestTimeBar.SetActiveRecursively(true);
						int endTimeSec = 3 + DataConvert.DateTimeToInt(DateTime.Now);
						Begin(endTimeSec);
					}
				}
			}
			
			break;
	
		case BattleStatus.End:
			{
				if (CoolBar != null)
				{
					float depth = CoolBar.GetDepth() - BaizVariableScript.DEPTH_OFFSET;
					ShowCasulatyWindow(depth);
			
					// 停止声音 ...
					BaizhanCampWin win1 = GameObject.FindObjectOfType(typeof(BaizhanCampWin)) as BaizhanCampWin;
					NanBeizhanScene win2 = GameObject.FindObjectOfType(typeof(NanBeizhanScene)) as NanBeizhanScene;
					if (win1 == null && win2 == null) 
					{ 
						SoundManager.StopBattleSound();
						SoundManager.PlayMainSceneSound();
					}
				
					// iphone5 挪开 ...
					KeepOut.instance.HideKeepOut();
				
					// cip ...
					Destroy(CoolBar.gameObject);
				
					// 释放资源 ...
					Resources.UnloadUnusedAssets();
				}
			}
			
			break;
		}
	}
	
	void ShowCasulatyWindow(float depth)
	{
		List<COMBAT_DEATH_INFO> cacheList = new List<COMBAT_DEATH_INFO>();		
		Hashtable jlMap = JiangLingManager.MyHeroMap;
		
		int[] n_hero = new int[2];
		int[] n_enemy = new int[2];
		int i=0, ox=0;
		int n_HeroNum = 0, n_EnemyNum = 0;
		int n_HeroProf = 0, n_EnemyProf = 0;
		
		int Tipset = CombatManager.GetCombatTipset(mCombatType);
		
		switch(mCombatType)
		{
		case (int)CombatTypeEnum.COMBAT_PVP_DEFENSE:
		case (int)CombatTypeEnum.COMBAT_PVP_WORLDGOLDMINE_DEFENSE:
			{
				n_hero[0] 	= 5;
				n_hero[1] 	= mEnemyNum;
				n_HeroProf	= A2PowerProf;
			
				n_enemy[0] 	= 0;
				n_enemy[1] 	= mHeroNum;
				n_EnemyProf = A1PowerProf;
			} 
			
			break;
			
		default:
			{
				n_hero[0] 	= 0;
				n_hero[1] 	= mHeroNum;
				n_HeroProf	= A1PowerProf;
			
				n_enemy[0] 	= 5;
				n_enemy[1] 	= mEnemyNum;
				n_EnemyProf = A2PowerProf;
			}
			
			break;
		}
		
		ox = n_hero[0];
		for (i=0; i<5; ++ i)
		{
			BattlefieldSlot g = mSpriteBatcher[i+ox];
			if (g == null) continue;
			
			COMBAT_DEATH_INFO info = new COMBAT_DEATH_INFO();
			info.HeroName = g.HeroName;
			info.nArmyNum = g.ArmyNumBefore;
			info.nArmyDeadNum = g.ArmyNumBefore-g.ArmyNumAfter;
			info.nArmyLevel = g.heroArmyLevel;
			info.nLevel = g.heroLevel;
			info.nArmyType = g.heroArmyType;
			if (mCombatType == (int)CombatTypeEnum.COMBAT_PVP_ATTACK ||
				mCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE )
			{
				info.nArmyNumRecover = 0;
			}
			else 
			{
				info.nArmyNumRecover = g.ArmyNumRecover;
			}
			
			cacheList.Add(info);
			
			n_HeroNum ++;
		}
		
		ox = n_enemy[0];
		for (i=0; i<5; ++ i)
		{
			BattlefieldSlot g = mSpriteBatcher[i+ox];
			if (g == null) continue;
			
			COMBAT_DEATH_INFO info = new COMBAT_DEATH_INFO();
			
			if (Tipset != 0)
			{
				Tipset = CombatManager.GetCombatGeneral(mCombatType);
				info.HeroName = U3dCmn.GetWarnErrTipFromMB(Tipset);
			}
			else 
			{
				info.HeroName = g.HeroName;
			}
			
			if (mCombatType == (int)CombatTypeEnum.COMBAT_PVE_WORLDGOLDMINE ||
				mCombatType == (int)CombatTypeEnum.COMBAT_PVE_RAID ||
				mCombatType == (int)CombatTypeEnum.COMBAT_PVP_ATTACK ||
				mCombatType == (int)CombatTypeEnum.COMBAT_PVP_DEFENSE ||
				mCombatType == (int)CombatTypeEnum.COMBAT_WORLDCITY ||
				mCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_BAIZHANBUDAI ||
				mCombatType == (int)CombatTypeEnum.COMBAT_INSTANCE_NANZHENGBEIZHAN)
			{
				info.nArmyNumRecover = 0;
			}
			else 
			{
				info.nArmyNumRecover = g.ArmyNumRecover;
			}
			
			info.nArmyNum = g.ArmyNumBefore;
			info.nArmyDeadNum = g.ArmyNumBefore-g.ArmyNumAfter;
			info.nArmyLevel = g.heroArmyLevel;
			info.nLevel = g.heroLevel;
			info.nArmyType = g.heroArmyType;
			cacheList.Add(info);
			
			n_EnemyNum ++;
		}

		if (cacheList.Count == 0) return;
		
		GameObject go = U3dCmn.GetObjFromPrefab("BaizhanDeathWin");
		if (go == null) return;
		
		BaizhanDeathWin obj = go.GetComponent<BaizhanDeathWin>();
		if (obj != null)
		{
			if (mCombatR == 1)
			{
				string tc = U3dCmn.GetWarnErrTipFromMB(50);
				obj.SetTitle(tc);
			}
			else if (mCombatR == -1)
			{
				string tc = U3dCmn.GetWarnErrTipFromMB(51);
				obj.SetTitle(tc);
			}
			
			obj.Depth(depth);
			obj.DeathDataCleanUp();
			obj.ApplyDeathData(cacheList, n_HeroNum, n_EnemyNum, n_HeroProf, n_EnemyProf);
		}
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{	
		LogicFighting();
	}
	
	
}
