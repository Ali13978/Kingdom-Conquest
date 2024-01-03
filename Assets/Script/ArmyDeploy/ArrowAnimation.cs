using UnityEngine;
using System.Collections;
using System;

public class ArrowAnimation : MonoBehaviour {
	
	public GameObject arrow = null;
	public bool NoActive = false;
	
	// Use this for initialization
	void Start () {
		arrow.SetActiveRecursively(false);
	}
	public void StartPlay()
	{
		int endTimeSec = DataConvert.DateTimeToInt(DateTime.Now) + 2;
		arrow.SetActiveRecursively(true);
		arrow.GetComponent<TweenPosition>().Play(true);
		StopCoroutine("AnimationCooldown");
		StartCoroutine("AnimationCooldown", endTimeSec);
	}
	IEnumerator AnimationCooldown(int endTimeSec)
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
		if (arrow != null)
		{
			TweenPosition tween = arrow.GetComponent<TweenPosition>();
			if (tween != null) { tween.style = UITweener.Style.Once; }
		}
		
		if (NoActive == true)
		{
			NGUITools.SetActive(gameObject, false);
		}
	}
}
