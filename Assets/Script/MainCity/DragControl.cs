using UnityEngine;
using System.Collections;

public class DragControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnPress (bool pressed)
	{
		if(pressed)
			MainCity.touch_enabel = true;
		else
			MainCity.touch_enabel = false;
	}
	void OnDrag (Vector2 delta)	
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
	}
}
