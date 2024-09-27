using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AntiCheatManager : MonoSingleton<AntiCheatManager>
{

    int cheatCount;

    string dateTime;

    static int xorCode;

    public static int SecureInt(int data) { return data ^ xorCode; }

    protected override void Init()
    {
        xorCode = (Random.Range(0, 10000) + Random.Range(0, 10000) + Random.Range(0, 10000)).GetHashCode();

        StartAntiCheat();
    }

    public void StartAntiCheat()
    {
        StartCoroutine(CheckAntiCheat());

        Debug.Log("[AntiCheatManager] START ANTI-CHEAT");
    }

    IEnumerator CheckAntiCheat()
    {
        TimeSpan timeDiff;

        DateTime nowTime, dateTime;

        dateTime = DateTime.Now;

        int checkCount = 0;

        yield return new WaitForSecondsRealtime(10);

        while (true)
        {
            nowTime = DateTime.Now;

            timeDiff = nowTime - dateTime;

            if ((int)timeDiff.TotalSeconds <= 8 || (int)timeDiff.TotalSeconds <= -8)
            {
                checkCount++;

                if (checkCount > 3)
                {
                    Application.Quit();
                }
            }
            else
                checkCount = 0;

            dateTime = DateTime.Now;

            yield return new WaitForSecondsRealtime(10);
        }
    }
}
