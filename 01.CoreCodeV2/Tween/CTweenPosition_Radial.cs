#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-30 오후 5:11:32
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;
#endif

public class CTweenPosition_Radial : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Rename_Inspector("트윈 진행 각도범위 180이면 12시 기준으로 6시까지")]
    public float fRaidalRangeAngle = 0f;
    [Rename_Inspector("트윈 시작 오프셋, 0일 땐 12시방향부터 오른쪽으로")]
    public float fRaidalStartAngle = 0f;
    [Rename_Inspector("Radial 개수")]
    public int iDefaultChildCount = 5;
    [Rename_Inspector("Tween Start")]
    public float fDistance_Start = 0f;
    [Rename_Inspector("Tween Dest")]
    public float fDistance_Dest = 10f;

    /* protected & private - Field declaration         */

    List<Transform> _listChild_Instance = new List<Transform>();
    List<Transform> _listChild_Managing = new List<Transform>();

    int _iChildCount;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        UpdateListManagingChild(iDefaultChildCount);
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        UpdateListManagingChild(_iChildCount);
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    public override void OnInitTween_EditorOnly()
    {
        throw new System.NotImplementedException();
    }

    public override void OnReleaseTween_EditorOnly()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnTween(float fProgress_0_1)
    {
        UpdateListManagingChild(_iChildCount);

        float fAngleGap = 360f / _iChildCount;
        float fAngleDelta = 0f;
        for (int i = 0; i < _listChild_Managing.Count; i++)
        {
            fAngleDelta = i * fAngleGap * Mathf.Deg2Rad;
            Transform pTransformChild = _listChild_Managing[i].transform;

            Vector3 vecDirection = new Vector3(Mathf.Sin(fAngleDelta), Mathf.Cos(fAngleDelta), 0f);
            vecDirection *= fDistance_Start * (1f - fProgress_0_1) + fDistance_Dest * fProgress_0_1;

            pTransformChild.localPosition = vecDirection;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(bIsDebug)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _listChild_Managing.Count; i++)
                Gizmos.DrawSphere(_listChild_Managing[i].position, 1f);
        }
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void UpdateListManagingChild(int iChildCount)
    {
        if (_listChild_Instance.Count != iChildCount)
        {
            _iChildCount = iChildCount;

            if (_listChild_Instance.Count < iChildCount)
            {
                while (_listChild_Instance.Count < iChildCount)
                {
                    GameObject pObjectChild = new GameObject();
                    Transform pTransChild = pObjectChild.transform;
                    pTransChild.SetParent(transform);

                    _listChild_Instance.Add(pTransChild);
                }

                for (int i = 0; i < _listChild_Instance.Count; i++)
                    _listChild_Instance[i].SetActive(false);
            }

            if (_listChild_Managing.Count != iChildCount)
            {
                if (_listChild_Managing.Count > iChildCount)
                {
                    while (_listChild_Managing.Count > iChildCount)
                    {
                        _listChild_Managing.Remove(_listChild_Managing[_listChild_Managing.Count]);
                    }
                }

                if (_listChild_Managing.Count < iChildCount)
                {
                    _listChild_Managing.Clear();
                    for (int i = 0; i < iChildCount; i++)
                        _listChild_Managing.Add(_listChild_Instance[i]);
                }

                for (int i = 0; i < _listChild_Managing.Count; i++)
                    _listChild_Managing[i].name = (i + 1).ToString();
            }

            for (int i = 0; i < _listChild_Managing.Count; i++)
                _listChild_Managing[i].SetActive(true);
        }
    }

#endregion Private
}