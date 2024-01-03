using UnityEngine;
using System.Collections;
using System;
using CMNCMD;
using CTSCMD;
using STCCMD;
public class LogicDeal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	public static void DealCmdIn_Logic(byte[] pData)	
	{	
		byte pcmd = pData[0];
		switch(pcmd)
		{
			case (byte)TTY_STC_MSG.TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST:
			{
				LoadingManager.instance.HideLoading();
				TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST_T msg = DataConvert.ByteToStruct<TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST_T>(pData);
				if(msg.nRst2 == (int)TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST_T.enum_rst.TTY_ACCOUNT_CREATE_RST_OK)
				{
				
					//MD5 md5hash = new MD5();
					CommonData.player_online_info.AccountName = DataConvert.BytesToStr(msg.szAccount4);
					//CommonData.player_online_info.AccountPass = md5hash.CalcMD5String(DataConvert.StrToBytes(""));
					
					PlayerPrefs.SetString("AccountName",CommonData.player_online_info.AccountName);
					PlayerPrefs.SetString("AccountPass",CommonData.player_online_info.AccountPass);
						//string aa = PlayerPrefs.GetString("AccountPass");
						//GameObject.Find("LoginManager").SendMessage("ShowCreateCharPanel");
						LoginManager.instance.ShowCreateCharPanel();

                }	
				else if(msg.nRst2 == (int)TTY_LPGAMEPLAY_CLIENT_CREATEACCOUNT_RST_T.enum_rst.TTY_ACCOUNT_CREATE_RST_DUP)
				{
					U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.ACCOUNT_NAME_EXISTS));
				}
				else
				{
					U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CREATE_ACCOUNT_ERR));
				}
				//print(msg.nRst2);
				//print (CommonData.player_online.AccountName);
			}
			break;
			case (byte)TTY_STC_MSG.TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST:
			{		
				TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T msg = DataConvert.ByteToStruct<TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T>(pData);
				switch(msg.nRst2)
				{
					case (int)TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T.enum_rst.TTY_CHAR_CREATE_RST_OK:
					{
						//创建角色成功就直接返回登录成功的指令了 一般到不了这里 
						//print("createcharOK");
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T.enum_rst.TTY_CHAR_CREATE_RST_NAMEDUP:
					{
						U3dCmn.ShowWarnWindow(String.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CHAR_NAME_DUPLICATE)));
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T.enum_rst.TTY_CHAR_CREATE_RST_ALREADYEXISTCHAR:
					{
						U3dCmn.ShowWarnWindow(String.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CHAR_NAME_DUPLICATE)));
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T.enum_rst.TTY_CHAR_CREATE_RST_WORLD_FULL:
					case (int)TTY_LPGAMEPLAY_CLIENT_CREATECHAR_RST_T.enum_rst.TTY_CHAR_CREATE_RST_POSITION_DUP:
					{
						U3dCmn.ShowWarnWindow(String.Format(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.SERVER_IS_FULL)));
					}
					break;
					default:
					{
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.CREATE_CHAR_ERR));
					}
					break;
				}
		
				//GameObject.Find("SceneManager").SendMessage("NextScene",null);
			}
			break;
			case (byte)TTY_STC_MSG.TTY_LPGAMEPLAY_CLIENT_LOGIN_RST:
			{
				TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T msg = DataConvert.ByteToStruct<TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T>(pData);
				print ("login rst"+msg.nRst2);
				switch(msg.nRst2)
				{
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_SUCCEED:
					{
						CommonData.player_online_info.AccountName = DataConvert.BytesToStr(msg.szAccount6);
						//CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
						
						if(GameObject.Find("SceneManager") ==null) 
						{
							LoadingManager.instance.HideLoading();
							//MD5 md5hash = new MD5(); 
							CommonData.player_online_info.AccountPass = DataConvert.BytesToStr(msg.szPass8);
							PlayerPrefs.SetString("AccountName",CommonData.player_online_info.AccountName);
							PlayerPrefs.SetString("AccountPass",CommonData.player_online_info.AccountPass);
							PlayerPrefs.SetString("ServerName",CommonData.select_server.ServerName);
							CommonData.AccountBandMac = DataConvert.BytesToStr(msg.szDeviceID4);
							DontDestroyOnLoad(GameObject.Find("TcpManager"));
							//AdMobBinding.destroyBanner();
							Application.LoadLevel("mainscene");
						}
						else
						{
							BuildingManager.IsOpenMoreGame =false;
							BuildingManager.UnreadNum = UnityEngine.Random.Range(1, 5);
							U3dCmn.SendMessage("BuildingManager","RefreshMoreGame",null);
							//U3dCmn.SendMessage("BuildingManager","CheckMainCityObjOrUI",null);
							//能找到SceneManager说明是断线重连不需要重新加载主场景 
							LoadingManager.instance.HideLoading();
							PlayerInfoManager.GetPlayerInfo();
							U3dCmn.SendMessage("BuildingManager","Start",null);
							U3dCmn.SendMessage("ProduceManager","Start",null);
							U3dCmn.SendMessage("AllianceManager","Start",null);
							U3dCmn.SendMessage("CangKuManager","Start",null);
							U3dCmn.SendMessage("ChatManager","Start",null);
							U3dCmn.SendMessage("EquipManager","Start",null);
							U3dCmn.SendMessage("FriendManager","Start",null);
							U3dCmn.SendMessage("JiangLingManager","Start",null);
							U3dCmn.SendMessage("SoldierManager","Start",null);
					
							U3dCmn.SendMessage("MessageManager","Start",null);
							U3dCmn.SendMessage("TechnologyManager","Start",null);
							U3dCmn.SendMessage("TrainGroundManager","Start",null);
							U3dCmn.SendMessage("_BaizInstanceManager","Start", null); // 上线重发状态 ...
							
							//请求战报 
							CombatManager.RequestCombatListData();
							CombatManager.ReqReinforceListData();
							// 大世界地图重连加载 ...
							U3dCmn.SendMessage("_TiledStorgeCacheData","DisbandForUnpack1",null);
							
							// 金矿重连刷新 ...
							U3dCmn.SendMessage("_GoldmineInstance","DisbandForUnpack1",null);
					
							// 战斗相关的重进副本流程 ...
							U3dCmn.SendMessage("_BaizhanInstance","DisbandForUnpack1",null);
							U3dCmn.SendMessage("_NanBeizhanInstance","DisbandForUnpack1",null);
					
							// 任务管理器 ...
							U3dCmn.SendMessage("TaskManager","DissbandForUnpack1",null);
							//请求联盟副本状态 
                            GBStatusManager.Instance.RequestMyGBStatus();
							//请求强制下线数据 
							PlayerInfoManager.ReqKickClientData();
							// 新手引导中, 断线重边 ...
							NewbieWaiting.ResetConn();
						}
						
						//U3dCmn.GetBasicInfo();
						//PlayerPrefs.DeleteKey("AccountName");
						//PlayerPrefs.DeleteKey("AccountPass");
					
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_NOCHAR:
					{
						//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NO_CHAR));
						GameObject.Find("LoginManager").SendMessage("ShowCreateCharPanel");
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_NO_DEVICEID:
					{
						//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NO_CHAR));
						GameObject.Find("LoginManager").SendMessage("CreateAccountCallBack");
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_BADACCOUNT:
					{
						//PlayerPrefs.DeleteKey("AccountName");
						//PlayerPrefs.DeleteKey("AccountPass");
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NO_ACCOUNT));
					}
					break; 
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_BADPASS:
					{
						
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PASS_WRONG));
					}
					break;
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_ALREADYINTHIS:
					case (int)TTY_LPGAMEPLAY_CLIENT_LOGIN_RST_T.enum_rst.TTY_LOGIN_RST_ALREADYINOTHER:
					{
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.PLAYER_ALREADY_ONLINE));
					}
					break;
					default:
					{
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.LOGIN_ERR));
					}
					break;
				
				}
		
			}
			break;
			case 	(int)TTY_STC_MSG.TTY_LPGAMEPLAY_CLIENT_LOGOUT_RST:
			{
				/*TTY_LPGAMEPLAY_CLIENT_RESELECT_CHAR_RST_T msg = DataConvert.ByteToStruct<TTY_LPGAMEPLAY_CLIENT_RESELECT_CHAR_RST_T>(pData);
				if(msg.nRst2 == (byte)CHAR_SUB_RST.TTY_CHARCMD_RST_OK)
				{
						DontDestroyOnLoad(GameObject.Find("TcpManager"));
						DontDestroyOnLoad(GameObject.Find("SoundManager"));
						CommonData.FirstLogin =false;
						Application.LoadLevel("startgame");
						
						
				}*/
			}
			break;
			case  (int)TTY_STC_MSG.TTY_LPGAMEPLAY_CLIENT_GAME_CMD:
			{
				//游戏逻辑消息分发 指令号采用INT类型  
				GameLogicMsg.DealCmdIn(pData);
			}
			break;
			default:
				break;
		}
	}

}
