using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponent : MonoBehaviour
{
    public ScrollRectButton button;
    public MapHandler mapH;
    public Color clickColor = Color.red;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.5f, 0.5f, 1f);
    public int componentId;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update

    public void HoverMapComponent()
    {
        spriteRenderer.color = hoverColor;
    }

    public void ClickMapComponent()
    {
        spriteRenderer.color = clickColor;
        button.CenterOnThis();
        mapH.StartMapMoveCoroutine(transform.position * -1f);
        GetComponent<TextGroupHandler>().tg.lines[0].startTime = 0f;
    }

    public void LoseFocus()
    {
        spriteRenderer.color = normalColor;

    }

    void CenterCamera() {
        
    }


}
