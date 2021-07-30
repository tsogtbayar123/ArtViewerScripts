using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInfo {
    public int id;
    public int refId;
    public string url;
    public string modelTitle;
    public string modelDescription;
    public string modelArtist;
    public string modelSize;
    public string modelMaterial;
    public string modelDate;
    public float lightRotation;
    public Vector2 v2RotationRestrictionX;
    public Vector2 v2RotationRestrictionY;
    public List<TagInfo3D> listTagInfo;
    
}

public class TagInfo3D
{
    public Vector3 tagPos;
    public string tagInfo;
}

