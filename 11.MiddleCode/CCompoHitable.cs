using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-22 오후 4:46:48
   Description : 
   Edit Log    : 
   ============================================ */

public class CCompoHitable : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	public event System.Action<int> p_EVENT_OnDamage;
	public event System.Action p_EVENT_OnDead;

	public float p_fModelOffsetY {  get { return _fModelOffsetY; } }
	public bool p_bIsAlive { get { return _iHP > 0; } }

	[SerializeField]
	private int _iHP; public int p_iHP { get { return _iHP; } }
	[SerializeField]
	private int _iHPMAX = 100; public int p_iHPMAX { get { return _iHPMAX; } }
	[SerializeField]
	private int _iArmor = 0;

	/* protected - Field declaration         */

	/* private - Field declaration           */
	
	private float _fModelOffsetY;
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoResetHitable()
	{
		_iHP = _iHPMAX;
	}

	public void DoInitHitAble(int iHP, int iArmor)
	{
		_iHPMAX = iHP;
		_iHP = _iHPMAX;
		_iArmor = iArmor;
	}

	public bool DoDamage(int iDamage)
	{
		iDamage -= _iArmor;
		_iHP -= iDamage;
		bool bIsDead = _iHP <= 0;

		if (p_EVENT_OnDamage != null)
			p_EVENT_OnDamage(iDamage);

		if (bIsDead && p_EVENT_OnDead != null)
			p_EVENT_OnDead();

		return bIsDead;
	}

	public void DoPushObject(Vector3 vecPushPos, float fPower, float fDuration)
	{
		Vector3 vecPushDirection = _pTransformCached.position - vecPushPos;
		StopAllCoroutines();
		StartCoroutine(CoPush(vecPushDirection, fPower, fDuration));
	}

	/* public - [Event] Function             
	   프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
	   자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		Renderer pRenderer = GetComponentInChildren<Renderer>();
		if(pRenderer != null)
			_fModelOffsetY = pRenderer.bounds.max.y;

		DoResetHitable();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private IEnumerator CoPush(Vector3 vecDirection, float fPower, float fDuration)
	{
		float fTimeFinish = Time.time + fDuration;
		while (Time.time <= fTimeFinish)
		{
			_pTransformCached.Translate(vecDirection * fPower * Time.deltaTime, Space.World);
			yield return null;
		}

		yield break;
	}

	/* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
