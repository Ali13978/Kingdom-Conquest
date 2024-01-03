using UnityEngine;
using System.Collections;
using CMNCMD;
public class SceneManager : MonoBehaviour {
	//obj哈希表 TAG做主键 
	static Hashtable  TagObj_Map = new Hashtable();
	bool check_register = false;
	public tk2dAnimatedSprite win_animation;
	// Use this for initialization
	void Start () {
		LoadingManager.instance.ShowLoading();
		LoadingManager.instance.LoginAddInitialData();
		Resources.UnloadUnusedAssets();
		SoundManager.StopLoginMusic();
		SoundManager.PlayMainSceneSound();
		win_animation.gameObject.SetActiveRecursively(false);
	}
	
	//根据TAG设置Gameobject是否可见   
	public static void SetObjVisibleByTag(string tag,bool visible)
	{
		
		if(TagObj_Map.Contains(tag))
		{
			GameObject[] objs = (GameObject[])TagObj_Map[tag];
			for(int i=0;i<objs.Length;i++)
			{
				objs[i].SetActiveRecursively(visible);
			}
		}
		else 
		{
			GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
			TagObj_Map.Add(tag,objs);
			for(int i=0;i<objs.Length;i++)
			{
				objs[i].SetActiveRecursively(visible);
			}
			
		}
	}
	//出城 进入世界地图  
	void OpenGlobalMap()
	{
		LoadingManager.instance.ShowLoading();
		
		// 全局地图定位的变量 ...
		GlobeMapLocation.byFastLocation = 0;
		
		DontDestroyOnLoad(GameObject.Find("TcpManager"));
		
		// 异步加载 ...
		StartCoroutine(WorldMap_LoadScene());
		Resources.UnloadUnusedAssets();
	}
	
	// 定位名城位置 ...
	public static void OpenGlobalMapLocation (int PosX, int PosY)
	{
		GameObject go = GameObject.Find("_TiledStorgeCacheData");
		if (go == null)
		{
			LoadingManager.instance.ShowLoading();
			
			// 全局地图定位的变量 ...
			GlobeMapLocation.byFastLocation = 1;
			GlobeMapLocation.PosX = PosX;
			GlobeMapLocation.PosY = PosY;
			
			DontDestroyOnLoad(GameObject.Find("TcpManager"));
			Application.LoadLevelAdditive("Tiled");
			Resources.UnloadUnusedAssets();
		}
		else 
		{
			TiledStorgeCacheData.instance.GPSSetLocation(PosX, PosY);
		}
	}
	
	//进入PK地图 
	void OpenPKScene()
	{

	}
	
	//弹出消息界面 
	void OpenBaizMilitaryWin()
	{

	}
	
	IEnumerator WorldMap_LoadScene()
	{
		// 异步加载世界地图 ...
		AsyncOperation async = Application.LoadLevelAdditiveAsync("Tiled");
		yield return async;
	}
	//登陆过程结束 
	void LoginOver()
	{
		if(CommonData.VERSION == VERSION_TYPE.VERSION_APPSTORE)
		{
			//苹果版本检查一下是否绑定邮箱 
			if(!check_register)
			{
				if(CommonData.player_online_info.BindMail == 0 &&(CommonData.player_online_info.TotalDiamond >0 || CommonData.player_online_info.Level>=10))
				{
					//print ("nihaomeassssssssssss");
					U3dCmn.SendMessage("91Manager","getLoginMacAddress", null);
					check_register = true;
					return ;
				}
				
			}	
		}
		
		bool pop = false;
		CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
		string gCol = string.Format(TaskManager.QUEST_POP,info.AccountName);
		int BooDone = PlayerPrefs.GetInt(gCol, 0);
		if (BooDone != 1) 
		{
			pop = true;
			
		}
		else 
		{
			pop = QuestGuideIcon.CheckQuestOver();
		
		}
		if(pop)
		{
			if(CommonData.player_online_info.FreeDrawLotteryNum >0)
			{
				GameObject win = U3dCmn.GetObjFromPrefab("ZhuanPan"); 
				if(win != null)
				{
					win.SendMessage("RevealPanel");
				}
			}
		}
		
	}
	//打开WIN或者LOSE动画 
	void OpenWinLoseAnimation(bool is_win)
	{
		win_animation.transform.localPosition += new Vector3(100,0,0);
		string clipName = "anim-victory";
		if(is_win)
		{
			clipName = "anim-victory";   
			
		}
		else 
		{
			 clipName = "anim-failure";
		}
		win_animation.animationCompleteDelegate = AnimComplete;
		win_animation.spriteId = win_animation.GetSpriteIdByName(clipName+"/0");
		win_animation.Play(clipName);
		win_animation.gameObject.SetActiveRecursively(true);
	}
	void AnimComplete(tk2dAnimatedSprite sprite, int clipId)
    {
        win_animation.gameObject.active = false;
		win_animation.transform.localPosition += new Vector3(-100,0,0);
    }
}
