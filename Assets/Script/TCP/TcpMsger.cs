using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;

using CMNCMD;
using CTSCMD;
using STCCMD;
using System.IO;
using System.Threading;
public enum TcpSate { nothing,INCAAFS,INCLS,CLOSE };
public enum SocketStep { recvhead,recvdata };

public struct SocketState
{
		public SocketStep 		step;
		public Socket			handle;
		public  int				TotalSize;
		public  int 			HeadSize;
		public  int 			RequestSize;
		public  int 			OffSet;
}
public   class TcpMsger : MonoBehaviour {
	public static byte[] sendkey;
	public static byte[] recvkey;
	public static int recv = 0;
	public static  bool coroutine = false;
	public static  bool recvtick = false;
	public static int ticknum = 0;

	const int socketbuffsize = 1024*1024;
	static byte[] socketrecvbuff = new byte[socketbuffsize];
	static  Queue<byte[]> messagequeue= new Queue<byte[]>();
	Hashtable MessageData=new Hashtable();
	//public static	 int headrecv = 0;
	//public static 	 int tailrecv = 0;
	public static Socket CaafsSocket = null;
	public static Socket ClsSocket = null;
	//public static ArrayList   checkSocket   =   new   ArrayList(); 
	public static TcpSate tcpstate = TcpSate.nothing; 
	static ManualResetEvent  TimeoutObject = new ManualResetEvent (false);
	 public static Mutex mutex = new Mutex();
	public static int CntrID = 0;
	public static int ConnectClsID = 0;
	public static int ConnectClsPass = 0;
	//public login loginstatic;
	//start.EventHandler(Listener);
	// Use this for initialization
	int frames=0;
	float updateInterval=0.5f;
	string fps;
	float accum=0.0f;
	float timeLeft;
	static string err_str = "";
	void Start () {
		//iPhoneSettings.screenCanDarken = false;
		//byte[] mem = new byte[10];
		//mem[0] = 10;
		
	//	MessageData.Add(1,mem);
	//	MessageData.Add("nihao",mem);
	//	foreach(DictionaryEntry dd in MessageData)
 		//{
		//	byte[] buff = (byte[])dd.Value;
		//	print(System.Text.Encoding.GetEncoding("UTF-8").GetString(buff));
			//print(dd.Key.ToString());
		//print(dd.Value.ToString());
	//	}
		//   index = 0 ;
	//	MemoryStream stream;
		// main = (Main)GetComponent(typeof(Main));
		//byte[] buff = new byte[10];
	//	messagequeue.Enqueue(buff);
	}
	
