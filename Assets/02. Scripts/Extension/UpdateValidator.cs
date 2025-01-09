using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateValidator : MonoBehaviour
{
    [SerializeField] GameObject updateButtonObj;

    const string serverAddress = "http://bu1ld.asuscomm.com:8772/";

    void Start()
    {
        UpdateLatestVersionInfo(serverAddress + "RequestVersion").Start(this);
    }

    public void OpenURL(string url)
    {
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
                if(request.downloadHandler.text != Application.version)
                {
                    updateButtonObj.SetActive(true);
                }

                Debug.LogFormat("[UpdateValidator] 현재 버전 : {0} | 최신 버전 : {1}", Application.version, request.downloadHandler.text);
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
