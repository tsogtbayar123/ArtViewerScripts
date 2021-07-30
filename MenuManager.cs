using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public void OnClick2DViewer ()
    {
        SceneManager.LoadScene("ArtViewer2D");
    }

    public void OnClick3DViewer ()
    {
        SceneManager.LoadScene("ArtViewer3D");
    }

        public void OnClickAPIViewer ()
    {
        SceneManager.LoadScene("OnlineGallery");
    }

}
