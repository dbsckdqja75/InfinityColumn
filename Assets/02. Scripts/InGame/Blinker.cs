using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Blinker : MonoBehaviour
{
    [SerializeField] float blinkInterval = 1f;

    Light targetLight;

    void Awake()
    {
        targetLight = this.GetComponent<Light>();        
    }

    void OnEnable() 
    {
        BlinkerLoop().Start(this);
    }

    void OnDisable() 
    {
        StopAllCoroutines();
    }

    IEnumerator BlinkerLoop()
    {
        float lightIntensity = targetLight.intensity;

        while(true)
        {
            yield return new WaitForSeconds(blinkInterval);

            lightIntensity = targetLight.intensity;
            targetLight.intensity = 0;

            yield return new WaitForSeconds(blinkInterval);

            targetLight.intensity = lightIntensity;

            yield return new WaitForEndOfFrame();
        }
    }
}
