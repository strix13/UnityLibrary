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

public class CUIPopupLogin_Info : CNGUIPanelHasButtonBase<CUIPopupLogin_Info.EButton>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */
    public enum EButton
    {
		Button_Accept
	}

    public enum ELabel
    {
        Label_Result
    }
    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoSetEventOnAccept()
	{
		EventDelegate.Add(GetUIButton(EButton.Button_Accept).onClick, CManagerUILogin.instance.DoShowLogin, true);
	}

	public void DoShowPopup_Info(string strResultKey)
	{
		UILabel pUILabel = GetUILabel(ELabel.Label_Result);
		pUILabel.text = strResultKey;

		DoShow();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */
	public override void OnClick_Buttons( EButton eButtonName )
	{
		switch (eButtonName)
		{
			case EButton.Button_Accept:
				DoHide();
				break;
		}
	}
	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
