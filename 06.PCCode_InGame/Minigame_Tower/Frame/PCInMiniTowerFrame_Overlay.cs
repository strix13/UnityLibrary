using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCInMiniTowerFrame_Overlay : CUIFrameBase, IButton_OnClickListener<PCInMiniTowerFrame_Overlay.EButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EButton
	{
		Button_Pause,
		Button_Touch
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void IOnClick_Buttons(EButton eButton)
	{
		switch (eButton)
		{
			case EButton.Button_Pause:
				PCManagerUIShared.instance.DoShowHide_Popup(PCManagerUIShared.EUIPopup.PCUISharedPopup_GamePause, true);
				break;

			case EButton.Button_Touch:
				PCManagerInMiniTower.instance.EventOnTouchDropTile();
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
