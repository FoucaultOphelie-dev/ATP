﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public static class Loader
{
    private static Action onLoaderCallback;
    public static void LoadWithLoadingScreen(string scene)
    {
        Debug.Log("Loading scene:" + scene);
        onLoaderCallback += () => { SceneManager.LoadScene(scene); };
        if (Fadder.Instance())
        {
            Fadder.OnFadeOutEnd += () => { SceneManager.LoadScene("Loading"); };
            Fadder.BeginFadeOut();
        }
        else
        {
            SceneManager.LoadScene("Loading");
        }
    }

    public static void Load(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void LoaderCallback()
    {
        onLoaderCallback?.Invoke();
        onLoaderCallback = null;
    }
}
