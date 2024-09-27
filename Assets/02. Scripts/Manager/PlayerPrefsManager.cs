using System;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{

    [SerializeField] bool onStartWithReset = false;

    [SerializeField] string configKey;
    const string salt = "build2002";
    static string resultKey;

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

        resultKey = (configKey + salt);
        configKey = "";
    }

    public static void SaveData<T>(string key, T value, bool encryption = true)
    {
        string convertValue = value.ToString();
        string saveKey = encryption ? EncryptAES.Encrypt256(key, resultKey) : key;
        string saveValue = encryption ? EncryptAES.Encrypt256(convertValue, resultKey) : convertValue;

        PlayerPrefs.SetString(saveKey, saveValue);
    }

    public static T LoadData<T>(string key, T defaultValue = default(T), bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(key, resultKey) : key;
        if(!PlayerPrefs.HasKey(saveKey))
        {
            return defaultValue;
        }

        string saveValue = PlayerPrefs.GetString(saveKey);
        string dataValue = encryption ? EncryptAES.Decrypt256(saveValue, resultKey) : saveValue;

        if(dataValue.Length <= 0)
        {
            return defaultValue;
        }

        return (T)Convert.ChangeType(dataValue, typeof(T));
    }

    public static bool HasData(string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(dataKey, resultKey) : dataKey;
        return PlayerPrefs.HasKey(saveKey);
    }

    public static void DeleteData(string dataKey, bool encryption = true)
    {
        string saveKey = encryption ? EncryptAES.Encrypt256(dataKey, resultKey) : dataKey;
        PlayerPrefs.DeleteKey(saveKey);
    }
}
