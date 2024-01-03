using UnityEngine;
using System.Collections;

public class PopConfirmWin : MonoBehaviour {
	public TweenPosition tween_position; 
	static PopConfirmWin mInst = null;
	
	public GameObject Button1 = null;
	public GameObject Button2 = null;
	//public GameObject isCollider = null;
	public UILabel Text1 = null;
	
	UIAnchor mDepth = null;

	static public PopConfirmWin instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(PopConfirmWin)) as PopConfirmWin;

				if (mInst == null)
				{
					GameObject objPrefab = (GameObject)Resources.Load("Prefab/PopConfirmWin",typeof(GameObject));
					if (objPrefab == null) return null;
					GameObject go = Instantiate(objPrefab) as GameObject;
					if (go != null)
					{
						go.name = "PopConfirmWin"; // 取个拉轰的名字   
						
						DontDestroyOnLoad(go);
						mInst = go.GetComponent<PopConfirmWin>();
					}
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() { if (mInst == null) { mInst = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
	void OnDestroy() { if (mInst == this) mInst = null; }

	
	// Use this for initialization
	void Start () {
	
	}
	
	
	void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	
	
	
	
	
	public void ShowPopWin(string text,UIEventListener.VoidDelegate on)
	{
		if (Text1 != null) {
			Text1.text = text;
		}
		if (Button1 != null) {
			UIEventListener _obj = UIEventListener.Get(Button1);
			if (_obj != null) _obj.onClick = on;
		}
		Popup();
	}

	public void Dismiss()
	{
		tween_position.Play(false);
	}
	
	public void Popup()
	{
		tween_position.Play(true);
	}
	

	
	
	
}
