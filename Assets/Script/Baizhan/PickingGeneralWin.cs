using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
public class PickingGeneralWin : MonoBehaviour {
	
	public delegate void ApplyGeneralDelegate(PickingGeneral data);
	public static ApplyGeneralDelegate applyGeneralDelegate = null;
	public PagingStorage uiGrid = null;
	
	UIAnchor mDepth = null;
	
	void Awake()
	{
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
		
	}
	
	// 初始化武将选择面板 ...
	public void AssignPickingGeneral()
	{
		if (uiGrid != null)
		{
			PickingGeneralManager.instance.ApplyHireGeneral();
			List<PickingGeneral> cacheList = PickingGeneralManager.instance.GetGeneralList();
			uiGrid.SetCapacity(cacheList.Count);
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
	
	// 完成选择后执行回调, 退出面板 ...
	public void OnApplyGeneralItem(PickingGeneral item)
	{
		if (applyGeneralDelegate != null) {
			applyGeneralDelegate(item);
		}
		
		applyGeneralDelegate = null;
		
		// 结束返回 gameObject
		Destroy(gameObject);
	}
	
	public void OnPickingGeneralClose()
	{
		// 结束返回 gameObject
		Destroy(gameObject);
	}
}
