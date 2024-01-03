using UnityEngine;
using System.Collections;

public class NanBeizhanGuard : MonoBehaviour {
	
	public UILabel PrettyGuardCol = null;
	public tk2dSprite ImageCol = null;
	public UISprite ImageGuardCol = null;
	public tk2dAnimatedSprite OnDoorCol = null;
	public tk2dAnimatedSprite ByDoorCol = null;
	
	public enum GuardStatus
	{
		Passed = 0,
		no = 1,
		Door = 2,
		OnDoor = 3,
	};
	
	int m_id = -1;
	string mPrettyCol;
	
	int mIsBoss = 0;
	public GuardStatus PrettyStatus = GuardStatus.no;
	
	NanBeizhanScene mCScene = null;
	
	void Awake() {
		
		if (PrettyGuardCol != null)
		{
			mPrettyCol = PrettyGuardCol.text;
			PrettyGuardCol.enabled = false;
		}
		
		if (OnDoorCol != null) {
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		
		if (ByDoorCol != null) {
			NGUITools.SetActive(ByDoorCol.gameObject, false);
		}
	}
	
	public int gid
	{
		get {
			
			return m_id;
		}
		
		set {
			
			m_id = value;
			
			if (PrettyGuardCol != null)
			{
				PrettyGuardCol.text = string.Format(mPrettyCol,m_id);
				PrettyGuardCol.enabled = (m_id >= 0);
			}
		}
	}
	
	public void BossSet1()
	{
		mIsBoss = 1;
		if (ImageCol != null) {
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("nzbz-da");
		}
	}
	
	public void PassedSet1()
	{
		if (OnDoorCol != null) {
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		if (ByDoorCol != null) {
			NGUITools.SetActive(ByDoorCol.gameObject,false);
		}
		
		if (mIsBoss == 1) return;
		if (ImageCol != null) {
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("nzbz-xiao-gray");
		}

	}
	
	public void Reset1()
	{
		if (OnDoorCol != null) {
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		if (ByDoorCol != null) {
			NGUITools.SetActive(ByDoorCol.gameObject,false);
		}
		if (mIsBoss == 1) return;
		if (ImageCol != null) {
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("nzbz-xiao");
		}
	}
	
	public void Warc1()
	{
		if (OnDoorCol != null) {
			NGUITools.SetActive(OnDoorCol.gameObject, true);
			OnDoorCol.Play();
		}
		if (ByDoorCol != null) {
			NGUITools.SetActive(ByDoorCol.gameObject,false);
		}
	}
	
	public void Door1()
	{
		if (OnDoorCol != null) {
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		if (ByDoorCol != null) {
			NGUITools.SetActive(ByDoorCol.gameObject,true);
			ByDoorCol.Play();
		}
		if (mIsBoss == 1) return;
		if (ImageCol != null) {
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("nzbz-xiao");
		}
	}
	
	public void SetCScene(NanBeizhanScene win)
	{
		this.mCScene = win;
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnClick()
	{

		if (mCScene == null) return;
		
		if (PrettyStatus == GuardStatus.Door )
		{
			mCScene.PopupNanBeizhanGuardWin(m_id);
		}
		else if (PrettyStatus == GuardStatus.no)
		{
			
		}
	}
	
	
}
