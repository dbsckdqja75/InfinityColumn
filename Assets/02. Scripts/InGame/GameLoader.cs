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

        float progress = 0;
        while(!asyncOperation.isDone || asyncOperation.allowSceneActivation == false)
        {
            loadingProgressImage.fillAmount = Mathf.Lerp(loadingProgressImage.fillAmount, progress, 0.5f * Time.deltaTime);

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
