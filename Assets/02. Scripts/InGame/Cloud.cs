using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] GameObject[] cloudPresets;

    void Awake() 
    {
        RandomizeCloud();
    }

    public void HideCloud()
    {
        foreach(GameObject cloud in cloudPresets)
        {
            cloud.SetActive(false);
        }
    }

    public void RandomizeCloud()
    {
        int presetIdx = Random.Range(0, cloudPresets.Length);
        HideCloud();

        cloudPresets[presetIdx].SetActive(true);
    }
}
