using System.Collections;
using UnityEngine;

public class ColumnEffect : MonoBehaviour
{

    [SerializeField] float explosionForce = 2000f;
    [SerializeField] float explosionRadius = 1.2f;
    [SerializeField] float releaseTime = 1f;
    [SerializeField] Vector3 effectOffset = Vector3.down;
    
    Rigidbody rb;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Spawn(Column targetColumn, float forceMultiple)
    {
        this.StopAllCoroutines();

        rb.velocity = Vector3.zero;
        transform.position = targetColumn.currentBranch.transform.position;
        transform.rotation = targetColumn.currentBranch.transform.rotation;

        targetColumn.ReleaseBranch();

        ExplosionEffect(targetColumn.transform.position, forceMultiple).Start(this);
    }

    public void Release()
    {
        gameObject.SetActive(false);
    }

    IEnumerator ExplosionEffect(Vector3 explosionPos, float forceMultiple)
    {
        rb.AddExplosionForce(explosionForce * forceMultiple, (explosionPos + effectOffset), explosionRadius);

        yield return new WaitForSeconds(releaseTime);

        // NOTE : 코루틴 액티브 중단
        Release();

        yield break;
    }
}
