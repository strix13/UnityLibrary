using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-06-19 오후 1:59:37
   Description : 
   Edit Log    : 
   ============================================ */

public class CUIFrameLogin_Login : CUIFrameBase, IButton_OnClickListener<CUIFrameLogin_Login.EButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EButton
	{
		Button_Login,
		Button_CreateAccount,
		Button_FindPassword,
		Button_ChangePassword
	}

    public enum EInput
    {
        Input_ID,
        Input_PW
    }

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void IOnClick_Buttons(EButton eButtonName)
	{
		switch (eButtonName)
		{
			case EButton.Button_Login:
				CManagerUIShared.instance.DoShowHide_Frame(CManagerUIShared.EFrame.CUIFrameShared_Loading, true);
				SCManagerLogIn.DoLogin(GetUIInput(EInput.Input_ID).value, GetUIInput(EInput.Input_PW).value, OnResultLogin);
				Debug.Log("로그인 요청 합니다.");
				break;

			case EButton.Button_CreateAccount:
				Debug.Log("계정 생성을 합니다.(팝업)");
				CManagerUILogin.instance.DoShowHide_Frame(CManagerUILogin.EFrame.CUIFrameLogin_CreateAccount, true);
				break;

			case EButton.Button_FindPassword:
				Debug.Log("비밀번호를 찾습니다.(팝업)");
				CManagerUILogin.instance.DoShowHide_Frame(CManagerUILogin.EFrame.CUIFrameLogin_FindPassword, true);
				break;

			case EButton.Button_ChangePassword:
				Debug.Log("비밀번호를 변경합니다.(팝업)");
				CManagerUILogin.instance.DoShowHide_Frame(CManagerUILogin.EFrame.CUIFrameLogin_ChangePassword	, true);
				break;
		}
	}

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

	private void OnResultLogin(bool bResult)
	{
		CManagerUIShared.instance.DoShowHide_Frame(CManagerUIShared.EFrame.CUIFrameShared_Loading, false);

		if (bResult == false)
		{
			CManagerUILogin.instance.DoShowPopup_Info(SCManagerLogIn.EResult_Login.Login_Fail_Wrong_Account);
			GetUIInput(EInput.Input_PW).value = "";
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
