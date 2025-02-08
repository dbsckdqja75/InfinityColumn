using Unity.VisualScripting;
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
    [SerializeField] int previewColorIdx = 0;
    #endif

    [SerializeField] Color[] colorPreset;

    Image image;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        image = this.GetComponent<Image>();

        if(colorPreset.Length <= 0)
        {
            colorPreset = new Color[1];
            colorPreset[0] = image.color;
        }
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
            image.color = colorPreset[0];
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
            SetColorPreset(previewColorIdx);
        }
        else if(image.color != colorPreset[0])
        {
            ResetColor();
        }
    }
    #endif
}
