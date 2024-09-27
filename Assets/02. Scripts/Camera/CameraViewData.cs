using System;
using UnityEngine;

namespace CameraViewSystem
{
    [Serializable]
    public struct CameraViewData
    {
        public string id;
        public bool onlyFollowAxisY;
        public Vector3 offsetPos;
        public Vector3 offsetRot;
        public float offsetFOV;
        public float offsetZoom;
        public float followSpeed;
    }
}
