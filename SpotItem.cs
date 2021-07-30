/***********************************************************
 * --------------SpotItem-----------------------------
 * It is the event handler for spot (dot) in Gallery scene.
 * It has 3 types of the items
 * SpotNewScene, 2d, 3d 
 * 2d: load ArtView2D scene
 * 3d: load ArtView3D scene
 * should be loaded from external data based on gallery id
 * *********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpotItem : MonoBehaviour {
    #region public variables
    public SpotType spotType;   //SpotItem's type 
    public int nSpot2DId;       // if it is spot2D then has 2d image index
    public int nSpot3DId;       // if it is spot3D then has 3d image index
    public int nSpotSceneId;    // if it is spotNewScene then has gallery index
    #endregion
    #region private variables
    Camera mainCam; // gallery scene's camera
    #endregion

    #region MonoBehaviour methods
    // Use this for initialization
    void Start () {
        // find the camera in the current scene
        mainCam = FindObjectOfType(typeof(Camera)) as Camera; 
	}
	
	// Update is called once per frame
	void Update () {
        // make the item turn toward the camera
        if (mainCam != null)
            transform.LookAt(mainCam.transform);
	}
    #endregion

    #region custom methods & functions
    /****************************************************
     * OnClickItem()
     * receive the button click event from inner button 
     * of this game object and handle the event based on 
     * the spot types.
     * *************************************************/
    public void OnClickItem()
    {
        switch (spotType)
        {
            case SpotType.spot2D:
                Config.current2DViewID = nSpot2DId;
                SceneManager.LoadScene(Utils.ARTVIEW_2D);
                break;
            case SpotType.spot3D:
                Config.current3DViewID = nSpot3DId;
                SceneManager.LoadScene(Utils.ARTVIEW_3D);
                break;
            case SpotType.spotNewScene:
                Config.currentGalleryID = nSpotSceneId;
                GalleryManager.Instance().ReloadScene();
                break;
            default:

                break;
        }
    }
    /*********************************************************
     * Init 
     * needed for initial loading and generating spotItem
     * objects from json files.
     * Will be called from GalleryManager InitGallery ()
     * ******************************************************/
    public void Init(SpotType spotType = SpotType.spot2D, int nSpot2d = -1, int nSpot3d = -1, int nSpotNewScene=-1)
    {
        this.spotType = spotType;
        this.nSpot2DId = nSpot2d;
        this.nSpot3DId = nSpot3d;
        this.nSpotSceneId = nSpotNewScene;
    }
    #endregion
}

public enum SpotType
{
    spot2D,
    spot3D,
    spotNewScene
}