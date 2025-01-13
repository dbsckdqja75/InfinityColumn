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
            #if UNITY_EDITOR || UNITY_STANDALONE
            if(QualitySettings.GetQualityLevel() <= 1) // NOTE : '하' 옵션부터 배경 간소화
            {
                currentPreset.ClearForLowQuality();
            }
            #else
            currentPreset.ClearForLowQuality();
            #endif

            if(QualitySettings.GetQualityLevel() <= 0) // NOTE : '저' 옵션부터 추가 간소화
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
