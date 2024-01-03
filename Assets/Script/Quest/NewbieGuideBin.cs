using UnityEngine;
using System.Collections;
using CMNCMD;
using STCCMD;

static public class NewbieQuestbook
{
	public static NewbieGuide mGuide = null;
	
	// 完成任务响应 ....
	public delegate void NewbieTaskDoneRst();
	public static NewbieTaskDoneRst processTaskDoneRst = null;
	
	// 打开任务响应 ...
	public delegate void NewbieOpenTaskRst();
	public static NewbieOpenTaskRst processOpenTaskRst = null;
	
	public static void init1()
	{
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,82f);
		
		// 坐标 ....
		Vector2 pos1 = new Vector2(-108f,-130f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		mGuide.VoidButtonSelector(true, pos1.x, pos1.y, VoidButtonStep1TaskWinDelegate);
		mGuide.VoidButtonJiantou(0f, 40f, 180f);
	}
	// 打开任务界面 ...
	static void VoidButtonStep1TaskWinDelegate(GameObject go)
	{
		processOpenTaskRst = VoidButtonStep1OpenTaskSuccess;
		U3dCmn.SendMessage("TaskManager", "OpenTaskWin", null);
		// UIEventListener.Get(go).onClick = null; // 网络无响应时可以再点几次 ....
	}
	static void VoidButtonStep1OpenTaskSuccess()
	{
		processOpenTaskRst = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonSelector(false,0f,0f,null);
		mGuide.VoidButtonPage1(true,VoidButtonStep2TaskWinDelegate);
	}
	static void VoidButtonStep2TaskWinDelegate(GameObject go)
	{
		mGuide.GotoCStep_CP();		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true, -130f, -83f, VoidButtonStep3TaskWinDelegate);
		mGuide.VoidFrame1Dimensions(82f,36f);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);
	}
	static void VoidButtonStep3TaskWinDelegate(GameObject go)
	{
		processTaskDoneRst = VoidButtonStep4DoneRst;
		U3dCmn.SendMessage("QuestFeeLaunchWin", "VoidDoneQuest", null);
		// UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep4DoneRst()
	{
		processTaskDoneRst = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true, 198f,140f, VoidButtonStep4TaskWinDelegate);
		mGuide.VoidFrame1Dimensions(82f,40f);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
	}
	public static void VoidButtonStep4TaskWinDelegate(GameObject go)
	{
		U3dCmn.SendMessage("QuestFeeLaunchWin", "OnQuestLaunchClose", null);
		
		// 下一个存储点 ....
		mGuide.GotoSavePoint_CP();
	}
}

// 背包引导 ...
static public class NewbieBackpack
{
	public static NewbieGuide mGuide = null;
	public delegate void NewbieUseArticlesItemRst();
	public static NewbieUseArticlesItemRst processUseArticlesItemRst = null;
	
	public static void init1()
	{
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,82f);
		
