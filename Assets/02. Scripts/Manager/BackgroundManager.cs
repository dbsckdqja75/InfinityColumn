using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    bool isOptimize = false;

    [SerializeField] List<GameObject> presetList;

    BackgroundPreset currentPreset;

    public void ResetBackground()
    {
        if(currentPreset)
        {
            Destroy(currentPreset.gameObject);
        }

        if(isOptimize == false)
        {
            currentPreset = Instantiate(presetList[Random.Range(0, presetList.Count)], Vector3.zero, Quaternion.identity).GetComponent<BackgroundPreset>();
            
            UpdatePresetQuality();
        }
    }

    public void UpdatePresetQuality()
    {
        if(isOptimize == false)
        {
            #if UNITY_ANDROID || UNITY_IPHONE
            currentPreset.ClearForLowQuality();
            #else
            if(QualitySettings.GetQualityLevel() < 2)
            {
                currentPreset.ClearForLowQuality();
            }
            #endif

            if(QualitySettings.GetQualityLevel() < 1)
            {
                currentPreset.ClearForVeryLowQuality();
            }
        }
    }

    public void EnableOptimization()
    {
        isOptimize = true;

        if(currentPreset)
        {
            Destroy(currentPreset.gameObject);
        }
    }
}
