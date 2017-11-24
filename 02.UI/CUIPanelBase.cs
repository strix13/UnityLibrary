#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class CUIPanelBase : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	[SerializeField]
	protected bool _bAlwaysShow = false; public bool p_bAlwaysShow { get { return _bAlwaysShow; } }
	[SerializeField]
	protected bool _bFixedSortOrder = false; public bool p_bFixedSortOrder { get { return _bFixedSortOrder; } }

	/* protected - Field declaration         */

	protected int _iCurrentSortOrder;

	/* private - Field declaration           */

	private List<CCompoEventTrigger> _listEventTrigger = new List<CCompoEventTrigger>();
	private System.Func<int, IEnumerator> _CoOnShow;
	private System.Func<IEnumerator> _CoOnHide;

	private bool _bIsShowCurrent = false; public bool p_bIsShowCurrent { get { return _bIsShowCurrent; } }
	private bool _bIsPlayUIAnimation = false; public bool p_bIsPlayingUIAnimation { get { return _bIsPlayUIAnimation; } }

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoShow()
	{
		gameObject.SetActive( true );
		_bIsShowCurrent = true;
		for (int i = 0; i < _listEventTrigger.Count; i++)
			_listEventTrigger[i].DoRecieveMessage_OnShow();
		
		StartCoroutine( _CoOnShow( _iCurrentSortOrder) );
	}

	public void DoShow( int iSortOrder )
	{
		EventUIPanel_SetOrder( iSortOrder );
		DoShow();
	}

	public void DoHide()
	{
		if (gameObject.activeSelf == false) return;
		_bIsShowCurrent = false;

		for (int i = 0; i < _listEventTrigger.Count; i++)
			_listEventTrigger[i].DoRecieveMessage_OnHide();

		StartCoroutine( _CoOnHide() );
	}

	public void DoHide( int iSortOrder )
	{
		EventUIPanel_SetOrder( iSortOrder );
		DoHide();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void EventUIPanel_SetOrder( int iSortOrder )
	{
		_iCurrentSortOrder = iSortOrder;
		OnSetSortOrder( _iCurrentSortOrder );
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	abstract protected void OnSetSortOrder( int iSortOrder );

	virtual protected IEnumerator OnShowPanel_PlayingAnimation( int iSortOrder ) { yield return null; }
	virtual protected IEnumerator OnHidePanel_PlayingAnimation() { yield return null; }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponentsInChildren( true, _listEventTrigger );

		_CoOnShow = CoProcShowPanel;
		_CoOnHide = CoProcHidePanel;

		if (_bAlwaysShow)
			DoShow();
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		_bIsShowCurrent = false;
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
	   로직을 처리(Process Local logic)           */

	protected IEnumerator CoProcShowPanel( int iSortOrder )
	{
		_bIsPlayUIAnimation = true;
		yield return StartCoroutine( OnShowPanel_PlayingAnimation( iSortOrder ) );
		_bIsPlayUIAnimation = false;
	}

	protected IEnumerator CoProcHidePanel()
	{
		_bIsPlayUIAnimation = true;
		yield return StartCoroutine( OnHidePanel_PlayingAnimation() );
		gameObject.SetActive( false );
		_bIsPlayUIAnimation = false;
	}


	/* private - Other[Find, Calculate] Func 
	   찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
