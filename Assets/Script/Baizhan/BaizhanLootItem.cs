using UnityEngine;
using System.Collections;
using CMNCMD;

public class BaizhanLootItem : PagingStorageSlot {
	
	public UISprite Faceid = null;
	public UILabel LootNameCol = null;
	public UILabel LootLevelCol = null;
	
	bool hasChanged = false;
	int m_id = -1;

	BaizhanLootScript mBaizPanel = null;
	
	void Awake()
	{
		if (LootNameCol != null)
		{
			LootNameCol.enabled = false;
		}
	}
	
	// Use this for initialization
	void Start () {
	
		if (mBaizPanel == null)
		{
			mBaizPanel = NGUITools.FindInParents<BaizhanLootScript>(gameObject);
		}
	}
	
	
	override public int gid 
	{
		get {
			return m_id;
		}
		
		set {
			
			if (m_id != value)
			{
				m_id = value;
				hasChanged = true;
			}
		}
	}
	
	override public void ResetItem() {}
	
	void LateUpdate() {
		
		if (true == hasChanged)
		{
			hasChanged = false;
					
			if (mBaizPanel == null) return;
			BaizInstanceLoot item = mBaizPanel.GetItem(m_id);
			if (item == null)
			{
				if (Faceid != null)
				{
					Faceid.enabled = false;
				}
				
				if (LootNameCol != null)
				{
					LootNameCol.enabled = false;
				}

				return;
			}

			if (Faceid != null)
			{
				ITEM_INFO d1 = U3dCmn.GetItemIconByExcelID((uint) item.nExcelID);
                if (d1.IconName != null)
                {
                    Faceid.spriteName = d1.IconName;
                    Faceid.MakePixelPerfect();
                    Faceid.enabled = true;
                }
				
			}
			
			if (LootNameCol != null)
			{
				uint excel_id = (uint) item.nExcelID;
				ITEM_INFO d1 = U3dCmn.GetItemIconByExcelID((uint) excel_id);
                if (d1.IconName != null)
                {
                    LootNameCol.text = d1.Name;
                    LootNameCol.enabled = true;
                }
			}
		}
	}
}
