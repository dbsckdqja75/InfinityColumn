using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


#if UNITY_IOS
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit;
using Apple.GameKit.Leaderboards;
#endif

public class AppleGameCenterManager : MonoBehaviour
{
    [SerializeField] Button leaderboardBtn;

    private readonly bool IsAccessPointAvailable = Availability.IsTypeAvailable<GKAccessPoint>();
    private readonly bool IsLoadLeaderboardsAvailable = Availability.IsMethodAvailable<GKLeaderboard>(nameof(GKLeaderboard.LoadLeaderboards));

#if UNITY_IPHONE
    string signature;
    string playerID;
    string salt;
    string publicKeyUrl;
    string timestamp;

    async void Start()
    {
        GKLocalPlayer.AuthenticateUpdate += OnAuthenticateUpdate;
        GKLocalPlayer.AuthenticateError += OnAuthenticateError;

        await Login();
    }

    async Task Login()
    {
        if (!GKLocalPlayer.Local.IsAuthenticated)
        {
            try
            {
                GKLocalPlayer localPlayer = await GKLocalPlayer.Authenticate();
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
            }
            catch (Exception e)
            {
                Debug.Log("Failed AppleGameCenter login.");
            }
        }
        else
        {
            Debug.Log("AppleGameCenter player already logged in.");
        }
    }

    public async void ShowLeaderboardUI()
    {
        if (IsAccessPointAvailable && GKLocalPlayer.Local.IsAuthenticated)
        {
            await GKAccessPoint.Shared.Trigger(GKGameCenterViewController.GKGameCenterViewControllerState.Leaderboards);
        }
    }

    async Task ReportLeaderboard(string boardID, int score)
    {
        #if UNITY_EDITOR
                return;
        #endif

        if (IsLoadLeaderboardsAvailable && GKLocalPlayer.Local.IsAuthenticated)
        {
            var leaderboards = await GKLeaderboard.LoadLeaderboards(boardID);
            if (leaderboards != null && leaderboards.Count > 0)
            {
                var leaderboard = leaderboards.FirstOrDefault();

                await leaderboard.SubmitScore(score, 0, GKLocalPlayer.Local);
            }
        }
    }

    public async void AllReportLeaderboard()
    {
        await ReportLeaderboard("TopScore_Infinity_Leaderboard", PlayerPrefsManager.LoadData("BestScore", 0));
        await ReportLeaderboard("TopScore_30Sec_Leaderboard", PlayerPrefsManager.LoadData("SecondTimeBestScore", 0));
        await ReportLeaderboard("TopScore_1Min_Leaderboard", PlayerPrefsManager.LoadData("OneTimeBestScore", 0));
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

    async void OnAuthenticateUpdate(GKLocalPlayer localPlayer)
    {
        await HandleAuthenticateUpdate(localPlayer);
    }

    private async Task HandleAuthenticateUpdate(GKLocalPlayer localPlayer)
    {
        if (localPlayer != null && localPlayer.IsAuthenticated)
        {
            leaderboardBtn.interactable = true;
        }
        else
        {
            leaderboardBtn.interactable = false;
        }
    }

    async void OnAuthenticateError(NSError error)
    {
        await HandleAuthenticateError(error);
    }

    async Task HandleAuthenticateError(NSError error)
    {
        if (Application.isEditor && error.Domain == GKErrorDomain.Name)
        {
            var code = (GKErrorCode)error.Code;
            if (code == GKErrorCode.GameUnrecognized || code == GKErrorCode.NotAuthenticated)
            {
                await HandleAuthenticateUpdate(GKLocalPlayer.Local);
                return;
            }
        }

        leaderboardBtn.interactable = false;
    }
    #endif

    #if !UNITY_IPHONE
    void OnEnable() 
    {
        Destroy(this.gameObject);
    }
    #endif
}
