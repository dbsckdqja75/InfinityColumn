using UnityEngine;

public abstract class GameLogic
{
    public abstract void OnChangeGameMode(PlayUI playUI);
    public abstract void OnGameReset();

    public abstract int GetMaxHealth();
    public abstract float GetTimePerDamage();
    public abstract string GetBestScoreDataKey();
    public abstract string GetGameModeStringKey();
}
