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

public class CManager2DInputEvent : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Camera pEventCamera;
    public bool _bIsPrintDebug = false;

    /* protected & private - Field declaration         */

    RaycastHit2D[] _arrHit = new RaycastHit2D[10];

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        while(true)
        {
            bool bMouseClick = Input.GetMouseButtonDown(0);
            int iHitCount = Physics2D.GetRayIntersectionNonAlloc(pEventCamera.ScreenPointToRay(Input.mousePosition), _arrHit, Mathf.Infinity);
            for (int i = 0; i < iHitCount; i++)
            {
                RaycastHit2D pHit = _arrHit[i];
                Transform pTransformHit = pHit.transform;

                if (_bIsPrintDebug)
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

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}