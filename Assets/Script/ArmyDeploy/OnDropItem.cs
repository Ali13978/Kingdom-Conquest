using UnityEngine;
using System.Collections;

public class OnDropItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnDrop (GameObject go)
	{
		U3dCmn.SendMessage("ArmyDeployWin","RevertItem",null);
	}
}
