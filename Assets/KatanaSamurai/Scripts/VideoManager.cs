using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer m_video;

    bool checkVideoPlaying = false;
    
    private void Start()
    {
        StartCoroutine(CheckVideo());
    }

    IEnumerator CheckVideo()
    {
        yield return new WaitForSeconds(1);
        checkVideoPlaying = true;
    }

    private void Update()
    {
        if (m_video.isPlaying )
        {
            return;
        }
        else if (checkVideoPlaying)
        {
            SceneManager.LoadScene("KatanaSamuraiUI_Intro");
        }
        
    }
}
