/******************************************************
 * Utils class which contains the const values
 * and static functions
 *************************************************** */
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Net;


public class Utils {
    #region public static values
    // initial 3D model's scale value
    public static float INITIAL_SCALE_VAL = 1f; 
    // Maximum zoom value for 3D model
    public static float MAX_SCALE_VAL = 5f;
    // Minimum zoom value for 3D model
    public static float MIN_SCALE_VAL = 0.1f;
    // maximum scale (zoom) value of the 2d image view
    public static float MAX_SCALE_VAL_2D = 10f;
    // minimum scale value of the 2d image view ( it will be chaged based on the image's width/height ratio so check out the ImageViewManager class)
    public static float MIN_SCALE_VAL_2D = 0.5f;
    // threshold of the changes from Big image view to deepth titled image view
    public static float TILED_SCALE_VAL_2D = 3f;
    // Action duration means the drag delay period
    public static float ACTION_DURATION = 1f;
    //rotation delay value and real target value's delta threshold
    public static float TOLERATE_ROTATION_DELTA = 0.01f;
    //zoom delay interval threshold value between real target and current value
    public static float TOLERATE_ZOOM_DELTA = 0.01f;
    // transition delay interval threshold between real target and current value
    public static float TOLERATE_TRANSLATION = 0.01f;
    // 3d model showing up animation time duration, can not control the model in this period
    public static float MODEL_SHOWUP_DURATION = 1.5f;
    // the duration of resetting model's position, scale and rotation
    public static float MODEL_RESET_DURATION = 1f;
    // directional light's intensity value
    public static float INTENSITY_DIRECTIONAL_LIGHT = .5f;
    //rotation of directional light
    public static float DEFAULT_X_ROTATION_DIRECTIONAL_LIGHT = 3.5f;
    // environmental light's intensity value
    public static float INTENSITY_ENVIRONMENTAL_LIGHT = 0.2f;
    // reflection light's intensity value
    public static float INTENSITY_REFLECTION_LIGHT = 0.1f;
    // smooth factor value for zoom and rotation for 3d model
    public static float EASE_DURATION = 0.025f;
    // smooth speed coeficent for zoom
    public static float ZOOM_SPEED_COEF = 5.0f;
    // rotation speed of the 3d model
    public static float ROTATION_SPEED = 10f;

    public static float CAMERA_ROTATION = 1f;
    // pan or translate speed
    public static float PAN_SPEED = 1f;
    // pan threshold
    public static float THRESHOLD_PAN = 0.06f;
    // needed for translation of 3d models in screen and it is default height of the movable distance in Y
    public static float DEFAULT_HEIGHT = 30f;
    // same as prev but X axis
    public static float DEFAULT_WIDTH = 40f;
    // default screen ratio 4:3 it is used to calculate current screen height and width
    public static float DEFAULT_RATIO = 0.75f;
    // JSON file name which stores 3d model's info lists in Resources folder
    public static string JSON_3DMODELS_PATH = "Model3D";
    // JSON file name which stores 2d model's info list in Resource folder
    public static string JSON_2DMODELS_PATH = "Model2D";
    // JSON file name which stores spot data which is in Resources directory and it used for gallery scene
    public static string JSON_SPOT_PATH = "Spot2D";
    //JSON file name which stores tile map data in Resources directory
    public static string JSON_TILE_PATH = "Tile2D";
    //test txt file name for testing tile image urls
    public static string TXT_TILE_PATH = "TestTileMapData";


    // Toleration threshold value for 2d canvas image movement in ArtViewer2D scene
    public static float CANVAS_TOLERATE_MOVEMENT_DELTA = 0.5f;
    //Canvas Height in the screen as it is set as height based, used to calculate image panel ratio and size
    public static float fCanvasHeight = 600f;
    //ArtViewer2D  Rotation angle value
    public static float ROTATION_STEP_ANGLE = 90f;
    //image 2d view scene name
    public static string ARTVIEW_2D = "ArtViewer2D";
    // 3d model view scene name
    public static string ARTVIEW_3D = "ArtViewer3D";
    // gallery scene name
    public static string ARTVIEW_GALLERY = "ArtViewGallery";
    // gallery touch or mouse drag sense adjuster
    public static float SAMPLE_WIDTH = 800f;
    public static float SAMPLE_HEIGHT = 600f;
    public static float MAX_TOUCH_DELTA = 3f;
    public static float TOLERATE_CAM_ROT = 0.3f;
    //sensitivity factor for touch, pinch, zoom of 3D models
    public static float SENSITIVITY = 2f;
    //Check Anchor position is changed or not
    public static float ANCHOR_DELTA = 3f;
    //test path of the gltf files
    public static string GLTF_FOLDER = "/SavedGLTF/";
    //AWS S3 bucket Name
    public static string S3BucketName = "tsogi-gltf";
    //AWS S3 tilemap bucket Name
    public static string S3TileMapBucketName = "test-tile";
    //AWS s3 tilemap objNames 
    public static string[] ArrayBucketHolders = { "img/", "gray_painting/", "tall_building/" };
    //AWS s3 Identity pool ID
    public static string IdentityPoolId = "us-east-1:7f6c68fe-337a-400b-9355-9e2ffd26b9e8";
    //AWS Cognito Identity Region
    public static string CognitoIdentityRegion = "us-east-1";
    //AWS S3 Region
    public static string S3Region = "us-east-1";
    //TEST Mode filePath
    public static string TEST_PATH = "F:/work";
    //GLTF file ext
    public static string GLTF_EXT = ".glb";
    //Tag local scale value
    public static float TAG_SCALE = 0.25f;

