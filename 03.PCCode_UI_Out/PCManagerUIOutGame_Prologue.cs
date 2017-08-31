using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerUIOutGame_Prologue : CManagerUIBase<PCManagerUIOutGame_Prologue, PCManagerUIOutGame_Prologue.EUIFrame, PCManagerUIOutGame_Prologue.EUIPopup>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIFrame
	{
		PCUIOutFrame_Prologue,
	}

	public enum EUIPopup
	{

	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCUIOutFrame_Prologue _pUIFrame_Prologue;

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

		_pUIFrame_Prologue = GetUIFrame<PCUIOutFrame_Prologue>();
	}

	protected override void OnDefaultFrameShow()
	{
		_pUIFrame_Prologue.DoShow();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
