using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class GltfLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadModel(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }
        var ext = Path.GetExtension(path).ToLower();
        if (ext != ".glb") return;
        var context = new UniGLTF.ImporterContext();
        var file = File.ReadAllBytes(path);
        context.ParseGlb(file);
        context.Load();
        context.ShowMeshes();
        context.EnableUpdateWhenOffscreen();
        context.ShowMeshes();
        
    }

    
}
