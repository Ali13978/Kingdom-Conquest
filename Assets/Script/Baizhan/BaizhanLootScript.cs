using UnityEngine;
using System.Collections.Generic;
using CMNCMD;
public class BaizhanLootScript : MonoBehaviour {
	
	public UILabel PrettyLabel = null;
	UIAnchor mDepth = null;
	
	public List<BaizInstanceLoot> mPrettyLoots = new List<BaizInstanceLoot>();
	public PagingStorage uiGrid = null;
	
	string mPrettyStr = "";
	int m_id = -1;
	
	// Use this for initialization
	void Awake ()
	{
		if (PrettyLabel != null)
		{
			mPrettyStr = PrettyLabel.text;
			PrettyLabel.text = "";
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
	
	
	public int gid 
	{
		
		get {		
			return m_id;
		}
		
		set {
			
			if (m_id != value)
			{
				m_id = value;
				
				if (PrettyLabel != null)
				{
					int front = (m_id-1)/10;
					if (front == 0) front = 1;
					front = (front + 1) * 10;
					PrettyLabel.text = string.Format(mPrettyStr,m_id,front);
				}
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
	
	public float GetDepth()
	{
		if (mDepth == null) return 0f;
		return mDepth.depthOffset;
	}
	
	void OnBaizGuardLootClose()
	{
		Destroy(gameObject);
	}
	
	public BaizInstanceLoot GetItem(int itemID)
	{
		if (itemID<0 || itemID> (mPrettyLoots.Count-1)) return null;
		return mPrettyLoots[itemID];
	}
	
	public void ApplyLootList(List<BaizInstanceLoot> cacheList)
	{
		mPrettyLoots.Clear();
		mPrettyLoots = cacheList;
		
		if (uiGrid != null)
		{
			uiGrid.SetCapacity(mPrettyLoots.Count);
		}
	}
	
}
