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
            RandomIdleMotion().Start(this);
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

            animator.SetInteger("MoveDirection", -1);
            animator.SetBool("isFall", false);
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

            animator.SetInteger("MoveDirection", -1);
            animator.SetBool("isFall", true);
        }
    }

    public void FaceMotion(int faceIdx)
    {
        if(characterFace)
        {
            characterFace.SetFace(faceIdx);
        }
    }

    IEnumerator RandomIdleMotion()
    {
        while(true)
        {
            float eventTime = Random.Range(10f, 60f);
            yield return new WaitForSeconds(eventTime);

            if(animator.GetInteger("MoveDirection") < 0 && !animator.GetBool("isFall"))
            {
                int eventIdx = Random.Range(0, idleEventCount+1);
                animator.SetInteger("EventIdx", eventIdx);
                animator.SetTrigger("Event");
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}
