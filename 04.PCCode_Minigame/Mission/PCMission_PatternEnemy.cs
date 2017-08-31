using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCMission_PatternEnemy : PCMission_PatternBase<EMissionEnemy, PCMission_Enemy>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void ProcShotGenerate( Vector3 vecBulletPos, Quaternion rotBulletAngle )
	{
		PCMission_Enemy pResource = CManagerPooling<EMissionEnemy, PCMission_Enemy>.instance.DoPop( p_eGenerateKey );
		pResource.transform.position = vecBulletPos;
		pResource.transform.rotation = Quaternion.identity;
		pResource.transform.localScale = Vector3.one;

		PCMission_EnemyMove pEnemyMove = pResource.GetComponent<PCMission_EnemyMove>();
		if(pEnemyMove.p_eMoveType != EMissionEnemyMoveType.WayPoint)
			pEnemyMove.DoMoveToDirection( _pTransformCached.up );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
