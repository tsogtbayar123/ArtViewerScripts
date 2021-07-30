/**********************************************************************
 * ---------------ModelViewManager-----------------------------------
 * The main core class to manipulate the model drag, zoom, translation
 * Attached in Manager object
 * ********************************************************************/
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using AWSSDK.Examples;

public class ModelViewManager : MonoBehaviour {
    #region public variables
    [Tooltip("3d model lists in this app")]
    public GameObject[] prefModelList;      // 3d model prefab lists, needed to choose from the required models, kind of 3d model pool in the app
    [Tooltip("Model is spawned in this position")]
    public Transform transModelSpawner;     // model spawner position
    [Tooltip("The light source of the scene")]
    public Light sceneLight;                // scene's directional light
    [Tooltip("The debug Text")]
    public Text dbgTxt;
    [Tooltip("Current Model Index")]
    public int nCurrentModelIndex;
    [Tooltip("Downloading panel")]
    public GameObject pnl_downloading;       // downloading processing bar which is active in file download
    [SerializeField]
    [Tooltip("Animation curve")]
    private AnimationCurve zoomCurve;
    [Tooltip("S3 bucket Handler")]
    public S3Handler s3Handler;
    [Tooltip("Tag object prefab")]
    public GameObject prefTagObject;
    [Tooltip("Tag UI object")]
    public GameObject pnl_tagInfo;
    [Tooltip("tag UI text field")]
    public Text txt_tagInfo;

    public Transform transContainer;

    public static ModelViewManager instance;
    public static ModelViewManager Instance()
    {
        if (instance ==  null)
        {
            instance = FindObjectOfType(typeof(ModelViewManager)) as ModelViewManager;
        }
        return instance;
    }
    
    #endregion

    #region private variables
    GameObject objTargetModel;                             // current model wanted to view in detail, instantiated from prefModelList in transModelSpawner position
    Vector3 v3OriginalRotation = Vector3.zero;             // original rotation needed for model's resetting position
    Vector3 v3OriginalPosition = new Vector3(0,0,50f);     // original position needed fro model's resetting position from transitioning
    Vector3 v3CurrentRotation;                              // current model's rotation value

    Quaternion currentRot;
    Quaternion originalRot = Quaternion.Euler(0,0,0);
    float rotationSpeed = 4;                // rotation value scaler
    float fCurrentZoom = 1f;                // current zoom value changed in the range of min and max zoom
    float fNewZoom = 1f;                    // new zoom value which is changed directly according to the event and the target value from Lerp of current zoom
    float fZoomScaler = 0.1f;               // zoom scaler value as the zoom event input is too big (-3, -2, -1, 1, 2, 3) so need to scale down for smooth
    bool bDragging = false;                 // store the dragging status maybe used future???
    bool bRewindRotate = false;             
    bool bScrolling = false;
    bool bHold = false;
    float fScrollingTimer = 0;
    float fDraggingTimer = 0;
    Vector3 v3CurrentLocalRotation = Vector3.zero;
    Vector3 v3NewLocalRotation = Vector3.zero;
    Vector3 v3DefaultRotation = Vector3.zero;

    Vector3 v3CurrentPosition = new Vector3(0, 0, 50f);
    Vector3 v3NewPosition = new Vector3(0, 0, 50f);
    
    float fSmoothFactor = 10f;

    float fShowUpCounter = 0f;             // the time counter of the model show up when the model instantiated : duration from Utils.MODEL_SHOWUP_DURATION
    bool bShowUpFinished = false;          // flag of model show up, when flagged false, lock the user drag, zoom, translation ...

    float fResetTimeCounter = 0f;           //the time counter for model reset process
    bool bResetModel = false;               // flag of model reset processing in reset duration for position, scale and rotation into initial state

    float fTranslateLimitY = Utils.DEFAULT_HEIGHT;
    float fTranslateLimitX = Utils.DEFAULT_WIDTH;

