using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class JinKuangFriendListWin : MonoBehaviour {
	//好友列表窗口  
	public TweenPosition 	friend_list_position;
	bool  friend_list_show = false;
	//好友列表GRID 
	public GameObject friend_list_grid;
	//好友列表ITEM数量 
	const int FriendItemNum = 10;
	//页数 
	int now_page_num;
	//页数LABEL 
	public UILabel page_label;
	//好友列表 
	List<JinKuangFriendItem>FriendList = new List<JinKuangFriendItem>();
	
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
			transform.localPosition = new Vector3(0.83f,transform.localPosition.y,transform.localPosition.z);
			gameObject.GetComponent<TweenPosition>().from = new Vector3(0.83f,transform.localPosition.y,transform.localPosition.z);
		}
	}
	// Use this for initialization
	void Start () {
		FriendList.Clear();
		//初始化好友列表 
		for(int i=0;i<FriendItemNum;i++)
		{
			JinKuangFriendItem item = U3dCmn.GetChildObjByName(friend_list_grid.gameObject,"FriendItem"+i).GetComponent<JinKuangFriendItem>();
			FriendList.Add(item);
			UIEventListener.Get(item.select_bg.gameObject).onClick -= ShowSelect;
			UIEventListener.Get(item.select_bg.gameObject).onClick += ShowSelect;
		}
		friend_list_grid.SetActiveRecursively(false);
		
		now_page_num = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//隐藏窗口 
	void DismissPanel()
	{
		friend_list_position.Play(false);
		Destroy(gameObject);
	}
	//打开好友列表 
	void OpenFriendList()
	{
		friend_list_show = !friend_list_show;
		friend_list_position.Play(friend_list_show);
		if(friend_list_show)
		{
			int from_num = (now_page_num-1)*FriendItemNum;
			int to_num	= now_page_num*FriendItemNum;
			if(FriendManager.FriendDataList.Count>=to_num)
				to_num = to_num-1;
			else 
				to_num = FriendManager.FriendDataList.Count-1;
			//ReqFriendGoldSmpInfo(0,num-1);
			
			ReqFriendGoldSmpInfo(from_num,to_num);
			//friend_list_grid.SetActiveRecursively(true);
			//if(now_page_num>GetPageNum(FriendManager.FriendDataList.Count))
			//	now_page_num = GetPageNum(FriendManager.FriendDataList.Count);
		
			//page_num.text = now_page_num +"/"+GetPageNum(FriendManager.FriendDataList.Count);
			
		}
		friend_list_grid.SetActiveRecursively(false);
		//now_page_num = 1;
		
	}
	//缩回好友列表
	void DisFriendList()
	{
		if (friend_list_show == false) return;
		friend_list_position.Play(false);
		friend_list_grid.SetActiveRecursively(false);
		friend_list_show = false;
	}
	//请求好友金矿是否可以摘采等信息  
	void ReqFriendGoldSmpInfo(int from_num,int end_num)
	{
		//print (from_num+" "+end_num);
		CTS_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_GET_GOLDORE_SMP_INFO_ALL;
		req.nNum3 = end_num-from_num+1;
		ulong[] account_id_array = new ulong[req.nNum3];
		for(int i=0;i<req.nNum3;i++)
		{
			//print ("ssssssssssssssssss"+DataConvert.BytesToStr(FriendManager.GetFriendDataFromID(FriendManager.FriendDataList[10].nAccountID1).szName3));
			account_id_array[i] = FriendManager.FriendDataList[FriendManager.FriendDataList.Count-from_num-i-1].nAccountID1;
		}
		req.AccountIdArray4 = account_id_array;
		TcpMsger.SendLogicData<CTS_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T >(req);
		friend_list_grid.SetActiveRecursively(false);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL);
	}
	//请求好友金矿是否可以摘采等信息返回 
	void ReqFriendGoldSmpInfoRst(byte[] buff)
	{
		STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T.enum_rst.RST_OK)
		{
			friend_list_grid.SetActiveRecursively(true);
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_GET_GOLDORE_SMP_INFO_ALL_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			GoldoreSmpInfo[] gold_info_array = (GoldoreSmpInfo[])DataConvert.ByteToStructArray<GoldoreSmpInfo>(data_buff,(int)sub_msg.nNum2);
			int i=0;
			for(;i<gold_info_array.Length;i++)
			{
				FriendUnit unit = FriendManager.GetFriendDataFromID(gold_info_array[i].nAccountID1);
				if(unit.nAccountID1 == 0)
					return;
				int n = gold_info_array.Length -i-1;
				FriendList[n].char_name.text =DataConvert.BytesToStr(unit.szName3);
				if(gold_info_array[i].bCanFetchGold2 != 1)
					FriendList[n].jinkang_icon.gameObject.SetActiveRecursively(false);
				if(gold_info_array[i].bCanWaterChristmasTree3 != 1)
					FriendList[n].yaoqianshu_icon.gameObject.SetActiveRecursively(false);
				FriendList[n].select_bg.spriteName = "sliced_bg4"; 
				FriendList[n].now_friend_id = unit.nAccountID1;
			}
			for(;i<FriendItemNum;i++)
			{
				FriendList[i].char_name.text = "";
				FriendList[i].jinkang_icon.gameObject.SetActiveRecursively(false);
				FriendList[i].yaoqianshu_icon.gameObject.SetActiveRecursively(false);
				FriendList[i].select_bg.spriteName = "transparent"; 
				FriendList[i].now_friend_id = 0;
			}
		}
		page_label.text = now_page_num+"/"+GetPageNum(FriendManager.FriendDataList.Count);
	}
	//下一页 
	void NextPage()
	{
		if(now_page_num < GetPageNum(FriendManager.FriendDataList.Count))
		{
			if((++now_page_num)*FriendItemNum > FriendManager.FriendDataList.Count)
			{
				ReqFriendGoldSmpInfo((int)((now_page_num-1)*FriendItemNum),FriendManager.FriendDataList.Count-1);
			}
			else
			{
				ReqFriendGoldSmpInfo((int)((now_page_num-1)*FriendItemNum),(int)(now_page_num*FriendItemNum-1));
			}
		}	
	}
	//上一页 
	void PrevPage()
	{
		if(now_page_num > 1)
		{
			if((--now_page_num)*FriendItemNum > FriendManager.FriendDataList.Count)
			{
				ReqFriendGoldSmpInfo((int)((now_page_num-1)*FriendItemNum), FriendManager.FriendDataList.Count-1);
			}
			else
			{
				ReqFriendGoldSmpInfo((int)((now_page_num-1)*FriendItemNum),(int)(now_page_num*FriendItemNum-1));
			}
		}	
	}
	//获取页数 
	int GetPageNum(int num)
	{
		int pagenum = 1;
		if(num>0)
			pagenum = (num-1)/MessageManager.item_num_max+1;
			
		return pagenum;
	}
	//选中显示  
	void ShowSelect(GameObject obj)
	{
		int item_num =0;
		if(now_page_num*FriendItemNum > FriendManager.FriendDataList.Count)
		{
			item_num = FriendManager.FriendDataList.Count-(now_page_num-1)*FriendItemNum ;
		}
		else 
			item_num = FriendItemNum;
		
	
		for(int i=0;i<item_num;i++)
		{
			FriendList[i].select_bg.spriteName = "sliced_bg4";
		}
		if(obj.transform.parent.GetComponent<JinKuangFriendItem>().now_friend_id >0)
			obj.GetComponent<UISlicedSprite>().spriteName = "sliced_bg";
	}
}
