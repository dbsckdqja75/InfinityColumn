using UnityEngine;

public class ThreeTimeLogic : GameLogic
{
    const int gameTime = 180;

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
        return "ThreeTimeBestScore";
    }

    public override string GetGameModeStringKey()
    {
        return "GameMode_ThreeTimeAttack";
    }
}
