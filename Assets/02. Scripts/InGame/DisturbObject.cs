using UnityEngine;

public abstract class DisturbObject : MonoBehaviour
{
    public virtual void Deactivate()
    {
        this.StopAllCoroutines();

        Destroy(this.gameObject);
    }

    public abstract bool OnTrigger();
}
