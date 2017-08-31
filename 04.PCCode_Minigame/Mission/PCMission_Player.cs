using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PCMission_Player : CObjectBase
{
	/* const & readonly declaration             */

	const float const_fTimeScaleMin = 0.001f;

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	const int const_iPlayerPosMinY = -1000;
	const int const_iPlayerPosMinX = -520;
	const int const_iPlayerPosMaxY = 840;
	const int const_iPlayerPosMaxX = 520;

	[SerializeField]
	private float _fBulletDelay_Main;
	[SerializeField]
	private float _fBulletSpeed_Main;
	[SerializeField]
	private int _iBulletDamage_Main;

	[Space(10f)]
	public ECharacterName eCharacterName = ECharacterName.None;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<PCMission_PatternBullet> _listBulletMuzzle = new List<PCMission_PatternBullet>();

	private Vector3 _vecTouchStartPos;
	private Vector3 _vecPlayerPos;
	private GameObject _pObjectShield;
	private CircleCollider2D _pCollider;

	private Camera _pCameraInGame;

	[SerializeField]
	private bool _bIsOnHit = false; public bool p_bIsOnHit { get { return _bIsOnHit; } }
	private bool _bMoveLock = false;
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSet_ShotingBullet(bool bShot)
	{
		for (int i = 0; i < _listBulletMuzzle.Count; i++)
			_listBulletMuzzle[i].DoSetGenrating( bShot );
	}

	public void DoSetDockingMode(bool bMoveLock)
	{
		_bMoveLock = bMoveLock;
		_bIsOnHit = _bMoveLock;
		DoSet_ShotingBullet( !_bMoveLock );

		PCManagerInMission.instance.DoPlayOrStop_DecreaseFuel( !_bMoveLock );
	}

	public void DoInitPlayer(Camera pCameraInGame, bool bTestMode)
	{
		_pCameraInGame = pCameraInGame;
		if (bTestMode == false)
		{
			EMissionCategory eMissionCategory = eCharacterName.ConvertCharacterName();
			SDataMission_Character pDataCharacter = PCManagerFramework.g_mapMissionCharacterInfo[eCharacterName];
			_fBulletDelay_Main = pDataCharacter.fBulletDelay;
			_iBulletDamage_Main = pDataCharacter.iBulletDamage;
			_fBulletSpeed_Main = pDataCharacter.fBulletSpeed;
		}

		gameObject.SetActive(true);
		TweenPosition pTweenPos = GetComponent<TweenPosition>();

		if (bTestMode)
		{
			pTweenPos.duration = 0.1f;
			pTweenPos.AddOnFinished(OnFinishTweenPos_Test);
		}
		else
			pTweenPos.AddOnFinished(OnFinishTweenPos);
		pTweenPos.PlayForward();

		enabled = false;

		for (int i = 0; i < _listBulletMuzzle.Count; i++)
			_listBulletMuzzle[i].DoSetGenrating( false );
	}

	public void DoDamagePlayer()
	{
		StartCoroutine(CoOnHitPlayer(3f));
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void OnTriggerEnter2D(Collider2D pCollider)
	{
		PCMission_Item pItem = pCollider.GetComponent<PCMission_Item>();
		if (pItem != null)
		{
			pItem.DoDisableCollider();

			PCCompo_Attracter pCompoAttracter = pCollider.GetComponent<PCCompo_Attracter>();
			if (pCompoAttracter != null)
				pCompoAttracter.DoInit(p_pTransCached, pItem.DoUseItem, pItem.DoEnableCollider);
			else
				pItem.DoUseItem();
		}
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pCollider = GetComponent<CircleCollider2D>();
		_pObjectShield = GetGameObject("Shield");

		GetComponentsInChildren(_listBulletMuzzle);

		for(int i = 0; i < _listBulletMuzzle.Count; i++)
			_listBulletMuzzle[i].p_EVENT_OnGenerate += PCMission_Player_p_EVENT_OnGenerate;
	}

	private void PCMission_Player_p_EVENT_OnGenerate( EMissionBullet arg1, PCMission_Bullet arg2 )
	{
		PCManagerFramework.p_pManagerSound.DoPlaySoundEffect( ESoundName.CK_Blaster_Shot_226 );
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(_bMoveLock == false)
		{
			//bool bIsMove = true;
			ProcCheckUpdatePlayerMove();
			//for (int i = 0; i < _listBulletMuzzle.Count; i++)
			//	_listBulletMuzzle[i].DoSetGenrating( bIsMove );
		}

		Vector3 vecCurrentPlayerPos = _pTransformCached.localPosition;
		if (vecCurrentPlayerPos.x < const_iPlayerPosMinX)
			vecCurrentPlayerPos = new Vector2( const_iPlayerPosMinX, vecCurrentPlayerPos.y );

		if (vecCurrentPlayerPos.x > const_iPlayerPosMaxX)
			vecCurrentPlayerPos = new Vector2( const_iPlayerPosMaxX, vecCurrentPlayerPos.y );

		if (vecCurrentPlayerPos.y < const_iPlayerPosMinY)
			vecCurrentPlayerPos = new Vector2( vecCurrentPlayerPos.x, const_iPlayerPosMinY );

		if (vecCurrentPlayerPos.y > const_iPlayerPosMaxY)
			vecCurrentPlayerPos = new Vector2( vecCurrentPlayerPos.x, const_iPlayerPosMaxY );

		_pTransformCached.localPosition = vecCurrentPlayerPos;

		//float fTimeScale = Time.timeScale;
		//if (bIsMove == false && fTimeScale != const_fTimeScaleMin)
		//	Time.timeScale = Mathf.Lerp( 1f, const_fTimeScaleMin, fTimeScale );
		//else if(fTimeScale != 1f)
		//	Time.timeScale = Mathf.Lerp( const_fTimeScaleMin, 1f, fTimeScale );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void OnFinishTweenPos()
	{
		enabled = true;

		for (int i = 0; i < _listBulletMuzzle.Count; i++)
		{
			_listBulletMuzzle[i].DoSetBulletInfo( true, _fBulletDelay_Main, _iBulletDamage_Main, _fBulletSpeed_Main );
			_listBulletMuzzle[i].DoSetGenrating( true );
		}
	}

	private void OnFinishTweenPos_Test()
	{
		enabled = true;

		for (int i = 0; i < _listBulletMuzzle.Count; i++)
		{
			_listBulletMuzzle[i].DoSetBulletInfo(true, _fBulletDelay_Main, _iBulletDamage_Main, _fBulletSpeed_Main);
			_listBulletMuzzle[i].DoSetGenrating( true );
		}
	}

	private bool ProcCheckUpdatePlayerMove()
	{
		bool bIsMove = false;

		if (_pCameraInGame == null) return false;

#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 vecCursorPos = Input.mousePosition;
			_vecPlayerPos = _pTransformCached.position;
			_vecTouchStartPos = _pCameraInGame.ScreenToWorldPoint( vecCursorPos);

			bIsMove = true;
		}

		if (Input.GetMouseButton(0))
		{
			Vector3 vecCursorPos = Input.mousePosition;

			float TouchingX = _pCameraInGame.ScreenToWorldPoint(vecCursorPos).x - _vecTouchStartPos.x;
			float TouchingY = _pCameraInGame.ScreenToWorldPoint(vecCursorPos).y - _vecTouchStartPos.y;
			_pTransformCached.position = new Vector2(_vecPlayerPos.x + TouchingX, _vecPlayerPos.y + TouchingY);

			bIsMove = true;
		}
#else
		if (Input.touchCount == 0)
			return false;

		Touch Touch = Input.GetTouch(0);
		if (Touch.phase == TouchPhase.Began)
		{
			_vecPlayerPos = _pTransformCached.position;
			_vecTouchStartPos = _pCameraInGame.ScreenToWorldPoint(Touch.position);
			bIsMove = true;
		}
		else if (Touch.phase == TouchPhase.Moved)
		{
			float TouchingX = _pCameraInGame.ScreenToWorldPoint(Touch.position).x - _vecTouchStartPos.x;
			float TouchingY = _pCameraInGame.ScreenToWorldPoint(Touch.position).y - _vecTouchStartPos.y;
			_pTransformCached.position = new Vector2(_vecPlayerPos.x + TouchingX, _vecPlayerPos.y + TouchingY);
			bIsMove = true;
		}
#endif

		return bIsMove;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private IEnumerator CoOnHitPlayer(float fWaitSec)
	{
		_bIsOnHit = true;
		_pObjectShield.SetActive(true);

		yield return new WaitForSeconds(fWaitSec);

		_pObjectShield.SetActive(false);
		_bIsOnHit = false;
	}

}
