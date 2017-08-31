using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerUIOutGame_Title : CManagerUIBase<PCManagerUIOutGame_Title, PCManagerUIOutGame_Title.EFrame, PCManagerUIOutGame_Title.EPopup>
{
	/* const & readonly declaration             */

	private const int const_iMaxLoadCount = 2;

	/* enum & struct declaration                */

	public enum EFrame
	{
		PCUIOutFrame_Title,
		PCUIOutFrame_TitleLoading
	}

	public enum EPopup
	{

	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCUIOutFrame_Title _pUIOutFrame_Title;

	private int _iLoadCount = 0;
	private bool _bDataLoaded = false;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void OnFinishLoad_Database()
	{
		Debug.LogWarning("DB 로딩이 끝났습니다.");
		ProcCheckDataLoading();
	}

	private void OnFirstChange_Localize()
	{
		Debug.LogWarning("로컬라이징 로딩이 끝났습니다.");
		DoShowHide_Frame(EFrame.PCUIOutFrame_TitleLoading, true);
		ProcCheckDataLoading();
	}

	private void OnFinish_DataLoading()
	{
		DoShowHideFramePanel_FadeInOut_Delayed(EFrame.PCUIOutFrame_TitleLoading, false, 0.5f);
		PCManagerFramework.p_pManagerSound.DoPlayBGM(ESoundName.BGM_Title_LenskoCetus, null);

		_pUIOutFrame_Title.DoEnableFrameButtons(true);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUIOutFrame_Title = GetUIFrame<PCUIOutFrame_Title>();
		_pUIOutFrame_Title.DoEnableFrameButtons(false);

		PCManagerFramework.p_EVENT_OnFirstChangeLocalize += OnFirstChange_Localize;
		PCManagerFramework.p_EVENT_OnDBLoadFinish += OnFinishLoad_Database;
	}

	protected override void OnDefaultFrameShow()
	{
		DoShowHide_Frame(EFrame.PCUIOutFrame_Title, true);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcCheckDataLoading()
	{
		_iLoadCount++;

		if (_iLoadCount == const_iMaxLoadCount)
			OnFinish_DataLoading();
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
