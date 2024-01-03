using UnityEngine;
using System.Collections;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class RankWin : MonoBehaviour {
	const int item_num_max = 10;
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 
	//我的排名 
	public UILabel my_rank_label;
	//等级排名TAB 
	public UICheckbox level_tab;
	//钻石排名TAB 
	public UICheckbox diamond_tab;
	//金币排名TAB 
	public UICheckbox gold_tab;
	//排名面板 
	public GameObject rank_item;
	public UIDraggablePanel rank_panel;
	public UIGrid rank_grid;
	GameObject[] rank_item_array = new GameObject[item_num_max];
	
	public UILabel page_num;
	int now_page_num;
	bool refresh = false;
	
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
			GameObject obj = NGUITools.AddChild(rank_grid.gameObject,rank_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			rank_item_array[i] = obj;
		}
		rank_grid.Reposition();
		now_page_num = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(refresh)
		{
			GetRankData();
			refresh = false;
		}
	}
	void RevealPanel()
	{
		now_page_num = 1;
		refresh = true;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		//now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	//面板更换 
	void ChangePanel()
	{
		now_page_num=1;
		GetRankData();
	}
	void GetRankData()
	{
		rank_panel.gameObject.SetActiveRecursively(false);
		uint rank_type =(uint)RANK_TYPE.rank_type_char_level; ;
		if(level_tab.isChecked)
			rank_type =  (uint)RANK_TYPE.rank_type_char_level;
		else if(diamond_tab.isChecked)
			rank_type =  (uint)RANK_TYPE.rank_type_char_diamond;
		else if(gold_tab.isChecked)
			rank_type =  (uint)RANK_TYPE.rank_type_char_gold;
		RankManager.GetRankListInfo(rank_type,(uint)((now_page_num-1)*item_num_max),(uint)(item_num_max));
	}
	void InitialPanelData()
	{
		rank_panel.gameObject.SetActiveRecursively(true);
		my_rank_label.text = RankManager.my_rank.ToString();
		if(now_page_num>GetPageNum((int)RankManager.total_num))
			now_page_num = GetPageNum((int)RankManager.total_num);
		
		int from_num = 0;
		int end_num = item_num_max;
		int i=0;
		//print ("nnnnnnnnnnnnnnnnnn"+RankManager.RankDataList.Count);
		for(int n= from_num;n<RankManager.RankDataList.Count;n++)
		{
			//print ("");
			//int index = RankManager.RankDataList.Count - n-1;
			CharRankInfo unit = (CharRankInfo)RankManager.RankDataList[n];
			
			rank_item_array[i].SetActiveRecursively(true);
			rank_item_array[i].SendMessage("InitialData",unit);
			i++;
		}
		for(;i<5;i++)
		{
			CharRankInfo unit =new CharRankInfo();
			rank_item_array[i].SetActiveRecursively(true);
			rank_item_array[i].SendMessage("InitialData",unit);
		}
		for(;i<item_num_max;i++)
		{
			rank_item_array[i].SetActiveRecursively(false);
		}
		rank_grid.Reposition();
		rank_panel.ResetPosition();
		page_num.text = now_page_num +"/"+GetPageNum((int)RankManager.total_num);
		
	}
	//下一页 
	void NextPage()
	{
		
		if(now_page_num < GetPageNum((int)RankManager.total_num))
		{
			now_page_num++;
			
		}
		GetRankData();	
		
	}
	//上一页 
	void PrevPage()
	{
	
		if(now_page_num > 1)
		{
			now_page_num--;
			
			//InitialPanelData();
		}	
		GetRankData();
	}

	//获取页数 
	int GetPageNum(int num)
	{
		int pagenum = 1;
		if(num>0)
			pagenum = (num-1)/item_num_max+1;
			
		return pagenum;
	}
}
