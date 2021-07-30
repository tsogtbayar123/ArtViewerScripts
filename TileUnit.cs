using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TileUnit : MonoBehaviour
{
    public int nTileUnitId;
    Color startColor = new Color(1f, 1f, 1f, 0);
    Color endColor = new Color(1f, 1f, 1f, 1f);
    float fCurrentTime = 0;

    private void Start()
    {
        StartCoroutine(FadeTile());
    }

    IEnumerator FadeTile()
    {
        while (fCurrentTime < Utils.TILE_FADE_DURATION)
        {
            GetComponent<RawImage>().color = Color.Lerp(startColor, endColor, fCurrentTime / Utils.TILE_FADE_DURATION);
            fCurrentTime += Time.deltaTime;
            yield return null;
        }
        
    }

}
