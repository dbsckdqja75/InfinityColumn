using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] Vector3 offsetPosition, offsetRotation;

    Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
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
            animator.SetBool("isFall", false);
            animator.SetTrigger("Idle");
        }
    }

    public void MoveMotion(bool isLeft)
    {
        if(animator)
        {
            animator.SetTrigger(isLeft ? "LeftMove" : "RightMove");
        }
    }

    public void FallMotion()
    {
        if(animator)
        {
            animator.SetBool("isFall", true);
        }
    }
}
