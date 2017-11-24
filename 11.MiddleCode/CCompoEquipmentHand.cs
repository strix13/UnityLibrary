using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-21 오후 4:03:47
   Description : 
   Edit Log    : 
   ============================================ */

public class CCompoEquipmentHand : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private HashSet<Collider> _pSetDamageTarget = new HashSet<Collider>();
	private Collider _pColliderWeapon;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoSetColliderOn(Collider pOwner)
	{
		_pColliderWeapon.enabled = true;

		_pSetDamageTarget.Clear();
		_pSetDamageTarget.Add(pOwner);
	}

	public HashSet<Collider> DoSetColliderOff()
	{
		if(_pColliderWeapon != null)
			_pColliderWeapon.enabled = false;

		return _pSetDamageTarget;
	}

	public void DoSetEquipment(Transform pTransEquipmentModel)
	{
		pTransEquipmentModel.parent = transform;
		pTransEquipmentModel.DoResetTransform();

		CCompoEventTrigger pTrigger = pTransEquipmentModel.GetComponent<CCompoEventTrigger>();
		if (pTrigger == null) return;

		//pTrigger.p_OnTriggerEnter += EventOnAttackTarget;
		_pColliderWeapon = pTrigger.GetComponent<Collider>();
		_pColliderWeapon.enabled = false;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void EventOnAttackTarget(Collider pCollider)
	{
		_pSetDamageTarget.Add(pCollider);
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
