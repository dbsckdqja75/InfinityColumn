using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    bool moveLock = false;

    [SerializeField] Vector3 startPos;

    [SerializeField] float movePosX;
    [SerializeField] float movePosY;

    [SerializeField] InputController inputController;
    [SerializeField] FakeShadow fakeShadow;

    [Header("GameOver Effect")]
    [SerializeField] Vector3 explosionOffset = new Vector3(0.5f, 0.25f, 0);
    [SerializeField] float explosionRadius = 0.5f;
    [SerializeField] float explosionPower = 200;

    PlayerCharacter playerCharacter;

    Rigidbody rb;

    [HideInInspector] public UnityEvent onMoveEvent;

    public void Init()
    {
        rb = this.GetComponent<Rigidbody>();

        inputController.AddTouchEvent(OnTouch);
    }

    void OnTouch(Vector3 iPosition)
    {
        float halfScreenWidth = (Screen.width / 2f);
        Move(iPosition.x < halfScreenWidth);
    }

    public void LeftMove(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            Move(true);
        }
    }

    public void RightMove(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            Move(false);
        }
    }

    void Move(bool isLeft)
    {
        if(moveLock)
            return;

        Vector2 nowPos = transform.position;
        Vector3 movePos = new Vector3(0, nowPos.y + movePosY, 0);
        movePos.x = (isLeft ? -movePosX : movePosX);

        if(nowPos.y >= GameManager.HEIGHT_LIMIT)
        {
            movePos.y = nowPos.y;
        }

        transform.position = movePos;

        if(playerCharacter)
        {
            playerCharacter.UpdatePosition(isLeft);
            playerCharacter.MoveMotion(isLeft);
        }

        SoundManager.Instance.PlaySound("Hit1");

        onMoveEvent?.Invoke();
    }

    public void SetPlayerCharacter(PlayerCharacter _playerCharacter)
    {
        playerCharacter = _playerCharacter;

        fakeShadow.SetTarget(playerCharacter.transform);
    }

    public void ResetPosition()
    {
        rb.isKinematic = true;

        startPos.x = (RandomExtensions.RandomBool() ? movePosX : -movePosX);

        transform.position = startPos;
        transform.rotation = Quaternion.identity;

        if(playerCharacter)
        {
            playerCharacter.ResetPosition();
            playerCharacter.ResetMotion();
            playerCharacter.HeadTrackEnable();
        }
    }

    public void Fall()
    {
        rb.isKinematic = false;

        Vector3 offset = explosionOffset;
        offset.x = (Mathf.Sign(transform.position.x) > 0 ? -offset.x : offset.x);

        rb.AddExplosionForce(explosionPower, transform.position + offset, explosionRadius);

        if(playerCharacter)
        {
            playerCharacter.FallMotion();
            playerCharacter.HeadTrackDisable();
        }

        SoundManager.Instance.PlaySound("BuzzError1");
    }

    public void Fever(bool isFever)
    {
        if(isFever)
        {
            playerCharacter?.FeverEyeMotion();
        }
        else
        {
            playerCharacter?.ResetEyeMotion();
        }
    }

    public int GetDirectionType()
    {
        return (transform.position.x < 0) ? 0 : 1;
    }

    public float GetPlayerHeight()
    {
        return transform.position.y;
    }

    public void SetMoveLock(bool isLock)
    {
        moveLock = isLock;
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
