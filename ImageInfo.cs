/***********************************************
 * ------------ ImageInfo --------------------
 * Structural object for 2d image object.
 * It is loaded from Model2d.json in Resources
 * when the ArtViewer2D is loaded and saved in
 * the ImageInfoPool object's list.
 ******************************************* */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageInfo {
    public int id;
    public int refId; // reference 3d model's id. If have no ref then default value is -1
    public string modelTitle;
    public string modelDescription;
    public string modelArtist;
    public string modelMaterial;
    public string modelSize;
    public string modelDate;
    public int rows;            // tiled view rows
    public int cols;            // tiled view cols
    public string path;
    public string subPath;      // tiled image folder
    public string spriteName;   // image Name
    public List<TagInfo2D> listTagInfo;

}

public class TagInfo2D
{
    public Vector2 tagPos;
    public string tagInfo;
}
