using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-15 오전 12:44:41
   Description : 
   Edit Log    : 
   ============================================ */

public class CPathFinder_RayCast : CObjectBase
{
    /* const & readonly declaration             */
    const int const_iRayCount = 5;

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public float fSpeed = 1f;
    public float fSpeedRotate = 1f;
    public Transform pTransTarget;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    [SerializeField]
    private float _fUpdateTime = 0.02f;
    [SerializeField]
    private float _fRayDistance = 3f;
    [SerializeField]
    private float _fRayHeight = 0.5f;

    private List<Vector3> _listRayDirection = new List<Vector3>();
    private bool[] _arrRayHit;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _arrRayHit = new bool[const_iRayCount];
        
        _listRayDirection.Add(new Vector3(0f, 0f, 1f) * _fRayDistance);
        _listRayDirection.Add(new Vector3(-0.25f, 0f, 1f) * _fRayDistance);
        _listRayDirection.Add(new Vector3(-0.5f, 0f, 1f) * _fRayDistance);
        _listRayDirection.Add(new Vector3(0.25f, 0f, 1f) * _fRayDistance);
        _listRayDirection.Add(new Vector3(0.5f, 0f, 1f) * _fRayDistance);
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        StartCoroutine(CoUpdatePathFinding());
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 vecPos = _pTransformCached.position;
        vecPos.y = _fRayHeight;
        for (int i = 0; i < _listRayDirection.Count; i++)
        {
            if (_arrRayHit[i])
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.blue;

            Gizmos.DrawRay(vecPos, _pTransformCached.TransformDirection(_listRayDirection[i]));
        }
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private IEnumerator CoUpdatePathFinding()
    {
        while(true)
        {
            Vector3 vecPos = _pTransformCached.position;
            vecPos.y = _fRayHeight;

            bool bHitRaySomthing = false;
            for (int i = 0; i < _listRayDirection.Count; i++)
            {
                Vector3 vecDirection = _pTransformCached.TransformDirection(_listRayDirection[i]);
                _arrRayHit[i] = Physics.Raycast(vecPos, vecDirection, _fRayDistance);

                if (_arrRayHit[i] && bHitRaySomthing == false)
                    bHitRaySomthing = true;
            }


            Vector3 vecDestDirection = Vector3.forward;
            if (bHitRaySomthing)
            {
                for (int i = 0; i < _listRayDirection.Count; i++)
                {
                    if (_arrRayHit[i] == false)
                    {
                        vecDestDirection = _listRayDirection[i];

                        if (i == 0)
                            _pTransformCached.Translate(Vector3.forward * fSpeed, Space.Self);

                        break;
                    }
                }
            }
            else
            {
                vecDestDirection = pTransTarget.position - _pTransformCached.position;
                _pTransformCached.Translate(Vector3.forward * fSpeed, Space.Self);
            }

            Quaternion quatTarget = Quaternion.LookRotation(vecDestDirection);
            _pTransformCached.rotation = Quaternion.RotateTowards(_pTransformCached.rotation, quatTarget, fSpeedRotate);

            yield return new WaitForSeconds(_fUpdateTime);
        }
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
