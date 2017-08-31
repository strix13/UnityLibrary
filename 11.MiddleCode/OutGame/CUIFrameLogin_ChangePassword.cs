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

public class CUIFrameLogin_ChangePassword : CUIFrameBase, IButton_OnClickListener<CUIFrameLogin_ChangePassword.EButton>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EButton
    {
		Button_Back,
        Button_Change
    }

    public enum EInput
    {
		Input_ID,
		Input_PW,
		Input_PW_Change,
		Input_PW_ChangeCheck,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */


	/* private - Variable declaration           */

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
	protected override void OnAwake()
	{
		base.OnAwake();

		EventInitUIButtons<EButton>();
	}
	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void OnFinishChangePassword(bool bResult)
	{
		if (bResult)
			CManagerUILogin.instance.DoShowPopup_Info(SCManagerLogIn.EResult_ChangePassword.ChangePassword_Success, true);
		else
			CManagerUILogin.instance.DoShowPopup_Info(SCManagerLogIn.EResult_ChangePassword.ChangePassword_Fail);
	}

	public void IOnClick_Buttons(EButton eButtonName)
	{
		switch (eButtonName)
		{
			case EButton.Button_Back:
				CManagerUILogin.instance.DoShowHide_Frame(CManagerUILogin.EFrame.CUIFrameLogin_Login, true);
				break;
			case EButton.Button_Change:
				string strPasswordChange = GetUIInput(EInput.Input_PW_Change).value;
				string strPasswordChange_Check = GetUIInput(EInput.Input_PW_ChangeCheck).value;

				if (strPasswordChange.CompareTo(strPasswordChange_Check) != 0)
					CManagerUILogin.instance.DoShowPopup_Info(SCManagerLogIn.EResult_ChangePassword.ChangePassword_Success, true);
				else
					SCManagerLogIn.DoChangePassword(GetUIInput(EInput.Input_ID).value, GetUIInput(EInput.Input_PW).value, strPasswordChange, OnFinishChangePassword);
				break;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
