using UnityEngine;
using System;
using System.Collections;
using CTSCMD;
using STCCMD;
using CMNCMD;
public class TreatManager : MonoBehaviour {
	public static  Hashtable HurtHeroMap =  new Hashtable(); 
	public static int use_caoyao;
	// Use this for initialization
	void Start () {
	
	}
	
	//治疗武将 
	public static void ReqTreatHero(ulong[] heroid_array,int caoyao_num)
	{
		CTS_GAMECMD_USE_DRUG_T req;
		req.nCmd1 = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_USE_DRUG;
		req.nNum3 = heroid_array.Length;
		req.HeroIDs4 = heroid_array;
		TcpMsger.SendLogicData<CTS_GAMECMD_USE_DRUG_T >(req);
		use_caoyao = caoyao_num;
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_USE_DRUG);
	}
	//治疗武将返回 
	public void ReqTreatHeroRst(byte[] buff)
	{
		STC_GAMECMD_USE_DRUG_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_USE_DRUG_T>(buff);
		if(sub_msg.nRst1 == (byte)STC_GAMECMD_USE_DRUG_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_USE_DRUG_T>();
			int data_len = buff.Length - head_len;
			byte[] data_buff = new byte[data_len];
			Array.Copy(buff,head_len,data_buff,0,data_len);
			HeroDesc[] hero_list = (HeroDesc[])DataConvert.ByteToStructArray<HeroDesc>(data_buff,sub_msg.nNum2);
			for(int i=0;i<hero_list.Length;i++)
			{
				HeroDesc hero_unit = hero_list[i];
				if(!JiangLingManager.MyHeroMap.Contains(hero_unit.nHeroID10))
					return ;
				JiangLingManager.UpdateHeroInfo(hero_unit.nHeroID10,hero_unit);
				
			}
			if(CommonData.player_online_info.CaoYao<use_caoyao)
				CommonData.player_online_info.CaoYao = 0;
			else 
				CommonData.player_online_info.CaoYao -= (uint)use_caoyao;
			U3dCmn.SendMessage("HospitalWin","InitialData",null);
			
			// < 新手引导 治疗武将> ...
			if (NewbieYiGuan.processTreatOneHeroRst != null)
			{
				NewbieYiGuan.processTreatOneHeroRst();
				NewbieYiGuan.processTreatOneHeroRst = null;
			}
		}
		
		// <新手引导清空 > ....
		NewbieYiGuan.processTreatOneHeroRst = null;
	}
	//获取医馆的容量 
	public static int GetHospitalCapacity()
	{
		int capacity = 0;
		if(CommonMB.CaoYaoByHospital.Contains((int)Hospital.NowLevel))
			capacity = (int)CommonMB.CaoYaoByHospital[(int)Hospital.NowLevel];
		return capacity;
	}
	//计算草药需求量 
	public static int CalNeedCaoYao(int health,int hero_level)
	{
		int caoyao = 1;
		int hurt = 100-health;
		if(hurt>0)
			caoyao = (hurt-1)/(int)CommonMB.HeroTreatCaoYao[hero_level]+1;
		if(hurt>=20 && hurt<50)
		{
			caoyao *= 2;
		}
		else if(hurt>=50)
		{
			caoyao *= 3;
		}
		return caoyao;
	}
}
