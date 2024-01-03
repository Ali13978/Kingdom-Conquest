using UnityEngine;
using System.Collections;

public class AresRankItem : MonoBehaviour {
	
	public UILabel RankCol = null;
	public UILabel nameCol = null;
	public UILabel LevelCol = null;
	public UILabel guardCol = null;
	
	public UISprite imageCol = null;
	
	ulong mAresAccountID = 0;
	
	void Awake() 
	{
		if (imageCol != null) {
			imageCol.enabled = false;
		}
		if (RankCol != null) {
			RankCol.enabled = false;
		}
		if (nameCol != null) {
			nameCol.enabled = false;
		}
		if (LevelCol != null) {
			LevelCol.enabled = false;
		}
		if (guardCol != null) {
			guardCol.enabled = false;
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ApplyAresRankCard(AresRankCard card)
	{
		if (card == null)
		{
			if (imageCol != null) {
				imageCol.enabled = false;
			}
			if (RankCol != null) {
				RankCol.enabled = false;
			}
			if (nameCol != null) {
				nameCol.enabled = false;
			}
			if (LevelCol != null) {
				LevelCol.enabled = false;
			}
			if (guardCol != null) {
				guardCol.enabled = false;
			}
			
			return;
		}
		
		mAresAccountID = card.nAccountID;
		
		if (imageCol != null) {
			imageCol.enabled = true;
		}
		if (RankCol != null) {
			RankCol.text = card.nRank.ToString();
			RankCol.enabled = true;
		}
		if (nameCol != null) {
			nameCol.text = card.name;
			nameCol.enabled = true;
		}
		if (LevelCol != null) {
			LevelCol.text = card.nLevel.ToString();
			LevelCol.enabled = true;
		}
		if (guardCol != null) {
			guardCol.text = card.nData.ToString();
			guardCol.enabled = true;
		}	
	}
	
	void OnClick()
	{
		GameObject obj  = U3dCmn.GetObjFromPrefab("PlayerCardWin");
		if (obj == null) return;			
		
		obj.SendMessage("RevealPanel",mAresAccountID);
	}
}
