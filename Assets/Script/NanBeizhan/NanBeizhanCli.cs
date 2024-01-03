using UnityEngine;
using System.Collections;

public class NanBeizhanCli : MonoBehaviour {
	
	public UILabel PrettyNameCol = null;
	public UILabel PowerCol = null;
	public UISlicedSprite Backgound = null; 
	
	NanBeizhanLaunchWin mCScene = null;
	
	int m_id = 0;
	
	string mPrettyPower;
	
	void Awake() {
		if (PowerCol != null) {
			mPrettyPower = PowerCol.text;
			PowerCol.enabled = false;
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	
	public void SetCScene(NanBeizhanLaunchWin obj)
	{
		this.mCScene = obj;
	}
	
	public void HiddenCol()
	{
		if (PrettyNameCol != null) {
			PrettyNameCol.enabled = false;
		}
		
		if (Backgound != null) {
			Backgound.enabled = false;
		}
		
		if (PowerCol != null) {
			PowerCol.enabled = false;
		}
		gameObject.GetComponent<Collider>().enabled = false;
	}
	
	public void ApplyCli(NanBeizhanMBInfo obj)
	{
		m_id = (int) obj.ExcelID;
		
		if (PrettyNameCol != null)
		{
			PrettyNameCol.text = obj.name;
			PrettyNameCol.enabled = true;
			
			if (Backgound != null) {
				Backgound.enabled = true;
			}
		}
		
		if (PowerCol != null) 
		{
			PowerCol.text = string.Format(mPrettyPower,obj.nProf);
			PowerCol.enabled = true;
		}
		gameObject.GetComponent<Collider>().enabled = true;
	}
	
	void OnClick()
	{
		if (mCScene != null)
		{
			mCScene.PopupNanBeizhanHall(m_id);
		}
	}
}
