using UnityEngine;
using System.Collections;
using Squared.Tiled;

public class TiledStorge : MonoBehaviour {
	
	public GameObject template;
	public int spacing = 90;
	public int mapWidth = 20;
	public int mapHeight = 20;
	TiledCoord CurCoordXY = null;

	private int _rowSize = 10;
	private int _numRows = 8;
	TiledStorgeSlot[,] mCacheSlots;

	/// <summary>
	/// The mTiledMap car x.
	/// </summary>
	public int mapCarX = 0;
	public int mapCarY = 0;
	
	Vector3 spCarX = Vector3.zero;
	Vector3 spCarY = Vector3.zero;
	Vector3 mBasedOnDelta = Vector3.zero;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (CurCoordXY == null) {
			CurCoordXY = GetComponent<TiledCoord>();
		}
		
		onCreateOrReposition();
	}
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		SceneManager.SetObjVisibleByTag("UnGlobal",false);
	}
	
	void onCreateOrReposition()
	{
		if (template != null)
		{
			Vector3 offset = new Vector3(-spacing * (_rowSize/2), spacing * (_numRows/2),0f);
			mCacheSlots = new TiledStorgeSlot[_numRows,_rowSize];
			for (int i=0; i<_numRows; ++ i)
			{
				for (int j=0; j<_rowSize; ++ j)
				{
					GameObject go = NGUITools.AddChild(gameObject, template);
					Transform t = go.transform;
					t.localPosition = new Vector3(offset.x + (j+0.5f) * spacing, offset.y - (i+0.5f) * spacing, 0.0f);
					
					TiledStorgeSlot ts = go.GetComponent<TiledStorgeSlot>();
					ts.SetCScene(this);
					ts.mapX = j;
					ts.mapY = i;
					
					mCacheSlots[i,j] = ts;
				}
			}
			
			spCarX = new Vector3(spacing,0f,0f);
			spCarY = new Vector3(0f,spacing,0f);
		}
	}
	
	public void MoveDrag(Vector4 clipRange)
	{
		int ox = 0;
		int oy = 0;
		
		if (Mathf.Abs(mBasedOnDelta.x) > (spacing))
		{
			ox = (int) (mBasedOnDelta.x/spacing);
			mBasedOnDelta.x -= (ox*spacing);
		}
		
		if (Mathf.Abs(mBasedOnDelta.y) > (spacing))
		{
			oy = (int) (mBasedOnDelta.y/spacing);
			mBasedOnDelta.y -= (oy*spacing);
		}
		
		int toCarY = (_numRows + mapCarY - 1) % _numRows;
		int toCarX = (_rowSize + mapCarX - 1) % _rowSize;		
		float hx = clipRange.z * 0.5f;
		float hy = clipRange.w * 0.5f;
		float twoSpacing = 2.0f * spacing;
	
		if (ox<0)
		{	
			// Move left to right
			int num = Mathf.Abs(ox);
			int toc = mapCarX;
			
			TiledStorgeSlot t = mCacheSlots[0,mapCarX];
			TiledStorgeSlot t1 = mCacheSlots[0,toCarX];
			if ((t1.mapX<(mapWidth-1)) && (t.gameObject.transform.localPosition.x + hx < -twoSpacing))
			{
				int ox1 = mapWidth-t1.mapX-1;
				if (ox1<num) num = ox1;
				for (int j=0; j<_numRows; ++ j)
				{
					t = mCacheSlots[j,toCarX];
					for (int i=0; i<num; ++ i)
					{
						toc = (_rowSize + mapCarX + i) % _rowSize;
						TiledStorgeSlot ts = mCacheSlots[j,toc];
						ts.mapX =  t.mapX + 1;
						
						ts.gameObject.transform.localPosition = t.gameObject.transform.localPosition + spCarX;
						t = ts;
					}
				}
			
				mapCarX = (_rowSize + toc + 1) % _rowSize;
			}
		}
		
		else if (ox>0)
		{
			// Move right to left
			int num = Mathf.Abs(ox);
			int toc = toCarX;
		
			TiledStorgeSlot t = mCacheSlots[0,toCarX];
			TiledStorgeSlot t0 = mCacheSlots[0,mapCarX];
			if ((t0.mapX>0) && (t.gameObject.transform.localPosition.x - hx > twoSpacing))
			{	
				if (t0.mapX<num) num = t0.mapX;
				for (int j=0; j<_numRows; ++ j)
				{
					t = mCacheSlots[j,mapCarX];
					for (int i=0; i<num; ++ i)
					{
						toc = (_rowSize + toCarX - i) % _rowSize;
						TiledStorgeSlot ts = mCacheSlots[j,toc];
						ts.mapX = t.mapX - 1;
						ts.gameObject.transform.localPosition = t.gameObject.transform.localPosition - spCarX;
						
						t = ts;
					}
				}
				
				mapCarX = toc;
			}
		}
		
		if (oy<0)
		{
			// Move down to up
			int num = Mathf.Abs(oy);
			int toc = toCarY;
			
			// Test tail can be dragged
			TiledStorgeSlot t = mCacheSlots[toCarY,0];
			TiledStorgeSlot t0 = mCacheSlots[mapCarY,0];
			if ((t0.mapY>0) && (t.gameObject.transform.localPosition.y + hy < -twoSpacing))
			{
				if (t0.mapY < num) num = t0.mapY;
				for (int j=0; j<_rowSize; ++ j)
				{
					t = mCacheSlots[mapCarY,j];
					for (int i=0; i<num; ++ i)
					{
						toc = (_numRows + toCarY - i) % _numRows;
						TiledStorgeSlot ts = mCacheSlots[toc,j];
						ts.mapY = t.mapY - 1;
						ts.gameObject.transform.localPosition = t.gameObject.transform.localPosition + spCarY;
						t = ts;
					}
				}
			
				mapCarY = toc;
			}
		}
		else if (oy>0)
		{
			// Move up to down
			int num = Mathf.Abs(oy);
			int toc = mapCarY;
			
			// Test tail can be dragged
			TiledStorgeSlot t = mCacheSlots[mapCarY,0];
			TiledStorgeSlot t1 = mCacheSlots[toCarY,0];
			if ((t1.mapY<(mapHeight-1)) && (t.gameObject.transform.localPosition.y - hy > twoSpacing))
			{
				int oy1 = mapHeight - t1.mapY - 1;
				if (oy1 < num) num = oy1;
				for (int j=0; j<_rowSize; ++ j)
				{
					t = mCacheSlots[toCarY,j];
					for (int i=0; i<num; ++ i)
					{
						toc = (_numRows + mapCarY + i) % _numRows;
						TiledStorgeSlot ts = mCacheSlots[toc,j];
						ts.mapY = t.mapY + 1;;
						ts.gameObject.transform.localPosition = t.gameObject.transform.localPosition - spCarY;
						t = ts;
					}
				}
			
				mapCarY = (_numRows + toc + 1) % _numRows;
			}
		}	
	}
	
	/// <summary> 
	/// Drag storge slots
	/// </summary>
	public void Drag (Vector3 delta)
	{		
		mBasedOnDelta += delta;
		
		// Move the all slots.
		Transform myTrans = transform;
		foreach(Transform go in myTrans)
		{
			go.localPosition += delta;
		}
	}
	
	public void SetFocus1(int x, int y)
	{
		if (CurCoordXY != null) {
			CurCoordXY.Set1(x, y);
		}
	}
	
	public Vector3 RestrictVisibleLocation(int X, int Y)
	{		
		this.mBasedOnDelta = Vector3.zero;
		
		int MidLine = _numRows / 2;
		int MidCol = _rowSize / 2;
		int[] prevLine = new int[_rowSize];
		
		int startLine = MidCol;
		if (X>=0 && X<MidCol) 
		{
			startLine = X;
		}
		else if (X>=395 && X<400)
		{
			startLine = _rowSize + X - 400;
		}
		
		int c=0;
		for(int i=startLine; i>0; -- i, ++ c)
		{
			prevLine[c] = X-i;
		}
		
		int c1 = c;
		for (int i=0;c<_rowSize; ++ c, ++ i)
		{
			prevLine[c] = X+i;
		}
		
		int[] prevCol = new int[_numRows];
		int startCol = MidLine;
		if (Y>=0 && Y<MidLine)
		{
			startCol = Y;
		}
		else if (Y>=396 && Y<400)
		{
			startCol = _numRows + Y - 400;
		}
		
		c = 0;
		for(int i=startCol; i>0; -- i, ++ c)
		{
			prevCol[c] = Y-i;
		}
		int c2 = c;
		for (int i=0;c<_numRows; ++ c, ++ i)
		{
			prevCol[c] = Y+i;
		}
		
		int tocX = this.mapCarX;
		int tocY = this.mapCarY;
		
		// 
		for (int j=0; j<_numRows; ++ j)
		{
			tocY = (_numRows + mapCarY + j) % _numRows;
			for (int ca=0; ca<_rowSize; ++ ca)
			{
				tocX = (_rowSize + mapCarX + ca) % _rowSize;
				TiledStorgeSlot go = mCacheSlots[tocY,tocX];
				go.mapX = prevLine[ca];
				go.mapY = prevCol[j];
			}
		}
		
		tocX = (_rowSize + mapCarX + c1) % _rowSize;
		tocY = (_numRows + mapCarY + c2) % _numRows;
		TiledStorgeSlot center = mCacheSlots[tocY,tocX];
		
		// 设置焦点 ...
		this.SetFocus1(center.mapX, center.mapY);
		TiledStorgeCacheData.instance.GPSFocus(center);
		
		Transform myTrans = center.transform;
		return (Vector3.zero - myTrans.localPosition);
	}
	
	public void ReUnpack1()
	{
		int tocX = this.mapCarX;
		int tocY = this.mapCarY;
		for (int j=0; j<_numRows; ++ j)
		{
			tocY = (_numRows + mapCarY + j) % _numRows;
			for (int ca=0; ca<_rowSize; ++ ca)
			{
				tocX = (_rowSize + mapCarX + ca) % _rowSize;
				TiledStorgeSlot go = mCacheSlots[tocY,tocX];
				go.ResetItem();
			}
		}
	}
}