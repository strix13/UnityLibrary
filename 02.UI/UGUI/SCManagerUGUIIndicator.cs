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

public class SCManagerUGUIIndicator : CManagerPooling<SCManagerUGUIIndicator.EUIObject, CUGUIIndicator>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIObject
	{
		UGUI_Indicator_Default,
		UGUI_Indicator_Left
	}

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	static private bool _bIsInit_InGameCamera;
	static private Canvas _pCanvas;
	static private Camera _pCamera_InGame;

	static private RectTransform _pRectTransformRoot;
	static private Vector3 _vecUIRootOffset;

	#endregion Field
	
	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	static public void DoInitInidcator(Camera pWorldCamera, int iSiblingIndex)
	{
		_bIsInit_InGameCamera = true;
		_pCamera_InGame = pWorldCamera;

		// CCompoEventSystemChecker는 웬만하면 ManagerUI에 같이 붙어있기 때문에 UIRoot 찾는 역할
		CCompoEventSystemChecker pObjectUIRoot = GameObject.FindObjectOfType<CCompoEventSystemChecker>();
		if (pObjectUIRoot != null)
		{
			DoSetParents_ManagerObject( pObjectUIRoot.transform );
			_pRectTransformRoot = pObjectUIRoot.GetComponent<RectTransform>();
			CanvasScaler pCanvasScaler = pObjectUIRoot.GetComponent<CanvasScaler>();

			_vecUIRootOffset = pCanvasScaler.referenceResolution / 2f;
			if (pCanvasScaler.matchWidthOrHeight == 0f)
				_vecUIRootOffset.x = 0f;
			else
				_vecUIRootOffset.y = 0f;
		}
		else
			Debug.LogError( "아직 처리안함" );

		GameObject pObjectManager = instance.p_pObjectManager;
		_pCanvas = instance.p_pObjectManager.AddComponent<Canvas>();
		// 캔버스를 AddComponent하는 순간 기존의 Transform이 삭제되고 RectTransform이 생기기 때문에 갱신해야 한다..
		EventUpdateTransform();

		pObjectManager.layer = LayerMask.NameToLayer( "UI" );
		pObjectManager.transform.SetSiblingIndex( iSiblingIndex );
	}

	static public void DoStartTween_UIObject( string strText, Vector3 v3From, Vector3 v3To, Color pColor,
				 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Default/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/ )
	{
		CUGUIIndicator pResource = instance.DoPop( eUIObject );
		pResource.DoStartTween( strText, v3From + _vecUIRootOffset, v3To + _vecUIRootOffset, pColor, pColor, fDuration/*, eEndEaseType*/ );
	}

	static public void DoStartTween_UIObject( string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo,
						 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Default/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/ )
	{
		CUGUIIndicator pResource = instance.DoPop( eUIObject );
		pResource.DoStartTween( strText, v3From + _vecUIRootOffset, v3To + _vecUIRootOffset, colFrom, colTo, fDuration/*, eEndEaseType*/ );
	}


	static public void DoStartTween_InGameObject( string strText, Vector3 v3From, Vector3 v3To, Color colFrom,
					 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Default/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/)
	{
		if(_bIsInit_InGameCamera == false)
		{
			Debug.Log( "UGUI Indicator를 사용하시려면 Init 함수를 호출해야 합니다", null );
			return;
		}

		CUGUIIndicator pResource = instance.DoPop( eUIObject );
		Vector3 vecUIPos = ProcConvertPosition_World_To_UI( pResource.transform, v3From );
		pResource.DoStartTween( strText, vecUIPos, vecUIPos + (v3To - v3From), colFrom, colFrom, fDuration/*, eEndEaseType*/ );
	}

	static public void DoStartTween_InGameObject(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo,
							 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Default/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/ )
	{
		if (_bIsInit_InGameCamera == false)
		{
			Debug.Log( "UGUI Indicator를 사용하시려면 Init 함수를 호출해야 합니다", null );
			return;
		}

		CUGUIIndicator pResource = instance.DoPop( eUIObject );
		Vector3 vecUIPos = ProcConvertPosition_World_To_UI( pResource.transform, v3From );
		pResource.DoStartTween(strText, vecUIPos, vecUIPos + (v3To - v3From), colFrom, colTo, fDuration/*, eEndEaseType*/);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	static private Vector3 ProcConvertPosition_World_To_UI( Transform pTransform, Vector3 vecPos )
	{
		Vector2 vecWorldCamPos = _pCamera_InGame.WorldToViewportPoint( vecPos );
		vecWorldCamPos.x *= _pRectTransformRoot.sizeDelta.x;
		vecWorldCamPos.y *= _pRectTransformRoot.sizeDelta.y;

		vecWorldCamPos.x -= _pRectTransformRoot.sizeDelta.x * _pRectTransformRoot.pivot.x;
		vecWorldCamPos.y -= _pRectTransformRoot.sizeDelta.y * _pRectTransformRoot.pivot.y;

		pTransform.position = vecWorldCamPos;
		return pTransform.position;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
