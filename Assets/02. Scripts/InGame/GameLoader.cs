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

        yield return new WaitForSeconds(1.5f);

        float progress = 0;
        while(!asyncOperation.isDone || asyncOperation.allowSceneActivation == false)
        {
            loadingProgressImage.fillAmount = Mathf.Lerp(loadingProgressImage.fillAmount, progress, Time.deltaTime);

            if(asyncOperation.progress < 0.9f)
            {
                progress = asyncOperation.progress;
            }
            else
            {
                progress = 1;
                asyncOperation.allowSceneActivation = true;
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
