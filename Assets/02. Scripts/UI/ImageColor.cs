using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(Image))]
public class ImageColor : MonoBehaviour
{

    #if UNITY_EDITOR
    [SerializeField] bool isPreview = false;
    #endif

    [SerializeField] Color[] colorPreset;

    Image image;

    Color originColor;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        image = this.GetComponent<Image>();

        originColor = image.color;
    }

    public void SetColorPreset(int presetIdx)
    {
        if(colorPreset.Length > 0)
        {
            image.color = colorPreset[presetIdx];
        }
    }

    public void ResetColor()
    {
        if(image)
        {
            image.color = originColor;
        }
    }

    #if UNITY_EDITOR
    void Update()
    {
        if(Application.isPlaying)
            return;

        if(!image)
        {
            Init();
        }
        
        if(isPreview)
        {
            SetColorPreset(0);
        }
        else if(image.color != originColor)
        {
            ResetColor();
        }
    }
    #endif
}
