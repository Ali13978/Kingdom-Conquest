using UnityEngine;
using System.Collections;
using CMNCMD;
public class MainCity : MonoBehaviour {
	public GameObject MainCityObj;
	public GameObject MainCityBg;
	public Camera mycamera;
	public UICamera mapcamera;
	public GameObject jiuguan_obj;
	public GameObject bingying_obj;
	public GameObject chengbao_obj;
	public GameObject yanjiuyuan_obj;
	LayerMask map_layer;
	LayerMask default_layer;
	public UIRoot root;
	public UIRoot mainui_root;
	public GameObject	top_char_info;
	public GameObject 	top_bar;
	public GameObject	bottom_bar;
	public GameObject	chat_smp;
	//事件按钮 
	public GameObject	event_win;
	//军情按钮 
	public GameObject	warsituation_btn;
	//交易按钮 
	public GameObject   Trade_btn;
	//moregame按钮 
	public GameObject 	moregame_btn;
	//竞技场按钮 
	public GameObject	arena_btn;
	bool ispress;
	Vector3 rootvector;
	Vector3 PrePosition;
	Vector3 LowerLeft;
	Vector3 TopRight;
	
	Vector2 oldPosition1;
	Vector2 oldPosition2; 
	
	float CameraSize;
	public static bool touch_enabel = false;
	//云朵 
	public GameObject cloud1;
	public GameObject cloud2;
	public GameObject cloud3;
	float cloud_xmax=0;
	//相机最大SIZE 
	float max_camerasize = 1.8f;
	float min_camerasize = 0.8f;
	float normal_camerasize = 1.0f;
	void Awake()
	{
		
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if(U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5)
			{
				root.manualHeight = 320;
				mainui_root.manualHeight = 320;
				rootvector = new Vector3(CommonData.iphonescale.x,CommonData.iphonescale.y,1);
				top_char_info.transform.localPosition += new Vector3(-44,0,0);  
				top_bar.transform.localPosition += new Vector3(44,0,0);
				chat_smp.transform.localPosition += new Vector3(-44,0,0);  
				warsituation_btn.transform.localPosition += new Vector3(44,0,0); 
				Trade_btn.transform.localPosition += new Vector3(44,0,0);
				event_win.transform.localPosition += new Vector3(44,0,0); 
				event_win.GetComponent<TweenPosition>().from += new Vector3(44,0,0); 
				event_win.GetComponent<TweenPosition>().to += new Vector3(44,0,0); 
				moregame_btn.transform.localPosition += new Vector3(44,0,0);
				arena_btn.transform.localPosition += new Vector3(-44,0,0);
				normal_camerasize = 1.0f;
				max_camerasize = 1.5f;
				min_camerasize = 0.8f;
				KeepOut.instance.HideKeepOut();
			}
			else
			{
				root.manualHeight = 320;
				mainui_root.manualHeight = 320;
				rootvector = new Vector3(CommonData.iphonescale.x,CommonData.iphonescale.y,1);
			}
				
			
			
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			root.manualHeight = 360;
			mainui_root.manualHeight = 360;
			rootvector = new Vector3(CommonData.ipadscale.x,CommonData.ipadscale.y,1);
			top_char_info.transform.localPosition += new Vector3(0,20,0);  
			top_bar.transform.localPosition += new Vector3(0,20,0);
			bottom_bar.transform.localPosition -= new Vector3(0,19,0);
			normal_camerasize = 1.0f;
			max_camerasize = 1.5f;
			min_camerasize = 1.0f;
		}
		
