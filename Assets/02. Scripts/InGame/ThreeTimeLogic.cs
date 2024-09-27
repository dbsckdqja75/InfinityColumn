using UnityEngine;

public class ThreeTimeLogic : GameLogic
{
    const int gameTime = 180;

    public override void OnChangeGameMode(GameManager manager)
    {
        manager.HideFeverUI();
        manager.ExposeHealthTimeUI();
        manager.UpdateGameModeText("GameMode_ThreeTimeAttack");
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
}
