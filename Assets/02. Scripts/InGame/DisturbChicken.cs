using UnityEngine;

public class DisturbChicken : DisturbObject, IDisturbBranch
{
    [SerializeField] Vector3 offset;

    void OnDisable() 
    {
        Deactivate();    
    }

    public void SetTargetBranch(GameObject branch)
    {
        if(branch)
        {
            transform.SetParent(branch.transform);
            transform.position = branch.transform.position;
        }
    }

    public override bool OnTrigger()
    {
        if(transform.parent)
        {
            bool isLeft = (transform.position.x < 0);
            float angleY = isLeft ? -90 : 90;
            
            Vector3 movePos = offset;
            movePos.x = (isLeft || transform.parent.localRotation.eulerAngles.y > 0) ? -offset.x : offset.x;

            transform.localPosition = movePos;
            transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        return true;
    }
}
