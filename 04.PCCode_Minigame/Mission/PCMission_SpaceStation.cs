using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCMission_SpaceStation : CObjectBase
{
	/* const & readonly declaration             */
	
	/* enum & struct declaration                */

	public enum EStationAnimationName
	{
		Close,
		Idle,
		Open,
		Open2,
	}

	/* public - Variable declaration            */

	[SerializeField]
	private float _fSpeed_Docking = 1f;
	[SerializeField]
	private float _fSpeed_DockingInto = 0.5f;
	[SerializeField]
	private float _fPosOffsetY_OnUnDockingPlayer = 1f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CSpineWrapper _pAnimation;
	private CNGUITweenPositionExtend _pTweenPos;
	private BoxCollider2D _pCollider_In;
	private Spine.Unity.SkeletonAnimation _pAnimationLayer;

	private Transform _pTrans_Docking;
	private Transform _pTrans_Docking_In;

	private GameObject _pObjectDockingShadow;
	private PCMission_Player _pPlayerDocking;

	private System.Action _OnDockingFinish;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoShowStation(System.Action OnDockingFinish)
	{
		_OnDockingFinish = OnDockingFinish;
	}

	public void DoLeaveStation()
	{
		EventDelegate.Add( _pTweenPos.onFinished, OnShowPlayer, true );
		_pTweenPos.DoPlayTween_Forward_1();
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

		GetComponent( out _pAnimation );
		GetComponent( out _pTweenPos );
		GetComponent( out _pCollider_In );
		GetComponentInChildren( out _pAnimationLayer );

		_pTrans_Docking = GetGameObject( "DockingSpot" ).transform;
		_pTrans_Docking_In = GetGameObject( "DockingIn" ).transform;
		_pObjectDockingShadow = GetGameObject( "DockingShadow" );
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		MeshRenderer pMeshRenderer = _pAnimationLayer.GetComponent<MeshRenderer>();
		pMeshRenderer.sortingOrder = -1;

		EventDelegate.Add( _pTweenPos.onFinished, ProcAnimationPlay_Open, true );
		_pTweenPos.DoPlayTween_Forward_0();
		ProcAnimationPlay_Idle();
	}

	//protected override void OnUpdate()
	//{
	//	base.OnUpdate();

	//	if (Input.GetKeyDown( KeyCode.Alpha1 ))
	//		_pAnimation.DoPlayAnimation( EStationAnimationName.Close );

	//	if (Input.GetKeyDown( KeyCode.Alpha2 ))
	//		_pAnimation.DoPlayAnimation( EStationAnimationName.Idle );

	//	if (Input.GetKeyDown( KeyCode.Alpha3 ))
	//		_pAnimation.DoPlayAnimation( EStationAnimationName.Open );

	//	if (Input.GetKeyDown( KeyCode.Alpha4 ))
	//		_pAnimation.DoPlayAnimation( EStationAnimationName.Open2 );

	//}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		_pPlayerDocking = collision.GetComponent<PCMission_Player>();
		if(_pPlayerDocking != null)
		{
			_pPlayerDocking.DoSetDockingMode( true );
			StopCoroutine( "CoAutoEnableDisable_Collider" );
			StartCoroutine( CoDockingTransform( _pPlayerDocking.transform) );
		}
	}



	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcAnimationPlay_Idle()
	{
		_pObjectDockingShadow.SetActive( false );
		_pCollider_In.enabled = false;
		_pAnimation.DoPlayAnimation( EStationAnimationName.Idle, true );
	}

	private void ProcAnimationPlay_Open()
	{
		StartCoroutine( "CoAutoEnableDisable_Collider" );
		_pAnimation.DoPlayAnimation( EStationAnimationName.Open, ProcAnimationPlay_OpenIdle );
	}

	private void ProcAnimationPlay_Close()
	{
		_pObjectDockingShadow.SetActive( false );
		_pCollider_In.enabled = false;
		_pAnimation.DoPlayAnimation( EStationAnimationName.Close, OnDockingFinish );
	}


	private void ProcAnimationPlay_OpenIdle()
	{
		_pAnimation.DoPlayAnimation( EStationAnimationName.Open2, true );
		_pObjectDockingShadow.SetActive( true );
		_pCollider_In.enabled = true;
	}

	private void OnDockingFinish()
	{
		ProcAnimationPlay_Idle();
		if (_OnDockingFinish != null)
			_OnDockingFinish();

		MeshRenderer pMeshRenderer = _pAnimationLayer.GetComponent<MeshRenderer>();
		pMeshRenderer.sortingOrder = 100;
	}

	private IEnumerator CoDockingTransform(Transform pDockingTarget)
	{
		Vector3 vecStartPos = pDockingTarget.position;
		Vector3 vecDestPos = _pTrans_Docking.position;
		float fProgress = 0f;
		while (fProgress < 1f)
		{
			Vector3 vecFollowPos = Vector3.Slerp( vecStartPos, vecDestPos, fProgress );
			pDockingTarget.position = vecFollowPos;

		   fProgress += _fSpeed_Docking * Time.deltaTime;
			yield return null;
		}

		vecStartPos = _pTrans_Docking.position;
		vecDestPos = _pTrans_Docking_In.position;
		fProgress = 0f;
		while (fProgress < 1f)
		{
			Vector3 vecFollowPos = Vector3.Lerp( vecStartPos, vecDestPos, fProgress );
			pDockingTarget.position = vecFollowPos;

			fProgress += _fSpeed_DockingInto * Time.deltaTime;
			yield return null;
		}

		_pPlayerDocking.gameObject.SetActive( false );
		ProcAnimationPlay_Close();
	}

	private void OnShowPlayer()
	{
		EventDelegate.Add( _pTweenPos.onFinished, ProcDisableStation, true );

		Vector3 vecUnDockingPos = _pTransformCached.position;
		vecUnDockingPos.y += _fPosOffsetY_OnUnDockingPlayer;

		_pPlayerDocking.transform.position = vecUnDockingPos;
		_pPlayerDocking.gameObject.SetActive( true );
		_pTweenPos.DoPlayTween_Forward_2();
	}

	private void ProcDisableStation()
	{
		CManagerPooling<EMissionObject, PCMission_SpaceStation>.instance.DoPush( this );
		PCManagerLevel.instance.p_bLock_RegenEnemy = false;
		//PCManagerLevel.instance._iDifficulty += 1;
		_pPlayerDocking.DoSetDockingMode( false );

		PCManagerInMission.instance.DoPlayOrStop_CheckPlayerPos( true );
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	// 플레이어 기체가 도킹 컬라이더에 가만히 있을때 체크하기 위해
	private IEnumerator CoAutoEnableDisable_Collider()
	{
		while(true)
		{
			Vector3 vecSize = _pCollider_In.size;
			vecSize.y += 1f;

			_pCollider_In.size = vecSize;

			yield return new WaitForSeconds( 0.5f );

			vecSize.y -= 1f;
			_pCollider_In.size = vecSize;

			yield return new WaitForSeconds( 0.5f );
		}
	}
}
