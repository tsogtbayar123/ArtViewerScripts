using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class TileInfoPool 
{
    public static List<TileInfo> listTileInfo = new List<TileInfo>();
    public static List<TileStepURL> list_tile_step_URLS = new List<TileStepURL>();
    public static string[] miniMapUrl = { "https://i.ibb.co/K0THHVd/mini.jpg",
                                            "https://i.ibb.co/yQ6dXX2/minimap2.png",
                                            "https://i.ibb.co/bF6zgkW/mini3.png"
                                        };


    public static void LoadTileInfo()
    {
        string jsonData = Utils.LoadTextFromFile(Utils.JSON_TILE_PATH);
        listTileInfo.Clear();

        var dict = JSON.Parse(jsonData);
        JSONNode objNode = dict;

        int nCount = objNode.Count;

        for (int i = 0; i < nCount; i++)
        {
            TileInfo tileInfo = new TileInfo();
            tileInfo.tileId = objNode[i]["tileSceneId"];
            tileInfo.rows = objNode[i]["tileRowCount"];
            tileInfo.cols = objNode[i]["tileColCount"];
            tileInfo.tileSize = objNode[i]["tileSize"];
            tileInfo.list_zoom = new List<float>();
            tileInfo.list_Splits = new List<int>();
            tileInfo.list_width = new List<float>();
            tileInfo.list_height = new List<float>();

            int nZoomStepCount = objNode[i]["tileZoomSteps"].Count;

            for (int j = 0; j < nZoomStepCount; j++)
            {
                float fVal = float.Parse(objNode[i]["tileZoomSteps"][j]);
                tileInfo.list_zoom.Add(fVal);
                float fWidth = float.Parse(objNode[i]["tileSizeStepsX"][j]);
                tileInfo.list_width.Add(fWidth);
                float fHeight = float.Parse(objNode[i]["tileSizeStepsY"][j]);
                tileInfo.list_height.Add(fHeight);
            }

            int nZoomSplitCount = objNode[i]["tileSplits"].Count;
            //Debug.LogError("split count: " + nZoomSplitCount);
            for (int k = 0; k < nZoomSplitCount; k++)
            {
                int nSplit = int.Parse(objNode[i]["tileSplits"][k]);
                tileInfo.list_Splits.Add(nSplit);
            }

            listTileInfo.Add(tileInfo);
        }
        Debug.Log("TileInfo: " + listTileInfo.Count);
    }

    public static TileInfo GetTileInfo(int nId)
    {
        return listTileInfo[nId];
    }

    public static string GetTileURL(int nZoomStep, int nId)
    {
        return list_tile_step_URLS[nZoomStep].list_tile_urls[nId];
    }

    public static void DumpTestURL(int nId)
    {
        string strFileURLDump = Utils.LoadTextFromFile(Utils.TXT_TILE_PATH);
        string[] strUrlLine = strFileURLDump.Split('\n');
        
        int nZoomStepLength = listTileInfo[nId].list_zoom.Count;
        for (int i = 0; i < nZoomStepLength; i++)
        {
            TileStepURL stepURLs = new TileStepURL();
            stepURLs.nStep = i;
            stepURLs.list_tile_urls.Clear();

            int nRows = (int)(listTileInfo[nId].rows * listTileInfo[nId].list_zoom[i]);
            int nCols = (int)(listTileInfo[nId].cols * listTileInfo[nId].list_zoom[i]);
            for (int j = 0; j < nRows; j++)
                for (int k = 0; k < nCols; k++)
                {
                    string fileName = "img-" + i + "-" + j * listTileInfo[nId].tileSize + "-" + k * listTileInfo[nId].tileSize;
                  
                    for (int m = 0; m < strUrlLine.Length; m++)
                    {
                        if (strUrlLine[m].Contains(fileName))
                        {
                            stepURLs.list_tile_urls.Add(strUrlLine[m]);
                            break;
                        }
                    }
                }
            Debug.Log("step url length: " + stepURLs.list_tile_urls.Count + " loop Counter:" + nRows * nCols);
            list_tile_step_URLS.Add(stepURLs);
        }
        
    }

    public static void GenerateS3FileName(int nId)
    {

        int nZoomStepLength = listTileInfo[nId].list_zoom.Count;
        list_tile_step_URLS.Clear();
        for (int i = 0; i < nZoomStepLength; i++)
        {
            TileStepURL stepURLs = new TileStepURL();
            stepURLs.nStep = i;
            stepURLs.list_tile_urls.Clear();

            int nRows = (int)(listTileInfo[nId].rows * listTileInfo[nId].list_zoom[i]);
            int nCols = (int)(listTileInfo[nId].cols * listTileInfo[nId].list_zoom[i]);
            for (int j = 0; j < nRows; j++)
                for (int k = 0; k < nCols; k++)
                {
                    string fileName = Utils.ArrayBucketHolders[nId] + "img_" + i + "_" + j * listTileInfo[nId].tileSize + "_" + k * listTileInfo[nId].tileSize + ".jpg";
                    stepURLs.list_tile_urls.Add(fileName);
                    
                }
            Debug.Log("step url length: " + stepURLs.list_tile_urls.Count + " loop Counter:" + nRows * nCols);
            list_tile_step_URLS.Add(stepURLs);
        }
    }

    public static void InitTileListValue()
    {
        
    }
}

public class TileInfo
{
    public int tileId;
    public int rows;
    public int cols;
    public int tileSize;
    public List<float> list_zoom;
    public List<int> list_Splits;
    public List<float> list_width;
    public List<float> list_height;

}

public class TileStepURL
{
    public int nStep;
    public List<string> list_tile_urls = new List<string> ();
    public string strMiniURL = "https://i.ibb.co/K0THHVd/mini.jpg";
    public Vector2 v2Pos = Vector2.zero;
}

