#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-12 오후 12:29:48
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPlatformerCalculator : CRaycastCalculator
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum ECollisionIgnoreType
    {
        None,
        Though_Up,
        Though_Down,
    }

    [System.Serializable]
    public struct CollisionInfo
    {
        public bool above;
        public bool below { get; private set; }
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope { get; private set; }

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int iFaceDir_OneIsLeft { get; private set; }
        public bool fallingThroughPlatform;

        [HideInInspector]
        public List<Transform> _listHitTransform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            DoSetSlopeSliding(false);
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            _listHitTransform.Clear();
        }

        System.Action<bool> _OnChangeBelow;
        System.Action<int> _OnChangeFaceDir;
        System.Action<bool> _OnChangeSlopeSliding;

        public void DoSet_Below(bool bBelow)
        {
            if (below != bBelow)
            {
                below = bBelow;
                _OnChangeBelow(bBelow);
            }
        }

        public void DoSet_Below(System.Action<bool> OnChangeBelow)
        {
            _OnChangeBelow = OnChangeBelow;
        }

        public void DoSet_OnChangeSlopeSliding(System.Action<bool> OnChangeSlopeSliding)
        {
            this._OnChangeSlopeSliding = OnChangeSlopeSliding;
        }

        public void DoSet_OnChangeFaceDir(System.Action<int> OnChangeFaceDir)
        {
            this._OnChangeFaceDir = OnChangeFaceDir;
        }

        public void DoSetSlopeSliding(bool bSliding)
        {
            if (slidingDownMaxSlope != bSliding)
            {
                if (_OnChangeSlopeSliding != null)
                    _OnChangeSlopeSliding(bSliding);

                slidingDownMaxSlope = bSliding;
            }
        }

        public void DoSetFaceDir_OneIsLeft(int iFaceDir)
        {
            if (iFaceDir_OneIsLeft != iFaceDir)
            {
                if (_OnChangeFaceDir != null)
                    _OnChangeFaceDir(iFaceDir);

                iFaceDir_OneIsLeft = iFaceDir;
            }
        }
    }

    /* public - Field declaration            */

    public delegate void OnHit_VerticalCollider(Transform pTransformHit, out ECollisionIgnoreType eIgnnore_ThisHitInfo);

    public bool bDebugMode;

    public LayerMask collisionMask;

    [Rename_Inspector("밑에서 몇번째 좌우 레이까지 닿았을 때 언덕으로 인식할 것인가")]
    public int _iRayIndex_HitIsSlope = 0;
    //[Rename_Inspector("레이 원점 X 오프셋")]
    //public float _fRayOriginOffset_X = 0f;
    //[Rename_Inspector("레이 원점 Y 오프셋")]
    //public float _fRayOriginOffset_Y = 0f;
    [Rename_Inspector("처음 바라보는 방향")]
    public int p_iFaceDir_OnAwake = 1;
    public float maxSlopeAngle = 80;

    public CollisionInfo p_pCollisionInfo;
    [HideInInspector]
    public Vector2 _vecPlayerInput;
    Vector3 _vecMoveAmountOrigin;
    Vector3 _vecMoveAmountResult;

    public int _iHorizontalHitCount { get; private set; }
    public int _iVerticalHitCount { get; private set; }

    /* protected & private - Field declaration         */

    OnHit_VerticalCollider _OnHit_VerticalCollider;
    float _fSlopeAngle_Last;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_CalcaulateVerticalCollider(OnHit_VerticalCollider OnHit_VerticalCollider)
    {
        _OnHit_VerticalCollider = OnHit_VerticalCollider;
    }

    public void DoSet_OnChangeFaceDir(System.Action<int> OnChangeFaceDir)
    {
        p_pCollisionInfo.DoSet_OnChangeFaceDir(OnChangeFaceDir);
    }
    
    public void DoSet_OnChangeSlopeSliding(System.Action<bool> OnChangeSlopeSliding)
    {
        p_pCollisionInfo.DoSet_OnChangeSlopeSliding(OnChangeSlopeSliding);
    }

    public void DoSet_OnChangeBelow(System.Action<bool> OnChangeBelow)
    {
        p_pCollisionInfo.DoSet_Below(OnChangeBelow);
    }

    public void DoMove(Vector2 moveAmount, bool standingOnPlatform)
    {
        DoMove(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void DoMove(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        DoUpdateRaycastOrigins();

        p_pCollisionInfo.Reset();
        p_pCollisionInfo.moveAmountOld = moveAmount;
        _vecPlayerInput = input;

        _vecMoveAmountOrigin = moveAmount;
        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            p_pCollisionInfo.DoSetFaceDir_OneIsLeft((int)Mathf.Sign(moveAmount.x));
        }

        HorizontalCollision(ref moveAmount);
        //if (moveAmount.y != 0)
        {
            VerticalCollision(ref moveAmount, input.x != 0f);
        }
        _vecMoveAmountResult = moveAmount;
        transform.Translate(moveAmount);
    }

    public void DoIgnoreCollider(float fRestoreSeconds)
    {
        p_pCollisionInfo.fallingThroughPlatform = true;
        Invoke("ResetFallingThroughPlatform", fRestoreSeconds);
    }
    
    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        p_pCollisionInfo._listHitTransform = new List<Transform>();
        p_pCollisionInfo.DoSetFaceDir_OneIsLeft(p_iFaceDir_OnAwake);
    }