    Vector2 v2RotRestrictionX = Vector2.zero;       //x axis rotation restriction angle range 
    Vector2 v2RotRestrictionY = Vector2.zero;       //y axis rotation restriction angle range
    Transform transTagDetail;
    bool bTagInfoOn = false;
    List<GameObject> listTagButtons = new List<GameObject> ();
    bool bTagOn = true;
    #endregion

    #region MonoBehavior methods
    // Use this for initialization
    void Start () {
        //S3FileDownloader.Instance().InitS3FileDownloader();
        s3Handler.InitHandler();
        
        InitModel();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (bShowUpFinished && !bResetModel)
        {
            RotateObject();
            ZoomObject();
            TranslateObject();
            UpdateTagDetailPos();
        }
    }

    void OnMouseDrag()
    {
        // get input from mouse axis and convert them into rotation value as radian.
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
        float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;

        objTargetModel.transform.RotateAround(transModelSpawner.position, Vector3.up, rotY);
    }


    /*****************************************
     * Add event listeners from EventManager
     * with delegates
     * ***************************************/
    private void OnEnable()
    {
        EventManager.OnDragMouse    += OnDrag;
        EventManager.OnScrollMouse  += OnScroll;
        EventManager.OnPointerUp += OnPointerUp;
        EventManager.OnMove += OnMove;
        EventManager.OnPointerDown += OnPointerDown;
        EventManager.OnPressNextModel3DButton += OnPressNextModel;
        EventManager.OnPressPrevModel3DButton += OnPressPrevModel;
        EventManager.OnDownloadSuccess += OnDownloadSuccess;
        EventManager.OnDownloadFail += OnDownloadFail;
    }

    /******************************************
     * Remove event listeners from EventManager
     * with delegates
     * ****************************************/
    private void OnDisable()
    {
        EventManager.OnDragMouse    -= OnDrag;
        EventManager.OnScrollMouse  -= OnScroll;
        EventManager.OnPointerUp -= OnPointerUp;
        EventManager.OnMove -= OnMove;
        EventManager.OnPointerDown -= OnPointerDown;
        EventManager.OnPressNextModel3DButton -= OnPressNextModel;
        EventManager.OnPressPrevModel3DButton -= OnPressPrevModel;
        EventManager.OnDownloadSuccess -= OnDownloadSuccess;
        EventManager.OnDownloadFail -= OnDownloadFail;
    }

    #endregion

    #region Custom methods & functions
    /****************************************************
     * Initialize the 3d model from the model prefab pool
     * Adjust the initial scale value
     * being called by Start()
     * **************************************************/
    void InitModel (int nModelIdx=0)
    {
        pnl_downloading.SetActive(false);
        Debug.Log("app path: " + Application.dataPath);
        //Set the scene type flag as it determines the touch controll behavior based on scene kind
        Config.currentSceneType = GameScene.Browse3D;
        //Destroy prev model
        if (objTargetModel != null)
            Destroy(objTargetModel);
        bShowUpFinished = false;
        // instantiate required model
        float currentRatio = 1f * Screen.height / Screen.width;

        nModelIdx = Config.current3DViewID;

        nCurrentModelIndex = nModelIdx;

        if (ModelInfoPool.listModelInfo.Count == 0)
        {
            ModelInfoPool.LoadModel3DInfo();
            
        }

        fTranslateLimitX = Utils.DEFAULT_WIDTH * Utils.DEFAULT_RATIO / currentRatio;
        //objTargetModel = Instantiate(prefModelList[nModelIdx], v3OriginalPosition, transModelSpawner.rotation) as GameObject;
        objTargetModel = Utils.LoadLocalGLTF(nModelIdx);
        //s3Handler.GetBucketList();
        if (objTargetModel == null)
        {
            pnl_downloading.SetActive(true);
            //StartCoroutine(SaveFileFromURL(nCurrentModelIndex)); // used it with public download url
            s3Handler.GetObject(); // use s3 bucket model download module
            return;
        } else
        {
            objTargetModel.transform.SetParent(transContainer);
            StartCoroutine(AddTagObjects());
        }
        fCurrentZoom = Utils.INITIAL_SCALE_VAL; // set the initial scale = 1
        bool bEnd = false;
//        if (ModelInfoPool.listModelInfo.Count == 0)
//        {
//            ModelInfoPool.LoadModel3DInfo();
//        }
        
        bool bSwitchBtn = false;
        if (ModelInfoPool.GetModelInfo(nModelIdx).refId > -1) bSwitchBtn = true;
        UIManager.Instance().SetSwitchButtonVisibility(bSwitchBtn);

        if (nModelIdx == (prefModelList.Length - 1))
            bEnd = true;
        v2RotRestrictionX = ModelInfoPool.GetModelInfo(nModelIdx).v2RotationRestrictionX;
        v2RotRestrictionY = ModelInfoPool.GetModelInfo(nModelIdx).v2RotationRestrictionY;
        Vector3 vec = new Vector3(ModelInfoPool.GetModelInfo(nModelIdx).lightRotation, sceneLight.transform.rotation.y, sceneLight.transform.rotation.z);
        sceneLight.transform.rotation = Quaternion.Euler(vec);
        EventManager.DoModelChanged3D(nModelIdx, bEnd);
        //bShowUpFinished = false;
        v3CurrentLocalRotation = Vector3.zero;
        v3NewLocalRotation = Vector3.zero;
        v3DefaultRotation = Vector3.zero;

        v3CurrentPosition = new Vector3(0, 0, 50f);
        v3NewPosition = new Vector3(0, 0, 50f);                                                  
        transModelSpawner.position = new Vector3(0, 0, 50f);
        fCurrentZoom = 1f;
        fNewZoom = 1f;
        fShowUpCounter = 0;

        StartCoroutine(ShowUpModel());
        
    }

