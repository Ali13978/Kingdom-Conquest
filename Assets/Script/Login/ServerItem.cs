using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerItem : MonoBehaviour {
	//public UILabel server_label;
	public Text server_label;
	public UISprite server_bg;
	ServerInfo now_info;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//初始化 
	void InitialData(ServerInfo info)
	{
		if(info.ServerName != "")
		{
			now_info = info;
			if(info.ServerStatus != "")
				server_label.text = info.ServerName+info.ServerStatus;
			else 
				server_label.text = info.ServerName;
			server_bg.spriteName  ="shurukuang";
		}
		else
		{
			server_label.text = "";
			server_bg.spriteName = "transparent";
		}
		
	}
	//选中服务器 
	void SelectServer()
	{
		CommonData.select_server.ServerName = now_info.ServerName;
		CommonData.select_server.ServerIP = now_info.ServerIP;
		CommonData.select_server.ServerPort = now_info.ServerPort;
		CommonData.select_server.ServerStatus = now_info.ServerStatus;
		CommonData.select_server.ServerID = now_info.ServerID;
		U3dCmn.SendMessage("LoginManager","ReSelectServer",null);
	}
}
