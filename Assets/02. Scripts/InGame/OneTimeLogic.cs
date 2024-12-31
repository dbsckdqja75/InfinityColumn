using UnityEngine;

public class OneTimeLogic : GameLogic
{
    const int gameTime = 60;

    public override void OnChangeGameMode(PlayUI playUI)
    {
        playUI.ShowHealthTimer();
        playUI.HideFeverGauge();
    }

    public override void OnGameReset()
    {
        
    }

    public override int GetMaxHealth()
    {
        return gameTime;
    }

    public override float GetTimePerDamage()
    {
        return Time.timeScale;
    }

    public override string GetBestScoreDataKey()
    {
        return "OneTimeBestScore";
    }

    public override string GetGameModeStringKey()
    {
        return "GameMode_OneTimeAttack";
    }
}
