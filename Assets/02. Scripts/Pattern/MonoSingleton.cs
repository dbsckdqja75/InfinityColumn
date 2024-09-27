using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    public static T Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType(typeof(T)) as T;
            }
            return instance;
        }
    }

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        Init();

        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void Init()
    {

    }
}