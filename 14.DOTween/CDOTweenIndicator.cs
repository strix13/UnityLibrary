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
 *	기능 : Required DOTween
   ============================================ */
#endregion Header
#if DOTween

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CDOTweenIndicator : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Text _pUIText;
	private RectTransform _pRectTrans_UIText;

#if TMPro
	private TMPro.TextMeshPro _pUIText_TMPro;
	private RectTransform _pRectTrans_UIText_TMPro;
#endif


	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoStartTween_UGUI(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo, float fDuration, Ease eEaseType = Ease.Linear)
	{
		EventInit_UGUI( strText, v3From, colFrom );
		EventStartTween_UGUI(strText, v3From, v3To, colFrom, colTo, fDuration);
	}

	public void DoStartTween_TMPro(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo, float fDuration, Ease eEaseType = Ease.Linear)
	{
		EventInit_TMPro(strText, v3From, colFrom);
		EventStartTween_TMPro(v3To, colTo, fDuration, eEaseType);
	}

	public void DoStartVelocity_TMPro(string strText, Vector3 v3From, Color colFrom, Color colTo, float fDuration)
	{
		EventInit_TMPro(strText, v3From, colFrom);
		EventStartColor_TMPro(colTo, fDuration);
	}

	private void EventStartTween_UGUI(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo, float fDuration, Ease eEaseType = Ease.Linear)
	{
		_pRectTrans_UIText.DOMove(v3To, fDuration).SetEase(eEaseType);
		_pUIText.DOBlendableColor(colTo, fDuration);
	}

	private void EventStartTween_TMPro(Vector3 v3To, Color colTo, float fDuration, Ease eEaseType)
	{
#if TMPro
		_pRectTrans_UIText_TMPro.DOMove(v3To, fDuration).SetEase(eEaseType);
		_pUIText_TMPro.DOBlendableColor(colTo, fDuration);
#endif
	}

	private void EventStartColor_TMPro(Color colTo, float fDuration)
	{
#if TMPro
		_pUIText_TMPro.DOBlendableColor(colTo, fDuration);
#endif
	}

	private void EventInit_UGUI(string strText, Vector3 v3Pos, Color pColor)
	{
		_pUIText.text = strText;
		_pUIText.color = pColor;

		_pRectTrans_UIText.position = v3Pos;

		p_pTransCached.SetAsLastSibling();
	}

	private void EventInit_TMPro(string strText, Vector3 v3Pos, Color pColor)
	{
#if TMPro
		if (GetComponent(out _pUIText_TMPro))
			_pRectTrans_UIText_TMPro = _pUIText_TMPro.rectTransform;

		_pUIText_TMPro.text = strText;
		_pUIText_TMPro.color = pColor;

		_pRectTrans_UIText_TMPro.position = v3Pos;

		p_pTransCached.SetAsLastSibling();
#endif
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

		if (GetComponent(out _pUIText))
			_pRectTrans_UIText = _pUIText.rectTransform;
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		OnFinishTween();
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
#endif