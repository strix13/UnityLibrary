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

public class CUIFrameLogin_FindPassword : CUIFrameBase, IButton_OnClickListener<CUIFrameLogin_FindPassword.EButton>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */
    public enum EButton
    {
		Button_Back,
		Button_Find
    }

    public enum EInput
    {
        Input_ID
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
			case EButton.Button_Back:
				CManagerUILogin.instance.DoShowHide_Frame(CManagerUILogin.EFrame.CUIFrameLogin_Login, true);
				break;
			case EButton.Button_Find:
				SCManagerLogIn.DoResetPassword(GetUIInput(EInput.Input_ID).value);
				CManagerUILogin.instance.DoShowPopup_Info(SCManagerLogIn.EResult_FindPassword.FindPassword_Success, true);
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

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
