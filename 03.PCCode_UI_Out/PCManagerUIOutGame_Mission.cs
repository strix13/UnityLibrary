using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerUIOutGame_Mission : CManagerUIBase<PCManagerUIOutGame_Mission, PCManagerUIOutGame_Mission.EUIFrame, PCManagerUIOutGame_Mission.EUIPopup>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIFrame
	{
		PCUIOutFrame_MissionFuel,
		PCUIOutFrame_MissionClear,
		PCUIOutFrame_MissionOver,
		PCUIOutFrame_MissionSelect
	}

	public enum EUIPopup
	{
		PCUIOutPopup_MissionPause
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

	protected override void OnDefaultFrameShow()
	{
		DoShowHide_Frame(EUIFrame.PCUIOutFrame_MissionSelect, true);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
