using UnityEngine;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class FriendWin : MonoBehaviour {
	const int item_num_max = 10;
	//窗口positon TWEEN     
	public TweenPosition tween_position; 
	//查找玩家名字 
	public UIInput player_name;
	//好友面板 
	public UICheckbox friend_tab;
	public GameObject friend_item;
	public UIDraggablePanel friend_panel;
	public UIGrid friend_grid;
	GameObject[] friend_item_array = new GameObject[item_num_max];
	//仇人面板  
	public UICheckbox enemy_tab;
	public GameObject enemy_item;
	public UIDraggablePanel enemy_panel;
	public UIGrid enemy_grid;
	GameObject[] enemy_item_array = new GameObject[item_num_max];
	//别人的请求信息面板   
	public UICheckbox others_apply_tab;
	public GameObject others_apply_item;
	public UIDraggablePanel others_apply_panel;
	public UIGrid others_apply_grid;
	GameObject[] others_apply_item_array = new GameObject[item_num_max];
	
	public UILabel page_num;
	int now_page_num;
	UIGrid now_grid = null;
	bool refresh = false;
	public static bool remind =  false;
	
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
			GameObject obj = NGUITools.AddChild(friend_grid.gameObject,friend_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			friend_item_array[i] = obj;
			
			obj = NGUITools.AddChild(enemy_grid.gameObject,enemy_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			enemy_item_array[i] = obj;
			
			obj = NGUITools.AddChild(others_apply_grid.gameObject,others_apply_item);
			obj.name = "item"+i;
			obj.SetActiveRecursively(false);
			others_apply_item_array[i] = obj;
		}
		now_page_num = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(refresh)
		{
			InitialPanelData();
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
		now_grid = null;
		tween_position.Play(false);
		Destroy(gameObject);
	}
	void ChangePanel()
	{
		now_page_num = 1;
		InitialPanelData();
	}
	void InitialPanelData()
	{
		
		//是否先显示好友申请界面 
		if(remind)
		{
			remind = false;
			others_apply_tab.isChecked = true;
		}
		
		if(friend_tab.isChecked)
		{
			friend_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(FriendManager.FriendDataList.Count,item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
			//ClearGrid(now_grid);
			
			
			//ArrayList list = new ArrayList(FriendManager.FriendMap.Keys);
      		//list.Sort();
			
			int from_num = (now_page_num-1)*item_num_max;
			int end_num = from_num+item_num_max>FriendManager.FriendDataList.Count?FriendManager.FriendDataList.Count:from_num+item_num_max;
			//print ("from_num"+from_num+"count"+FriendManager.FriendDataList.Count+"end num"+end_num);
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				//print ("");
				int index = FriendManager.FriendDataList.Count - n-1;
				FriendUnit unit = (FriendUnit)FriendManager.FriendDataList[index];
				
				friend_item_array[i].SetActiveRecursively(true);
				friend_item_array[i].SendMessage("InitialData",unit);
				i++;
			}
			for(;i<4;i++)
			{
				FriendUnit unit =new FriendUnit();
				friend_item_array[i].SetActiveRecursively(true);
				friend_item_array[i].SendMessage("InitialData",unit);
			}
			for(;i<item_num_max;i++)
			{
				friend_item_array[i].SetActiveRecursively(false);
			}
			friend_grid.Reposition();
			friend_panel.ResetPosition();
			page_num.text = now_page_num +"/"+max_page_num;
		}
		else if(enemy_tab.isChecked)
		{
			enemy_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(FriendManager.EnemyDataList.Count,item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
			//ClearGrid(now_grid);
			
			
			//ArrayList list = new ArrayList(FriendManager.FriendMap.Keys);
      		//list.Sort();
			
			int from_num = (now_page_num-1)*item_num_max;
			int end_num = from_num+item_num_max>FriendManager.EnemyDataList.Count?FriendManager.EnemyDataList.Count:from_num+item_num_max;
			//print ("from_num"+from_num+"count"+FriendManager.EnemyDataList.Count+"end num"+end_num);
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				//print ("");
				int index = FriendManager.EnemyDataList.Count - n-1;
				EnemyUnit unit = (EnemyUnit)FriendManager.EnemyDataList[index];
				
				enemy_item_array[i].SetActiveRecursively(true);
				enemy_item_array[i].SendMessage("InitialData",unit);
				i++;
			}
			for(;i<4;i++)
			{
				EnemyUnit unit =new EnemyUnit();
				enemy_item_array[i].SetActiveRecursively(true);
				enemy_item_array[i].SendMessage("InitialData",unit);
			}
			for(;i<item_num_max;i++)
			{
				enemy_item_array[i].SetActiveRecursively(false);
			}
			enemy_grid.Reposition();
			enemy_panel.ResetPosition();
			page_num.text = now_page_num +"/"+max_page_num;
		}
		else if(others_apply_tab.isChecked)
		{
			others_apply_panel.gameObject.SetActiveRecursively(true);
			int max_page_num = U3dCmn.GetPageNum(FriendManager.FriendApplyDataList.Count,item_num_max);
			if(now_page_num>max_page_num)
				now_page_num = max_page_num;
		
			
			int from_num = (now_page_num-1)*item_num_max;
			int end_num = from_num+item_num_max>FriendManager.FriendApplyDataList.Count?FriendManager.FriendApplyDataList.Count:from_num+item_num_max;
			int i=0;
			for(int n= from_num;n<end_num;n++)
			{
				//print ("");
				int index = FriendManager.FriendApplyDataList.Count - n-1;
				FriendApplyUnit unit = (FriendApplyUnit)FriendManager.FriendApplyDataList[index];
				
				others_apply_item_array[i].SetActiveRecursively(true);
				others_apply_item_array[i].SendMessage("InitialData",unit);
				i++;
			}
			for(;i<4;i++)
			{
				FriendApplyUnit unit =new FriendApplyUnit();
				others_apply_item_array[i].SetActiveRecursively(true);
				others_apply_item_array[i].SendMessage("InitialData",unit);
			}
			for(;i<item_num_max;i++)
			{
				others_apply_item_array[i].SetActiveRecursively(false);
			}
			others_apply_grid.Reposition();
			others_apply_panel.ResetPosition();
			page_num.text = now_page_num +"/"+max_page_num;
		}
	}
	//下一页 
	void NextPage()
	{
		if(friend_tab.isChecked)
		{
			if(now_page_num < U3dCmn.GetPageNum(FriendManager.FriendDataList.Count,item_num_max))
			{
				now_page_num++;
				InitialPanelData();
			}	
		}
		else if(enemy_tab.isChecked)
		{
			if(now_page_num < U3dCmn.GetPageNum(FriendManager.EnemyDataList.Count,item_num_max))
			{
				now_page_num++;
				InitialPanelData();
			}	
		}
		else if(others_apply_tab.isChecked)
		{
			if(now_page_num <  U3dCmn.GetPageNum(FriendManager.FriendApplyDataList.Count,item_num_max))
			{
				now_page_num++;
				InitialPanelData();
			}	
		}
	}
	//上一页 
	void PrevPage()
	{
	
		if(now_page_num > 1)
		{
			now_page_num--;
			InitialPanelData();
		}	
		
	}
	//查找玩家 
	void SearchPlayer()
	{
		if(player_name.text == null || player_name.text== "")
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NO_THIS_PALYER));
		}
		else 
		{
			OtherPlayerInfoManager.GetOtherPlayerCard(player_name.text);
			//注册回调 
			OtherPlayerInfoManager.RegisterCallBack(gameObject,"SearchPlayerRst");
		}
	}
	//查找玩家 
	void SearchPlayerRst(STC_GAMECMD_GET_PLAYERCARD_T player_card)
	{
		//注销回调 
		OtherPlayerInfoManager.UnRegisterCallBack(gameObject);
		if(player_card.nRst1 == (int)STC_GAMECMD_GET_PLAYERCARD_T.enum_rst.RST_OK)
		{
			GameObject win = U3dCmn.GetObjFromPrefab("PlayerCardWin"); 
			if(win != null)
			{
				PlayerCardWin.Counterattack = false;
				win.GetComponent<PlayerCardWin>().RevealPanelByParam(player_card);
			}	
		}
		else 
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NO_THIS_PALYER));
		}
		//STC_GAMECMD_GET_PLAYERCARD_T player_card
	}
}