    //Current 3D model's scale
    public static float CURRENT_SCALE3D = 1f;

    //Current 3D Tag ID 
    public static int TAG_3D_ID = -1;

    //current 2D Tag ID
    public static int TAG_2D_ID = -1;

    //online Object loader url
    public static string URL_METMUSEUM_OBJS = "https://collectionapi.metmuseum.org/public/collection/v1/search";
    //online Object info url
    public static string URL_METMUSEUM_OBJ = "https://collectionapi.metmuseum.org/public/collection/v1/objects/";
    //online Object ID lists
    public static List<int> LIST_MUSEUM_OBJ_ID = new List<int>();

    //current museum object index in the list
    public static int nObjId = 0;

    //Experimental calculation value - initial 1:1 ratio height 
    public static float INITIAL_HEIGHT = 30f;
    public static float INITIAL_HEIGHT_2 = 38f;

    //Experimental calculation value - initial 1:N ratio increase height
    public static float RATIO_STEP_VALUE = 60f;
    //tile fade time
    public static float TILE_FADE_DURATION = 2f;


    #endregion
    #region Utils handy functions

    //utility function needed to load text from the resource file
    public static string LoadTextFromFile(string filePath)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        return targetFile.text;
    }

    /// <summary>
    /// Clamp a Vector3.  This is old as well.  Todo: Remove.
    /// </summary>
    /// <param name="vectIn"></param>
    /// <param name="vectMin"></param>
    /// <param name="vectMax"></param>
    /// <returns></returns>
    public static Vector3 ClampVector(Vector3 vectIn, Vector3 vectMin, Vector3 vectMax)
    {
        Vector3 outVect = new Vector3();
        outVect.x = Mathf.Clamp(vectIn.x, vectMin.x, vectMax.x);
        outVect.y = Mathf.Clamp(vectIn.y, vectMin.y, vectMax.y);
        outVect.z = Mathf.Clamp(vectIn.z, vectMin.z, vectMax.z);
        return outVect;
    }

    /// <summary>
    /// Clamps a rotation vector.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector3 ClampAngle(Vector3 angle,float min,float max)
    {
        Vector3 outVect=new Vector3();
        outVect.x = ClampAngle(angle.x, min, max);
        outVect.y = ClampAngle(angle.y, min, max);
        outVect.z = ClampAngle(angle.z, min, max);
        return outVect;
    }

    /// <summary>
    /// Find the distance between two floats.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="x2"></param>
    /// <returns></returns>
    public static float FloatDistance(float x1, float x2)
    {
        float dx = Mathf.Abs(x1 - x2);
        return dx;
    }

    // Utility function used for clamping Angle value in 3d model view scene as well as gallery scene
    public static float ClampAngle(float angle, float min, float max)
    {
        if (min < 0 && max > 0 && (angle > max || angle < min))
        {
            angle -= 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }
        else if (min > 0 && (angle > max || angle < min))
        {
            angle += 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }



        if (angle < min) return min;
        else if (angle > max) return max;
        else return angle;
    }

    //load the GLTF files 
    public static GameObject LoadLocalGLTF(int nModelIndex)
    {
        //UnityEngine.Debug.Log("Application Path: " + Application.dataPath);
        //string path = Application.dataPath + "/Raw/"+ GLTF_FOLDER + nModelIndex + ".glb"; //"F:/work/" + GLTF_FOLDER + nModelIndex + ".glb"; // Application.dataPath + GLTF_FOLDER + nModelIndex + ".glb";


        string path = Path.Combine(Application.persistentDataPath, nModelIndex + Utils.GLTF_EXT);  // use it in real ipad build
        //string path = TEST_PATH + GLTF_FOLDER + nModelIndex + GLTF_EXT; // use it in test mode with Unity Editor
        if (!File.Exists(path))
        {
            return null;
        }
        var ext = Path.GetExtension(path).ToLower();
        if (ext != GLTF_EXT) return null;
        var context = new UniGLTF.ImporterContext();
        var file = File.ReadAllBytes(path);
        context.ParseGlb(file);
        context.Load();
        context.ShowMeshes();
        context.EnableUpdateWhenOffscreen();
        context.ShowMeshes();
        return context.Root;
        
    }

    public static void AddObjId2List(int nObjId)
    {
        LIST_MUSEUM_OBJ_ID.Add(nObjId);
    }

    public static void ClearObjIdList()
    {
        LIST_MUSEUM_OBJ_ID.Clear();
    }

    public static string CreateRequest(string postData, string apiURL)
    {
        string api_url = apiURL;
        var request = (HttpWebRequest)WebRequest.Create(api_url);
        var data = Encoding.UTF8.GetBytes(postData);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        var response = (HttpWebResponse)request.GetResponse();
        var responseString = "";

        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        return responseString;

    }

    public static bool IsMuseumObjectsLoaded()
    {
        if (LIST_MUSEUM_OBJ_ID.Count > 0) return true;
        else return false;
    }

    #endregion

    //ease function 
    public static float EaseInOut(float initial, float final, float time, float duration)
    {
        float change = final - initial;
        time /= duration / 2;
        if (time < 1f) return change / 2 * time * time + initial;
        time--;
        return -change / 2 * (time * (time - 2) - 1) + initial;
    }

    //convert binary byte array into texture 2D
    public static Texture2D Bytes2Texture2D(byte[] imageBytes)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        return tex;
    }
}
