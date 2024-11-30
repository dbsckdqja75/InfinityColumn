using System.Collections.Generic;
using System.Linq;

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    Dictionary<CurrencyType, int> currency = new Dictionary<CurrencyType, int>();

    protected override void Init()
    {
        currency = new Dictionary<CurrencyType, int>();
        currency.Add(CurrencyType.VOXEL_POINT, 0);

        LoadCurrencyData();

        /* [TODO]
         * VoxelPoint (VP)
         * Score 10 -> 1VP
         * Score 100 -> 10VP
         * Score 1,000 -> 100VP
         * Score 10,000 -> 1,000VP
         * 최소 캐릭터 가격 -> 3,000VP
        */
    }

    void LoadCurrencyData()
    {
        CurrencyType[] keyList = currency.Keys.ToArray();
        foreach(CurrencyType currencyType in keyList)
        {
            string key = currencyType.ToString();
            currency[currencyType] = PlayerPrefsManager.LoadData(key, 0);
        }

        CurrencyLabel.refresh?.Invoke();
    }

    void SaveCurrencyData()
    {
        CurrencyType[] keyList = currency.Keys.ToArray();
        foreach(CurrencyType currencyType in keyList)
        {
            string key = currencyType.ToString();
            PlayerPrefsManager.SaveData(key, currency[currencyType]);
        }
    }

    public int GetCurrency(CurrencyType currencyType)
    {
        if(currency.ContainsKey(currencyType))
        {
            // return currency[currencyType];
            // TODO : 추후에 실시간으로 변수 암호화 (구조체에 xorCode 적용?)
            // FIXME : 현재 실제 데이터 수정은 암호화된 저장 데이터를 다시 불러와서 진행함

            string key = currencyType.ToString();
            return PlayerPrefsManager.LoadData(key, 0);
        }

        return 0;
    }

    public bool CanPurchase(CurrencyType currencyType, int price)
    {
        return (GetCurrency(currencyType) >= price);
    }

    public bool Purchase(CurrencyType currencyType, int price)
    {
        int currencyData = GetCurrency(currencyType);
        if(currencyData >= price)
        {
            currencyData -= price;
            currency[currencyType] = currencyData;

            SaveCurrencyData();

            CurrencyLabel.refresh?.Invoke();

            return true;
        }

        return false;
    }

    public void RewardCurrency(CurrencyType currencyType, int amount)
    {
        if(currency.ContainsKey(currencyType))
        {
            int currencyData = GetCurrency(currencyType);
            currencyData += amount;
            currency[currencyType] = currencyData;

            SaveCurrencyData();

            CurrencyLabel.refresh?.Invoke();
        }
    }

    public void UpdateCurrencyData()
    {
        LoadCurrencyData();
    }
}
