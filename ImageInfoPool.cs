/*******************************************************************
 * -------------- ImageInfoPool -----------------------------------
 * It is the storage class that load the 2d image info from 
 * Model2D.json. The info is stored in the listImageInfo.
 * It is not a MonoBehaviour class and static class so can use 
 * it anywhere in the project.
 * ****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class ImageInfoPool {
    // static list for image info which contains id, title, ref3dId, description, tiled image and big image path in resources directory
    public static List<ImageInfo> listImageInfo = new List<ImageInfo>();

    public static void LoadImageInfo()
    {
        string jsonData = Utils.LoadTextFromFile(Utils.JSON_2DMODELS_PATH);
        listImageInfo.Clear();

        var dict = JSON.Parse(jsonData);
        JSONNode objNode = dict;

        int nCount = objNode.Count;

        for (int i = 0; i < nCount; i++)
        {
            ImageInfo imgInfo = new ImageInfo();

            imgInfo.id = objNode[i]["id"];
            imgInfo.modelTitle = objNode[i]["title"];
            imgInfo.refId = objNode[i]["ref3d"];     //reference 3D model Id that is needed to switch the 2D scene into 3D scene
            imgInfo.modelDescription = objNode[i]["description"];
            imgInfo.modelArtist = objNode[i]["artist"];
            imgInfo.modelMaterial = objNode[i]["material"];
            imgInfo.modelSize = objNode[i]["size"];
            imgInfo.modelDate = objNode[i]["date"];
            imgInfo.rows = objNode[i]["rows"];      //tiledimage view row counts 
            imgInfo.cols = objNode[i]["cols"];      //titledimage view column counts
            imgInfo.path = objNode[i]["path"];      //the top directory of this image's resources in Resources directory
            imgInfo.subPath = objNode[i]["subpath"];    // tiled images directory in imgInfo.path directory
            imgInfo.spriteName = objNode[i]["imgName"]; // big image name in imgInfo.path

            JSONNode objTagNode = objNode[i]["tagList"];
            int nTagCounter = objTagNode.Count;
            imgInfo.listTagInfo = new List<TagInfo2D>();

            if (nTagCounter > 0)
            {
                
                for (int j = 0; j < nTagCounter; j++)
                {
                    TagInfo2D tagInfo = new TagInfo2D();
                    float posX = objTagNode[j]["position"][0];
                    float posY = objTagNode[j]["position"][1];
                    string strInfo = objTagNode[j]["tagInfo"] + "";
                    tagInfo.tagPos = new Vector2(posX, posY);
                    tagInfo.tagInfo = strInfo;
                    imgInfo.listTagInfo.Add(tagInfo);
                }
            }

            listImageInfo.Add(imgInfo);
        }

    }

    // returns the ImageInfo object of index nImageId 
    // if the pool list does not contains it then returns null, accident case!!
    public static ImageInfo GetImageInfo(int nImageId)
    {
        for (int i = 0; i < listImageInfo.Count; i++)
        {
            if (i == nImageId)
            {
                return listImageInfo[i];
            }
        }
        return null;
    }


}
