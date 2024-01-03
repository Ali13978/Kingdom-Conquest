using UnityEngine;
using System.Collections;
using System;

public class PickingGeneralTipBar : MonoBehaviour {
	
	public GameObject RoadGuideCol = null;
	public bool RoadNoActive = false;
	
	// Use this for initialization
	void Start () {
		int endTimeSec = DataConvert.DateTimeToInt(DateTime.Now) + 2;
		Begin(endTimeSec);
	}
	
	void Begin(int endTimeSec)
	{
		StopCoroutine("RoadCooldown");
		StartCoroutine("RoadCooldown", endTimeSec);
	}
	
	IEnumerator RoadCooldown(int endTimeSec)
	{
		int nSecs = (endTimeSec - DataConvert.DateTimeToInt(DateTime.Now));
		if (nSecs <0) nSecs = 0;
		
		while(nSecs > 0)
		{
			nSecs = endTimeSec - DataConvert.DateTimeToInt(DateTime.Now);
			if(nSecs <0)
				nSecs = 0;	
			yield return new WaitForSeconds(1);
		}
		
		if (RoadGuideCol != null)
		{
			TweenPosition tween = RoadGuideCol.GetComponent<TweenPosition>();
			if (tween != null) { tween.style = UITweener.Style.Once; }
		}
		
		if (RoadNoActive == true)
		{
			NGUITools.SetActive(gameObject, false);
		}
	}
}
