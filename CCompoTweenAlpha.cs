#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-16 오전 10:56:54
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCompoTweenAlpha : CObjectBase
{
    public enum EDirection
    {
        Forward,
        Reverse
    }

    [Range(0, 1f)]
    public float p_fTweenStart = 1f;
    [Range(0, 1f)]
    public float p_fTweenDest = 0f;

    public float p_fTweenDurationSec = 1f;
    public float p_fTweenDelaySec = 0.5f;

    public bool bIsIgnoreTimeScale = false;
    public EDirection p_eDirectionStart;

    [GetComponent]
    protected UnityEngine.UI.Image _pImageTarget;

    private void Reset()
    {
        _pImageTarget = GetComponent<UnityEngine.UI.Image>();
        p_fTweenStart = _pImageTarget.color.a;
        if (p_fTweenStart > 0.5f)
            p_fTweenDest = 1f;
        else
            p_fTweenDest = 0f;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();
        
        if (_pImageTarget != null)
            StartCoroutine(CoUpdateTween_Image());
    }

    private IEnumerator CoUpdateTween_Image()
    {
        EDirection eDirection = EDirection.Forward;
        float fTweenStart = p_fTweenStart;
        float fTweenDest = p_fTweenDest;
        float fTweenAmmount = (Mathf.Abs(p_fTweenDest - p_fTweenStart) / p_fTweenDurationSec);
        while (true)
        {
            float fProgress = 0f;
            while(fProgress < 1f)
            {
                Color pColorCurrent = _pImageTarget.color;
                pColorCurrent.a = Mathf.Lerp(fTweenStart, fTweenDest, fProgress);
                _pImageTarget.color = pColorCurrent;

                if (bIsIgnoreTimeScale)
                {
                    fProgress += fTweenAmmount * 0.02f;
                    yield return new WaitForSecondsRealtime(0.02f);

                }
                else
                {
                    fProgress += fTweenAmmount * Time.deltaTime;
                    yield return null;
                }
            }

            if(eDirection == EDirection.Forward)
            {
                eDirection = EDirection.Reverse;
                fTweenDest = p_fTweenStart;
                fTweenStart = p_fTweenDest;
            }
            else
            {
                eDirection = EDirection.Forward;
                fTweenDest = p_fTweenDest;
                fTweenStart = p_fTweenStart;
            }

            if (bIsIgnoreTimeScale)
                yield return new WaitForSecondsRealtime(p_fTweenDelaySec);
            else
                yield return new WaitForSeconds(p_fTweenDelaySec);
        }
    }
}
