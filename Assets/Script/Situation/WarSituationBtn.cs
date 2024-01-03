using UnityEngine;
using System.Collections;

public class WarSituationBtn : MonoBehaviour {
	public GameObject unreadicon;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnEnable()
	{
		unreadicon.SetActiveRecursively(false);
	}
}
