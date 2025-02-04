using UnityEngine;

public class DisturbDrinkStand : DisturbObject
{
    [SerializeField] GameObject effectPrefab;
    [SerializeField] string soundName = "Coke";
    [SerializeField] Vector3 offset;

    public override void Init(DisturbManager disturbManager)
    {
        SpawnManager spawnManager = disturbManager.GetSpawnManager();
        GameObject branch = spawnManager.GetLastColumnBranch();
        SetTargetBranch(branch);
    }

    void OnDisable()
    {
        if(transform.parent == null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
            
            SoundManager.Instance.PlaySound(soundName, transform.position);
        }

        Deactivate();
    }

    void SetTargetBranch(GameObject branch)
    {
        if(branch != null)
        {
            transform.SetParent(branch.transform);
            transform.localPosition = Vector3.zero;
        }
    }

    public override bool OnTrigger()
    {
        if(transform.parent)
        {
            bool isLeft = (transform.position.x < 0);
            
            Vector3 movePos = offset;
            movePos.x = isLeft ? -offset.x : offset.x;

            transform.localPosition = movePos;
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        return true;
    }
}
