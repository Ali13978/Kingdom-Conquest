using UnityEngine;
using System.Collections;

public class GoldmineDropWin : MonoBehaviour {
	
	public delegate void ProcessGoldmineDropAcceptDelegate();
	public ProcessGoldmineDropAcceptDelegate dropAcceptDelegate;
	
	UIAnchor mDepth = null;
	
	// Use this for initialization
	void Start () {
	
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
	
	public void OnGoldmineDropAccept()
	{
		if (dropAcceptDelegate != null) { dropAcceptDelegate(); }
		dropAcceptDelegate = null;
		
		// 删除自己 ...
		Destroy(gameObject);
	}
	
	public void OnGoldmineDropClose()
	{
		// 删除自己 ...
		Destroy(gameObject);
	}
}
