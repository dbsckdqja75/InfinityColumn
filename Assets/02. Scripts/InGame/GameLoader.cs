using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [SerializeField] int targetSceneIdx = 1;

    [SerializeField] Image loadingProgressImage;

    void Start()
    {
        LoadScene().Start(this);
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneIdx);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(3f);

        while(!asyncOperation.isDone)
        {
            if(asyncOperation.progress < 0.9f)
            {
                loadingProgressImage.fillAmount = asyncOperation.progress;
            }
            else
            {
                loadingProgressImage.fillAmount = 1;
                asyncOperation.allowSceneActivation = true;
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
