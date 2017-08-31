using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Edit Log    : 
   ============================================ */

public enum EMissionItem
{
	CoinItem,
	FuelItem,
	RouletteItem
}

[RequireComponent( typeof( Rigidbody2D ) )]
public class PCMission_Item : CObjectBase
{
	/* const & readonly declaration             */
	const int const_iRandomForceRange_OnDrop = 20;
	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public EMissionItem p_eMissionItem;
	[SerializeField]
	private float _fAutoDisableSec = 0f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Rigidbody2D _pRigidbody;
	private CircleCollider2D _pCollider;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */
	 
	public void DoUseItem()
	{
		switch(p_eMissionItem)
		{
			case EMissionItem.CoinItem:
				CManagerPooling<EMissionEffect, PCEffect>.instance.DoPop( EMissionEffect.OnEnemyHit ).DoPlayEffect(_pTransformCached.position);
				PCManagerInMission.instance.DoAddGold(1);
				break;

			case EMissionItem.RouletteItem:
				PCManagerInMission.instance.DoAddRouletteTicket();
				break;
		}

		PCManagerFramework.p_pManagerSound.DoPlaySoundEffect( ESoundName.getgold );
		CManagerPooling<EMissionItem, PCMission_Item>.instance.DoPush( this );
	}

	public void DoResetItem(Vector3 vecPos)
	{
		_pTransformCached.position = vecPos;

		float fRandomX = Random.Range( -const_iRandomForceRange_OnDrop, const_iRandomForceRange_OnDrop );
		float fRandomY = Random.Range( -const_iRandomForceRange_OnDrop, const_iRandomForceRange_OnDrop );

		_pRigidbody.AddForceAtPosition( new Vector2( fRandomX, fRandomY), vecPos );
	}

	public void DoEnableCollider()
	{
		if (_pCollider != null)
			_pCollider.enabled = true;
	}

	public void DoDisableCollider()
	{
		if (_pCollider != null)
			_pCollider.enabled = false;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent( out _pRigidbody );

		_pCollider = GetComponent<CircleCollider2D>();
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (_fAutoDisableSec != 0f)
			StartCoroutine( CoDelayDisable( _fAutoDisableSec ) );

		DoEnableCollider();
	}

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	private void OnCollisionEnter2D( Collision2D collision )
	{
		PCGameObstacle pObstacle = collision.gameObject.GetComponent<PCGameObstacle>();
		if (pObstacle != null && pObstacle.p_eObstacleType == EObstacleType.Wall_Bottom)
			CManagerPooling<EMissionItem, PCMission_Item>.instance.DoPush( this );
	}


	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

	private IEnumerator CoDelayDisable(float fDelaySec)
	{
		yield return new WaitForSeconds( fDelaySec );

		CManagerPooling<EMissionItem, PCMission_Item>.instance.DoPush( this );
	}

}
