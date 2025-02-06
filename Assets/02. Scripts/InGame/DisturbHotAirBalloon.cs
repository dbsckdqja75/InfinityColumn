using System.Collections;
using UnityEngine;

public class DisturbHotAirBalloon : DisturbObject
{
    [SerializeField] float minSpeed = 0.1f;
    [SerializeField] float maxSpeed = 0.15f;

    [Space(10)]
    [SerializeField] float startY = -12f;
    [SerializeField] float endY = 12f;
    [SerializeField] Vector3 minOffset, maxOffset;

    public override bool OnTrigger()
    {
        transform.SetParent(Camera.main.transform);

        FlyMotion().Start(this);

        return true;
    }

    IEnumerator FlyMotion()
    {
        Vector3 startPos = transform.localPosition;
        startPos.x = Random.Range(minOffset.x, maxOffset.x);
        startPos.y = startY;
        startPos.z = Random.Range(minOffset.z, maxOffset.z);
        startPos.x *= RandomExtensions.RandomBool() ? 1 : -1;

        bool onLeftStart = RandomExtensions.RandomBool();
        startPos.x *= onLeftStart ? -1 : 1;

        Vector3 endPos = startPos;
        endPos.y = endY;

        Vector3 currentPos = startPos;

        float speed = Random.Range(minSpeed, maxSpeed);
        yield return CoroutineExtensions.ProcessAction(speed, (t) => {
            currentPos = Vector3.Lerp(startPos, endPos, t);

            transform.localPosition = currentPos;
        });

        Deactivate();

        yield break;
    }
}
