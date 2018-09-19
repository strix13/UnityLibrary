#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-07-23 오후 7:14:44
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

public class CCompoCustomPhysics : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EColliderType
    {
        None,

        SphereCollider,
        BoxCollider,

        GreaterIs2D,

        CircleCollider_2D,
        BoxCollider_2D,
    }

    public enum EPhysicsEventCustom
    {
        Enter,
        Stay,
        Exit
    }

    /* public - Field declaration            */

    public delegate void OnPhysicsEvent2D(List<Collider2D> listCollider, EPhysicsEventCustom ePhysicsEvent);

    public event OnPhysicsEvent2D p_Event_OnPhysicsEvent_Custom2D;

    [Rename_Inspector("디버깅")]
    public bool p_bIsDebuging;

    [Rename_Inspector("타겟 트렌스폼", false)]
    public Transform p_pTransformTarget;

    [Rename_Inspector("물리 체크 TimeDelta")]
    public float p_fPhysicsCheckDelay = 0.02f;

    [Rename_Inspector("히트 Array Capcity")]
    public int p_iHitInfoCount = 10;

    [Rename_Inspector("히트 레이어 마스크")]
    public LayerMask p_pLayerMask;

    [Rename_Inspector("항상 사이즈를 갱신할 것인지")]
    public bool p_bUpdateTransform = true;

    /* protected & private - Field declaration         */

    delegate int OnGetHit();

    [SerializeField]
    [Rename_Inspector("컬라이더 타입", false)]
    EColliderType _eColliderType;

    List<Collider2D> _listCollider2D_InCollider = new List<Collider2D>();
    // List<Collider> _listCollider3D_InCollider = new List<Collider>();

    List<Collider2D> _listCollider2D_NewInner = new List<Collider2D>();
    List<Collider2D> _listCollider2D_Exit = new List<Collider2D>();

    //List<Collider> _listCollider3D_NewInner = new List<Collider>();
    //List<Collider> _listCollider3D_Exit = new List<Collider>();


    // RaycastHit[] _arrHitInfo3D;
    RaycastHit2D[] _arrHitInfo2D;

    OnGetHit _OnGetHit;

    BoxCollider2D _pBoxCollider2D_Origin;
    BoxCollider2D _pBoxCollider2D_Current;

    CircleCollider2D _pCircleCollider2D_Orign;

    Vector3 _vecSize;
    Vector3 _vecOffset;
    float _fRadius;
    bool _bIs2D;
    bool _bIsLock_CalculatePhysics = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoClear_InColliderList()
    {
        _listCollider2D_InCollider.Clear();
    }

    public List<Collider2D> GetList_InCollider2D()
    {
        return _listCollider2D_InCollider;
    }

    public bool DoCheck_IsInner(Collider2D pCollider)
    {
        return _listCollider2D_InCollider.Contains(pCollider);
    }

    public void DoLock_CalculatePhysics(bool bLock)
    {
        _bIsLock_CalculatePhysics = bLock;
    }

    public void DoExcute_CalculatePhysics()
    {
        CalculatePhysics(_OnGetHit());
    }

    public void DoRevertOriginTarget()
    {
        if (_pBoxCollider2D_Origin)
            SetCollider(transform, _pBoxCollider2D_Origin);

        if (_pCircleCollider2D_Orign)
            SetCollider(transform, _pCircleCollider2D_Orign);
    }

    public void DoChangeCollider(Transform pTransform, BoxCollider2D pBoxCollider)
    {
        SetCollider(pTransform, pBoxCollider);
    }

    public void DoChangeCollider(Transform pTransform, CircleCollider2D pCircleCollider)
    {
        SetCollider(pTransform, pCircleCollider);
    }

    public void DoChangeBoxColliderSize(Vector3 vecSize)
    {
        _vecSize = vecSize;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _eColliderType = EColliderType.None;
        _pBoxCollider2D_Origin = GetComponent<BoxCollider2D>();
        _pCircleCollider2D_Orign = GetComponent<CircleCollider2D>();
        DoRevertOriginTarget();

        _bIs2D = _eColliderType > EColliderType.GreaterIs2D;
        if (_bIs2D)
            _arrHitInfo2D = new RaycastHit2D[p_iHitInfoCount]; 
        //else
        //    _arrHitInfo3D = new RaycastHit[p_iHitInfoCount];

        if (_eColliderType == EColliderType.None)
            Debug.LogWarning(name + "_eColliderType == EColliderType.None");
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        if (_bIs2D)
        {
            while (true)
            {
                yield return new WaitForSeconds(p_fPhysicsCheckDelay);

                if (_bIsLock_CalculatePhysics)
                    continue;

                DoExcute_CalculatePhysics();
            }
        }
        else
        {
            while (true)
            {
                yield return new WaitForSeconds(p_fPhysicsCheckDelay);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (p_bIsDebuging == false)
            return;

        Gizmos.color = Color.green;
        if (_pBoxCollider2D_Origin)
            Gizmos.DrawWireCube(p_pTransformTarget.position + (p_pTransformTarget.rotation * _vecOffset), _vecSize);

        if (_pCircleCollider2D_Orign)
            Gizmos.DrawWireSphere(p_pTransformTarget.position + _vecOffset, _fRadius);
    }
#endif

    /* protected - [abstract & virtual]         */

    virtual protected void OnCalculate_Inner_And_ExitCollider(List<Collider2D> list_InCollider_Already, List<Collider2D> list_InCollider_New, List<Collider2D> list_ExitCollider)
    {
        for (int i = 0; i < list_InCollider_Already.Count; i++)
        {
            Collider2D pCollider = list_InCollider_Already[i];
            if (list_InCollider_New.Contains(pCollider))
                list_InCollider_New.Remove(pCollider);
            else
                list_ExitCollider.Add(pCollider);
        }
    }

    // ========================================================================== //

    #region Private

    private void CalculatePhysics(int iHitCount)
    {
        _listCollider2D_NewInner.Clear();
        _listCollider2D_Exit.Clear();

        for (int i = 0; i < iHitCount; i++)
            _listCollider2D_NewInner.Add(_arrHitInfo2D[i].collider);

        OnCalculate_Inner_And_ExitCollider(_listCollider2D_InCollider, _listCollider2D_NewInner, _listCollider2D_Exit);

        if (_listCollider2D_NewInner.Count != 0)
        {
            if (p_bIsDebuging)
                Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Enter - " + _listCollider2D_NewInner.ToStringList());

            if (p_Event_OnPhysicsEvent_Custom2D != null)
                p_Event_OnPhysicsEvent_Custom2D.Invoke(_listCollider2D_NewInner, EPhysicsEventCustom.Enter);
        }

        if (_listCollider2D_InCollider.Count != 0)
        {
            if (p_bIsDebuging)
                Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Stay - " + _listCollider2D_InCollider.ToStringList());

            if(p_Event_OnPhysicsEvent_Custom2D != null)
                p_Event_OnPhysicsEvent_Custom2D.Invoke(_listCollider2D_InCollider, EPhysicsEventCustom.Stay);
        }

        if (_listCollider2D_Exit.Count != 0)
        {
            if (p_bIsDebuging)
                Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Exit - " + _listCollider2D_Exit.ToStringList());

            if (p_Event_OnPhysicsEvent_Custom2D != null)
                p_Event_OnPhysicsEvent_Custom2D.Invoke(_listCollider2D_Exit, EPhysicsEventCustom.Exit);
        }

        _listCollider2D_InCollider.AddRange(_listCollider2D_NewInner);

        for (int i = 0; i < _listCollider2D_Exit.Count; i++)
            _listCollider2D_InCollider.Remove(_listCollider2D_Exit[i]);


        if (p_bIsDebuging)
            Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Current - " + _listCollider2D_InCollider.ToStringList());
    }

    private void SetCollider(Transform pTransformTarget, CircleCollider2D pCircleCollider2D)
    {
        pCircleCollider2D.enabled = false;

        p_pTransformTarget = pTransformTarget;
        _eColliderType = EColliderType.CircleCollider_2D;
        _vecOffset = pCircleCollider2D.offset;
        _fRadius = pCircleCollider2D.radius;
        _OnGetHit = GetHit2D_CircleCollider;
    }

    private void SetCollider(Transform pTransformTarget, BoxCollider2D pBoxCollider2D)
    {
        pBoxCollider2D.enabled = false;

        p_pTransformTarget = pTransformTarget;
        _pBoxCollider2D_Current = pBoxCollider2D;
        _eColliderType = EColliderType.BoxCollider_2D;
        UpdateSize(_pBoxCollider2D_Current);

        _OnGetHit = GetHit2D_BoxCollider;
    }

    private void UpdateSize(BoxCollider2D pBoxCollider2D)
    {
        _vecSize = pBoxCollider2D.size;
        _vecSize.x *= p_pTransformTarget.lossyScale.x;
        if (_vecSize.x < 0f)
            _vecSize.x *= -1f;

        _vecSize.y *= p_pTransformTarget.lossyScale.y;
        _vecSize.z *= p_pTransformTarget.lossyScale.x;

        _vecOffset = pBoxCollider2D.offset * p_pTransformTarget.lossyScale;
    }

    int GetHit2D_BoxCollider()
    {
        if (p_bUpdateTransform)
            UpdateSize(_pBoxCollider2D_Current);

        return Physics2D.BoxCastNonAlloc(p_pTransformTarget.position + (p_pTransformTarget.rotation * _vecOffset), _vecSize, 0f, Vector2.zero, _arrHitInfo2D, 0f, p_pLayerMask);
    }

    int GetHit2D_CircleCollider()
    {
        return Physics2D.CircleCastNonAlloc(p_pTransformTarget.position + _vecOffset, _fRadius, Vector2.zero, _arrHitInfo2D, 0f, p_pLayerMask);
    }

#endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test