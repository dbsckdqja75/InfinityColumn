using System.Collections;
using UnityEngine;

public class DisturbBird : DisturbObject
{
    [SerializeField] float minSlope, maxSlope;

    [Space(10)]
    [SerializeField] float minSpeed = 1f;
    [SerializeField] float maxSpeed = 1.5f;

    Vector3 startPos, endPos, currentPos;

    public void SetWayPoint(Vector3 startPoint, Vector3 endPoint)
    {
        startPos = startPoint;
        endPos = endPoint;
    }

    public override bool OnTrigger()
    {
        FlyMotion().Start(this);

        return true;
    }

    IEnumerator FlyMotion()
    {
        bool isPlayedSound = false;

        transform.localPosition = startPos;

        Vector3 lookAtPoint = (transform.parent.position + endPos);
        transform.LookAt(lookAtPoint);

        float angleZ = Random.Range(minSlope, maxSlope);
        transform.Rotate(Vector3.forward, angleZ);

        float speed = Random.Range(minSpeed, maxSpeed);
        yield return CoroutineExtensions.ProcessAction(speed, (t) => {
            currentPos = Vector3.Lerp(startPos, endPos, t);

            transform.localPosition = currentPos;

            if(!isPlayedSound && t >= 0.5f)
            {
                isPlayedSound = true;

                SoundManager.Instance.PlaySound("BirdSwish", transform.position);
            }
        });

        Deactivate();

        yield break;
    }
}
