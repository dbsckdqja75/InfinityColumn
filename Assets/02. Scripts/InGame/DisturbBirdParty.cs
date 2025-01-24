using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisturbBirdParty : DisturbObject
{
    [SerializeField] bool canCrossPoint;
    [SerializeField] float flyDistance = 8f; // NOTE : PC 해상도 기준 카메라 시점 고려
    [SerializeField] float maxDelayTime = 1f;

    [Space(10)]
    [SerializeField] Vector3[] points;

    [SerializeField] List<GameObject> birdPrefab = new List<GameObject>();

    List<DisturbBird> birdList = new List<DisturbBird>();

    void OnEnable() 
    {
        transform.SetParent(Camera.main.transform);
        transform.localPosition = Vector3.zero;

        int birdCount = Random.Range(1, (points.Length + 1));
        foreach(Vector3 point in points)
        {
            if(birdList.Count < birdCount)
            {
                GameObject prefab = birdPrefab[Random.Range(0, birdPrefab.Count)];

                float startPosX = RandomExtensions.RandomBool() ? flyDistance : -flyDistance;

                Vector3 startPos = new Vector3(startPosX, point.y, point.z);
                Vector3 endPos = new Vector3(-startPosX, point.y, point.z);

                // NOTE : 단방향이 아닌 양방향으로 서로 엇갈릴 수 있도록 함
                if(canCrossPoint)
                {
                    Vector3 randomPoint = points[Random.Range(0, points.Length)];
                    randomPoint.z = RandomExtensions.RandomBool() ? randomPoint.z : point.z;

                    // NOTE : XY축의 사선으로도 엇갈릴 수 있도록 함
                    endPos = new Vector3(endPos.x, randomPoint.y, randomPoint.z);
                }

                DisturbBird bird = Instantiate(prefab, startPos, Quaternion.identity, this.transform).GetComponent<DisturbBird>();
                bird.SetWayPoint(startPos, endPos);

                birdList.Add(bird);
            }
        }
    }

    public override bool OnTrigger()
    {
        BirdPartyLogic().Start(this);

        return true;
    }

    IEnumerator BirdPartyLogic()
    {
        foreach(DisturbBird bird in birdList)
        {
            bird.OnTrigger();

            float delayTime = Random.Range(0.01f, maxDelayTime);
            yield return new WaitForSeconds(delayTime);
        }

        foreach(DisturbBird bird in birdList)
        {
            while(bird != null)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        Deactivate();

        yield break;
    }
}
