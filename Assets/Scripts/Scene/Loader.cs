using System.Collections;
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
        SceneManager.LoadScene("Loading");
        onLoaderCallback += () => { SceneManager.LoadScene(scene); };
    }

    public static void Load(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void LoaderCallback()
    {
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
