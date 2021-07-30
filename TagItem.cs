using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagItem : MonoBehaviour
{
    public TMPro.TextMeshPro textId;
    public int nId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        float fNewScale = Utils.TAG_SCALE / Utils.CURRENT_SCALE3D;
        transform.localScale = Vector3.one * fNewScale;
    }

    public void InitTag(int nTagId)
    {
        nId = nTagId;
        textId.text = (nTagId + 1) + "";
    }

    private void OnMouseDown()
    {
        Debug.Log("Tag " + nId + " is clicked.");
        ModelViewManager.Instance().ClickTag(transform, nId);
    }
}
