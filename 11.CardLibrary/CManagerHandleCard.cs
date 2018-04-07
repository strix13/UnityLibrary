#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-18 오후 5:04:28
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CManagerHandleCard : CSingletonMonoBase<CManagerHandleCard>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Header("카드 위치 옵션 조절")]
    [Rename_Inspector("중점에서의 거리")]
    public float _fMiddlePoint_To_Distance;
    [Rename_Inspector("카드간의 갭")]
    public float _fCardGap;

    [Rename_Inspector("총 카드 갯수")]
    public int _iLimitHandleCount = 10;

    /* protected & private - Field declaration         */

    private List<Transform> _listHandleCard = new List<Transform>();
    private List<Transform> _listHandleTransform = new List<Transform>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoInsertHandle(Transform pTransCard)
    {
        _listHandleCard.Add(pTransCard);
    }

    public void DoRemoveHandle(Transform pTransCard)
    {
        _listHandleCard.Remove(pTransCard);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        GetComponentsInChildren(_listHandleTransform);        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        ProcSyncHandleCardCount();
        ProcUpdateHandlePosition();

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pTransHandle = transform.GetChild(i);
            UnityEditor.Handles.Label(pTransHandle.position, pTransHandle.name);
            Gizmos.DrawWireCube(pTransHandle.position, Vector3.one * 0.1f);
        }
    }
#endif

    /* protected - [abstract & virtual]         */

    // ========================================================================== //

    #region Private

    private void ProcSyncHandleCardCount()
    {
        int iChildCount = transform.childCount;
        if (iChildCount < _iLimitHandleCount)
        {
            int iGenreateCount = _iLimitHandleCount - iChildCount;
            for (int i = 0; i < iGenreateCount; i++)
            {
                GameObject pObjectNewHandle = new GameObject(string.Format("HandleSlot_{0}", transform.childCount + 1));
                pObjectNewHandle.transform.SetParent(transform);
                pObjectNewHandle.transform.SetAsLastSibling();
            }
        }
        else if (iChildCount > _iLimitHandleCount)
        {
            int iRemoveCount = iChildCount - _iLimitHandleCount;
            for (int i = 0; i < iRemoveCount; i++)
            {
                GameObject pObjectChildLast = transform.GetChild(transform.childCount - 1).gameObject;
                DestroyImmediate(pObjectChildLast);
            }
        }
    }

    private void ProcUpdateHandlePosition()
    {
        Vector2 vecMiddlePointPos = transform.position;
        float fAngleGap = 180 / transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pTransHandle = transform.GetChild(i);
            float fPosX = Mathf.Cos(fAngleGap * i);
            float fPosY = Mathf.Sign(fAngleGap * i);

            Vector2 vecPos = vecMiddlePointPos + new Vector2(fPosX, fPosY);
            Vector2 vecDirection = vecPos - vecMiddlePointPos;
            pTransHandle.position = vecDirection.normalized * _fMiddlePoint_To_Distance;
        }
    }

    #endregion Private
}
