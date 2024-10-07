using UnityEngine;

public class Cloud : MonoBehaviour
{

    [SerializeField] GameObject[] cloudPresets;

    void Awake() 
    {
        RandomizeCloud();
    }

    public void RandomizeCloud()
    {
        int presetIdx = Random.Range(0, cloudPresets.Length);
        foreach(GameObject cloud in cloudPresets)
        {
            cloud.SetActive(false);
        }

        cloudPresets[presetIdx].SetActive(true);
    }
}
