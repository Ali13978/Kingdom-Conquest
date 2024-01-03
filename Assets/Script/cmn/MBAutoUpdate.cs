using UnityEngine;
using System.IO;
using System.Collections;
using CMNCMD;
using System;

public class MBAutoUpdate : MonoBehaviour {
	
	
	static string	pass		= "1c705c602d095075cff92b16a9b610bb";
	byte[]	tabByte		= new byte[1];
	byte[]	lineEndByte	= new byte[1];
	//string	updateDir	= "";
	static public string	localDir;
    //static bool isOutterIP	= true;
	byte[]	downTitle;
	public static int	needUpdateFiles	= 0;
	public static int	updatedFiles	= 0;
	public static bool	beginUpdate		= false;
	int down_num = 0;
	MD5		md5;
	string	timeStamp		= "";
	
	void Awake()
	{
		localDir	= U3dCmn.GetCachePath();
		downTitle	= System.Text.UTF8Encoding.UTF8.GetBytes("placeholder\tplaceholder\n");
	}
	static public string Pass
	{
		get
		{
			return pass;
		}
	}
	static public WWW newHTTPDownload(string url)
	{
		// check if url is valid
		// if it do not contain a header, add a default http:// header to it
		if( url.Substring(0, 7)!= "http://" && url.Substring(0, 8)!= "https://" ) {
			url	= "http://"+url;
		}
		// test the dest port
		// get the url content
		WWW		w	= new WWW(url);
		return	w;
	}
	
	// Use this for initialization
	void Start () 
	{
		md5	= new MD5();
		//if (Application.isEditor) 
		//	updateDir = "pixelgame.vicp.net/update/";
		//else
		//	updateDir = "http://pixelgame.vicp.net/update/";
		
		timeStamp		= "&t=" + DataConvert.DateTimeToInt(DateTime.Now);
		tabByte[0]		= (byte)'\t';
		lineEndByte[0]	= (byte)'\n';
		if (!Directory.Exists(localDir))
		{
			Directory.CreateDirectory(localDir);
		}
		beginUpdate =  false;
		down_num =0;
		StartCoroutine (LoadWebFileFromOut());
	
		
	}
	
	
	enum UseType
	{
		use_all			= 0,
		use_server		= 1,
		use_client		= 2,
		use_none		= 3,
	};
	class UpdateEntryData
	{
		public string	tableID;
		public string	filePath;
		public string	fileName;
		public string	checksum;
		public UseType	useType;
	}
	
	class DownloadEntry
	{
		public string	tableID;
		public string	checksum;
	}
	
