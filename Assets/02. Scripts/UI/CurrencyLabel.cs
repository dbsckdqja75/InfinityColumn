using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyLabel : MonoBehaviour
{
    [SerializeField] CurrencyType currencyType;

    [SerializeField] TMP_Text labelText;

    [SerializeField] string labelFormat;

    public static Action refresh;

    void Start()
    {
        Init();
    }

    void OnEnable()
    {
        UpdateLabel();
    }

    void Init()
    {
        if (labelText == null)
        {
            this.TryGetComponent<TMP_Text>(out labelText);
        }

        refresh += UpdateLabel;

        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (labelText != null && labelFormat.Length > 0)
        {
            int value = CurrencyManager.Instance.GetCurrency(currencyType);
            labelText.text = string.Format(labelFormat, value);
        }
    }
}
