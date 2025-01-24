using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    bool isControlLock = false;

    [SerializeField] Vector3 startPos;

    [SerializeField] float movePosX;
    [SerializeField] float movePosY;

    [SerializeField] FakeShadow fakeShadow;

    [Header("GameOver Physics Effect")]
    [SerializeField] Vector3 explosionOffset = new Vector3(0.5f, 0.25f, 0);
    [SerializeField] float explosionRadius = 0.5f;
    [SerializeField] float explosionPower = 200;

    [Space(10)]
    [SerializeField] InputController inputController;

    PlayerCharacter playerCharacter;

    Rigidbody rb;

    UnityEvent onMoveEvent = new UnityEvent();

    public void Init()
    {
        isControlLock = true;

        rb = this.GetComponent<Rigidbody>();

        inputController.AddTouchEvent(OnMoveInput);
    }

    void Move(bool isLeft)
    {
        if(isControlLock == false)
        {
            Vector2 nowPos = transform.position;
            Vector3 movePos = new Vector3(0, nowPos.y + movePosY, 0);
            movePos.x = (isLeft ? -movePosX : movePosX);

            if(nowPos.y >= GameManager.PLAYABLE_HEIGHT_LIMIT)
            {
                movePos.y = nowPos.y;
            }

            transform.position = movePos;

            if(playerCharacter != null)
            {
                playerCharacter.UpdatePosition(isLeft);
                playerCharacter.MoveMotion(isLeft);
            }

            SoundManager.Instance.PlaySound("Hit1");

            onMoveEvent?.Invoke();
        }
    }

    void OnMoveInput(Vector3 iPosition)
    {
        float halfScreenWidth = (Screen.width / 2f);
        
        Move(iPosition.x < halfScreenWidth);
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            Move(true);
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            Move(false);
        }
    }

    public void SetPlayerCharacter(PlayerCharacter newPlayerCharacter)
    {
        playerCharacter = newPlayerCharacter;

        fakeShadow.SetTarget(playerCharacter.transform);
    }

    public void ResetPosition()
    {
        rb.isKinematic = true;

        startPos.x = (RandomExtensions.RandomBool() ? movePosX : -movePosX);

        transform.position = startPos;
        transform.rotation = Quaternion.identity;

        if(playerCharacter != null)
        {
            playerCharacter.ResetPosition();
            playerCharacter.ResetMotion();
            playerCharacter.EnableHeadTracking(true);
        }
    }

    public void OnFever(bool isFever)
    {
        if(playerCharacter != null)
        {
            if(isFever)
            {
                playerCharacter.FeverEyeMotion();
            }
            else
            {
                playerCharacter.ResetEyeMotion();
            }
        }
    }

    public void OnFall()
    {
        rb.isKinematic = false;

        Vector3 offset = explosionOffset;
        offset.x = (Mathf.Sign(transform.position.x) > 0 ? -offset.x : offset.x);

        rb.AddExplosionForce(explosionPower, transform.position + offset, explosionRadius);

        if(playerCharacter != null)
        {
            playerCharacter.FallMotion();
            playerCharacter.EnableHeadTracking(false);
        }

        SoundManager.Instance.PlaySound("BuzzError1");
    }

    public int GetDirectionType()
    {
        return (transform.position.x < 0) ? 0 : 1;
    }

    public float GetPlayerHeight()
    {
        return transform.position.y;
    }

    public void SetControlLock(bool isLock)
    {
        isControlLock = isLock;
    }

    public void AddMoveEvent(UnityAction action)
    {
        onMoveEvent.AddListener(action);
    }

    #if UNITY_EDITOR
    public void DebugMove(bool isLeft)
    {
        Move(isLeft);
    }
    #endif
}
