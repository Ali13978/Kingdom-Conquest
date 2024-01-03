using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour {
	//未完成任务图标 
	public GameObject task_icon;
	public static int growFee_unread_num = 0;
	public static int dayFee_unread_num = 0;
	public static int vipFee_unread_num = 0;
	
	public static string QUEST_POP = "{0}:POP";
	
	void Awake() 
	{	
		if (task_icon != null) {
			NGUITools.SetActive(task_icon, false);
		}
	}
	
	// Use this for initialization
	void Start () {
		
		// 上线提醒 ...
		DissbandForUnpack1();
		
		// 查看引导 ...
		CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
		string gCol = string.Format(QUEST_POP,info.AccountName);
		
		int BooDone = PlayerPrefs.GetInt(gCol, 0);
		if (BooDone == 1) 
		{
			QuestGuideIcon.instance.StartNewbieGuide();
		}
	}
	// 上线提醒 ...
	void DissbandForUnpack1()
	{
		// 上线提醒有多少任务奖励未接 ...
		QuestFeeManager.processListQuestDelegate = null;
		QuestFeeManager.RequesGetFeeQuest();
	}

	//打开任务窗口 
	public void OpenTaskWin()
	{
		LoadingManager.instance.ShowLoading();
		QuestFeeManager.processListQuestDelegate = OnProcessGetFeeQuestDelegate;
		QuestFeeManager.RequesGetFeeQuest();
	}
	
	public void OpenTaskLaunch()
	{
		GameObject go = U3dCmn.GetObjFromPrefab("QuestFeeLaunchWin");
		if (go == null ) return;
		QuestLaunchWin win1 = go.GetComponent<QuestLaunchWin>();
		if (win1 != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
		}
	}
	
	void OnProcessGetFeeQuestDelegate()
	{
		LoadingManager.instance.HideLoading();
		GameObject go = U3dCmn.GetObjFromPrefab("QuestFeeLaunchWin");
		if (go == null ) return;

		QuestLaunchWin win1 = go.GetComponent<QuestLaunchWin>();
		if (win1 != null)
		{
			float depth = -BaizVariableScript.DEPTH_OFFSET;
			win1.Depth(depth);
		}
		
		// <新手引导开启> 打开任务界面 ...
		if (NewbieQuestbook.processOpenTaskRst != null) 
		{
			NewbieQuestbook.processOpenTaskRst();
			NewbieQuestbook.processOpenTaskRst = null;
		}
	}
	// 设置未读图标 ...
	public void SetUnReadIcon()
	{
		int unread_num = growFee_unread_num + dayFee_unread_num + vipFee_unread_num;
		if (unread_num >0)
		{
			task_icon.SetActiveRecursively(true);
			if(unread_num >99)
			{
				task_icon.GetComponentInChildren<UILabel>().text = "99+";
			}
			else 
			{
				task_icon.GetComponentInChildren<UILabel>().text = unread_num.ToString();
			}
		}
		else
		{
			task_icon.SetActiveRecursively(false);
		}
		
		// 更新任务界面 ...
		U3dCmn.SendMessage("QuestFeeLaunchWin","SetUnReadNow",null);
	}
}