	// Update is called once per frame
	void Update () {
		//timeLeft-=Time.deltaTime;
	   // accum+=Time.timeScale/Time.deltaTime;
	   // ++frames;
	   // if(timeLeft<=0.0f)
	   // {
	    //   fps=(accum/frames).ToString("f2");
	    //   timeLeft=updateInterval;
	    //   accum=0.0f;
	   //    frames=0;
	  //  }
		
		if(tcpstate == TcpSate.CLOSE)
		{
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR)+err_str);
			tcpstate = TcpSate.nothing;
			StopAllCoroutines();
			Clear();
			U3dCmn.SendMessage("ReLoginManager","StartLogin",null);
			//U3dCmn.ShowNetErrWindow((string)CommonData.MB_TEXT_MAP[1]);
		}
		
		DealCmdIn_Logic();
		if(CntrID !=0 && !coroutine)
		{
			StartCoroutine("StartSendTick",10);
			StartCoroutine("StartRecvTick",6);
			coroutine = true;
		}

	}
	//void OnGUI()
	//{
	//   GUI.Box(new Rect(0,0,100,20),"FPS:"+fps);
	//}
	
	static void SetCodeKey(byte[] srcKey)
	{
		MD5		md5		= new MD5();
		byte[] tmpKey	= md5.CalcMD5(srcKey);
		byte	b 	= tmpKey[2];
		tmpKey[2]	= tmpKey[11];
		tmpKey[11]	= b;
		b			= tmpKey[5];
		tmpKey[5]	= tmpKey[7];
		tmpKey[7]	= b;
		tmpKey		= md5.CalcMD5(tmpKey);
		
		sendkey = new byte[tmpKey.Length];
		recvkey = new byte[tmpKey.Length];
		Array.Copy(tmpKey,0,sendkey,0,tmpKey.Length);
		Array.Copy(tmpKey,0,recvkey,0,tmpKey.Length);
	}

	void OnApplicationPause(bool state)
	{
		//if(state)
		//	print("pause ture"+ticknum.ToString());
	//	else
		//	print("pause false"+ticknum.ToString());
	//	print("OnApplicationPause"+state);
	}
	void OnApplicationQuit()
	{
		Clear();
		//print("OnApplicationQuit");
	}
	void OnAppQuit()
	{
	//	print("OnAppQuit");
	}
	public static void Clear()
	{
		TcpSate tcpstate = TcpSate.nothing; 
		recvtick = false;
		if(CaafsSocket != null)
			CaafsSocket.Close();
		
		if(ClsSocket != null)
			ClsSocket.Close();
		CaafsSocket = null;
		ClsSocket = null;
		coroutine = false;
		CntrID = 0;
		ConnectClsID = 0;
		ConnectClsPass = 0;
	}
	public static void Connect(Socket socket_handle,string ipadr,int port,TcpSate connectstate,int timeout)
	{
		try
		{
			IPAddress ip = IPAddress.Parse(ipadr);	
			IPEndPoint ipe = new IPEndPoint(ip,port);
			socket_handle.BeginConnect(ipe,ConnectCallBack,socket_handle);
			TimeoutObject.Reset();
			if(!TimeoutObject.WaitOne( timeout,false))
			{
			
				if (tcpstate != TcpSate.INCAAFS && tcpstate != TcpSate.INCLS )
           		{
                	//tcpstate = TcpSate.nothing;
					if(Application.isEditor)
					{
						if(socket_handle.Connected)
							socket_handle.Disconnect(false);
					}
					else
					{
						if(socket_handle != null)
							socket_handle.Disconnect(false);
					}
					//err_str = "net err0";
					//print ("net err");
					U3dCmn.ShowNetErrWindow();
           		}
			}
			else
			{
					tcpstate = connectstate;
					RecvTick(socket_handle);
			}
		}
		catch(Exception e)
		{
			//tcpstate = TcpSate.CLOSE;	
			if(Application.isEditor)
			{
				if(socket_handle.Connected)
					socket_handle.Disconnect(false);
			}
			else
			{
				if(socket_handle != null)
					socket_handle.Disconnect(false);
			}
			//err_str = "net err 1";
			//print ("net err 1");
			U3dCmn.ShowNetErrWindow();
			//U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR));
		}
	
		
	}
	
	public static void ConnectCallBack(IAsyncResult asyncresult)
	{
			Socket connectsocket =  (Socket)asyncresult.AsyncState;
			connectsocket.EndConnect(asyncresult);
			TimeoutObject.Set();
			
	}
	public static void ConnectCaafs(string ipadr,int port,int timeout)
	{
		CaafsSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		Connect(CaafsSocket,ipadr,port,TcpSate.INCAAFS,timeout);
		
	
	
	}
	public static void ConnectCls(string ipadr,int port,int timeout)
	{
		
			CaafsSocket.Close();
			tcpstate = TcpSate.nothing;
			ClsSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		//	usocket.e;
			Connect(ClsSocket,ipadr,port,TcpSate.INCLS,timeout);
	}
	IEnumerator StartSendTick(int WaitTime)
	{	
		int i = 0;
		while(true)
		{
			SendTickData();
			yield return new WaitForSeconds(WaitTime);
		}
	
	}
	IEnumerator StartRecvTick(int WaitTime)
	{
		int i =0;
		while(true)
		{
			yield return new WaitForSeconds(WaitTime);
			if(!recvtick)
			{
				//print("no recv heart tick");
				err_str = "no recv heart tick";
				tcpstate = TcpSate.CLOSE;
			}
			recvtick = false;
		}
	
	}
	public static  void SendTickData()
	{
		CMN_MSG_HEAD_T  Msg_Head = new CMN_MSG_HEAD_T();
		CMN_MSG_BASE_T  Msg_Base = new CMN_MSG_BASE_T();
		Msg_Base.Cmd1 = (byte)MSG_HEAD.MSG_KA;
		int headlen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_HEAD_T>();
		int msgbaselen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_BASE_T>();
		Msg_Head.nSize1 = msgbaselen;
		Msg_Head.CntrID2 = CntrID;
		byte[] headbuff = DataConvert.StructToByte<CMN_MSG_HEAD_T>(Msg_Head);
		byte[] msgbasebuff = DataConvert.StructToByte<CMN_MSG_BASE_T>(Msg_Base);
		byte[] buff = new byte[headlen+msgbaselen];
		Array.Copy(headbuff,0,buff,0,headlen);
		Array.Copy(msgbasebuff,0,buff,headlen,msgbaselen);
		SendData(buff);
	}
	public static void SendKeyData<T>(T structure)
	{
	    byte[] buff = DataConvert.StructToByte<T>(structure);
		CMN_MSG_SUBCMD_T subcmd;
		subcmd.SubCmd1 = (byte)SUBCMD.CLIENT_CLS4Web_DATA;
		byte []newbuff = DataConvert.PackageData(buff,CntrID,(byte)MSG_HEAD.MSG_EXCHANGE_KEY,subcmd);
		SendData(newbuff);
	}
	public static void SendLogicData<T>(T structure)
	{
		if(tcpstate == TcpSate.INCLS)
		{
		    byte[] buff = DataConvert.StructToByte<T>(structure);
	
		
			CMN_MSG_SUBCMD_T subcmd;
			subcmd.SubCmd1 = (byte)SUBCMD.CLIENT_CLS4Web_DATA;
			byte []newbuff = DataConvert.PackageData(buff,CntrID,(byte)MSG_HEAD.MSG_DATA,subcmd);
			SendData(newbuff);	
		}
		else
		{
			//print ("net err 10");
			err_str = "no recv tick";
			U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR));
		}
	}
	public static void SendData(byte[] CmdBuff)
	{
	
		try
		{
			int bytes = 0;
			if (tcpstate == TcpSate.INCAAFS)
				bytes = CaafsSocket.Send(CmdBuff);
			else if (tcpstate == TcpSate.INCLS)
				bytes = ClsSocket.Send(CmdBuff);
		}
		catch(SocketException   e)
		{
			tcpstate = TcpSate.CLOSE;	
			err_str = "net err 2";
			//print ("net err 2");
		}
	}
	//public static void SendData(Hashtable CmdBuff)
