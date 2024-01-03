using UnityEngine;
using System.Collections;
using CMNCMD;
public class WorldMapLocate : MonoBehaviour {
	
	public GameObject Button1 = null;
	public UILabel xPosCol = null;
	public UILabel yPosCol = null;
	UIAnchor mDepth = null;
	
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
	}
	// Use this for initialization
	void Start () {
	
		if (Button1 != null) {
			UIEventListener.Get(Button1).onClick = VoidGPSDelegate;
		}
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
	
	void VoidGPSDelegate(GameObject go)
	{
		int xPOS = -1;
		if (xPosCol != null) 
		{
			string s = xPosCol.text;
			if (false == int.TryParse(s,out xPOS)) return;
		}
		
		int yPOS = -1;
		if (yPosCol != null)
		{
			string s = yPosCol.text;
			if (false == int.TryParse(s,out yPOS)) return;
		}
		
		TiledStorgeCacheData.instance.GPSSetLocation(xPOS, yPOS);
		
		// 关闭
		this.OnTerrainGPSClose();
	}
	
	void OnTerrainGPSClose()
	{
		// 删除 ...
		Destroy(gameObject);
	}
}
