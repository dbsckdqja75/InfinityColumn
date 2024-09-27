using UnityEngine;

public class Panel : MonoBehaviour
{
    public void SetPanel(bool isOn)
    {
        this.gameObject.SetActive(isOn);
    }

    public virtual void Open()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        this.gameObject.SetActive(false);
    }
}
