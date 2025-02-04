using UnityEngine;

public class DisturbCamera : DisturbObject
{
    Transform cameraTrf;

    Animation anim;

    Transform playerTrf, pivotTrf;

    Vector3 lookAtPos;

    void Awake()
    {
        cameraTrf = transform.GetChild(0).transform;

        anim = this.GetComponent<Animation>();
    }

    public override void Init(DisturbManager disturbManager)
    {
        pivotTrf = Camera.main.transform;
        playerTrf = disturbManager.GetPlayerTransform();
    }

    void Update()
    {
        if(pivotTrf != null)
        {
            transform.position = pivotTrf.position;
        }

        if(playerTrf != null)
        {
            lookAtPos = playerTrf.position;
            lookAtPos.x = 0;
            lookAtPos.y = (pivotTrf.position.y - 1.7f);

            cameraTrf.LookAt(lookAtPos);
        }
    }

    public override bool OnTrigger()
    {
        SoundManager.Instance.PlaySound("Camera");

        Destroy(this.gameObject, anim.clip.length);

        return true;
    }
}
