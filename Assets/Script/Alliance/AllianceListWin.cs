using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class AllianceListWin : MonoBehaviour {
	const int item_num_max = 7;
	//消息窗口positon TWEEN     
	public TweenPosition tween_position; 
	//联盟列表面板 
	public UIDraggablePanel alliancelist_panel;
	public UIGrid alliancelist_grid;
	public GameObject 	alliance_item;
	GameObject[] alliance_item_array = new GameObject[item_num_max];
	
	public UISprite next_btn;
	public UISprite	pre_btn;
	public UILabel page_num;
	int now_page_num;
	
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
			GameObject obj = NGUITools.AddChild(alliancelist_grid.gameObject,alliance_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			alliance_item_array[i] = obj;
		}
		now_page_num = 1;
	}
	

	void RevealPanel()
	{
		now_page_num = 1;
		AllianceManager.ReqAllianceList(0,item_num_max-1);
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void InitialPanelData()
	{
		alliancelist_panel.gameObject.SetActiveRecursively(true);
	
		int from_num = 0;
		int end_num = AllianceManager.AllianceList.Count;
		int i=0;
		for(int n= from_num;n<end_num;n++)
		{
			alliance_item_array[i].SetActiveRecursively(true);
			alliance_item_array[i].SendMessage("InitialData",n);
			i++;
		}
		for(;i<6;i++)
		{
			alliance_item_array[i].SetActiveRecursively(true);
			alliance_item_array[i].SendMessage("InitialData",-1);
		}
		for(;i<item_num_max;i++)
		{
			alliance_item_array[i].SetActiveRecursively(false);
		}
		page_num.text = now_page_num +"/"+GetPageNum(AllianceManager.alliance_total_num);
		alliancelist_grid.Reposition();
		alliancelist_panel.ResetPosition();
		next_btn.gameObject.SetActiveRecursively(true);
		pre_btn.gameObject.SetActiveRecursively(true);
		page_num.gameObject.SetActiveRecursively(true);
	
	}
	//下一页 
	void NextPage()
	{
		if(now_page_num < GetPageNum(AllianceManager.alliance_total_num))
		{
			now_page_num++;
			AllianceManager.ReqAllianceList((now_page_num-1)*item_num_max,now_page_num*item_num_max-1);
			
		}	
	}
	//上一页 
	void PrevPage()
	{	
		if(now_page_num > 1)
		{
			--now_page_num;
			AllianceManager.ReqAllianceList((now_page_num-1)*item_num_max,now_page_num*item_num_max-1);
		}	
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
