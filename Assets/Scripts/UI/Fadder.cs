using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadder : MonoBehaviour
{
    static private Fadder instance;
    static public Fadder Instance()
    {
        if (instance)
        {
            return instance;
        }
        else
        {
            Fadder fadder = GameObject.FindObjectOfType<Fadder>();
            if (!fadder)
            {
                Debug.LogError("No fadder available");
                return null;
            }
            instance = fadder;
            return instance;
        }
    }
    public Animator animator;

    public delegate void FadeInEnd();
    public static FadeInEnd OnFadeInEnd;

    public delegate void FadeOutEnd();
    public static FadeOutEnd OnFadeOutEnd;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void AnimFadeInEnd()
    {
        OnFadeInEnd?.Invoke();
    }
    public void AnimFadeOutEnd()
    {
        OnFadeOutEnd?.Invoke();
    }

    static public void BeginFadeIn()
    {
        instance?.animator.Play("FadeIn");
    }

    static public void BeginFadeOut()
    {
        instance?.animator.Play("FadeOut");
    }
}
