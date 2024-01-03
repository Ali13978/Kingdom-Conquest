using UnityEngine;
using System.Collections;
using CMNCMD;
public class QuestGuideIcon : MonoBehaviour {
	
	static QuestGuideIcon mInst = null;	
	int mGuidePoint = 0;

	/// <summary>
	/// The instance of the CombatManager class. Will create it if one isn't already around.
	/// </summary>

	static public QuestGuideIcon instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(QuestGuideIcon)) as QuestGuideIcon;

				if (mInst == null)
				{
					GameObject objPrefab = (GameObject) Resources.Load("Prefab/QuestGuideIcon", typeof(GameObject));
					if (objPrefab == null) return null;
					GameObject go = Instantiate(objPrefab) as GameObject;
					if (go != null) 
					{
						go.name = "_QuestGuideIcon"; // 取个拉轰的名字...
						
						DontDestroyOnLoad(go);
						mInst = go.GetComponent<QuestGuideIcon>();
					}
				}
			}
			
			return mInst;
		}
	}
	
	void Awake() 
	{
		
		if (mInst == null) 
		{ 
			mInst = this; 
			DontDestroyOnLoad(gameObject); 
		}
		else 
		{ 
			Destroy(gameObject); 
		} 
	}
	void OnDestroy() { onExceptDelegate(); if (mInst == this) mInst = null; }
	void onExceptDelegate()
	{
		
	}
	
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
		
		// 暂时设置为不可风 ... 
	}
	public static bool CheckQuestOver()
	{
		CMN_PLAYER_CARD_INFO player = CommonData.player_online_info;
		string gCol = string.Format(NewbieGuide.NEWBIE_GUIDE_1,player.AccountName);
		int c = Mathf.Max(1,PlayerPrefs.GetInt(gCol));
		return (false == instance.needGuide_1 (c));
	}
	
	//  新手报到输入一句话....
	public void PopChat(float depth)
	{
		GameObject go = U3dCmn.GetObjFromPrefab("PopWriteChatMsgWin");
		if (go == null) return;
		
		PopWriteChatMsg win1 = go.GetComponent<PopWriteChatMsg>();
		if (win1 != null)
		{
			win1.Popup1(depth);
		}
	}
	
	bool needGuide_1(int c)
	{
		uint cStep = (uint) c;
		Hashtable gzMap = CommonMB.NewbieGuideInfo_Map;
		return (gzMap.ContainsKey(cStep) );
		// Fini
	}
	
	public void StartNewbieGuide()
	{
		CMN_PLAYER_CARD_INFO player = CommonData.player_online_info;
		string gCol = string.Format(NewbieGuide.NEWBIE_GUIDE_1,player.AccountName);
		int c = Mathf.Max(1,PlayerPrefs.GetInt(gCol));
		if (true == needGuide_1(c))
		{
			mGuidePoint = c;
			
			GameObject go = U3dCmn.GetObjFromPrefab("NewbieGuideWin");
			if (go == null ) return;
	
			NewbieGuide win1 = go.GetComponent<NewbieGuide>();
			if (win1 != null)
			{
				float depth = -BaizVariableScript.DEPTH_OFFSET * 3.0f;
				win1.Depth(depth);
				win1.LoadBttleStep(mGuidePoint);
			}
		}
	}
}
