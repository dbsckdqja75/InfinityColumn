using UnityEngine;

public class BackgroundPreset : MonoBehaviour
{
    [SerializeField] GameObject[] unnecessaryObjects;

    public void ClearForLowQuality()
    {
        foreach(GameObject obj in unnecessaryObjects)
        {
            obj.SetActive(false);
        }
    }

    public void RestoreForHighQuality()
    {
        foreach(GameObject obj in unnecessaryObjects)
        {
            obj.SetActive(true);
        }
    }
}