//	{
			
	//}
	public static void RecvTick(Socket socket_handle)
	{
		int headlen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_HEAD_T>();
		SocketState state;
		state.step = SocketStep.recvhead;
		state.handle = socket_handle;
		state.TotalSize = 0;
		state.HeadSize	  = headlen;
		state.RequestSize	  = headlen;
		state.OffSet = 0;
		socket_handle.BeginReceive(socketrecvbuff,0,headlen,0,new AsyncCallback(ReadCallback),state);
		
	}
	public static void ReadCallback(IAsyncResult ar) 
	{
		SocketState state =  (SocketState)ar.AsyncState;
		Socket handle = state.handle;
		int len = handle.EndReceive(ar);
	
		if (len >0)
		{
			if (state.step == SocketStep.recvhead)
			{
				if (len ==state.RequestSize )
				{
					CMN_MSG_HEAD_T msghead = DataConvert.ByteToStruct<CMN_MSG_HEAD_T>(socketrecvbuff);
					state.step = SocketStep.recvdata;
					state.handle = handle;
					state.TotalSize = msghead.nSize1 +state.HeadSize; 
					state.RequestSize = msghead.nSize1;
					state.OffSet		= state.OffSet+len;
					handle.BeginReceive(socketrecvbuff,state.OffSet,state.RequestSize,0,new AsyncCallback(ReadCallback),state);
				}
				else if (len < state.RequestSize)
				{
						
					state.step = SocketStep.recvhead;
					state.handle = handle;
					state.RequestSize = state.RequestSize - len;
					state.OffSet		=  state.OffSet+len;
					handle.BeginReceive(socketrecvbuff,state.OffSet,state.RequestSize,0,new AsyncCallback(ReadCallback),state);
				}
			}
			else if(state.step == SocketStep.recvdata)
			{
				if (len == state.RequestSize)
				{
					
					DealCmdIn_Basic(socketrecvbuff);
					
					RecvTick(handle);
				}
				else if (len < state.RequestSize)
				{
					state.step = SocketStep.recvdata;
					state.handle = handle;
					state.RequestSize = state.RequestSize - len;
					state.OffSet		=  state.OffSet+len;
					handle.BeginReceive(socketrecvbuff,state.OffSet,state.RequestSize,0,new AsyncCallback(ReadCallback),state);
				}
			}
			
		}
		else
		{
			//print("end end"+len);
			err_str = "end end";
			tcpstate = TcpSate.CLOSE;
		}
	}
	public static void DealCmdIn_Basic(byte[] pData)
	{
			
		CMN_MSG_HEAD_T msghead = DataConvert.ByteToStruct<CMN_MSG_HEAD_T>(pData);
		int headlen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_HEAD_T>();
		int cmdbaselen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_BASE_T>();
		int cmdlen = msghead.nSize1 - cmdbaselen;
	
		byte[] BaseCmd = new byte[cmdbaselen];
	    Array.Copy(socketrecvbuff,headlen,BaseCmd,0,cmdbaselen);
		CMN_MSG_BASE_T pcmd = DataConvert.ByteToStruct<CMN_MSG_BASE_T>(BaseCmd);
		byte[] CmdBuff = new byte[cmdlen];
		Array.Copy(socketrecvbuff,headlen+cmdbaselen,CmdBuff,0,cmdlen);
		//byte pcmd = msghead.Cmd3;
		recvtick = true;//心跳标记
		
		switch(pcmd.Cmd1)
		{
			case   (byte)MSG_HEAD.MSG_EXCHANGE_KEY:
			{
					
				if(tcpstate == TcpSate.INCAAFS) 
				{
				
					CMN_MSG_EXCHANGEKEY_T msg_key = DataConvert.ByteToStruct<CMN_MSG_EXCHANGEKEY_T>(CmdBuff);
					CntrID = msghead.CntrID2;
					CMN_MSG_CAAFS_EXCHANGEKEY_T caafs_exchenge_key =  new CMN_MSG_CAAFS_EXCHANGEKEY_T();
					caafs_exchenge_key.CryptType1 = msg_key.CryptType1;
					caafs_exchenge_key.KATimeout2 = msg_key.KATimeout2;
					caafs_exchenge_key.Len3 = msg_key.Len3;
					caafs_exchenge_key.szKey4 = msg_key.szKey4;
					caafs_exchenge_key.TermType5 = 1;
					SendKeyData<CMN_MSG_CAAFS_EXCHANGEKEY_T >(caafs_exchenge_key);
				}
				else if (tcpstate == TcpSate.INCLS) 
				{
					
					CMN_MSG_EXCHANGEKEY_T msg_key = DataConvert.ByteToStruct<CMN_MSG_EXCHANGEKEY_T>(CmdBuff);
					CntrID = msghead.CntrID2;
					//print(System.Text.Encoding.GetEncoding("UTF-8").GetString(msg_key.szKey4));
					CMN_MSG_CLS_EXCHANGEKEY_T msg_cls_key = new CMN_MSG_CLS_EXCHANGEKEY_T();
					msg_cls_key.CryptType1 = msg_key.CryptType1;
					msg_cls_key.KATimeout2 = msg_key.KATimeout2;
					//print(msg_cls_key.KATimeout2 );
					msg_cls_key.Len3 = msg_key.Len3;
					msg_cls_key.szKey4 = msg_key.szKey4;
					msg_cls_key.ConID5 = ConnectClsID;
					msg_cls_key.Password6 = ConnectClsPass;
					//print( System.Text.Encoding.GetEncoding("UTF-8").GetString(msg_key.szKey5))
					SendKeyData<CMN_MSG_CLS_EXCHANGEKEY_T >(msg_cls_key);
					
					byte[] srcKey	= new byte[msg_cls_key.Len3];
					Array.Copy(msg_key.szKey4,0,srcKey,0,msg_key.Len3);
					SetCodeKey(srcKey);
					LoginManager.enable_login = true;
				}
			}
			break;
			case   (byte)MSG_HEAD.MSG_KA:
			{
					recvtick = true;
			}
			break;
			case   (byte)MSG_HEAD.MSG_CLOSE:
			{
					tcpstate = TcpSate.CLOSE;
					err_str = "net err 3";
					//print ("net err 3");
			}
			break;
			case   (byte)MSG_HEAD.MSG_DATA:
			{
					mutex.WaitOne();
					messagequeue.Enqueue(CmdBuff);
					mutex.ReleaseMutex();
			}
			break;
			default:
			break;
		}
			
	}
	public  void DealCmdIn_Logic()	
	{
		int len = messagequeue.Count;
		if (len != 0)
		{
			mutex.WaitOne();
			byte[] pData =messagequeue.Dequeue();
			mutex.ReleaseMutex();
			int CmdBuffLen = pData.Length-1;
			byte[] CmdBuff = new byte[CmdBuffLen];
		    Array.Copy(pData,1,CmdBuff,0,CmdBuffLen);
	
			
			if(pData[0] != DataConvert.CRC8(CmdBuff))
			{
				tcpstate = TcpSate.CLOSE;
				//print ("CRC err");
				err_str = "CRC err";
				return ;
			}
			byte pcmd = CmdBuff[0];
		
			switch(pcmd)
			{
			case (byte)STC_MSG.CAAFS4Web_CLIENT_QUEUEINFO:
				{
						
			
				}
				break;
			case (byte)STC_MSG.CAAFS4Web_CLIENT_CAAFS4WebINFO:
				{
					CAAFS4Web_CLIENT_CAAFS4WebINFO_T msg = DataConvert.ByteToStruct<CAAFS4Web_CLIENT_CAAFS4WebINFO_T>(CmdBuff);
					CommonData.select_server.ServerVersion = DataConvert.BytesToStr(msg.szVer4);
					//print ("ssssssssssssss"+CommonData.select_server.ServerVersion);
					if(GameObject.Find("SceneManager") !=null) 
					{
						//是否是在游戏中 
						/*if(!U3dCmn.CheckLittleVersion(CommonData.select_server.ClientVersion,CommonData.select_server.ServerVersion))
						{
							LoadingManager.instance.HideLoading();
							//小版本不一致 提醒玩家从新登陆  
							U3dCmn.GetObjFromPrefab("NewVersionWin").SendMessage("RevealPanel");
						}*/
					}
					
				}
				break;
			case (byte)STC_MSG.CAAFS4Web_CLIENT_GOTOCLS4Web:
				{
					CAAFS4Web_CLIENT_GOTOCLS4Web_T msg = DataConvert.ByteToStruct<CAAFS4Web_CLIENT_GOTOCLS4Web_T>(CmdBuff);
					//string ip_str = DataConvert.IntToIP(msg.IP3);
					//IPAddress[] ips = Dns.GetHostAddresses("pixelgame.vicp.net");
					//print (ips[0]);
					string ip_str = CommonData.select_server.ServerIP;
				//print(ip_str);
					CntrID = 0;
					ConnectClsID = msg.nClientID5;
					ConnectClsPass = msg.nPassword4;
					ConnectCls(ip_str,(int)msg.nPort2,5000);
				}
				break;
			case (byte)SUBCMD.CLS4Web_CLIENT_DATA:
				{
					int subcmdlen = CmdBuff.Length -1;
					byte[] SubCmdBuff = new byte[subcmdlen];
					Array.Copy(CmdBuff,1,SubCmdBuff,0,subcmdlen);
					LogicDeal.DealCmdIn_Logic(SubCmdBuff);
				}
				break;
				default:
					//print (pcmd);
				break;
			}
		}
	}

}
