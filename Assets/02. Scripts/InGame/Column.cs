using UnityEngine;

public class Column : MonoBehaviour
{
    [SerializeField] GameObject[] branchs;

    public GameObject currentBranch { get; private set; }

    public void Init(int direction = -1)
    {
        direction = Mathf.Clamp(direction, -1, branchs.Length-1);

        ReleaseBranch();

        foreach(GameObject branch in branchs)
        {
            branch.SetActive(false);
        }

        if(direction != -1)
        {
            branchs[direction].SetActive(true);
            currentBranch = branchs[direction];
        }
    }

    // NOTE : 해당 방향의 가지 존재 여부 체크
    public bool HasBranch(int direction = -1)
    {
        direction = Mathf.Clamp(direction, -1, branchs.Length-1);

        return (direction != -1) ? branchs[direction].activeSelf : false;
    }

    public GameObject GetCurrentBranch()
    {
        foreach(GameObject branch in branchs)
        {
            if(branch.activeSelf)
            {
                return branch;
            }
        }

        return null;
    }

    public void ReleaseBranch()
    {
        currentBranch?.SetActive(false);
        currentBranch = null;
    }
}
