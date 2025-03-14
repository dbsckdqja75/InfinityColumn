using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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
        UpdateLatestVersionInfo(serverAddress + "RequestAndroidVersion").Start(this); // NOTE : Android 버전
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

    IEnumerator UpdateLatestVersionInfo(string url) // NOTE : 플레이어 순위와 기록 가져오기
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
}
