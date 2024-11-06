using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DisturbConfig
{
    public int startScore, endScore;
    public GameObject prefab;
}

public class DisturbManager : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField] bool isDebug = false;
    #endif

    [SerializeField] int minRandScore = 100;
    [SerializeField] int maxRandScore = 300;

    [Space(10)]
    [SerializeField] Transform playerTrf;
    [SerializeField] SpawnManager spawnManager;

    [Space(10)]
    [SerializeField] DisturbData disturbData;

    int targetScore;

    DisturbObject currentDisturb;

    public void ResetTrigger()
    {
        targetScore = GetRandTargetScore();

        if(currentDisturb)
        {
            currentDisturb.Deactivate();
        }
    }

    public void UpdateTrigger(int currentScore)
    {
        if(targetScore <= currentScore && currentDisturb == null)
        {
            GameObject disturbPrefab = disturbData.GetRandomDisturbPrefab(currentScore);
            if(disturbPrefab == null)
            {
                return;
            }

            DisturbObject disturbObj = Instantiate(disturbPrefab, Vector3.zero, Quaternion.identity).GetComponent<DisturbObject>();
            currentDisturb = disturbObj;

            InitCurrentDisturb();

            if(currentDisturb.OnTrigger())
            {
                targetScore = (currentScore + GetRandTargetScore());
            }
        }
    }

    void InitCurrentDisturb()
    {
        if(currentDisturb is IDisturbPlayer)
        {
            ((IDisturbPlayer)currentDisturb).SetTargetPlayer(playerTrf);
        }

        if(currentDisturb is IDisturbBranch)
        {
            ((IDisturbBranch)currentDisturb).SetTargetBranch(spawnManager.GetLastColumnBranch());
        }

        if(currentDisturb is IDisturbColumn)
        {
            ((IDisturbColumn)currentDisturb).SetTargetColumn(spawnManager.GetLastColumn(3));
        }
    }

    int GetRandTargetScore()
    {
        #if UNITY_EDITOR
        if(isDebug)
        {
            return 10;
        }
        #endif

        return Random.Range(minRandScore, maxRandScore + 1);
    }
}
