using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public enum EMissionEnemy
{
	BlackHole, BossMiddle_Blue, BossMiddle_Brown, BossMiddle_Copper, BossMiddle_Navy, BossMiddle_Purple, Jeff, L1, L2, Meteo, Meteo2, Meteo3, Mine, R1, R2, Rex, S1, S2, S3, Satellite_1, Satellite_2, Satellite_3, Worm_Hole, Boss2
}

public enum EMissionEffect
{
	OnDead_Boomb, OnEnemyHit, Meteo_Effect, boomeffect, BeShot
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PCMission_EnemyMove))]
public class PCMission_Enemy : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EAnimationName
	{
		Bossframe,
		Bossidle,
		Bosstransform,
	}

	/* public - Variable declaration            */

	static public CManagerPooling<EMissionEffect, PCEffect> g_pManagerPool_Effect;
	static public CManagerPooling<EMissionItem, PCMission_Item> g_pManagerPool_Item;


	public EMissionEnemy eMissionEnemy;

	[Header("테스트용")]
	public bool _bIsBoss = false;
	[SerializeField]
	private int _iHPMax;

	/* protected - Variable declaration         */

	protected Dictionary<string, PCCompo_PatternPlay> _mapBulletMuzzle = new Dictionary<string, PCCompo_PatternPlay>();
	protected List<PCCompo_PatternPlay> _listBulletMuzzle = new List<PCCompo_PatternPlay>();

	protected Transform _pTransModel;	public Transform p_pTransModel {  get { return _pTransModel; } }
	protected CSpineWrapper _pAnimator;
	protected PCMission_EnemyMove _pEnemyMove;
	protected SDataMission_Enemy _pDataEnemy;

	/* private - Variable declaration           */

	private UISlider _pHPSlider;
	private Collider2D _pCollider;

	[Header("디버그용 모니터링")]
	[SerializeField] // For Test
	private int _iHP; public int p_iHPCurrent { get { return _iHP; } }
	[SerializeField]
	private float _fSpeed;
	private int _iScore;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInitEnemy(SDataMission_Enemy pDataMissionEnemy)
	{
		_pDataEnemy = pDataMissionEnemy;
		_iHPMax = pDataMissionEnemy.iHP;
		_fSpeed = pDataMissionEnemy.fSpeed;
		_iHP = _iHPMax;
		_iScore = pDataMissionEnemy.iScore;

		if (_pEnemyMove == null)
			_pEnemyMove = GetComponent<PCMission_EnemyMove>();
		_pEnemyMove.DoInitEnemyMove( pDataMissionEnemy.fSpeed);

		DoInitEnemy();
	}

	public void DoInitEnemy()
	{
		transform.localScale = Vector3.one;
	}

	public void DoResetEnemy(Vector3 vecPos)
	{
		_iHP = _iHPMax;
		if (_pHPSlider != null)
			_pHPSlider.value = 1f;

		_pTransformCached.position = vecPos;
		_pEnemyMove.DoMovePlay();
	}

	public void DoDecreaseHP(int iDamage)
	{
		if(_iHP > 0)
		{
			_iHP -= iDamage;
			float fHPPercentage = (float)_iHP / _iHPMax;
			if (_pHPSlider != null)
				_pHPSlider.value = fHPPercentage;

			OnDecreaseHP( fHPPercentage );
		}
	}

	public void DoRotateAngle(Vector3 vecAngle)
	{
		CNGUITweenRotationSpin pTweenRotate = _pTransModel.GetComponent<CNGUITweenRotationSpin>();
		if (pTweenRotate == null)
			pTweenRotate = _pTransModel.gameObject.AddComponent<CNGUITweenRotationSpin>();

		pTweenRotate.enabled = true;
		pTweenRotate.ResetToFactor();
		pTweenRotate.to = vecAngle;
		pTweenRotate.PlayForward();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void EventSetColliderOnOff(bool bOn)
	{
		_pCollider.enabled = bOn;
	}

	public void EventSetMovePlay(bool bMovePlay)
	{
		if (bMovePlay)
			_pEnemyMove.DoMovePlay();
		else
			_pEnemyMove.DoMoveStop();
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	virtual protected void OnDecreaseHP(float fHPPercentage)
	{
		if (_iHP <= 0)
		{
			CManagerPooling<EMissionEnemy, PCMission_Enemy>.instance.DoPush( this );
			PCEffect pEffect = g_pManagerPool_Effect.DoPop( EMissionEffect.boomeffect );
			pEffect.DoPlayEffect( _pTransformCached.position );

			PCManagerInMission.instance.DoAddFeverGuage(_pDataEnemy.fFeverOnDead);
			PCManagerInMission.instance.DoAddScore(_pDataEnemy.iScore);

			ProcDropCoin();
			ProcDropRouletteTicket();

			if (_bIsBoss)
				PCManagerFramework.p_pManagerSound.DoPlaySoundEffect( ESoundName.bossboom );
			else
				PCManagerFramework.p_pManagerSound.DoPlaySoundEffect( ESoundName.smallenemyboom, _pDataEnemy.fSoundVolume );
		}
		else
		{
			PCManagerInMission.instance.DoAddFeverGuage(_pDataEnemy.fFeverOnHit);
			// TEST
			//PCManagerInMission.instance.DoAddScore(10);
		}
	}

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	public PCCompo_PatternPlay EventGetBulletMuzzle<Enum_MuzzleName>(Enum_MuzzleName eMuzzleName)
	{
		PCCompo_PatternPlay pBulletMuzzle = null;
		if (_mapBulletMuzzle.TryGetValue(eMuzzleName.ToString(), out pBulletMuzzle) == false)
			Debug.LogWarning(name + "에는 " + eMuzzleName + " 이 없습니다");

		return pBulletMuzzle;
	}

	public void EventStopAllBulletMuzzle(bool bGameObjectActiveOff = false)
	{
		for (int i = 0; i < _listBulletMuzzle.Count; i++)
			_listBulletMuzzle[i].DoStopPattern(bGameObjectActiveOff);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent<Rigidbody2D>().gravityScale = 0f;
		GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		GetComponent<Collider2D>().isTrigger = true;

		GetComponent(out _pAnimator);
		GetComponent(out _pEnemyMove);
		GetComponent(out _pCollider);
		GetComponentInChildren(out _pHPSlider);

		UIWidget[] arrWidget = GetComponentsInChildren<UIWidget>();
		for(int i = 0; i < arrWidget.Length; i++)
			arrWidget[i].gameObject.layer = LayerMask.NameToLayer( "Enemy" );
		
		_pTransModel = GetComponentInChildren<Spine.Unity.SkeletonAnimation>().transform;

		PCCompo_PatternPlay[] arrBullet = GetComponentsInChildren<PCCompo_PatternPlay>(true);
		_mapBulletMuzzle.DoAddItem(arrBullet);
		_mapBulletMuzzle.Values.ToList(_listBulletMuzzle);
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		_iHP = _iHPMax;
		if(_pHPSlider != null)
			_pHPSlider.value = 1f;

		PCManagerInMission.instance.DoRegistEnemyList( this );
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		PCManagerInMission.instance.DoRemoveEnmeyList( this );
	}

	private void ProcSetAnimationState()
	{
		if (_pAnimator != null)
			_pAnimator.DoPlayAnimation(EAnimationName.Bossidle);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PCMission_Player pPlayer = collision.gameObject.GetComponent<PCMission_Player>();
		if (pPlayer != null && pPlayer.p_bIsOnHit == false)
			PCManagerInMission.instance.DoDecreasePlayerHP(1, true);
		else
		{
			PCMission_Bullet pBullet = collision.gameObject.GetComponent<PCMission_Bullet>();
			if (pBullet != null)
			{
				DoDecreaseHP(pBullet.p_iDamage);
				pBullet.EventOnHitEnemy();
			}
		}

		PCGameObstacle pObstacle = collision.gameObject.GetComponent<PCGameObstacle>();
		if (pObstacle != null && pObstacle.p_eObstacleType == EObstacleType.Wall_Bottom)
			CManagerPooling<EMissionEnemy, PCMission_Enemy>.instance.DoPush( this );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcDropCoin()
	{
		int iRandomCoinCount = Random.Range( _pDataEnemy.iGoldMin, _pDataEnemy .iGoldMax);
		for (int i = 0; i < iRandomCoinCount; i++)
		{
			PCMission_Item pItem = g_pManagerPool_Item.DoPop(EMissionItem.CoinItem);
			pItem.DoResetItem(_pTransformCached.position);
		}
	}

	private void ProcDropRouletteTicket()
	{
		int iTicketPercent = Random.Range( 0, 100 );
		if (iTicketPercent <= _pDataEnemy.fPercent_Roulette)
		{
			PCMission_Item pItem = g_pManagerPool_Item.DoPop( EMissionItem.RouletteItem );
			pItem.DoResetItem( _pTransformCached.position );
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
