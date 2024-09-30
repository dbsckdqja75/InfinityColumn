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

    void OnEnable()
    {
        if(headTrf)
        {
            transform.SetParent(headTrf);
        }
    }

    void Update()
    {
        if(headTrf)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
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
