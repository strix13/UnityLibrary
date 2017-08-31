using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCMission_EnemyJeff : PCMission_Enemy
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EWayPoint
	{
		LeftUp,
		RightUp,
		Up,

		Left,
		Middle,
		Right,
	}

	public enum EPatternName
	{
		Pattern_1_33,

		Pattern_2_RotateShot_DownToRight_21,
		Pattern_2_RotateShot_DownToLeft_21,

		Pattern_3_RotateShot_DownToRight_13,
		Pattern_3_RotateShot_DownToLeft_13,
	}

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

	protected override void OnAwake()
	{
		base.OnAwake();

		_pEnemyMove.p_EVENT_OnArriveWayPoint += _pEnemyMove_p_EVENT_OnArriveWayPoint;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
	   로직을 처리(Process Local logic)           */

	private void _pEnemyMove_p_EVENT_OnArriveWayPoint( string obj )
	{
		if (obj.Contains( EWayPoint.Left.ToString_GarbageSafe() ))
		{
			int iRand = Random.Range( 0, 2 );
			if (iRand == 0)
				EventGetBulletMuzzle( EPatternName.Pattern_2_RotateShot_DownToRight_21 ).DoPlayPattern();
			if (iRand == 1)
				EventGetBulletMuzzle( EPatternName.Pattern_3_RotateShot_DownToRight_13 ).DoPlayPattern();
		}

		else if (obj.Contains( EWayPoint.Right.ToString_GarbageSafe() ))
		{
			int iRand = Random.Range( 0, 2 );
			if (iRand == 0)
				EventGetBulletMuzzle( EPatternName.Pattern_2_RotateShot_DownToLeft_21 ).DoPlayPattern();
			if (iRand == 1)
				EventGetBulletMuzzle( EPatternName.Pattern_3_RotateShot_DownToLeft_13 ).DoPlayPattern();
		}
		else
			EventGetBulletMuzzle( EPatternName.Pattern_1_33 ).DoPlayPattern();
	}

	/* private - Other[Find, Calculate] Func 
	   찾기, 계산등 단순 로직(Simpe logic)         */

}
