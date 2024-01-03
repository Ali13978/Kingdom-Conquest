using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class NanBeizhanDiTu : MonoBehaviour {
	
	Vector3[] mArrange = new Vector3[11];
	
	public NanBeizhanLaunchWin Launcher = null;
	public UILabel DiTuLevelCol = null;
	public GameObject template = null;
	
	NanBeizhanCli[] mCRoadCol = new NanBeizhanCli[5];
	
	int mCDiTuCar = 1;
	
	string mPrettyLevel;
	
	void Awake()
	{
		if (DiTuLevelCol != null) {
			mPrettyLevel = DiTuLevelCol.text;
		}
		
		
		onCreateOrReposition();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	
	void onCreateOrReposition()
	{
		if (template != null)
		{
			mArrange = new Vector3[11];
			mArrange[0] = new Vector3(-118f,	65f,	0f);
			mArrange[1] = new Vector3(94f, 		90f, 	0f);
			mArrange[2] = new Vector3(-94f,		-88f, 	0f);
			mArrange[3] = new Vector3(-60f, 	-10f,	0f);
			mArrange[4] = new Vector3(110f,		-60f,	0f);
			mCRoadCol = new NanBeizhanCli[5];
			for(int i=0, imax=5; i<imax; ++ i)
			{
				GameObject go = NGUITools.AddChild(gameObject, template);
				go.transform.localPosition = mArrange[i];
				NanBeizhanCli t = go.GetComponent<NanBeizhanCli>();
				if (t != null)
				{
					t.SetCScene(Launcher);
					mCRoadCol[i] = t;
				}
			}
			
			if (DiTuLevelCol != null) {
				DiTuLevelCol.text = string.Format(mPrettyLevel,1);
			}
		}
	}
	
	public void SetCDiTuCar(int newCar)
	{
		mCDiTuCar = newCar;
		int d1 = newCar - 1;
		NanBeizhanDiTuCli diTu = NanBeizhanInstance.instance.GetDiTuCli(d1);
		if (diTu != null)
		{
			int i;
			int imax = 5;
			for (i=0; i<imax; ++ i)
			{
				mCRoadCol[i].HiddenCol();
			}
			
			imax = Mathf.Min(NanBeizhanInstance.NanBeizhanLevelNum,diTu.d5.Count);
			for (i=0; i<imax; ++ i)
			{
				NanBeizhanMBInfo obj = diTu.d5[i];
				int gx = obj.nCGuard - 1;
				if (gx<0 || gx>4) continue;
				
				NanBeizhanCli pic = mCRoadCol[gx];
				if (pic != null) {
					pic.ApplyCli(obj);
				}
			}
		}
		
		if (DiTuLevelCol != null) {
			DiTuLevelCol.text = string.Format(mPrettyLevel,mCDiTuCar);
		}
	}
	
	void OnNanBeizhanCDiTuPrev()
	{
		int newCar = mCDiTuCar;
		if (newCar == 1) {
			newCar = NanBeizhanInstance.NanBeizhanLevelNum;
		}
		else
		{
			newCar = Mathf.Max(1,mCDiTuCar-1);
		}
		
		if (newCar != mCDiTuCar)
		{
			SetCDiTuCar(newCar);
		}
	}
	
	void OnNanBeizhanCDiTuNext()
	{
		int newCar = mCDiTuCar;
		if (mCDiTuCar == NanBeizhanInstance.NanBeizhanLevelNum) {
			newCar = 1;
		}
		else 
		{	
		 	newCar = Mathf.Min(NanBeizhanInstance.NanBeizhanLevelNum,mCDiTuCar+1);
		}
		if (newCar != mCDiTuCar)
		{
			SetCDiTuCar(newCar);
		}
	}
	
}
