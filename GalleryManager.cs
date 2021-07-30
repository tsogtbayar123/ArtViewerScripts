/*******************************************************
 * --------------GalleryMananger----------------------
 * Manager for ArtViewerGallery Scene
 * It controlls the camera rotation dragging
 * and loads the 360 degree textures and spotItem
 * objects from json files
 * ****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class GalleryManager : MonoBehaviour {
    #region public variables
    [Tooltip("texture list for 1st texture of the 360 degree material in this scene")]
    public Texture[] textureInner;
    [Tooltip("texture list for 2nd texture of the 360 degree material in this scene")]
    public Texture[] textureOuter;
    [Tooltip ("camera of the gallery scene")]
    public Camera mainCam;
    [Tooltip ("camera x axis rotation euler angle restriction range")]
    public Vector2 v2RotRestrictionX;
    [Tooltip ("Camera's Y axis rotation Euler angle restriction range")]
    public Vector2 v2RotRestrictionY;
    [Tooltip ("360 degree texture material")]
    public Material[] mat360;
    [Tooltip("MeshRenderer of the 360 photo")]
    public MeshRenderer meshRenderer;
    [Tooltip ("2d spot prefab")]
    public GameObject prefab2DSpot;
    [Tooltip ("3d spot prefab")]
    public GameObject prefab3DSpot;
    [Tooltip("Gallery prefab")]
    public GameObject prefabGallerySpot;
    [Tooltip ("static public variable for Gallery Manager")]
    public static GalleryManager instant;
    [Tooltip("Debug text")]
    public Text txt_dbg;
    // returns the static GalleryManager Object
    public static GalleryManager Instance()
    {
        if (!instant)
            instant = FindObjectOfType(typeof (GalleryManager)) as GalleryManager;
        return instant;
    }
    #endregion
    #region private variables
    Vector3 v3CurrentCameraRotation = Vector3.zero;     //current camera's rotation in Euler angles 
    Vector3 v3NewCameraRotation = Vector3.zero;         //New updated camera's destination rotation
    Vector3 v3DefaultRotation = Vector3.zero;           //default rotation value of the camera
    bool bDragging = false;                             //detects the mouse or touch dragging
    bool bRewindRotate;                                 // smooth rotation end flag
    bool bHold;
    List<GameObject> listSpot2D = new List<GameObject>();
    List<GameObject> listSpot3D = new List<GameObject>();
    List<GameObject> listSpotGallery = new List<GameObject>();
    List<SpotContainer> listSpotInfoContainer = new List<SpotContainer>();

    #endregion

    #region MonoBehaviour methods
    // Use this for initialization
    void Start () {
        LoadJsonData();
        ReloadScene();
    }
	
	// Update is called once per frame
	void Update () {
        RotateCamera();
    }
    #endregion

    #region custom methods
    /************************************************************
     * RotateCamera
     * It is used to check the mouse or touch dragging event 
     * and rotate the camera based on the input value
     * *********************************************************/
    void RotateCamera()
    {
        float dragX = 0;
        float dragY = 0;
#if UNITY_EDITOR || UNITY_STANDALONE
        dragX = Input.GetAxis("Mouse X") * Utils.SAMPLE_WIDTH / Screen.width;
        dragY = Input.GetAxis("Mouse Y") * Utils.SAMPLE_HEIGHT / Screen.height;
#endif
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1) {
            Touch touchZero = Input.GetTouch(0);
            dragX = touchZero.deltaPosition.x * Utils.SAMPLE_WIDTH / Screen.width ;
            dragY = touchZero.deltaPosition.y * Utils.SAMPLE_HEIGHT / Screen.height;
        }
