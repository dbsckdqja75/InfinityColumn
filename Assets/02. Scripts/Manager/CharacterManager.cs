using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    CharacterID currentCharacterID = CharacterID.BASIC;

    [SerializeField] PlayerController playerController;

    [Space(10)]
    [SerializeField] List<CharacterData> characterData = new List<CharacterData>();
    [SerializeField] List<CharacterID> playerData = new List<CharacterID>();

    Dictionary<CharacterID, CharacterData> characterList = new Dictionary<CharacterID, CharacterData>();

    void Awake()
    {
        Init();
    }

    void Init()
    {
        LoadCharacterData();
        LoadPlayerData();
        
        UpdatePlayerCharacter();
    }

    void LoadCharacterData()
    {
        characterList.Clear();

        foreach(CharacterData data in characterData)
        {
            if(!characterList.ContainsKey(data.GetID()))
            {
                characterList.Add(data.GetID(), data);
            }
        }
    }

    void LoadPlayerData()
    {
        string currentCharacterID = PlayerPrefsManager.LoadData("CurrentCharacterData", "0");
        this.currentCharacterID = (CharacterID)int.Parse(currentCharacterID);

        playerData.Clear();

        string phrase = PlayerPrefsManager.LoadData("PlayerCharacterData", "0");
        string[] splitData = phrase.Split(',');

        foreach(string data in splitData)
        {
            int id;
            if(int.TryParse(data, out id))
            {
                if(!playerData.Contains((CharacterID)id))
                {
                    playerData.Add((CharacterID)id);
                }
            }
        }
    }

    void SavePlayerData()
    {
        string currentCharacterID = ((int)this.currentCharacterID).ToString();
        PlayerPrefsManager.SaveData("CurrentCharacterData", currentCharacterID.ToString());
        
        StringBuilder stringBuilder = new StringBuilder();
        foreach(CharacterID id in playerData)
        {
            stringBuilder.Append(((int)id).ToString());
            if(playerData.Count > 1)
            {
                stringBuilder.Append(',');
            }
        }

        PlayerPrefsManager.SaveData("PlayerCharacterData", stringBuilder.ToString());
    }

    void UpdatePlayerCharacter()
    {
        if(playerController.transform.childCount > 0)
        {
            Destroy(playerController.transform.GetChild(0).gameObject);
        }

        GameObject prefab = characterList[currentCharacterID].GetPrefab();
        PlayerCharacter playerCharacter = Instantiate(prefab, playerController.transform.position, Quaternion.identity, playerController.transform).GetComponent<PlayerCharacter>();
        playerCharacter.ResetPosition();
        playerCharacter.SetHeadTrackTarget(Camera.main.transform);
        playerCharacter.HeadTrackEnable();

        playerController.SetPlayerCharacter(playerCharacter);
    }

    public void ApplyCharacter(CharacterID id)
    {
        currentCharacterID = id;

        if(!playerData.Contains(id))
        {
            RewardCharacter(id);
        }
        else
        {
            SavePlayerData();
        }

        UpdatePlayerCharacter();
    }

    public void RewardCharacter(CharacterID id)
    {
        playerData.Add(id);

        SavePlayerData();
    }

    public CharacterID GetCurrentCharacterID()
    {
        return currentCharacterID;
    }

    public int GetCharacterCount()
    {
        return characterList.Count;
    }

    public CharacterData GetCharacterData(CharacterID id)
    {
        return characterList[id];
    }

    public bool GetCharacterUnlockState(CharacterID id)
    {
        return playerData.Contains(id);
    }

    public List<CharacterID> GetOwnedCharacterData()
    {
        return playerData;
    }

    public void UpdateCharacterData()
    {
        LoadPlayerData();
    }
}
