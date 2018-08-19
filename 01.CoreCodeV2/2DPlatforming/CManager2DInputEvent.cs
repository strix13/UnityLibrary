#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-22 오후 12:18:11
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CManager2DInputEvent : CSingletonMonoBase<CManager2DInputEvent>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Rename_Inspector("디버깅")]
    public bool p_bIsPrintDebug = false;
    [Rename_Inspector("이벤트 카메라")]
    public Camera p_pEventCamera;
    [Rename_Inspector("히트할 레이어")]
    public LayerMask p_pLayerMask_Hit;

    public List<RaycastHit2D> p_listLastHit { get; private set; }

    /* protected & private - Field declaration         */

    RaycastHit2D[] _arrHit = new RaycastHit2D[10];
    Ray pRay_OnClick_ForDebug;
    int _iLastHitCount;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public Vector3 DoRayCasting_MousePos(Camera pCamera, LayerMask pLayerMask_Hit)
    {
        int iLayerMask = pLayerMask_Hit.value;
        iLayerMask = ~iLayerMask;

        var pHitInfo = Physics2D.GetRayIntersection(pCamera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, iLayerMask);

        if (p_bIsPrintDebug)
        {
            Ray pRay = pCamera.ScreenPointToRay(Input.mousePosition);
            if (pHitInfo)
                Debug.DrawRay(pRay.origin, (Vector3)pHitInfo.point - pRay.origin, Color.red, 1f);
            else
                Debug.DrawRay(pRay.origin, pRay.direction * 1000f, Color.green, 1f);
        }

        if (pHitInfo)
            return pHitInfo.point;
        else
            return Vector3.zero;
    }


    public Vector3 DoRayCasting_MousePos(LayerMask pLayerMask_Hit)
    {
        int iLayerMask = pLayerMask_Hit.value;
        iLayerMask = ~iLayerMask;

        var pHitInfo = Physics2D.GetRayIntersection(p_pEventCamera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, iLayerMask);

        if(p_bIsPrintDebug)
        {
            Ray pRay = p_pEventCamera.ScreenPointToRay(Input.mousePosition);
            if (pHitInfo)
                Debug.DrawRay(pRay.origin, (Vector3)pHitInfo.point - pRay.origin, Color.red, 1f);
            else
                Debug.DrawRay(pRay.origin, pRay.direction * 1000f, Color.green, 1f);
        }

        if (pHitInfo)
            return pHitInfo.point;
        else
            return Vector3.zero;
    }

    public Vector3 DoRayCasting_MousePos()
    {
        var pHitInfo = Physics2D.GetRayIntersection(p_pEventCamera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
        if (pHitInfo)
            return pHitInfo.point;
        else
            return Vector3.zero;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        p_listLastHit = new List<RaycastHit2D>();
        if (p_pEventCamera == null)
        {
            Camera[] arrCamera = FindObjectsOfType<Camera>();
            for (int i = 0; i < arrCamera.Length; i++)
            {
                if (arrCamera[i].gameObject.tag == "MainCamera")
                {
                    p_pEventCamera = arrCamera[i];
                    break;
                }
            }

            if (p_pEventCamera == null)
                p_pEventCamera = arrCamera[0];
        }
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        yield return null;

        while (true)
        {
            p_listLastHit.Clear();
            bool bMouseClick = Input.GetMouseButtonDown(0);
            _iLastHitCount = Physics2D.GetRayIntersectionNonAlloc(p_pEventCamera.ScreenPointToRay(Input.mousePosition), _arrHit, Mathf.Infinity, p_pLayerMask_Hit.value);
            if (p_bIsPrintDebug && bMouseClick)
                pRay_OnClick_ForDebug = p_pEventCamera.ScreenPointToRay(Input.mousePosition);

            for (int i = 0; i < _iLastHitCount; i++)
            {
                RaycastHit2D pHit = _arrHit[i];
                p_listLastHit.Add(pHit);
                Transform pTransformHit = pHit.transform;

                if (p_bIsPrintDebug)
                    Debug.Log(pTransformHit.name + " RayCast Hit", pTransformHit);

                var pEnter = pTransformHit.GetComponent<IPointerEnterHandler>();
                if (pEnter != null)
                    pEnter.OnPointerEnter(null);

                //var pExit = pTransformHit.GetComponent<IPointerExitHandler>();
                //if (pExit != null)
                //    pExit.OnPointerExit(null);

                if (bMouseClick)
                {
                    var pClick = pTransformHit.GetComponent<IPointerClickHandler>();
                    if (pClick != null)
                        pClick.OnPointerClick(null);
                }
            }

            yield return null;
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (p_bIsPrintDebug == false)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pRay_OnClick_ForDebug.origin, pRay_OnClick_ForDebug.origin + (pRay_OnClick_ForDebug.direction * Mathf.Infinity));

        for (int i = 0; i < _iLastHitCount; i++)
        {
            RaycastHit2D pHit = _arrHit[i];
            Gizmos.DrawSphere(pHit.point, 1f);
        }
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}