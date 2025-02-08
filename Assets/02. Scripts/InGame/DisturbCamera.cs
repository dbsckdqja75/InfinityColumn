using UnityEngine;

public class DisturbCamera : DisturbObject
{
    Transform cameraTrf; // NOTE : 방해 이벤트 모델 트랜스폼

    Animation anim;

    Transform playerTrf, pivotTrf;

    Vector3 lookAtPos;

    void Awake()
    {
        anim = this.GetComponent<Animation>();

        cameraTrf = transform.GetChild(0).transform;
    }

    public override void Init(DisturbManager disturbManager)
    {
        transform.SetParent(Camera.main.transform);
        transform.localPosition = Vector3.zero;

        pivotTrf = Camera.main.transform;
        playerTrf = disturbManager.GetPlayerTransform();
    }

    void Update()
    {
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
        SoundManager.Instance.PlaySound("Camera", 1f);

        Destroy(this.gameObject, anim.clip.length);

        return true;
    }
}
