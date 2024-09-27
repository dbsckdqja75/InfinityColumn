using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] int maxColumnCount;
    [SerializeField] Vector3 spawnGap;
    [SerializeField] Transform columnSpawnParent;
    [SerializeField] Transform effectSpawnParent;
    [SerializeField] GameObject columnPrefab;
    [SerializeField] GameObject columnEffectPrefab;

    bool isStackMode = false;

    int centerIndex = 0;

    Vector3 lastSpawnPoint;

    List<Column> columnList = new List<Column>();

    Queue<ColumnEffect> branchEffectList = new Queue<ColumnEffect>();

    public void Init()
    {
        InitColumnPool();
        InitEffectPool();
    }

    void InitColumnPool()
    {
        columnList.Clear();

        centerIndex = 0;
        lastSpawnPoint = (columnSpawnParent.position - spawnGap);
        
        for(int i = 0; i < maxColumnCount; i++)
        {
            Vector3 spawnPos = lastSpawnPoint + spawnGap;
            lastSpawnPoint = spawnPos;

            Column column = Instantiate(columnPrefab, spawnPos, Quaternion.identity, columnSpawnParent).GetComponent<Column>();
            column.Init((i < 1) ? -1 : Random.Range(-1, 2));

            columnList.Add(column);
        }
    }

    void InitEffectPool()
    {
        branchEffectList.Clear();

        for(int i = 0; i < maxColumnCount; i++)
        {
            ColumnEffect columnEffect = Instantiate(columnEffectPrefab, effectSpawnParent.position, Quaternion.identity, effectSpawnParent).GetComponent<ColumnEffect>();
            branchEffectList.Enqueue(columnEffect);
            columnEffect.Release();
        }
    }

    public void ResetPool()
    {
        foreach (Transform child in columnSpawnParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in effectSpawnParent)
        {
            Destroy(child.gameObject);
        }

        InitColumnPool();
        InitEffectPool();
    }

    public void UpdateColumnMode(bool isSwapMode)
    {
        isStackMode = !isSwapMode;
    }

    public void NextColumn()
    {
        if(isStackMode)
        {
            NextColumnStack();
        }
        else
        {
            NextColumnSwap();
        }
    }

    void NextColumnStack()
    {
        // NOTE : 3번째 컬럼 높이부터 사라져도 화면이 보이지 않음
        if(centerIndex < 4)
        {
            centerIndex += 1;
            return;
        }

        Vector3 movePos = lastSpawnPoint + spawnGap;

        Column firstColumn = columnList[0];
        firstColumn.transform.position = movePos;
        firstColumn.transform.SetAsLastSibling();
        firstColumn.Init(Random.Range(-1, 2));

        columnList.Remove(firstColumn);
        columnList.Add(firstColumn);

        lastSpawnPoint = movePos;
    }

    void NextColumnSwap()
    {
        int lastIndex = columnList.Count-1;
        Column firstColumn = columnList[0];
        Vector3 lastPos = columnList[lastIndex].transform.position;

        for(int i = lastIndex; i > 0; i--)
        {
            columnList[i].transform.position = columnList[i-1].transform.position;
        }
        
        firstColumn.transform.position = lastPos;
        firstColumn.transform.SetAsLastSibling();
        firstColumn.Init(Random.Range(-1, 2));

        columnList.Remove(firstColumn);
        columnList.Add(firstColumn);
    }

    public void SpawnEffect(Column targetColumn, float forceMultiple = 1f)
    {
        ColumnEffect effect = branchEffectList.Dequeue();
        effect.gameObject.SetActive(true);

        effect.Play(targetColumn, forceMultiple);
        branchEffectList.Enqueue(effect);
    }

    public Column GetNextColumn()
    {
        return columnList[centerIndex+1];
    }
}
