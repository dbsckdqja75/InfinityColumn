using UnityEngine;

public static class TransformExtensions
{
        public static void BezierCurvePosition(this Transform trf, Vector3 startPos, Vector3 endPos, Vector3 heightOffset, float t)
        {
            Vector3 startPosH = startPos + heightOffset;
            Vector3 endPosH = endPos + heightOffset;
        
            Vector3 M0 = Vector3.Lerp(startPos, startPosH, t);
            Vector3 M1 = Vector3.Lerp(startPosH, endPosH, t);
            Vector3 M2 = Vector3.Lerp(endPosH, endPos, t);
        
            Vector3 B0 = Vector3.Lerp(M0, M1, t);
            Vector3 B1 = Vector3.Lerp(M1, M2, t);

            trf.position = Vector3.Lerp(B0, B1, t);
        }

        public static void LerpScale(this Transform trf, Vector3 startScale, Vector3 endScale, float t)
        {
            trf.SetScale(Vector3.Lerp(startScale, endScale, t));
        }

        public static void SetPosition(this Transform trf, Vector3 newPos)
        {
            trf.position = newPos;
        }

        public static void SetPositionX(this Transform trf, float newX)
        {
            Vector3 newPos = trf.position;
            trf.position = newPos.SetX(newX);
        }

        public static void SetPositionY(this Transform trf, float newY)
        {
            Vector3 newPos = trf.position;
            trf.position = newPos.SetY(newY);
        }
        
        public static void SetPositionZ(this Transform trf, float newZ)
        {
            Vector3 newPos = trf.position;
            trf.position = newPos.SetZ(newZ);
        }

        public static void SetRotation(this Transform trf, Quaternion newRot)
        {
            trf.rotation = newRot;
        }

        public static void SetScale(this Transform trf, float newScaleXYZ)
        {
            Vector3 newScale = new Vector3(newScaleXYZ, newScaleXYZ, newScaleXYZ);
            trf.localScale = newScale;
        }

        public static void SetScale(this Transform trf, Vector3 newScale)
        {
            trf.localScale = newScale;
        }
        
        public static void SetScaleX(this Transform trf, float newX)
        {
            Vector3 newScale = trf.localScale;
            trf.localScale = newScale.SetX(newX);
        }
        
        public static void SetScaleY(this Transform trf, float newY)
        {
            Vector3 newScale = trf.localScale;
            trf.localScale = newScale.SetY(newY);
        }
        
        public static void SetScaleZ(this Transform trf, float newZ)
        {
            Vector3 newScale = trf.localScale;
            trf.localScale = newScale.SetZ(newZ);
        }
        
        public static void MultipleScale(this Transform trf, float multiple)
        {
            trf.localScale *= multiple;
        }
        
        public static void MultipleScaleX(this Transform trf, float multiple)
        {
            Vector3 newScale = trf.localScale;
            trf.localScale = newScale.SetX(newScale.x * multiple);
        }
        
        public static void MultipleScaleY(this Transform trf, float multiple)
        {
            Vector3 newScale = trf.localScale;
            trf.localScale = newScale.SetY(newScale.y * multiple);
        }
        
        public static void MultipleScaleZ(this Transform trf, float multiple)
        {
            Vector3 newScale = trf.localScale;
            trf.localScale = newScale.SetZ(newScale.z * multiple);
        }
}