		// 包装坐标 ....
		Vector2 pos1 = new Vector2(108f,-130f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1BackpackDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	static void VoidButtonStep1BackpackDelegate(GameObject go)
	{
		U3dCmn.SendMessage("CangKuManager","OpenCangKuInfoWin",null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,-170f,45f,VoidButtonStep2BackpackDelegate);
		mGuide.VoidFrame1Dimensions(80f,80f);
		mGuide.VoidButtonJiantou(0f,50f,180f);
	}
	static void VoidButtonStep2BackpackDelegate(GameObject go)
	{
		U3dCmn.SendMessage("CangKuWin","SelectFirstItem",null);
		
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,92f,-112f,VoidButtonStep3BackpackDelegate);
		mGuide.VoidFrame1Dimensions(80f,36f);
		mGuide.VoidButtonJiantou(0f,30f,180f);
	}
	static void VoidButtonStep3BackpackDelegate(GameObject go)
	{
		processUseArticlesItemRst = VoidButtonStep4UseItemRstSuccess;
		U3dCmn.SendMessage("CangKuWin","UseArticlesItem", null);
		// UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep4UseItemRstSuccess()
	{
		processUseArticlesItemRst = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,200f,138f,VoidButtonStep4BackpackDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep4BackpackDelegate(GameObject go)
	{
		U3dCmn.SendMessage("CangKuWin","DismissPanel", null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,82f);
		
		// 任务坐标 ....
		Vector2 pos1 = new Vector2(-108f,-130f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep5BackpackDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	static void VoidButtonStep5BackpackDelegate(GameObject go)
	{
		NewbieQuestbook.processOpenTaskRst = VoidButtonStep5OpenTaskSuccess;
		U3dCmn.SendMessage("TaskManager", "OpenTaskWin", null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep5OpenTaskSuccess()
	{
		NewbieQuestbook.processOpenTaskRst = null;
		if (mGuide == null) return;

		// 下一个节点 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,-130f,30f,null);
		mGuide.VoidButtonJiantouFalse();
		mGuide.VoidFrame1Dimensions(130f,45f);
		mGuide.VoidButtonPage1(true, VoidButtonStep6BackpackDelegate);
	}
	static void VoidButtonStep6BackpackDelegate(GameObject go)
	{		
		// 下一个节点 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,198f,140f, VoidButtonStep7TaskWinDelegate);
		mGuide.VoidFrame1Dimensions(82f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep7TaskWinDelegate(GameObject go)
	{
		U3dCmn.SendMessage("QuestFeeLaunchWin", "OnQuestLaunchClose", null);
		
		// 下一个存储点 ....
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieHouse
{
	public static NewbieGuide mGuide = null;
	
	public delegate void NewbieBuildingRst();
	public static NewbieBuildingRst processNewbieBuildingRst = null;
	
	public delegate void NewbieUpgradeMinJuRst();
	public static NewbieUpgradeMinJuRst processUpgradeMinJuRst = null;
	
	public static void init1()
	{
		// 第十三步 ...
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.MINJU);

		// 民居区域 ...
		Vector2 pos1 = new Vector2(117f,68f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0,-20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(44f,0f);
		}
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1BuildMinJuDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	// 领取奖励 ...
	public static void init2()
	{		
		// 第16步 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.MINJU);
		
		// < 领取奖励 >
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonPage1(false, null);
		
		// 任务坐标 ....
		Vector2 pos1 = new Vector2(-108f,-130f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		mGuide.VoidButtonSelector(true, pos1.x, pos1.y, VoidButtonStep4BuildMinJuDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f, 40f, 180f);
	}
	// 升级民居区 ...
	public static void init3()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.MINJU);
		
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(140f,68f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0,-20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(44f,0f);
		}
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1UpgradeMinJuDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	static void VoidButtonStep2ExitingMinJuDelegate(GameObject tween)
	{
		U3dCmn.SendMessage("BuildingManager", "DismissMinJuPanel", null);
		
		// 下一步节点引导 ...
		mGuide.GotoSavePoint_CP();
	}
	static void VoidButtonStep1ExitingMinJuDelegate(GameObject tween)
	{
		// 离开第一步 ...
		U3dCmn.SendMessage("MinJuInfoWin","DismissPanel",null);
		
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(200f,140f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,20f);
		}
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep2ExitingMinJuDelegate);
		mGuide.VoidFrame1Dimensions(90f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep1Page1Delegate(GameObject tween)
	{
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true,180f,118f,VoidButtonStep1ExitingMinJuDelegate);
		mGuide.VoidFrame1Dimensions(70f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep1UpgradeMinJuDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BuildingManager", "RevealMinJuPanel", null);
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,12f,0f,VoidButtonStep2UpgradeMinJuDelegate);
	}
	// 升级民居房子 ...
	static void VoidButtonStep2UpgradeMinJuDelegate(GameObject go)
	{
		// 打开民居界面 ...
		BuildingManager.OpenMinJuWin(5);
		
		if (ChengBao.NowLevel == 1)
		{
			mGuide.GotoCStep_CP();
			mGuide.VoidButtonPage1(true, VoidButtonStep1Page1Delegate);
			mGuide.VoidButtonSelector(true,47f,-8f,null);
			mGuide.VoidFrame1Dimensions(200f,60f);
			mGuide.VoidButtonJiantouFalse();
		}
		else if (ChengBao.NowLevel > 1)
		{
			mGuide.GotoCStep_CP();
			mGuide.VoidButtonPage1(false,null);
			mGuide.VoidButtonSelector(true,0f,-100f,VoidButtonStep3UpgradeMinJuDelegate);
			mGuide.VoidFrame1Dimensions(85f,36f);
			mGuide.VoidSetManipIntro(0f,82f);
			mGuide.VoidButtonJiantou(0f,-30f,0f);
		}
	}
	static void VoidButtonPage1UpgradeDelegate(GameObject go)
	{
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(200f,140f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,20f);
		}
		
		mGuide.GotoCStep_CP();
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidFrame1Dimensions(90f,40f);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep2ExitingMinJuDelegate);
	}
	static void VoidButtonStep3UpgradeMinJuDelegate(GameObject go)
	{
		processUpgradeMinJuRst = VoidButtonStep3UpgradeMinJuSuccess;
		U3dCmn.SendMessage("MinJuInfoWin", "ReqUpgradeMinJu", null);
		// UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep3UpgradeMinJuSuccess()
	{
		processUpgradeMinJuRst = null;
		if (mGuide == null) return;
		
		// 接下一步 ....
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true, VoidButtonPage1UpgradeDelegate);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	// 打开民居区 ...
	static void VoidBuildMinJu1SuccessDelegate()
	{
		NewbieWaiting.processNewbieWaiting = null;
		MinJu.BuildSuccessCallBack = null;
		if (mGuide == null) return;
		mGuide.GotoCStep_CP(); // 离开民居区, 打开任务 ...
		
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(200f,140f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,20f);
		}
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep3BuildMinJuDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep2BuildMinJuDelegate(GameObject tween)
	{
		processNewbieBuildingRst = VoidButtonStep2ReqBuildMinJuSuccess;
		MinJu.BuildSuccessCallBack = VoidBuildMinJu1SuccessDelegate;
		BuildingManager.BuildMinJu(5);
	}
	static void VoidButtonStep2ReqBuildMinJuSuccess()
	{
		processNewbieBuildingRst = null;
		if (mGuide == null) return;
				
		// 断线重连回调 ...
		NewbieWaiting.processNewbieWaiting = VoidBuildMinJu1SuccessDelegate;
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonStep1BuildMinJuDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BuildingManager", "RevealMinJuPanel", null);
		
		mGuide.GotoCStep_CP();
		
		int num = BuildingManager.GetBuildingNum((int)BuildingSort.MINJU);
		if (num > 0) 
		{
			VoidBuildMinJu1SuccessDelegate();
		} 
		else
		{
			mGuide.VoidButtonPage1(false,null);
			mGuide.VoidButtonSelector(true,12f,-12f,VoidButtonStep2BuildMinJuDelegate);
		}
	}
	static void VoidButtonStep3BuildMinJuDelegate(GameObject tween)
	{
		U3dCmn.SendMessage("BuildingManager","DismissMinJuPanel",null);
		// < 关闭 > ...
		// < 领取奖励 >
		mGuide.GotoSavePoint_CP();
	}
	static void VoidButtonStep4BuildMinJuDelegate(GameObject tween)
	{
		NewbieQuestbook.processOpenTaskRst = VoidButtonStep4OpenTaskSuccess;
		U3dCmn.SendMessage("TaskManager", "OpenTaskWin", null);
		//UIEventListener.Get(tween).onClick = null;
	}
	static void VoidButtonStep4OpenTaskSuccess()
	{
		NewbieQuestbook.processOpenTaskRst = null;
		if (mGuide == null) return;
		
		// 下一个节点 ...
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true, -130f, -83f, VoidButtonStep5BuildMinJuDelegate);
		mGuide.VoidFrame1Dimensions(82f,36f);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);	
	}
	static void VoidButtonStep5BuildMinJuDelegate(GameObject tween)
	{
		NewbieQuestbook.processTaskDoneRst = VoidButtonStep5TaskDoneSuccess;
		U3dCmn.SendMessage("QuestFeeLaunchWin", "VoidDoneQuest", null);
		// UIEventListener.Get(tween).onClick = null;
	}
	static void VoidButtonStep5TaskDoneSuccess()
	{
		NewbieQuestbook.processTaskDoneRst = null;
		if (mGuide == null) return;
		
		// 接住下一步 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true, 198f,140f, VoidButtonStep6BuildMinJuDelegate);
		mGuide.VoidFrame1Dimensions(82f,40f);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
	}
	static void VoidButtonStep6BuildMinJuDelegate(GameObject tween)
	{
		U3dCmn.SendMessage("QuestFeeLaunchWin", "OnQuestLaunchClose", null);
		
		// 缓慢移动定位 ...
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieChengBao
{
	static public NewbieGuide mGuide = null;
	
	// 请求城堡升级成功 ...
	public delegate void NewbieChengBaoReqUpgradeRst();
	public static NewbieChengBaoReqUpgradeRst processChengBaoReqUpgradeRst = null;
	
	// 新手引导须要的回调, 接下去 ...
	public delegate void NewbieChengBaoUpgradeRst();
	public static NewbieChengBaoUpgradeRst processChengBaoUpgradeRst = null;
	
	public static void init1()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.CHENGBAO);
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(0f,40f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(-10f,0f);	
		}
		
		if (ChengBao.NowLevel<2)
		{
			if (ChengBao.BuildState == (int)BuildingState.UPGRADING)
			{
				VoidChengBaoUpgradeWaiting();
			}
			else if (ChengBao.BuildState == (int)BuildingState.NORMAL)
			{
				mGuide.VoidButtonPage1(false, null);
				mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1ChengBaoDelegate);
				mGuide.VoidFrame1Dimensions(70f,60f);
				mGuide.VoidButtonJiantou(0f,-40f,0f);
			}
		}
		else 
		{
			mGuide.GotoCStep_CP2(3); // 跳三步 ... 到 26 ...
			mGuide.VoidButtonPage1(true, VoidButtonPage1Delegate);
			mGuide.VoidButtonSelector(false, 0f, 0f, null);
		}
	}
	static void VoidButtonStep1ChengBaoDelegate(GameObject tween)
	{
		U3dCmn.SendMessage("ChengBao","OpenChengBaoInfoWin",null);
		
		mGuide.GotoCStep_CP();
		
		// 城堡未到2级 ... 
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,0f,-100f,VoidButtonStep2UpgradeChengBaoDelegate);
		mGuide.VoidFrame1Dimensions(100f,40f);
		mGuide.VoidButtonJiantou(0f,30f,180f);
	}
	static void VoidChengBaoUpgradeWaiting()
	{
		if (mGuide == null) return;
		processChengBaoUpgradeRst = VoidChengBaoUpgradeSuccessDelegate;
		// 接一步 ....
		mGuide.GotoCStep_CP2(2);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonPage1Delegate(GameObject go)
	{
		// 下一个存储点 ....
		mGuide.GotoSavePoint_CP();
	}
	static void VoidChengBaoUpgradeSuccessDelegate()
	{
		NewbieWaiting.processNewbieWaiting = null;
		NewbieChengBao.processChengBaoUpgradeRst = null;
		if (mGuide == null) return;
		
		// 接下一步 ....
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true, VoidButtonPage1Delegate);
	}
	static void VoidButtonStep2ReqUpgradeSuccess()
	{
		processChengBaoReqUpgradeRst = null;
		if (mGuide == null) return;
		
		// 断线重连续 ...
		NewbieWaiting.processNewbieWaiting = VoidChengBaoUpgradeSuccessDelegate;
		
		// 接下一步等待 CD结束 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonStep2UpgradeChengBaoDelegate(GameObject go)
	{
		// 升级城堡 ...
		processChengBaoUpgradeRst = VoidChengBaoUpgradeSuccessDelegate;
		processChengBaoReqUpgradeRst = VoidButtonStep2ReqUpgradeSuccess;
		U3dCmn.SendMessage("ChengBaoInfoWin","ReqUpgradeChengBao", null);
		// UIEventListener.Get(go).onClick = null;
	}
}

static public class NewbieJinKuang
{
	public static NewbieGuide mGuide = null;
	
	public delegate void NewbieJinKuangProduceDelegate();
	public static NewbieJinKuangProduceDelegate processJinKuangProduceRst = null;
	
	// 初始化引导 ...
	public static void init1()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.JINKUANG);
		
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(-19f,46f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(-44f,0f);
		}
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1BuildJinKuangDelegate);
		mGuide.VoidFrame1Dimensions(80f,60f);
		mGuide.VoidButtonJiantou(0f,-40f,0f);
	}
	// 生产引导 ...
	public static void init2()
	{
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.JINKUANG);
		U3dCmn.SendMessage("BuildingManager", "RevealJinKuangPanel", null);
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,12f,0f,VoidButtonStep1ProduceGoldDelegate);
	}
	// 摇钱树引导 ...
	public static void init3()
	{
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.JINKUANG);
		U3dCmn.SendMessage("BuildingManager", "RevealJinKuangPanel", null);
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true, 175f,-120f, VoidButtonStep1WateringDelegate);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	// 引导 ...
	static void VoidButtonStep1BuildJinKuangDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BuildingManager", "RevealJinKuangPanel", null);
		
		mGuide.GotoCStep_CP();
		int num = BuildingManager.GetBuildingNum((int)BuildingSort.JINKUANG);
		if (num > 0)
		{
			// 接下去 ...
			VoidBuildJinKuang1SuccessDelegate();
		}
		else
		{
			mGuide.VoidButtonPage1(false, null);
			mGuide.VoidSetManipIntro(0f,-100f);
			mGuide.VoidButtonSelector(true,12f,0f,VoidButtonStep2BuildJinKuangDelegate);
		}
	}
	static void VoidButtonStep2BuildJinKuangDelegate(GameObject tween)
	{
		JinKuang.BuildSuccessCallBack = VoidBuildJinKuang1SuccessDelegate;
		NewbieHouse.processNewbieBuildingRst = VoidButtonStep2ReqBuildJinKuangSuccess;
		BuildingManager.BuildJinKuang(5);
		//UIEventListener.Get(tween).onClick = null;
	}
	static void VoidButtonStep2ReqBuildJinKuangSuccess()
	{
		NewbieHouse.processNewbieBuildingRst = null;
		if (mGuide == null) return;
		
		// 断线重连 ...
		NewbieWaiting.processNewbieWaiting = VoidBuildJinKuang1SuccessDelegate;
		mGuide.VoidButtonSelector(false, 0f, 0f, null);
	}
	static void VoidBuildJinKuang1SuccessDelegate()
	{
		NewbieWaiting.processNewbieWaiting = null;
		JinKuang.BuildSuccessCallBack = null;
		if (mGuide == null) return;
		
		// 开始生产的引导 ...
		mGuide.GotoCStep_CP();
		// 生产 ...
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,12f,0f,VoidButtonStep1ProduceGoldDelegate);
	}
	static void VoidButtonStep1ProduceGoldDelegate(GameObject go)
	{
		// 打开第五个金矿 ....
		BuildingManager.OpenJinKuangWin(5);
		// 接下一步 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidFrame1Dimensions(86f,36f);
		mGuide.VoidButtonJiantou(0f,30f,180f);
		mGuide.VoidButtonSelector(true,0f,-105f,VoidButtonStep2ProduceGoldDelegate);
	}
	static void VoidButtonStep3ExistingDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BuildingManager","DismissJinKuangPanel",null);
		
		// 下一个存储点 ....
		mGuide.GotoSavePoint_CP();
	}
	static void VoidButtonStep3WateringDelegate(GameObject go)
	{
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(200f,140f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,20f);
		}
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
		mGuide.VoidFrame1Dimensions(90f,40f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep3ExistingDelegate);
	}
	static void VoidButtonStep2WateringDelegate()
	{
		YaoQianShu.WaterSuccessCallBack = null;
		if (mGuide == null) return;
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true, VoidButtonStep3WateringDelegate);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonStep1WateringDelegate(GameObject tween)
	{
		// 浇水指令 ...
		YaoQianShu.WaterSuccessCallBack = VoidButtonStep2WateringDelegate;
		YaoQianShu.NewCharWaterTree();
		//UIEventListener.Get(tween).onClick = null;
	}
	static void VoidButtonStep1ClosingJinKuangInfoWin(GameObject go)
	{
		U3dCmn.SendMessage("JinKuangInfoWin","DismissPanel",null);
		
		// 摇钱树浇水引导 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,82f);
		
		if (false == YaoQianShu.can_water)
		{
			VoidButtonStep2WateringDelegate();
		}
		else
		{
			mGuide.VoidButtonSelector(true, 175f,-120f, VoidButtonStep1WateringDelegate);
			mGuide.VoidFrame1Dimensions(70f,60f);
			mGuide.VoidButtonJiantou(0f,40f,180f);
		}
	}
	static void VoidButtonPage1Delegate(GameObject go)
	{
		mGuide.GotoCStep_CP();
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonSelector(true,197f,130f,VoidButtonStep1ClosingJinKuangInfoWin);
	}
	static void VoidButtonStep2ProduceGoldDelegate(GameObject go)
	{
		processJinKuangProduceRst = VoidButtonStep2ProduceGoldSuccess;
		U3dCmn.SendMessage("JinKuangInfoWin","ReqProduce",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep2ProduceGoldSuccess()
	{
		processJinKuangProduceRst = null;
		if (mGuide == null) return;
				
		// 生产倒计时 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true, VoidButtonPage1Delegate);
		mGuide.VoidButtonSelector(false, 0f,0f,null);
	}
}

static public class NewbieYanJiuYuan
{
	public static NewbieGuide mGuide = null;
	
	// 研究请求返回 ...
	public delegate void NewbieReqResearchEndRst();
	public static NewbieReqResearchEndRst processReqResearchEndRst = null;
		
	// 一切为了新手引导 ( 研究兵种结束 ) ... 
	public delegate void NewbieResearchEndDelegate();
	public static NewbieResearchEndDelegate processNewbieResearchEnd = null;
	
	public static void init1()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.YANJIUYUAN);
		
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(12f,30f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(14f,0f);
		}
		
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1BingUpgradeDelegate);
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,-40f,0f);
	}
	// 第一步引导 ...
	static void VoidButtonStep1BingUpgradeDelegate(GameObject go)
	{
		// 无须返回处理 ...
		U3dCmn.SendMessage("Institute","OpenInstituteWin", null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonSelector(true,-97f,118f,VoidButtonStep2BingUpgradeDelegate);
		mGuide.VoidFrame1Dimensions(80f,36f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep2BingUpgradeDelegate(GameObject go)
	{
		U3dCmn.SendMessage("TechnologyWin", "TabYanJiuYuan", null);
		
		mGuide.GotoCStep_CP();
				
		uint nExcelID = 1; // 骑兵 ...
		SoldierUnit info = SoldierManager.GetTopSoldierInfo(nExcelID);
		if (info.nLevel2 > 0)
		{
			VoidResearchEndSuccessDelegate();
		}
		else
		{
			mGuide.VoidButtonPage1(false, null);
			mGuide.VoidButtonSelector(true,-132f,-1f,VoidButtonStep3BingUpgradeDelegate);
			mGuide.VoidFrame1Dimensions(80f,36f);
		}
	}
	static void VoidButtonStep3BingUpgradeDelegate(GameObject go)
	{
		uint nExcelID = 1;
		
		// 研究院结束 ....
		processReqResearchEndRst = VoidButtonStep3ReqBingUpgradeSuccess;
		processNewbieResearchEnd = VoidResearchEndSuccessDelegate;
		TechnologyManager.ReqResearchTech(nExcelID);
		// UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep3ReqBingUpgradeSuccess()
	{
		processReqResearchEndRst = null;
		if (mGuide == null) return;
		// 断线重连 ....
		NewbieWaiting.processNewbieWaiting = VoidResearchEndSuccessDelegate;
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	public static void VoidResearchEndSuccessDelegate()
	{
		NewbieWaiting.processNewbieWaiting = null;
		processNewbieResearchEnd = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,202f,138f,VoidButtonStep3ExistingDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
	}
	static void VoidButtonStep3ExistingDelegate(GameObject go)
	{
		U3dCmn.SendMessage("TechnologyWin","DismissPanel",null);
				
		// 下一步节点引导 ...
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieBingYing
{
	public static NewbieGuide mGuide = null;
	public delegate void NewbieBingYingTrainRst();
	public static NewbieBingYingTrainRst processBingYingTrainRst = null;
	
	public static void init1()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.BINGYING);
				
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(12f,-20f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(-14f,0f);
		}
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1BuildBingYingDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	public static void init2()
	{
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.BINGYING);
		U3dCmn.SendMessage("BuildingManager", "RevealBingYingPanel", null);	
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0,-100f);
		mGuide.VoidButtonSelector(true,-13f,14f,VoidButtonStep1TrainSoldierDelegate);
	}
	static void VoidButtonStep1BuildBingYingDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BuildingManager", "RevealBingYingPanel", null);
		mGuide.GotoCStep_CP();
		
		int num = BuildingManager.GetBuildingNum((int)BuildingSort.BINGYING);
		if (num > 0)
		{
			// 连下去 ...
			VoidBuildBingYing1SuccessDelegate();
		}
		else
		{
			mGuide.VoidButtonPage1(false,null);
			mGuide.VoidButtonSelector(true,-13f,14f,VoidButtonStep2BuildBingYingDelegate);
			mGuide.VoidSetManipIntro(0f,-100f);
			mGuide.VoidButtonJiantou(0f,-40f,0f);
		}
	}
	static void VoidButtonStep2BuildBingYingSuccess()
	{
		NewbieHouse.processNewbieBuildingRst = null;
		if (mGuide == null) return;
		// 断线重连 ...
		NewbieWaiting.processNewbieWaiting = VoidBuildBingYing1SuccessDelegate;
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonStep2BuildBingYingDelegate(GameObject tween)
	{
		BingYing.BuildSuccessCallBack = VoidBuildBingYing1SuccessDelegate;
		NewbieHouse.processNewbieBuildingRst = VoidButtonStep2BuildBingYingSuccess;
		BuildingManager.BuildBingYing(5);
		//UIEventListener.Get(tween).onClick = null;
	}
	static void VoidBuildBingYing1SuccessDelegate()
	{
		NewbieWaiting.processNewbieWaiting = null;
		BingYing.BuildSuccessCallBack = null;
		if (mGuide == null) return;
		mGuide.GotoCStep_CP();
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,-13f,14f,VoidButtonStep1TrainSoldierDelegate);
	}
	static void VoidButtonStep1TrainSoldierDelegate(GameObject go)
	{
		// 打开训练界面 ...
		BuildingManager.OpenBingYingWin(5);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,-96f,116f,VoidButtonStep2TrainSoldierDelegate);
		mGuide.VoidFrame1Dimensions(84f,36f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep2TrainSoldierDelegate(GameObject go)
	{	
		// 切换至训练面板 ...
		U3dCmn.SendMessage("BingYingInfoWin","TabXunLian",null);
			
		int num = SoldierManager.GetSoldierNum();
		if (num > 0)
		{
			mGuide.GotoCStep_CP2(3); // 跳过三步 ....
			mGuide.VoidButtonPage1(true, VoidButtonPage1ZhaoMuDelegate);
			mGuide.VoidSetManipIntro(0f,-100f);
			mGuide.VoidButtonSelector(false,0f,0f,null);
		}
		else 
		{
			mGuide.GotoCStep_CP();
			mGuide.VoidButtonPage1(false,null);
			mGuide.VoidButtonSelector(true,-108f,73f,VoidButtonStep3TrainSoldierDelegate);
			mGuide.VoidFrame1Dimensions(60f,36f);
		}
	}
	static void VoidButtonStep3TrainSoldierDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BingYingInfoWin","OpenTrainSoldierWin", null);
		U3dCmn.SendMessage("TrainSoldierWin","TabInputChange", null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,0f,-91f,VoidButtonStep4TrainSoldierDelegate);
		mGuide.VoidFrame1Dimensions(100f,40f);
	}
	static void VoidButtonStep4TrainSoldierDelegate(GameObject go)
	{
		// 训练讲学
		processBingYingTrainRst = VoidButtonStep4TrainSoldierSuccess;
		U3dCmn.SendMessage("TrainSoldierWin","ReqTrainSoldier",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep4TrainSoldierSuccess()
	{
		processBingYingTrainRst = null;
		if (mGuide == null) return;
		
		// 接住下一步 ....
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true, VoidButtonPage1ZhaoMuDelegate);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonPage1ZhaoMuDelegate(GameObject go)
	{
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true, 206,138f, VoidButtonStep5TrainSoldierDelegate);
		mGuide.VoidFrame1Dimensions(70f,40f);
	}
	static void VoidButtonStep5TrainSoldierDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BingYingInfoWin","DismissPanel",null);
		
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(200f,140f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f, 20f);
		}
		
		mGuide.GotoCStep_CP();
		mGuide.VoidFrame1Dimensions(90f,40f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y, VoidButtonStep6ExistingDelegate);
	}
	static void VoidButtonStep6ExistingDelegate(GameObject go)
	{
		U3dCmn.SendMessage("BuildingManager", "DismissBingYingPanel", null);
		
		// 下一个存储点 ...
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieJiuGuan
{
	public static NewbieGuide mGuide = null;
	// 成功招募返回 ...
	public delegate void NewbieJiuGuanZhaoMuRst();
	public static NewbieJiuGuanZhaoMuRst processZhaoMuRst = null;
	
	// 刷新酒馆返回 ...
	public delegate void NewbieTavernRefreshRst();
	public static NewbieTavernRefreshRst processTavernRefreshRst = null;
	
	public static void init1()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.JIUGUAN);
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,90f,0f,VoidButtonStep1ZhaoMuDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	static void VoidButtonStep1ZhaoMuDelegate(GameObject go)
	{
		// 打开酒馆界面 ....
		U3dCmn.SendMessage("JiuGuan", "OpenJiuGuanInfoWin", null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonSelector(true,-90f,117f,VoidButtonStep2ZhaoMuDelegate);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
		mGuide.VoidFrame1Dimensions(86f,40f);
	}
	static void VoidButtonStep2ZhaoMuDelegate(GameObject go)
	{
		processTavernRefreshRst = VoidButtonStep2ReqTavernRefreshSuccess;
		U3dCmn.SendMessage("JiuGuanInfoWin","TabZhaoMu",null);
		// UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep2ReqTavernRefreshSuccess()
	{
		NewbieWaiting.processNewbieWaiting = null;
		NewbieJiuGuan.processTavernRefreshRst = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true,VoidButtonStep3ZhaoMuDelegate);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonStep3ZhaoMuDelegate(GameObject go)
	{
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		
		// 是否已经完成招募了 ...
		if (JiangLingManager.MyHeroMap.Count>0)
		{
			mGuide.GotoCStep_CP();
			mGuide.VoidButtonSelector(true,203f,138f,VoidButtonStep5ExistingDelegate);
		}
		else 
		{
			mGuide.VoidButtonSelector(true,-112f,-10f,VoidButtonStep4ZhaoMuDelegate);
			mGuide.VoidFrame1Dimensions(70f,40f);
		}
	}
	static void VoidButtonStep4ZhaoMuDelegate(GameObject go)
	{
		// 执行招募 ...
		processZhaoMuRst = VoidButtonStep4ZhaoMuSuccess;
		U3dCmn.SendMessage("JiangLing1", "ZhaoMuJiangLing", null);
		// UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep4ZhaoMuSuccess()
	{
		processZhaoMuRst = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,203f,138f,VoidButtonStep5ExistingDelegate);
	}
	static void VoidButtonStep5ExistingDelegate(GameObject go)
	{
		U3dCmn.SendMessage("JiuGuanInfoWin","DismissPanel",null);
		
		// 下一个存储点 ...
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieJiangLing
{
	public static bool NewbiePeiBing = false;
	public static NewbieGuide mGuide = null;
	
	// 设置装备返回调用 ....
	public delegate void NewbieSetEquipRst();
	public static NewbieSetEquipRst processSetEquipRst = null;
	public delegate void NewbieSaveSoldierRst();
	public static NewbieSaveSoldierRst processSaveSoldierRst = null;
	
	public static void init1()
	{
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(55f,-128f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1HeroDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
		mGuide.Depth(BaizVariableScript.DEPTH_TOP1);
	}
	public static void init2()
	{
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(55f,-128f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,pos1.x,pos1.y,VoidButtonStep1PeiBingDelegate);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
		mGuide.Depth(BaizVariableScript.DEPTH_TOP1);
	}
	static void VoidButtonStep1PeiBingDelegate(GameObject go)
	{
		// 没有武将 ... 不能下一步了 ...
		if (JiangLingManager.MyHeroMap.Count < 1) return;
		
		GameObject infowin = U3dCmn.GetObjFromPrefab("JiangLingInfoWin");
		if( infowin != null ) {
			infowin.SendMessage("RevealPanel");
		}
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,160f,-112f,VoidButtonStep7PeiBingDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonJiantou(0f,30f,180f);
	}
	static void VoidButtonStep1HeroDelegate(GameObject go)
	{
		// 没有武将 ... 不能下一步了 ...
		if (JiangLingManager.MyHeroMap.Count < 1) return;
		
		GameObject infowin = U3dCmn.GetObjFromPrefab("JiangLingInfoWin");
		if( infowin != null ) {
			infowin.SendMessage("RevealPanel");
		}
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true,VoidButtonStep2EquipDelegate);
		mGuide.VoidButtonSelector(false,0f,0f,null);	
	}
	static void VoidButtonStep2EquipDelegate(GameObject go)
	{
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,80f,-112f,VoidButtonStep3EquipDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonJiantou(0f,30f,180f);
	}
	static void VoidButtonStep3EquipDelegate(GameObject go)
	{
		U3dCmn.SendMessage("JiangLingInfoWin","OpenEquipWin",null);
		mGuide.GotoCStep_CP();
		
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,3f,-8f,VoidButtonStep4EquipDelegate);
		mGuide.VoidFrame1Dimensions(60f,60f);
		mGuide.VoidButtonJiantou(0f,40f,180f);
	}
	static void VoidButtonStep4EquipDelegate(GameObject go)
	{
		U3dCmn.SendMessage("HeroEquipWin","DressEqiupForNewbie",null);
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true,0f,-115f,VoidButtonStep5EquipDelegate);
		mGuide.VoidFrame1Dimensions(88f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);

	}
	static void VoidButtonStep5EquipDelegate(GameObject go)
	{
		processSetEquipRst = VoidButtonStep5SetEquipSuccess;
		U3dCmn.SendMessage("MyEquipInfoWin","ChangeEquip",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep5SetEquipSuccess()
	{
		processSetEquipRst = null;
		if (mGuide == null) return;
		
		mGuide.GotoCStep_CP();	
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true,205f,140f,VoidButtonStep6EquipDelegate);
		mGuide.VoidFrame1Dimensions(70f,40f);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
	}
	static void VoidButtonStep6EquipDelegate(GameObject go)
	{
		U3dCmn.SendMessage("HeroEquipWin","DismissPanel",null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true,160f,-112f,VoidButtonStep7PeiBingDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonJiantou(0f,30f,180f);
	}
	static void VoidButtonStep7PeiBingDelegate(GameObject go)
	{
		NewbieJiangLing.NewbiePeiBing = true;
		U3dCmn.SendMessage("JiangLingInfoWin", "OpenWithSoldierWin", null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidButtonSelector(true,90f,-110f,VoidButtonStep8PeiBingDelegate);
		mGuide.VoidFrame1Dimensions(90f,40f);
	}
	static void VoidButtonStep8PeiBingDelegate(GameObject go)
	{
		NewbieJiangLing.NewbiePeiBing = false;
		processSaveSoldierRst = VoidButtonStep8SaveSoldierSuccess;
		U3dCmn.SendMessage("WithSoldierWin","ReqSaveWithSoldierData",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep8SaveSoldierSuccess()
	{
		processSaveSoldierRst = null;
		if (mGuide == null) return;
		
		// 保存设置 ....
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonJiantou(0f,-30f,0f);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonSelector(true,200f,140f,VoidButtonStep9ExistingDelegate);	
	}
	static void VoidButtonStep9ExistingDelegate(GameObject go)
	{
		U3dCmn.SendMessage("JiangLingInfoWin", "DismissPanel", null);
		
		// 下一个存储点 ...
		mGuide.GotoSavePoint_CP();
	}
}

// <新手引导> 出征 ....
static public class NewbieChuZheng
{
	public static NewbieGuide mGuide = null;
	public delegate void NewbieStartCombatDelegate();
	public static NewbieStartCombatDelegate processNewbieStartCombat = null;
	
	public delegate void NewbieArmyAccelDelegate();
	public static NewbieArmyAccelDelegate processNewbieArmyAccel = null;
	
	public delegate void NewbieCombatRstDelegate();
	public static NewbieCombatRstDelegate processNewbieCombatRst = null;
	
	public static void init1()
	{
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(-108f,-130f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		}
		
		// 缓慢移动定位 ...		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true, pos1.x, pos1.y, VoidButtonStep1ChuZhengDelegate);
		mGuide.VoidButtonJiantou(0f, 40f, 180f);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.Depth(BaizVariableScript.DEPTH_TOP1);
	}
	static void VoidButtonStep1ChuZhengDelegate(GameObject go)
	{
		NewbieQuestbook.processOpenTaskRst = VoidButtonStep1OpenTaskSuccess;
		U3dCmn.SendMessage("TaskManager", "OpenTaskWin", null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep1OpenTaskSuccess()
	{
		NewbieQuestbook.processOpenTaskRst = null;
		if (mGuide == null) return;
		
		// 成功返回, 接下一步 ....		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0,-100f);
		mGuide.VoidButtonSelector(true,-10f,111f,VoidButtonStep2ChuZhengDelegate);
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);
		mGuide.VoidFrame1Dimensions(90f,40f);
	}
	static void VoidButtonStep2ChuZhengDelegate(GameObject go)
	{
		U3dCmn.SendMessage("QuestFeeLaunchWin","TabChuZheng",null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0,82f);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);
		mGuide.VoidButtonSelector(true,-131f,-82f,VoidButtonStep3ChuZhengDelegate);
	}
	static void VoidButtonStep3ChuZhengDelegate(GameObject go)
	{
		// < 此处没有消息等待 > ....
		U3dCmn.SendMessage("QuestFeeLaunchWin","VoidDoneChuZheng",null);
		
		mGuide.GotoCStep_CP2(2); // 跳一步吧 ...
		mGuide.VoidSetManipIntro(0,82f);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);
		mGuide.VoidFrame1Dimensions(90f,40f);
		mGuide.VoidButtonSelector(true,160,-94f,VoidButtonStep4ChuZhengDelegate);
	}
	static void VoidButtonStep4ChuZhengDelegate(GameObject go)
	{
		processNewbieStartCombat = VoidButtonStep5WarCCDelegate;
		U3dCmn.SendMessage("PopGeneralCrushWin","OnQuestFeeStartCombat",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep5WarCCDelegate()
	{
		// 战斗返回结果 等待 ....
		processNewbieCombatRst = VoidButtonStep7ArmyAccelSuccess;
		
		// 下一步吧 ....
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0,-100f);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);
		mGuide.VoidFrame1Dimensions(55f,38f);
		mGuide.VoidButtonSelector(true,135f,70f,VoidButtonStep6ChuZhengDelegate);
	}
	static void VoidButtonStep6ChuZhengDelegate(GameObject go)
	{
		processNewbieArmyAccel = VoidButtonStep7ArmyAccelSuccess;
		U3dCmn.SendMessage("WarSituationWin","NewbieJiaShu",null);
		//UIEventListener.Get(go).onClick = null;
		
		mGuide.VoidSetManipIntro(0,82f);
		mGuide.VoidButtonJiantou(0f, 30f, 180f);
		mGuide.VoidFrame1Dimensions(90f,40f);
		mGuide.VoidButtonSelector(true,-2f,-84,VoidButtonStep7ChuZhengDelegate);
	}
	static void VoidButtonStep7ChuZhengDelegate(GameObject go)
	{
		U3dCmn.SendMessage("WarSituationWin","NewbieJiashuRepair",null);
		U3dCmn.SendMessage("PopupAccelatePin","NewbieDoArmyAccel",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep7ArmyAccelSuccess()
	{
		processNewbieArmyAccel = null;
		processNewbieCombatRst = null;
		if (mGuide == null) return;
		
		// 时间到了 ...
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonSelector(true,200f,132f,VoidButtonStep8ChuZhengDelegate);
		mGuide.VoidFrame1Dimensions(70f,40f);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
	}
	
	static void VoidButtonStep8ChuZhengDelegate(GameObject go)
	{
		U3dCmn.SendMessage("WarSituationWin","DismissPanel",null);
		mGuide.GotoCStep_CP();
		
		mGuide.VoidSetManipIntro(0,-100f);
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
		mGuide.VoidFrame1Dimensions(80f,40f);
		mGuide.VoidButtonSelector(true,200f,140f,VoidButtonStep9ChuZhengDelegate);
	}
	static void VoidButtonStep9ChuZhengDelegate(GameObject go)
	{
		U3dCmn.SendMessage("QuestFeeLaunchWin", "OnQuestLaunchClose", null);
		
		// 下一个存储点 ....
		mGuide.GotoSavePoint_CP();
	}
}

// < 新手引导 ... 医院 > ... 
static public class NewbieYiGuan
{
	public static NewbieGuide mGuide = null;
	
	public delegate void NewbieTreatOneHeroRst();
	public static NewbieTreatOneHeroRst processTreatOneHeroRst = null;
	
	public static void init1()
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("CityMap","LocatePosition",(int)BuildingSort.MINJU);
				
		// 坐标修正 ...
		Vector2 pos1 = new Vector2(50f,-30f);
		if (U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD) {
			pos1 = pos1 + new Vector2(0f,-20f);
		} else if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5) {
			pos1 = pos1 + new Vector2(40f,0f);
		}
		
		mGuide.VoidButtonPage1(false,null);
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true, pos1.x, pos1.y, VoidButtonStep1YiGuanDelegate);
		mGuide.VoidButtonJiantou(0f, -40f, 0f);
		mGuide.VoidFrame1Dimensions(70f,60f);
		mGuide.Depth(BaizVariableScript.DEPTH_TOP1);
	}
	static void VoidButtonStep1YiGuanDelegate(GameObject go)
	{
		// 缓慢移动定位 ...
		U3dCmn.SendMessage("Hospital","OpenHospitalWin",null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonSelector(true, -88f,113f, VoidButtonStep2YiGuanDelegate);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
		mGuide.VoidFrame1Dimensions(90f,40f);

	}
	static void VoidButtonStep2YiGuanDelegate(GameObject go)
	{
		U3dCmn.SendMessage("HospitalWin","TabYiLiao",null);
		
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(true, VoidButtonStep3YiGuanDelegate);
		mGuide.VoidSetManipIntro(0f,82f);
		mGuide.VoidButtonSelector(true, -50f, -115f, null);
		mGuide.VoidFrame1Dimensions(300f,40f);
		mGuide.VoidButtonJiantouFalse();
	}
	static void VoidButtonStep3YiGuanDelegate(GameObject go)
	{
		mGuide.GotoCStep_CP();
		mGuide.VoidSetManipIntro(0f,-100f);
		mGuide.VoidButtonSelector(true, 164f, 68f, VoidButtonStep4YiGuanDelegate);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
		mGuide.VoidFrame1Dimensions(90f,40f);
	}
	static void VoidButtonStep4YiGuanDelegate(GameObject go)
	{
		processTreatOneHeroRst = VoidButtonStep4TreatOneHeroSuccess;
		U3dCmn.SendMessage("HospitalWin","NewbieTreatOneHero",null);
		//UIEventListener.Get(go).onClick = null;
	}
	static void VoidButtonStep4TreatOneHeroSuccess()
	{
		processTreatOneHeroRst = null;
		if (mGuide == null) return;
		
		// 接下一步引导 ....
		mGuide.GotoCStep_CP();
		mGuide.VoidButtonPage1(false, null);
		mGuide.VoidButtonJiantou(0f, -30f, 0f);
		mGuide.VoidButtonSelector(true,200f, 138f,VoidButtonStep5YiGuanDelegate);
		mGuide.VoidFrame1Dimensions(80f,40f);
	}
	static void VoidButtonStep5YiGuanDelegate(GameObject go)
	{
		U3dCmn.SendMessage("HospitalWin","DismissPanel", null);
		
		// 下一个存储点 ....
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieEnding
{
	public static NewbieGuide mGuide = null;
	public static void init1()
	{
		mGuide.VoidButtonPage1(true, VoidButtonStep1EndingDelegate);
		mGuide.VoidButtonSelector(false,0f,0f,null);
	}
	static void VoidButtonStep1EndingDelegate(GameObject go)
	{
		mGuide.GotoSavePoint_CP();
	}
}

static public class NewbieWaiting
{
	public delegate void NewbieWaitingDelegate();
	public static NewbieWaitingDelegate processNewbieWaiting = null;
	
	public static void ResetConn()
	{
		if (processNewbieWaiting != null)
		{
			processNewbieWaiting();
			processNewbieWaiting = null;
		}
	}
}