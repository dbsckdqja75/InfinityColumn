using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class CharacterFace : MonoBehaviour
{
    [SerializeField] Transform headTrf;

    Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if(headTrf)
        {
            transform.position = headTrf.position;
            transform.rotation = headTrf.rotation;
        }
    }

    public void SetFace(int faceIdx)
    {
        if(animator)
        {
            animator.SetInteger("FaceIdx", faceIdx);
        }
    }
}
