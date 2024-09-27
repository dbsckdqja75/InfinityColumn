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
    [SerializeField] float perspectiveClipFar = 300;

    [SerializeField] List<CameraViewData> cameraViewData = new List<CameraViewData>();

    Camera mainCamera;
    Vector3 targetPos, resultPos, effectPos;

    void FixedUpdate()
    {
        UpdateView();
    }

    #if UNITY_STANDALONE
    int targetWidth = 1080, targetHeight = 1920;
    int screenWidth, screenHeight;

    void Awake()
    {
        screenWidth = Screen.resolutions[Screen.resolutions.Length-1].width;
        screenHeight = Screen.resolutions[Screen.resolutions.Length-1].height;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            float aspectRatio = ((float)screenHeight / (float)targetHeight);
            float extraRatio = 0.925f;

            int changeWidth = (int)((targetWidth * aspectRatio) * extraRatio);
            int changeHeight = (int)(screenHeight * extraRatio);

            Screen.SetResolution(changeWidth, changeHeight, false);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Screen.SetResolution(900, 1600, false);
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Screen.SetResolution(720, 1280, false);
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Screen.SetResolution(540, 960, false);
        }
    }
    #endif

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

        resultPos.Set(targetPos + offsetPos + effectPos);

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
