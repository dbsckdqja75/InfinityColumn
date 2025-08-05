using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_ANDROID
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

public class UpdateValidator : MonoBehaviour
{
    [SerializeField] string androidURL, iosURL, desktopURL;

    [Space(10)]
    [SerializeField] GameObject updateButtonObj;

    const string serverAddress = "";

    void Start()
    {
        #if UNITY_EDITOR
        UpdateLatestVersionInfo(serverAddress + "RequestVersionDebug").Start(this);
        #elif UNITY_ANDROID
        CheckForGoogleUpdate().Start(this);
        // UpdateLatestVersionInfo(serverAddress + "RequestAndroidVersion").Start(this); // NOTE : Android 버전
        #elif UNITY_IPHONE
        UpdateLatestVersionInfo(serverAddress + "RequestiOSVersion").Start(this); // NOTE : iOS 버전
        #else
        UpdateLatestVersionInfo(serverAddress + "RequestVersion").Start(this); // NOTE : PC 버전
        #endif
    }

    void ValidateVersion(string version)
    {
        string[] latestVersionInfo = version.Split('.');
        string[] currentVersionInfo = Application.version.Split('.');

        int latestPatch = int.Parse(latestVersionInfo[latestVersionInfo.Length-1]);
        int currentPatch = int.Parse(currentVersionInfo[currentVersionInfo.Length-1]);

        if(currentPatch != latestPatch && currentPatch < latestPatch)
        {
            #if !UNITY_STANDALONE && !UNITY_EDITOR
            updateButtonObj.SetActive(true);
            #endif
        }

        Debug.LogFormat("[UpdateValidator] 현재 버전 : {0} | 최신 버전 : {1}", Application.version, version);
    }

    public void OpenUpdatePage()
    {
        string url = "https://teamcampfire.tistory.com/";

        #if UNITY_EDITOR || UNITY_STANDALONE
        url = desktopURL;
        #elif UNITY_ANDROID
        url = androidURL;
        #elif UNITY_IPHONE
        url = iosURL;
        #endif

        Application.OpenURL(url);
    }

    IEnumerator UpdateLatestVersionInfo(string url)
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.timeout = 10;

            yield return request.SendWebRequest();
            yield return new WaitUntil(() => request.isDone);

            if(request.result.IsEquals(UnityWebRequest.Result.Success))
            {
                ValidateVersion(request.downloadHandler.text);
            }
            else
            {
                Debug.LogWarningFormat("[UpdateValidator] 서버 접근 실패 : {0}", request.error);
            }

            request.Dispose();
        }

        yield break;
    }

    #if UNITY_ANDROID
    IEnumerator CheckForGoogleUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForEndOfFrame();
    
        AppUpdateManager appUpdateManager = new AppUpdateManager();
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        yield return appUpdateInfoOperation; // NOTE : 업데이트 정보 확인

        if (appUpdateInfoOperation.IsSuccessful) // 확인 완료
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            // NOTE : 업데이트 가능 상태
            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                updateButtonObj.SetActive(true);

                var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
                yield return startUpdateRequest;

                while(!startUpdateRequest.IsDone)
                {
                    if(startUpdateRequest.Status == AppUpdateStatus.Canceled || startUpdateRequest.Status == AppUpdateStatus.Failed)
                    {
                        Debug.Log("다운로드 실패 또는 취소");
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("다운로드 진행 중");
 
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        updateButtonObj.SetActive(false);

                        Debug.Log("다운로드 완료");
                    }
 
                    yield return new WaitForEndOfFrame();
                    yield return null;
                }

                var result = appUpdateManager.CompleteUpdate();
 
                while(!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("업데이트 없음");
            }
        }

        yield return new WaitForEndOfFrame();
        yield break;
    }
    #endif
}
