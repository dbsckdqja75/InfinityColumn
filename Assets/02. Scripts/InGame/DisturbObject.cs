using UnityEngine;

public abstract class DisturbObject : MonoBehaviour
{
    public virtual void Init(DisturbManager disturbManager)
    {
        
    }

    public abstract bool OnTrigger();

    public virtual void Deactivate()
    {
        this.StopAllCoroutines();

        Destroy(this.gameObject);
    }
}
