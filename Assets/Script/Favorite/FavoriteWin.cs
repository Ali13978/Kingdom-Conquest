using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;

public class FavoriteWin : MonoBehaviour {
	const int item_num_max = 10;
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 
	//排名面板 
	public GameObject favorite_item;
	public UIDraggablePanel favorite_panel;
	public UIGrid favorite_grid;
	GameObject[] favorite_item_array = new GameObject[item_num_max];
	
	public UILabel page_num;
	int now_page_num;
	int total_count = 0;
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
	}
	// Use this for initialization
	void Start () {
		for(int i=0;i<item_num_max;i++)
		{
			GameObject obj = NGUITools.AddChild(favorite_grid.gameObject,favorite_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			favorite_item_array[i] = obj;
		}
		favorite_grid.Reposition();
		now_page_num = 1;
	}
	
	void RevealPanel()
	{
		now_page_num = 1;
		FavoriteManager.Instance.GetFavoriteData();
		favorite_panel.gameObject.SetActiveRecursively(false);
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void InitialPanelData()
	{
		total_count = FavoriteManager.Instance.FavoriteDataList.Count;
		print ("tttttt"+total_count);
		favorite_panel.gameObject.SetActiveRecursively(true);
		if(now_page_num>U3dCmn.GetPageNum(total_count,item_num_max))
			now_page_num = U3dCmn.GetPageNum(total_count,item_num_max);
		
		int from_num = (now_page_num-1)*item_num_max;
		int end_num	= now_page_num*item_num_max;
		int i=0;
		//print ("nnnnnnnnnnnnnnnnnn"+RankManager.RankDataList.Count);
		for(int n= from_num;n<end_num;n++)
		{
			//print ("");
			//int index = RankManager.RankDataList.Count - n-1;
			if(n >= FavoriteManager.Instance.FavoriteDataList.Count)
				break;
			PositionMark unit = (PositionMark)FavoriteManager.Instance.FavoriteDataList[n];
			
			favorite_item_array[i].SetActiveRecursively(true);
			favorite_item_array[i].SendMessage("InitialData",unit);
			i++;
		}
		for(;i<5;i++)
		{
			PositionMark unit =new PositionMark();
			favorite_item_array[i].SetActiveRecursively(true);
			favorite_item_array[i].SendMessage("InitialData",unit);
		}
		for(;i<item_num_max;i++)
		{
			favorite_item_array[i].SetActiveRecursively(false);
		}
		favorite_grid.Reposition();
		favorite_panel.ResetPosition();
		page_num.text = now_page_num +"/"+U3dCmn.GetPageNum(total_count,item_num_max);
		
	}
	//下一页 
	void NextPage()
	{
		
		if(now_page_num < U3dCmn.GetPageNum(total_count,item_num_max))
		{
			now_page_num++;
			
		}
		InitialPanelData();	
		
	}
	//上一页 
	void PrevPage()
	{
	
		if(now_page_num > 1)
		{
			now_page_num--;
			
			//InitialPanelData();
		}	
		InitialPanelData();
	}
}
