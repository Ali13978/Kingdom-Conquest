using UnityEngine;
using System.Collections;

// 地图关卡描述 ...
public class BaizGuard : MonoBehaviour {
	
	public UILabel PrettyGuardCol = null;
	public tk2dSprite ImageCol = null;
	public UISprite ImageGuardCol = null;
	public tk2dAnimatedSprite OnDoorCol = null;
	public tk2dAnimatedSprite ByDoorCol = null;
	
	public enum GuardStatus
	{
		Passed = 0,				// 已过关状态 ...
		no = 1,					// 未打 ...
		Door = 2,				// 当前要攻打的关卡 ...
		OnDoor = 3,				// 正在攻打的关卡 ...
	};
	
	BaizhanCampWin mCScene = null;
	public GuardStatus PrettyStatus = GuardStatus.no;

	int m_id = 0;
	string mPretty;
	
	int mIsBoss = 0;
	
	public int idGuard
	{
		get {
			
			return m_id;
		}
		
		set {
			
			m_id = value;
			
			if (PrettyGuardCol != null)
			{
				PrettyGuardCol.text = string.Format(mPretty,m_id);
				PrettyGuardCol.enabled = (m_id>0);
			}
		}
	}
	
	void Awake() {
		if (PrettyGuardCol != null) 
		{ 
			mPretty = PrettyGuardCol.text; 
			PrettyGuardCol.enabled = false;
		}
		if (OnDoorCol != null)
		{
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		
		if (ByDoorCol != null) 
		{
			NGUITools.SetActive(ByDoorCol.gameObject, false);
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	public void SetCScene(BaizhanCampWin obj)
	{
		this.mCScene = obj;
	}
	
	public void BossSet1()
	{
		mIsBoss = 1;
		if (ImageCol != null) 
		{
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("baizh-da");
		}
	}
	
	public void PassedSet1()
	{
		if (OnDoorCol != null)
		{
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		if (ByDoorCol != null) 
		{
			NGUITools.SetActive(ByDoorCol.gameObject, false);
		}
		if (mIsBoss == 1) return;
		if (ImageCol != null)
		{
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("baizh-xiao-gray");
		}
	}
	
	public void Reset1()
	{
		if (OnDoorCol != null)
		{
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		if (ByDoorCol != null) 
		{
			NGUITools.SetActive(ByDoorCol.gameObject, false);
		}
		
		if (mIsBoss == 1) return;
		if (ImageCol != null)
		{
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("baizh-xiao");
		}
	}
	
	public void Warc1()
	{
		if (OnDoorCol != null)
		{
			NGUITools.SetActive(OnDoorCol.gameObject, true);
			OnDoorCol.Play();
		}
		if (ByDoorCol != null) 
		{
			NGUITools.SetActive(ByDoorCol.gameObject, false);
		}
	}
	
	public void Door1()
	{
		if (OnDoorCol != null)
		{
			NGUITools.SetActive(OnDoorCol.gameObject, false);
		}
		if (ByDoorCol != null) 
		{
			NGUITools.SetActive(ByDoorCol.gameObject, true);
			ByDoorCol.Play();
		}
		if (mIsBoss == 1) return;
		if (ImageCol != null)
		{
			ImageCol.spriteId = ImageCol.GetSpriteIdByName("baizh-xiao");
		}
	}
	
	void OnClick()
	{
		if (PrettyStatus == GuardStatus.Door )
		{
			if (mCScene != null ) {
				mCScene.PopupBaizhanGuardWin(m_id);
			}
		}
		else if (PrettyStatus == GuardStatus.no)
		{
			// BaizhanCampWin.PopupBaizhanGuardLoot(m_id);
		}
	}
}
