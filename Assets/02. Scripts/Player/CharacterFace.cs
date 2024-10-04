using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class CharacterFace : MonoBehaviour
{
    [SerializeField] Transform headTrf;

    [SerializeField] Color feverEyeColor;
    [SerializeField] Material feverEyeMaterial;

    [SerializeField] SpriteRenderer[] eyes;

    Color originalEyeColor;
    Material originalEyeMaterial;

    Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();

        if(eyes.Length > 0)
        {
            originalEyeColor = eyes[0].color;
            originalEyeMaterial = eyes[0].material;
        }
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

    public void ChangeFeverEyes()
    {
        foreach(SpriteRenderer eye in eyes)
        {
            eye.material = feverEyeMaterial;
            eye.color = feverEyeColor;
        }
    }

    public void ResetEyes()
    {
        foreach(SpriteRenderer eye in eyes)
        {
            eye.material = originalEyeMaterial;
            eye.color = originalEyeColor;
        }
    }
}
