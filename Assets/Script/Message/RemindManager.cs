using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
public class RemindManager : MonoBehaviour {
	struct Remind
	{
		public string 		name;
		public byte			type;	
		public byte			visible;
		public GameObject 	obj;
		
	}
	const int remind_num = 4;
	Remind[] remind_array = new Remind[remind_num];
	public static List<byte> remind_list = new List<byte>();
	
	void Awake()
	{
	 	if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
			transform.localPosition = transform.localPosition+new Vector3(0,20*CommonData.ipadscale.y,0);
		}
		else if(U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5)
		{
			transform.localPosition = transform.localPosition+new Vector3(-40*CommonData.ipadscale.x,0,0);
		}
	}
	// Use this for initialization
	void Start () {
		
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>(true);
		int index = 0;
		for(int i=0;i<remind_num;i++)
		{
			GameObject obj =  U3dCmn.GetChildObjByName(gameObject,"remind"+(i+1));
			
			obj.SetActiveRecursively(false);
			UIEventListener.Get(obj).onClick -= Operate;
			UIEventListener.Get(obj).onClick += Operate;
			remind_array[i].name = "remind"+(i+1);
			remind_array[i].obj = obj;
			remind_array[i].type = 0;
			remind_array[i].visible = 0;
			
		}
		StopCoroutine("CheckRemind");
		StartCoroutine("CheckRemind");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//每两秒检查一下是否有各种提醒   
	IEnumerator CheckRemind()
	{
		//int sec = (int)(EndTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		
		while(true)
		{
			bool refresh = false;
			if(FriendManager.FriendApplyDataList.Count > 0)
			{
				if(!remind_list.Contains((byte)REMIND_TYPE.FRIEND_APPLY))
				{
					remind_list.Add((byte)REMIND_TYPE.FRIEND_APPLY);
					refresh = true;
				}
			}	
			else 
			{
				if(remind_list.Contains((byte)REMIND_TYPE.FRIEND_APPLY))
				{
					remind_list.Remove((byte)REMIND_TYPE.FRIEND_APPLY);
					refresh = true;
				}
			}	
			if(BuildingManager.GetRipeJinKuangNum()>0)
			{
				if(!remind_list.Contains((byte)REMIND_TYPE.GOLD_RIPE))
				{
					remind_list.Add((byte)REMIND_TYPE.GOLD_RIPE);
					refresh = true;
				}
			}
			else 
			{
				if(remind_list.Contains((byte)REMIND_TYPE.GOLD_RIPE))
				{
					remind_list.Remove((byte)REMIND_TYPE.GOLD_RIPE);
					refresh = true;
				}
			}
			//检查一下是否有公告信息 
			MessageManager.CheckUnreadAnnounce();
			/*if(MessageManager.CheckUnreadAnnounce()>0)
			{
				if(!remind_list.Contains((byte)REMIND_TYPE.ANNOUNCE_REMIND))
				{
					remind_list.Add((byte)REMIND_TYPE.ANNOUNCE_REMIND);
					refresh = true;
				}
			}
			else 
			{
				if(remind_list.Contains((byte)REMIND_TYPE.ANNOUNCE_REMIND))
				{
					remind_list.Remove((byte)REMIND_TYPE.ANNOUNCE_REMIND);
					refresh = true;
				}
			}*/
			if(AllianceManager.AllianceApplyList.Count>0)
			{
				if(!remind_list.Contains((byte)REMIND_TYPE.ALLIANCE_APPLY))
				{
					remind_list.Add((byte)REMIND_TYPE.ALLIANCE_APPLY);
					refresh = true;
				}
			}
			else 
			{
				if(remind_list.Contains((byte)REMIND_TYPE.ALLIANCE_APPLY))
				{
					remind_list.Remove((byte)REMIND_TYPE.ALLIANCE_APPLY);
					refresh = true;
				}
			}
			
			if(refresh)
			{
				RefreshRemindIcon();
			}
			yield return new WaitForSeconds(2);
		}
	}
	//刷新提醒图标  
	void RefreshRemindIcon()
	{
		//Remind remind = new Remind();
		for(int i=0;i<remind_num;i++)
		{
			//remind =  remind_array[i];
			remind_array[i].obj.SetActiveRecursively(false);
			//remind_array[i].name="";
			remind_array[i].visible = 0;
			remind_array[i].type = 0;	
		}
		int num = remind_list.Count;
		//print ("hhhhhhhhhh"+num);
		for(int i=0;i<num;i++)
		{
			remind_array[i].type = (byte)remind_list[i];
			remind_array[i].visible = 1;
			remind_array[i].obj.GetComponent<UISprite>().spriteName = GetIconName((byte)remind_list[i]);
			remind_array[i].obj.SetActiveRecursively(true);
		}
	}
	string GetIconName(byte type)
	{
		string name = "";
		if(type == (byte)REMIND_TYPE.ANNOUNCE_REMIND)
		{
			name = "laba";
		}
		else if(type == (byte)REMIND_TYPE.FRIEND_APPLY)
		{
			name = "haoyoutixing";
		}
		else if(type == (byte)REMIND_TYPE.GOLD_RIPE)
		{
			name = "jinbitixing";
		}
		else if(type == (byte)REMIND_TYPE.ALLIANCE_APPLY)
		{
			name = "lianmengtishi";
		}
		return name;
	}
	//点击图标打开相应操作界面 
	void Operate(GameObject obj)
	{
		byte type = GetTypeByObjName(obj.name);
		if(type == (byte)REMIND_TYPE.ANNOUNCE_REMIND)
		{
			GameObject win  = U3dCmn.GetObjFromPrefab("MessageWin");
			if (win != null)
			{
				win.SendMessage("RevealPanel");
			}
		}
		else if(type == (byte)REMIND_TYPE.FRIEND_APPLY)
		{
			GameObject win  = U3dCmn.GetObjFromPrefab("FriendWin");
			if (win != null)
			{
				FriendWin.remind = true;
				win.SendMessage("RevealPanel");
			}
		}
		else if(type == (byte)REMIND_TYPE.GOLD_RIPE)
		{
			U3dCmn.SendMessage("BuildingManager","RevealJinKuangPanel",null);
		}
		else if(type == (byte)REMIND_TYPE.ALLIANCE_APPLY)
		{
			
			GameObject win  = U3dCmn.GetObjFromPrefab("AllianceWin");
			if (win != null)
			{
				win.SendMessage("RevealPanel");
			}
			AllianceWin.open_apply = true;
		}
	}
	//根据OBJ NAME获取提醒类型 
	byte GetTypeByObjName(string name)
	{
		byte rst =0;
		for(int i= 0;i<remind_num;i++)
		{
			if(remind_array[i].name == name)
			{
				rst = remind_array[i].type;
			}
		}
		return rst;
	}
	
	//检查是否已经为这个事件设置了提醒图标 
	/*bool CheckRemindVisible(byte type)
	{
		bool rst = false;
		for(int i= 0;i<remind_num;i++)
		{
			if(remind_array[i].type == type)
			{
				rst = true;
			}
		}
		return rst;
	}*/
}
