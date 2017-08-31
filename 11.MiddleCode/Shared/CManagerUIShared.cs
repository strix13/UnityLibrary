using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-03-03 오후 11:00:06
   Description : 
   Edit Log    : 
   ============================================ */
   
public class CManagerUIShared : CManagerUIBase<CManagerUIShared, CManagerUIShared.EFrame, CManagerUIShared.EPopup>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EFrame
	{
		CUIFrameShared_Loading
	}

	public enum EPopup
	{
		CUIPopupShared_DebugConsole,
		CUIPopupShared_LoginBar
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CUIPopupShared_DebugConsole _pUIPopup_DebugConsole = null;
	private bool bTouched = false;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

    public bool CheckIsAlreadyShowConsole()
    {
        return _pUIPopup_DebugConsole.p_bShowCurrent;
    }

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnDefaultFrameShow()
	{

	}

	protected override void OnAwake()
	{
        base.OnAwake();

        _pUIPopup_DebugConsole = GetUIPopup<CUIPopupShared_DebugConsole>();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (Input.GetMouseButtonUp(1) || Input.GetButtonUp("Jump"))
			DoShowHide_Popup(EPopup.CUIPopupShared_DebugConsole, !CheckIsAlreadyShowConsole());
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
