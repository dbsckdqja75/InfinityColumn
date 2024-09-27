using UnityEngine;

public class ActiveSound : MonoBehaviour
{
    [SerializeField] string soundName;

    void OnEnable() 
    {
        SoundManager.Instance.PlaySound(soundName);
    }
}
