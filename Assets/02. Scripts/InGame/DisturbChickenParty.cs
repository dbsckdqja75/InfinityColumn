using System.Collections.Generic;
using UnityEngine;

public class DisturbChickenParty : DisturbObject
{
    [SerializeField] int maxChickenCount = 3;

    [SerializeField] List<GameObject> chickenPrefab = new List<GameObject>();

    List<DisturbChicken> chickenList = new List<DisturbChicken>();

    public override void Init(DisturbManager disturbManager)
    {
        SpawnManager spawnManager = disturbManager.GetSpawnManager();
        SetTargetColumn(spawnManager.GetLastColumn(3));
    }

    public void SetTargetColumn(Column[] columns)
    {
        int chickenCount = Random.Range(1, (maxChickenCount + 1));

        foreach(Column column in columns)
        {
            if(chickenList.Count < chickenCount)
            {
                GameObject branch = column.GetCurrentBranch();
                if(branch)
                {
                    GameObject prefab = chickenPrefab[Random.Range(0, chickenPrefab.Count)];

                    DisturbChicken chicken = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<DisturbChicken>();
                    chicken.SetTargetBranch(branch);
                    
                    chickenList.Add(chicken);
                }
            }
        }
    }

    public override bool OnTrigger()
    {
        foreach(DisturbChicken chicken in chickenList)
        {
            chicken.OnTrigger();
        }

        Deactivate();

        return true;
    }
}
