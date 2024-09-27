using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class CharacterFace : MonoBehaviour
{
    [SerializeField] Transform headTrf;

    void Update()
    {
        if(headTrf)
        {
            transform.position = headTrf.position;
            transform.rotation = headTrf.rotation;
        }
    }
}
