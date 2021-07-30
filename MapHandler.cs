using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapHandler : MonoBehaviour
{
    public Text txt_dbg;
    public GameObject objMapBackground;
    public float fMaxScaleVal = 3f;
    public float fMinScaleVal = 0.5f;
    public float fScaleFactor = 0.2f;
    public float fScaleVal = 1f;
    public int nLayerMapBG = 10;
    public int nLayerMapRoom = 11;
    public float fResetTimeLimit = 1f;
    public float fMouseReleaseTimeLimit = 0.2f;
    public float fMouseDragMinLength = 0.1f;
    public bool bReset = false;
    MapComponent prevHoverComp;
    MapComponent prevClickComp;
    MapComponent currentComp;
    int nCurrentId = -1;
    int nPrevId = -1;
    Vector3 v3PointerDownPos = Vector3.zero;
    Vector3 v3CurrentPos = Vector3.zero;
    Vector3 v3DifFromCenter = Vector3.zero;
    Vector3 v3MouseDownPos = Vector3.zero;
    float fMaxPosY = 9f;
    float fMaxPosX = 12f;
    float fResetTimeCounter = 0;
    float fMouseHoldTimeCounter = 0;

    bool bMouseHold = false;

    private void OnEnable()
    {
        EventManager.OnPointerDown += OnPointerDown;
        EventManager.OnPointerUp += OnPointerUp;
        EventManager.OnDragMouse += OnDragMouse;
        EventManager.OnScrollMouse += OnScrollMouse;
        EventManager.OnPinchScreen += OnPinchScreen;        
    }

    private void OnDisable()
    {
        EventManager.OnPointerDown -= OnPointerDown;
        EventManager.OnPointerUp -= OnPointerUp;
        EventManager.OnDragMouse -= OnDragMouse;
        EventManager.OnScrollMouse -= OnScrollMouse;
        EventManager.OnPinchScreen -= OnPinchScreen;
    }

    void OnPointerDown ()
    {
        if (bReset) return;
        bMouseHold = true;
        fMouseHoldTimeCounter = 0;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, nLayerMapBG);
        if (hit.collider != null)
        {
            v3CurrentPos = new Vector3(hit.point.x, hit.point.y, 0);
            v3DifFromCenter = v3CurrentPos - objMapBackground.transform.position;
            v3MouseDownPos = v3CurrentPos;

        }

    }

    void OnPointerUp ()
    {
        if (bReset) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, nLayerMapBG);
        if (hit.collider != null)
        {
            v3CurrentPos = objMapBackground.transform.position;
            v3DifFromCenter = Vector3.zero;
        }
        bMouseHold = false;

    }

    void OnDragMouse(float fPosX, float fPosY)
    {
        if (bReset) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, nLayerMapBG);
        
        if (hit.collider != null)
        {
            if (v3DifFromCenter != Vector3.zero)
            {
                v3CurrentPos = new Vector3(hit.point.x, hit.point.y, 0) - v3DifFromCenter;
                objMapBackground.transform.position = v3CurrentPos;
                CheckPositionLimitOver();
            }
        }
    }

    void TargetButton() {
        v3DifFromCenter = v3CurrentPos - objMapBackground.transform.position;
    }

    void OnScrollMouse(float fScroll)
    {
        if (bReset) return;
        //Debug.Log("scroll Value" + fScroll);
        if (fScroll > 0)
        {
            if (fScaleVal < fMaxScaleVal)
            {
                fScaleVal += fScroll * fScaleFactor;
                if (fScaleVal > fMaxScaleVal)
                    fScaleVal = fMaxScaleVal;
            }
        } else
        {
            if (fScaleVal > fMinScaleVal)
            {
                fScaleVal += fScroll * fScaleFactor;
                if (fScaleVal < fMinScaleVal)
                {
                    fScaleVal = fMinScaleVal;
                }
            }
        }
        objMapBackground.transform.localScale = fScaleVal * Vector3.one;
    }

    void OnPinchScreen(float fScroll, Vector2 v2CenterPos)
    {
        if (bReset) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, nLayerMapBG);
        if (hit.collider != null)
        {
            v3CurrentPos = new Vector3(hit.point.x, hit.point.y, 0);
            v3DifFromCenter = v3CurrentPos - objMapBackground.transform.position;
        }

       // Debug.Log("OnPinchScreen: " + fScroll + ":" + v2CenterPos);
        float fPrevScale = fScaleVal;
        if (fScroll > 0)
        {
            if (fScaleVal < fMaxScaleVal)
            {
                fScaleVal += fScroll * fScaleFactor;
                if (fScaleVal > fMaxScaleVal)
                    fScaleVal = fMaxScaleVal;

            }
        }
        else
        {
            if (fScaleVal > fMinScaleVal)
            {
                fScaleVal += fScroll * fScaleFactor;
                if (fScaleVal < fMinScaleVal)
                {
                    fScaleVal = fMinScaleVal;
                }
            }
        }
        v3DifFromCenter = v3DifFromCenter * fScaleVal / fPrevScale;
        objMapBackground.transform.localScale = fScaleVal * Vector3.one;
        v3CurrentPos = new Vector3(hit.point.x, hit.point.y, 0) - v3DifFromCenter;
        objMapBackground.transform.position = v3CurrentPos;

    }


    // Start is called before the first frame update
    void Start()
    {
        Config.currentSceneType = GameScene.MapView;
        nLayerMapBG = 1 << nLayerMapBG;             // bit shift for layer flag set active
        nLayerMapRoom = 1 << nLayerMapRoom;         // bit shift for layer flag set active
        CalculateMaxLimitPosX();
    }

    // Update is called once per frame
    void Update()
    {
        if (bReset) return;

        if (bMouseHold)
        {
            fMouseHoldTimeCounter += Time.deltaTime;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log("bDragging:" + bDragging);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, nLayerMapRoom);
        if (hit.collider != null)
        {
            MapComponent mapComp = hit.collider.gameObject.GetComponent<MapComponent>();
            if (mapComp != null)
            {
                if (CheckMouseClickEvent())
                {
                    //txt_dbg.text = "Room " + mapComp.componentId + " is clicked." + fMouseHoldTimeCounter;
                    mapComp.ClickMapComponent();
                } else
                {
                   // Debug.Log("Room " + mapComp.componentId + " is hovered.");
                    mapComp.HoverMapComponent();
                    if (mapComp.componentId != nCurrentId)
                    {
                        prevHoverComp = currentComp;
                        currentComp = mapComp;
                        nPrevId = nCurrentId;
                        nCurrentId = mapComp.componentId;
                    }
                    if (prevHoverComp != null)
                    {
                        prevHoverComp.LoseFocus();

                    }
                    else
                    {
                        prevHoverComp = mapComp;
                        nPrevId = prevHoverComp.componentId;
                    }
                }
            }
        }
    }

    void OnPointerEnter()
    {
        if (bReset) return;
        Debug.Log("Enter-- point -- ");
    }

    //calculate the max limit value of the drag X based on default ratio and current screen ratio
    void CalculateMaxLimitPosX()
    {
        float fCurrentScreenRatio = Screen.height * 1f / Screen.width;
        fMaxPosX = fMaxPosY / fCurrentScreenRatio;
    }

    void CheckPositionLimitOver()
    {
        Vector3 objPos = objMapBackground.transform.position;
        if (objPos.x > fMaxPosX * fScaleVal) objPos.x = fMaxPosX * fScaleVal;
        if (objPos.x < -fMaxPosX * fScaleVal) objPos.x = -fMaxPosX * fScaleVal;
        if (objPos.y > fMaxPosY * fScaleVal) objPos.y = fMaxPosY * fScaleVal;
        if (objPos.y < -fMaxPosY * fScaleVal) objPos.y = -fMaxPosY * fScaleVal;
        objMapBackground.transform.position = objPos;
    }

    public void OnClickResetButton()
    {
        bReset = true;
        fResetTimeCounter = 0;
        StartCoroutine(ResetMapPosition());
    }

    IEnumerator ResetMapPosition()
    {
        while (fResetTimeCounter < fResetTimeLimit)
        {
            fResetTimeCounter += Time.deltaTime;

            float fScale = Mathf.Lerp(objMapBackground.transform.localScale.x, 1f, fResetTimeCounter / fResetTimeLimit);
            Vector3 v3Pos = Vector3.Lerp(objMapBackground.transform.position, Vector3.zero, fResetTimeCounter/fResetTimeLimit);

            objMapBackground.transform.localScale = fScale * Vector3.one;
            objMapBackground.transform.position = v3Pos;

            if (fResetTimeCounter >= fResetTimeLimit)
            {
                bReset = false;
                objMapBackground.transform.localScale = Vector3.one;
                objMapBackground.transform.position = Vector3.zero;
            }
                
            yield return null;
        }
    }

    public void StartMapMoveCoroutine(Vector3 newPos) {
       // StartCoroutine(EventChangesMapPosition(newPos));
        EventChangesMapPosition(newPos);
    }
    void EventChangesMapPosition(Vector3 newPos)
    {
        bReset = true;

       
        //fResetTimeCounter = 0;
        // while (fResetTimeCounter < fResetTimeLimit)
        // {
            //fResetTimeCounter += Time.deltaTime;

            //float fScale = Mathf.Lerp(objMapBackground.transform.localScale.x, 1f, fResetTimeCounter / fResetTimeLimit);
            //Vector3 v3Pos = Vector3.Lerp(objMapBackground.transform.position, newPos, fResetTimeCounter/fResetTimeLimit);

            //objMapBackground.transform.localScale = fScale * Vector3.one;
            //objMapBackground.transform.position = v3Pos;
            objMapBackground.transform.DOScale(new Vector3(1f,1f,1f), 1f).OnComplete(ResetFalse).SetEase(Ease.OutQuad);
            objMapBackground.transform.DOMove(newPos, 1f).OnComplete(ResetFalse).SetEase(Ease.OutQuad);
            // if (fResetTimeCounter >= fResetTimeLimit)
            // {
            //     bReset = false;
            //     //objMapBackground.transform.localScale = Vector3.one;
            //     objMapBackground.transform.position = newPos;
            // }
       // }
    }

    void ResetFalse() {
        bReset = false;
    }

    bool CheckMouseClickEvent()
    {
        float fDistance = Vector3.Distance(v3CurrentPos, v3MouseDownPos);
        if (Input.GetMouseButtonUp(0) && fMouseHoldTimeCounter < fMouseReleaseTimeLimit && fDistance < fMouseDragMinLength) return true;
        else return false;
    }
}
