using UnityEngine;
using System.Collections;
using TinyWar;

public class MoveCameraBehavior : MonoBehaviour {
    public Rect Bounds;
    private bool isPressed;
    private Vector3 lastPosition;
    public GameObject battleWin;

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
	}

    private void MoveCamera() {
        float minX = Bounds.xMin;
        float maxX = Bounds.xMax;
        Vector3 moved = -(Input.mousePosition - lastPosition);
        
        float newX = battleWin.transform.localPosition.x - moved.x;
        if (newX < minX) {
            newX = minX;
        }
        if (newX > maxX) {
            newX = maxX;
        }
        lastPosition = Input.mousePosition;
        battleWin.transform.localPosition = new Vector3(newX, battleWin.transform.localPosition.y, battleWin.transform.localPosition.z);
    }
}
