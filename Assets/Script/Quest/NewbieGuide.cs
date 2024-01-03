using UnityEngine;
using System.Collections;
using CMNCMD;
using STCCMD;


public class NewbieGuide : MonoBehaviour {
		
	public GameObject BtnFini 	= null; 		// 全部 ...
	public GameObject BtnClose 	= null;			// 关闭 ...
	public GameObject BtnPage1 	= null;
	public GameObject Selector 		= null;
	public GameObject ManipIntro 	= null;
	public UILabel DepictionCol 	= null;
	public GameObject ButtonF 		= null;
	public UISprite Jiantou 		= null;
	public tk2dSlicedSprite Frame1 = null;
	UIAnchor mDepth = null;
	
	public static string NEWBIE_GUIDE_1 = "{0}:Guide1-1";
	public static string CHUZHENG_1 = "{0}:g-CHUZHENG";
	
	bool mBooDone = false;
	int m_guide_point = 0;
	
	void Awake() 
	{		
		if (BtnClose != null) {
			NGUITools.SetActive(BtnClose, false);
		}
		if (BtnFini != null) {
			UIEventListener.Get(BtnFini).onClick = VoidButtonSkipOverDelegate;
		}
		
		// IPAD 适配尺寸 ...
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			if (root != null) { root.manualHeight = 360; }
		}
	}
	
	public int CP
	{
		get { return m_guide_point; }
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	public float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	void PopFreeZhuanPan()
	{
		//弹出免费抽奖 
		if(CommonData.player_online_info.FreeDrawLotteryNum >0)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPan"); 
			if(win != null)
			{
				win.SendMessage("RevealPanel");
			}
		}
	}
	
	// 跳至最后一步 ....
	void VoidButtonOkDelegate(GameObject go)
	{
		CMN_PLAYER_CARD_INFO player = CommonData.player_online_info;
		string gCol = string.Format(NEWBIE_GUIDE_1,player.AccountName);
		
		// 跳到最后一步引导 ...
		Hashtable gcMap = CommonMB.NewbieGuideInfo_Map;
		int imax = gcMap.Count;
		if (imax > 0)
		{
			// 解除指引界面 ...
			PlayerPrefs.SetInt(gCol, (imax+1));
		}	
		
		PopTipDialog.instance.Dissband();
		
		gCol = string.Format(TaskManager.QUEST_POP,player.AccountName);
		int BooDone = PlayerPrefs.GetInt(gCol, 0);
		if (BooDone == 1)
		{			
			Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
			uint canChatPop = 14;
			if (true == sMap.ContainsKey(canChatPop))
			{
				CmnDefineMBInfo newCard = (CmnDefineMBInfo) sMap[canChatPop];
				if (newCard.num == 1)
				{
					float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
					QuestGuideIcon.instance.PopChat(depth);
				}
				else if (newCard.num == 0)
				{
					PopFreeZhuanPan();
				}
			}
		}
		
		// 关闭加速 ...
		PopAccelatePin win1 = GameObject.FindObjectOfType(typeof(PopAccelatePin)) as PopAccelatePin;
		if (win1 != null) {
			Destroy(win1.gameObject);
		}
		
		// 关闭军情 ...
		WarSituationWin win2 = GameObject.FindObjectOfType(typeof(WarSituationWin)) as WarSituationWin;
		if (win2 != null) {
			Destroy(win2.gameObject);
		}
		
		// 指引结束 ....
		Destroy(gameObject);
	}
	
	// 跳至最后一步 ....
	void VoidButtonSkipOverDelegate(GameObject go)
	{
		float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
		int Tipset = BaizVariableScript.CHAT_MSG_QUEST_GUIDE_SKIP_OVER;
		PopTipDialog.instance.Mode1(true, true, Tipset);
		PopTipDialog.instance.VoidButton1(VoidButtonOkDelegate);
		PopTipDialog.instance.Popup1(depth);
	}
	// 设置橙色选择框位置 ...
	public void VoidButtonSelector(bool active, float xPos, float yPos, UIEventListener.VoidDelegate onClick)
	{
		if (Selector != null) {
			Transform tc1 = Selector.transform;
			tc1.localPosition = new Vector3(xPos,yPos,0f);
			UIEventListener.Get(Selector).onClick = onClick;
			NGUITools.SetActive(Selector, active);
		}
	}
	// 设置橙色选择框大小 ....
	public void VoidFrame1Dimensions(float xPos, float yPos)
	{
		if (Frame1 != null) {
			Frame1.dimensions = new Vector2(xPos,yPos);
		}
	}
	// 设置箭头指示器位置 ...
	public void VoidButtonJiantou(float xPos, float yPos, float d1)
	{
		if (ButtonF != null) {
			Transform tc1 = ButtonF.transform;
			tc1.localPosition = new Vector3(xPos,yPos,0f);
			tc1.localRotation = Quaternion.AngleAxis(d1,Vector3.forward);
		}
	}
	public void VoidButtonJiantouFalse()
	{
		if (ButtonF != null) {
			NGUITools.SetActive(ButtonF, false);
		}
	}
	// 下一步响应 ....
	public void VoidButtonPage1(bool active, UIEventListener.VoidDelegate onClick)
	{
		if (BtnPage1 != null) {
			UIEventListener.Get(BtnPage1).onClick = onClick;
			NGUITools.SetActive(BtnPage1, active);
		}
	}
	// 描述 ...
	public void VoidSetManipIntro(float xPos, float yPos)
	{
		if (ManipIntro != null) {
			Transform myTrans = ManipIntro.transform;
			myTrans.localPosition = new Vector3(xPos, yPos, 0f);
		}
	}
	
	// 欢迎界面引导 ...
	void Frozen1Welcome()
	{
		if (BtnPage1 != null) {
			UIEventListener.Get(BtnPage1).onClick = VoidButtonPage1WelcomeDelegate;
			NGUITools.SetActive(BtnPage1, true);
		}
		if (Selector != null) {
			NGUITools.SetActive(Selector, false);
		}
	}
	
	void VoidButtonPage1WelcomeDelegate(GameObject go)
	{
		int c = m_guide_point + 1;
		LoadCStep_CP_1(c);
		UIEventListener.Get(go).onClick = VoidButtonPage1Delegate;
	}
	
	void VoidButtonPage1Delegate(GameObject go)
	{
		int imax = CommonMB.NewbieGuideInfo_Map.Count + 1;
		int c = Mathf.Min(imax, m_guide_point + 1);
		LoadCStep(c);
	}
	
	// 查看任务 ...
	void Frozen1Questbook()
	{
		NewbieQuestbook.mGuide = this;
		NewbieQuestbook.init1();
	}
	
	// 查看包裹 ...
	void Frozen1Backpack()
	{
		NewbieBackpack.mGuide = this;
		NewbieBackpack.init1();
	}
	
	// 民居 ...
	void Frozen1Housing(int c)
	{
		NewbieHouse.mGuide = this;
		if (c == 13)
		{
			NewbieHouse.init1();
		}
		else if (c == 16)
		{
			// 领取奖励 ...
			NewbieHouse.init2();
		}
		else if (c == 18 || c == 27)
		{
			// 升及 ...
			NewbieHouse.init3();
		}
	}
	// 主城堡 ...
	void Frozen1ChengBao()
	{
		NewbieChengBao.mGuide = this;
		NewbieChengBao.init1();
	}
	// 金矿引导 ...
	void Frozen1JinKuang(int c)
	{
		NewbieJinKuang.mGuide = this;
		
		if (c == 32)
		{
			NewbieJinKuang.init1();
		}
		else if (c == 34) 
		{
			NewbieJinKuang.init2();
		}
		else if (c == 38)
		{
			NewbieJinKuang.init3();
		}
	}
	// 研究院引导 ...
	void Frozen1YanJiuYuan()
	{
		NewbieYanJiuYuan.mGuide = this;
		NewbieYanJiuYuan.init1();
	}
	// 兵营引导 ...
	void Frozen1BingYing(int c)
	{
		NewbieBingYing.mGuide = this;
		if (c == 45)
		{
			NewbieBingYing.init1();
		}
		else if (c == 47)
		{
			NewbieBingYing.init2();
		}
	}
	// 酒馆引导 ...
	void Frozen1JiuGuan()
	{
		NewbieJiuGuan.mGuide = this;
		NewbieJiuGuan.init1();
	}
	// 将领引导 ...
	void Frozen1JingLing(int c)
	{
		NewbieJiangLing.mGuide = this;
		if (c == 55)
		{
			NewbieJiangLing.init1();
		}
		else if (c == 60)
		{
			NewbieJiangLing.init2();
		}
	}
	// 出征 ....
	void Frozen1ChuZheng()
	{
		NewbieChuZheng.mGuide = this;
		NewbieChuZheng.init1();
	}
	// 医馆 ....
	void Frozen1YiGuan()
	{
		NewbieYiGuan.mGuide = this;
		NewbieYiGuan.init1();
	}
	// 结束语 ...
	void Frozen1Ending()
	{
		NewbieEnding.mGuide = this;
		NewbieEnding.init1();
	}
	// 保存下一个存储点 ...
	public void GotoSavePoint_CP()
	{
		// 下一步节点引导 ...
		int imax = CommonMB.NewbieGuideInfo_Map.Count + 1;
		int c = Mathf.Min(imax, CP + 1);
		LoadCStep(c);
	}
	public void GotoCStep_CP()
	{
		int c = (m_guide_point+1);
		LoadCStep_CP_1(c);
	}
	public void GotoCStep_CP2(int next)
	{
		int c = (m_guide_point + next);
		LoadCStep_CP_1(c);
	}
	// 更新至下一步文字 ...
	void LoadCStep_CP_1(int c)
	{
		CMN_PLAYER_CARD_INFO player = CommonData.player_online_info;
		string gCol = string.Format(NEWBIE_GUIDE_1,player.AccountName);
		
		// 下一个点 ...
		uint cStep = (uint) c;
		Hashtable guideMap = CommonMB.NewbieGuideInfo_Map;
		if (true == guideMap.ContainsKey(cStep))
		{
			QuestGuideMBInfo info = (QuestGuideMBInfo) guideMap[cStep];
			if (DepictionCol != null)
			{
				DepictionCol.text = info.target;
				NGUITools.SetActiveSelf(DepictionCol.gameObject,true);
			}
			
			// 重新引导点 ...
			PlayerPrefs.SetInt(gCol, info.Savepoint);
		}
		
		m_guide_point = c;
	}
	//新手引导 拉起战斗动画  
	public void LoadBttleStep(int c)
	{
		if(c ==1)
		{
			uint cStep = 0;
			Hashtable guideMap = CommonMB.NewbieGuideInfo_Map;
			if (true == guideMap.ContainsKey(cStep))
			{
				QuestGuideMBInfo info = (QuestGuideMBInfo) guideMap[cStep];
				if (DepictionCol != null)
				{
					DepictionCol.text = info.target;
					//NGUITools.SetActiveSelf(DepictionCol.gameObject,true);
				}
				if (BtnPage1 != null) {
					UIEventListener.Get(BtnPage1).onClick = VoidStartBattle;
					NGUITools.SetActive(BtnPage1, true);
				}
				if (Selector != null) {
					NGUITools.SetActive(Selector, false);
				}
			}
		}
		else 
		{
			LoadCStep(c);
		}
	}
	void VoidStartBattle(GameObject go)
	{
		HeroUnit[] attack_heros = new HeroUnit[5];
		for(int i=0;i<5;i++)
		{
			attack_heros[i].nCol6 = (uint)i/5+1;
			attack_heros[i].nRow5 = (uint)i%5+1;
			attack_heros[i].nHeroID12 =(ulong)(i+1);
			attack_heros[i].nArmyLevel8 =2;
			attack_heros[i].nArmyType7 =(uint)i%5 +1;
			attack_heros[i].nArmyNum9 = 100; 
			attack_heros[i].nProf4 = 2000;
		}
		HeroUnit[] defense_heros = new HeroUnit[5];
		for(int i=0;i<5;i++)
		{
			defense_heros[i].nCol6 = (uint)i/5+1;
			defense_heros[i].nRow5 = (uint)i%5+1;
			defense_heros[i].nHeroID12 =(ulong)(i+1+10);
			defense_heros[i].nArmyLevel8 =1;
			defense_heros[i].nArmyType7 =(uint)i%5 +1;
			defense_heros[i].nArmyNum9 = 100; 
			defense_heros[i].nProf4 = 1000;
		}
		ComplexCombatLog battle_data = new ComplexCombatLog();
		battle_data.AttackEnhanceRatio = 1.2f;
		battle_data.CombatSeed = 1;
		battle_data.IsLeft = true;
		battle_data.IsWin = true;
		battle_data.AttackHeros = attack_heros;
		battle_data.DefenseHeros = defense_heros;
		battle_data.OnBattleFieldEnd = OnBattleEnd;
		BattleManager.Instance.ShowBattleWindow(battle_data,true,false);
	}
	void OnBattleEnd(bool isWin, bool isSkip)
	{
		LoadCStep(1);
	}
	// 更新下一步 ...
	public void LoadCStep( int c)
	{
		CMN_PLAYER_CARD_INFO player = CommonData.player_online_info;
		string gCol = string.Format(NEWBIE_GUIDE_1,player.AccountName);
		
		// Level ...
		uint cStep = (uint) c;
		Hashtable guideMap = CommonMB.NewbieGuideInfo_Map;
		if (true == guideMap.ContainsKey(cStep))
		{
			QuestGuideMBInfo info = (QuestGuideMBInfo) guideMap[cStep];
			if (DepictionCol != null)
			{
				DepictionCol.text = info.target;
				NGUITools.SetActiveSelf(DepictionCol.gameObject,true);
			}
			
			PlayerPrefs.SetInt(gCol, info.Savepoint);			
			m_guide_point = c;
		}
		else 
		{
			// 解除指引界面 ...
			PlayerPrefs.SetInt(gCol, c);
			
			// 提示发送聊天问候 ...
			Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
			uint canChatPop = 14;
			if (true == sMap.ContainsKey(canChatPop))
			{
				CmnDefineMBInfo newCard = (CmnDefineMBInfo) sMap[canChatPop];
				if (newCard.num == 1)
				{
					float depth = GetDepth() - BaizVariableScript.DEPTH_OFFSET;
					QuestGuideIcon.instance.PopChat(depth);
				}
				else if (newCard.num == 0)
				{
					PopFreeZhuanPan();
				}
			}
			
			// 指引结束 ....
			Destroy(gameObject);
			return;
		}
	
		if (c == 1)
		{
			Frozen1Welcome();
		}
		else if (c == 3) // 任务礼包 ...
		{
			Frozen1Questbook();
		}
		else if (c == 7) // 包果 ...
		{
			Frozen1Backpack();
		}
		else if (c == 13 || c == 16 || c == 18 || c == 27) // 民居引导 ...
		{
			Frozen1Housing(c);
		}
		else if (c == 23)
		{
			Frozen1ChengBao();
		}
		else if (c == 32 || c == 34 || c == 38)
		{
			Frozen1JinKuang(c);
		}
		else if (c == 41)
		{
			c = 45;
			//Frozen1YanJiuYuan();
			Frozen1BingYing(c);
		}
		else if (c == 45 || c == 47)
		{
			Frozen1BingYing(c);
		}
		else if (c == 50)
		{
			Frozen1JiuGuan();
		}
		else if (c == 55 || c == 60)
		{
			Frozen1JingLing(c);
		}
		else if (c == 63)
		{
			Frozen1ChuZheng();
		}
		else if (c == 71)
		{
			Frozen1YiGuan();
		}
		else if (c == 76)
		{
			Frozen1Ending();
		}
	}
	
}
