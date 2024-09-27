using UnityEngine;

public class Column : MonoBehaviour
{

    [SerializeField] GameObject effectPrefab;
    [SerializeField] GameObject[] branchs;

    public GameObject currentBranch { get; private set; }

    public void Init(int type = -1)
    {
        type = Mathf.Clamp(type, -1, branchs.Length-1);

        ReleaseBranch();

        foreach(GameObject branch in branchs)
        {
            branch.SetActive(false);
        }

        if(type != -1)
        {
            branchs[type].SetActive(true);
            currentBranch = branchs[type];
        }
    }

    // NOTE : 해당 방향의 가지 존재 여부 체크
    public bool IsHaveBranch(int type = -1)
    {
        type = Mathf.Clamp(type, -1, branchs.Length-1);

        return (type != -1) ? branchs[type].activeSelf : false;
    }

    public void ReleaseBranch()
    {
        currentBranch?.SetActive(false);
        currentBranch = null;
    }
}
