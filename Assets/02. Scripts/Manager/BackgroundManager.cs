using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] List<GameObject> presetList;

    BackgroundPreset currentPreset;

    public void ResetBackground()
    {
        if(currentPreset)
        {
            Destroy(currentPreset.gameObject);
        }

        currentPreset = Instantiate(presetList[Random.Range(0, presetList.Count)], Vector3.zero, Quaternion.identity).GetComponent<BackgroundPreset>();

        UpdatePresetQuality();
    }

    public void UpdatePresetQuality()
    {
        if(QualitySettings.GetQualityLevel() < 1)
        {
            currentPreset.ClearForLowQuality();
        }
        else
        {
            currentPreset.RestoreForHighQuality();
        }
    }
}
