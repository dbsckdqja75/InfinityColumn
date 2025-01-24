using System.Collections;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] Vector3 offsetPosition, offsetRotation;

    [Space(10)]
    [SerializeField] int idleEventCount;

    [SerializeField] CharacterFace characterFace;
    [SerializeField] HeadTracker headTracker;

    Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void OnEnable() 
    {
        if(idleEventCount > 0)
        {
            RandomMotionLoop().Start(this);
        }
    }

    void OnDisable() 
    {
        RandomMotionLoop().Stop(this);
    }

    public void UpdatePosition(bool isLeft)
    {
        Vector3 movePos = offsetPosition;
        movePos.x = (isLeft ? -offsetPosition.x : offsetPosition.x);

        transform.localPosition = movePos;
    }

    public void ResetPosition()
    {
        Vector3 movePos = offsetPosition;
        movePos.x = (transform.parent.position.x > 0 ? offsetPosition.x : -offsetPosition.x);

        transform.localPosition = movePos;
        transform.localRotation = Quaternion.Euler(offsetRotation);
    }

    public void ResetMotion()
    {
        FaceMotion(0);
        
        if(animator)
        {
            animator.ResetTrigger("Event");
            animator.ResetTrigger("Move");

            animator.SetBool("isFall", false);
            animator.SetInteger("MoveDirection", -1);
            animator.SetTrigger("Move");
        }
    }

    public void MoveMotion(bool isLeft)
    {
        if(animator)
        {
            animator.ResetTrigger("Event");
            animator.ResetTrigger("Move");

            animator.SetInteger("MoveDirection", isLeft ? 0 : 1);
            animator.SetTrigger("Move");
        }
    }

    public void FallMotion()
    {
        FaceMotion(3);

        if(animator)
        {
            animator.ResetTrigger("Event");
            animator.ResetTrigger("Move");

            animator.SetBool("isFall", true);
            animator.SetInteger("MoveDirection", -1);
            animator.SetTrigger("Move");
        }
    }

    public void FaceMotion(int faceIdx)
    {
        if(characterFace)
        {
            characterFace.SetFace(faceIdx);
        }
    }

    public void FeverEyeMotion()
    {
        if(characterFace)
        {
            characterFace.ChangeFeverEyes();
        }
    }

    public void ResetEyeMotion()
    {
        if(characterFace)
        {
            characterFace.ResetEyes();
        }
    }

    public void SetHeadTrackTarget(Transform targetTrf)
    {
        if(headTracker)
        {
            headTracker.SetTarget(targetTrf);
        }
    }

    public void EnableHeadTracking(bool isOn)
    {
        if(headTracker)
        {
            headTracker.SetActive(isOn);
        }
    }

    public void EventMotion(int motionIdx)
    {
        if(animator.GetInteger("MoveDirection") < 0 && !animator.GetBool("isFall"))
        {
            animator.ResetTrigger("Event");

            if(motionIdx > 0)
            {
                animator.SetInteger("EventIdx", motionIdx);
                animator.SetTrigger("Event");
            }
        }
    }

    public void RandomEventMotion()
    {
        int eventMotionIdx = Random.Range(0, idleEventCount+1);
        EventMotion(eventMotionIdx);
    }

    IEnumerator RandomMotionLoop()
    {
        while(true)
        {
            float eventTime = Random.Range(10f, 60f);
            yield return new WaitForSeconds(eventTime);

            RandomEventMotion();
            
            yield return new WaitForEndOfFrame();
        }
    }
}
