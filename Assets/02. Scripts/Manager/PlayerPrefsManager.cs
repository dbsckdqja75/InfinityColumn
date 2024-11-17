using System;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    [SerializeField] bool onStartWithReset = false;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if(onStartWithReset)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public static void SaveData<T>(string key, T value, bool encryption = true)
    {
        string convertValue = value.ToString();
        string saveKey = encryption ? EncryptAES.Encrypt256(key) : key;
        string saveValue = encryption ? EncryptAES.Encrypt256(convertValue) : convertValue;

        PlayerPrefs.SetString(saveKey, saveValue);
    }

    public static T LoadData<T>(string key, T defaultValue = default(T), bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(key) : key;
        if(!PlayerPrefs.HasKey(saveKey))
        {
            return defaultValue;
        }

        string saveValue = PlayerPrefs.GetString(saveKey);
        string dataValue = encryption ? EncryptAES.Decrypt256(saveValue) : saveValue;

        if(dataValue.Length <= 0)
        {
            return defaultValue;
        }

        return (T)Convert.ChangeType(dataValue, typeof(T));
    }

    public static bool HasData(string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(dataKey) : dataKey;
        return PlayerPrefs.HasKey(saveKey);
    }

    public static void DeleteData(string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(dataKey) : dataKey;
        PlayerPrefs.DeleteKey(saveKey);
    }
}
