#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-27 오후 8:29:44
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class WaitForSeconds_Custom : CustomYieldInstruction
{
    private float _fWaitSeconds;

    public WaitForSeconds_Custom(float fWaitSeconds)
    {
        _fWaitSeconds = Time.time + fWaitSeconds;
    }

    public override bool keepWaiting
    {
        get
        {
            return Time.time < _fWaitSeconds;
        }
    }
}

abstract public class CTweenBase : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum ETweenDirection
    {
        Forward,
        Reverse
    }

    public enum ETweenStyle
    {
        Once,
        Loop,
        PingPong,
    }

    /* public - Field declaration            */

    public delegate void OnCreateYield(out CustomYieldInstruction pYield);


    public bool bIsDebug = false;

    public AnimationCurve p_pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    public bool p_bIgnoreTimeScale = false;
    public float p_fDuration = 1f;
    public ETweenStyle p_eTweenStyle = ETweenStyle.Once; 

    public bool p_bIsPlay_OnEnable = false;
    public ETweenDirection p_eTweenDirection_OnDefaultPlay = ETweenDirection.Forward;

    public UnityEvent p_Event_OnFinishTween = new UnityEvent();

    public ETweenDirection p_eTweenDirection { get; private set; }
    public float p_fProgress_0_1 { get; private set; }

    /* protected & private - Field declaration         */

    int _iTweenDirectionDelta;
    Coroutine _pCoroutineTween;

    bool _bIsFinishForward;
    bool _bIsFinishRevese;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoStopTween()
    {
        if (_pCoroutineTween != null)
            StopCoroutine(_pCoroutineTween);
    }

    public void DoStartTween()
    {
        StartTween(p_eTweenDirection_OnDefaultPlay);
    }

    public void DoStartTween_Forward()
    {
        StartTween(ETweenDirection.Forward);
    }

    public void DoStartTween_Reverse()
    {
        StartTween(ETweenDirection.Reverse);
    }

    public void DoInitTween(ETweenDirection eTweenDirection)
    {
        p_eTweenDirection = eTweenDirection;
        p_fProgress_0_1 = eTweenDirection == ETweenDirection.Forward ? 0f : 1f;
        _iTweenDirectionDelta = eTweenDirection == ETweenDirection.Forward ? 1 : -1;

        _bIsFinishForward = false;
        _bIsFinishRevese = false;
    }

    public void DoSetTweening()
    {
        if (p_fProgress_0_1 > 1f)
            _bIsFinishForward = true;
        else if (p_fProgress_0_1 < 0f)
            _bIsFinishRevese = true;

        if (p_fDuration == 0f)
        {
            Debug.Log(name + " p_fDuration == 0f", this);
            return;
        }

        p_fProgress_0_1 += Mathf.Abs(1f / p_fDuration) * Time.deltaTime * _iTweenDirectionDelta;
        OnTween(p_pAnimationCurve.Evaluate(p_fProgress_0_1));

        if (_bIsFinishForward)
        {
            switch (p_eTweenStyle)
            {
                case ETweenStyle.Loop:
                    {
                        p_fProgress_0_1 = 0f;
                        _bIsFinishForward = false;
                    }
                    break;

                case ETweenStyle.PingPong:
                    {
                        p_fProgress_0_1 = 1f - (p_fProgress_0_1 - Mathf.Floor(p_fProgress_0_1));
                        _iTweenDirectionDelta *= -1;
                        _bIsFinishForward = false;
                    }
                    break;
            }
        }
        else if (_bIsFinishRevese)
        {
            switch (p_eTweenStyle)
            {
                case ETweenStyle.Loop:
                    {
                        p_fProgress_0_1 = 1f;
                        _bIsFinishRevese = false;
                    }
                    break;

                case ETweenStyle.PingPong:
                    {
                        p_fProgress_0_1 = -p_fProgress_0_1;
                        p_fProgress_0_1 -= Mathf.Floor(p_fProgress_0_1);
                        _iTweenDirectionDelta *= -1;
                        _bIsFinishRevese = false;
                    }
                    break;
            }
        }
    }


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_bIsPlay_OnEnable)
            DoStartTween();
    }

    /* protected - [abstract & virtual]         */

    abstract public void OnEditorButtonClick_SetStartValue_IsCurrentValue();
    abstract public void OnEditorButtonClick_SetDestValue_IsCurrentValue();
    abstract protected void OnTween(float fProgress_0_1);

    abstract public void OnInitTween_EditorOnly();
    abstract public void OnReleaseTween_EditorOnly();

    virtual protected void Reset() { }
    virtual public object OnTween_EditorOnly(float fProgress_0_1) { return null; }


    // ========================================================================== //

    #region Private

    void StartTween(ETweenDirection eTweenDirection, float fProgres_0_1 = 0f)
    {
        DoStopTween();

        if (p_bIgnoreTimeScale)
            _pCoroutineTween = StartCoroutine(CoStartTween(eTweenDirection, OnCreate_YieldForSecond_Real));
        else
            _pCoroutineTween = StartCoroutine(CoStartTween(eTweenDirection, OnCreate_YieldForSecond));
    }
    
    private IEnumerator CoStartTween(ETweenDirection eTweenDirection, OnCreateYield OnCreatorYield)
    {
        DoInitTween(eTweenDirection);

        while (_bIsFinishForward == _bIsFinishRevese)
        {
            DoSetTweening();

            CustomYieldInstruction pYield;
            OnCreatorYield(out pYield);

            yield return pYield;
        }

        if (p_Event_OnFinishTween != null)
            p_Event_OnFinishTween.Invoke();
    }
    
    // 이렇게 하면 안된다..
    //public WaitForSeconds_Custom OnCreate_YieldForSecond()
    //{
    //    return new WaitForSeconds_Custom(Time.deltaTime);
    //}

    //public WaitForSecondsRealtime OnCreate_YieldForSecond_Real()
    //{
    //    return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
    //}

    public void OnCreate_YieldForSecond(out CustomYieldInstruction pReturn)
    {
        pReturn = new WaitForSeconds_Custom(Time.deltaTime);
    }

    public void OnCreate_YieldForSecond_Real(out CustomYieldInstruction pReturn)
    {
        pReturn = new WaitForSecondsRealtime(Time.unscaledDeltaTime);
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test