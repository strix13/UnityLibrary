using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public enum EMinigameTile
{
	Default_Tile
}

public class PCMiniTower_Tile : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	private enum EAnimName
	{
		boxlight,
		boxmissdropl,
		boxmissdropr
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private BoxCollider2D _pCollider;
	private Rigidbody2D _pRigidbody;

	private SkeletonAnimation _pSpineAnim;

	private bool _bCollision;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInit()
	{
		_pRigidbody.bodyType = RigidbodyType2D.Kinematic;
	}

	public void DoDrop()
	{
		p_pTransCached.parent = CManagerPooling<EMinigameTile, PCMiniTower_Tile>.instance.p_pObjectManager.transform;

		_pRigidbody.bodyType = RigidbodyType2D.Dynamic;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventOnCollisionEnablePhys(bool bEnable = true)
	{
		_pCollider.enabled = bEnable;

		_pRigidbody.simulated = bEnable;
		_pRigidbody.bodyType = bEnable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
	}

	private void OnCollisionEnter2D(Collision2D pCollider)
	{
		if (_bCollision) return;
		_bCollision = true;

		Transform pTransColl = pCollider.transform;
		BoxCollider2D pCollBox = pTransColl.GetComponent<BoxCollider2D>();

		float fOwnerPosX = transform.localPosition.x;
		float fCollPosX = pTransColl.localPosition.x;

		// 콜라이더 사이즈를 월드 포지션 값으로 변환후 * 0.5 해준다.
		print(pCollBox.size);
		float fCollCenterX = transform.TransformPoint(pCollBox.size).x;
		print(transform.TransformPoint(pCollBox.size));
		float fDistX = Mathf.Abs(fOwnerPosX - fCollPosX);
		if (fDistX > fCollCenterX)
		{
			if (fOwnerPosX < fCollPosX)
				ProcPlaySpineAnim(EAnimName.boxmissdropl);
			else
				ProcPlaySpineAnim(EAnimName.boxmissdropr);

			EventOnCollisionEnablePhys(false);
		}

		PCManagerInMiniTower.instance.EventOnSupplyTile();
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pCollider = GetComponent<BoxCollider2D>();
		_pRigidbody = GetComponent<Rigidbody2D>();
		_pSpineAnim = GetComponent<SkeletonAnimation>();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcPlaySpineAnim(EAnimName eAnimTime, float fTimeScale = 1f, bool fLoop = false)
	{
		if (_pSpineAnim == null)
		{
			Debug.LogWarning("SkeletonAnimation 이 없습니다.", this);
			return;
		}

		_pSpineAnim.AnimationName = eAnimTime.ToString_GarbageSafe();
		_pSpineAnim.timeScale = fTimeScale;
		_pSpineAnim.loop = fLoop;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
