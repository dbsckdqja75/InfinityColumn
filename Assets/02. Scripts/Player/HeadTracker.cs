using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(AimConstraint))]
public class HeadTracker : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 4;

    AimConstraint aimConstraint;

    Coroutine trackFadeCoroutine;

    void Awake()
    {
        aimConstraint = this.GetComponent<AimConstraint>();

        aimConstraint.AddSource(new ConstraintSource());
    }

    public void SetTarget(Transform targetTrf)
    {
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = targetTrf;
        constraintSource.weight = 1;

        aimConstraint.SetSource(0, constraintSource);
    }

    public void SetActive(bool isOn)
    {
        trackFadeCoroutine?.Stop(this);

        if(isOn)
        {
            trackFadeCoroutine = TrackFadeIn().Start(this);
        }
        else
        {
            trackFadeCoroutine = TrackFadeOut().Start(this);
        }
    }

    IEnumerator TrackFadeIn()
    {
        if(aimConstraint)
        {
            aimConstraint.constraintActive = true;

            while(aimConstraint.weight < 1)
            {
                aimConstraint.weight += fadeSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            aimConstraint.weight = 1;
        }

        yield break;
    }

    IEnumerator TrackFadeOut()
    {
        if(aimConstraint)
        {
            while(aimConstraint.weight > 0)
            {
                aimConstraint.weight -= (fadeSpeed * 2) * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            aimConstraint.weight = 0;
            aimConstraint.constraintActive = false;
        }

        yield break;
    }
}
