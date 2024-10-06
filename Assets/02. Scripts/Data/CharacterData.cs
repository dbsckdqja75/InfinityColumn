using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField] CharacterID id;
    [SerializeField] string characterLocaleKey;
    [SerializeField] int price;
    [SerializeField] float previewScale = 1;
    [SerializeField] GameObject prefab;

    public CharacterID GetID()
    {
        return id;
    }

    public string GetNameLocaleKey()
    {
        return characterLocaleKey;
    }

    public int GetPrice()
    {
        return price;
    }

    public float GetPreviewScale()
    {
        return previewScale;
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }
}
