using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public enum EMissionBullet
{
	Awl_3_Violet, Awl_Blue, Awl_Green, Awl_Red, Circle_Orange, Circle_Purple, Circle_Red, Complex_Blue, Energy_Green, Energy_Pink, Energy_Purple, Energy_Red, Energy_Violet, Energy_Yellow, Mushroom_Blue, Stick_Red, Wing_Green, Circle_Orange_Small,
}

[RequireComponent(typeof(Rigidbody2D))]
public class PCMission_Bullet : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	static public CManagerPooling<EMissionEffect, PCEffect> g_pManagerPool_Effect;

	[Header( "Test용 노출" )]
	[SerializeField]
	private int _iDamage = 1; public int p_iDamage { get { return _iDamage; } }
	[SerializeField]
	private float _fSpeed = 1f;
	[SerializeField]
	private bool _bOwnerIsPlayer = false;

	[Space( 10f )]
	[SerializeField]
	private bool _bIsTrough = false;
	[SerializeField]
	private bool _bIsGuiding = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Collider2D _pCollider;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitBullet( bool bOwnerIsPlayer, int iDamage, float fSpeed, Transform pTransMuzzle )
	{
		_bOwnerIsPlayer = bOwnerIsPlayer;
		if (bOwnerIsPlayer)
			gameObject.layer = LayerMask.NameToLayer( "Bullet_Player" );
		else
			gameObject.layer = LayerMask.NameToLayer( "Bullet_Enemy" );

		_iDamage = iDamage;
		_fSpeed = fSpeed;

		gameObject.SetActive( true );
	}

	public void EventOnHitEnemy()
	{
		if (_bIsTrough == false)
			CManagerPooling<EMissionBullet, PCMission_Bullet>.instance.DoPush( this );

		PCEffect pEffect = g_pManagerPool_Effect.DoPop( EMissionEffect.BeShot );
		pEffect.DoPlayEffect( _pTransformCached.position );
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

		Rigidbody2D pRigidbody = GetComponent<Rigidbody2D>();
		pRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
		pRigidbody.gravityScale = 0f;
		GetComponent( out _pCollider );
		_pCollider.isTrigger = false;
	}

	protected override void OnStart()
	{
		base.OnStart();

		g_pManagerPool_Effect = CManagerPooling<EMissionEffect, PCEffect>.instance;
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		_pCollider.enabled = true;

		PCMission_BulletGuide pBulletGuide = GetComponent<PCMission_BulletGuide>();
		if (_bIsGuiding && pBulletGuide == null)
			p_pGameObjectCached.AddComponent<PCMission_BulletGuide>();

		if (pBulletGuide != null)
		{
			pBulletGuide.enabled = _bIsGuiding;

			if (_bIsGuiding)
				pBulletGuide.DoInitBulletGuide( _bOwnerIsPlayer );
		}

		StartCoroutine( CoDelayAutoDisable( 10f ) );
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		_pTransformCached.Translate( Vector3.up * _fSpeed * Time.deltaTime, Space.Self );
	}

	private void OnCollisionEnter2D( Collision2D collision )
	{
		PCGameObstacle pObstacle = collision.gameObject.GetComponent<PCGameObstacle>();
		if (pObstacle != null)
		{
			_pCollider.enabled = false;
			StartCoroutine( CoDelayAutoDisable( 1f ) );
			return;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PCMission_Player pPlayer = collision.GetComponent<PCMission_Player>();
		if (pPlayer != null && pPlayer.p_bIsOnHit == false)
			PCManagerInMission.instance.DoDecreasePlayerHP(1, true);
	}


	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoDelayAutoDisable(float fDelaySec)
	{
		yield return new WaitForSeconds( fDelaySec );

		CManagerPooling<EMissionBullet, PCMission_Bullet>.instance.DoPush( this );
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
