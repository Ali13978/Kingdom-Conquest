using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STCCMD;
using CTSCMD;
using CMNCMD;
// 武将布阵控制器 
public class BaizGeneralRoadWin : MonoBehaviour {
	
	PickingGeneralItem[] camps = new PickingGeneralItem[5];
	
	public UILabel RoadTipCol = null;
	
	public delegate void ApplyBaizCampDataDelegate(List<PickingGeneral> data);
	public static ApplyBaizCampDataDelegate applyBaizCampDataDelegate = null;
	public PagingStorage uiGrid = null;
	
	enum Method
	{
		Picking = 0,
		ArgsRoad = 1,
	}
	
	Method PickingStyle = Method.Picking;
	
	UIAnchor mDepth = null;
	
	void Awake() {
		if (RoadTipCol != null) 
		{
			RoadTipCol.enabled = false;
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
	}
	
	// 从自有武将初始化选将列表 ...
	public void AssignRoadGeneral()
	{
		PickingStyle = Method.Picking;
		
		if (uiGrid != null)
		{
			PickingGeneralManager.instance.ApplyHireGeneral();
			List<PickingGeneral> cacheList = PickingGeneralManager.instance.GetGeneralList();
			uiGrid.SetCapacity(cacheList.Count);
			uiGrid.ResetAllSurfaces();
			
			// 如果数量小0 ...
			if (cacheList.Count <= 0)
			{
				if (RoadTipCol != null) 
				{
					int Tipset = BaizVariableScript.INSTANCE_PICKING_GENERAL_NO_FREE;
					RoadTipCol.text = U3dCmn.GetWarnErrTipFromMB(Tipset);
					RoadTipCol.enabled = true;
				}
			}
		}
	}
	// 从副本武将初始化选将列表 ...
	public void AssignNzbzRoadGeneral()
	{
		PickingStyle = Method.ArgsRoad;
		
		if (uiGrid != null)
		{
			PickingGeneralManager.instance.ApplyNzbzGeneral();
			List<PickingGeneral> cacheList = PickingGeneralManager.instance.GetGeneralList();
			uiGrid.SetCapacity(cacheList.Count);
			uiGrid.ResetAllSurfaces();
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
	
	public bool OnActivateCampSlot(PickingGeneralItem theData)
	{
		PickingGeneral item = null;
		for (int i=0; i<5; ++ i)
		{
			PickingGeneralItem t = camps[i];
			if (t == null)
			{
				camps[i] = theData;
				theData.campid = (i+1);
				return true;
			}
			else if (t == theData)
			{				
				return false;
			}
			else 
			{
				item = PickingGeneralManager.instance.GetItem(t.gid);
				if (item == null)
				{
					// 空 
					camps[i] = theData;
					theData.campid = (i+1);
					return true;
				}
			}
		}
		
		return false;
	}
	
	public void OnDeactivateCampSlot(PickingGeneralItem theData)
	{		
		int firstid = 0;
		int i = 0;
		for (; i<5; ++ i)
		{
			PickingGeneralItem t = camps[i];
			if (t == null) return;
			if (t.gid == theData.gid)
			{	
				firstid = t.campid;
				if (PickingStyle == Method.ArgsRoad) {
					t.campid = 6;
				}
				else {
					t.campid = -1;
				}
				
				break;
			}
		}
				
		for (; i<4; ++ i)
		{
			PickingGeneralItem t = camps[i+1];
			if (t == null) break;
			t.campid = firstid ++;
			camps[i] = t;
		}
		
		if (i<5)
		{
			camps[i] = null;
		}
	}
	
	void OnBaizControllerClose()
	{
		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
	}
	
	void OnBaizControllerAccept()
	{
		if (applyBaizCampDataDelegate != null)
		{
			List<PickingGeneral> args = new List<PickingGeneral>();
			
			if (PickingStyle == Method.ArgsRoad)
			{
				int[] pickBool = new int[5] { 0,0,0,0,0 };
				for (int i=0; i<5; ++ i)
				{
					PickingGeneralItem t = camps[i];
					PickingGeneral item = null;
					if (t != null) 
					{
						item = PickingGeneralManager.instance.GetItem(t.gid);
						pickBool[t.gid] = 1;
					}
					if (item != null) {
						args.Add(item);
					}
				}
				
				for (int j=0; j<5; ++ j)
				{
					if (pickBool[j] == 0)
					{
						pickBool[j] = 1;
						PickingGeneral item = PickingGeneralManager.instance.GetItem(j);
						args.Add(item);
					}
				}
			}
			else
			{
				for (int i=0; i<5; ++ i)
				{
					PickingGeneralItem t = camps[i];
					PickingGeneral item = null;
					if (t != null)
					{
						item = PickingGeneralManager.instance.GetItem(t.gid);
					}
					
					args.Add(item);
				}
			}

			
			if (args.Count == 0) return;
			applyBaizCampDataDelegate(args);
		}
		
		applyBaizCampDataDelegate = null;
		
		// 删除自己吧 
		NGUITools.SetActive(gameObject,false);
		Destroy(gameObject);
	}
	
	
}
