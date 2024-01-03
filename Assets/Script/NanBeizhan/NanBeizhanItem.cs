using UnityEngine;
using System.Collections;

public class NanBeizhanItem : MonoBehaviour {
	
	public UILabel PrettyNameCol = null;
	public UILabel PrettyHeroPowerCol = null;
	public UILabel ArmyOrReadyCol = null;
	
	string mPrettyHeroPower;	
	ulong mAccountID = 0;
	int m_id = -1;
	
	NanBeizhanRoom mRoomPanel = null;
	
	void Awake() {
		if (PrettyHeroPowerCol != null)
		{
			mPrettyHeroPower = PrettyHeroPowerCol.text;
			PrettyHeroPowerCol.text = string.Format(mPrettyHeroPower,0);
		}
		if (ArmyOrReadyCol != null)
		{
			NGUITools.SetActiveSelf(ArmyOrReadyCol.gameObject,false);
		}
		if (PrettyNameCol != null)
		{
			NGUITools.SetActiveSelf(PrettyNameCol.gameObject,false);
		}
	}
	
	// Use this for initialization
	void Start () {
	
		if (mRoomPanel == null)
		{
			mRoomPanel = NGUITools.FindInParents<NanBeizhanRoom>(gameObject);
		}
	}
	
	public int idCol
	{
		get {
			return m_id;
		}
		
		set {
			m_id = value;
		}
	}
	
	public ulong idAccount
	{		
		set {
			mAccountID = value;
		}
	}
	
	
	public void ApplyGeneralProf(int nProf)
	{
		if (PrettyHeroPowerCol != null)
		{
			PrettyHeroPowerCol.text = string.Format(mPrettyHeroPower, nProf);
		}
	}
	
	public void ApplyGeneralItem(BaizInstanceData item)
	{
		if (item == null) 
		{
			if (ArmyOrReadyCol != null)
			{
				NGUITools.SetActiveSelf(ArmyOrReadyCol.gameObject, false);
			}
			if (PrettyNameCol != null)
			{
				NGUITools.SetActiveSelf(PrettyNameCol.gameObject, false);
			}
			
			return;
		}
		
		if (PrettyNameCol != null)
		{
			PrettyNameCol.text = item.name;
			NGUITools.SetActiveSelf(PrettyNameCol.gameObject, true);
		}
		
		if (ArmyOrReadyCol != null) {
			NGUITools.SetActiveSelf(ArmyOrReadyCol.gameObject,(item.nStauts==1));
		}
	}

	
	public void OnOpenNanBeizhanLeader()
	{
		if (idCol == -1) return;
		if (mRoomPanel != null)
		{
			mRoomPanel.PopupNanBeizhanTroop(idCol, mAccountID);
		}
	}
	
}
