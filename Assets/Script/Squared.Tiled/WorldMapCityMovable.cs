using UnityEngine;
using System.Collections;
using STCCMD;
using CMNCMD;
public class WorldMapCityMovable : MonoBehaviour {
	
	public GameObject Button1 = null;
	public UILabel PosCol = null;
	public UILabel TipsetCol = null;
	UIAnchor mDepth = null;
	
	int mMapX = 0;
	int mMapY = 0;
	
	string mMapPS;
	
	void Awake() 
	{
		if (PosCol != null) {
			mMapPS = PosCol.text;
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
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		TiledStorgeCacheData.processCityMovableDelegate -= OnProcessCityMoveableDelegate;
	}
	
	// Use this for initialization
	void Start () {
		if (Button1 != null) {
			UIEventListener.Get(Button1).onClick = clickCityMovableDelegate;
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
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void clickCityMovableDelegate(GameObject go)
	{	
		LoadingManager.instance.ShowLoading();
		TiledStorgeCacheData.processCityMovableDelegate += OnProcessCityMoveableDelegate;
		TiledStorgeCacheData.RequestCityMoveTo(mMapX, mMapY);
	}
	
	void OnProcessCityMoveableDelegate()
	{
		LoadingManager.instance.HideLoading();
		CommonData.player_online_info.PosX = (uint)mMapX;
		CommonData.player_online_info.PosY = (uint)mMapY;
		
		TiledStorgeCacheData.instance.isCityMovable = false;
		TiledStorgeCacheData.instance.GPSDisbandUnit(mMapX,mMapY);
		
		Destroy(gameObject);
	}
	
	void OnCityMovableClose()
	{
		Destroy(gameObject);
	}
	
	public void ApplyCityMoveable(int mapX, int mapY)
	{
		mMapX = mapX;
		mMapY = mapY;
		
		if (PosCol != null) {
			PosCol.text = string.Format(mMapPS, mapX, mapY);
		}
		
		if (TipsetCol != null)
		{	
			ItemDesc desc = CangKuManager.GetItemInfoByExcelID(2004);
			int Tipset = BaizVariableScript.ITEM_CITY_MOVABLE_CONSUME_OK;
			if (desc.nNum3 == 0)
			{
				Tipset = BaizVariableScript.ITEM_CITY_MOVABLE_REEL_IS_NULL;
				if (Button1 != null) {
					NGUITools.SetActive(Button1, false);
				}
			}
			
			ITEM_INFO info = U3dCmn.GetItemIconByExcelID(2004);
			string cc = U3dCmn.GetWarnErrTipFromMB(Tipset);
			TipsetCol.text = string.Format(cc, info.Name);
		}
	}
}
