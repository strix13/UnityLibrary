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

public interface IUIPanel
{
	void IUIPanel_SetOrder( int iSetSortOrder );
	bool p_bIsAlwaysShow { get; }
	bool p_bIsFixedSortOrder { get; }

	IEnumerator IUIPanel_OnShowPanel_PlayingAnimation( int iSortOrder );
	IEnumerator IUIPanel_OnHidePanel_PlayingAnimation();
}

abstract public partial class CManagerUIBase<CLASS_Instance, ENUM_Panel_Name, CLASS_Panel, Class_Button> : CSingletonBase<CLASS_Instance>
	where CLASS_Instance : CManagerUIBase<CLASS_Instance, ENUM_Panel_Name, CLASS_Panel, Class_Button>
	where ENUM_Panel_Name : System.IFormattable, System.IConvertible, System.IComparable
	where CLASS_Panel : CObjectBase, IUIPanel
{
	[System.Serializable]
	public class CUIPanelData
	{
		[SerializeField]
		private ENUM_Panel_Name _ePanelName;	public ENUM_Panel_Name p_ePanelName { get { return _ePanelName; } }
		[SerializeField]
		private CLASS_Panel _pPanel;	public CLASS_Panel p_pPanel {  get { return _pPanel; } }

		[SerializeField]
		private int _iCurrentSortOrder;

		private bool _bIsShowCurrent = false; public bool p_bIsShowCurrent { get { return _bIsShowCurrent; } }
		private bool _bIsPlayUIAnimation = false; public bool p_bIsPlayingUIAnimation { get { return _bIsPlayUIAnimation; } }

		public CUIPanelData( ENUM_Panel_Name ePanelName, CLASS_Panel pPanel )
		{
			_ePanelName = ePanelName;
			_pPanel = pPanel;
		}

		public void DoShow()
		{
			_pPanel.gameObject.SetActive( true );
			_bIsShowCurrent = true;
			
			_pPanel.StartCoroutine( CoProcShowPanel( _iCurrentSortOrder ) );
		}

		public void DoShow( int iSortOrder )
		{
			EventSetOrder( iSortOrder );
			DoShow();
		}

		public void DoHide()
		{
			if (_pPanel.gameObject.activeSelf == false) return;
			_bIsShowCurrent = false;
			
			_pPanel.StartCoroutine( CoProcHidePanel() );
		}

		public void DoHide( int iSortOrder )
		{
			EventSetOrder( iSortOrder );
			DoHide();
		}

		public void EventSetOrder( int iSortOrder )
		{
			if (_pPanel.p_bIsFixedSortOrder) return;

			_iCurrentSortOrder = iSortOrder;
			_pPanel.IUIPanel_SetOrder( _iCurrentSortOrder );
		}

		public void SetActive(bool bActive)
		{
			_pPanel.gameObject.SetActive( bActive );
		}



		protected IEnumerator CoProcShowPanel( int iSortOrder )
		{
			_bIsPlayUIAnimation = true;
			yield return _pPanel.StartCoroutine( _pPanel.IUIPanel_OnShowPanel_PlayingAnimation( iSortOrder ) );
			_bIsPlayUIAnimation = false;
		}

		protected IEnumerator CoProcHidePanel()
		{
			_bIsPlayUIAnimation = true;
			yield return _pPanel.StartCoroutine( _pPanel.IUIPanel_OnHidePanel_PlayingAnimation() );
			_pPanel.gameObject.SetActive( false );
			_bIsPlayUIAnimation = false;
		}
	}
}