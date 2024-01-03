using UnityEngine;
using System.Collections;

public class MessageBtn : MonoBehaviour {
	public MessageManager messagemanager;
	// Use this for initialization
	void Start () {
	
	}
	void OnEnable()
	{
		messagemanager.SetUnReadIcon();
	}
}
