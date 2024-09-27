using UnityEngine;

public class OneTimeLogic : GameLogic
{
    const int gameTime = 60;

    public override void OnChangeGameMode(GameManager manager)
    {
        manager.HideFeverUI();
        manager.ExposeHealthTimeUI();
        manager.UpdateGameModeText("GameMode_OneTimeAttack");
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
}
