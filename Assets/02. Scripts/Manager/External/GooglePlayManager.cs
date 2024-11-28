using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GooglePlayManager : MonoBehaviour
{
    [SerializeField] GameObject testObj;

    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        SignIn();
    }

    void Update()
    {
        
    }

    void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ShowAchievementUI()
	{
		PlayGamesPlatform.Instance.ShowAchievementsUI();
	}

    public void ShowLeaderboardUI()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI();
	}

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            testObj.SetActive(true);

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

            Debug.Log("Success \n" + name);
        }
        else
        {
            Debug.Log("Sign in Failed!");
        }
    }
}
