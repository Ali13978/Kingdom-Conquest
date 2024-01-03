using UnityEngine;
using CMNCMD;

public class TiledDraggablePanel : IgnoreTimeScale {
	
	public enum DragEffect
	{
		None,
		Momentum,
	}
	
	/// <summary>
	///  Effect to apply when dragging. 
	/// </summary> 

	public DragEffect dragEffect = DragEffect.Momentum;
	
	/// <summary>
	///  How much momentum gets applied when the press is released after dragging.
	/// </summary>
	
	public float momentumStrength = 9f;
	public float momentumAmount = 35f;
	Vector3 scale = Vector3.one;
	
	public UIPanel mPanel;
	Vector3 mLastPos;
	Vector3 mBasedOnDelta = Vector3.zero;
	Transform mTrans;
	
	bool mPressed = false;
	Vector3 mMomentum = Vector3.zero;
	Bounds mBounds;
	
	bool mCalculatedBounds = false;
	bool mShouldMove = false;

	Plane mPlane;
	int mTouches = 0;
	
	public bool restrictWithinPanel = true;
	
	public TiledStorge draggableStorge = null;
	
	/// <summary>
	///  Calculate the bounds used by the widgets.
	/// </summary>
	public Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
			}
			
			return mBounds;
		}
	}
	
	/// <summary>
	/// Current momentum, exposed just in case it's needed.
	/// </summary>

	public Vector3 currentMomentum { get { return mMomentum; } set { mMomentum = value; } }
	
	/// <summary>
	/// Whether the contents of the panel should actually be draggable depends on whether they currently fit or not.
	/// </summary>

	bool shouldMove
	{
		get
		{
			if (mPanel == null) mPanel = GetComponent<UIPanel>();
			Vector4 clip = mPanel.clipRange;
			Bounds b = bounds;

			float hx = clip.z * 0.5f;
			float hy = clip.w * 0.5f;

			if (!Mathf.Approximately(scale.x, 0f))
			{
				if (b.min.x < clip.x - hx) return true;
				if (b.max.x > clip.x + hx) return true;
			}

			if (!Mathf.Approximately(scale.y, 0f))
			{
				if (b.min.y < clip.y - hy) return true;
				if (b.max.y > clip.y + hy) return true;
			}
			return false;
		}
	}
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake () 
	{
		mTrans = transform;
		mPanel = GetComponent<UIPanel>();
	}
	
	// Use this for initialization
	void Start ()
	{
		if (GlobeMapLocation.byFastLocation == 1)
		{
			GlobeMapLocation.byFastLocation = 0;
			int PosX = GlobeMapLocation.PosX;
			int PosY = GlobeMapLocation.PosY;
			SetLocation (PosX, PosY);
		}
		else 
		{
			CMN_PLAYER_CARD_INFO info = CommonData.player_online_info;
			SetLocation((int) info.PosX,(int) info.PosY);
		}
		
		// iPAD适应 ...
		if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPHONE)
		{
			if (U3dCmn.GetIphoneType() == IPHONE_TYPE.IPHONE5)
			{
				if (mPanel != null) { 
					mPanel.clipRange = new Vector4(0f,0f,570f,322f);
				}
			}
			else 
			{
				if (mPanel != null) { 
					mPanel.clipRange = new Vector4(0f,0f,482f,322f);
				}
			}
		}
		else if(U3dCmn.GetDeviceType() == DEVICE_TYPE.IPAD)
		{
			if (mPanel != null) { 
				mPanel.clipRange = new Vector4(0f,0f,482f,362f);
			}
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void RestrictWithinBounds ()
	{
		Vector3 constraint = mPanel.CalculateConstrainOffset(bounds.min, bounds.max);
		if (constraint.magnitude > 0.001f)
		{
			// Jump back into place
			MoveRelative(constraint);
			// mMomentum = Vector3.zero;
		}
		else
		{
			// Remove the spring as it's no longer needed
			DisableSpring();
		}
	}

	/// <summary>
	/// Disable spring panel
	/// </summary>
	void DisableSpring()
	{
		SpringPanel sp = GetComponent<SpringPanel>();
		if (sp != null) sp.enabled = false;
	}
	
	/// <summary>
	/// Move the panel by the specified amount
	/// </summary>
	void MoveRelative (Vector3 relative)
	{
		if (enabled && gameObject.active && draggableStorge != null)
		{
			mBasedOnDelta += relative;
			
			float x = Mathf.Floor(mBasedOnDelta.x);
			float y = Mathf.Floor(mBasedOnDelta.y);
			Vector3 delta = new Vector3(x, y, 0f);
			
			if (delta.magnitude > 0.005f)
			{
				// Move child transform
				draggableStorge.Drag(delta);
				mCalculatedBounds = false;
			}
			
			mBasedOnDelta -= delta;
		}
	}
	
	void MoveAbsolute (Vector3 absolute)
	{
		Vector3 a = mTrans.InverseTransformPoint(absolute);
		Vector3 b = mTrans.InverseTransformPoint(Vector3.zero);
		MoveRelative(a - b);
	}
	
	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>
	public void Press (bool pressed)
	{
		if (enabled && gameObject.active)
		{
			mTouches += (pressed ? 1: -1);
			mCalculatedBounds = false;
			mShouldMove = shouldMove;
			if (!mShouldMove) return;
			mPressed = pressed;
			
			if (pressed)
			{
				// Remove all momentum on press
				mMomentum = Vector3.zero;
				
				// Disable the spring movement
				DisableSpring();
				
				// Remember the last hit position
				mLastPos = UICamera.lastHit.point;
				
				// Create the plane to drag along
				mPlane = new Plane(mTrans.rotation * Vector3.back, mLastPos);
			}
			else if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
			{
				RestrictWithinBounds();
			}
		}
	}
	
	/// <summary>
	/// Drag the object along the plane.
	/// </summary>
	public void Drag (Vector2 delta)
	{
		if (enabled && gameObject.active && mShouldMove)
		{
			// If drag delta magnitude is less than 9f, no clicks
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float dist = 0f;
			if (mPanel == null) mPanel = GetComponent<UIPanel>();
			
			if (mPlane.Raycast(ray, out dist))
			{
				Vector3 currentPos = ray.GetPoint(dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;
				
				if (offset.x != 0f || offset.y != 0f)
				{
					offset = mTrans.InverseTransformDirection(offset);
					offset.Scale(scale);
					offset = mTrans.TransformDirection(offset);
				}
					
				// Adjust the momentum
				mMomentum = Vector3.Lerp(mMomentum, mMomentum + offset * (0.02f*momentumAmount), 0.67f);
				
				// Move the panel
				MoveAbsolute(offset);
				
				// We want to constrain the UI to be within bounds
				if (restrictWithinPanel &&
					mPanel.clipping != UIDrawCall.Clipping.None)
				{
					RestrictWithinBounds();
				}
				
				if (draggableStorge != null)
				{
					draggableStorge.MoveDrag(mPanel.clipRange);
				}
				
				// no stopped
				TiledStorgeCacheData.instance.isAutoDelay = true;
			}
		}
	}
	
	public void SetLocation(int X, int Y)
	{
		if (draggableStorge != null)
		{
			Vector3 delta = draggableStorge.RestrictVisibleLocation(X, Y);
			if (delta.magnitude > 0.005f)
			{
				draggableStorge.Drag(delta);
				mCalculatedBounds = false;
				RestrictWithinBounds();
				
				if (mPanel == null) mPanel = GetComponent<UIPanel>();
				draggableStorge.MoveDrag(mPanel.clipRange);
				mPanel.Refresh(); // 立即更新
			}
		}
	}
	
	public void ResetPacket1()
	{
		if (draggableStorge != null) {
			draggableStorge.ReUnpack1();
		}
	}
	
	/// <summary>
	/// Lates the update.
	/// </summary>
	public void LateUpdate ()
	{
#if UNITY_EDITOR
		// Don't play animations when not in play mode
		if (!Application.isPlaying)
			return;
#endif
		
		float delta = UpdateRealTimeDelta();

		// Apply the momentum
		if (mShouldMove && !mPressed)
		{
			if (mMomentum.magnitude > 0.01f)
			{				
				// 9f;
				// Move the panel
				Vector3 offset = NGUIMath.SpringDampen(ref mMomentum, momentumStrength, delta);
				MoveAbsolute(offset);
				
				// Restrict the contents to be within the panel's bounds
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None) RestrictWithinBounds();
				
				if (draggableStorge != null)
				{
					if (mPanel == null) mPanel = GetComponent<UIPanel>();
					draggableStorge.MoveDrag(mPanel.clipRange);
					mPanel.Refresh(); // 立即更新
				}
				
				return;
			}
			else if (true == TiledStorgeCacheData.instance.isAutoDelay)
			{
				TiledStorgeCacheData.instance.isAutoDelay = false;
				return;
			}
		}
		
		// Dampen the momentum
		NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
	}
	
#if UNITY_EDITOR

	/// <summary>
	/// Draw a visible orange outline of the bounds.
	/// </summary>

	void OnDrawGizmos ()
	{
		if (mPanel != null && mPanel.debugInfo == UIPanel.DebugInfo.Gizmos)
		{
			Bounds b = bounds;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 0.4f, 0f);
			Gizmos.DrawWireCube(new Vector3(b.center.x, b.center.y, b.min.z), new Vector3(b.size.x, b.size.y, 0f));
		}
	}
#endif
}
