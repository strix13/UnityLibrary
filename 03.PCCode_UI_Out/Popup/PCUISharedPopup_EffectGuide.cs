using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUISharedPopup_EffectGuide : CUIPopupBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Transform _pTrans_EffectGuide;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInitUI(Transform pParent, float fScale = 100f)
	{
		if (_pTrans_EffectGuide == null) return;

		_pTrans_EffectGuide.parent = pParent;
		_pTrans_EffectGuide.localScale = Vector3.one * fScale;
		_pTrans_EffectGuide.localPosition = Vector3.zero;
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

		_pTrans_EffectGuide = GetGameObject("Spine_Effect_Guide").transform;
	}

	protected override void OnHide()
	{
		base.OnHide();

		_pTrans_EffectGuide.parent = _pTransformCached;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
