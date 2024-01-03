using UnityEngine;
using System.Collections;

public class GoldmineGuard : MonoBehaviour {
	
	public UILabel goldmineName = null;
	public GameObject goldmineLabel = null;	
	public tk2dSprite goldmineLevel = null;
	public tk2dSlicedSprite NameBgCol = null;
	GoldmineScene mCScene = null;
	
	int m_id = 0;
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate() 
	{
		GoldmineInstance.processGetMyGoldmineCon1 -= OnProcesssGetMyGoldmineGuardDelegate;
		GoldmineInstance.processGetWorldGoldmineDelegate -= OnProcessGetGoldmineGuardDelegate;
	}
	// Use this for initialization
	void Start () {
		
		if (NameBgCol != null) {
			NGUITools.SetActiveSelf(NameBgCol.gameObject,false);
		}
	}
	
	public int gid
	{
		get {
			return m_id;
		}
		
		set {
			m_id = value;
		}
	}
	
	
	public void SetAcceptItem(GoldmineCardData data)
	{
		if (data.nAccountID == 0)
		{
			goldmineLabel.SetActiveRecursively(false);
		}
		else 
		{
			goldmineLabel.SetActiveRecursively(true);
			if (goldmineName != null)
			{
				if (!string.IsNullOrEmpty(data.name))
				{
					NGUITools.SetActiveSelf(NameBgCol.gameObject, true);
					float MaxLen = Mathf.Min(100f, 13f*data.name.Length);
					NameBgCol.dimensions = new Vector2(MaxLen, 20f);
				}
				
				goldmineName.text = data.name;
			}
		}
	}
	
	public void SetCScene(GoldmineScene obj)
	{
		this.mCScene = obj;
	}
	
	void OnProcessGetGoldmineGuardDelegate()
	{
		LoadingManager.instance.HideLoading();
		GoldmineCardData newCard = GoldmineInstance.instance.GetItem(gid);
		if (newCard == null) return;
		
		GameObject go = U3dCmn.GetObjFromPrefab("GoldmineGuardWin");
		if (go == null) return;
		
		GoldmineGuardWin t = go.GetComponent<GoldmineGuardWin>();
		if (t != null)
		{
			float depth = mCScene.GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyGoldmineData(newCard);
		}
	}
	
	void OnProcesssGetMyGoldmineGuardDelegate()
	{
		LoadingManager.instance.HideLoading();
		MyGoldmineData newCard = GoldmineInstance.instance.MyGoldmine;
		
		GameObject go = U3dCmn.GetObjFromPrefab("GoldmineDefenseWin");
		if (go == null) return;
		
		GoldmineChuZhen t = go.GetComponent<GoldmineChuZhen>();
		if (t != null)
		{
			float depth = mCScene.GetDepth() - BaizVariableScript.DEPTH_OFFSET;
			t.Depth(depth);
			t.ApplyMyGoldmineData(newCard);
		}
	}
	
	void OnClick()
	{
		GoldmineCardData newCard = GoldmineInstance.instance.GetItem(gid);
		if (newCard == null) return;
		
		if (newCard.nAccountID == CommonData.player_online_info.AccountID)
		{
			LoadingManager.instance.ShowLoading();
			GoldmineInstance.processGetMyGoldmineCon1 = OnProcesssGetMyGoldmineGuardDelegate;
			GoldmineInstance.RequestGetMyGoldmine();
		}
		else 
		{
			// 查看 newcard
			LoadingManager.instance.ShowLoading();
			GoldmineInstance.processGetWorldGoldmineDelegate = OnProcessGetGoldmineGuardDelegate;
			GoldmineInstance.RequestGetWorldGoldmine(newCard.nAreaID, newCard.nClass, newCard.nIdx);
		}
		
	}
}
