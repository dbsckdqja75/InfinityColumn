using UnityEngine;

public class InfinityLogic : GameLogic
{
    const int maxHealth = 100;
    const float minDamage = 12.5f;
    const float maxDamage = 30;
    float damage = 12.5f;

    public override void OnChangeGameMode(PlayUI playUI)
    {
        playUI.ShowFeverGauge();
        playUI.HideHealthTimer();
    }

    public override void OnGameReset()
    {
        damage = minDamage;
    }

    public override int GetMaxHealth()
    {
        return maxHealth;
    }

    public void AddTimePerDamage()
    {
        damage = Mathf.Clamp(damage + 0.00006f, minDamage, maxDamage);
    }

    public float GetConfigDamage()
    {
        return minDamage;
    }

    public override float GetTimePerDamage()
    {
        return damage;
    }

    public override string GetBestScoreDataKey()
    {
        return "BestScore";
    }

    public override string GetGameModeStringKey()
    {
        return "GameMode_Infinity";
    }
}
