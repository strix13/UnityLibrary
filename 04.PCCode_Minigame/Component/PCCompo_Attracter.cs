using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-08-17 오후 5:24:00
   Description : 
   Edit Log    : 
   ============================================ */

public class PCCompo_Attracter : CObjectBase
{
	/* const & readonly declaration             */

	private const float const_fCenterUp = 100f;

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	[Header("달라붙는 속도")]				public float p_fAttracterSpeed = 10f;
	[Header("달라붙는 속도 증가량")]		public float p_fAttracterFactor = 5f;
	[Header("아이템 비 활성화되는 거리")]	public float p_fDisableDistance = 10f;
	[Header("아이템 놓치는 거리")]			public float p_fMissObjectDistance = 1000f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Transform _pTrans_Target;
	private Coroutine _pCoProcUpdatePosition;

	private System.Action _EVENT_OnFinishAttract;
	private System.Action _EVENT_OnMissObject;

	private float _fAttracterSpeed;
	private float _fGravityScaleOrigin;
	private Rigidbody2D _pRigidbody;
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInit(Transform pTarget, System.Action OnFinishAttract, System.Action OnMissObject = null)
	{
		if (gameObject.activeInHierarchy == false) return;

		_pTrans_Target = pTarget;
		_fAttracterSpeed = 0f;

		_EVENT_OnFinishAttract = OnFinishAttract;
		_EVENT_OnMissObject = OnMissObject;

		if (_pCoProcUpdatePosition != null)
		{
			StopCoroutine(_pCoProcUpdatePosition);
			_pCoProcUpdatePosition = null;
		}

		_pCoProcUpdatePosition = StartCoroutine(ProcUpdatePosition());

		_pRigidbody.gravityScale = 0f;
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

		GetComponent( out _pRigidbody );
		_fGravityScaleOrigin = _pRigidbody.gravityScale;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private IEnumerator ProcUpdatePosition()
	{
		while (true)
		{
			float fSpeed = (_fAttracterSpeed * _fAttracterSpeed) * p_fAttracterFactor;

			Vector2 v2OwnerPos = p_pTransCached.localPosition;
			Vector2 v2TargetPos = _pTrans_Target.localPosition + Vector3.up * const_fCenterUp;
			Vector2 v2ClosePos = Vector2.MoveTowards(v2OwnerPos, v2TargetPos, fSpeed);

			_fAttracterSpeed += p_fAttracterSpeed * Time.deltaTime;

			float fDistance = Vector2.Distance(v2OwnerPos, v2TargetPos);
			if (fDistance < p_fDisableDistance)
			{
				p_pTransCached.localPosition = v2TargetPos;

				if (_EVENT_OnFinishAttract != null)
				{
					_EVENT_OnFinishAttract();
					_EVENT_OnFinishAttract = null;
				}

				yield break;
			}
			else if (fDistance > p_fMissObjectDistance)
			{
				if (_EVENT_OnMissObject != null)
				{
					_EVENT_OnMissObject();
					_EVENT_OnMissObject = null;
				}

				yield break;
			}
			_pRigidbody.gravityScale = _fGravityScaleOrigin;
			p_pTransCached.localPosition = v2ClosePos;

			yield return null;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
