using UnityEngine;

public class FollowLight : MonoBehaviour
{
    [SerializeField] Transform targetTrf;

    Vector3 offset, followPos;

    void Awake()
    {
        offset = transform.position;
    }

    void FixedUpdate()
    {
        if(targetTrf)
        {
            followPos = (targetTrf.position + offset);
            followPos.x = 0;

            transform.position = followPos;
        }
    }
}
