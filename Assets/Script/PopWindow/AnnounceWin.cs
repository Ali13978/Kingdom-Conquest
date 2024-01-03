using UnityEngine;
using System.Collections;
using CMNCMD;
public class AnnounceWin : MonoBehaviour {
	public TweenPosition tween_position; 
	public UILabel text_label;
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
	
	// Update is called once per frame
	void Update () {
	
	}
	void RevealPanel(string text)
	{
		text_label.text = text;
		tween_position.Play(true);
	}
	//隐藏窗口 
	void DismissPanel()
	{
		tween_position.Play(false);
		Destroy(gameObject);
	}
}
