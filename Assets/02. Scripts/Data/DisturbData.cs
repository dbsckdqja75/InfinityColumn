using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisturbData", menuName = "ScriptableObject/DisturbData")]
public class DisturbData : ScriptableObject
{
    [SerializeField] List<DisturbConfig> dataList;

    public GameObject GetRandomDisturbPrefab(int targetScore)
    {
        List<DisturbConfig> filteredData = dataList.FindAll(x => (x.startScore <= targetScore) && (x.endScore >= targetScore));

        if(filteredData.Count > 0)
        {
            return filteredData[Random.Range(0, filteredData.Count)].prefab;
        }

        return null;
    }
}
