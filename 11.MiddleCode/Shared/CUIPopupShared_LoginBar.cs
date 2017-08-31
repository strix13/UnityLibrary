using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-06-19 오후 10:30:40
   Description : 
   Edit Log    : 
   ============================================ */

public class CUIPopupShared_LoginBar : CUIPopupBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */
	enum ELabel
	{
		Label_Name
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private TweenPosition _pTweenPos = null;

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
	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		GetUILabel(ELabel.Label_Name).text = "더미";

		StartCoroutine(ProcShowLoginBar());
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		_pTweenPos = GetComponentInChildren<TweenPosition>(true);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */
	private IEnumerator ProcShowLoginBar()
	{
		_pTweenPos.PlayForward();
		yield return new WaitForSeconds(_pTweenPos.duration + 1);

		_pTweenPos.PlayReverse();
		yield return new WaitForSeconds(_pTweenPos.duration);

		_pTweenPos.ResetToBeginning();
		p_pGameObjectCached.SetActive(false);
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
