using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCMission_EnemyRex : PCMission_Enemy
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

		LeftDown,
		Down,
		RightDown,
	}

	public enum EPatternName
	{
		Pattern_1_Circle_Robot,

		Pattern_2_SprialBlue_Robot,
		Pattern_2_SprialRed_Robot,

		Pattern_3_DoubleLine_Robot,
		Pattern_4_AimShot_Robot,

		Pattern_5_AimShot_Robot,

		Pattern_1_Circle_Airplane,
		Pattern_1_RotateShot_DownToRight_Airplane,
		Pattern_1_RotateShot_DownToLeft_Airplane,
		Pattern_1_3_Airplane,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	[SerializeField]
	private Transform _pTargetTest = null;

	private TweenPosition _pTweenPos;

	[SerializeField]
	private bool _bIsTransformRobot = false;

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

		_bIsBoss = true;
		GetComponent( out _pTweenPos );

		EventDelegate.Add( _pTweenPos.onFinished, OnFinishTweenPos );
		_pEnemyMove.p_EVENT_OnArriveWayPoint += PCMission_Boss_1_p_EVENT_OnArriveWayPoint;
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		_pTweenPos.enabled = true;
		_pTweenPos.ResetToBeginning();
		_pTweenPos.PlayForward();

		_pEnemyMove.enabled = false;
		EventSetColliderOnOff( false );
		EventSetMovePlay( false );
		EventStopAllBulletMuzzle();

		PCManagerInMission.instance.DoPlayOrStop_CheckPlayerPos(false);

		// TestCode
		//_pTweenPos.enabled = false;
		//_bIsTransformRobot = true;
		//EventSetColliderOnOff( false );
		//_pAnimator.DoPlayAnimation( EAnimationName.Bosstransform, OnFinishTransformAnimation );
		//EventStopAllBulletMuzzle();
	}

	protected override void OnDecreaseHP( float fHPPercentage )
	{
		base.OnDecreaseHP( fHPPercentage );

		if(_bIsTransformRobot == false && fHPPercentage <= 0.5f)
		{
			ProcTransformRobot();
		}

		if (fHPPercentage <= 0f)
		{
			PCManagerInMission.instance.DoShowSpaceStation();
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void PCMission_Boss_1_p_EVENT_OnArriveWayPoint( string obj )
	{
		//Debug.Log(obj);

		if(_bIsTransformRobot)
		{
			EventStopAllBulletMuzzle(true);
			if (obj == EWayPoint.LeftUp.ToString_GarbageSafe() || obj == EWayPoint.RightUp.ToString_GarbageSafe())
			{
				int iRand = Random.Range( 0, 2 );
				if (iRand == 0)
				{
					_pEnemyMove.EventSetEnemyMoveSpeed( 1.5f );
					if (obj == EWayPoint.LeftUp.ToString_GarbageSafe())
						_pEnemyMove.DoMoveToWayPoint( EWayPoint.RightUp.ToString_GarbageSafe(), 1f, ProcMoveToWayPoint_LeftUp );
					else
						_pEnemyMove.DoMoveToWayPoint( EWayPoint.LeftUp.ToString_GarbageSafe(), 1f, ProcMoveToWayPoint_RightUp );

					EventGetBulletMuzzle( EPatternName.Pattern_3_DoubleLine_Robot ).DoPlayPattern();
				}
				else if (iRand == 1)
				{
					EventGetBulletMuzzle( EPatternName.Pattern_4_AimShot_Robot ).DoPlayPattern();
				}
			}

			else
			{
				int iRand = Random.Range( 0, 3 );
				if(obj.Contains (EWayPoint.Down.ToString_GarbageSafe()))
					iRand = Random.Range( 0, 2 );

				if(obj.Contains(EWayPoint.Left.ToString_GarbageSafe()) || obj.Contains(EWayPoint.Right.ToString_GarbageSafe()))
				{
					EventGetBulletMuzzle( EPatternName.Pattern_1_Circle_Robot ).DoPlayPattern();
				}
				else
				{
					if (iRand == 0)
					{
						EventGetBulletMuzzle( EPatternName.Pattern_1_Circle_Robot ).DoPlayPattern();
					}
					else if (iRand == 1)
					{
						_pEnemyMove.EventSetEnemyMoveSpeed( 0f );
						EventGetBulletMuzzle( EPatternName.Pattern_2_SprialBlue_Robot ).DoPlayPattern( ProcPlaySprial_Red );
					}
					else if (iRand == 2)
					{
						EventGetBulletMuzzle( EPatternName.Pattern_5_AimShot_Robot ).DoPlayPattern();
					}
				}
			}
		}
		else
		{
			if (obj == EWayPoint.LeftUp.ToString_GarbageSafe())
			{
				EventGetBulletMuzzle(EPatternName.Pattern_1_Circle_Airplane).DoStopPattern();
				EventGetBulletMuzzle(EPatternName.Pattern_1_RotateShot_DownToRight_Airplane).DoPlayPattern();
			}

			else if (obj == EWayPoint.Up.ToString_GarbageSafe())
			{
				EventGetBulletMuzzle(EPatternName.Pattern_1_Circle_Airplane).DoPlayPattern();
				EventGetBulletMuzzle(EPatternName.Pattern_1_3_Airplane).DoPlayPattern();
			}

			else if (obj == EWayPoint.RightUp.ToString_GarbageSafe())
			{
				EventGetBulletMuzzle(EPatternName.Pattern_1_Circle_Airplane).DoStopPattern();
				EventGetBulletMuzzle(EPatternName.Pattern_1_RotateShot_DownToLeft_Airplane).DoPlayPattern();
			}

			else
			{
				EventGetBulletMuzzle(EPatternName.Pattern_1_Circle_Airplane).DoPlayPattern();
			}
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */
	   
	private void ProcPlaySprial_Red()
	{
		EventGetBulletMuzzle(EPatternName.Pattern_2_SprialBlue_Robot).DoSetGenrating(false);
		EventGetBulletMuzzle(EPatternName.Pattern_2_SprialRed_Robot).DoSetGenrating(true);
		EventGetBulletMuzzle(EPatternName.Pattern_2_SprialRed_Robot).DoPlayPattern(ProcFinishSprial);
	}

	private void ProcFinishSprial()
	{
		EventGetBulletMuzzle(EPatternName.Pattern_2_SprialBlue_Robot).DoStopPattern(true);
		EventGetBulletMuzzle(EPatternName.Pattern_2_SprialRed_Robot).DoStopPattern(true);
		_pEnemyMove.EventSetEnemyMoveSpeed(1f);
	}

	private void ProcMoveToWayPoint_RightUp()
	{
		_pEnemyMove.DoMoveToWayPoint( EWayPoint.RightUp.ToString_GarbageSafe(), 0f, ProcFinish_PingPong );
	}

	private void ProcMoveToWayPoint_LeftUp()
	{
		_pEnemyMove.DoMoveToWayPoint( EWayPoint.LeftUp.ToString_GarbageSafe(), 0f, ProcFinish_PingPong );
	}
	
	private void ProcFinish_PingPong()
	{
		EventStopAllBulletMuzzle();
		_pEnemyMove.EventSetEnemyMoveSpeed_Origin();
		_pEnemyMove.DoMovePlay();
	}

	private void OnFinishTweenPos()
	{
		_pEnemyMove.enabled = true;
		EventSetColliderOnOff(true);
		EventSetMovePlay(true);
	}

	private void ProcTransformRobot()
	{
		_bIsTransformRobot = true;
		EventSetColliderOnOff( false );
		EventStopAllBulletMuzzle();
		_pEnemyMove.EventSetEnemyMoveSpeed( 0f );
		_pAnimator.DoPlayAnimation( EAnimationName.Bosstransform, OnFinishTransformAnimation );
	}

	private void OnFinishTransformAnimation()
	{
		_pAnimator.DoPlayAnimation( EAnimationName.Bossidle );
		EventSetColliderOnOff( true );
		_pEnemyMove.EventSetEnemyMoveSpeed( 1f );
		DoRotateAngle( Vector3.zero );
	}

}
