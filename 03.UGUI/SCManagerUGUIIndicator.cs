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
#if DOTween
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SCManagerUGUIIndicator : CManagerPooling<SCManagerUGUIIndicator.EUIObject, CDOTweenIndicator>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIObject
	{
		UGUI_Indicator_Hanna,
		UGUI_Indicator_Nanum,
		UGUI_Indicator_Nanum_TMPro
	}

	public enum EInidicatorType
	{
		World,
		UI, // UI의 경우 빌드 후 CanvasScaler에 의해 모양이 이상해 질수 있다.
	}


	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	static private bool _bIsInit_InGameCamera;
	static private Canvas _pCanvas;
	static private Camera _pCamera_InGame;

	static private RectTransform _pRectTransform_CanvasScaler;
	static private Vector3 _vecUIRootOffset;

	static private EInidicatorType _eType;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	static public void DoInitInidcator_World( Camera pWorldCamera, Canvas pCanvas )
	{
		_eType = EInidicatorType.World;
		_bIsInit_InGameCamera = true;
		_pCamera_InGame = pWorldCamera;

		DoSetParents_ManagerObject( pCanvas.transform );
		
		GameObject pObjectManager = instance.p_pObjectManager;
		_pCanvas = instance.p_pObjectManager.AddComponent<Canvas>();
		// 캔버스를 AddComponent하는 순간 기존의 Transform이 삭제되고 RectTransform이 생기기 때문에 갱신해야 한다..
		EventUpdateTransform();

		pObjectManager.layer = LayerMask.NameToLayer( "UI" );
		pObjectManager.transform.SetAsLastSibling();
	}

	static public void DoInitInidcator_UI(Camera pWorldCamera, CanvasScaler pCanvasScaler/*, int iSiblingIndex*/)
	{
		_eType = EInidicatorType.UI;
		_bIsInit_InGameCamera = true;
		_pCamera_InGame = pWorldCamera;

		DoSetParents_ManagerObject( pCanvasScaler.transform );
		_pRectTransform_CanvasScaler = pCanvasScaler.GetComponent<RectTransform>();

		_vecUIRootOffset = pCanvasScaler.referenceResolution / 2f;
		if (pCanvasScaler.matchWidthOrHeight == 0f)
			_vecUIRootOffset.x = 0f;
		else
			_vecUIRootOffset.y = 0f;

		GameObject pObjectManager = instance.p_pObjectManager;
		_pCanvas = instance.p_pObjectManager.AddComponent<Canvas>();
		// 캔버스를 AddComponent하는 순간 기존의 Transform이 삭제되고 RectTransform이 생기기 때문에 갱신해야 한다..
		EventUpdateTransform();

		pObjectManager.layer = LayerMask.NameToLayer( "UI" );
		//pObjectManager.transform.SetSiblingIndex( iSiblingIndex );
	}

	static public void DoStartTween_UIObject( string strText, Vector3 v3From, Vector3 v3To, Color pColor,
				 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Hanna/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/ )
	{
		CDOTweenIndicator pResource = instance.DoPop( eUIObject );
		pResource.DoStartTween_UGUI( strText, v3From + _vecUIRootOffset, v3To + _vecUIRootOffset, pColor, pColor, fDuration/*, eEndEaseType*/ );
	}

	static public void DoStartTween_UIObject( string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo,
						 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Hanna/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/ )
	{
		CDOTweenIndicator pResource = instance.DoPop( eUIObject );
		pResource.DoStartTween_UGUI( strText, v3From + _vecUIRootOffset, v3To + _vecUIRootOffset, colFrom, colTo, fDuration/*, eEndEaseType*/ );
	}

	static public void DoStartTween_TMPro(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo, float fDuration, EUIObject eUIObject)
	{
		CDOTweenIndicator pResource = instance.DoPop(eUIObject);
		pResource.DoStartTween_TMPro(strText, v3From + _vecUIRootOffset, v3To + _vecUIRootOffset, colFrom, colTo, fDuration);
	}


	static public void DoStartTween_InGameObject( string strText, Vector3 v3From, Vector3 v3To, Color colFrom,
					 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Hanna/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/)
	{
		if(_bIsInit_InGameCamera == false)
		{
			Debug.Log( "UGUI Indicator를 사용하시려면 Init 함수를 호출해야 합니다", null );
			return;
		}

		CDOTweenIndicator pResource = instance.DoPop( eUIObject );
		Vector3 vecUIPos = ProcConvertPosition_World_To_UI( pResource.transform, v3From );
		pResource.DoStartTween_UGUI( strText, vecUIPos, vecUIPos + (v3To - v3From), colFrom, colFrom, fDuration/*, eEndEaseType*/ );
	}

	static public void DoStartTween_InGameObject(string strText, Vector3 v3From, Vector3 v3To, Color colFrom, Color colTo,
							 float fDuration, EUIObject eUIObject = EUIObject.UGUI_Indicator_Hanna/*, LeanTweenType eEndEaseType = LeanTweenType.linear*/ )
	{
		if (_bIsInit_InGameCamera == false)
		{
			Debug.Log( "UGUI Indicator를 사용하시려면 Init 함수를 호출해야 합니다", null );
			return;
		}

		CDOTweenIndicator pResource = instance.DoPop( eUIObject );
		Vector3 vecUIPos = ProcConvertPosition_World_To_UI( pResource.transform, v3From );
		pResource.DoStartTween_UGUI(strText, vecUIPos, vecUIPos + (v3To - v3From), colFrom, colTo, fDuration/*, eEndEaseType*/);
	}

	static public CDOTweenIndicator DoPop(string strText, Vector3 vecFrom, Vector3 vecTo, Color colFrom, Color colTo, float fDuration, EUIObject eUIObject)
	{
		if (_bIsInit_InGameCamera == false)
		{
			Debug.Log("UGUI Indicator를 사용하시려면 Init 함수를 호출해야 합니다", null);
			return null;
		}

		CDOTweenIndicator pResource = instance.DoPop(eUIObject);
		pResource.DoStartTween_UGUI(strText, vecFrom, vecTo, colFrom, colTo, fDuration);

		return pResource;
	}

	static public CDOTweenIndicator DoPop_TMPro(string strText, Vector3 vecFrom, Vector3 vecTo, Color colFrom, Color colTo, float fDuration, EUIObject eUIObject)
	{
		CDOTweenIndicator pResource = instance.DoPop(eUIObject);
		pResource.DoStartTween_TMPro(strText, vecFrom, vecTo, colFrom, colTo, fDuration);

		return pResource;
	}

	static public CDOTweenIndicator DoPop_Velocity_TMPro(string strText, Vector3 vecFrom, Color colFrom, Color colTo, float fDuration, EUIObject eUIObject)
	{
		CDOTweenIndicator pResource = instance.DoPop(eUIObject);
		pResource.DoStartVelocity_TMPro(strText, vecFrom, colFrom, colTo, fDuration);

		return pResource;
	}

	new static public void DoStartPooling(int iPoolingCount)
	{
		instance.DoStartPooling( iPoolingCount );
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

		if (_eType == EInidicatorType.UI)
		{
			vecWorldCamPos.x *= _pRectTransform_CanvasScaler.sizeDelta.x;
			vecWorldCamPos.y *= _pRectTransform_CanvasScaler.sizeDelta.y;

			vecWorldCamPos.x -= _pRectTransform_CanvasScaler.sizeDelta.x * _pRectTransform_CanvasScaler.pivot.x;
			vecWorldCamPos.y -= _pRectTransform_CanvasScaler.sizeDelta.y * _pRectTransform_CanvasScaler.pivot.y;
		}
		pTransform.position = vecWorldCamPos;

		return pTransform.position;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
   #endif