using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_IOS
using Apple.GameKit;
using Apple.GameKit.Leaderboards;
#endif

public class AppleGameCenterManager : MonoBehaviour
{
    #if UNITY_IPHONE
    bool isAuthorized = false;

    GKLocalPlayer localPlayer;

    string signature;
    string playerID;
    string salt;
    string publicKeyUrl;
    string timestamp;

    async void Start()
    {
        await Login();
    }

    async Task Login()
    {
        if (!GKLocalPlayer.Local.IsAuthenticated)
        {
            try
            {
                localPlayer = await GKLocalPlayer.Authenticate();
                Debug.Log($"GameKit Authentication: player {localPlayer}");
                Debug.Log($"Local Player: {localPlayer.DisplayName}");

                var fetchItemsResponse = await GKLocalPlayer.Local.FetchItems();

                signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
                playerID = localPlayer.GamePlayerId;
                Debug.Log($"Player ID: {playerID}");

                salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
                publicKeyUrl = fetchItemsResponse.PublicKeyUrl;
                timestamp = fetchItemsResponse.Timestamp.ToString();

                Debug.Log($"GameKit Authentication: Signature => {signature}");
                Debug.Log($"GameKit Authentication: PublicKeyUrl => {publicKeyUrl}");
                Debug.Log($"GameKit Authentication: Salt => {salt}");
                Debug.Log($"GameKit Authentication: Timestamp => {timestamp}");

                isAuthorized = true;
            }
            catch (Exception e)
            {
                isAuthorized = false;

                Debug.Log("Failed AppleGameCenter login.");
            }
        }
        else
        {
            isAuthorized = true;

            Debug.Log("AppleGameCenter player already logged in.");
        }
    }

    public async void ShowLeaderboardUI()
    {
        if (isAuthorized)
        {
            await OpenLeaderboard();
        }
        else
        {
            await Login();
        }
    }

    async Task OpenLeaderboard()
    {
        #if UNITY_EDITOR
        return;
        #endif

        if (isAuthorized)
        {
            if (localPlayer == null || localPlayer.IsAuthenticated == false)
            {
                localPlayer = await GKLocalPlayer.Authenticate();
            }

            var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Leaderboards);
            await gameCenter.Present();
        }
    }
    
    async Task ReportLeaderboard(string boardID, int score)
    {
        #if UNITY_EDITOR
        return;
        #endif
        
        if (isAuthorized)
        {
            if (localPlayer == null || localPlayer.IsAuthenticated == false)
            {
                localPlayer = await GKLocalPlayer.Authenticate();
            }

            var context = 0;

            var leaderboards = await GKLeaderboard.LoadLeaderboards(boardID);
            var leaderboard = leaderboards.FirstOrDefault();

            await leaderboard.SubmitScore(score, context, GKLocalPlayer.Local);
        }
    }

    public void AllReportLeaderboard()
    {
        ReportLeaderboard(GameType.INFINITY, PlayerPrefsManager.LoadData("BestScore", 0));
        ReportLeaderboard(GameType.SECOND_ATTACK, PlayerPrefsManager.LoadData("SecondTimeBestScore", 0));
        ReportLeaderboard(GameType.ONE_MIN_ATTACK, PlayerPrefsManager.LoadData("OneTimeBestScore", 0));
    }

    public async void ReportLeaderboard(GameType gameType, int score)
    {
        switch (gameType)
        {
            case GameType.INFINITY:
                await ReportLeaderboard("TopScore_Infinity_Leaderboard", score);
                break;
            case GameType.SECOND_ATTACK:
                await ReportLeaderboard("TopScore_30Sec_Leaderboard", score);
                break;
            case GameType.ONE_MIN_ATTACK:
                await ReportLeaderboard("TopScore_1Min_Leaderboard", score);
                break;
            default:
                break;
        }
    }
    #endif

#if !UNITY_IPHONE
    void OnEnable() 
    {
        Destroy(this.gameObject);
    }
#endif
}
