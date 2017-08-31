using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerUIInMission : CManagerUIBase<PCManagerUIInMission, PCManagerUIInMission.EFrame, PCManagerUIInMission.EPopup>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EFrame
	{
		PCUIInFrame_MissionOverlay,
		PCUIInFrame_MissionResult
	}

	public enum EPopup
	{
		PCUIInPopup_MissionRoulette,
		PCUIInPopup_MissionStage
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCUIInFrame_MissionOverlay _pUIFrame_MissionOverlay;
	private PCUIInFrame_MissionResult _pUIFrame_MissionResult;
	private PCUIInPopup_MissionStage _pUIPopup_MissionStage;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoSetTotal_Gold(int iGoldTotal, int iGoldAdd = 0)
	{
		_pUIFrame_MissionOverlay.DoSetTotal_Gold(iGoldTotal, iGoldAdd);
	}

	public void DoSetTotal_Ticket(int iTicketTotal)
	{
		_pUIFrame_MissionOverlay.DoSetTotal_Ticket(iTicketTotal);
	}

	public void DoSetTotal_Score(int iScoreTotal, int iScoreAdd)
	{
		_pUIFrame_MissionOverlay.DoSetTotal_Score(iScoreTotal, iScoreAdd );
	}

	public void DoSetTotal_FeverGuage(float fFeverTotal, float fFeverAdd)
	{
		_pUIFrame_MissionOverlay.DoSetTotal_Fever(fFeverTotal, fFeverAdd );
	}

	public void DoSetTotal_Runa(int iRunaTotal)
	{
		_pUIFrame_MissionOverlay.DoSetTotal_Runa(iRunaTotal);
	}

	public void DoSetTotal_Roulette(int iRouletteTotal, int iAddAmount)
	{
		_pUIFrame_MissionOverlay.DoSetTotal_Roulette(iRouletteTotal, iAddAmount );
	}

	public void DoShowUI_BossSpawn(System.Action OnFinishTween_BossSpawn = null)
	{
		_pUIPopup_MissionStage.DoShowUI_BossSpawn(OnFinishTween_BossSpawn);
	}

	public void DoShowUI_BossClear(System.Action OnFinishTween_BossClear = null)
	{
		_pUIPopup_MissionStage.DoShowUI_BossClear(OnFinishTween_BossClear);
	}

	public void DoShowUI_StageInfo(int iCurStage, int iCurWave, System.Action OnFinishTween_StageInfo = null)
	{
		_pUIPopup_MissionStage.DoShowUI_StageInfo(iCurStage, iCurWave, OnFinishTween_StageInfo);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void EventOnFever()
	{
		_pUIFrame_MissionOverlay.EventOnFever();
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	protected override void OnDefaultFrameShow()
	{
		if (PCManagerFramework.p_pInfoUser == null) return;

		DoShowHide_Frame(EFrame.PCUIInFrame_MissionOverlay, true);
		DoShowHide_Popup(EPopup.PCUIInPopup_MissionStage, true);
		DoShowHide_Popup(EPopup.PCUIInPopup_MissionStage, true);

		PCManagerInMission.instance.p_EVENT_OnGameFinish += _pUIFrame_MissionResult.DoInitShowUI;
	}

	/* protected - [Event] Function           
       자식 객체가 호출                         */
	
	private void OnFinishLoad_Database() // InfoUser 가 없을때만 실행
	{
		OnDefaultFrameShow();
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUIFrame_MissionOverlay = GetUIFrame<PCUIInFrame_MissionOverlay>();
		_pUIFrame_MissionResult = GetUIFrame<PCUIInFrame_MissionResult>();
		_pUIPopup_MissionStage = GetUIPopup<PCUIInPopup_MissionStage>();

		PCManagerFramework.p_EVENT_OnDBLoadFinish += OnFinishLoad_Database;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
