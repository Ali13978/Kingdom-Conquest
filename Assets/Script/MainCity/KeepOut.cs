using UnityEngine;
using System.Collections;
using CMNCMD;
public class KeepOut : MonoBehaviour {
	static KeepOut mInst = null;
	
	public static int byShown = 0;
	
	static public KeepOut instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(KeepOut)) as KeepOut;

				if (mInst == null)
				{
					GameObject go = new GameObject("KeepOut");
					mInst = go.AddComponent<KeepOut>();
				}
			}
			
			return mInst;
		}
	}
	public GameObject HeadInfoUI;
	//void Awake() { if (mInst == null) { mInst = this; } else { Destroy(gameObject); } }
	//void OnDestroy() { if (mInst == this) mInst = null; }
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void HideKeepOut()
	{
		byShown = Mathf.Max(0, byShown-1);
		
		//print ("- byShown:" + byShown);
		
		if (byShown == 0)
		{
			if(U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5)
			{
				transform.localPosition = new Vector3(0,10,-10);
				HeadInfoUI.transform.localPosition =  new Vector3(-44,0,-50);
			}
			
			//print ("hideKeepOut");
		}
	}
	
	public void ShowKeepOut()
	{
		if (byShown == 0)
		{
			if(U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5)
			{
				transform.localPosition = new Vector3(0,0,-10);
				HeadInfoUI.transform.localPosition =  new Vector3(0,0,-50);
			}
			
			//print ("showKeepOut");
		}
		
		++ byShown;
		
		//print ("+ byShown:" + byShown);
	}
	
}
