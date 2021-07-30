/*************************************************
 * ------------TextDisplayAnimation--------------
 * Used to show text characters in time intervals
 * In this project it is used to show loading text
 * Repeats showing characters
 * **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextDisplayAnimation : MonoBehaviour {
    #region public variables
    [Tooltip ("displaying text UI component")]
    public TextMeshProUGUI displayText;
    [Tooltip ("displaying Text string content")]
    public string strLoading = "LOADING...";
    [Tooltip ("display text counter")]
    public int nCounter = 0;
    [Tooltip ("text character display interval")]
    public float fDisplayInterval = 0.3f;
    #endregion

    #region MonoBehavior methods
    // Use this for initialization
    void Start () {
        StartCoroutine(DisplayText());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    #region custom methods & fucntions
    /*********************************************************************
     *DisplayText() 
     *DisPlay text characters with fDisplayInterval time sec
     * It repeats displaying text.
     * Runs in Start() function
     * *******************************************************************/
    IEnumerator DisplayText()
    {
        while (true)
        {
            nCounter++;
            displayText.text = strLoading.Substring(0, nCounter);
            
            nCounter = nCounter % strLoading.Length;
            yield return new WaitForSeconds(fDisplayInterval);
        }
        
    }
    #endregion
}
