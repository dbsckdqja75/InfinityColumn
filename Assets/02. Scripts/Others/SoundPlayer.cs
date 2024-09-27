using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] bool onlyPlayOnActive = false;
    [SerializeField] string soundName;

    public void PlaySound()
    {
        if((onlyPlayOnActive && gameObject.activeSelf) || !onlyPlayOnActive)
        {
            SoundManager.Instance.PlaySound(soundName);
        }
    }

    public void PlayCustomSound(string soundName)
    {
        if((onlyPlayOnActive && gameObject.activeSelf) || !onlyPlayOnActive)
        {
            SoundManager.Instance.PlaySound(soundName);
        }
    }
}
