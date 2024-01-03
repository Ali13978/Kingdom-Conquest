using UnityEngine;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;

public class GoldmineFi : MonoBehaviour {
	
	public GameObject template = null;
	
	GoldmineGuard[] mGoldmineSlots;
	
	public GoldmineScene Scene = null;
	
	string[] mGoldmineName;
	
	Vector3[] mMineArrange;
	
	void Awake() {
		onCreateOrReposition();
	}
	// Use this for initialization
	void Start () {
	}
	
	
	void onCreateOrReposition()
	{
		if (template != null)
		{
			mMineArrange 		= new Vector3[11];
			mMineArrange[0] 	= new Vector3(18f,  -1f, 	0f);
			mMineArrange[1] 	= new Vector3(98f,-55f,	0f);
			mMineArrange[2] 	= new Vector3(-60f,	50f,	0f);
			mMineArrange[3] 	= new Vector3(180f,	5f,		0f);
			mMineArrange[4] 	= new Vector3(105f,	55f,	0f);
			mMineArrange[5] 	= new Vector3(18f,	110f,	0f);
			mMineArrange[6] 	= new Vector3(-135f,85f,	0f);
			mMineArrange[7] 	= new Vector3(165f,-100f,	0f);
			mMineArrange[8] 	= new Vector3(-140f,-12f,	0f);
			mMineArrange[9] 	= new Vector3(-60f,	-68f,	0f);
			mMineArrange[10] 	= new Vector3(23f,	-116f,	0f);
		
			mGoldmineName 		= new string[11];
			mGoldmineName[0]	= "qianzh-zongdi";
			mGoldmineName[1] 	= "qianzh-tianzi";
			mGoldmineName[2] 	= "qianzh-tianzi";
			mGoldmineName[3] 	= "qianzh-renzi";
			mGoldmineName[4] 	= "qianzh-renzi";
			mGoldmineName[5] 	= "qianzh-renzi";
			mGoldmineName[6] 	= "qianzh-dizi";
			mGoldmineName[7] 	= "qianzh-dizi";
			mGoldmineName[8] 	= "qianzh-dizi";
			mGoldmineName[9] 	= "qianzh-dizi";
			mGoldmineName[10] 	= "qianzh-dizi";
			
			mGoldmineSlots = new GoldmineGuard[11];
			for(int i=0, imax=11; i<imax; ++ i)
			{
				GameObject go = NGUITools.AddChild(gameObject, template);
				go.transform.localPosition = mMineArrange[i];
				GoldmineGuard t = go.GetComponent<GoldmineGuard>();
				t.goldmineLabel.SetActiveRecursively(false);
				t.goldmineLevel.spriteId = t.goldmineLevel.GetSpriteIdByName(mGoldmineName[i]);
				t.gid = i;
				t.SetCScene(Scene);
				
				mGoldmineSlots[i] = t;
			}	
		}
	}
	
	public void ApplyGoldmineList(List<GoldmineCardData> cacheList)
	{
		for (int i=0, imax=cacheList.Count; i<imax; ++ i)
		{
			GoldmineGuard gx = mGoldmineSlots[i];
			if (gx == null) continue;
			
			GoldmineCardData newCard = cacheList[i];			
			gx.SetAcceptItem(newCard);
		}
	}
	
	public GoldmineGuard GetGuard(int nIdx)
	{
		if (nIdx<0 || nIdx>mGoldmineSlots.Length-1) return null;
		return mGoldmineSlots[nIdx];
	}
	
	public void Pop1MyGuard()
	{
		MyGoldmineData d1 = GoldmineInstance.instance.MyGoldmine;
		int firstid = 0;
		if (d1.nClass>1) {
			firstid = firstid + 1;
		}
		if (d1.nClass>2) {
			firstid = firstid + 2;
		}
		if (d1.nClass>3) {
			firstid = firstid + 3;
		}
				
		firstid = firstid + d1.nIdx - 1;
		GoldmineGuard gg = GetGuard(firstid);
		if (gg == null) return;
		gg.SendMessage("OnClick");
	}
	
}
