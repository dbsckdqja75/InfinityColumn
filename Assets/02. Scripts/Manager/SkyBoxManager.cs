using System;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxManager : MonoBehaviour
{
    [SerializeField] Material skyboxMaterialOri;

    [SerializeField] float starsFadeMinHeight = 30000;
    [SerializeField] float starsFadeMaxHeight = 60000;

    [SerializeField] float starsMaxIntensity = 1f;
    
    [SerializeField] List<ColorSet> colorList;

    Material skyboxMaterial;
    
    ColorSet curColorSet;
    
    float curStarsIntensity = 0;

    float curHeight = 0;
    float prevHeight = 0;
    
    int curIndex = 0;

    [Serializable]
    public struct ColorSet
    {
        public int targetHeight;
        
        public Color top;
        public Color center;
        public Color bottom;
    }

    void Start()
    {
        Init();
    }
    
    void Update()
    {
        UpdateSkyBox();
        UpdateStars();
    }

    void Init()
    {
        skyboxMaterial = RenderSettings.skybox;

        curColorSet = colorList[curIndex];
        UpdateColor();

        curIndex++;
    }

    public void UpdateHeight(float height)
    {
        curHeight = height;
    }

    public void ResetHeight()
    {
        curHeight = 0;
    }

    void UpdateSkyBox()
    {
        // NOTE : Target의 Position Y값을 현재 높이로 두어 갱신
        float targetHeight = colorList[curIndex].targetHeight;
        
        if (curHeight < prevHeight) // NOTE : 이전의 갱신 높이보다 낮은 경우
        {
            curIndex = Mathf.Clamp(--curIndex, 0, colorList.Count-1);

            prevHeight = colorList[curIndex].targetHeight;
        }
        else if (curHeight < targetHeight) // NOTE : 다음 갱신 높이보다 낮은 상태일 경우
        {
            float t = (curHeight - prevHeight) / (targetHeight - prevHeight);
            
            // NOTE : 이전, 현재, 다음 높이로 진행도를 산출하여 색상 값 반영
            curColorSet.top = Color.Lerp(colorList[curIndex-1].top, colorList[curIndex].top, t);
            curColorSet.center = Color.Lerp(colorList[curIndex-1].center, colorList[curIndex].center, t);
            curColorSet.bottom = Color.Lerp(colorList[curIndex-1].bottom, colorList[curIndex].bottom, t);
            
            UpdateColor();
        }
        else if(curHeight >= targetHeight) // NOTE : 다음 높이에 도달한 경우
        {
            prevHeight = targetHeight;

            curIndex = Mathf.Clamp(++curIndex, 0, colorList.Count-1);
        }
    }

    void UpdateStars()
    {
        if(curHeight >= starsFadeMinHeight)
        {
            float t = (curHeight - starsFadeMinHeight) / (starsFadeMaxHeight - starsFadeMinHeight);

            curStarsIntensity = Mathf.Lerp(0, starsMaxIntensity, t);
        }
        else
        {
            curStarsIntensity = 0;
        }

        skyboxMaterial.SetFloat("_StarsIntensity", curStarsIntensity);
    }

    void UpdateColor()
    {
        skyboxMaterial.SetColor("_Top", curColorSet.top);
        skyboxMaterial.SetColor("_Center", curColorSet.center);
        skyboxMaterial.SetColor("_Bottom", curColorSet.bottom);
    }

    void OnApplicationQuit()
    {
        RenderSettings.skybox.SetColor("_Top", skyboxMaterialOri.GetColor("_Top"));
        RenderSettings.skybox.SetColor("_Center", skyboxMaterialOri.GetColor("_Center"));
        RenderSettings.skybox.SetColor("_Bottom", skyboxMaterialOri.GetColor("_Bottom"));

        RenderSettings.skybox.SetFloat("_StarsIntensity", skyboxMaterialOri.GetFloat("_StarsIntensity"));
    }
}
