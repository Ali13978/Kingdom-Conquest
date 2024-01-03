using UnityEngine;
using System.Collections;
using TinyWar;

public class DragDeployScene : MonoBehaviour {
	public ArmyDeployWin deploy_win;
    public Rect Bounds;
    private bool isPressed;
    private Vector3 lastPosition;
    public GameObject drag_obj;

    void Awake() {
        if (U3dCmn.GetIphoneType() == CMNCMD.IPHONE_TYPE.IPHONE5) { 
            this.Bounds.xMin = -436;
            this.Bounds.xMax = -44;
        }
    }

	
	// Update is called once per frame
	void Update () {  
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    if (Input.GetMouseButtonDown(0))
    {
        isPressed = true;
        lastPosition = Input.mousePosition;
    }
    if (Input.GetMouseButtonUp(0))
    {
        isPressed = false;
    }
    if (isPressed) {
        this.MoveCamera();
    }        
#endif
        if (Input.touchCount == 1) {
            Touch touch = Input.touches[0];
            switch (touch.phase) { 
                case TouchPhase.Began:
                    isPressed = true;
                    lastPosition = Input.mousePosition;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isPressed = false;
                    lastPosition = Input.mousePosition;
                    break;
                case TouchPhase.Moved:
                    this.MoveCamera();
                    break;
            }
        }
		lastPosition = Input.mousePosition;
	}

    private void MoveCamera() {
		if(ArmyDeployManager.deploy_type == CMNCMD.COMBAT_TYPE.COMBAT_INSTANCE_GUILD)
		{
			if(!ArmyDeployManager.is_drag)
			{
				
				float minX = Bounds.xMin;
		        float maxX = Bounds.xMax;
		        Vector3 moved = -(Input.mousePosition - lastPosition);
		        if(Mathf.Abs(moved.x) <3)
					return ;
		        float newX = drag_obj.transform.localPosition.x - moved.x;
		        if (newX < minX) {
		            newX = minX;
		        }
		        if (newX > maxX) {
		            newX = maxX;
		        }
		        lastPosition = Input.mousePosition;
		        drag_obj.transform.localPosition = new Vector3(newX, drag_obj.transform.localPosition.y, drag_obj.transform.localPosition.z);
			}
		}
		
			
        
    }
}
