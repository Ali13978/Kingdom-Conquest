using UnityEngine;
using System.Collections;
using CMNCMD;

public class TiledMapInIPad : MonoBehaviour {
	
	public Transform TopBar = null;
	public Transform BottomBar = null;
	
	void Awake() {
	
		float OffsetY = 0f, OffsetX = 0f;
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
			if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5)
			{
				OffsetX = 44f;
			}
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			OffsetY = 20f;
			if (root != null) { root.manualHeight = 360; }
		}
		
		if (TopBar != null)
		{
			TopBar.localPosition += new Vector3(OffsetX,OffsetY,0f);
		}
		if (BottomBar != null)
		{
			BottomBar.localPosition += new Vector3(OffsetX,-OffsetY,0f); 
		}
	}
}
