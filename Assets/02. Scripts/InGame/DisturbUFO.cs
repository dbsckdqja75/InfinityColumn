using UnityEngine;

public class DisturbUFO : DisturbObject
{
    Animation anim;

    Transform ufoTrf;

    Vector3 rotateDir;

    void Awake()
    {
        anim = this.GetComponent<Animation>();

        ufoTrf = transform.GetChild(0).transform;

        rotateDir = RandomExtensions.RandomBool() ? Vector3.up : Vector3.down;
    }

    public override void Init(DisturbManager disturbManager)
    {
        transform.SetParent(Camera.main.transform);
        transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if(ufoTrf != null)
        {
            ufoTrf.Rotate(rotateDir, 360 * Time.smoothDeltaTime);
        }
    }

    public override bool OnTrigger()
    {
        // TODO : UFO 사운드 추가
        // SoundManager.Instance.PlaySound("Camera");

        Destroy(this.gameObject, anim.clip.length);

        return true;
    }
}
