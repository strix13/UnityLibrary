#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CUGUIIndicator : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Text _pUIText;
	private RectTransform _pRectTrans_UIText;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoStartTween(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo, float fDuration/*, LeanTweenType eEndEaseType*/)
	{
		// UGUI 는 하이어라키 순으로 렌더링 하기때문에 자동으로 끝부분으로 밀어준다.
		p_pTransCached.SetAsLastSibling();

		_pUIText.color = colFrom;
		_pUIText.text = strText;

		_pRectTrans_UIText.localPosition = v3From;

		//LeanTween.move( _pRectTrans_UIText, v3To, fDuration ).setEase( eEndEaseType );
		//LeanTween.colorText( _pRectTrans_UIText, colTo, fDuration )
		//		 .setOnComplete( OnFinishTween );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void OnFinishTween()
	{
		SCManagerUGUIIndicator.instance.DoPush(this);
	}

	/* protected - Override & Unity API         */
	
	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent(out _pUIText);

		_pRectTrans_UIText = _pUIText.rectTransform;
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
