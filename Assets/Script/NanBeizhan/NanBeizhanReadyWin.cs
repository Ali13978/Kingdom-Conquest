using UnityEngine;
using System.Collections;
using CMNCMD;
public class NanBeizhanReadyWin : MonoBehaviour {
	
	public UILabel PrettyEnemyPowerCol = null;
	public UILabel PrettyHeroPowerCol = null;
	
	public GameObject Button1 = null;
	
	public int spacing = 80;
	public GameObject RoadGeneralCol = null;
	public GameObject template = null;
	UIAnchor mDepth = null;
	
	NanBeizhanGeneralItem[] RoadCol = new NanBeizhanGeneralItem[5];
	
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
		
		if (PrettyHeroPowerCol != null)
		{
			mPrettyHeroPower = PrettyHeroPowerCol.text;
			PrettyHeroPowerCol.enabled = false;
		}
		
		if (PrettyEnemyPowerCol != null)
		{
			mPrettyEnemyPower = PrettyEnemyPowerCol.text;
			PrettyHeroPowerCol.enabled = false;
		}
	
		onCreateOrReposition();
	}

	// Use this for initialization
	void Start () {
	}
	
	
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
				
				// 选择不了 ...
				Collider col = go.GetComponent<Collider>();
				if (col != null) col.enabled = false;
				
				// 当前位置 ...
				RoadCol[i] = t;
			}
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
		
		if (PrettyHeroPowerCol != null)
		{
			PrettyHeroPowerCol.text = string.Format(mPrettyHeroPower,totalProf);
			PrettyHeroPowerCol.enabled = true;
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
			if (PrettyEnemyPowerCol != null) {
				PrettyEnemyPowerCol.text = string.Format(mPrettyEnemyPower,obj.nProf);
				PrettyEnemyPowerCol.enabled = true;
			}
		}
	}
	
	public void VoidButton1(UIEventListener.VoidDelegate on)
	{
		if (Button1 != null) {
			UIEventListener onEvent = UIEventListener.Get(Button1);
			onEvent.onClick = on;
		}
	}
	
	public void OnNanBeizhanReadyClose()
	{
		gameObject.SetActiveRecursively(false);
		Destroy(gameObject);
	}
	
}
