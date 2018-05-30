#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-12 오후 12:25:24
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CRaycastCalculator : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EDimensionType
    {
        TwoD,
        ThreeD
    }

    public struct RaycastOrigins
    {
        public Vector2 vecBound_TopLeft, vecBound_TopRight;
        public Vector2 vecBound_BottomLeft, vecBound_BottomRight;

        public Vector2 vecRayOrigin_VerticalUp;
        public Vector2 vecRayOrigin_VerticalDown;
        public float fRayLength_Vertical;
    }

    /* public - Field declaration            */

    [Rename_Inspector("가로 방향 레이 개수", false)]
    public int _iHorizontalRayCount;
    [Rename_Inspector("세로 방향 레이 개수", false)]
    public int _iVerticalRayCount;

    [Rename_Inspector("레이와 레이 사이 갭 - 좌우")]
    public float _fDstBetweenRays_Horizontal = .25f;
    [Rename_Inspector("레이와 레이 사이 갭 - 상하")]
    public float _fDstBetweenRays_Vertical = .25f;

    [Rename_Inspector("스킨 두께 - 가로")]
    public float _fSkinWidth_Horizontal = .03f;
    [Rename_Inspector("스킨 두께 - 세로")]
    public float _fSkinWidth_Vertical = .03f;

    [Rename_Inspector("좌우 방향 무시할 레이 개수")]
    public int _iIgnoreRayCount_Horizontal = 0;
    [Rename_Inspector("아래 방향 무시할 레이 개수")]
    public int _iIgnoreRayCount_Vertical = 0;

    [HideInInspector]
    public float _fHorizontalRaySpacing;
    [HideInInspector]
    public float _fVerticalRaySpacing;

    public RaycastOrigins _pRaycastOrigins;

    public Collider p_pCollider { get; private set; }
    public Collider2D p_pCollider2D { get; private set; }
    public EDimensionType p_eDimensionType { get; private set; }

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSetCollider(Collider pCollider)
    {
        p_pCollider = pCollider;
        DoCalculateRaySpacing();
        p_eDimensionType = EDimensionType.ThreeD;
    }

    public void DoSetCollider(Collider2D pCollider)
    {
        p_pCollider2D = pCollider;
        DoCalculateRaySpacing();
        p_eDimensionType = EDimensionType.TwoD;
    }

    public void DoUpdateRaycastOrigins()
    {
        Bounds bounds = GetBounds();

        _pRaycastOrigins.vecRayOrigin_VerticalUp = bounds.center;
        _pRaycastOrigins.vecRayOrigin_VerticalUp.y += _fSkinWidth_Vertical;
        _pRaycastOrigins.vecRayOrigin_VerticalDown = bounds.center;
        _pRaycastOrigins.vecRayOrigin_VerticalDown.y -= _fSkinWidth_Vertical;

        _pRaycastOrigins.fRayLength_Vertical = Mathf.Abs(bounds.max.y - bounds.center.y) - _fSkinWidth_Vertical;
        _pRaycastOrigins.vecBound_BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _pRaycastOrigins.vecBound_BottomRight = new Vector2(bounds.max.x , bounds.min.y);
        _pRaycastOrigins.vecBound_TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        _pRaycastOrigins.vecBound_TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void DoCalculateRaySpacing()
    {
        if (p_pCollider == null && p_pCollider2D == null)
        {
            Debug.LogError("DoCalculateRaySpacing - p_pCollider == null && p_pCollider2D == null");
            return;
        }

        Bounds bounds = GetBounds();
        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        _iHorizontalRayCount = Mathf.RoundToInt(boundsHeight / _fDstBetweenRays_Horizontal);
        _iVerticalRayCount = Mathf.RoundToInt(boundsWidth / _fDstBetweenRays_Vertical);

        _fHorizontalRaySpacing = bounds.size.y / (_iHorizontalRayCount - 1);
        _fVerticalRaySpacing = bounds.size.x / (_iVerticalRayCount - 1);
    }


    public Bounds GetBounds()
    {
        Bounds pBounds;
        if (p_pCollider2D)
            pBounds = p_pCollider2D.bounds;
        else
            pBounds = p_pCollider.bounds;

        // pBounds.Expand((_fSkinWidth_Horizontal) * -2);
        
        return pBounds;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (GetComponent<Collider2D>())
            DoSetCollider(GetComponent<Collider2D>());
        else if(GetComponent<Collider>())
            DoSetCollider(GetComponent<Collider>());
    }


    /* protected - [abstract & virtual]         */

    // ========================================================================== //

    #region Private

    #endregion Private
}