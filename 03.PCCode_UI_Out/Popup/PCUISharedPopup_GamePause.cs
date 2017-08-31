using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUISharedPopup_GamePause : CUIPopupBase, IButton_OnClickListener<PCUISharedPopup_GamePause.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_Continue,
		Button_Exit
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void IOnClick_Buttons(EUIButton eButton)
	{
		switch (eButton)
		{
			case EUIButton.Button_Continue:
				DoStartFadeInOutPanel_Delayed(false, 0, 2f);
				break;

			case EUIButton.Button_Exit:
				DoStartFadeInOutPanel_Delayed(false);
				PCManagerFramework.DoLoadScene_FadeInOut(ESceneName.OutGame, 1f, Color.black);
				break;
		}
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		PCManagerFramework.DoSetTimeScale(0f);
	}

	protected override void OnHide()
	{
		base.OnHide();

		PCManagerFramework.DoSetTimeScale(1f);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
