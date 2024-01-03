using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CTSCMD;
using STCCMD;
using CMNCMD;

public class FavoriteManager : MonoBehaviour {
	int total_num = 0;
	public  List<PositionMark> FavoriteDataList = new List<PositionMark>();
	PositionMark position_data;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	static FavoriteManager _instance;
    private FavoriteManager() { }
    static FavoriteManager()
    {
        _instance = new FavoriteManager();
    }
    public static FavoriteManager Instance
    {
        get { return _instance; }
    }
	//收藏坐标  
	public void AddFavoriteData(PositionMark data)
	{
		CTS_GAMECMD_POSITION_MARK_ADD_RECORD_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_POSITION_MARK_ADD_RECORD;
		req.nPosX3 = data.nPosX1;
		req.nPosY4 = data.nPosY2;
		req.nHeadID5 = data.nHeadID3;
		req.nLength6 = data.nLength4;
		req.szName7 = data.szName5;
		TcpMsger.SendLogicData<CTS_GAMECMD_POSITION_MARK_ADD_RECORD_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_POSITION_MARK_ADD_RECORD);
	}
	//收藏坐标返回 
	public void AddFavoriteDataRst(byte[] buff)
	{
		STC_GAMECMD_POSITION_MARK_ADD_RECORD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_POSITION_MARK_ADD_RECORD_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_POSITION_MARK_ADD_RECORD_T.enum_rst.RST_OK)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.COLLECT_SUCCESS));
			
		}
		else if(sub_msg.nRst1 == (int)STC_GAMECMD_POSITION_MARK_ADD_RECORD_T.enum_rst.RST_FULL)
		{
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.FAVORITE_IS_FULL));	
		}
	}
	
	//删除坐标 
	public void DeleteFavoriteData(PositionMark data)
	{
		position_data = data;
		CTS_GAMECMD_POSITION_MARK_DEL_RECORD_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_POSITION_MARK_DEL_RECORD;
		req.nPosX3 = data.nPosX1;
		req.nPosY4 = data.nPosY2;
		TcpMsger.SendLogicData<CTS_GAMECMD_POSITION_MARK_DEL_RECORD_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_POSITION_MARK_DEL_RECORD);
	}
	//删除坐标返回  
	public void DeleteFavoriteDataRst(byte[] buff)
	{
		STC_GAMECMD_POSITION_MARK_DEL_RECORD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_POSITION_MARK_DEL_RECORD_T>(buff);
		if(sub_msg.nRst1 == (int)STC_GAMECMD_POSITION_MARK_DEL_RECORD_T.enum_rst.RST_OK)
		{
			FavoriteDataList.Remove(position_data);
			U3dCmn.SendMessage("FavoriteWin","InitialPanelData",null);
		}
	}
	//获取收藏夹数据 
	public void GetFavoriteData()
	{
		CTS_GAMECMD_POSITION_MARK_GET_RECORD_T req;
		req.nCmd1     = (byte)CTS_MSG.TTY_CLIENT_LPGAMEPLAY_GAME_CMD;
		req.nGameCmd2 = (uint)SUB_CTS_MSG.CTS_GAMECMD_POSITION_MARK_GET_RECORD;
		TcpMsger.SendLogicData<CTS_GAMECMD_POSITION_MARK_GET_RECORD_T>(req);
		LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_POSITION_MARK_GET_RECORD);
	}
	//获取收藏夹数据返回 
	public void GetFavoriteDataRst(byte[] buff)
	{
		STC_GAMECMD_POSITION_MARK_GET_RECORD_T sub_msg = DataConvert.ByteToStruct<STC_GAMECMD_POSITION_MARK_GET_RECORD_T>(buff);
		
		if(sub_msg.nRst1 == (int)STC_GAMECMD_POSITION_MARK_GET_RECORD_T.enum_rst.RST_OK)
		{
			int head_len = U3dCmn.GetSizeofSimpleStructure<STC_GAMECMD_POSITION_MARK_GET_RECORD_T>();
			PositionMark[] data_array = DataConvert.ByteToStructArrayByOffset<PositionMark>(buff,sub_msg.nNum2,head_len);
			FavoriteDataList.Clear();
			for(int i=0;i<data_array.Length;i++)
			{
				FavoriteDataList.Add(data_array[i]);
			}
			U3dCmn.SendMessage("FavoriteWin","InitialPanelData",null);
		}
	}
	
}
