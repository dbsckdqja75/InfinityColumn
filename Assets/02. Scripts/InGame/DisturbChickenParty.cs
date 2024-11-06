using System.Collections.Generic;
using UnityEngine;

public class DisturbChickenParty : DisturbObject, IDisturbMultiple, IDisturbColumn
{
    [SerializeField] int maxChickenCount = 3;

    [SerializeField] List<GameObject> chickenPrefab = new List<GameObject>();

    List<DisturbChicken> disturbChickenList = new List<DisturbChicken>();

    public void SetTargetColumn(Column[] columns)
    {
        int chickenCount = Random.Range(1, (maxChickenCount + 1));

        foreach(Column column in columns)
        {
            if(disturbChickenList.Count < chickenCount)
            {
                GameObject branch = column.GetCurrentBranch();
                if(branch)
                {
                    GameObject prefab = chickenPrefab[Random.Range(0, chickenPrefab.Count)];

                    DisturbChicken chicken = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<DisturbChicken>();
                    chicken.SetTargetBranch(branch);
                    
                    disturbChickenList.Add(chicken);
                }
            }
        }
    }

    public override bool OnTrigger()
    {
        foreach(DisturbChicken chicken in disturbChickenList)
        {
            chicken.OnTrigger();
        }

        Deactivate();

        return true;
    }
}
