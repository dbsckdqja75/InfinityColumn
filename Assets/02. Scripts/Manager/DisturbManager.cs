using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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

    [Space(10)]
    [SerializeField] Transform playerTrf;
    [SerializeField] SpawnManager spawnManager;

    [Space(10)]
    [SerializeField] DisturbData disturbData;

    // NOTE : 이벤트 발생 점수 간격 (Random Range)
    SecureValue<int> currentMinScoreStep = new SecureValue<int>(50); // NOTE : 기본값 50
    SecureValue<int> currentMaxScoreStep = new SecureValue<int>(60); // NOTE : 기본값 60

    SecureValue<int> targetScore = new SecureValue<int>(0);

    DisturbObject currentDisturb;

    public void ChangeScoreStep(int minScoreStep, int maxScoreStep)
    {
        currentMinScoreStep.SetValue(minScoreStep);
        currentMaxScoreStep.SetValue(maxScoreStep);
    }

    public void ResetTrigger()
    {
        targetScore.SetValue(GetRandTargetScore());

        if(currentDisturb)
        {
            currentDisturb.Deactivate();
        }
    }

    public void UpdateTrigger(int currentScore, bool forceRandom = false)
    {
        if(targetScore.GetValue() <= currentScore && currentDisturb == null)
        {
            GameObject disturbPrefab = forceRandom ? disturbData.GetRandomDisturbPrefab() : disturbData.GetRandomDisturbPrefab(currentScore);
            if(disturbPrefab != null)
            {
                DisturbObject disturb = Instantiate(disturbPrefab, Vector3.zero, Quaternion.identity).GetComponent<DisturbObject>();
                currentDisturb = disturb;
                currentDisturb.Init(this);

                if(currentDisturb.OnTrigger())
                {
                    targetScore.SetValue(currentScore + GetRandTargetScore());
                }
            }
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

        return Random.Range(currentMinScoreStep.GetValue(), currentMaxScoreStep.GetValue() + 1);
    }

    public Transform GetPlayerTransform()
    {
        return playerTrf;
    }

    public SpawnManager GetSpawnManager()
    {
        return spawnManager;
    }

    #if UNITY_EDITOR
    public void DebugForceTrigger()
    {
        ResetTrigger();
        
        int debugScore = targetScore.GetValue();
        UpdateTrigger(debugScore);
        targetScore.SetValue(debugScore);
    }

    public DisturbData DebugGetDisturbData()
    {
        return disturbData;
    }

    public void DebugSetDisturbData(DisturbData data)
    {
        disturbData = data;
    }

    public void DebugSetToggle(bool isOn)
    {
        isDebug = isOn;
    }
    #endif
}
