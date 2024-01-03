using UnityEngine;
using System.Collections;
using CMNCMD;
// 踢出队友功能界面 ...
public class NanBeizhanKickWin : MonoBehaviour {
	
	public UILabel EnemyPowerCol = null;			// 副本战斗力 ...
	public UILabel HeroPowerCol = null;				// 队伍战斗力 ...
	
	public GameObject Button1 = null;
	public GameObject BtnClose = null;				// 关闭按钮 ...
	
	public UISprite Button1gnd = null;
	
	public int spacing = 80;
	public GameObject RoadGeneralCol = null;		// 队伍成员 ...
	public GameObject template = null;
	UIAnchor mDepth = null;
	
	int myGenTotalProf = 0;
	
	NanBeizhanGeneralItem[] RoadCol = new NanBeizhanGeneralItem[5];
	NanBeizhanGeneralItem myCurSelect = null;
	
	string mPrettyHeroPower;
	string mPrettyEnemyPower;
	
	void Awake() {
		
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
		
		if (HeroPowerCol != null)
		{
			mPrettyHeroPower = HeroPowerCol.text;
			HeroPowerCol.enabled = false;
		}
		
		if (EnemyPowerCol != null)
		{
			mPrettyEnemyPower = EnemyPowerCol.text;
			EnemyPowerCol.enabled = false;
		}
		
		onCreateOrReposition();
	}
	
	void OnDestroy() {
		BaizInstanceManager.processKickInstanceHeroDelegate -= OnNanBeizhanKickClose;
	}
	
	void Start() {
		if (Button1 != null) {
			UIEventListener.Get(Button1).onClick = clickButtonKickDelegate;
			Button1.GetComponent<Collider>().enabled = false;
		}
		
		if (BtnClose != null) {
			UIEventListener.Get(BtnClose).onClick = clickBtnCloseDelegate;
		}
		
		if (Button1gnd != null) {
			Button1gnd.spriteName = "button1_disable";
		}
	}
	
	// 分布当前角色 ...
	void onCreateOrReposition()
	{
		if (template != null)
		{
			float[] fCol = {-2f,-1f,0,1f,2f};
			RoadCol = new NanBeizhanGeneralItem[5];
			for(int i=0, imax=5; i<imax; ++ i)
			{
				GameObject go = NGUITools.AddChild(RoadGeneralCol, template);
				go.transform.localPosition = new Vector3(fCol[i] * spacing, 0f, 0f);
				NanBeizhanGeneralItem t = go.GetComponent<NanBeizhanGeneralItem>();
				t.AllyID = i;
				
				// 加个选择项 ...
				UIEventListener.Get(go).onClick = OnButtonSelectLeaveins;
				
				RoadCol[i] = t;
			}
		}
	}
	
	void OnButtonSelectLeaveins(GameObject tween)
	{
		NanBeizhanGeneralItem item = tween.GetComponent<NanBeizhanGeneralItem>();
		if (item == null) return;
		
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		PickingGeneral gen = args[item.AllyID];
		if (gen == null) return;
		ulong myAcctID = CommonData.player_online_info.AccountID;
		if (gen.nAccountID == myAcctID) return;
		
		// 重新计算总战力 ...
		int totalProf = myGenTotalProf;
		if (myCurSelect != item) 
		{
			if (myCurSelect != null) {
				myCurSelect.TurnLeaveins();
			}
			
			myCurSelect = item;
			item.TurnLeaveins();
			
			// 可以踢出了 ...
			if (Button1 != null) {
				Button1.GetComponent<Collider>().enabled = true;
				Button1gnd.spriteName = "button1";
			}
			
			totalProf -= gen.nProf;
		}
		else 
		{
			myCurSelect.TurnLeaveins();
			myCurSelect = null;
			
			// 可以踢出了 ...
			if (Button1 != null) {
				Button1.GetComponent<Collider>().enabled = false;
				Button1gnd.spriteName = "button1_disable";
			}
		}
		
		if (HeroPowerCol != null)
		{
			HeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
			HeroPowerCol.enabled = true;
		}
	}
	
	public void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	public void ApplyRoadGeneral()
	{
		if (RoadCol == null) return;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		
		int totalProf = 0;
		int i=0;
		
		for(i=0; i<args.Length; ++ i)
		{
			PickingGeneral t = args[i];
			if (t != null) {
				totalProf = totalProf + t.nProf;
			}
			
			RoadCol[i].ApplyGeneralItem(t);
		}
		
		myGenTotalProf = totalProf;
		
		if (HeroPowerCol != null)
		{
			HeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
			HeroPowerCol.enabled = true;
		}
	}
	
	public void ApplyEnemyProf()
	{
		// 战斗力 ...
		uint nExcelID = (uint) NanBeizhanInstance.instance.idClass;
		Hashtable nzbzMap = CommonMB.NanBeizhanInfo_Map;
		if (true == nzbzMap.ContainsKey(nExcelID))
		{
			NanBeizhanMBInfo obj = (NanBeizhanMBInfo) nzbzMap[nExcelID];
			if (EnemyPowerCol != null) {
				EnemyPowerCol.text = string.Format(mPrettyEnemyPower,obj.nProf);
				EnemyPowerCol.enabled = true;
			}
		}
	}
	
	public void clickButtonKickDelegate(GameObject tween)
	{
		ulong nInstanceID = NanBeizhanInstance.instance.idInstance;
		if (nInstanceID == 0) return;
		
		if (myCurSelect == null) return;
		PickingGeneral[] args = NanBeizhanInstance.instance.GetGenerals();
		PickingGeneral gen = args[myCurSelect.AllyID];
		if (gen == null) return;
		
		BaizInstanceManager.processKickInstanceHeroDelegate = OnNanBeizhanKickClose;
		BaizInstanceManager.RequestKickInstanceHero(101, nInstanceID, gen.nAccountID);
	}
	
	void OnNanBeizhanKickClose()
	{
		gameObject.SetActiveRecursively(false);
		Destroy(gameObject);
	}
	
	public void clickBtnCloseDelegate(GameObject tween)
	{
		Destroy(gameObject);
	}
}
