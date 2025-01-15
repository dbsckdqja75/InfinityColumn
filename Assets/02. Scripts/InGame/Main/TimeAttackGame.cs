using UnityEngine;

public class TimeAttackGame : MainGame
{
    [SerializeField] float gameTime = 60;

    const int rewardVP = 1; // NOTE : 기준당 보상 (VP)
    const int scoreRewardStep = 100; // NOTE : 1VP 보상 기준 (점수)

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        playUI.ShowHealthTimer();
        playUI.HideFeverGauge();
    }

    public override void UpdateLogic()
    {
        if(IsCurrentState(GameState.PLAYING))
        {
            UpdateHealth();
        }
    }

    protected override void UpdateHealth()
    {
        base.UpdateHealth();

        if(health > 0)
        {
            health -= Time.deltaTime;
            health = Mathf.Clamp(health, 0f, gameTime);
        }

        playUI.UpdateHealth(health / gameTime);
        playUI.UpdateHealthTimer((int)health);
    }

    public override void OnGameOver()
    {
        base.OnGameOver();

        RewardVP(score.GetValue(), scoreRewardStep);
    }

    public override void OnGameReset()
    {
        base.OnGameReset();

        health = (gameTime+1);

        playUI.UpdateHealthTimer((int)gameTime);
    }

    public override void OnGameResume()
    {
        base.OnGameResume();

        SoundManager.Instance.PlayMusic("Infinity Music");
    }

    protected override void CheckBranch()
    {
        Column column = spawnManager.GetNextColumn();
        if(column.HasBranch(playerController.GetDirectionType()))
        {
            spawnManager.SpawnEffect(column);

            gameManager.GameOver();
        }
    }



    public override void OnPlayerMoved()
    {
        base.OnPlayerMoved();

        // NOTE : 다음 기둥 가지에 걸리면 게임 오버라 진행되지 않음
        if(IsCurrentState(GameState.PLAYING))
        {
            EarnScore();

            disturbManager.UpdateTrigger(score.GetValue());
            
            spawnManager.UpdateColumnMode(score.GetValue() >= GameManager.PLAYABLE_HEIGHT_LIMIT);

            skyBoxManager.UpdateHeight(score.GetValue());

            if(score.GetValue() > GameManager.PLAYABLE_HEIGHT_LIMIT)
            {
                cameraView.SetFakeView();
            }
        }
    }
}
