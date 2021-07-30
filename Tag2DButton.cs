using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tag2DButton : MonoBehaviour
{
    public Text txt_id;
    public int nId;
    bool bClicked = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one / ImageViewManager.Inst.GetScale();
    }

    public void InitBtn(int nTagId)
    {
        nId = nTagId;
        txt_id.text = (nTagId + 1) + "";
    }

    public void ClickTagButton()
    {
        ImageViewManager.Inst.ClickTag(this.GetComponent<RectTransform>(), nId);
    }
}
