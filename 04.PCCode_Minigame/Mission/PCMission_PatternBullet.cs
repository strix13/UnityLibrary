using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCMission_PatternBullet : PCMission_PatternBase<EMissionBullet, PCMission_Bullet>, IDictionaryItem<string>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	[Header( "총알 관련 테스트 옵션" )]
	[SerializeField]
	private bool _bOwnerIsPlayer;
	[SerializeField]
	private int _iBulletDamage;
	[SerializeField]
	private float _fBulletSpeed;
	[SerializeField]
	private bool _bMoveStopOnPlayPattern = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCMission_Enemy _pEnemyOwner;
	private PCMission_EnemyMove _pEnemyOwnerMove;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetBulletInfo( bool bOwnerIsPlayer, float fGenerateDelaySec, int iBulletDamage, float fBulletSpeed )
	{
		_bOwnerIsPlayer = bOwnerIsPlayer;
		_iBulletDamage = iBulletDamage;
		_fBulletSpeed = fBulletSpeed;
		_fDelaySec_Generate = fGenerateDelaySec;
	}

	public string IDictionaryItem_GetKey()
	{
		return name;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pEnemyOwner = GetComponentInParent<PCMission_Enemy>();
		_pEnemyOwnerMove = GetComponentInParent<PCMission_EnemyMove>();
	}

	protected override void OnPatternPlay()
	{
		base.OnPatternPlay();

		if (_bMoveStopOnPlayPattern)
		{
			//Debug.Log("_bMoveStopOnPlayPattern On" + name, this);

			if (_pEnemyOwnerMove != null)
				_pEnemyOwnerMove.EventSetEnemyMoveSpeed(0f);
		}
	}

	protected override void OnPatternStop()
	{
		base.OnPatternStop();

		if (_bMoveStopOnPlayPattern)
		{
			//Debug.Log("_bMoveStopOnPlayPattern Off" + name, this);

			if (_pEnemyOwnerMove != null)
				_pEnemyOwnerMove.EventSetEnemyMoveSpeed(1f);
		}
	}

	protected override void OnGenerateSomthing( EMissionBullet eKey, PCMission_Bullet pResource )
	{
		base.OnGenerateSomthing( eKey, pResource );

		pResource.DoInitBullet( _bOwnerIsPlayer, _iBulletDamage, _fBulletSpeed, transform );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
