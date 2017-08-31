using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCMission_BulletGuide : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	[Header( "디버그용" )]
	[SerializeField]
	private Transform _pTransTarget = null;
	[SerializeField]
	private float _fAngleMoveLimit = 30f;
	[SerializeField]
	private float _fAngleSpeed = 1f;
	[SerializeField]
	private float _fAngleMoveCurrent = 0f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCMission_Enemy _pTargetEnemy;
	private bool _bOwnerIsPlayer = false;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitBulletGuide(bool bOwnerIsPlayer)
	{
		_bOwnerIsPlayer = bOwnerIsPlayer;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		_fAngleMoveCurrent = 0f;

		if (_bOwnerIsPlayer)
		{
			//_pTargetEnemy = PCManagerInMission.instance.DoGetClosestEnemy((Vector2)_pTransformCached.position );
			if (_pTargetEnemy != null)
				_pTransTarget = _pTargetEnemy.transform;
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (_pTransTarget == null || _pTargetEnemy.p_iHPCurrent <= 0) return;
		if (_fAngleMoveCurrent >= _fAngleMoveLimit) return;

		Vector2 vecPosTarget = _pTransTarget.position;
		Vector2 vecPosMine = _pTransformCached.position;
		Vector2 vecTargetDirection = vecPosTarget - vecPosMine;
		vecTargetDirection.Normalize();

		float fAngle = Vector2.Angle( _pTransformCached.up, vecTargetDirection );
		Vector3 vecRotateAngle = Vector3.Cross( _pTransformCached.up, vecTargetDirection );

		if (fAngle > _fAngleSpeed)
			fAngle = _fAngleSpeed;
		else if (fAngle < -_fAngleSpeed)
			fAngle = -_fAngleSpeed;

		_pTransformCached.Rotate( vecRotateAngle * fAngle );
		_fAngleMoveCurrent += fAngle;
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		_pTargetEnemy = null;
		_pTransTarget = null;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