	void FlushAndCloseFileStream(ref FileStream fs)
	{
		if (fs == null)
		{
			return;
		}
		fs.Flush();
		fs.Close();
		fs			= null;
	}
	IEnumerator LoadWebFileFromOut()
	{
		CommonMB.InitialClientVersion();
		return LoadWebFile("", false, false);
		
	}
	IEnumerator LoadWebFile(string updateDir, bool needSleepFirst, bool needReturnSoon)
	{
		return _LoadWebFile(updateDir, needSleepFirst, needReturnSoon, true);
	}
	IEnumerator _LoadWebFile(string updateDir, bool needSleepFirst, bool needReturnSoon, bool getRouterFirst)
	{
		if (needReturnSoon)
		{
			down_num++;
			yield break;
		}
		if (needSleepFirst)
		{
			yield return new WaitForSeconds(1.0f);
		}
		if (beginUpdate)
		{
			yield break;
		}
		
		if( getRouterFirst )
		{
			// 
			string  updateRouter = "http://103.38.236.112/battleforempire/update/";
		
			updateRouter	= updateRouter + "VERSION_APPSTORE/?cliver=" + CommonMB.ClientVersion + "&appid=" + CommonMB.AppID;
			print("**** updateRouter:"+updateRouter);
			// get update address for this version first
			WWW		www_updateDir		= null;
			yield return www_updateDir	= newHTTPDownload(updateRouter + timeStamp);
			print("**** rst:"+www_updateDir.isDone+","+www_updateDir.error+","+www_updateDir.size+","+www_updateDir.bytes.Length);
			if (www_updateDir!=null && www_updateDir.isDone && www_updateDir.error==null && www_updateDir.size>0 && www_updateDir.bytes!=null && www_updateDir.bytes.Length>0)
			{
				// check if it is some command
				char [] delimiter = ",".ToCharArray();
				string [] a	= www_updateDir.text.Split(delimiter, 2);
				if( a.Length==1 ) {
					updateDir	= a[0];
				}
				else {
					print("**** dealing cmd:"+a[0]);
					switch( a[0] )
					{
					case	"msg":
						// show the message and quit
						U3dCmn.ShowQuitWindow(a[1]);
						yield break;
						break;
					default:
						// quit
						yield break;
						break;
					}
				}
				//print("**** get updateDir:"+updateDir);
			}
			else
			{
				print("**** can not get updateRouter:"+updateRouter);
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR));
				yield break;
			}
		}

		needUpdateFiles	= 0;
		updatedFiles	= 0;
		
		// 最先下载的是server_list.txt的校验文件   
		print("**** updating from:"+updateDir);
		WWW		webfile4serverlistchecksum		= null;
		yield return webfile4serverlistchecksum	= newHTTPDownload(updateDir + "server_list_checksum.txt" + timeStamp);
		if (webfile4serverlistchecksum!=null && webfile4serverlistchecksum.isDone && webfile4serverlistchecksum.error==null && webfile4serverlistchecksum.size>0 && webfile4serverlistchecksum.bytes!=null && webfile4serverlistchecksum.bytes.Length>0)
		{
			if (beginUpdate)
			{
				yield break;
			}
			else
			{
				beginUpdate						= true;
			}
			bool	needDownloadServerListfile	= true;
			if (File.Exists(localDir + "server_list_checksum.txt"))
			{
				string checksum				= File.ReadAllText(localDir + "server_list_checksum.txt");
				if (checksum == webfile4serverlistchecksum.text)
				{
					needDownloadServerListfile	= false;
					//print ("no need update server_list.txt");
				}
			}
			if (needDownloadServerListfile)
			{
				WWW	webfile;		
				yield return webfile	= newHTTPDownload(updateDir + "server_list.txt" + timeStamp);
			
				if (webfile!=null && webfile.isDone && webfile.error==null && webfile.size>0 && webfile.bytes!=null && webfile.bytes.Length>0)
				{
					FileStream	fs		= new FileStream(localDir + "server_list.txt", FileMode.Create);
					fs.Write(webfile.bytes, 0, webfile.bytes.Length);
					FlushAndCloseFileStream(ref fs);
					
					// 将新的checksum写入本地  
					File.WriteAllText(localDir + "server_list_checksum.txt", webfile4serverlistchecksum.text);
				}
			}
		}
		else
		{
			// index_list.txt的校验文件下载失败  
			down_num++;
			if(down_num >= 2)
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR));
			yield  break;
		}
		
		// 先下载index_list.txt的校验文件,看看需不需要做进一步下载    
		WWW		webfile4checksum		= null;
		yield return webfile4checksum	= newHTTPDownload(updateDir + "index_list_checksum.txt" + timeStamp);

		if (webfile4checksum!=null && webfile4checksum.isDone && webfile4checksum.error==null && webfile4checksum.size>0 && webfile4checksum.bytes!=null && webfile4checksum.bytes.Length>0)
		{
			bool	needDownloadIndexfile	= true;
			if (File.Exists(localDir + "index_list_checksum.txt"))
			{
				string checksum				= File.ReadAllText(localDir + "index_list_checksum.txt");
				if (checksum == webfile4checksum.text)
				{
					needDownloadIndexfile	= false;
					//print ("no need update index_list.txt");
				}
			}
			
			byte[]	data				= null;
			byte[]	key					= (byte[])System.Text.UTF8Encoding.UTF8.GetBytes(pass).Clone();
			if (needDownloadIndexfile)
			{
				// 保存校验文件   
				//File.WriteAllText(localDir + "index_list_checksum.txt", webfile4checksum.text);
				
				WWW	webfile;		
				yield return webfile	= newHTTPDownload(updateDir + "index_list.txt" + timeStamp);
				
				if (webfile!=null && webfile.isDone && webfile.error==null && webfile.size>0 && webfile.bytes!=null && webfile.bytes.Length>0)
				{
					// 1.下载index_list.txt并解密添加hashtable 
					FileStream	fs		= new FileStream(localDir + "index_list.txt", FileMode.Create);
					fs.Write(webfile.bytes, 0, webfile.bytes.Length);
					FlushAndCloseFileStream(ref fs);
					
					// 将新的checksum写入本地  
					File.WriteAllText(localDir + "index_list_checksum.txt", webfile4checksum.text);
					
					data				= (byte[])webfile.bytes.Clone();
				}
				else
				{
					down_num++;
					if(down_num >= 2)
						U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR));
					yield break;
				}
			}
			else
			{
				data					= File.ReadAllBytes(localDir + "index_list.txt");
			}
			
			Hashtable	hash4index		= new Hashtable();
			string[][]	newStrings		= MBDeal.readMBFromBytes(data, false, true);
			if (newStrings!=null && newStrings.Length>0)
			{
				for (int i=0; i<newStrings[0].Length; i++)
				{
					UpdateEntryData	ued		= new UpdateEntryData();
					ued.tableID				= newStrings[1][i];
					ued.filePath			= newStrings[2][i] + ".txt";
					int	index				= ued.filePath.LastIndexOf('/')+1;
					ued.fileName			= ued.filePath.Substring(index);
					ued.checksum			= newStrings[8][i];
					ued.useType				= (UseType)System.Convert.ToInt32(newStrings[5][i]);
					hash4index.Add(ued.tableID, ued);
				}
			}
					
			// 2.读取download_old.txt,download.txt并加入hashtable，然后写入download_old.txt(防止上次未更新完程序即退出) 
			// download.txt,download_old.txt的格式是tableid	checksum	
			Hashtable	hash4download	= new Hashtable();
			if (File.Exists(localDir + "download_old.txt"))
			{
				FileStream	fsDownloadOld	= new FileStream(localDir + "download_old.txt", FileMode.Open);
				byte[]		dataDownloadOld	= new byte[fsDownloadOld.Length];
				fsDownloadOld.Read(dataDownloadOld, 0, dataDownloadOld.Length);
				string[][]	downOldStrings	= MBDeal.readMBFromBytes(dataDownloadOld, false, true);
				if (downOldStrings!=null && downOldStrings.Length>0)
				{
					for (int i=0; i<downOldStrings[0].Length; i++)
					{
						DownloadEntry	e		= new DownloadEntry();
						e.tableID				= downOldStrings[0][i];
						e.checksum				= downOldStrings[1][i];
						hash4download.Add(e.tableID, e);
					}
				}
				FlushAndCloseFileStream(ref fsDownloadOld);
			}
			if (File.Exists(localDir + "download.txt"))
			{
				FileStream	fsDownload		= new FileStream(localDir + "download.txt", FileMode.Open);
				byte[]		dataDownload	= new byte[fsDownload.Length];
				fsDownload.Read(dataDownload, 0, dataDownload.Length);
				string[][]	downStrings		= MBDeal.readMBFromBytes(dataDownload, false, true);
				if (downStrings!=null && downStrings.Length>0)
				{
					for (int i=0; i<downStrings[0].Length; i++)
					{
						string		tableID		= downStrings[0][i];
						if (hash4download.Contains(tableID))
						{
							DownloadEntry	e	= (DownloadEntry)hash4download[tableID];
							e.checksum			= downStrings[1][i];
						}
						else
						{
							DownloadEntry	e	= new DownloadEntry();
							e.tableID			= downStrings[0][i];
							e.checksum			= downStrings[1][i];
							hash4download.Add(e.tableID, e);
						}
					}
				}
				FlushAndCloseFileStream(ref fsDownload);
			}
			FileStream	fsOldDownloadNew	= new FileStream(localDir + "download_old.txt", FileMode.Create);
			fsOldDownloadNew.Write(downTitle, 0, downTitle.Length);
			fsOldDownloadNew.Flush();
			foreach (DictionaryEntry de in hash4download)
			{
				DownloadEntry	e			= (DownloadEntry)de.Value;
				byte[]	keyByteArray		= System.Text.UTF8Encoding.UTF8.GetBytes(e.tableID);
				byte[]	checksumByteArray	= System.Text.UTF8Encoding.UTF8.GetBytes(e.checksum);
				fsOldDownloadNew.Write(keyByteArray, 0, keyByteArray.Length);
				fsOldDownloadNew.Write(tabByte, 0, tabByte.Length);
				fsOldDownloadNew.Write(checksumByteArray, 0, checksumByteArray.Length);
				fsOldDownloadNew.Write(lineEndByte, 0, lineEndByte.Length);
				fsOldDownloadNew.Flush();
			}
			FlushAndCloseFileStream(ref fsOldDownloadNew);
			
			// 3.比较开始下载  
			FileStream	fsDownloadNew		= new FileStream(localDir + "download.txt", FileMode.Create);
			fsDownloadNew.Write(downTitle, 0, downTitle.Length);
			fsDownloadNew.Flush();
			
			ArrayList	needUpdateFileList	= new ArrayList();
			foreach (DictionaryEntry de in hash4index)
			{
				UpdateEntryData	e			= (UpdateEntryData)de.Value;
				bool	needCheck			= false;
				switch (e.useType)
				{
				case UseType.use_all:
				case UseType.use_client:
					{
						needCheck			= true;
					}
					break;
				default:
					{
					
					}
					break;
				}
				if (needCheck)
				{
					if (hash4download.Contains(de.Key))
					{
						DownloadEntry	d		= (DownloadEntry)hash4download[de.Key];
						if (d.checksum != e.checksum)
						{
							needUpdateFileList.Add(e);
						}
					}
					else
					{		
						needUpdateFileList.Add(e);
					}	
				}
			}
			
			needUpdateFiles	= needUpdateFileList.Count;
			//beginUpdate		= true;
			foreach (UpdateEntryData e in needUpdateFileList)
			{
			reload_file:
				WWW	newWebfile					= null;
				yield return newWebfile			= newHTTPDownload(updateDir + e.filePath + timeStamp);
				if (newWebfile!=null && newWebfile.isDone && newWebfile.error==null && newWebfile.size>0 && newWebfile.bytes!=null && newWebfile.bytes.Length>0)
				{
					string	checksumStr			= md5.CalcMD5String(newWebfile.bytes);
					// 4.检验校验和,因为客户端没有自恢复的方法 
					if (checksumStr == e.checksum)
					{
						updatedFiles			+= 1;
						FileStream	fsNew		= new FileStream(localDir + e.fileName, FileMode.Create);
						fsNew.Write(newWebfile.bytes, 0, newWebfile.bytes.Length);
						FlushAndCloseFileStream(ref fsNew);
						
						// 5.修改download.txt,记录本次更新了的文件  
						byte[]	keyByteArray		= System.Text.UTF8Encoding.UTF8.GetBytes(e.tableID);
						byte[]	checksumByteArray	= System.Text.UTF8Encoding.UTF8.GetBytes(e.checksum);
						fsDownloadNew.Write(keyByteArray, 0, keyByteArray.Length);
						fsDownloadNew.Write(tabByte, 0, tabByte.Length);
						fsDownloadNew.Write(checksumByteArray, 0, checksumByteArray.Length);
						fsDownloadNew.Write(lineEndByte, 0, lineEndByte.Length);
						fsDownloadNew.Flush();
					}
					else
					{
						//print ("checksum err:" + e.filePath + "," + checksumStr + "," + e.checksum);
						goto reload_file;
					}
				}
				else
				{
					goto reload_file;
				}
			}
			
			FlushAndCloseFileStream(ref fsDownloadNew);
			//beginUpdate				= false;
			LoginManager.canLoadMB	= true;
		}
		else
		{
			// index_list.txt的校验文件下载失败  
			down_num++;
			if(down_num >= 2)
				U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NET_ERR));
			yield  break;
		}
	}
}
