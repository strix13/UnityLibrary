#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-28 오후 2:56:28
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CTweenRotationHelper
{
    static public void DoStartTween_Rotation(this Transform pTransStart, Transform pTransDestRot, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenRotation pTweenRot = pTransStart.GetComponent<CTweenRotation>();
        if (pTweenRot == null)
            pTweenRot = pTransStart.gameObject.AddComponent<CTweenRotation>();

        pTweenRot.p_vecRotStart = pTransStart.rotation.eulerAngles;
        pTweenRot.p_vecRotDest = pTransDestRot.rotation.eulerAngles;
        pTweenRot.p_fDuration = fDuration;

        pTweenRot.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenRot.DoStartTween_Forward();
    }

    static public void DoStartTween_Rotation(this Transform pTransStart, Vector3 vecAngleEulerDest_Offset, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenRotation pTweenRot = pTransStart.GetComponent<CTweenRotation>();
        if (pTweenRot == null)
            pTweenRot = pTransStart.gameObject.AddComponent<CTweenRotation>();

        pTweenRot.p_vecRotStart = pTransStart.rotation.eulerAngles;
        pTweenRot.p_vecRotDest = pTweenRot.p_vecRotStart + vecAngleEulerDest_Offset;
        pTweenRot.p_fDuration = fDuration;

        pTweenRot.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenRot.DoStartTween_Forward();
    }
}

public class CTweenRotation : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Vector3 p_vecRotStart;
    public Vector3 p_vecRotDest;

    public bool p_bIsLocal;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void Reset()
    {
        base.Reset();

        p_vecRotStart = transform.rotation.eulerAngles;
        p_vecRotDest = transform.rotation.eulerAngles;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        if (p_bIsLocal)
            transform.localRotation = Quaternion.Euler(Vector3.Slerp(p_vecRotStart, p_vecRotDest, fProgress_0_1));
        else
            transform.rotation = Quaternion.Euler(Vector3.Slerp(p_vecRotStart, p_vecRotDest, fProgress_0_1));
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecRotStart = transform.localRotation.eulerAngles;
        else
            p_vecRotStart = transform.rotation.eulerAngles;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecRotDest = transform.localRotation.eulerAngles;
        else
            p_vecRotDest = transform.rotation.eulerAngles;
    }

    public override void OnInitTween_EditorOnly()
    {
        throw new System.NotImplementedException();
    }

    public override void OnReleaseTween_EditorOnly()
    {
        throw new System.NotImplementedException();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}