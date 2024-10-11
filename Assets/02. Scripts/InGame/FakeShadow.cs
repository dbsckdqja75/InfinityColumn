using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FakeShadow : MonoBehaviour
{
    Transform targetTrf;

    [SerializeField] float height = -0.49f;

    Vector3 followPos;

    Color originalColor;

    MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();

        originalColor = meshRenderer.material.GetColor("_BaseColor");
    }

    void FixedUpdate()
    {
        if(targetTrf)
        {
            followPos = targetTrf.position;
            followPos.y = height;

            float heightDistance = (1 - Mathf.Clamp(targetTrf.GetChild(0).position.y, 0, 1));

            Color shadowColor = originalColor;
            shadowColor.a = Mathf.Lerp(0, originalColor.a, heightDistance);

            meshRenderer.material.SetColor("_BaseColor", shadowColor);

            transform.position = followPos;
        }
    }

    public void SetTarget(Transform targetTrf)
    {
        this.targetTrf = targetTrf;
    }
}
