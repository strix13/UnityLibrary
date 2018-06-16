#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-08 오전 11:41:40
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

public class CManagerPoolingSimple : CSingletonDynamicMonoBase<CManagerPoolingSimple>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    public Dictionary<GameObject, int> _mapAllInstance = new Dictionary<GameObject, int>();
    public Dictionary<int, LinkedList<GameObject>> _mapUsed = new Dictionary<int, LinkedList<GameObject>>();
    public Dictionary<int, LinkedList<GameObject>> _mapUnUsed = new Dictionary<int, LinkedList<GameObject>>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public GameObject DoPop(GameObject pObjectCopyTarget, bool bActive = true)
    {
        int iID = pObjectCopyTarget.GetHashCode();
        if (_mapUnUsed.ContainsKey(iID) == false)
        {
            _mapUsed.Add(iID, new LinkedList<GameObject>());
            _mapUnUsed.Add(iID, new LinkedList<GameObject>());
        }

        GameObject pObjectUnUsed = null;
        if (_mapUnUsed[iID].Count != 0)
        {
            pObjectUnUsed = _mapUnUsed[iID].First.Value;
            _mapUnUsed[iID].RemoveFirst();
        }
        else
        {
            pObjectUnUsed = Instantiate(pObjectCopyTarget);
            pObjectUnUsed.transform.SetParent(transform);

            CCompoEventTrigger pEventTrigger = pObjectUnUsed.AddComponent<CCompoEventTrigger>();
            pEventTrigger.p_eInputType = CCompoEventTrigger.EInputType.OnDisable;
            pEventTrigger.p_Event_IncludeThisObject += DoPush;

            _mapAllInstance.Add(pObjectUnUsed, iID);
        }

        _mapUsed[iID].AddLast(pObjectUnUsed);
        pObjectUnUsed.SetActive(bActive);
        return pObjectUnUsed;
    }

    public void DoPush(GameObject pObjectReturn)
    {
        if (_mapAllInstance.ContainsKey(pObjectReturn) == false)
            return;

        if (pObjectReturn.activeSelf)
            pObjectReturn.SetActive(false);

        int iID = _mapAllInstance[pObjectReturn];
        _mapUsed[iID].Remove(pObjectReturn);
        _mapUnUsed[iID].AddLast(pObjectReturn);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

#if UNITY_EDITOR
    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        int iUseCount = 0;
        foreach(var pList in _mapUsed.Values)
            iUseCount += pList.Count;

        name = string.Format("풀링매니져_심플/ 총생산:{0} /사용중:{1}", _mapAllInstance.Count, iUseCount);
    }
#endif

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