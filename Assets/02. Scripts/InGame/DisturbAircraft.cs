using System.Collections;
using UnityEngine;

public class DisturbAircraft : DisturbObject
{
    [SerializeField] bool canFlipPosition;
    [SerializeField] float angleZ = 30;

    [Space(10)]
    [SerializeField] float minSpeed = 0.15f;
    [SerializeField] float maxSpeed = 0.25f;

    [Space(10)]
    [SerializeField] Vector3 startOffset;
    [SerializeField] Vector3 endOffset;

    public override bool OnTrigger()
    {
        transform.SetParent(Camera.main.transform);

        FlyMotion().Start(this);

        return true;
    }

    IEnumerator FlyMotion()
    {
        bool onFlipPosition = canFlipPosition ? RandomExtensions.RandomBool() : false;

        Vector3 startPos = onFlipPosition ? endOffset : startOffset;
        Vector3 endPos = onFlipPosition ? startOffset : endOffset;

        Vector3 currentPos = startPos;
        transform.localPosition = currentPos;

        Vector3 lookAtPoint = (transform.position + endPos);
        lookAtPoint.z = transform.position.z;

        transform.LookAt(lookAtPoint);
        transform.Rotate(Vector3.forward, onFlipPosition ? angleZ : -angleZ);

        float speed = Random.Range(minSpeed, maxSpeed);
        yield return CoroutineExtensions.ProcessAction(speed, (t) => {
            currentPos = Vector3.Lerp(startPos, endPos, t);

            transform.localPosition = currentPos;
        });

        Deactivate();

        yield break;
    }
}
