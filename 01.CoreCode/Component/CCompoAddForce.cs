﻿using UnityEngine;
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

	public Vector3 _vecRandomForce_AbsoluteMin = new Vector3( 1f, 1f, 0f );

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

	protected override void OnPlayEventMain()
	{
		base.OnPlayEventMain();

		Vector3 vecRandomForce = PrimitiveHelper.RandomRange( _vecRandomForce_Min, _vecRandomForce_Max );
		if (vecRandomForce.x < 0f && vecRandomForce.x < -_vecRandomForce_AbsoluteMin.x)
			vecRandomForce.x = -_vecRandomForce_AbsoluteMin.x;
		else if(vecRandomForce.x > _vecRandomForce_AbsoluteMin.x)
			vecRandomForce.x = _vecRandomForce_AbsoluteMin.x;

		if (vecRandomForce.y < 0f && vecRandomForce.y < -_vecRandomForce_AbsoluteMin.y)
			vecRandomForce.y = -_vecRandomForce_AbsoluteMin.y;
		else if (vecRandomForce.y > _vecRandomForce_AbsoluteMin.y)
			vecRandomForce.y = _vecRandomForce_AbsoluteMin.y;

		if (vecRandomForce.z < 0f && vecRandomForce.z < -_vecRandomForce_AbsoluteMin.z)
			vecRandomForce.z = -_vecRandomForce_AbsoluteMin.z;
		else if (vecRandomForce.z > _vecRandomForce_AbsoluteMin.z)
			vecRandomForce.z = _vecRandomForce_AbsoluteMin.z;


		if (_pRigidbody != null)
			_pRigidbody.AddForce( vecRandomForce );
		else if(_pRigidbody2D != null)
			_pRigidbody2D.AddForce( vecRandomForce, ForceMode2D.Impulse );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