    IEnumerator AddTagObjects()
    {
        int nTagCount = ModelInfoPool.listModelInfo[nCurrentModelIndex].listTagInfo.Count;
        DestroyTagButtons();
        if (nTagCount > 0)
        {
            for (int i = 0; i < nTagCount; i++)
            {
                GameObject objTag = Instantiate(prefTagObject) as GameObject;
                objTag.transform.SetParent(objTargetModel.transform);
                objTag.transform.localPosition = ModelInfoPool.listModelInfo[nCurrentModelIndex].listTagInfo[i].tagPos;
                objTag.transform.localScale = Utils.TAG_SCALE * Vector3.one;
                objTag.GetComponent<TagItem>().InitTag(i);
                listTagButtons.Add(objTag);
                if (!bTagOn) objTag.SetActive(false);
                yield return null;
            }
        }
        yield return null;
      
    }

    void DestroyTagButtons()
    {
        for (int i = 0; i < listTagButtons.Count; i++)
        {
            Destroy(listTagButtons[i]);
        }
        listTagButtons.Clear();
    }

    public void ShowTagInfo(int nTagId)
    {
        
        txt_tagInfo.text = ModelInfoPool.listModelInfo[nCurrentModelIndex].listTagInfo[nTagId].tagInfo;
        if (Utils.TAG_3D_ID != nTagId || bTagInfoOn == false)
        {
            if (pnl_tagInfo.activeSelf == false)
            {
                pnl_tagInfo.SetActive(true);
                pnl_tagInfo.GetComponent<Animator>().Play("tagDetailShow");
 
            } else
            {
                StartCoroutine(RefreshTagPanel());
            }
            bTagInfoOn = true;
            Utils.TAG_3D_ID = nTagId;
        } else if (Utils.TAG_3D_ID == nTagId && bTagInfoOn == true)
        {
            //pnl_tagInfo.SetActive(false);
            pnl_tagInfo.GetComponent<Animator>().Play("tagDetailHide");
            bTagInfoOn = false;
            Utils.TAG_3D_ID = -1;
        }
            
    }

    IEnumerator RefreshTagPanel()
    {
        pnl_tagInfo.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        pnl_tagInfo.SetActive(true);
    }

