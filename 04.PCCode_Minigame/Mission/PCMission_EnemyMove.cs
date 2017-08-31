using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public enum EMissionEnemyMoveType
{
	None,
	PingPongDown,
	Y,
	WayPoint,
}

public class PCMission_EnemyMove : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EMoveDirection
	{
		Left,
		Right,
	}

	/* public - Variable declaration            */

	public event System.Action<string> p_EVENT_OnArriveWayPoint;

	[SerializeField]
	private EMoveDirection _eMoveDirection = EMoveDirection.Left;
	[SerializeField]
	private EMissionEnemyMoveType _eMoveType = EMissionEnemyMoveType.None;	public EMissionEnemyMoveType p_eMoveType {  get { return _eMoveType; } }
	[SerializeField]
	private PCCompo_WayPoint _pWayPoint = null;	public PCCompo_WayPoint p_pWayPoint {  get { return _pWayPoint; } }
	[SerializeField]
	private float _fDelayWaitSec = 0.5f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	[SerializeField] // For Test
	private float _fSpeed = 1f;
	[SerializeField]
	private float _fSpeed_Origin = 1f;

	private float _fLastDistance;
	private Transform _pTransModel;
	private System.Action _OnTouchStageSide;
	private System.Func<IEnumerator> _OnMove;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoMoveToDirection(Vector3 vecDirection)
	{
		StopAllCoroutines();
		StartCoroutine( CoMove_LineDirection( vecDirection ) );
	}

	public void DoInitEnemyMove( float fSpeed )
	{
		_fSpeed = fSpeed;
		_fSpeed_Origin = _fSpeed;
	}

	public void DoMoveToWayPoint(string strWayPointName, float fArriveWaitSec, System.Action OnArriveWayPoint)
	{
		StopAllCoroutines();
		StartCoroutine(CoMove_WayPoint( strWayPointName, fArriveWaitSec, false, OnArriveWayPoint ));
	}

	public void DoMovePlay()
	{
		StopAllCoroutines();
		_OnTouchStageSide = ProcEmptyFunction;
		switch (_eMoveType)
		{
			case EMissionEnemyMoveType.PingPongDown:
				_OnMove = CoMove_PingPong;
				_OnTouchStageSide = ProcOnSide_PingPong;
				break;

			case EMissionEnemyMoveType.Y:
				_OnMove = CoMove_Y;
				break;

			case EMissionEnemyMoveType.WayPoint:
				_pWayPoint = PCManagerWayPoint.instance.GetWayPoint( GetComponent<PCMission_Enemy>().eMissionEnemy );
				_OnMove = CoMove_WayPoint; break;
		}

		if (_eMoveType != EMissionEnemyMoveType.None && _OnMove != null)
			StartCoroutine( _OnMove() );
	}

	public void DoMoveStop()
	{
		StopAllCoroutines();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void EventSetEnemyMoveSpeed(float fMagnification)
	{
		_fSpeed = _fSpeed_Origin * fMagnification;
		//Debug.Log( _fSpeed );
	}

	public void EventSetEnemyMoveSpeed_Origin()
	{
		_fSpeed = _fSpeed_Origin;
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		if(_eMoveType == EMissionEnemyMoveType.PingPongDown)
			_pTransModel = GetGameObject("Model").transform;
	}

	//private void OnTriggerEnter2D( Collider2D collision )
	//{
	//	PCGameObstacle pObstacle = collision.gameObject.GetComponent<PCGameObstacle>();
	//	if (pObstacle != null && pObstacle.p_eObstacleType == EObstacleType.Wall_Side)
	//		_OnTouchStageSide();
	//}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */
	   
	private void ProcEmptyFunction()
	{

	}

	private void ProcOnSide_PingPong()
	{
		if (_eMoveDirection == EMoveDirection.Left)
		{
			_eMoveDirection = EMoveDirection.Right;
			_pTransModel.localRotation = Quaternion.Euler( 0f, 180f, 0f );
		}
		else if (_eMoveDirection == EMoveDirection.Right)
		{
			_eMoveDirection = EMoveDirection.Left;
			_pTransModel.localRotation = Quaternion.Euler( Vector3.zero );
		}
	}

	private IEnumerator CoMove_PingPong()
	{
		while(true)
		{
			if (_eMoveDirection == EMoveDirection.Left)
				_pTransformCached.Translate( new Vector3( -_fSpeed, -_fSpeed ) * Time.deltaTime, Space.World );
			else if (_eMoveDirection == EMoveDirection.Right)
				_pTransformCached.Translate( new Vector3( _fSpeed, -_fSpeed ) * Time.deltaTime, Space.World );

			yield return null;
		}
	}

	private IEnumerator CoMove_Y()
	{
		// 시작은 대각선 이동
		while(_pTransformCached.localPosition.x > _fSpeed || _pTransformCached.localPosition.x < -_fSpeed)
		{
			float fPosX = _pTransformCached.localPosition.x;
			bool bIsRight = fPosX > _fSpeed;
			if (bIsRight)
				_pTransformCached.Translate(new Vector3(-_fSpeed, -_fSpeed) * Time.deltaTime, Space.World);
			else
				_pTransformCached.Translate(new Vector3(_fSpeed, -_fSpeed) * Time.deltaTime, Space.World);

			// 움직인 뒤 부호가 바뀌면 중단
			if (bIsRight && _pTransformCached.localPosition.x < 0f)
				break;
			else if (bIsRight == false && _pTransformCached.localPosition.x > 0f)
				break;

			yield return null;
		}

		// 여기에 오면 중앙에 온것이다..
		// 3분의 1정도 오면 다시 꺾임.
		while (true)
		{
			_pTransformCached.Translate(new Vector3(0f, -_fSpeed) * Time.deltaTime, Space.World);

			yield return null;
		}
	}

	private IEnumerator CoMove_WayPoint()
	{
		if (_eMoveType == EMissionEnemyMoveType.WayPoint && _pWayPoint == null)
		{
			//Debug.LogWarning( "무브 타입이 웨이포인트인데 웨이포인트가 없습니다", this );
			yield break;
		}
		
		while (true)
		{
			string strWayPointName = _pWayPoint.GetRandomWayPointName();			
			yield return StartCoroutine( CoMove_WayPoint( strWayPointName, _fDelayWaitSec, true ) );
		}
	}

	private IEnumerator CoMove_WayPoint( string strWayPointName, float fDelayWaitSec, bool bExcuteEvent, System.Action OnArriveWayPoint = null)
	{
		Vector3 vecDestination = _pWayPoint.GetWayPointPos( strWayPointName );
		Vector3 vecDirection = vecDestination - _pTransformCached.position;
		vecDirection.z = 0;
		vecDirection.Normalize();
		_fLastDistance = float.MaxValue;
		while (true)
		{
			_pTransformCached.Translate( vecDirection * _fSpeed * Time.deltaTime, Space.World );

			float fDistance = Vector3.Distance( vecDestination, _pTransformCached.position );
			if (fDistance < 0.05f || _fLastDistance < fDistance || fDistance > 10f)
			{
				//Debug.Log( "vecDestination" + vecDestination + "_pTransformCached.position " + _pTransformCached.position  + "fDistance" + fDistance );
				if (bExcuteEvent && p_EVENT_OnArriveWayPoint != null)
					p_EVENT_OnArriveWayPoint( strWayPointName );

				if (OnArriveWayPoint != null)
					OnArriveWayPoint();

				yield return new WaitForSeconds( fDelayWaitSec );
				break;
			}

			_fLastDistance = fDistance;
			yield return null;
		}
	}

	private IEnumerator CoMove_LineDirection(Vector3 vecDirection)
	{
		while (true)
		{
			_pTransformCached.Translate( vecDirection * _fSpeed * Time.deltaTime, Space.World );
			yield return null;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
