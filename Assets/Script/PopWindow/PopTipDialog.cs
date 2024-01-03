using UnityEngine;
using System.Collections;
using CMNCMD;
public class PopTipDialog : MonoBehaviour {
	
	static PopTipDialog mInst = null;
	
	public GameObject target = null;
	public GameObject Button1 = null;
	public GameObject Button2 = null;
	public GameObject isCollider = null;
	public UILabel Text1 = null;
	
	static Vector3 Pop1Left = new Vector3(-40f,0f,0f);
	static Vector3 Pop2Right = new Vector3(40f,0f,0f);
	
	UIAnchor mDepth = null;

	static public PopTipDialog instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(PopTipDialog)) as PopTipDialog;

				if (mInst == null)
				{
					GameObject objPrefab = (GameObject)Resources.Load("Prefab/PopupTipDialog",typeof(GameObject));
					if (objPrefab == null) return null;
					GameObject go = Instantiate(objPrefab) as GameObject;
					if (go != null)
					{
						go.name = "_PopTipInstance"; // 取个拉轰的名字   
						
						DontDestroyOnLoad(go);
						mInst = go.GetComponent<PopTipDialog>();
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
	
	
	void Depth(float depth)
	{
		if (mDepth == null) {
			mDepth = GetComponentInChildren<UIAnchor>();
		}
		
		if (mDepth == null) return;
		mDepth.depthOffset = depth;
	}
	
	void PopMode1(bool onButton1, bool onButton2)
	{
		if (Button1 != null) {
			Button1.SetActiveRecursively(onButton1);
			VoidButton1(VoidButton1Delegate);
			
			if (onButton2 == true) {
				Button1.transform.localPosition = Pop1Left;
			}
			else {
				Button1.transform.localPosition = Vector3.zero;
			}
		}
		
		if (Button2 != null) {
			Button2.SetActiveRecursively(onButton2);
			VoidButton2(VoidButton1Delegate);
			
			if (onButton1 == true) {
				Button2.transform.localPosition = Pop2Right;
			}
			else {
				Button2.transform.localPosition = Vector3.zero;
			}
		}
	}
	
	public void Mode1(bool onButton1, bool onButton2, int tipID)
	{		
		PopMode1(onButton1, onButton2);
		
		if (Text1 != null) {
			Text1.text = U3dCmn.GetWarnErrTipFromMB(tipID);
		}
	}
	
	public void VoidSetText1(bool onButton1, bool onButton2, int tipID)
	{
		PopMode1(onButton1, onButton2);
		
		if (Text1 != null) {
			Text1.text = U3dCmn.GetWarnErrTipFromMB(tipID);
		}

		Popup();
	}
	
	public void VoidSetText2(bool onButton1, bool onButton2, string text)
	{
		PopMode1(onButton1, onButton2);

		if (Text1 != null) {
			Text1.text = text;
		}

		Popup();
	}
	public void VoidSetText3(bool onButton1, bool onButton2, string text)
	{
		PopMode1(onButton1, onButton2);

		if (Text1 != null) {
			Text1.text = text;
		}
	}
	public void ShowPopWin(bool onButton1, bool onButton2, string text,UIEventListener.VoidDelegate on)
	{
		PopMode1(onButton1, onButton2);
		
		if (Text1 != null) {
			Text1.text = text;
		}
		if (Button1 != null) {
			UIEventListener _obj = UIEventListener.Get(Button1);
			if (_obj != null) _obj.onClick = on;
		}
		Popup();
	}
	public void VoidButton1(UIEventListener.VoidDelegate on)
	{
		if (Button1 != null) {
			UIEventListener _obj = UIEventListener.Get(Button1);
			if (_obj != null) _obj.onClick = on;
		}
	}
	
	public void VoidButton2(UIEventListener.VoidDelegate on)
	{
		if (Button2 != null) {
			UIEventListener _obj = UIEventListener.Get(Button2);
			if (_obj != null) _obj.onClick = on;
		}
	}
	
	void VoidButton1Delegate(GameObject go)
	{
		Dissband();
	}
	
	public void Popup()
	{
		if (target == null) return;

		float depth = BaizVariableScript.DEPTH_TOPMOST_OFFSET;
		Depth(depth);
		
		if (isCollider != null) {
			isCollider.SetActiveRecursively(true);
		}
		
		TweenPosition comp = TweenPosition.Begin(target, 0.2f, Vector3.zero);
		if (comp != null)
		{
			comp.from = new Vector3(0f,-400f,0f);
			comp.method = UITweener.Method.EaseOut;
			comp.eventReceiver = null;
			comp.Play(true);
		}
	}
	
	public void Popup1(float depth)
	{
		if (target == null) return;
		Depth(depth);
		
		if (isCollider != null) {
			NGUITools.SetActive(isCollider,false);
		}
		
		TweenPosition comp = TweenPosition.Begin(target, 0.2f, Vector3.zero);
		if (comp != null)
		{
			comp.from = new Vector3(0f,-400f,0f);
			comp.method = UITweener.Method.EaseOut;
			comp.eventReceiver = null;
			comp.Play(true);
		}
	}
	
	void TweenCooldown1()
	{		
		Depth(1f);
		
		if (isCollider != null) {
			NGUITools.SetActive(isCollider,false);
		}
	}
	
	public void Dissband()
	{
		if (target == null) return;
		TweenPosition comp = TweenPosition.Begin(target, 0.2f, new Vector3(0f,-400f,0f));
		if (comp != null) 
		{
			comp.method = UITweener.Method.EaseInOut;
			comp.eventReceiver = gameObject;
			comp.callWhenFinished = "TweenCooldown1";
			comp.Play(true); 
		}
		
		// 停止1秒 ... 
	}
	
}
