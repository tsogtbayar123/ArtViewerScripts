/***************************************************
 * ---------- Config ------------------------------
 * It is the config file that contains the global
 * variables for general app running.
 
 * ************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config  {

    public static GameScene currentSceneType = GameScene.Museum;    // game scene kind flag	
    public static int current2DViewID = 0;                          // current 2d image index
    public static int current3DViewID = 0;                          // current 3d image index
    public static int currentGalleryID = 0;                         // current gallery id
    public static int currentTileViewId = 0;
    
}

/*************************************************
 * Game Scene type
 * Needed for determine the separate logic such as 
 * touch control for each scene
 * ***********************************************/
public enum GameScene
{
    Museum,
    Browse3D,
    Browse2D,
    MapView,
    OnlineBrowse2D
}


