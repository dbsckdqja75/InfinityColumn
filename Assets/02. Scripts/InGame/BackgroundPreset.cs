using UnityEngine;

public class BackgroundPreset : MonoBehaviour
{
    [SerializeField] GameObject[] unnecessaryObjects;
    [SerializeField] GameObject[] extraUnnecessaryObjects;

    public void ClearForLowQuality()
    {
        foreach(GameObject obj in unnecessaryObjects)
        {
            Destroy(obj);
        }

        unnecessaryObjects = new GameObject[0];
    }

    public void ClearForVeryLowQuality()
    {
        foreach(GameObject obj in extraUnnecessaryObjects)
        {
            Destroy(obj);
        }

        extraUnnecessaryObjects = new GameObject[0];
    }
}
