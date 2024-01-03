using UnityEngine;
using System.Collections;
using CMNCMD;
public class LoadingWin : MonoBehaviour {
	//消息窗口positon TWEEN    
	public TweenPosition tween_position; 
	public UISlider progress_bar;
	
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			transform.localScale = CommonData.iphonescale;
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			transform.localScale = CommonData.ipadscale;
		}
		
	}
	// Use this for initialization
	void Start () {
		tween_position.Play(true);
		progress_bar.sliderValue =0;
	}
	
	// Update is called once per frame
	void Update () {
		if (LoginManager.alreadyLoadMB)
		{
			progress_bar.sliderValue = 1;
			tween_position.Play(false);
		}
		if(!LoginManager.canLoadMB)
		{
			if(MBAutoUpdate.needUpdateFiles>0)
				progress_bar.sliderValue = (float)MBAutoUpdate.updatedFiles/(float)MBAutoUpdate.needUpdateFiles;
		}
	}
}
