using UnityEngine;
using System.Collections.Generic;
using CMNCMD;
public class BaizhanDeathWin : MonoBehaviour {
		
	public GameObject template = null;
	public GameObject heroBoard = null;
	public GameObject enemyBoard = null;
	public UILabel winTitle = null;
	
	public UILabel heroProfCol = null;
	public UILabel enemyProfCol = null;
	
	UIAnchor mDepth = null;
	
	string mPrettyHeroProf;
	string mPrettyEnemyProf;
	
	void Awake() 
	{
		if (heroProfCol != null) {
			mPrettyHeroProf = heroProfCol.text;
			heroProfCol.text = string.Format(mPrettyHeroProf,0);
		}
		if (enemyProfCol != null) {
			mPrettyEnemyProf = enemyProfCol.text;
			enemyProfCol.text = string.Format(mPrettyEnemyProf,0);
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
	
	void OnBaizCasualtyClose()
	{
		gameObject.SetActiveRecursively(false);
		Destroy(gameObject);
	}
 
	public void SetTitle(string name)
	{
		if (winTitle != null) {
			winTitle.text = name;
		}
	}
	
	public void DeathDataCleanUp()
	{
		if (this.heroBoard != null)
		{
			Transform myTrans = this.heroBoard.transform;
			int childCount = myTrans.childCount;
				
			for (int i=0; i<childCount; ++ i)
			{
				Transform child = myTrans.GetChild(i);
				NGUITools.SetActive(child.gameObject,false);
				Destroy(child.gameObject);
			}
		}
			
		if (this.enemyBoard != null)
		{
			Transform myTrans = this.enemyBoard.transform;
			int childCount = myTrans.childCount;
			
			for (int i=0; i<childCount; ++ i)
			{
				Transform child = myTrans.GetChild(i);
				NGUITools.SetActive(child.gameObject,false);
				Destroy(child.gameObject);
			}
		}
	}
	
	public void ApplyDeathData(List<COMBAT_DEATH_INFO> cacheList, int heroNum, int enemyNum, int heroProf, int enemyProf)
	{
		if (template != null)
		{
			float spacing = 13f;
			if (heroNum>0)
			{
				Vector3 loc = heroBoard.transform.localPosition;
				for (int i=0; i<heroNum; ++i)
				{
					COMBAT_DEATH_INFO data = cacheList[i];
					GameObject go = NGUITools.AddChild(heroBoard, template);
					Transform t = go.transform;
					t.localPosition = new Vector3(0, - (i+0.5f) * spacing, loc.z);
							
					BaizDeathItem item = go.GetComponent<BaizDeathItem>();	
					item.SetDeathData(data);
				}
				
				if (heroProfCol != null) {
					heroProfCol.text = string.Format(mPrettyHeroProf, heroProf);
				}
			}
			
			
			if (enemyNum>0)
			{
				Vector3 loc = enemyBoard.transform.localPosition;
				for (int i=0; i<enemyNum; ++ i)
				{
					COMBAT_DEATH_INFO data = cacheList[i+heroNum];
					GameObject go = NGUITools.AddChild(enemyBoard, template);
					Transform t = go.transform;
					t.localPosition = new Vector3(0, - (i+0.5f) * spacing, loc.z);
							
					BaizDeathItem item = go.GetComponent<BaizDeathItem>();	
					item.SetDeathData(data);
				}
				
				if (enemyProfCol != null) {
					enemyProfCol.text = string.Format(mPrettyEnemyProf, enemyProf);
				}
			}
		}
	}
	
}
