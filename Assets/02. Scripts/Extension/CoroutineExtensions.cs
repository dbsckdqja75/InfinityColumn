using System;
using System.Collections;
using UnityEngine;

public static class CoroutineExtensions
{
    public static Coroutine Start(this IEnumerator logic, MonoBehaviour monoBehaviour)
    {
        return monoBehaviour.StartCoroutine(logic);
    }
    
    public static Coroutine Start(this IEnumerator logic, ref Coroutine coroutine, MonoBehaviour monoBehaviour)
    {
        coroutine = monoBehaviour.StartCoroutine(logic);
        return coroutine;
    }

    public static IEnumerator Start(this IEnumerator logic, MonoBehaviour monoBehaviour, float waitSeconds = 0f)
    {
        if (waitSeconds > 0f)
        {
            yield return new WaitForSeconds(waitSeconds);
        }
        yield return monoBehaviour.StartCoroutine(logic);
    }
    
    public static void Stop(this Coroutine logic, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StopCoroutine(logic);
    }
    
    public static IEnumerator Stop(this IEnumerator logic, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StopCoroutine(logic);
        yield break;
    }
    
    public static IEnumerator ProcessAction(float speed, Action action)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            action.Invoke();

            progress += speed * Time.smoothDeltaTime;

            yield return null;
        }
    }

    public static IEnumerator ProcessAction(float speed, Action<float> action)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            action.Invoke(progress);

            progress += speed * Time.smoothDeltaTime;

            yield return null;
        }
    }
}