/*************************************************
 * --------------UIManager-----------------------
 * UI Manager script for both ArtViewer 2d and 3d 
 * ***********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    #region public variable
    [Tooltip("object's title UI text component")]
    public TextMeshProUGUI txt_obj_title;
    [Tooltip("object's description UI text component")]
    public TextMeshProUGUI txt_obj_descriptions;
    [Tooltip("object's artist UI text component")]
    public TextMeshProUGUI txt_obj_artist;
    [Tooltip("object's material UI text component")]
    public TextMeshProUGUI txt_obj_material;
    [Tooltip("object's size UI text component")]
    public TextMeshProUGUI txt_obj_size;
    [Tooltip("object's date UI text component")]
    public TextMeshProUGUI txt_obj_date;

    [Tooltip("total Object / current Number")]
    public TextMeshProUGUI txt_obj_index;
    [Tooltip("Side panel's animation controller")]
    public Animator animPanel;
    [Tooltip("3d modle view manager which is not null in ArtView 3D scene")]
    public ModelViewManager _manager;
    [Tooltip("2d image view manager which is not null in ArtViewer2D scene")]
    public ImageViewManager _manager2D;
    [Tooltip("online 2d image view manager which is not null in ArtViewer2D scene")]
    public OnlineImageGalleryManager _managerOnline2D;
    [Tooltip("Panel's current showing case: description or setting pannel")]
    public UISTATE uiState = UISTATE.DESCRIPTIONS;
    [Tooltip("setting button")]
    public Button btn_settings;
    [Tooltip("description button")]
    public Button btn_descriptions;
    [Tooltip("close button-> need to load gallery scene")]
    public Button btn_close;
    [Tooltip("reset button for rotation, position and scale of the model")]
    public Button btn_reset;
    [Tooltip("UI input for 3d model's rotation value")]
    public InputField inpt_rotation_speed;
    [Tooltip("UI input for 3d model's pan speed")]
    public InputField inpt_pan_speed;
    [Tooltip("UI input for rotation ease time")]
    public InputField inpt_ease_time;
    [Tooltip("UI input for minimun zoom value")]
    public InputField inpt_zoom_min;
    [Tooltip("UI input for button zoom value")]
    public float inpt_zoom_val = 5;
    [Tooltip("UI input for maximum zoom value")]
    public InputField inpt_zoom_max;
    [Tooltip("UI input for time duration of the reset")]
    public InputField inpt_reset_timer;
    [Tooltip("UI slider input for directional light's intensity")]
    public Slider slider_DL;
    [Tooltip("UI slider input for environmental light's intensity")]
    public Slider slider_EL;
    [Tooltip("UI slider input for reflectional light's intensity")]
    public Slider slider_RL;
    [Tooltip("UI game object for 3d view settings")]
    public GameObject pnl_settings;
    [Tooltip("UI game Object for description")]
    public GameObject pnl_descriptions;
    [Tooltip("3D game scene Render setting of post processing")]
    public GameObject pnl_render_settings;
    [Tooltip("mini picture panel object for 2d view scene")]
    public GameObject pnl_mini;
    [Tooltip("previous object browse button")]
    public GameObject btn_prev;
    [Tooltip("Next objet browse button")]
    public GameObject btn_next;
    [Tooltip("previous object browse button alt")]
    public GameObject btn_prev_alt;
    [Tooltip("Next objet browse button alt")]
    public GameObject btn_next_alt;
    [Tooltip("Switch mode button")]
    public GameObject btn_switch;
    [Tooltip("Parent panel view")]
    public GameObject m_ParentPanelView;
    [Tooltip("Image panel")]
    public GameObject m_ImagePanel;
    [Tooltip("Rendered model panel")]
    public GameObject pnl_model_render;
    [Tooltip("Bottom button panel Rect transform")]
    public RectTransform rectButtonPanel;
    [Tooltip("sample button for size adjustment")]
    public RectTransform rectSampleButton;
    public float fResetTimer = 1f;
    public bool bResetTime = false;
    public GameObject sceneContainer;

    public static UIManager uiManager;

    public static UIManager Instance()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        }

        return uiManager;
    }
    #endregion

    #region private variable
    bool bShowPanel = false; // show side panel on off
    bool bShowingMini = false;
    bool bShowRender = false; // show render setting panel
    #endregion

    #region MonoBehaviour methods
    // Use this for initialization
    void Start()
    {
        //InitSettingPanel();
        AdjustButtonSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (bResetTime)
            fResetTimer -= Time.deltaTime;
        if (fResetTimer <= 0)
        {
            bResetTime = false;
            fResetTimer = 1f;
        }
    }
    // add the event listener
    private void OnEnable()
    {
        EventManager.OnModelChanged3D += OnModelChanged3D;
        EventManager.OnModelChanged2D += OnImageChanged;
    }
    // remove the event listener
    private void OnDisable()
    {
        EventManager.OnModelChanged3D -= OnModelChanged3D;
        EventManager.OnModelChanged2D -= OnImageChanged;
    }
    #endregion

    #region custom methods & functions
    private void OnModelChanged3D(int nIndex, bool bEnd)
    {
        
        // if the model is the first model in the model pool then 
        // hide the prev button, otherwise show this button
        if (nIndex == 0)
            btn_prev.GetComponent<Button>().interactable = false;
        else
            btn_prev.GetComponent<Button>().interactable = true;
        

        // if the model is the last model in the mnodel pool then 
        // hide the next button, otherwise show this button
        if (bEnd)
            btn_next.GetComponent<Button>().interactable = false;
        else
            btn_next.GetComponent<Button>().interactable = true;
        

        string strTitle = ModelInfoPool.listModelInfo[nIndex].modelTitle;
        string strDescription = ModelInfoPool.listModelInfo[nIndex].modelDescription;
        string strMaterial = ModelInfoPool.listModelInfo[nIndex].modelMaterial;
        string strSize = ModelInfoPool.listModelInfo[nIndex].modelSize;
        string strArtist = ModelInfoPool.listModelInfo[nIndex].modelArtist;
        string strDate = ModelInfoPool.listModelInfo[nIndex].modelDate;
        float xLightRot = ModelInfoPool.listModelInfo[nIndex].lightRotation;
        Debug.Log(xLightRot);
        txt_obj_title.text = strTitle;
        txt_obj_descriptions.text = strDescription;
        txt_obj_material.text = strMaterial;
        txt_obj_size.text = strSize;
        txt_obj_date.text = strDate;
        txt_obj_artist.text = strArtist;
    }

    //when press the next or prev button in ArtViewer2D scene the image is changed
    private void OnImageChanged(int nIndex, bool bEnd)
    {

        if (nIndex == 0)
            btn_prev.GetComponent<Button>().interactable = false;
        else
            btn_prev.GetComponent<Button>().interactable = true;


        if (bEnd)
            btn_next.GetComponent<Button>().interactable = false;
        else
            btn_next.GetComponent<Button>().interactable = true;
        txt_obj_title.text = ImageInfoPool.GetImageInfo(nIndex).modelTitle;
        txt_obj_descriptions.text = ImageInfoPool.GetImageInfo(nIndex).modelDescription;
        txt_obj_material.text = ImageInfoPool.GetImageInfo(nIndex).modelMaterial;
        txt_obj_size.text = ImageInfoPool.GetImageInfo(nIndex).modelSize;
        txt_obj_artist.text = ImageInfoPool.GetImageInfo(nIndex).modelArtist;
        txt_obj_date.text = ImageInfoPool.GetImageInfo(nIndex).modelDate;
    }

    public void UpdateOnlineObjectDescription(OnlineMuseumObject objMuseum)
    {
        txt_obj_title.text = objMuseum.title;
        txt_obj_descriptions.text = objMuseum.medium;
        txt_obj_material.text = objMuseum.classification;
        txt_obj_size.text = objMuseum.dimensions;
        txt_obj_artist.text = objMuseum.artistDisplayName;
        txt_obj_date.text = objMuseum.objectEndDate;
        txt_obj_index.text = (Utils.nObjId + 1) + " / " + Utils.LIST_MUSEUM_OBJ_ID.Count;
    }

    //Show on / off the description panel
    public void OnClickShowDescription()
    {
        if (bShowPanel)
        {
            //animPanel.Play("closeDescription");
            pnl_descriptions.GetComponent<RectTransform>().DOAnchorPosX(-245, 1f);
            bShowPanel = false;
            
            if (pnl_model_render != null) {
                pnl_model_render.GetComponent<RectTransform>().DOAnchorPosX(0, 1f);
            }
            //StartCoroutine(ActivateSettingButton());

        }
        else
        {
            uiState = UISTATE.DESCRIPTIONS;
            //animPanel.Play("showDescription");
            pnl_descriptions.GetComponent<RectTransform>().DOAnchorPosX(230, 1f);
            
            if (pnl_model_render != null) {
                pnl_model_render.GetComponent<RectTransform>().DOAnchorPosX(210, 1f);
            }
            bShowPanel = true;
            // btn_close.gameObject.SetActive(false);
            // btn_settings.gameObject.SetActive(false);
            // btn_descriptions.gameObject.SetActive(true);
            // pnl_descriptions.SetActive(true);
            // pnl_settings.SetActive(false);
        }
    }

    public void OnClickRenderSetting()
    {
        if (bShowRender)
        {
            pnl_render_settings.GetComponent<RectTransform>().DOAnchorPosX(-245, 1f);
            bShowRender = false;
        } else
        {
            uiState = UISTATE.DESCRIPTIONS;
            pnl_render_settings.GetComponent<RectTransform>().DOAnchorPosX(230, 1f);
            bShowRender = true;
        }
    }

    // toggle on off showing settings panel
    public void OnClickShowSettings()
    {
        if (bShowPanel)
        {
            animPanel.Play("closeDescription");
            bShowPanel = false;
            StartCoroutine(ActivateDescriptionButton());

        }
        else
        {
            uiState = UISTATE.SETTINGS;
            animPanel.Play("showDescription");
            bShowPanel = true;
            btn_settings.gameObject.SetActive(true);
            btn_descriptions.gameObject.SetActive(false);
            pnl_descriptions.SetActive(false);
            pnl_settings.SetActive(true);
        }
    }

    //Initialize the settings panel contents
    void InitSettingPanel()
    {
        inpt_rotation_speed.text = "" + Utils.ROTATION_SPEED;
        inpt_pan_speed.text = "" + Utils.PAN_SPEED;
        inpt_reset_timer.text = "" + Utils.MODEL_RESET_DURATION;
        inpt_zoom_min.text = "" + Utils.MIN_SCALE_VAL;
        inpt_zoom_max.text = "" + Utils.MAX_SCALE_VAL;
        inpt_ease_time.text = "" + Utils.EASE_DURATION;

        slider_DL.value = Utils.INTENSITY_DIRECTIONAL_LIGHT;
        slider_EL.value = Utils.INTENSITY_ENVIRONMENTAL_LIGHT;
        slider_RL.value = Utils.INTENSITY_REFLECTION_LIGHT;

    }

    /**********************************************************
     * CheckSwitchButton
     * If the current scene's 2d or 3d object does not have 
     * related 3d or 2d object then it hide the switch button,
     * otherwise shows button.
     * ******************************************************/
    void CheckSwitchButton()
    {
        if (Config.currentSceneType == GameScene.Browse2D)
        {
            if (ImageInfoPool.GetImageInfo(Config.current2DViewID).refId == -1)
                btn_switch.SetActive(false);
            else
                btn_switch.SetActive(true);
        }
        else if (Config.currentSceneType == GameScene.Browse3D)
        {
            if (ModelInfoPool.GetModelInfo(Config.current3DViewID).refId == -1)
                btn_switch.SetActive(false);
            else
                btn_switch.SetActive(true);
        }
    }

    IEnumerator ActivateDescriptionButton()
    {
        yield return new WaitForSeconds(1f);
        btn_descriptions.gameObject.SetActive(true);
    }

    IEnumerator ActivateSettingButton()
    {
        yield return new WaitForSeconds(1f);
        btn_close.gameObject.SetActive(true);
        btn_settings.gameObject.SetActive(true);
    }
    #endregion

    #region Panel Settings event handlers
    public void UpdateRotationSpeed()
    {
        float fRotationSpeed = float.Parse(inpt_rotation_speed.text);
        Utils.ROTATION_SPEED = fRotationSpeed;
    }

    public void UpdateEaseValue()
    {
        float fEaseVal = float.Parse(inpt_ease_time.text);
        Utils.EASE_DURATION = fEaseVal;
    }

    public void UpdatePanSpeed()
    {
        float fPanVal = float.Parse(inpt_pan_speed.text);
        Utils.PAN_SPEED = fPanVal;
    }

    public void UpdateZoomMin()
    {
        float fZoomMin = float.Parse(inpt_zoom_min.text);
        Utils.MIN_SCALE_VAL = fZoomMin;
    }

    public void UpdateZoomMax()
    {
        float fZoomMax = float.Parse(inpt_zoom_max.text);
        Utils.MAX_SCALE_VAL = fZoomMax;
    }

    public void UpdateReset()
    {
        float fResetVal = float.Parse(inpt_reset_timer.text);
        Utils.MODEL_RESET_DURATION = fResetVal;
    }

    public void UpdateDLI()
    {
        if (_manager != null)
        {
            float fDLI = slider_DL.value;
            Utils.INTENSITY_DIRECTIONAL_LIGHT = fDLI;
            _manager.ChangeDLI();
        }

    }

    public void UpdateELI()
    {
        if (_manager != null)
        {
            float fELI = slider_EL.value;
            Utils.INTENSITY_ENVIRONMENTAL_LIGHT = fELI;
            _manager.ChangeELI();
        }
    }

    public void UpdateRLI()
    {
        if (_manager != null)
        {
            float fRIL = slider_RL.value;
            Utils.INTENSITY_REFLECTION_LIGHT = fRIL;
            _manager.ChangeRLI();
        }

    }

    void AdjustButtonSize()
    {
        Vector2 currentPanelSize = rectButtonPanel.sizeDelta;
        float fScreenX = Screen.width * 1f;
        float fScreenY = Screen.height * 1f;
        float fInit = Utils.INITIAL_HEIGHT;

        if (_manager2D != null)
            fInit = Utils.INITIAL_HEIGHT_2;
        float fAdjustmentValue = Utils.INITIAL_HEIGHT + Utils.RATIO_STEP_VALUE * (fScreenX / fScreenY - 1f);
        currentPanelSize = new Vector2(currentPanelSize.x, fAdjustmentValue);
        rectButtonPanel.sizeDelta = currentPanelSize;
    }
    #endregion

    #region panel side's buttons event handlers
    //make the switch button on | off 
    // called from ImageViewManager and ModelViewManager
    public void SetSwitchButtonVisibility(bool bVisible)
    {
        btn_switch.SetActive(bVisible);
    }

    public void OnClickZoomIn()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
        {
            if (_manager != null)
                _manager.OnScroll(inpt_zoom_val);
        }
        else if (Config.currentSceneType == GameScene.Browse2D)
        {
            if (_manager2D != null)
            {
                _manager2D.OnScroll(inpt_zoom_val);
                //_manager2D.OnMiniDrag();
            }
        }
        else if (Config.currentSceneType == GameScene.OnlineBrowse2D)
        {
            if (_managerOnline2D != null)
            {
                _managerOnline2D.OnScroll(inpt_zoom_val);
            }
        }
    }

    public void OnClickZoomOut()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
        {
            if (_manager != null)
                _manager.OnScroll(-inpt_zoom_val);
        }
        else if (Config.currentSceneType == GameScene.Browse2D)
        {
            if (_manager2D != null)
                _manager2D.OnScroll(-inpt_zoom_val);
        }
        else if (Config.currentSceneType == GameScene.OnlineBrowse2D)
        {
            if (_managerOnline2D != null)
                _managerOnline2D.OnScroll(-inpt_zoom_val);
        }
    }

    public void OnClickReset()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
        {
            if (_manager != null)
                _manager.ResetModel();
        }
        else if (Config.currentSceneType == GameScene.Browse2D)
        {
            
            if (_manager2D != null)
            {
                if (bResetTime == false)
                {
                    _manager2D.ResetImage();
                    bResetTime = true;
                }
                
            }
                
        } else if (Config.currentSceneType == GameScene.OnlineBrowse2D)
        {
            if (_managerOnline2D != null)
            {
                if (bResetTime == false)
                {
                    _managerOnline2D.ResetImage();
                    bResetTime = true;
                }

            }
        }

    }

    public void GetNoiseDestruction()
    {

    }

    public void OnSwitchTo2D()
    {
        if (ModelInfoPool.GetModelInfo(Config.current3DViewID).refId == -1)
            return;
        Config.current2DViewID = ModelInfoPool.GetModelInfo(Config.current3DViewID).refId;
        SceneManager.LoadScene("ArtViewer2D");
    }

    public void OnSwitchTo3D()
    {
        if (ImageInfoPool.GetImageInfo(Config.current2DViewID).refId == -1)
            return;
        Config.current3DViewID = ImageInfoPool.GetImageInfo(Config.current2DViewID).refId;
        Debug.Log("Current3DView:" + Config.current3DViewID);
        SceneManager.LoadScene("ArtViewer3D");
    }

    public void OnReturn2Gallery()
    {
        SceneManager.LoadScene("ArtViewerGallery");
    }

    public void OnReturn2Menu()
    {
        SceneManager.LoadScene("ArtViewerMenu");
    }

    public void OnReturn2Video()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
        {
            _manager.RemoveS3Handler();

            Destroy(sceneContainer);
        }
        else if (Config.currentSceneType == GameScene.Browse2D)
        {
            Destroy(sceneContainer);
        }
        if (MainManager.Instance() != null)
            //MainManager.Instance().ActivateVideo();
            MainManager.Instance().ActivateCameras();
            //MainManager.Instance().PauseVideoOnOff();
    }

    public void OnPressPrevBtn()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
            EventManager.DoPressPrevModel3DButton();
        else if (Config.currentSceneType == GameScene.Browse2D || Config.currentSceneType == GameScene.OnlineBrowse2D)
            EventManager.DoPressPrevImage();

    }

    public void OnPressNextBtn()
    {
        if (Config.currentSceneType == GameScene.Browse3D)
            EventManager.DoPressNextModel3DButton();
        else if (Config.currentSceneType == GameScene.Browse2D || Config.currentSceneType == GameScene.OnlineBrowse2D)
        {
            EventManager.DoPressNextImage();
        }
    }

    public void OnPressRotateRight()
    {
        EventManager.DoRotateImageRight();
    }

    public void OnPressRotateLeft()
    {
        EventManager.DoRotateImageLeft();
    }
    #endregion
    #region minimap toggle
    public void OnMiniToggleOn()
    {
        pnl_mini.transform.localScale = Vector3.one;
    }

    public void OnMiniToggleOff()
    {
        pnl_mini.transform.localScale = Vector3.zero;
    }

    public void OnMiniToggle() {
        if (bShowingMini) {
            OnMiniToggleOff();
            bShowingMini = false;
        } else {
            OnMiniToggleOn();
            bShowingMini = true;
        }

    }
    #endregion
}

public enum UISTATE
{
    SETTINGS,
    DESCRIPTIONS
}
