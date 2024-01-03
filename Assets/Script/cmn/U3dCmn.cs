using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using CMNCMD;
using CTSCMD;
public class U3dCmn : MonoBehaviour 
{
	
	//名字库 
	static string[][] name_libray =null;

	//获取简单结构体的大小 不能是变长的结构体  
	public static int GetSizeofSimpleStructure<T>()
	{
		 T structure= default(T);
		object obj = structure as object;
		Type type=typeof(T);
		FieldInfo [] infos=type.GetFields();
		int infonum = infos.Length;
		FieldInfo [] sortinfos = new FieldInfo[infonum];
		for (int i=0; i<infonum; i++)
		{
			string strinfo =  infos[i].ToString();
			int num = -1;
			char [] strchararray = new char[strinfo.Length];
			strchararray = strinfo.ToCharArray();
			for (num = 1; num <=strchararray.Length; num++)
			{
				if (!Char.IsNumber(strchararray[strchararray.Length-num]))
					break;
			}
			num--;
			int	index = Convert.ToInt32 (strinfo.Substring(strinfo.Length-num,num));
		
			index--;
			sortinfos[index] = infos[i];
		}
		int length = 0;
		for (int i =0; i<infonum;i++)
		{
			if (!sortinfos[i].IsStatic)
			{
				if (sortinfos[i].FieldType ==typeof(byte))
				{
   						 length+=1;
				}
				else if (sortinfos[i].FieldType ==typeof(short) || sortinfos[i].FieldType ==typeof(ushort))
				{
   						 length+=2;
				}
				else if (sortinfos[i].FieldType ==typeof(uint) || sortinfos[i].FieldType ==typeof(int))
				{
   						 length+=4;
				}
				else if (sortinfos[i].FieldType ==typeof(long) || sortinfos[i].FieldType ==typeof(ulong))
				{
   						 length+=8;
				}
				else if (sortinfos[i].FieldType ==typeof(float))
				{
   						 length+=4;
				}
				else if(sortinfos[i].FieldType ==typeof(byte[]))
				{
						int len = (int) sortinfos[i-1].GetValue(obj);
						length += len;
				}
				else if(sortinfos[i].FieldType ==typeof(short[]))
				{
						int len = (int) sortinfos[i-1].GetValue(obj);
						len *=2;
						length += len;
				}
				else if(sortinfos[i].FieldType ==typeof(int[]))
				{
						int len = (int) sortinfos[i-1].GetValue(obj);
						len *=4;
						length += len;
				}
			}
		}
			
		return length;
	}
	//登陆或者重连后获取基本信息（建筑、个人信息等） 
	/*public static void GetBasicInfo()
	{
		PlayerInfoManager.GetPlayerInfo();
		BuildingManager.GetBuildingList();
		BuildingManager.GetBuildingTEList();
		TechnologyManager.GetTechInfo();
		JiangLingManager.GetJiangLingInfo();
		SoldierManager.GetSoldierInfo();
	}*/
	//向某个gameobject发送message
	public static void SendMessage(string obj_name,string method_name,object data)
	{
		GameObject obj = GameObject.Find(obj_name);
		if(obj != null)
		{
			obj.SendMessage(method_name,data);
		}
	}
	//获取可以写的目录 用于存储随APP永久保留的文件  
	public static string GetCachePath()
	{
	        string cachePath="";
			
			if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
				cachePath =  "cache/";
			}
			else if(Application.platform == RuntimePlatform.IPhonePlayer)
			{
				//for iPhone
				//path should be <Application_Home>/Library/Caches
				//app data Path is /<Application_Home>/AppName.app/Data		
			
				/*string dataPath = Application.dataPath;
				cachePath = dataPath.Replace ("/Data", "");
				//find index of "/" in /AppName.app
				int index = cachePath.LastIndexOf ("/");
				cachePath = cachePath.Substring (0, cachePath.Length - (cachePath.Length - index));
				cachePath = cachePath + "/Documents/";*/
				cachePath = Application.persistentDataPath+"/";
			}
			else
			{
				/*string dataPath = Application.dataPath;
				cachePath = dataPath.Replace ("/Data", "");
				//find index of "/" in /AppName.app
				int index = cachePath.LastIndexOf ("/");
				cachePath = cachePath.Substring (0, cachePath.Length - (cachePath.Length - index));
				cachePath = cachePath + "/Documents/";*/
				cachePath = Application.persistentDataPath+"/";
			}
		return cachePath;
	}
	//在可写目录中保存文本文件 
	public static void WriteText(string file,string text)
	{
		byte[] buff =  DataConvert.StrToBytes(text);
		FileStream fs = new FileStream(GetCachePath()+file,FileMode.Create);
		fs.Write(buff,0,buff.Length);
		fs.Close();
	}
	//在可写目录中读取文本文件 
	public static string ReadText(string file)
	{
		if(!File.Exists(GetCachePath()+file))
			return "";
		FileStream fs = new FileStream(GetCachePath()+file,FileMode.Open);
		
		byte[] buff = new byte[fs.Length];
		fs.Read(buff,0,(int)fs.Length);
		return DataConvert.BytesToStr(buff);
		fs.Close();
	}
	//是否是数字 
	public static bool IsNumeric(string str)   
	{   
	    if (str == null || str.Length == 0)   
	        return false;   
	    System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();   
	    byte[] bytestr = ascii.GetBytes(str);   
	    foreach (byte c in bytestr)   
	    {   
	        if (c < 48 || c > 57)   
	        {   
	            return false;   
	        }   
	    }   
	    return true;   
	}  
	//获取随机名字 
	public static string GetRandomName(bool isboy)
	{
		if(name_libray == null)
		{
			name_libray = CommonMB.GetStringFromMBText(MBAutoUpdate.localDir + "player_name",false);
			//CommonMB.GetStringFromMBText(MBAutoUpdate.localDir + "player_name");
			//TextAsset text_asset = (TextAsset)Resources.Load("MB/player_name",typeof(TextAsset));
			//byte[] dat = DataConvert.StrToBytes(text_asset.text);
			//name_libray = MBDeal.readMBFromBytes(dat,false,false);
			
		}
		
		int[] prob = { 15, 5, 40, 40 };
        RandomName rn = new RandomName(name_libray, prob);
		return rn.gen(isboy);
		
	}
	//获取随机英文名字  
	public static string GetRandomNameEn(bool isboy, bool isRandSex)
	{
		if (name_libray == null)
		{
			name_libray	= CommonMB.GetStringFromMBText(MBAutoUpdate.localDir + "player_name_en", false);
		}
		RandomNameEn rn	= new RandomNameEn(name_libray);
		return rn.gen(isboy, isRandSex);
	}
	//根据MB表格中的基数和增长系数算出不同等级下需要的数值 
	public static float CalculateValueFromMB(uint level,float basevalue,float scale)
	{
		return basevalue*(1+(level*scale));
	}
	
	//计算当前民居的总容量 
	public static int GetMinJuCapacity()
	{
		int num =0;
		foreach(DictionaryEntry	de	in BuildingManager.MinJu_Capacity_Map)  
  		{
			num += (int)de.Value;
		}
		return num;
	}
	//计算当前民居的每小时生产量 
	public static int GetMinJuProduct()
	{
		int num =0;
		foreach(DictionaryEntry	de	in BuildingManager.MinJu_Product_Map)  
  		{
			num += (int)de.Value;
		}
		return num;
	}
	//设置gameobj是否可见（包括子OBJ） 
	public static GameObject  SetObjVisible(GameObject father_obj,string name,bool visible)
	{
		string objname= name;
		
		GameObject obj = GetChildObjByName(father_obj,objname);
		if(obj!=null)
		{
			obj.SetActiveRecursively(visible);
		}
		return obj;
	}

	//根据gameobj的名字从一级children中获取gameobject 
	public static GameObject GetChildObjByName(GameObject obj,string name)
	{
		GameObject child_obj = null;
		/*foreach(Transform childobj in obj.transform)
		{
			if(childobj.gameObject.name == name)
				return childobj.gameObject;
		}*/
		Transform[] trans = obj.GetComponentsInChildren<Transform>(true);
		for(int i=0;i<trans.Length;i++)
		{
			if(trans[i].gameObject.name == name)
			{
				child_obj = trans[i].gameObject;
				return child_obj;
			}
		}
		return child_obj;
	}
	//根据gameobj的名字从prefab生成GAMEOBJECT 
	public static GameObject GetObjFromPrefab(string objname)
	{
		GameObject obj = GameObject.Find(objname);
		if(obj == null)
		{
			GameObject obj_prefab = (GameObject)Resources.Load("Prefab/"+objname,typeof(GameObject));
			if(obj_prefab !=null)
			{
				obj = (GameObject)Instantiate(obj_prefab);
				obj.name = objname;
			}
		}
		return obj;
	}
	//根据武将头像ID获取图片名称 
	public static string GetHeroIconName(int model_id)
	{
		if(model_id <=0)
			model_id =1;
		return "hero"+model_id;
	}
	//根据君主头像ID获取简单图片名称 
	public static string GetCharSmpIconName(int model_id)
	{
		if(model_id <=0)
			model_id =1;
		return "junzhu"+model_id;
	}
	//根据君主头像ID获取图片名称 
	public static string GetCharIconName(int model_id)
	{
		if(model_id <=0)
			model_id =1;
		return "junzhu"+model_id;
	}
	//获取警告或者错误提示 用于弹出框提示  
	public static string GetWarnErrTipFromMB(int tipid)
	{
		string tip_str = "unknown err";
		if(tipid == (int)MB_WARN_ERR.NET_ERR)
		{
			tip_str = "Net error,please check internet";
			return tip_str;
		}
		if(CommonMB.Warn_Err_Tips_Map.Contains(tipid))
		{
			
			tip_str =(string)CommonMB.Warn_Err_Tips_Map[tipid];
			//print ("get err from mb");
		}
		//if(tipid == (int)MB_WARN_ERR.NET_ERR && LoadingManager.mInst!=null)
		//{
		//	LoadingManager.mInst.HideLoading();
		//}
		return tip_str;
	}
	//弹出警告提示框 
	public static void ShowWarnWindow(string content)
	{
		
		GameObject PopWarnWin = GameObject.Find("PopWarnWin");
		if(PopWarnWin == null)
		{
			GameObject obj_prefab = (GameObject)Resources.Load("Prefab/PopWarnWin",typeof(GameObject));
			if(obj_prefab !=null)
				PopWarnWin = (GameObject)Instantiate(obj_prefab);
			if(PopWarnWin == null)
				return ;
		}
		PopWarnWin.name = "PopWarnWin";
		PopWarnWin.SendMessage("RevealPanel",content);
		LoadingManager.instance.HideLoading();
		//Instantiate();
		//SendMessage("ShowWarnWindow",context);	
		
	}
	public static void ShowQuitWindow(string content)
	{
		GameObject PopWarnWin = GameObject.Find("QuitWin");
		if(PopWarnWin == null)
		{
			GameObject obj_prefab = (GameObject)Resources.Load("Prefab/QuitWin",typeof(GameObject));
			if(obj_prefab !=null)
				PopWarnWin = (GameObject)Instantiate(obj_prefab);
			if(PopWarnWin == null)
				return ;
		}
		PopWarnWin.name = "QuitWin";
		PopWarnWin.SendMessage("RevealPanel",content);
		LoadingManager.instance.HideLoading();
	}
	//弹出充值提示框 
	public static void ShowRechargePromptWindow()
	{
		
		GameObject PopWarnWin = GetObjFromPrefab("PopLackDiamondWin");
		if(PopWarnWin != null)
		{
			PopWarnWin.SendMessage("RevealPanel");
			PopWarnWin.name = "PopLackDiamondWin";
		}
		
		//Instantiate();
		//SendMessage("ShowWarnWindow",context);	
		
	}
	//弹出网络连接错误提示框（可选择重连和退出的）  
	public static void ShowNetErrWindow()
	{
		GameObject Win = GameObject.Find("NetErrWin");
		if(Win == null)
		{
			GameObject obj_prefab = (GameObject)Resources.Load("Prefab/NetErrWin",typeof(GameObject));
			if(obj_prefab !=null)
				Win = (GameObject)Instantiate(obj_prefab);
			if(Win == null)
				return ;
		}
		Win.name = "NetErrWin";
		Win.SendMessage("RevealPanel");
		LoadingManager.instance.HideLoading();
		LoadingManager.instance.CmdListClear();
		//Instantiate();
		//SendMessage("ShowWarnWindow",context);	
		
	}
	//弹出君主信息窗口 
	public static void ShowPlayerInfoWindow()
	{
		GameObject PopWin = GetObjFromPrefab("PlayerInfoWin");
		if(PopWin !=null)
		{
			PopWin.SendMessage("RevealPanel");
		}
		
	}
	//根据等级和ID从建筑信息MAP表中获取建筑信息
	public static BuildInfo GetBuildingInfoFromMb(int ID,int Level)
	{
		BuildInfo info = new BuildInfo();
		if(CommonMB.BuildingInfo_Map.Contains(ID))
		{
			Hashtable table = (Hashtable)CommonMB.BuildingInfo_Map[ID];
			if(table.Contains(Level))
				info = (BuildInfo)table[Level];
		}
		return info;
	}
	//根据兵种EXCEL_ID和等级获取兵种信息 
	public static SoldierMBInfo GetSoldierInfoFromMb(uint ExcelID,uint Level)
	{
		SoldierMBInfo info = new SoldierMBInfo();
		if(CommonMB.SoldierMB_Map.Contains(ExcelID))
		{
			Hashtable table = (Hashtable)CommonMB.SoldierMB_Map[ExcelID];
			if(table.Contains(Level))
				info = (SoldierMBInfo)table[Level];	
		}
		return info;
	}
	//根据秒数返回时间格式化字符串 
	public static string GetTimeStrFromSec(int sec)
	{
		string str = "";
		int hour = sec/3600;
		int minute = sec/60%60;
		int second = sec%60;
		if(hour>=100)
			str =string.Format("{0}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
		else
			str =string.Format("{0:D2}", hour)+":"+string.Format("{0:D2}", minute)+":"+string.Format("{0:D2}", second);
		return str;
	}
	//根据ITEM数量和每个页面的数量获取页面数   
	public static int GetPageNum(int num,int num_per_page)
	{
		int pagenum = 1;
		if(num>0)
			pagenum = (num-1)/num_per_page+1;
			
		return pagenum;
	}
	//根据EXCELID获取物品的图片名字 
	public static ITEM_INFO GetItemIconByExcelID(uint excel_id)
	{
		ITEM_INFO item_info =  new ITEM_INFO();
		item_info.ExcelID = excel_id;
		if(!CommonMB.ExcelToSort_Map.Contains(excel_id))
			return item_info;
		uint sort_id = (uint)CommonMB.ExcelToSort_Map[excel_id];
		if(sort_id == (uint)ItemSort.EQUIP)
		{
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[(uint)excel_id];
			item_info.Name = equip_info.EquipName;
			item_info.Describe = equip_info.EquipDescribe;
			item_info.IconName = equip_info.IconName;
		}
		else if(sort_id == (uint)ItemSort.GEMSTONE)
		{
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[(uint)excel_id];
			item_info.Name = gem_info.GemName;
			item_info.Describe = gem_info.GemDescribe;
			item_info.IconName = gem_info.IconName;
		}
		else if(sort_id == (uint)ItemSort.ARTICLES)
		{
			ArticlesInfo articles_info = (ArticlesInfo)CommonMB.Articles_Map[(uint)excel_id];
			item_info.Name = articles_info.Name;
			item_info.Describe = articles_info.Describe;
			item_info.IconName = articles_info.IconName;
		}
		else if(sort_id == (uint)ItemSort.TASKITEM)
		{
			TaskInfo task_info = (TaskInfo)CommonMB.Task_Map[(uint)excel_id];
			item_info.Name = task_info.Name;
			item_info.Describe = task_info.Describe;
			item_info.IconName = task_info.IconName;
		}
		return item_info;
		
	}
	//根据兵种的EXCEL_ID获取图标名字  
	public static string GetIconFromSoldierID(uint excel_id)
	{
		string str = "";
		if(excel_id == (uint)SoldierSort.GONGBING)
		{
			str =  "gongbing";
		}
		else if(excel_id == (uint)SoldierSort.JIANSHI)
		{
			str =  "jianshi";
		}
		else if(excel_id == (uint)SoldierSort.QIANGBING)
		{
			str =  "qiangbing";
		}
		else if(excel_id == (uint)SoldierSort.QIBING)
		{
			str =  "qishi";
		}
		else if(excel_id == (uint)SoldierSort.QIXIE)
		{
			str =  "qixie";
		}
		return str;
	}
	//根据建筑ID获取当前等级 
	public static uint GetBuildingLevelByID(int building_id)
	{
		uint level = 0;
		if((int)BuildingSort.CHENGBAO == building_id)
		{
			level = ChengBao.NowLevel;
		}
		else if((int)BuildingSort.DASHIGUAN == building_id)
		{
			level = DashiGuan.NowLevel;
		}
		else if((int)BuildingSort.JIUGUAN == building_id)
		{
			level = JiuGuan.NowLevel;
		}
		else if((int)BuildingSort.XIULIAN == building_id)
		{
			level = TrainGround.NowLevel;
		}
		else if((int)BuildingSort.YANJIUYUAN == building_id)
		{
			level = Institute.NowLevel;
		}
		else if((int)BuildingSort.YIGUAN == building_id)
		{
			level = Hospital.NowLevel;
		}
		return level;
	}
	//根据商品信息组装TIP字符串 
	public static string GetTipByExcelID(uint excel_id)
	{
		string tipstr = "";
		uint sort_id = (uint)CommonMB.ExcelToSort_Map[excel_id];
		if(sort_id == (uint)ItemSort.EQUIP)
		{
			EquipInfo equip_info = (EquipInfo)CommonMB.EquipInfo_Map[excel_id];
			if(equip_info.Attack!=0)
			{
				tipstr += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_ATTACK)+" [006500]+"+equip_info.Attack+"[-]\n";
			}
			if(equip_info.Defense!=0)
			{
				tipstr += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_DEFENSE)+" [006500]+"+equip_info.Defense+"[-]\n";
			}
			if(equip_info.Life!=0)
			{
				tipstr += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_LIFE)+" [006500]+"+equip_info.Life+"[-]\n";
			}
			if(equip_info.Leader!=0)
			{
				tipstr += U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.HERO_LEADER)+" [006500]+"+equip_info.Leader+"[-]\n";
			}
		}
		else if(sort_id == (uint)ItemSort.GEMSTONE)
		{
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[excel_id];
			if(gem_info.Attack!=0)
			{
				tipstr = gem_info.GemName+"  [006500]"+"ATK+"+gem_info.Attack+"[-]";
			}
			else if(gem_info.Defense!=0)
			{
				tipstr = gem_info.GemName+"  [006500]"+"DEF+"+gem_info.Defense+"[-]";
			}
			else if(gem_info.Life!=0)
			{
				tipstr = gem_info.GemName+"  [006500]"+"HP+"+gem_info.Life+"[-]";
			}
			else if(gem_info.Leader!=0)
			{
				tipstr = gem_info.GemName+"  [006500]"+"COM+"+gem_info.Leader+"[-]";
			}
		}
		return tipstr;
	}
	//根据宝石信息组装简单TIP字符串 
	public static string GetGemTipByExcelID(uint excel_id)
	{
		string tipstr = "";
		uint sort_id = (uint)CommonMB.ExcelToSort_Map[excel_id];
		if(sort_id == (uint)ItemSort.GEMSTONE)
		{
			GemInfo gem_info = (GemInfo)CommonMB.Gem_Map[excel_id];
			if(gem_info.Attack!=0)
			{
				tipstr = "ATK+"+gem_info.Attack;
			}
			else if(gem_info.Defense!=0)
			{
				tipstr = "DEF+"+gem_info.Defense;
			}
			else if(gem_info.Life!=0)
			{
				tipstr = "HP+"+gem_info.Life;
			}
			else if(gem_info.Leader!=0)
			{
				tipstr = "COM+"+gem_info.Leader;
			}
		}
		return tipstr;
	}
	//根据君主等级获取将领最大数量 
	public static int GetHeroMaxNumByCharLevel(uint level)
	{
		int num =0;
		uint now_level = 0;
		ArrayList list = new ArrayList(CommonMB.HeroNum_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			uint char_level = (uint)list[i];
			if(char_level > level)
			{
				break;
			}
			else
			{
				now_level = char_level;
			}
		}
		
		if(CommonMB.HeroNum_Map.Contains(now_level))
			num = (int)CommonMB.HeroNum_Map[now_level];
		return num;
	}
	//获取下一次可以增加将领数量的君主等级 
	public static uint	GetNextCharLevelForHero(uint level)
	{
		uint next_level = 0;
		ArrayList list = new ArrayList(CommonMB.HeroNum_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			uint char_level = (uint)list[i];
			if(char_level > level)
			{
				next_level = char_level;
				break;
			}
		}
		return next_level;
	}
	//获取下一次可以建造（民居、兵营、金矿）的数据 
	public static BuildingNum GetNextCharLevelForBuilding(int building_type,int building_num)
	{
		BuildingNum building_info = new BuildingNum();
		ArrayList list = new ArrayList(CommonMB.BuildingNum_Map.Keys);
      	list.Sort();
		if(building_type== (int)BuildingSort.JINKUANG)
		{
			
		}
		uint now_level = 0;
		for(int i=0;i<list.Count;i++)
		{
			uint char_level = (uint)list[i];
			BuildingNum building_num_info = (BuildingNum)CommonMB.BuildingNum_Map[char_level];
			if(building_type== (int)BuildingSort.JINKUANG)
			{
				if(building_num_info.GoldNum >building_num)
				{
					now_level = char_level;
					break;
				}
			}
			else if(building_type== (int)BuildingSort.BINGYING)
			{
				if(building_num_info.CampNum >building_num)
				{
					now_level = char_level;
					break;
				}
			}
			else if(building_type== (int)BuildingSort.MINJU)
			{
				if(building_num_info.HouseNum >building_num)
				{
					now_level = char_level;
					break;
				}
			}
		}
		
		if(now_level != 0)
		{
			building_info = (BuildingNum)CommonMB.BuildingNum_Map[now_level];
		}
		//CommonMB.BuildingNum_Map
		return building_info;	
	}
	//根据君主等级获得金矿、民居、军营的数量 
	public static BuildingNum GetBuildingNumByCharLevel(uint level)
	{
		BuildingNum building_num_info = new BuildingNum();
		uint now_level = 0;
		ArrayList list = new ArrayList(CommonMB.BuildingNum_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			uint char_level = (uint)list[i];
			if(char_level > level)
			{
				break;
			}
			else
			{
				now_level = char_level;
			}
		}
		if(CommonMB.BuildingNum_Map.Contains(now_level))
			building_num_info = (BuildingNum)CommonMB.BuildingNum_Map[now_level];
		return building_num_info;
	}
	//根据数值返回字符串（大于10000用万表示） 
	public static string GetNumStr(uint num)
	{
		string str = "";
		if(num<=100000)
		{
			str = num.ToString();
		}
		else 
		{
			str = (num/1000)+GetWarnErrTipFromMB((int)MB_WARN_ERR.TEN_THOUSAND);
		}
		return str;
	}
	//返回用千表示的数字串 
	public static string GetThousandNumStr(uint num)
	{
		string str = "";
		if(num<1000)
		{
			str = num.ToString();
		}
		else 
		{
			str = (num/1000)+GetWarnErrTipFromMB((int)MB_WARN_ERR.TEN_THOUSAND);
		}
		return str;
	}
	//根据加速需要的钻石计算出打折后的钻石数 
	public static int GetAccelerateDiscount(int diamond)
	{
		int discountnum = 0;
		ArrayList list = new ArrayList(CommonMB.Accelereate_Discount_Map.Keys);
      	list.Sort();
		if(diamond<=(int)list[0])
		{
			discountnum+= diamond;
		}
		else 
		{
			discountnum+=(int)list[0];
		}
		for(int i=0;i<list.Count-1;i++)
		{
			int down_num = (int)list[i];
			int up_num = (int)list[i+1]; 
			float rate = (float)CommonMB.Accelereate_Discount_Map[down_num];
			if(diamond>down_num && diamond<=up_num)
			{
				discountnum += UpperInt((diamond - down_num)*rate);
			}
			else if(diamond>up_num)
			{
				discountnum+= UpperInt((up_num - down_num)*rate);
			}
		}
		if(diamond>(int)list[list.Count-1])
		{
			int down_num = (int)list[list.Count-1];
			float rate = (float)CommonMB.Accelereate_Discount_Map[down_num];
			discountnum+=  UpperInt((diamond - down_num)*rate);;
		}
		
		return discountnum;
	}
	//根据君主等级获取单笔可以寄卖的最大金币数 
	public static int GetGoldSellNumByCharLevel(uint level)
	{
		int num =0;
		uint now_level = 0;
		ArrayList list = new ArrayList(CommonMB.GoldMarketLevel_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			uint char_level = (uint)list[i];
			if(char_level > level)
			{
				break;
			}
			else
			{
				now_level = char_level;
			}
		}
		
		if(CommonMB.GoldMarketLevel_Map.Contains(now_level))
		{
			GoldMarketLevel unit  = (GoldMarketLevel)CommonMB.GoldMarketLevel_Map[now_level];
			num = unit.GoldSellMax;
		}
			
		return num;
	}
	//获取下一次可以增加将领数量的君主等级 
	public static GoldMarketLevel GetNextLevelGoldSell(uint level)
	{
		uint next_level = 0;
		ArrayList list = new ArrayList(CommonMB.GoldMarketLevel_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			uint char_level = (uint)list[i];
			if(char_level > level)
			{
				next_level = char_level;
				break;
			}
		}
		GoldMarketLevel unit = new GoldMarketLevel();
		if(CommonMB.GoldMarketLevel_Map.Contains(next_level))
			unit = (GoldMarketLevel)CommonMB.GoldMarketLevel_Map[next_level];
		return unit;
	}
	//浮点数取上限 
	public static int UpperInt(float num)
	{
		int rst = (int)num;
		if(num - (int)num > 0)
		{
			rst++;
		}
		return rst;
	}
	//把NULL字符串转换为"" 
	public static string CheckNullStr(string str)
	{
		string return_str = str;
		if(str == null)
			return_str = "";
		return return_str;
	}
	//检查字符串是否含有非法字符 
	public static bool IsContainBadName(string check_str)
	{
		
		bool contain = false;
		if(check_str.Contains(" ") || check_str.Contains("	") || check_str.Contains("\""))
		{
			contain = true;
			return contain;
		}
		for(int i=0;i<CommonMB.BadNameList.Count;i++)
		{
			string str = CommonMB.BadNameList[i];
			if(check_str.Contains(str))
			{
				contain = true;
				break;
			}
		}
		
		return contain;
	}
	//检查版本是否一致 
	public static bool CheckLittleVersion(string client_version,string server_version)
	{
	//	print (client_version);
	//	print (server_version);
		bool rst = true;
		if(client_version == server_version)
		{
			rst =  true;
		}
		else 
		{
			rst = false;
		}
		return rst;
	}
	//根据当前钱数查找下一级VIP数据 
	public static VipRightInfo GetNextVipInfo(int now_diamond)
	{
		
		VipRightInfo vip_info = new VipRightInfo();
		ArrayList list = new ArrayList(CommonMB.VipRightInfo_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			VipRightInfo info = (VipRightInfo)CommonMB.VipRightInfo_Map[(int)list[i]];

			if(info.NeedDiamond > now_diamond)
			{
				vip_info = info;
				break;
			}
		}
	
		return vip_info;
	}
	//判断当前设备是IPHONE还是IPAD 
	public static DEVICE_TYPE GetDeviceType()
	{	
		DEVICE_TYPE device_type = DEVICE_TYPE.IPHONE;
		#if UNITY_3_5_6
		if(iPhone.generation == iPhoneGeneration.iPad1Gen || iPhone.generation == iPhoneGeneration.iPad2Gen || iPhone.generation == iPhoneGeneration.iPad3Gen)
		{
			device_type = DEVICE_TYPE.IPAD;
		}
		else 
		{
			device_type = DEVICE_TYPE.IPHONE;
		}
		#endif
		//device_type = DEVICE_TYPE.IPAD;
		return device_type;
	}
	//获取IPHONE TOUCH的种类 
	public static IPHONE_TYPE GetIphoneType()
	{
		IPHONE_TYPE iphone_type = IPHONE_TYPE.IPHONE;
		#if UNITY_3_5_6
		if(iPhone.generation == iPhoneGeneration.iPhone5 || iPhone.generation == iPhoneGeneration.iPodTouch5Gen)
		{
			iphone_type = IPHONE_TYPE.IPHONE5;
		}
		else
		{
			iphone_type = IPHONE_TYPE.IPHONE;
		}
		#endif
		return iphone_type;
	}
	//字符是否为数字或者英文 
	public static bool IsNumOrLetter(string str)
    {
        System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
        return reg1.IsMatch(str);
    }
	//检查邮箱格式 
	public static bool CheckMailFormat(string mail_str)
	{
		bool rst = true;
		if(!mail_str.Contains("@"))
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MAIL_ADDRESS_WRONG));	
			rst = false;
		}
		else if(DataConvert.StrToBytes(mail_str).Length >32)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.MAIL_ACCOUNT_TOO_LONG));	
			rst =false;
		}
		return rst;
	}
	//检查密码格式是否正确  是否为数字或字母  长度是否合适 
	public static bool CheckPassFormat(string pass_str)
	{
		bool rst = true;
		if(!U3dCmn.IsNumOrLetter(pass_str))
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_ISONLY_NUM_LETTER));
			rst = false;
		}
		else if(pass_str.Length <6)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_TOO_SHORT));
			rst = false;
		}
		else if(pass_str.Length >20)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_TOO_LONG));
			rst = false;
		}
		return rst;
	}
	//根据EXCELID获取兵种图标 
	public static string GetTroopIconFromID(int ExcelID)
	{
		string troop_name= "transparent";
		if(ExcelID >=1 && ExcelID <= 5)
			troop_name = "troop"+ExcelID;
		return 	troop_name;
	}
	//根据EXCELID获取兵种图标(带底图的图标)  
	public static string GetTroopBigIconFromID(int ExcelID)
	{
		string troop_name= "transparent";
		if(ExcelID >=1 && ExcelID <= 5)
			troop_name = "troopicon"+ExcelID;
		return 	troop_name;
	}
	//获取透明图片  
	public static string GetTransparentPic()
	{
		string pic_name = "transparent";
		
		return 	pic_name;
	}
	//根据KEY获取CMN配置的相应数字 
	public static int GetCmnDefNum(uint key)
	{
		int rst = 0;
		Hashtable sMap = CommonMB.CmnDefineMBInfo_Map;
		if (true == sMap.ContainsKey(key))
		{
			CmnDefineMBInfo cmndef = (CmnDefineMBInfo)sMap[key];
			rst = (int)cmndef.num;
		}
		return rst;
	}
	//获取征战天下最大的关卡数 
	public static int GetWorldConquestMaxLv()
	{
		int max = 10;
		foreach(DictionaryEntry de in CommonMB.NanBeizhanInfo_Map)
		{
			uint lv = (uint)de.Key;
			if(lv >max)
				max = (int)lv;
		}
		return max;
	}
	//根据当前增兵数查找下一级联盟等级数据 
	public static GuildReinforceInfo GetNextGuildReinforceInfo(int reinforce_num)
	{
		
		GuildReinforceInfo reinforce_info = new GuildReinforceInfo();
		ArrayList list = new ArrayList(CommonMB.GuildReinforceInfo_Map.Keys);
      	list.Sort();
		
		for(int i=0;i<list.Count;i++)
		{
			GuildReinforceInfo info = (GuildReinforceInfo)CommonMB.GuildReinforceInfo_Map[(uint)list[i]];

			if(info.ReinforceMax > reinforce_num)
			{
				reinforce_info = info;
				break;
			}
		}
	
		return reinforce_info;
	}
	//从服务器MB中根据APPid获取版本号 
	public static string GetServerVersionByAppID(int AppID)
	{
		string version = "";
		if(CommonMB.ServerVersion_Map.Contains(AppID))
		{
			ServerVersion info = (ServerVersion)CommonMB.ServerVersion_Map[AppID];
			version = info.Version;
		}
		return version;
	}
	//数组转换为LIST 
	public static void ArrayToList<T>(T[] from_array,List<T> des_list)
	{
		des_list.Clear();
		for(int i= 0;i<from_array.Length;i++)
		{
			des_list.Add(from_array[i]);
		}
	}
	//打开君主信息简洁窗口 
	public static void OpenPlayerCardWin(ulong account_id)
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
		if (obj == null) return;			
		
		obj.SendMessage("RevealPanel",account_id);
	}

    public static void ScaleScreen(Transform transform){
        if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
    }
}
