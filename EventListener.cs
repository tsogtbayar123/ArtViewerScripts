/****************************************************************
 * EventListener
 * It catches the screen drag, scrolling (zoom) touch events and
 * invoke event handling methods
 * Attached in PanelModelRenderer object.
 * **************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventListener : MonoBehaviour {
    //debug text
    public Text dbgText;

    int totalTouches;

    int resolutionX = 0;
    int resolutionY = 0;
	// Use this for initialization
	void Start () {
        CalculateScreenSize();

    }
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount < 1) {
            totalTouches = 0;
        }
	}


    public void Drag()
    {
  
        //Debug.Log("drag");
        if (Config.currentSceneType == GameScene.Browse3D)
        {
        
            if ( (Input.touchCount < 2) && (totalTouches < 2) )
            {
               totalTouches = 1;
                float rotX = Input.GetAxis("Mouse X");
                float rotY = Input.GetAxis("Mouse Y");
                if (Input.GetMouseButton(0))
                {
                    EventManager.DoDragMouse(rotX, rotY);
                }
                else
                {
                    EventManager.DoMove(rotX / 12, rotY / 12);
                }

            }
            if (Input.touchCount == 2)
            {
                totalTouches = 2;
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                float fNormalDifference = 6 * difference / resolutionY; //was 6


                if (Mathf.Abs(fNormalDifference) < Utils.THRESHOLD_PAN)
                {
                    float fNormalTouchX = touchZero.deltaPosition.x;
                    if (fNormalTouchX > touchOne.deltaPosition.x)
                        fNormalTouchX = touchOne.deltaPosition.x;
                    float fNormalTouchY = touchZero.deltaPosition.y;
                    if (fNormalTouchY > touchOne.deltaPosition.y)
                        fNormalTouchY = touchOne.deltaPosition.y;
                    fNormalTouchY = fNormalTouchY / resolutionY;
                    fNormalTouchX = fNormalTouchX / resolutionX;

                    EventManager.DoMove(fNormalTouchX, fNormalTouchY);
                }
                else
                {
                    EventManager.DoScrollMouse(fNormalDifference * 2f);
                }
            }

        } else if (Config.currentSceneType == GameScene.Browse2D || Config.currentSceneType == GameScene.MapView || Config.currentSceneType == GameScene.OnlineBrowse2D)
        {
            if (Input.touchCount < 2)
            {
                EventManager.DoDragMouse(Input.mousePosition.x, Input.mousePosition.y);
            } else
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                float fNormalDifference = 6 * difference / resolutionY;
                Vector2 v2CenterPos = (touchZero.position + touchOne.position) / 2;
                EventManager.DoPinchScreen(fNormalDifference, v2CenterPos);
            }
        }
    }

    public void Move()
    {
        Debug.Log("Mouse Move");
    }

    public void Scroll()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
        {
            float fScrollVal = Input.mouseScrollDelta.y;
            EventManager.DoScrollMouse(fScrollVal);
        } else if (Config.currentSceneType == GameScene.Browse2D || Config.currentSceneType == GameScene.MapView || Config.currentSceneType == GameScene.OnlineBrowse2D || Config.currentSceneType == GameScene.OnlineBrowse2D)
        {
            float fScrollVal = Input.mouseScrollDelta.y;
            Vector2 v2CenterPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
            EventManager.DoPinchScreen(fScrollVal, v2CenterPos);
        }
    }

    public void OnPointerUp()
    {
        EventManager.DoMousePointerUp();
    }

    public void OnPointerDown()
    {
        EventManager.DoMousePointerDown();
    }

    public void OnEndDrag()
    {

    }

    public void CalculateScreenSize()
    {
        resolutionX = Screen.width;
        resolutionY = Screen.height;
    }

    public void DragMiniFrame()
    {

        EventManager.DoMiniDrag();
    }

    public void OnMiniPointerDown()
    {
        Debug.Log("Minidrag down");
        EventManager.DoMiniPointerDown();
    }

    public void OnMiniPointerUp()
    {
        EventManager.DoMiniPointerUp();
    }

}
