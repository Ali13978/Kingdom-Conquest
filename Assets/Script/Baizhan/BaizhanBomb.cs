using UnityEngine;
using System.Collections;
using System;

public class BaizhanBomb : MonoBehaviour {
	
	public tk2dAnimatedSprite sprite = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	public void Set(ArmyCampColor campId)
	{
		string playClipId = "";
		
		if (campId == ArmyCampColor.Red) 
		{
			playClipId = "red";
		}
		else if (campId == ArmyCampColor.Blue) {
			playClipId = "blue";
		}
		
		playClipId += "/xie/bomb";
	
		// Play clip id fini
		this.sprite.Play(playClipId);
		
		int endTimeSec = 3 + DataConvert.DateTimeToInt(DateTime.Now);
		Begin(endTimeSec);
		
	}
	
	public void Begin(int endTimeSec)
	{
		StopCoroutine("BombCooldown");
		StartCoroutine("BombCooldown", endTimeSec);
	}
	
	// 战斗Cooldown 
	IEnumerator BombCooldown(int endTimeSec)
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
