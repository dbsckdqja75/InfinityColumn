using System;
using System.Collections;
using UnityEngine.Networking;

public class DateManager : MonoSingleton<DateManager>
{
    bool isSynced = false;

    public DateTime todayDateTime { get; private set; }

    void Start()
    {
        WebTimeCheck().Start(this);
    }

    public bool IsSynced()
    {
        return isSynced;
    }

    public bool IsDateEqual(DateTime targetDateTime)
    {
        string lastDay = string.Format("{0:yyyy-MM-dd}", targetDateTime);
        string today = string.Format("{0:yyyy-MM-dd}", todayDateTime);

        return (today == lastDay);
    }

    IEnumerator WebTimeCheck()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result.IsEquals(UnityWebRequest.Result.Success))
            {
                string date = request.GetResponseHeader("date");
                todayDateTime = DateTime.Parse(date).ToLocalTime();

                isSynced = true;
            }
        }

        yield break;
    }
}
