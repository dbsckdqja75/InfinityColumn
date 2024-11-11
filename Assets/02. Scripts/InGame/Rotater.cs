using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField] float speed = 360;
    [SerializeField] Vector3 rotateDirection = Vector3.forward;

    void FixedUpdate() 
    {
        transform.Rotate(rotateDirection * speed * Time.smoothDeltaTime, Space.Self);
    }
}
