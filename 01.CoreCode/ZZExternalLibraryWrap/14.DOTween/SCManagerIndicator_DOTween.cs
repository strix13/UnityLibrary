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

#if DOTween
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

static public class SCIndicatorHelper_DOTween
{
	static public void DoTween_Indicator_Color( this CIndicator pIndicator, Color pColorStart, Color pColorDest, float fDuration)
	{
		pIndicator.DoSetColor( pColorStart );
		switch (pIndicator.p_eType)
		{
			case CIndicator.EIndicatorType.UGUI:
				pIndicator.p_pUIText.DOBlendableColor( pColorDest, fDuration );
				break;
#if TMPro
			case CIndicator.EIndicatorType.TextMeshPro:
				pIndicator.p_pUIText_TMPro.DOBlendableColor( pColorDest, fDuration );
				break;
#endif
			default:
				Debug.Log( "Error" );
				break;
		}
	}

	static public void DoTween_Indicator_Pos( this CIndicator pIndicator, Vector3 vecPosStart, Vector3 vecOffsetDest, float fDuration )
	{
		pIndicator.transform.position = vecPosStart;
		pIndicator.transform.DOMove( vecPosStart + vecOffsetDest, fDuration ).SetEase( Ease.Linear );
	}
}

#endif