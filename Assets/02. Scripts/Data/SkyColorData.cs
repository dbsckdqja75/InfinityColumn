using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkyColorData", menuName = "ScriptableObject/SkyColorData")]
public class SkyColorData : ScriptableObject
{
    [SerializeField] bool showsCloud;
    [SerializeField] Color lightColor;
    [SerializeField] float lightIntensity = 1f;
    [SerializeField] List<ColorSet> colorList;

    public bool GetShowsSkyCloud()
    {
        return showsCloud;
    }

    public Color GetSkyLightColor()
    {
        return lightColor;
    }

    public float GetSkyLightIntensity()
    {
        return lightIntensity;
    }

    public List<ColorSet> GetSkyColorSetList()
    {
        return colorList;
    }
}