#if UNITY_EDITOR
    Vector3 vecDebugOffset = new Vector2(1f, 1f);

    private void OnDrawGizmos()
    {
        if (bDebugMode == false) return;

        Vector3 vecPos = transform.position + vecDebugOffset;
        UnityEditor.Handles.Label(vecPos, "SlopeAngle_Last : " + _fSlopeAngle_Last);

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "collisions.climbingSlope : " + p_pCollisionInfo.climbingSlope);

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "_pCollisionInfo.slidingDownMaxSlope : " + p_pCollisionInfo.slidingDownMaxSlope);

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "_iHorizontalHitCount : " + _iHorizontalHitCount);

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "_iVerticalHitCount : " + _iVerticalHitCount);

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "_vecPlayerInput : " + _vecPlayerInput.ToString("F4"));

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "_vecMoveAmountOrigin : " + _vecMoveAmountOrigin.ToString("F4"));

        vecPos.y -= 0.5f;
        UnityEditor.Handles.Label(vecPos, "_vecMoveAmountResult : " + _vecMoveAmountResult.ToString("F4"));
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    void HorizontalCollision(ref Vector2 moveAmount)
    {
        float directionX = p_pCollisionInfo.iFaceDir_OneIsLeft;
        float rayLength = Mathf.Abs(moveAmount.x) + _fSkinWidth_Horizontal;

        if (Mathf.Abs(moveAmount.x) < _fSkinWidth_Horizontal)
        {
            rayLength = _fSkinWidth_Horizontal * 2f;
        }

        _iHorizontalHitCount = 0;
        moveAmount = HorizontalCollision(moveAmount, directionX, rayLength);
    }

    private Vector2 HorizontalCollision(Vector2 moveAmount, float directionX, float rayLength)
    {
        for (int i = 0; i < _iHorizontalRayCount; i++)
        {
            if (i < _iIgnoreRayCount_Horizontal)
                continue;

            Vector2 rayOrigin = (directionX == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_BottomRight;
            // rayOrigin.x += (directionX == -1) ? _fRayOriginOffset_X * -1 : _fRayOriginOffset_X;

            rayOrigin += Vector2.up * (_fHorizontalRaySpacing * i);
            CRaycastHitWrapper pHit;
            if (p_eDimensionType == EDimensionType.TwoD)
                pHit = CRaycastHitWrapper.Raycast2D(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            else
                pHit = CRaycastHitWrapper.Raycast3D(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (pHit)
            {
                if (i <= _iRayIndex_HitIsSlope)
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, new Color(0f, 0.5f, 0f, 1f));
                else
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);
                Debug.DrawRay(rayOrigin, pHit.normal, Color.yellow);
            }
            else
            {
                if ( i <= _iRayIndex_HitIsSlope )
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.magenta);
                else
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            }

            if (pHit && pHit.transform == transform)
                continue;

            if (pHit)
            {
                p_pCollisionInfo._listHitTransform.Add(pHit.transform);
                _iHorizontalHitCount++;
                if (pHit.distance == 0)
                {
                    continue;
                }
                _fSlopeAngle_Last = Vector2.Angle(pHit.normal, Vector2.up);

                if (i <= _iRayIndex_HitIsSlope && _fSlopeAngle_Last <= maxSlopeAngle)
                {
                    if (p_pCollisionInfo.descendingSlope)
                    {
                        p_pCollisionInfo.descendingSlope = false;
                        moveAmount = p_pCollisionInfo.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (_fSlopeAngle_Last != p_pCollisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = pHit.distance - _fSkinWidth_Horizontal;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, _fSlopeAngle_Last, pHit.normal);

                    //Debug.Log(" distanceToSlopeStart : " + distanceToSlopeStart);
                    //moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!p_pCollisionInfo.climbingSlope || _fSlopeAngle_Last > maxSlopeAngle)
                {
                    moveAmount.x = (pHit.distance - _fSkinWidth_Horizontal) * directionX;
                    rayLength = pHit.distance;

                    if (p_pCollisionInfo.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(p_pCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    p_pCollisionInfo.left = directionX == -1;
                    p_pCollisionInfo.right = directionX == 1;
                }
            }
        }

        return moveAmount;
    }


    void VerticalCollision(ref Vector2 moveAmount, bool bIsMoveX)
    {
        float directionY = -1f;
        if (moveAmount.normalized.y != 0f)
            directionY = Mathf.Sign(moveAmount.normalized.y);

        _iVerticalHitCount = 0;
        moveAmount = VerticalCollision(moveAmount, directionY, bIsMoveX);

        float rayLength = Mathf.Abs(moveAmount.y) + _fSkinWidth_Vertical;
        if (p_pCollisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + _fSkinWidth_Vertical;
            Vector2 rayOrigin = ((directionX == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_BottomRight) + Vector2.up * moveAmount.y;

            CalculateClimbSlope(rayOrigin, directionX, rayLength, ref moveAmount);
        }

        bool bIsStandOnPlatform = _iVerticalHitCount != 0 && directionY == -1f;
        if (bIsStandOnPlatform == false)
            p_pCollisionInfo.DoSetSlopeSliding(false);

    }

    private Vector2 VerticalCollision(Vector2 moveAmount, float directionY, bool bIsMoveX)
    {
        bool bDirectionIsLeft = p_pCollisionInfo.iFaceDir_OneIsLeft == 1;
        for (int i = 0; i < _iVerticalRayCount; i++)
        {
            if (bDirectionIsLeft && i < _iIgnoreRayCount_Vertical)
                continue;

            if (bDirectionIsLeft == false && i > _iVerticalRayCount - _iIgnoreRayCount_Vertical)
                continue;

            float rayLength = Mathf.Abs(moveAmount.y) + _pRaycastOrigins.fRayLength_Vertical;
            Vector2 rayOrigin = (directionY == -1) ? _pRaycastOrigins.vecRayOrigin_VerticalDown : _pRaycastOrigins.vecRayOrigin_VerticalUp;
            rayOrigin.x = _pRaycastOrigins.vecBound_BottomLeft.x;
            // rayOrigin.y += (directionY == -1) ? _fRayOriginOffset_Y : _fRayOriginOffset_Y * -1;
            rayOrigin += Vector2.right * (_fVerticalRaySpacing * i + moveAmount.x);

            CRaycastHitWrapper pHit;
            if (p_eDimensionType == EDimensionType.TwoD)
                pHit = CRaycastHitWrapper.Raycast2D(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            else
                pHit = CRaycastHitWrapper.Raycast3D(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (pHit && pHit.transform == transform)
                continue;

            if (pHit)
            {
                Debug.DrawRay(rayOrigin, Vector2.up * pHit.distance * directionY, Color.green);
                Debug.DrawRay(rayOrigin, pHit.normal, Color.yellow);
            }
            else
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (pHit)
            {
                ECollisionIgnoreType eIgnoreType = ECollisionIgnoreType.None;
                if (p_pCollisionInfo.fallingThroughPlatform)
                    continue;

                if (_OnHit_VerticalCollider != null)
                {
                    _OnHit_VerticalCollider(pHit.transform, out eIgnoreType);
                }
                else
                {
                    if (pHit.transform.CompareTag("Through"))
                    {
                        if (directionY == 1 || pHit.distance == 0)
                            eIgnoreType = ECollisionIgnoreType.Though_Up;
                        if (_vecPlayerInput.y == -1)
                            eIgnoreType = ECollisionIgnoreType.Though_Down;
                    }
                }

                if (eIgnoreType != ECollisionIgnoreType.None)
                {
                    if (eIgnoreType == ECollisionIgnoreType.Though_Down)
                            DoIgnoreCollider(0.5f);

                    continue;
                }

                p_pCollisionInfo._listHitTransform.Add(pHit.transform);
                _iVerticalHitCount++;
                moveAmount.y = (pHit.distance - _pRaycastOrigins.fRayLength_Vertical) * directionY;
                //rayLength = hit.distance;

                if (bIsMoveX && p_pCollisionInfo.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(p_pCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                p_pCollisionInfo.DoSet_Below(directionY == -1);
                p_pCollisionInfo.above = directionY == 1;
            }
        }

        return moveAmount;
    }

    void CalculateClimbSlope(Vector2 rayOrigin, float directionX, float rayLength, ref Vector2 moveAmount)
    {
        CRaycastHitWrapper pHit;
        if (p_eDimensionType == EDimensionType.TwoD)
            pHit = CRaycastHitWrapper.Raycast2D(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
        else
            pHit = CRaycastHitWrapper.Raycast3D(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

        if (pHit)
        {
            float slopeAngle = Vector2.Angle(pHit.normal, Vector2.up);
            if (slopeAngle != p_pCollisionInfo.slopeAngle)
            {
                moveAmount.x = (pHit.distance - _fSkinWidth_Horizontal) * directionX;
                p_pCollisionInfo.slopeAngle = slopeAngle;
                p_pCollisionInfo.slopeNormal = pHit.normal;
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            p_pCollisionInfo.DoSet_Below(true);
            p_pCollisionInfo.climbingSlope = true;
            p_pCollisionInfo.slopeAngle = slopeAngle;
            p_pCollisionInfo.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        CRaycastHitWrapper pHitLeft, pHitRight;

        Vector2 vecRayOriginYOffset = new Vector2(0f, _pRaycastOrigins.vecRayOrigin_VerticalDown.y);

        if (p_eDimensionType == EDimensionType.TwoD)
        {

            pHitLeft = Physics2D.Raycast(_pRaycastOrigins.vecBound_BottomLeft + vecRayOriginYOffset, Vector2.down, Mathf.Abs(moveAmount.y) + _fSkinWidth_Vertical, collisionMask);
            pHitRight = Physics2D.Raycast(_pRaycastOrigins.vecBound_BottomRight + vecRayOriginYOffset, Vector2.down, Mathf.Abs(moveAmount.y) + _fSkinWidth_Vertical, collisionMask);
        }
        else
        {
            RaycastHit maxSlopeHitLeft;
            RaycastHit maxSlopeHitRight;
            Physics.Raycast(new Ray(_pRaycastOrigins.vecBound_BottomLeft + vecRayOriginYOffset, Vector2.down), out maxSlopeHitLeft, Mathf.Abs(moveAmount.y) + _pRaycastOrigins.fRayLength_Vertical, collisionMask);
            Physics.Raycast(new Ray(_pRaycastOrigins.vecBound_BottomRight + vecRayOriginYOffset, Vector2.down), out maxSlopeHitRight, Mathf.Abs(moveAmount.y) + _pRaycastOrigins.fRayLength_Vertical, collisionMask);

            pHitLeft = maxSlopeHitLeft;
            pHitRight = maxSlopeHitRight;
        }

        if (pHitLeft ^ pHitRight)
        {
            SlideDownMaxSlope(pHitLeft, ref moveAmount);
            SlideDownMaxSlope(pHitRight, ref moveAmount);
        }

        if (!p_pCollisionInfo.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? _pRaycastOrigins.vecBound_BottomRight : _pRaycastOrigins.vecBound_BottomLeft;
            rayOrigin += vecRayOriginYOffset;

            CRaycastHitWrapper pHit;
            if (p_eDimensionType == EDimensionType.TwoD)
                pHit = CRaycastHitWrapper.Raycast2D(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
            else
                pHit = CRaycastHitWrapper.Raycast3D(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (pHit)
            {
                float slopeAngle = Vector2.Angle(pHit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(pHit.normal.x) == directionX)
                    {
                        if (pHit.distance - _pRaycastOrigins.fRayLength_Vertical <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            p_pCollisionInfo.slopeAngle = slopeAngle;
                            p_pCollisionInfo.descendingSlope = true;
                            p_pCollisionInfo.DoSet_Below(true);
                            p_pCollisionInfo.slopeNormal = pHit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(CRaycastHitWrapper pHit, ref Vector2 moveAmount)
    {
        if (!pHit) return;

        float slopeAngle = Vector2.Angle(pHit.normal, Vector2.up);
        bool bIsSlopeSliding = slopeAngle > maxSlopeAngle;
        if (bIsSlopeSliding)
        {
            moveAmount.x = Mathf.Sign(pHit.normal.x) * (Mathf.Abs(moveAmount.y) - pHit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
            p_pCollisionInfo.slopeAngle = slopeAngle;
            p_pCollisionInfo.DoSetSlopeSliding(true);
            p_pCollisionInfo.slopeNormal = pHit.normal;
        }

        p_pCollisionInfo.DoSetSlopeSliding(bIsSlopeSliding);
    }

    void ResetFallingThroughPlatform()
    {
        p_pCollisionInfo.fallingThroughPlatform = false;
    }

    #endregion Private
}