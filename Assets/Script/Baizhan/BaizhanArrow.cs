using UnityEngine;
using System.Collections;
using System;

public class BaizhanArrow : MonoBehaviour {
	
	public tk2dAnimatedSprite sprite = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void Set(ArmyCampColor campId)
	{
		string playClipId = "anim";
		Transform myTrans = transform;
		Vector3 target = myTrans.localPosition;
		if (campId == ArmyCampColor.Red) 
		{
			target = myTrans.localPosition + new Vector3(40f,0f,0f);
		}
		else if (campId == ArmyCampColor.Blue) 
		{
			target = myTrans.localPosition + new Vector3(-40f,0f,0f);
			sprite.FlipX();
		}
		
		playClipId += "/gong/jianyu";
		
		TweenPosition tween = TweenPosition.Begin(gameObject, 0.02f, target);
		tween.style = UITweener.Style.Once;
		tween.method = UITweener.Method.EaseOut;
		tween.Play(true);
	
		// Play clip id fini
		this.sprite.Play(playClipId);
				
		int endTimeSec = 2 + DataConvert.DateTimeToInt(DateTime.Now);
		Begin(endTimeSec);
	}
	
	public void Begin(int endTimeSec)
	{
		StopCoroutine("ArrowCooldown");
		StartCoroutine("ArrowCooldown", endTimeSec);
	}
	
	// 战斗Cooldown 
	IEnumerator ArrowCooldown(int endTimeSec)
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
		
		gameObject.SetActiveRecursively(false);
		Destroy(gameObject);
	}
}