		mycamera.orthographicSize = normal_camerasize;
	}
	// Use this for initialization
	void Start () {	
		map_layer.value =  (1<<LayerMask.NameToLayer("Map"));
		default_layer.value = (1<<LayerMask.NameToLayer("Default"));
		//if()
		
		ispress = false;
		//TopRight = DividePoint(mycamera.ScreenToWorldPoint(new Vector3(0,0,0)),rootvector)-new Vector3(-MainCityBg.transform.localScale.x/2,-MainCityBg.transform.localScale.y/2,0);
		TopRight = new Vector3(157, 126, 0);
		//LowerLeft = DividePoint(mycamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0)),rootvector) - new Vector3(MainCityBg.transform.localScale.x/2,MainCityBg.transform.localScale.y/2,0);
		LowerLeft = new Vector3(-159, -129, 0);
		CameraSize = mycamera.orthographicSize;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		//mapcamera.eventReceiverMask = map_layer;
		if(touch_enabel)
		{
			
			#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			if (Input.GetButtonDown("Fire1")) 
			{
				ispress = true;
				PrePosition= Input.mousePosition;
				
			}
			if(Input.GetButtonUp("Fire1"))
			{
				ispress = false;
				//mapcamera.eventReceiverMask = map_layer;
				///mapcamera.eventReceiverMask = LayerMask.NameToLayer("Default");
			}
			DragScreen();
			#endif
	
	      	if(Input.touchCount >1 )  
		    {  
		        //前两只手指触摸类型都为移动触摸  
				if(Input.GetTouch(0).phase!=TouchPhase.Moved && Input.GetTouch(1).phase!=TouchPhase.Moved)  
		        {  
					oldPosition1 = Input.GetTouch(0).position;  
	                oldPosition2 = Input.GetTouch(1).position;  
				}
		        if(Input.GetTouch(0).phase==TouchPhase.Moved||Input.GetTouch(1).phase==TouchPhase.Moved)  
		        {  
					//mapcamera.eventReceiverMask = default_layer;
	                //计算出当前两点触摸点的位置  
	                Vector2 tempPosition1 = Input.GetTouch(0).position;  
	                Vector2 tempPosition2 = Input.GetTouch(1).position;  
	                //函数返回真为放大，返回假为缩小  
					int len = isEnlarge(oldPosition1,oldPosition2,tempPosition1,tempPosition2);
					if(Mathf.Abs(len)<100)
					{
		                if(len>20)  
		                {  
		               		ZoomIn();
		                }else if(len <-20)
		                { 
		                	ZoomOut(); 
		                } 
					}
					CorrectPosition();
		            //备份上一次触摸点的位置，用于对比  
		            oldPosition1=tempPosition1;  
		            oldPosition2=tempPosition2;  
		        }  
				else if(Input.GetTouch(0).phase==TouchPhase.Ended && Input.GetTouch(1).phase==TouchPhase.Ended)
				{
					//mapcamera.eventReceiverMask = map_layer;
				}
		    } 
			else if(Input.touchCount==1)
			{
			
				if(Input.GetTouch(0).phase==TouchPhase.Began)
				{ 
					ispress = true;
					PrePosition= Input.mousePosition;
				}
				if(Input.GetTouch(0).phase==TouchPhase.Ended)
				{
					ispress = false;
					//mapcamera.eventReceiverMask = map_layer;
				}
				if(Input.GetTouch(0).phase==TouchPhase.Moved)
				{
					DragScreen();
				}
			}
		}
		//云彩飘动 
		if(cloud_xmax == 0)
			 cloud_xmax = cloud1.transform.localScale.x+MainCityBg.transform.localScale.x/2;
		if(cloud1.transform.localPosition.x<-cloud_xmax)
			cloud1.transform.localPosition =  new Vector3(cloud_xmax,cloud1.transform.localPosition.y,cloud1.transform.localPosition.z);
		else
			cloud1.transform.localPosition = cloud1.transform.localPosition - transform.right *Time.deltaTime*20;
		if(cloud2.transform.localPosition.x<-cloud_xmax)
			cloud2.transform.localPosition =  new Vector3(cloud_xmax,cloud2.transform.localPosition.y,cloud2.transform.localPosition.z);
		else
			cloud2.transform.localPosition = cloud2.transform.localPosition - transform.right *Time.deltaTime*20;
		
	}
	void DragScreen()
	{
		//MapCamera.
		
	//	UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		int leng =(int)Mathf.Sqrt((Input.mousePosition.x-PrePosition.x)*(Input.mousePosition.x-PrePosition.x)+(Input.mousePosition.y-PrePosition.y)*(Input.mousePosition.y-PrePosition.y));  
		
		if(ispress && leng<(Screen.width/4) && leng>0)
		{
			//mapcamera.eventReceiverMask = default_layer;
			Vector3 preworldposition =DividePoint(mycamera.ScreenToWorldPoint(PrePosition),root.transform.localScale);	
			Vector3 nowworldposition = DividePoint(mycamera.ScreenToWorldPoint(Input.mousePosition),root.transform.localScale);	
			Vector3 newposition =  MainCityObj.transform.localPosition + (nowworldposition - preworldposition);
			PrePosition = Input.mousePosition;
		
			if(newposition.x>TopRight.x)
				newposition.x = TopRight.x;
			if(newposition.y>TopRight.y)
				newposition.y = TopRight.y;
			if(newposition.x<LowerLeft.x)
				newposition.x = LowerLeft.x;
			if(newposition.y<LowerLeft.y)
				newposition.y = LowerLeft.y;
			MainCityObj.transform.localPosition = newposition;

			
			
		}
	}

	//函数返回真为放大，返回假为缩小  
	
	int isEnlarge(Vector2 oP1,Vector2 oP2,Vector2 nP1,Vector2 nP2)  
	{  
	    //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势  
	    float leng1 =Mathf.Sqrt((oP1.x-oP2.x)*(oP1.x-oP2.x)+(oP1.y-oP2.y)*(oP1.y-oP2.y));  
	    float leng2 =Mathf.Sqrt((nP1.x-nP2.x)*(nP1.x-nP2.x)+(nP1.y-nP2.y)*(nP1.y-nP2.y));  
		return (int)(leng2-leng1);
	    
	}
	
	//放大
	
	void ZoomIn()
	{
		if(CameraSize-0.15>min_camerasize)
			CameraSize -= 0.15f;
		else
			CameraSize = min_camerasize;
		mycamera.orthographicSize =CameraSize;
	}
	
	//缩小
	
	void ZoomOut()
	{
		if(CameraSize+0.15f<max_camerasize)
			CameraSize += 0.15f;
		else
			CameraSize = max_camerasize;
		mycamera.orthographicSize =CameraSize;

	}
	
	//调整位置适应摄像机
	
	void CorrectPosition()
	{
		TopRight = DividePoint(mycamera.ScreenToWorldPoint(new Vector3(0,0,0)),rootvector)-new Vector3(-MainCityBg.transform.localScale.x/2,-MainCityBg.transform.localScale.y/2,0);
		LowerLeft = DividePoint(mycamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0)),rootvector) - new Vector3(MainCityBg.transform.localScale.x/2,MainCityBg.transform.localScale.y/2,0);
		
		Vector3 newposition = MainCityObj.transform.localPosition;
		if(newposition.x>TopRight.x)
			newposition.x = TopRight.x;
		if(newposition.y>TopRight.y)
			newposition.y = TopRight.y;
		if(newposition.x<LowerLeft.x)
			newposition.x = LowerLeft.x;
		if(newposition.y<LowerLeft.y)
			newposition.y = LowerLeft.y;
		MainCityObj.transform.localPosition = newposition;
		
	}
	public Vector3 DividePoint (Vector3 a,Vector3 b)
	{
		Vector3 result;
		result.x = a.x/b.x;
		result.y = a.y/b.y;
		result.z = a.z/b.z;
		return result;
	}
	public void LocatePosition(int building_sort)
	{
		mycamera.orthographicSize = normal_camerasize;
		TopRight = DividePoint(mycamera.ScreenToWorldPoint(new Vector3(0,0,0)),rootvector)-new Vector3(-MainCityBg.transform.localScale.x/2,-MainCityBg.transform.localScale.y/2,0);
		LowerLeft = DividePoint(mycamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0)),rootvector) - new Vector3(MainCityBg.transform.localScale.x/2,MainCityBg.transform.localScale.y/2,0);
		
		if(building_sort == (int)BuildingSort.MINJU)
		{
			Vector3 newposition = new Vector3(LowerLeft.x,TopRight.y,0);
			Vector3 oldPos = MainCityObj.transform.localPosition;
			oldPos.z = 0f;
			
			float timeMs = 0.87f; // Vector3.Distance(newposition,oldPos) / (120f);
			TweenPosition comp = TweenPosition.Begin(MainCityObj, timeMs, newposition);
			if (comp != null) 
			{
				comp.method = UITweener.Method.Linear;
				//comp.eventReceiver = gameObject;
				//comp.callWhenFinished = "TweenCooldown1";
				comp.Play(true); 
			}
			else
			{
				MainCityObj.transform.localPosition = newposition;
			}
			//Vector3 newposition = new Vector3(LowerLeft.x,TopRight.y,0);
			//MainCityObj.transform.localPosition = newposition;
		}
		else if(building_sort == (int)BuildingSort.JINKUANG)
		{	
			Vector3 newposition = new Vector3(TopRight.x,LowerLeft.y,0);
			Vector3 oldPos = MainCityObj.transform.localPosition;
			oldPos.z = 0f;
			
			float timeMs = 0.87f; // Vector3.Distance(newposition,oldPos) / (120f);
			TweenPosition comp = TweenPosition.Begin(MainCityObj, timeMs, newposition);
			if (comp != null) 
			{
				comp.method = UITweener.Method.Linear;
				//comp.eventReceiver = gameObject;
				//comp.callWhenFinished = "TweenCooldown1";
				comp.Play(true); 
			}
			else
			{
				MainCityObj.transform.localPosition = newposition;
			}
			//MainCityObj.transform.localPosition = newposition;
		}
		else if(building_sort == (int)BuildingSort.BINGYING)
		{
			Vector3 newposition = new Vector3(-bingying_obj.transform.localPosition.x,-bingying_obj.transform.localPosition.y-10,0);
			//Vector3 newposition = new Vector3(TopRight.x,TopRight.y,0);
			Vector3 oldPos = MainCityObj.transform.localPosition;
			oldPos.z = 0f;
			
			float timeMs = 0.87f; // Vector3.Distance(newposition,oldPos) / (120f);
			TweenPosition comp = TweenPosition.Begin(MainCityObj, timeMs, newposition);
			if (comp != null) 
			{
				comp.method = UITweener.Method.Linear;
				//comp.eventReceiver = gameObject;
				//comp.callWhenFinished = "TweenCooldown1";
				comp.Play(true); 
			}
			else
			{
				MainCityObj.transform.localPosition = newposition;
			}
			//MainCityObj.transform.localPosition = newposition;
		}
		else if(building_sort == (int)BuildingSort.JIUGUAN)
		{	
			Vector3 newposition = new Vector3(-jiuguan_obj.transform.localPosition.x+90,-jiuguan_obj.transform.localPosition.y,0);
			Vector3 oldPos = MainCityObj.transform.localPosition;
			oldPos.z = 0f;
			
			float timeMs = 0.87f; // Vector3.Distance(newposition,oldPos) / (120f);
			TweenPosition comp = TweenPosition.Begin(MainCityObj, timeMs, newposition);
			if (comp != null) 
			{
				comp.method = UITweener.Method.Linear;
				//comp.eventReceiver = gameObject;
				//comp.callWhenFinished = "TweenCooldown1";
				comp.Play(true); 
			}
			else
			{
				MainCityObj.transform.localPosition = newposition;
			}
			//MainCityObj.transform.localPosition = newposition;
		}
		else if(building_sort == (int)BuildingSort.CHENGBAO)
		{	
			Vector3 newposition = new Vector3(-chengbao_obj.transform.localPosition.x,-chengbao_obj.transform.localPosition.y+40,0);
			Vector3 oldPos = MainCityObj.transform.localPosition;
			oldPos.z = 0f;
			
			float timeMs = 0.87f; // Vector3.Distance(newposition,oldPos) / (120f);
			TweenPosition comp = TweenPosition.Begin(MainCityObj, timeMs, newposition);
			if (comp != null) 
			{
				comp.method = UITweener.Method.Linear;
				//comp.eventReceiver = gameObject;
				//comp.callWhenFinished = "TweenCooldown1";
				comp.Play(true); 
			}
			else
			{
				MainCityObj.transform.localPosition = newposition;
			}
			//MainCityObj.transform.localPosition = newposition;
		}
		else if(building_sort == (int)BuildingSort.YANJIUYUAN)
		{	
			Vector3 newposition = new Vector3(-yanjiuyuan_obj.transform.localPosition.x,-yanjiuyuan_obj.transform.localPosition.y+40,0);
			Vector3 oldPos = MainCityObj.transform.localPosition;
			oldPos.z = 0f;
			
			float timeMs = 0.87f; // Vector3.Distance(newposition,oldPos) / (120f);
			TweenPosition comp = TweenPosition.Begin(MainCityObj, timeMs, newposition);
			if (comp != null) 
			{
				comp.method = UITweener.Method.Linear;
				//comp.eventReceiver = gameObject;
				//comp.callWhenFinished = "TweenCooldown1";
				comp.Play(true); 
			}
			else
			{
				MainCityObj.transform.localPosition = newposition;
			}
			//MainCityObj.transform.localPosition = newposition;
		}
		//CorrectPosition();
	}
}
