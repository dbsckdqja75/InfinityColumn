using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] List<GameObject> presetList;

    GameObject currentPreset;

    public void ResetBackground()
    {
        if(currentPreset)
        {
            Destroy(currentPreset);
        }

        currentPreset = Instantiate(presetList[Random.Range(0, presetList.Count)], Vector3.zero, Quaternion.identity);
    }
}
