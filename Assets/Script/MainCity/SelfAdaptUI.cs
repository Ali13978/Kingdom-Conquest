using UnityEngine;
using System.Collections;
using CMNCMD;
public class SelfAdaptUI : MonoBehaviour {
	public GameObject TopObj1;
	public GameObject TopObj2;
	public GameObject BottomObj;
	
	void Awake()
	{
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			TopObj1.transform.localPosition = TopObj1.transform.localPosition+new Vector3(0,20,0);
			TopObj2.transform.localPosition = TopObj2.transform.localPosition+new Vector3(0,20,0);
			BottomObj.transform.localPosition = BottomObj.transform.localPosition+new Vector3(0,-20,0);
		}
	}
	// Use this for initialization
	void Start () {
	
	}
}