    public void ClickTag(Transform transTag, int nTagId)
    {
        transTagDetail = transTag;
        Vector3 v3TagScreenPos = Camera.main.WorldToScreenPoint(transTagDetail.position);
        float fScreenWidth = Screen.width * 1f;
        float fScreenHeight = Screen.height * 1f;

        float fCanvasPosY = (v3TagScreenPos.y / fScreenHeight - 0.5f) * Utils.SAMPLE_HEIGHT;
        float fCanvasPosX = (v3TagScreenPos.x / fScreenWidth - 0.5f) * Utils.SAMPLE_WIDTH;

        pnl_tagInfo.GetComponent<RectTransform>().anchoredPosition = new Vector2(fCanvasPosX + 20f, fCanvasPosY);

        ShowTagInfo(nTagId);

    }

    void UpdateTagDetailPos()
    {
        if (transTagDetail == null) return;
        Vector3 v3TagScreenPos = Camera.main.WorldToScreenPoint(transTagDetail.position);
        float fScreenWidth = Screen.width * 1f;
        float fScreenHeight = Screen.height * 1f;

        float fCanvasPosY = (v3TagScreenPos.y / fScreenHeight - 0.5f) * Utils.SAMPLE_HEIGHT;
        float fCanvasPosX = (v3TagScreenPos.x / fScreenWidth - 0.5f) * Utils.SAMPLE_WIDTH;

        pnl_tagInfo.GetComponent<RectTransform>().anchoredPosition = new Vector2(fCanvasPosX + 20f, fCanvasPosY);

    }

    public void DisplayTagButtons()
    {
        if (bTagOn) bTagOn = false;
        else bTagOn = true;
       
        for (int i = 0; i < listTagButtons.Count; i++)
        {
            listTagButtons[i].SetActive(bTagOn);
        }
    }

    /**************************************
     *SaveFileFromURL
     *Save file from url
     * 
     * ************************************/

