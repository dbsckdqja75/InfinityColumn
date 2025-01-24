using UnityEngine;

public class InfinityGame : MainGame
{
    bool isFeverTime = false;
    float feverTimer = 0;
    float feverTimeRate = 0f;
    SecureValue<int> feverCharger = new SecureValue<int>(0);

    float currentDamage = 12.5f;

    // NOTE : 기본 설정
    const int maxHealth = 100;
    const float regainHealth = 4;
    const float minDamage = 20, maxDamage = 30;
    const float damageStep = 0.0001f;

    // NOTE : 방해 이벤트 관련 설정
    const int disturbMinScoreStep = 50;
    const int disturbMaxScoreStep = 60;

    // NOTE : 점수 보상 설정 (VP)
    const int rewardVP = 1; // NOTE : 기준당 보상 (VP)
    const int scoreRewardStep = 100; // NOTE : 1VP 보상 기준 (점수)

    // NOTE : 피버타임 설정
    const float maxFeverTime = 100;
    const float regainFeverTime = 1;
    const float feverDuration = 5f;

    const int feverChargeStep = 2;
    const int feverChargeAmount = 1;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        feverTimeRate = maxFeverTime * (1 / feverDuration);

        disturbManager.ChangeScoreStep(disturbMinScoreStep, disturbMaxScoreStep);

        playUI.ShowFeverGauge();
        playUI.HideHealthTimer();
    }

    public override void UpdateLogic()
    {
        if(IsCurrentState(GameState.PLAYING))
        {
            UpdateHealth();
            UpdateFeverTime();
        }
    }

    protected override void UpdateHealth()
    {
        base.UpdateHealth();

        if(health > 0)
        {
            health -= (currentDamage * Time.deltaTime);
            health = Mathf.Clamp(health, 0f, maxHealth);
        }

        playUI.UpdateHealth(health / maxHealth);
    }

    void UpdateFeverTime()
    {
        if(isFeverTime)
        {
            health = maxHealth;

            if(feverTimer > 0f)
            {
                feverTimer = Mathf.Clamp(feverTimer, 0f, maxFeverTime);
                feverTimer -= (feverTimeRate * Time.deltaTime);
            }
            else
            {
                ResetFeverTime();
            }
        }

        playUI.UpdateFever(feverTimer / maxFeverTime);
    }

    public override void OnGameOver()
    {
        base.OnGameOver();

        RewardVP(score.GetValue(), scoreRewardStep);
    }

    public override void OnGameReset()
    {
        base.OnGameReset();

        health = maxHealth;

        currentDamage = minDamage;

        isFeverTime = false;
        feverTimer = 0;
    }

    public override void OnGameResume()
    {
        base.OnGameResume();

        if(isFeverTime)
        {
            SoundManager.Instance.PlayMusic("Infinity Fever Music", false);
        }
        else
        {
            SoundManager.Instance.PlayMusic("Infinity Music");
        }
    }

    protected override void CheckBranch()
    {
        Column column = spawnManager.GetNextColumn();
        if(column.HasBranch(playerController.GetDirectionType()))
        {
            if(isFeverTime)
            {
                spawnManager.SpawnEffect(column, 3);
            }
            else
            {
                spawnManager.SpawnEffect(column);

                gameManager.GameOver();
            }
        }
    }

    void RegainHealth()
    {
        health = Mathf.Clamp(health + regainHealth, 0, maxHealth);
    }

    void AddDamage()
    {
        currentDamage = Mathf.Clamp(currentDamage + damageStep, minDamage, maxDamage);
    }

    void ChargeFever()
    {
        if(isFeverTime == false)
        {
            feverCharger.SetValue(feverCharger.GetValue() + feverChargeAmount);

            if(feverCharger.GetValue() >= feverChargeStep)
            {
                feverTimer = Mathf.Clamp(feverTimer + regainFeverTime, 0, maxFeverTime);
                feverCharger.SetValue(0);

                if(feverTimer >= maxFeverTime)
                {
                    OnFeverTime();
                }
            }
        }
    }

    void OnFeverTime()
    {
        isFeverTime = true;

        feverTimer = maxFeverTime;

        playerController.OnFever(true);

        spawnManager.FeverColumn(10, 3f);

        playUI.OnFeverTime();

        cameraView.SetCameraPreset("FeverTime");
        SoundManager.Instance.PlayMusic("Infinity Fever Music", false);
    }

    void ResetFeverTime()
    {
        isFeverTime = false;

        feverTimer = 0;
        feverCharger.SetValue(0);

        playerController.OnFever(false);

        playUI.OnResetFeverTime();

        if(IsCurrentState(GameState.PLAYING))
        {
            cameraView.SetCameraPreset("Playing");
            SoundManager.Instance.PlayMusic("Infinity Music", false);
        }
    }

    public override void OnPlayerMoved()
    {
        base.OnPlayerMoved();

        // NOTE : 다음 기둥 가지에 걸리면 게임 오버라 진행되지 않음
        if(IsCurrentState(GameState.PLAYING))
        {
            EarnScore();
            RegainHealth();
            ChargeFever();
            AddDamage();

            // if(!isFeverTime)
            // {
            //     disturbManager.UpdateTrigger(score.GetValue());
            // }

            // TODO : 피버타임이 아닐때도 이벤트 발생 (테스트 필요)
            disturbManager.UpdateTrigger(score.GetValue());
            
            spawnManager.UpdateColumnMode(score.GetValue() >= GameManager.PLAYABLE_HEIGHT_LIMIT);

            skyBoxManager.UpdateHeight(score.GetValue());

            if(score.GetValue() > GameManager.PLAYABLE_HEIGHT_LIMIT)
            {
                cameraView.SetFakeView();
            }
        }
    }

    #if UNITY_EDITOR
    public void DebugFeverTime()
    {
        OnFeverTime();
    }
    #endif
}
