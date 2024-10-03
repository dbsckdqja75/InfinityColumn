using UnityEngine;

[CreateAssetMenu(fileName = "ColumnData", menuName = "ScriptableObject/ColumnData")]
public class ColumnData : ScriptableObject
{
    [SerializeField] GameObject columnPrefab;
    [SerializeField] GameObject effectPrefab;

    public GameObject GetColumnPrefab()
    {
        return columnPrefab;
    }

    public GameObject GetEffectPrefab()
    {
        return effectPrefab;
    }
}
