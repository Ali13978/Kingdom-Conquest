using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;

public class AresRankCard
{
	public ulong		nAccountID;
	public string 		name;
	public uint			nLevel;
	public uint			nHeadID;
	public uint			nRank;
	public uint			nData;
}

public class BaizAresRankWin : MonoBehaviour {
	
	public GameObject template = null;
	public UIGrid uiGrid = null;
	public UIDraggablePanel rankPanel;
	public UILabel PagedRankCol = null;
	
	AresRankItem[] Ranks = new AresRankItem[10];
	public static int PAGED_RANK_NUM = 10;
	
	int nowPaged = 1;
	int TotalPagedNum = 1;
	
	int mAresRankType = (int)RANK_TYPE.rank_type_instance_wangzhe;
	
	public GameObject prevButton = null;
	public GameObject nextButton = null;
	UIAnchor mDepth = null;
	
	void Awake()
	{
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
		
		onCreateOrReposition();
	}
	
	void OnDestroy() { onExceptDelegate(); }
	void onExceptDelegate()
	{
		AresRankInstance.processAresRankCacheCB -= OnProcessAresRankCacheCBDelegate;
	}
	
	// Use this for initialization
	void Start () {
	
		if (prevButton != null) {
			UIEventListener.Get(prevButton).onClick = PrevPage;
		}
		if (nextButton != null) {
			UIEventListener.Get(nextButton).onClick = NextPage;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
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
	
	void onCreateOrReposition()
	{
		if (template != null)
		{
			for (int i=0; i<PAGED_RANK_NUM; ++ i)
			{
				GameObject go = NGUITools.AddChild(uiGrid.gameObject, template);
				Ranks[i] = go.GetComponent<AresRankItem>();
			}
			
			uiGrid.Reposition();
		}
	}
	
	public void ApplyBaizhanAresList()
	{
		mAresRankType = (int)RANK_TYPE.rank_type_instance_wangzhe;
		ResetBaizAresRanks();
	}
	
	void OnProcessAresRankCacheCBDelegate()
	{
		//print ("now:" + nowPaged + "," + TotalPagedNum);
		ResetBaizAresRanks();
	}
	
	void NextPage(GameObject go)
	{
		int paged = Mathf.Min(TotalPagedNum,nowPaged+1);
		if (nowPaged != paged)
		{
			nowPaged = paged;
			GetAresRankData( mAresRankType);
		}
	}
	
	void ResetBaizAresRanks()
	{
		List<AresRankCard> cacheList = AresRankInstance.GetAresRankList();
		int i=0, imax=cacheList.Count;
		for (i=0; i<imax; ++ i)
		{
			AresRankCard card = (AresRankCard) cacheList[i];
			Ranks[i].SendMessage("ApplyAresRankCard", card);
		}
		
		for (;i<PAGED_RANK_NUM; ++ i)
		{
			Ranks[i].ApplyAresRankCard(null);
		}
		
		TotalPagedNum = GetPagedNum(AresRankInstance.ARES_RANK_MAX_NUM);
		
		if (PagedRankCol != null)
		{
			PagedRankCol.text = string.Format("{0}/{1}",nowPaged,TotalPagedNum);
		}
		rankPanel.ResetPosition();
	}
	
	void PrevPage(GameObject go)
	{
		int paged = Mathf.Max(1,nowPaged-1);
		if (nowPaged != paged)
		{
			nowPaged = paged;
			GetAresRankData( mAresRankType);
		}
	}
	
	int GetPagedNum(int num)
	{
		int pagedNum = 1;
		if( num>0 )
		{
			pagedNum = (num-1)/PAGED_RANK_NUM+1;
		}
			
		return pagedNum;
	}
	
	void GetAresRankData(int rank_type)
	{
		AresRankInstance.processAresRankCacheCB = OnProcessAresRankCacheCBDelegate;
		RankManager.GetRankListInfo((uint)rank_type,(uint)((nowPaged-1)*PAGED_RANK_NUM),(uint)(PAGED_RANK_NUM));
	}
	
	void OnBaizhanRankClose()
	{
		NGUITools.SetActive(gameObject, false);
		Destroy(gameObject);
	}
}
