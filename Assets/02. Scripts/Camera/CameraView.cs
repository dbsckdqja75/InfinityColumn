using System.Collections.Generic;
using CameraViewSystem;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    bool isPerspectiveView = true;

    [Header("Camera Info")]
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Vector3 offsetRot;
    [SerializeField] float offsetFOV;
    [SerializeField] float offsetZoom;
    float originFOV, originZoom;

    [Header("Camera Follow")]
    [SerializeField] bool isLerp = true;
    [SerializeField] bool onlyFollowAxisY = true;
    [SerializeField] Transform targetTrf;
    [SerializeField] float followSpeed = 10f;
    [SerializeField] float orthographicClipFar = 80;
    [SerializeField] float orthographicOffsetZ = -70;
    [SerializeField] float perspectiveClipFar = 300;

    [SerializeField] List<CameraViewData> cameraViewData = new List<CameraViewData>();

    Camera mainCamera;
    Vector3 targetPos, resultPos, extraPos;

    void FixedUpdate()
    {
        UpdateView();
    }

    public void Init()
   {
        mainCamera = Camera.main;
        originFOV = mainCamera.fieldOfView;
        originZoom = mainCamera.orthographicSize;
    }

    void UpdateView()
    {
        UpdateCameraPos(isLerp);
        UpdateCameraRot();
        UpdateCameraZoom();
    }

    void UpdateCameraPos(bool isLerp)
    {
        if (targetTrf != null)
        {
            targetPos.Set(targetTrf.position);

            if(onlyFollowAxisY)
            {
                targetPos.SetX(0);
            }
        }

        extraPos.z = isPerspectiveView ? 0 : orthographicOffsetZ;

        resultPos.Set(targetPos + offsetPos + extraPos);

        if (isLerp)
        {
            transform.position = Vector3.Lerp(transform.position, resultPos, followSpeed * Time.smoothDeltaTime);
        }
        else
        {
            transform.SetPosition(resultPos);
        }
    }

    void UpdateCameraRot()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(offsetRot), followSpeed * Time.smoothDeltaTime);
    }
    
    void UpdateCameraZoom()
    {
        if(isPerspectiveView)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, (originFOV + offsetFOV), followSpeed * Time.smoothDeltaTime);
        }
        else
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, (originZoom + offsetZoom), followSpeed * Time.smoothDeltaTime);
        }

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, (originZoom + offsetZoom), followSpeed * Time.smoothDeltaTime);
    }

    public void SetFakeView()
    {
        transform.position = (resultPos + (Vector3.down * 0.58f));
    }

    public void RepositionView()
    {
        UpdateCameraPos(false);
    }

    public void SetTargetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
        Time.fixedDeltaTime = (1f/(float)frameRate);

        Debug.LogFormat("[CameraView] Update CameraFrameRate : {0} / {1}", frameRate, Time.fixedDeltaTime);
    }

    public void SetCameraProjection(bool isPerspective)
    {
        isPerspectiveView = isPerspective;

        mainCamera.orthographic = !isPerspectiveView;
        mainCamera.farClipPlane = isPerspectiveView ? perspectiveClipFar : orthographicClipFar;
    }

    public void SetCameraLerp(bool isOn)
    {
        isLerp = isOn;
    }

    public void SetCameraTarget(Transform trf)
    {
        targetTrf = trf;
    }
    
    public void SetCameraPreset(CameraViewData data)
    {
        isLerp = true;

        onlyFollowAxisY = data.onlyFollowAxisY;
        offsetPos = data.offsetPos;
        offsetRot = data.offsetRot;
        offsetFOV = data.offsetFOV;
        offsetZoom = data.offsetZoom;
        followSpeed = data.followSpeed;
    }

    public void SetCameraPreset(int presetIdx)
    {
        SetCameraPreset(cameraViewData[presetIdx]);
    }

    public void SetCameraPreset(string presetId)
    {
        CameraViewData data = cameraViewData[0];
        data = cameraViewData.Find(x => x.id.ToLower() == presetId.ToLower());
        SetCameraPreset(data);
    }
}
