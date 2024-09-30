using System.Collections;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] Vector3 offsetPosition, offsetRotation;

    [SerializeField] int idleEventCount;

    [SerializeField] CharacterFace characterFace;

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
        this.StopAllCoroutines();    
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
        movePos.x = -offsetPosition.x;

        transform.localPosition = movePos;
        transform.localRotation = Quaternion.Euler(offsetRotation);
    }

    public void ResetMotion()
    {
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

    public void IdleMotion(int motionIdx)
    {
        if(animator.GetInteger("MoveDirection") < 0 && !animator.GetBool("isFall"))
        {
            animator.ResetTrigger("Event");

            animator.SetInteger("EventIdx", motionIdx);
            animator.SetTrigger("Event");
        }
    }

    public void RandomIdleMotion()
    {
        int eventMotionIdx = Random.Range(0, idleEventCount+1);
        IdleMotion(eventMotionIdx);
    }

    IEnumerator RandomMotionLoop()
    {
        while(true)
        {
            float eventTime = Random.Range(10f, 60f);
            yield return new WaitForSeconds(eventTime);

            RandomIdleMotion();
            
            yield return new WaitForEndOfFrame();
        }
    }
}
