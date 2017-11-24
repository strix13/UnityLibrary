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

public class CManagerUILogin : CManagerNGUIBase<CManagerUILogin, CManagerUILogin.EFrame>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EFrame
	{
		CUIFrameLogin_ChangePassword,
		CUIFrameLogin_CreateAccount,
		CUIFrameLogin_FindPassword,
		CUIFrameLogin_Login,
		CUIPopupLogin_Info,
	}

	/* public - Variable declaration            */

	static public bool p_bLockFadeInOut;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private EFrame _eCurrentFrame;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoShowLogin()
	{
		DoShowHide_Panel(EFrame.CUIFrameLogin_Login, true);
	}

	public void DoShowPopup_Info<ENUM_Result>(ENUM_Result eResult, bool bSuccess = false)
	{
		string strValue = CManagerUILocalize.DoGetCurrentLocalizeValue(eResult.ToString());
		//if (bSuccess)
		//	GetUIPopup<CUIPopupLogin_Info>().DoSetEventOnAccept();

		//GetUIPopup<CUIPopupLogin_Info>().DoShowPopup_Info(strValue);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */
	protected override void OnAwake()
    {
        base.OnAwake();
	}

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnDefaultPanelShow()
	{
		DoShowHide_Panel(EFrame.CUIFrameLogin_Login, true);
	}
		// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	//private IEnumerator CoProcFadeInOut(EPopup ePopup)
	//{
	//	TweenAlpha pAlpha = GetUIPopup<CUIPopupBase>(_eLastPopup).GetTweenCurrent<TweenAlpha>();
	//	pAlpha.PlayReverse();

	//	pAlpha = GetUIPopup<CUIPopupBase>(ePopup).GetTweenCurrent<TweenAlpha>();
	//	pAlpha.ResetToBeginning();

	//	yield return SCManagerYield.GetWaitForSecond(pAlpha.duration);

	//	DoShowHide_Popup(_eLastPopup, false);
	//	pAlpha.PlayForward();

	//	print("a");
	//}
	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
