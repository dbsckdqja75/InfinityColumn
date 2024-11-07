using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisturbBirdParty : DisturbObject
{
    [SerializeField] bool canCrossPoint;
    [SerializeField] float flyDistance = 5f;
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

                float pointX = RandomExtensions.RandomBool() ? flyDistance : -flyDistance;

                Vector3 startPos = new Vector3(pointX, point.y, point.z);
                Vector3 endPos = new Vector3(-pointX, point.y, point.z);

                if(canCrossPoint)
                {
                    Vector3 randPoint = points[Random.Range(0, points.Length)];
                    randPoint.z = RandomExtensions.RandomBool() ? randPoint.z : point.z;

                    endPos = new Vector3(endPos.x, randPoint.y, randPoint.z);
                }

                DisturbBird bird = Instantiate(prefab, startPos, Quaternion.identity, this.transform).GetComponent<DisturbBird>();
                bird.SetStartPoint(startPos);
                bird.SetEndPoint(endPos);

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
            if(bird != null)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        Deactivate();

        yield break;
    }
}
