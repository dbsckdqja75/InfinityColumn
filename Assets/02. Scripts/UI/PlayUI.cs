using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float healthFillSpeed = 15f;
    [SerializeField] float feverFillSpeed = 5f;

    [Space(10)]
    [SerializeField] float healthAlertAmount = 0.4f; // NOTE : 체력 경고 이펙트 발생 시점 (40%)

    [Header("Health")]
    [SerializeField] Image healthBarImage;
    [SerializeField] ImageColor healthBarColor; // NOTE : Normal Color (0) | Warning Color (1) | FeverTime Color (2)
    [SerializeField] TMP_Text healthTimerText;
    [SerializeField] GameObject infinityTextObj; 

    [Header("Fever")]
    [SerializeField] Image feverBarImage;
    [SerializeField] ImageColor feverBarColor; // NOTE : Normal Color (0) | FeverTime Color (1)
    [SerializeField] GameObject feverGaugeObj;

    [Header("Score")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Animation scoreTextAnim;

    [Header("Effect")]
    [SerializeField] GameObject warningEffectObj;
    [SerializeField] GameObject feverEffectObj;

    // NOTE : 게임 시작 가이드 (터치 또는 키보드 조작)
    [Header("Guide")]
    [SerializeField] GameObject startGuideObj; // TOOD : PC 플랫폼에 따른 대응 필요

    public void UpdateHealth(float healthAmount)
    {
        healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, healthAmount, healthFillSpeed * Time.deltaTime);

        warningEffectObj.SetActive(healthAmount <= healthAlertAmount);

        if(feverEffectObj.activeSelf == false)
        {
            healthBarColor.SetColorPreset(warningEffectObj.activeSelf ? 1 : 0);
        }
    }

    public void UpdateHealthTimer(int health)
    {
        healthTimerText.text = health.ToString();
    }

    public void UpdateFever(float feverAmount)
    {
        feverBarImage.fillAmount = Mathf.Lerp(feverBarImage.fillAmount, feverAmount, feverFillSpeed * Time.deltaTime);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();

        scoreTextAnim.Stop();
        scoreTextAnim.Play();
    }

    public void OnFeverTime()
    {
        feverBarImage.fillAmount = 1;

        healthBarColor.SetColorPreset(2); // NOTE : FeverTime Color
        feverBarColor.SetColorPreset(1); // NOTE : FeverTime Color

        scoreText.gameObject.SetActive(false);
        infinityTextObj.SetActive(true);

        feverEffectObj.SetActive(true);
        warningEffectObj.SetActive(false);
    }

    public void OnResetFeverTime()
    {
        healthBarColor.ResetColor();
        feverBarColor.ResetColor();

        infinityTextObj.SetActive(false);
        scoreText.gameObject.SetActive(true);

        feverEffectObj.SetActive(false);
    }

    public void ResetUI()
    {
        healthBarImage.fillAmount = 1;
        feverBarImage.fillAmount = 0;
        
        scoreText.text = "0";

        infinityTextObj.SetActive(false);
        warningEffectObj.SetActive(false);

        startGuideObj.SetActive(true);
    }

    public void ShowHealthTimer()
    {
        healthTimerText.gameObject.SetActive(true);
    }

    public void HideHealthTimer()
    {
        healthTimerText.gameObject.SetActive(false);
    }

    public void ShowFeverGauge()
    {
        feverGaugeObj.SetActive(true);
    }

    public void HideFeverGauge()
    {
        feverGaugeObj.SetActive(false);
    }

    public void HideStartGuide()
    {
        startGuideObj.SetActive(false);
    }
}