#endif
        if (Mathf.Abs(dragX) > Utils.MAX_TOUCH_DELTA)
        {
            if (dragX < 0)
                dragX = -Utils.MAX_TOUCH_DELTA;
            else
                dragX = Utils.MAX_TOUCH_DELTA;

        }

        if (Mathf.Abs(dragY) > Utils.MAX_TOUCH_DELTA)
        {
            if (dragY < 0)
                dragY = -Utils.MAX_TOUCH_DELTA;
            else
                dragY = Utils.MAX_TOUCH_DELTA;

        }

        Debug.Log("dragX: " + dragX);
        
        // if the input values are over the threshold
        if ((Mathf.Abs(dragX) + Mathf.Abs(dragY)) > Utils.TOLERATE_CAM_ROT && Input.GetMouseButton(0))
        {
            float rotX = dragX * Utils.CAMERA_ROTATION;
            float rotY = dragY * Utils.CAMERA_ROTATION;
            v3NewCameraRotation = new Vector3(rotY, -rotX, 0);
            bDragging = true;
            bRewindRotate = false;
            bHold = false;
            txt_dbg.text = "X:" + dragX + " Y:" + dragY;
        }
        if (Vector3.Distance(v3CurrentCameraRotation, v3NewCameraRotation) > Utils.TOLERATE_ROTATION_DELTA && !bRewindRotate)
        {
            v3CurrentCameraRotation = Vector3.Lerp(v3CurrentCameraRotation, v3NewCameraRotation, Time.deltaTime / Utils.EASE_DURATION);
            mainCam.transform.Rotate(v3CurrentCameraRotation);
            if (Vector3.Distance(v3CurrentCameraRotation, v3NewCameraRotation) <= Utils.TOLERATE_ROTATION_DELTA && !bDragging)
                bRewindRotate = true;
        }
        else if (Vector3.Distance(v3CurrentCameraRotation, v3NewCameraRotation) > Utils.TOLERATE_ROTATION_DELTA && bRewindRotate && !Input.GetMouseButton(0))
        {
            v3CurrentCameraRotation = Vector3.Lerp(v3CurrentCameraRotation, v3DefaultRotation, Time.deltaTime / Utils.EASE_DURATION);
            mainCam.transform.Rotate(v3CurrentCameraRotation);
        }

        Vector3 modelEulerAngles = mainCam.transform.eulerAngles;
        // remove the Z axis rotation into zero
        
        mainCam.transform.eulerAngles = new Vector3(Utils.ClampAngle(modelEulerAngles.x, v2RotRestrictionX.x, v2RotRestrictionX.y), Utils.ClampAngle(modelEulerAngles.y, v2RotRestrictionY.x, v2RotRestrictionY.y), 0);
    }

    /*************************************************
     *ReloadScene()
     * It reads the info from json files and updates
     * the textures of the 360 photo and reinitiates
     * the spotItems
     * **********************************************/
    public void ReloadScene()
    {
        int nCurrentGallery = Config.currentGalleryID;
        Debug.Log("nCurrentGallery: " + nCurrentGallery);
        meshRenderer.material = mat360[nCurrentGallery];
        for (int i = 0; i < listSpot2D.Count; i++)
        {
            Destroy(listSpot2D[i]);
        }

        for (int i = 0; i < listSpot3D.Count; i++)
        {
            Destroy(listSpot3D[i]);
        }

        for (int i = 0; i < listSpotGallery.Count; i++)
        {
            Destroy(listSpotGallery[i]);
        }
        listSpot2D.Clear();
        listSpot3D.Clear();
        listSpotGallery.Clear();

        for (int i = 0; i < listSpotInfoContainer[nCurrentGallery].listSpot2DInfo.Count; i++)
        {
            GameObject objSpot2D = Instantiate(prefab2DSpot) as GameObject;
            string[] arrPosition = listSpotInfoContainer[nCurrentGallery].listSpot2DInfo[i].strPosition.Split(',');
            float fXpos = float.Parse(arrPosition[0]);
            float fYpos = float.Parse(arrPosition[1]);
            float fZpos = float.Parse(arrPosition[2]);

            Vector3 v3Position = new Vector3(fXpos, fYpos, fZpos);
            objSpot2D.transform.position = v3Position;
            int nId = listSpotInfoContainer[nCurrentGallery].listSpot2DInfo[i].nId;
            objSpot2D.GetComponent<SpotItem>().Init(SpotType.spot2D, nId, -1, -1);
            listSpot2D.Add(objSpot2D);
        }

        for (int i = 0; i < listSpotInfoContainer[nCurrentGallery].listSpot3DInfo.Count; i++)
        {
            GameObject objSpot3D = Instantiate(prefab3DSpot) as GameObject;
            string[] arrPosition = listSpotInfoContainer[nCurrentGallery].listSpot3DInfo[i].strPosition.Split(',');
            float fXpos = float.Parse(arrPosition[0]);
            float fYpos = float.Parse(arrPosition[1]);
            float fZpos = float.Parse(arrPosition[2]);

            Vector3 v3Position = new Vector3(fXpos, fYpos, fZpos);
            objSpot3D.transform.position = v3Position;
            int nId = listSpotInfoContainer[nCurrentGallery].listSpot3DInfo[i].nId;
            objSpot3D.GetComponent<SpotItem>().Init(SpotType.spot3D, -1, nId, -1);
            listSpot3D.Add(objSpot3D);
        }

        for (int i = 0; i < listSpotInfoContainer[nCurrentGallery].listSpotGalleryInfo.Count; i++)
        {
            GameObject objSpotGallery = Instantiate(prefabGallerySpot) as GameObject;
            string[] arrPosition = listSpotInfoContainer[nCurrentGallery].listSpotGalleryInfo[i].strPosition.Split(',');
            float fXpos = float.Parse(arrPosition[0]);
            float fYpos = float.Parse(arrPosition[1]);
            float fZpos = float.Parse(arrPosition[2]);

            Vector3 v3Position = new Vector3(fXpos, fYpos, fZpos);
            objSpotGallery.transform.position = v3Position;
            int nId = listSpotInfoContainer[nCurrentGallery].listSpotGalleryInfo[i].nId;
            objSpotGallery.GetComponent<SpotItem>().Init(SpotType.spotNewScene, -1, -1, nId);
            listSpotGallery.Add(objSpotGallery);
        }
    }

    public void LoadJsonData()
    {
        string jsonData = Utils.LoadTextFromFile(Utils.JSON_SPOT_PATH);
        var dict = JSON.Parse(jsonData);
        JSONNode objNode = dict;

        int nCount = objNode.Count;
        for (int i = 0; i < nCount; i++)
        {
            SpotContainer spotContainer = new SpotContainer();
            spotContainer.nGalleryId = i;
            JSONNode objSpot2dNode = objNode[i]["spot2DArray"];
            if (objSpot2dNode.Count > 0)
            {
                for (int j = 0; j < objSpot2dNode.Count; j++)
                {
                    SpotData spotData = new SpotData();
                    spotData.nId = objSpot2dNode[j]["spot2DId"];
                    spotData.strPosition = objSpot2dNode[j]["position"];
                    Debug.Log("Position: " + objSpot2dNode[j]["position"]);
                    spotContainer.listSpot2DInfo.Add(spotData);
                }
            }
            JSONNode objSpot3dNode = objNode[i]["spot3DArray"];
            if (objSpot3dNode.Count > 0)
            {
                for (int j = 0; j < objSpot3dNode.Count; j++)
                {
                    SpotData spotData = new SpotData();
                    spotData.nId = objSpot3dNode[j]["spot3DId"];
                    spotData.strPosition = objSpot3dNode[j]["position"];
                    spotContainer.listSpot3DInfo.Add(spotData);
                }
            }
            JSONNode objSpotGallery = objNode[i]["spotSceneArray"];
            if (objSpotGallery.Count > 0)
            {
                for (int j = 0; j < objSpotGallery.Count; j++)
                {
                    SpotData spotData = new SpotData();
                    spotData.nId = objSpotGallery[j]["spotSceneId"];
                    spotData.strPosition = objSpotGallery[j]["position"];
                    spotContainer.listSpotGalleryInfo.Add(spotData);
                }
            }
            Debug.Log(spotContainer.ToString());
            listSpotInfoContainer.Add(spotContainer);
        }
    }

#endregion

}

class SpotContainer
{
    public int nGalleryId;
    public List<SpotData> listSpot2DInfo = new List<SpotData>();
    public List<SpotData> listSpot3DInfo = new List<SpotData>();
    public List<SpotData> listSpotGalleryInfo = new List<SpotData>();
}

class SpotData
{
    public string strPosition;
    public int nId;
}