    IEnumerator SaveFileFromURL( int nIndex)
    {
        string path = ModelInfoPool.GetModelInfo(nIndex).url;
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        Debug.Log("progress: " + www.downloadProgress);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            pnl_downloading.SetActive(false);
        }
        else
        {
            byte[] results = www.downloadHandler.data;
            Debug.Log("File data size: " + results.Length);
            string filePath = Path.Combine(Application.persistentDataPath,nIndex + ".glb");

            //string filePath = Utils.TEST_PATH + Utils.GLTF_FOLDER + nIndex + Utils.GLTF_EXT;
            File.WriteAllBytes(filePath, results);
            InitModel(nIndex);
        }
    }

    /******************************************************************************************************************
     * ShowUpModel() 
     * It shows up the 3d model by rotating for 3 seconds then disable the animator as it prevent the zoom and drag
     * Lighting for directional, env, reflection intensities also changes dynamically as fading effects for 3 seconds
     * The user can manipulate the model after 3 seconds (Utils.MODEL_SHOWUP_DURATION)
     **************************************************************************************************************** */
    IEnumerator ShowUpModel()
    {
        objTargetModel.transform.position = v3OriginalPosition;
        while (fShowUpCounter < Utils.MODEL_SHOWUP_DURATION) 
        {
            fShowUpCounter += Time.deltaTime;
            float fLightIntensity = Mathf.Lerp(0, .6f, fShowUpCounter / Utils.MODEL_SHOWUP_DURATION);
            sceneLight.intensity = fLightIntensity;
            RenderSettings.reflectionIntensity = fLightIntensity;
            RenderSettings.ambientIntensity = fLightIntensity;
            if (objTargetModel != null)
            {
                float fCurveValue = zoomCurve.Evaluate(fShowUpCounter / Utils.MODEL_SHOWUP_DURATION);
                objTargetModel.transform.localScale = Vector3.Lerp(new Vector3(0.5f ,0.5f, 0.5f), Vector3.one, fShowUpCounter / Utils.MODEL_SHOWUP_DURATION);
                objTargetModel.transform.localEulerAngles = Vector3.Lerp(new Vector3 (0,180f,0), Vector3.zero, fShowUpCounter / Utils.MODEL_SHOWUP_DURATION);

            }
            yield return null;
        }
        
        //objTargetModel.GetComponent<Animator>().enabled = false;
        objTargetModel.transform.localEulerAngles = Vector3.zero;
        objTargetModel.transform.localScale = Vector3.one;
        bShowUpFinished = true;
    }

    /**********************************************************************************************************
     * RotateObject: rotate the 3d model based on spawner position's axis ( prevent the rotation distortion )
     * it uses smooth functionality: Lerp
     * mouse drag and touch dragging works 
     * *******************************************************************************************************/
    void RotateObject()
    {
        if (bHold)
        {
            v3CurrentLocalRotation = v3DefaultRotation;
            v3NewLocalRotation = v3CurrentLocalRotation;
            return;
        }
           
        if (Vector3.Distance(v3CurrentLocalRotation, v3NewLocalRotation) > Utils.TOLERATE_ROTATION_DELTA && !bRewindRotate)
        {
            v3CurrentLocalRotation = Vector3.Lerp(v3CurrentLocalRotation, v3NewLocalRotation, Time.deltaTime / Utils.EASE_DURATION);
           // objTargetModel.transform.Rotate(v3CurrentLocalRotation);
            objTargetModel.transform.RotateAround(transModelSpawner.position, Vector3.up, v3CurrentLocalRotation.y);
            objTargetModel.transform.RotateAround(transModelSpawner.position, Vector3.right, v3CurrentLocalRotation.x / 2);
            if (Vector3.Distance(v3CurrentLocalRotation, v3NewLocalRotation) <= Utils.TOLERATE_ROTATION_DELTA && !bDragging)
                bRewindRotate = true;

        }
        else if (Vector3.Distance(v3CurrentLocalRotation, v3DefaultRotation) > Utils.TOLERATE_ROTATION_DELTA && bRewindRotate)
        {  
                v3CurrentLocalRotation = Vector3.Lerp(v3CurrentLocalRotation, v3DefaultRotation, Time.deltaTime / Utils.EASE_DURATION);
                // objTargetModel.transform.Rotate(v3CurrentLocalRotation);
                objTargetModel.transform.RotateAround(transModelSpawner.position, Vector3.up, v3CurrentLocalRotation.y);
                objTargetModel.transform.RotateAround(transModelSpawner.position, Vector3.right, v3CurrentLocalRotation.x / 2);
        }

        Vector3 modelEulerAngles = objTargetModel.transform.eulerAngles;

        objTargetModel.transform.eulerAngles = new Vector3(Utils.ClampAngle(modelEulerAngles.x, v2RotRestrictionX.x, v2RotRestrictionX.y), Utils.ClampAngle(modelEulerAngles.y, v2RotRestrictionY.x, v2RotRestrictionY.y), 0);
    }

    /***********************************************************************************
     * ZoomObject() : 
     * Scale the 3d model based on zoom in and out event of finger touch or mouse scroll
     * Smooth Functionality is used with Lerp
     * *********************************************************************************/
    void ZoomObject()
    {
        if (Mathf.Abs(fNewZoom - fCurrentZoom) > Utils.TOLERATE_ZOOM_DELTA)
        {
            fCurrentZoom = Mathf.Lerp(fCurrentZoom, fNewZoom, Time.deltaTime / Utils.EASE_DURATION);
            objTargetModel.transform.localScale = Vector3.one * fCurrentZoom;
            Utils.CURRENT_SCALE3D = fCurrentZoom;
        }
    }


    /****************************************************************************************
     * TranslateObject()
     * It is a method that changes the object position ( panning ) based on v3NewPosition,
     * 
     * **************************************************************************************/
    void TranslateObject()
    {
        if (Vector3.Distance(v3NewPosition, v3CurrentPosition) > Utils.TOLERATE_TRANSLATION)
        {
            v3CurrentPosition = Vector3.Lerp(v3CurrentPosition, v3NewPosition, Time.deltaTime / Utils.EASE_DURATION);
            
            objTargetModel.transform.position = v3CurrentPosition;

            transModelSpawner.position = v3CurrentPosition;
        }
    }

    /****************************************************************************************
     * UpdateTransitionLimit()
     * It is needed to clamp the transition of the model so that it will not over the screen
     * use some value based on 4:3 screen ratio and calculate the current screen ratio based
     * whenever needed
     * It is called in OnMove()
     * **************************************************************************************/
    void UpdateTransitionLimit()
    {
        float currentRatio = 1f * Screen.height / Screen.width;

        fTranslateLimitX = Utils.DEFAULT_WIDTH * Utils.DEFAULT_RATIO / currentRatio;
    }

    /*******************************************************************************
     * ResetModel()
     * Reset the model position, rotation and zoom into initial
     * It uses Coroutine function to smoothly transit from current
     * to initial position.
     * It is called when the user press the reset button in UI
     * ****************************************************************************/
    public void ResetModel()
    {
        bResetModel = true;
        fResetTimeCounter = 0;
        v3CurrentLocalRotation = objTargetModel.transform.localEulerAngles;
        currentRot = objTargetModel.transform.rotation;
        fNewZoom = 1f;
        fCurrentZoom = 1f;
        v3CurrentLocalRotation = v3DefaultRotation;
        StartCoroutine(SmoothResetModel());

    }

    /************************************************************
     * SmoothRestModel()
     * Coroutine function to change the model state into initial 
     * into some time period by looking smoothness
     * It is called by ResetModel()
     * ***********************************************************/
    IEnumerator SmoothResetModel()
    {
        while (fResetTimeCounter < Utils.MODEL_RESET_DURATION)
        {
            fResetTimeCounter += Time.deltaTime;
            float fLerpVal = fResetTimeCounter / Utils.MODEL_RESET_DURATION;
            objTargetModel.transform.localPosition = Vector3.Lerp(objTargetModel.transform.localPosition, v3OriginalPosition, fLerpVal);
            objTargetModel.transform.localScale = Vector3.Lerp(objTargetModel.transform.localScale, Vector3.one, fLerpVal);

            objTargetModel.transform.rotation = Quaternion.Lerp(currentRot, originalRot, fLerpVal);
            transModelSpawner.position = objTargetModel.transform.position;
            
            yield return null;
        }
        bResetModel = false;
        
    }

    /*****************************************************
     * ChangeDLI ()
     * changes the directional light intensity value
     * called from UIManager UpdateDLI
     * ***************************************************/
    public void ChangeDLI ()
    {
        sceneLight.intensity = Utils.INTENSITY_DIRECTIONAL_LIGHT;
    }

    /*****************************************************
     * ChangeELI()
     * changes the environmental light intensity value
     * called from UIManager UpdateELI
     * ***************************************************/
    public void ChangeELI ()
    {
        RenderSettings.ambientIntensity = Utils.INTENSITY_ENVIRONMENTAL_LIGHT;
    }

    /****************************************************
     * ChangeRLI()
     * changes the reflection light intensity value
     * Called from UIManager UpdateRLI
     * **************************************************/
    public void ChangeRLI()
    {
        RenderSettings.reflectionIntensity = Utils.INTENSITY_REFLECTION_LIGHT;
    }

    float ClampAngle(float angle, float min, float max)
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

    #endregion

    #region Event Listener methods & functions

    /**********************************************************
     * OnDrag()
     * Event listener function for OnDragMouse event delegate
     * associated with EventManager.OnDragMouse()
     * augments: dragX, dragY
     * registered when the class object is enabled
     * removed when the class object is disabled
     * *******************************************************/
    void OnDrag(float dragX, float dragY)
    {
        Debug.Log("OnDrag: " + dragX + ":" + dragY);
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
        float rotX = dragX * Utils.ROTATION_SPEED;
        float rotY = dragY * Utils.ROTATION_SPEED;
#endif
#if ((UNITY_IOS || UNITY_ANDROID) && (!UNITY_EDITOR))
        float rotX = dragX * Utils.ROTATION_SPEED * .3f;
        float rotY = dragY * Utils.ROTATION_SPEED * .5f;
#endif
        v3NewLocalRotation = new Vector3(rotY, -rotX, 0);
        bDragging = true;
        bRewindRotate = false;
        bHold = false;
    }

    /************************************************************
     * OnScroll(float fScroll)
     * Event listener function for OnScrollMouse event delegate
     * associated with EventManager.OnScrollMouse()
     * augments: fScroll
     * registered when the class object is enabled
     * removed when the class object is disabled
     * **********************************************************/
    public void OnScroll(float fScroll)
    {
        float zoomVal = fScroll;
        fNewZoom += zoomVal * Utils.EASE_DURATION * Utils.SENSITIVITY * Utils.SENSITIVITY;
        bHold = false;
        // check the zoom value range from min to max
        if (fNewZoom > Utils.MAX_SCALE_VAL * Utils.SENSITIVITY)
            fNewZoom = Utils.MAX_SCALE_VAL * Utils.SENSITIVITY;
        else if (fNewZoom < Utils.MIN_SCALE_VAL)
            fNewZoom = Utils.MIN_SCALE_VAL;
        
    }

    /**************************************************
     * OnPointerUp()
     * mouse or touch pointer up event listener
     * Associated with EventManager.OnPointerUp
     * added in enable function
     * removed in disable function
     * ***********************************************/
    private void OnPointerUp()
    {
        bDragging = false;
        bHold = false;
    }

    /**************************************************
     * OnPointerDown()
     * mouse or touch pointer Down event listener
     * Associated with EventManager.OnPointerDown deligator
     * make the rotation false
     * added in enable function
     * removed in disable function
     * ***********************************************/
    private void OnPointerDown()
    {
        bHold = true;
        
    }

    public void RemoveS3Handler()
    {
        Destroy(s3Handler.gameObject);
    }

    /******************************************
     * OnMove(float fMoveX, float fMoveY)
     * panning function it is called in 
     * 2 finger touch or middle, right mouse btn
     * press dragging
     *************************************** */
    private void OnMove(float fMoveX, float fMoveY)
    {
        //Debug.Log("moveX: " + fMoveX + " moveY:" + fMoveY);
        UpdateTransitionLimit();
        float fRealMoveDeltaX = fMoveX * fTranslateLimitX;
        float fRealMoveDeltaY = fMoveY * fTranslateLimitY;

        v3NewPosition += new Vector3(fRealMoveDeltaX, fRealMoveDeltaY, 0f);
        if (v3NewPosition.x > fTranslateLimitX * fCurrentZoom)
            v3NewPosition.x = fTranslateLimitX * fCurrentZoom;
        else if (v3NewPosition.x < -fTranslateLimitX * fCurrentZoom)
        {
            v3NewPosition.x = -fTranslateLimitX * fCurrentZoom;
        }
        if (v3NewPosition.y > fTranslateLimitY * fCurrentZoom)
            v3NewPosition.y = fTranslateLimitY * fCurrentZoom;
        else if (v3NewPosition.y < -fTranslateLimitY * fCurrentZoom)
        {
            v3NewPosition.y = -fTranslateLimitY * fCurrentZoom;
        }
       // dbgTxt.text += "v3NewPosition:" + v3NewPosition.x + " : " + v3NewPosition.y + "\n";
    }

    void OnPressPrevModel()
    {
        if (nCurrentModelIndex > 0)
        {
            Config.current3DViewID = nCurrentModelIndex - 1;
            InitModel(nCurrentModelIndex - 1);
        }
    }

    void OnPressNextModel()
    {
        if (nCurrentModelIndex < prefModelList.Length - 1)
        {
            Config.current3DViewID = nCurrentModelIndex + 1;
            InitModel(nCurrentModelIndex + 1);
        }
    }

    void OnDownloadSuccess()
    {
        Debug.Log("File download Success.");
        pnl_downloading.SetActive(false);
        InitModel(Config.current3DViewID);
    }

    void OnDownloadFail()
    {
        Debug.Log("File download Fail.");
        pnl_downloading.SetActive(false);
    }

    #endregion
}