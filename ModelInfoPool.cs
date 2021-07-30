/******************************************************************************
 * --------------------- ModelInfoPool----------------------------------------
 * List container of the 3d model info storing the ModelInfo class objects.
 * It is generated when the ArtViewer3D scene loaded.
 * If you want to add more info please check the Model3D.json file in Resources
 * **************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class ModelInfoPool {
    public static List<ModelInfo> listModelInfo = new List<ModelInfo>();
    
    public static void LoadModel3DInfo()
    {
        string jsonData = Utils.LoadTextFromFile(Utils.JSON_3DMODELS_PATH);

        listModelInfo.Clear();

        var dict = JSON.Parse(jsonData);
        JSONNode objNode = dict;

        int nCount = objNode.Count;

        for (int i = 0; i < nCount; i++)
        {
            ModelInfo modelInfo = new ModelInfo();
            modelInfo.id = objNode[i]["id"];
            modelInfo.modelTitle = objNode[i]["title"];
            modelInfo.url = objNode[i]["url"];
            modelInfo.refId = objNode[i]["ref2d"];
            modelInfo.modelDescription = objNode[i]["description"];
            modelInfo.modelSize = objNode[i]["size"];
            modelInfo.modelArtist = objNode[i]["artist"];
            modelInfo.modelMaterial = objNode[i]["material"];
            modelInfo.modelDate = objNode[i]["date"];
            modelInfo.lightRotation = objNode[i]["lightRot"];

            float rotMaxX = objNode[i]["rotMaxX"];
            float rotMinX = objNode[i]["rotMinX"];
            float rotMaxY = objNode[i]["rotMaxY"];
            float rotMinY = objNode[i]["rotMinY"];

            modelInfo.v2RotationRestrictionX = new Vector2(rotMinX, rotMaxX);
            modelInfo.v2RotationRestrictionY = new Vector2(rotMinY, rotMaxY);

            JSONNode objTagNode = objNode[i]["tagList"];
            int nTagCounter = objTagNode.Count;
            modelInfo.listTagInfo = new List<TagInfo3D>();

            if (nTagCounter > 0)
            {
                
                for (int j = 0; j < nTagCounter; j++)
                {
                    TagInfo3D tagInfo = new TagInfo3D();
                    float posX = objTagNode[j]["position"][0];
                    float posY = objTagNode[j]["position"][1];
                    float posZ = objTagNode[j]["position"][2];
                    Vector3 pos = new Vector3(posX, posY, posZ);
                    Debug.Log("tag Pos: " + pos);
                    string strInfo = objTagNode[j]["tagInfo"] + "";
                    Debug.Log("tag Info: " + strInfo);
                    tagInfo.tagPos = pos;
                    tagInfo.tagInfo = strInfo;
                    modelInfo.listTagInfo.Add(tagInfo);
                }
            }
            listModelInfo.Add(modelInfo);
        }
    }

    public static ModelInfo GetModelInfo (int nId)
    {
        for (int i = 0; i < listModelInfo.Count; i++)
        {
            if (i == nId)
                return listModelInfo[i];
        }
        return null;
    }

}
