using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-07-20 오후 3:02:30
   Description : 
   Edit Log    : 
   ============================================ */

public class CNGUITweenExtendListener : CUIObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public event System.Action p_EVENT_OnEnabled = null;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (p_EVENT_OnEnabled != null)
			p_EVENT_OnEnabled();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
