using UnityEngine;
using System.Collections.Generic;

public class BaizFi : MonoBehaviour {
	
	BaizGuard[] mBaizSlots;
	
	public GameObject template = null;
	public UILabel PrettyBaizLabel = null;
	
	public BaizhanCampWin Scene = null;
	
	Vector3[] mBaizArrange;

	int mBaizhanCar = 1;
	int mBaizhanLevel  = 1;
	
	string mPretty = "{0} - {1}";
	
	void Awake() {
		
		onCreateOrReposition();
	}
	
	// Use this for initialization
	void Start () {
	}
	
	
	public int level
	{
		get {
			return mBaizhanLevel;
		}
	}
	
	// 战场关卡分布图 ....
	void onCreateOrReposition()
	{
		if (template != null)
		{
			mBaizArrange = new Vector3[10];
			mBaizArrange[0] = new Vector3(-9f,	-143f,	0f);
			mBaizArrange[1] = new Vector3(-92f,	-73f,	0f);
			mBaizArrange[2] = new Vector3(-184f,-2f,	0f);
			mBaizArrange[3] = new Vector3(-106f,47f,	0f);
			mBaizArrange[4] = new Vector3(24f,76f,	0f);
			mBaizArrange[5] = new Vector3(147f, 53f,	0f);
			mBaizArrange[6] = new Vector3(200f,	-26f,	0f);
			mBaizArrange[7] = new Vector3(156f,	-85f,	0f);
			mBaizArrange[8] = new Vector3(70f, -46f,	0f);
			mBaizArrange[9] = new Vector3(-18f, 15f,	0f);
			
			mBaizSlots = new BaizGuard[10];
			for(int i=0, imax=10; i<imax; ++ i)
			{
				GameObject go = NGUITools.AddChild(gameObject, template);
				go.transform.localPosition = mBaizArrange[i];
				BaizGuard t = go.GetComponent<BaizGuard>();
				t.SetCScene(Scene);
				t.idGuard = (i+1);
				
				if (i == 9) { t.BossSet1(); }
				
				mBaizSlots[i] = t;
			}
	
			if (PrettyBaizLabel != null)
			{
				PrettyBaizLabel.text = string.Format(mPretty, mBaizhanCar, mBaizhanCar+9);
			}
		}
	}
	
	void SetBaizhanCar(int newCar)
	{
		mBaizhanCar = newCar;
				
		if (PrettyBaizLabel != null)
		{
			PrettyBaizLabel.text = string.Format(mPretty, mBaizhanCar, mBaizhanCar+9);
		}

		int i=0;
		foreach (BaizGuard go in mBaizSlots)
		{
			go.idGuard = mBaizhanCar + i;
			
			if (go.idGuard > mBaizhanLevel)
			{
				go.PrettyStatus = BaizGuard.GuardStatus.no;
				go.Reset1();
				if (go.ImageGuardCol != null)
				{
					go.ImageGuardCol.color = Color.cyan;
				}
			}
			else if (go.idGuard == mBaizhanLevel)
			{
				go.PrettyStatus = BaizGuard.GuardStatus.Door;
				go.Door1();
				if (go.ImageGuardCol != null)
				{
					go.ImageGuardCol.color = Color.green;
				}
			}
			else 
			{
				go.PrettyStatus = BaizGuard.GuardStatus.Passed;
				go.PassedSet1();
				if (go.ImageGuardCol != null)
				{
					go.ImageGuardCol.color = Color.gray;
				}
			}
			
			i ++;
		}
	}
	
	// 向后翻页 ...
	void OnBaizhanNext()
	{
		int toCar = mBaizhanCar + 10;
		if (mBaizhanLevel<toCar)
		{
			return;
		}
		
		int newCar = Mathf.Min(91, mBaizhanCar + 10);
		if (newCar == mBaizhanCar) return;
		
		SetBaizhanCar(newCar);
	}
	
	// 向前翻页 ...
	void OnBaizhanPrev()
	{
		int newCar = Mathf.Max(1,mBaizhanCar - 10);
		if (newCar == mBaizhanCar) return;
		SetBaizhanCar(newCar);
	}
	
	// 设置当前的关卡号 ....
	public void SetBaizhanCurLevel(int nLevel)
	{
		mBaizhanLevel = nLevel;		
		if (mBaizhanLevel > (mBaizhanCar+9))
		{
			int paged = (nLevel-1)/10;
			int newCar = paged*10 + 1;
			SetBaizhanCar(newCar);
			return;
		}
				
		int i=0;
		int toCar = 10;
		int curCheck = mBaizhanLevel - mBaizhanCar;
		
		//未知关卡 
		int nDead = Mathf.Min(curCheck,toCar);
		for (i=0; i<nDead; ++ i)
		{
			BaizGuard go = mBaizSlots[i];
			go.PrettyStatus = BaizGuard.GuardStatus.Passed;
			go.PassedSet1();
			if (go.ImageGuardCol != null)
			{
				go.ImageGuardCol.color = Color.gray;
			}
		}
		
		BaizGuard goCur = mBaizSlots[i++];		
		goCur.PrettyStatus = BaizGuard.GuardStatus.Door;
		goCur.Door1();
		if (goCur.ImageGuardCol != null)
		{
			goCur.ImageGuardCol.color = Color.green;
		}
		
		//未知关卡 
		for (; i<toCar; ++ i)
		{
			BaizGuard go = mBaizSlots[i];
			go.PrettyStatus = BaizGuard.GuardStatus.no;
			if (go.ImageGuardCol != null)
			{
				go.ImageGuardCol.color = Color.cyan;
			}
		}
		
		// Fini
	}
	
	// 战斗标记当前的关卡号 ....
	public void SetBaizOnDoor(int gid)
	{
		int to = gid - mBaizhanCar;
		if (to<0 || to>9) return;

		BaizGuard goCur = mBaizSlots[to];
		goCur.PrettyStatus = BaizGuard.GuardStatus.OnDoor;
		goCur.Warc1();
	}
	
	// 清除战斗标记 ...
	public void StopAtPoster()
	{
		int to = level - mBaizhanCar;
		if (to<0 || to>9) return;
		BaizGuard goCur = mBaizSlots[to];
		goCur.PrettyStatus = BaizGuard.GuardStatus.Door;
		goCur.Door1();
	}
	
	// 是否是正在猛打 ...
	public bool isOnDoor()
	{
		int to = level - mBaizhanCar;
		if (to<0 || to>9) return false;
		BaizGuard goCur = mBaizSlots[to];
		return (goCur.PrettyStatus == BaizGuard.GuardStatus.OnDoor);
	}
	

}
