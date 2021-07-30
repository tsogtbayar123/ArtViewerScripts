/*****************************************************
 * EventManager
 * It is used to manage custom events on the project.
 * Using delegate methods to event management.
 * ***************************************************/
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EventManager : MonoBehaviour {

    #region mouse & touch, pinch, scroll events
    /***************
     *DragMouse event 
     * 
     * **************/
    public delegate void DragMouse(float rotX, float rotY);
    public static event DragMouse OnDragMouse;

    public static void DoDragMouse(float rotX, float rotY)
    {
        if (OnDragMouse != null)
            OnDragMouse(rotX, rotY);
    }

    /***************
    *Scroll Mouse event 
    * **************/
    public delegate void ScrollMouse(float scrollVal);
    public static event ScrollMouse OnScrollMouse;

    public static void DoScrollMouse (float scrollVal)
    {
        if (OnScrollMouse != null)
            OnScrollMouse(scrollVal);
    }

   /***************
   *Scroll Mouse event 
   * **************/
    public delegate void PinchScreen(float scrollVal, Vector2 v2CenterPos);
    public static event PinchScreen OnPinchScreen;

    public static void DoPinchScreen(float scrollVal, Vector2 v2CenterPos)
    {
        if (OnPinchScreen != null)
            OnPinchScreen(scrollVal, v2CenterPos);
    }

    /***************
    *Pointer up event 
    * **************/
    public delegate void MousePointerUp();
    public static event MousePointerUp OnPointerUp;

    public static void DoMousePointerUp()
    {
        if (OnPointerUp != null)
            OnPointerUp();
    }

    /***************
    *Pointer down event 
    * **************/
    public delegate void MousePointerDown();
    public static event MousePointerUp OnPointerDown;

    public static void DoMousePointerDown()
    {
        if (OnPointerDown != null)
            OnPointerDown();
    }

   /***************
   *MOVE event 
   * **************/
    public delegate void Move(float fMoveX, float fMoveY);
    public static event Move OnMove;

    public static void DoMove(float fMoveX, float fMoveY)
    {
        if (OnMove != null)
            OnMove(fMoveX, fMoveY);
    }
    #endregion

    #region 3D model handling events
    /**************************************************
     * event listener delegate for 
     * changing 3d models when the users press the prev 
     * or next arrow in 3d model browser.
     * if the model index is 0 then we need to hide 
     * the prev button, if the bEnd is true then we
     * need to hide the next button 
     * Currently it is needed for this implementation, 
     * but expect more usage so far
     * ************************************************/
    public delegate void ModelChanged3D(int nIndex, bool bEnd=false);
    public static event ModelChanged3D OnModelChanged3D;

    public static void DoModelChanged3D(int nIndex, bool bEnd=false)
    {
        if (OnModelChanged3D != null)
        {
            OnModelChanged3D(nIndex, bEnd);
        }
    }

    /**************************************************
     * PressPrevModel3dButton
     * Informs the ModelViewManager to catch the event
     * and run the InitModel() function
     * ************************************************/
    public delegate void PressPrevModel3DButton();
    public static event PressPrevModel3DButton OnPressPrevModel3DButton;

    public static void DoPressPrevModel3DButton()
    {
        if (OnPressPrevModel3DButton != null)
        {
            OnPressPrevModel3DButton();
        }
    }

    /***************************************************************
     * PressNextModel3DButton
     * informs ModelViewManager to catch the button press event and
     * run the InitModel() function
     * ************************************************************/
    public delegate void PressNextModel3DButton();
    public static event PressNextModel3DButton OnPressNextModel3DButton;

    public static void DoPressNextModel3DButton()
    {
        if (OnPressNextModel3DButton != null)
        {
            OnPressNextModel3DButton();
        }
    }
    #endregion
    #region 2D image handling events
    /**********************************************************************
     * Similar event listener delegater with ModleChanged3D but for 2d
     * *******************************************************************/
    public delegate void ModelChanged2D(int nIndex, bool bEnd = false);
    public static event ModelChanged2D OnModelChanged2D;

    public static void DoModelChanged2D(int nIndex, bool bEnd = false)
    {
        if (OnModelChanged2D != null)
        {
            OnModelChanged2D(nIndex, bEnd);
        }
    }

    /*******************************************************************
     * MiniPointerDown
     * It is the pointer down event handler for mini view 
     * in ArtViewer2D scene
     * ****************************************************************/
    public delegate void MiniPointerDown();
    public static event MiniPointerDown OnMiniPointerDown;

    public static void DoMiniPointerDown ()
    {
        if (OnMiniPointerDown != null)
        {
            OnMiniPointerDown();
        }
    }

    
    public void SetPropActive()
    {
       // MainManager.Instance().BroadcastMessage();
    }


    /*******************************************************************
     * MiniPointerUp
     * It is the pointer down event handler for mini view 
     * in ArtViewer2D scene
     * ****************************************************************/
    public delegate void MiniPointerUp();
    public static event MiniPointerUp OnMiniPointerUp;

    public static void DoMiniPointerUp()
    {
        if (OnMiniPointerUp != null)
        {
            OnMiniPointerUp();
        }
    }

    /*******************************************************************
     * MiniPointerDown
     * It is the pointer down event handler for mini view 
     * in ArtViewer2D scene
     * ****************************************************************/
    public delegate void MiniDrag();
    public static event MiniDrag OnMiniDrag;

    public static void DoMiniDrag()
    {
        if (OnMiniDrag != null)
        {
            OnMiniDrag();
        }
    }

    /*****************************************************************
     * RotateImageRight
     * It is used for 2d image's rotation in ArtViewer2D
     * **************************************************************/
    public delegate void RotateImageRight();
    public static event RotateImageRight OnRotateImageRight;

    public static void DoRotateImageRight()
    {
        if (OnRotateImageRight != null)
        {
            OnRotateImageRight();
        }
    }

    /*****************************************************************
     * RotateImageLeft
     * It is used for 2d image's rotation in ArtViewer2D
     * **************************************************************/
    public delegate void RotateImageLeft();
    public static event RotateImageLeft OnRotateImageLeft;

    public static void DoRotateImageLeft()
    {
        if (OnRotateImageLeft != null)
        {
            OnRotateImageLeft();
        }
    }

    /*****************************************************************
     * PressPrevImage
     * It is used for browsing prev image in ArtViewer2D
     * **************************************************************/
    public delegate void PressPrevImage();
    public static event PressPrevImage OnPressPrevImage;

    public static void DoPressPrevImage()
    {
        if (OnPressPrevImage != null)
        {
            Debug.Log("i'm pressing");
            OnPressPrevImage();
        }
    }

    /*****************************************************************
    * PressNextImage
    * It is used for browsing prev image in ArtViewer2D
    * **************************************************************/
    public delegate void PressNextImage();
    public static event PressNextImage OnPressNextImage;

    public static void DoPressNextImage()
    {
        if (OnPressNextImage != null)
        {
            OnPressNextImage();
        }
    }
    #endregion
    #region AWS file download Events
    /************************************************************
     * DownLoadSuccess
     * It returns success result of the download the file from url 
     * and save to local.
     * Called by S3FileDownloader.cs when process success.
     * used in ModelViewManager.cs
     * *********************************************************/
    public delegate void DownLoadSuccess();
    public static event DownLoadSuccess OnDownloadSuccess;

    public static void DoDownloadSuccess()
    {
        if (OnDownloadSuccess != null)
        {
            OnDownloadSuccess();
        }
    }

    /**********************************************************
     * DownLoadFail
     * It returns the download file failed result from AWS
     * Called by S3FileDownloader.cs when process fail.
     * used in ModelViewManager.cs
     * *******************************************************/
    public delegate void DownLoadFail();
    public static event DownLoadFail OnDownloadFail;

    public static void DoDownloadFail()
    {
        if (OnDownloadFail != null)
            OnDownloadFail();
    }
    #endregion
}
