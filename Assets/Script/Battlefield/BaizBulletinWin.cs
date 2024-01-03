using UnityEngine;
using System.Collections;
using CMNCMD;
// 活动介绍界面 ... 
public class BaizBulletinWin : MonoBehaviour {
	
	public UILabel awardLineCol = null;
	public UILabel modeLineCol = null;
	public UILabel norm1Col = null;
	public GameObject BtnClose = null;
	
	UIAnchor mDepth = null;
	
	void Awake() {
		if (awardLineCol != null) {
			awardLineCol.enabled = false;
		}
		if (modeLineCol != null) {
			modeLineCol.enabled = false;
		}
		if (norm1Col != null) {
			norm1Col.enabled = false;
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
	
	// Use this for initialization
	void Start () {
		
		if (BtnClose != null) {
			UIEventListener.Get (BtnClose).onClick = OnBulletinClose;
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
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// 填充公告内容 ...
	public void ApplyBulletin(int ExcelID)
	{
		Hashtable iaMap = CommonMB.InstanceMBInfo_Map;
		uint id = (uint) ExcelID;
		
		if (true == iaMap.ContainsKey(id))
		{
			InstanceMBInfo info = (InstanceMBInfo) iaMap[id];
			
			if (info.mode != null) {
				modeLineCol.text = info.mode;
				modeLineCol.enabled = true;
			}
			if (info.award != null) {
				awardLineCol.text = info.award;
				awardLineCol.enabled = true;
			}
			if (info.Rule1 != null) {
				norm1Col.text = info.Rule1;
				norm1Col.enabled = true;
			}
		}
	}
	
	void OnBulletinClose(GameObject go)
	{
		Destroy(gameObject);
	}
}
