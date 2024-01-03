using UnityEngine;
using System.Collections;
using CMNCMD;
public class BattlefieldWin : MonoBehaviour {
	
	UIAnchor mDepth = null;
	public GameObject HealthBar = null;
	
	void Awake()
	{
		// IPAD 适配尺寸 ...
		float OffsetY = 0f;
		UIRoot root = GetComponent<UIRoot>();
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (root != null) { root.manualHeight = 320; }
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			OffsetY = 20f;
			if (root != null) { root.manualHeight = 360; }
		}
		
		if (HealthBar != null)
		{
			Transform myTrans = HealthBar.transform;
			Vector3 pos = myTrans.localPosition;
			myTrans.localPosition = new Vector3(pos.x, pos.y + OffsetY, pos.z);
		}
	}

	// Use this for initialization
	void Start () {
		if (SoundManager.palying_music == CMNCMD.BG_MUSIC.MAINSCENE_MUSIC)
		{
			SoundManager.StopMainSceneSound();
		}
		
		// iphone5 遮挡拉来 ...
		KeepOut.instance.ShowKeepOut();
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
	
	
	void OnBaizBattlefieldClose()
	{
		if (SoundManager.palying_music == CMNCMD.BG_MUSIC.BATTLE_MUSIC)
		{
			SoundManager.StopBattleSound();
			SoundManager.PlayMainSceneSound();
		}
		
		
		// iphone5 挪开 ...
		KeepOut.instance.HideKeepOut();
		
		// 删除 ...
		Destroy(gameObject);
		
		Resources.UnloadUnusedAssets();
	}
}
