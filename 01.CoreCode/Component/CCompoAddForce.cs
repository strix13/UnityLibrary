using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoAddForce : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public Vector3 _vecRandomForce_Min = new Vector3(-10f, -10f, 0f);
	public Vector3 _vecRandomForce_Max = new Vector3( 10f, 10f, 0f );

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Rigidbody _pRigidbody;
	private Rigidbody2D _pRigidbody2D;

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

		_pRigidbody = GetComponentInChildren<Rigidbody>( true );
		_pRigidbody2D = GetComponentInChildren<Rigidbody2D>( true );
	}

	protected override void OnPlayEvent()
	{
		base.OnPlayEvent();

		Vector3 vecRandomForce = PrimitiveHelper.RandomRange( _vecRandomForce_Min, _vecRandomForce_Max );
		if (_pRigidbody != null)
			_pRigidbody.AddForce( vecRandomForce );
		else if(_pRigidbody2D != null)
			_pRigidbody2D.AddForce( vecRandomForce );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
