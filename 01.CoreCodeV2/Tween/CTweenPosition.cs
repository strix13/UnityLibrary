#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-27 오후 9:05:30
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public static class CTweenPositionHelper
{
    static public void DoStartTween_Position(this Transform pTransStart, Transform pTransDestPos, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenPosition pTweenPos = pTransStart.GetComponent<CTweenPosition>();
        if (pTweenPos == null)
            pTweenPos = pTransStart.gameObject.AddComponent<CTweenPosition>();

        pTweenPos.p_vecPosStart = pTransStart.position;
        pTweenPos.p_vecPosDest = pTransDestPos.position;
        pTweenPos.p_fDuration = fDuration;

        pTweenPos.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenPos.DoStartTween_Forward();
    }

    static public void DoStartTween_Position2D(this Transform pTransStart, Transform pTransDestPos, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenPosition pTweenPos = pTransStart.GetComponent<CTweenPosition>();
        if (pTweenPos == null)
            pTweenPos = pTransStart.gameObject.AddComponent<CTweenPosition>();

        Vector3 vecPos = pTransStart.position;
        vecPos.z = pTransDestPos.position.z;
        pTweenPos.p_vecPosStart = vecPos;
        pTweenPos.p_vecPosDest = pTransDestPos.position;
        pTweenPos.p_fDuration = fDuration;

        pTweenPos.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenPos.DoStartTween_Forward();
    }
}

public class CTweenPosition : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Vector3 p_vecPosStart;
    public Vector3 p_vecPosDest;

    public bool p_bIsLocal;

    /* protected & private - Field declaration         */

    Vector3 _vecPos_Backup;


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void Reset()
    {
        base.Reset();

        p_vecPosStart = transform.position;
        p_vecPosDest = transform.position;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        if (p_bIsLocal)
            transform.localPosition = Vector3.Lerp(p_vecPosStart, p_vecPosDest, fProgress_0_1);
        else
            transform.position = Vector3.Lerp(p_vecPosStart, p_vecPosDest, fProgress_0_1);
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        if (p_bIsLocal)
           p_vecPosStart = transform.localPosition;
        else
           p_vecPosStart = transform.position;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecPosDest = transform.localPosition;
        else
            p_vecPosDest = transform.position;
    }

    public override void OnInitTween_EditorOnly()
    {
        _vecPos_Backup = transform.position;
    }

    public override void OnReleaseTween_EditorOnly()
    {
        transform.position = _vecPos_Backup;
    }

    public override object OnTween_EditorOnly(float fProgress_0_1)
    {
        return Vector3.Lerp(p_vecPosStart, p_vecPosDest, fProgress_0_1);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test