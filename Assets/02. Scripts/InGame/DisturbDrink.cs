using System.Collections;
using UnityEngine;

public class DisturbDrink : DisturbObject
{
    [SerializeField] float dropDistance = 3.5f;
    [SerializeField] float dropSpeed = 0.7f;
    [SerializeField] Vector3 offset;

    Transform playerTrf;

    public override void Init(DisturbManager disturbManager)
    {
        SpawnManager spawnManager = disturbManager.GetSpawnManager();
        GameObject branch = spawnManager.GetLastColumnBranch();
        SetTargetBranch(branch);

        playerTrf = disturbManager.GetPlayerTransform();
    }

    void Update()
    {
        if(playerTrf != null)
        {
            float curDis = Vector3.Distance(transform.position, playerTrf.position);
            if(curDis <= dropDistance)
            {
                OnDrop();
            }
        }
    }

    void OnDisable()
    {
        Deactivate();
    }

    void SetTargetBranch(GameObject branch)
    {
        if(branch != null)
        {
            transform.SetParent(branch.transform);
            transform.localPosition = Vector3.zero;
        }
    }

    public override bool OnTrigger()
    {
        if(transform.parent)
        {
            bool isLeft = (transform.position.x < 0);
            
            Vector3 movePos = offset;
            movePos.x = isLeft ? -offset.x : offset.x;

            transform.localPosition = movePos;
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        return true;
    }

    public void OnDrop()
    {
        playerTrf = null;

        DropMotion().Start(this);
    }

    IEnumerator DropMotion()
    {
        // isDrop = true;

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = new Vector3(0, -8, -2);
        Vector3 height = new Vector3(0, 1, 0);

        SoundManager.Instance.PlaySound("Coke", transform.position);

        yield return CoroutineExtensions.ProcessAction(dropSpeed, (t) =>
        {
            transform.BezierCurvePosition(startPos, endPos, height, t);

            if(Mathf.Abs(transform.rotation.x) < 0.7f)
            {
                transform.Rotate(Vector3.left * 360 * Time.smoothDeltaTime);
            }
        });

        Deactivate();

        yield break;
    }
}
