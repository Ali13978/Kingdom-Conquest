using UnityEngine;
using System.Collections;
using System;

public class BaizhanPoster : MonoBehaviour {
	
	public tk2dAnimatedSprite main = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	public void Play(string name)
	{
		if (main != null) 
		{
			gameObject.SetActiveRecursively(true);
			main.Play(name);
			main.color = Color.white;
			
			Begin();
		}
	}
	
	void Begin()
	{
		StopCoroutine("L1Cooldown");
		StartCoroutine("L1Cooldown");
	}

		// 战斗Cooldown 
	IEnumerator L1Cooldown()
	{		
		yield return new WaitForSeconds(1);
		
		float curTime = 1.0f;
		while (curTime>0.0f)
		{
			curTime -= 0.03f;
			float b = Mathf.Lerp(1.0f,0.0f,1.0f-curTime);
			main.color = new Color(1.0f,1.0f,1.0f,Mathf.Max(0f,b));
			
			yield return new WaitForSeconds(0.03f);
		}

		gameObject.SetActiveRecursively(false);
	}
}
