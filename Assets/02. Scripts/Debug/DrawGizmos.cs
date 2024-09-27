using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    
    [SerializeField] Transform targetTrf;

    public enum drawType { CUBE, SPHERE }

    public drawType dType = drawType.CUBE;
    
    [Range(0.0f,10.0f)]
    public float scale = 1f;

    public Color drawColor = Color.green;
    
    [Range(0.0f,1.0f)]
    public float colorA = 0.5f;

    public void SetTrf(Transform trf)
    {
        targetTrf = trf;
    }

    void OnDrawGizmos()
    {
        if(!targetTrf)    
        {
            targetTrf = this.transform;
        }

        Color color = drawColor;
        color.a = colorA;
        
        Gizmos.color = color;
        switch (dType)
        {
            case drawType.CUBE:
                Gizmos.DrawCube(targetTrf.position, Vector3.one * scale);
                break;
            case drawType.SPHERE:
                Gizmos.DrawSphere(targetTrf.position, scale);            
                break;
            default:
                return;
        }
    }
}
