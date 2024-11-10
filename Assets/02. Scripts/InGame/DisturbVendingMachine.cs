using System.Collections;
using UnityEngine;

public class DisturbVendingMachine : DisturbObject
{
    [SerializeField] float launchAngle = 30;

    [Space(10)]
    [SerializeField] float minSpeed = 0.15f;
    [SerializeField] float maxSpeed = 0.25f;

    [Space(10)]
    [SerializeField] float startY = -20f;
    [SerializeField] float endY = 30f;
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

        bool onLeftStart = RandomExtensions.RandomBool();
        startPos.x *= onLeftStart ? -1 : 1;

        Vector3 endPos = startPos;
        endPos.x = (startPos.x * -1);
        endPos.y = endY;

        Vector3 currentPos = startPos;

        float angleZ = onLeftStart ? -launchAngle : launchAngle;
        transform.Rotate(Vector3.forward, angleZ);

        float speed = Random.Range(minSpeed, maxSpeed);
        yield return CoroutineExtensions.ProcessAction(speed, (t) => {
            currentPos = Vector3.Lerp(startPos, endPos, t);

            transform.localPosition = currentPos;
        });

        Deactivate();

        yield break;
    }
}
