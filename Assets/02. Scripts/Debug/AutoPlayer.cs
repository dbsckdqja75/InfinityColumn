using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayer : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField] bool autoPlay = false;
    [SerializeField, Range(0.01f, 1f)] float playSpeed = 0.05f;

    [Space(10)]
    [SerializeField] Transform spawnPoint;
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] PlayerController pController;

    [Space(10)]
    [SerializeField] GameObject feverEffectObj;
    [SerializeField] GameObject autoplayTextObj, gameOverPanelObj;

    Coroutine autoPlayCoroutine;

    void Update()
    {
        if(autoPlay && !autoplayTextObj.activeSelf)
        {
            AutoPlay().Start(ref autoPlayCoroutine, this);
            autoplayTextObj.SetActive(true);
        }

        if(!autoPlay && autoplayTextObj.activeSelf)
        {
            autoPlayCoroutine.Stop(this);
            autoplayTextObj.SetActive(false);
        }
    }

    IEnumerator AutoPlay()
    {
        while(autoPlay)
        {
            if(spawnPoint.childCount <= 0 || gameOverPanelObj.activeSelf)
                yield return new WaitForEndOfFrame();

            Column nextColumn = spawnManager.GetNextColumn()    ;
            
            if(feverEffectObj.activeSelf) // FeverTime
            {
                if(nextColumn.HasBranch(0)) // Right
                {
                    pController.DebugMove(true);
                }
                else if(nextColumn.HasBranch(1)) // Left
                {
                    pController.DebugMove(false);
                }
                else // None (Random)
                {
                    pController.DebugMove(Random.Range(0, 2) > 0 ? true : false);
                }
            }
            else // Normal
            {
                if(nextColumn.HasBranch(0)) // Left
                {
                    pController.DebugMove(false);
                }
                else if(nextColumn.HasBranch(1)) // Right
                {
                    pController.DebugMove(true);
                }
                else // None (Random)
                {
                    pController.DebugMove(Random.Range(0, 2) > 0 ? true : false);
                }
            }

            yield return new WaitForSeconds(playSpeed);
        }

        yield break;
    }
    #endif
}