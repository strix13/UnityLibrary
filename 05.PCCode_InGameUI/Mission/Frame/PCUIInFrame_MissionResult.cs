using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-08-17 오후 12:15:55
   Description : 
   Edit Log    : 
   ============================================ */

public enum EMissionBonus
{
	GrandPrix,
	BonusGame
}

public class PCUIInFrame_MissionResult : CUIFrameBase, IButton_OnClickListener<PCUIInFrame_MissionResult.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_Lobby,
		Button_NextMission,
		Button_RetryMission,
		Button_BonusGame
	}

	public enum EUILabel
	{
		Label_StageNum,
		Label_StageResult,
		Label_HighScoreNum,
		Label_ScoreNum,
		Label_LevelNum,
		Label_GetGoldNum,

		Label_Mission_LevelNextExp
	}

	private enum EUIObj_MissionResult
	{
		UIObj_MissionClear,
		UIObj_MissionFail,
	}

	private enum EProgressBar
	{
		ProgressBar_Exp,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CUIWidgetProgressExtend _pProgressBar_Exp;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInitShowUI(bool bClear)
	{
		DoEditLabel_StageInfo(1, bClear);
		DoShow();
	}

	public void DoEditLabel_StageInfo(int iCurStage, bool bClear)
	{
		GetUILabel(EUILabel.Label_StageNum).text = iCurStage.ToString();

		EUIObj_MissionResult eMissionResult = bClear ? EUIObj_MissionResult.UIObj_MissionClear : EUIObj_MissionResult.UIObj_MissionFail;

		int iLen = PrimitiveHelper.GetEnumLength<EUIObj_MissionResult>();
		for (int i = 0; i < iLen; i++)
			GetGameObject(((EUIObj_MissionResult)i).ToString()).SetActive(i == (int)eMissionResult);
	}

	public void DoEditLabel_LevelInfo(int iCurLevel, int iCurExp, int iMaxExp)
	{
		GetUILabel(EUILabel.Label_LevelNum).text = iCurLevel.ToString();

		string strLocValue = CManagerUILocalize.DoGetCurrentLocalizeValue(EUILabel.Label_Mission_LevelNextExp.ToString_GarbageSafe());
		string strFormat = string.Format(strLocValue, iMaxExp - iCurExp);

		GetUILabel(EUILabel.Label_Mission_LevelNextExp).text = strFormat;

		float fPercent_CurExp = (float)iCurExp / iMaxExp;
		_pProgressBar_Exp.DoStartTween(fPercent_CurExp, 0, iCurExp, "{0:#,###}");
	}

	public void DoSetTotal_Score(int iScoreTotal)
	{
		DoEditLabel(EUILabel.Label_ScoreNum, iScoreTotal.ToString().CommaString());
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void IOnClick_Buttons(EUIButton eButton)
	{
		switch (eButton)
		{
			case EUIButton.Button_Lobby:
				break;
			case EUIButton.Button_NextMission:
				break;
			case EUIButton.Button_RetryMission:
				break;
			case EUIButton.Button_BonusGame:
				break;
		}

		DoStartFadeInOutPanel_Delayed(false);
		PCManagerFramework.DoLoadScene_FadeInOut(ESceneName.OutGame, 1f, Color.black);
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventOnMakeResource_Bonus(EMissionBonus eBonusName, PCUIScrollViewItem_Bonus pBonus)
	{

	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pProgressBar_Exp = GetGameObject(EProgressBar.ProgressBar_Exp).transform.GetComponent<CUIWidgetProgressExtend>();

		/*
		CManagerPooling<EMissionBonus, PCUIScrollViewItem_Bonus> pManagerPool_Bonus = CManagerPooling<EMissionBonus, PCUIScrollViewItem_Bonus>.instance;
		pManagerPool_Bonus.p_pObjectManager.transform.SetParent(p_pTransCached);
		pManagerPool_Bonus.p_pObjectManager.transform.DoResetTransform();
		pManagerPool_Bonus.p_EVENT_OnMakeResource += EventOnMakeResource_Bonus;
		*/
	}

	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		DoEditLabel_StageInfo(1, false);
		DoEditLabel_LevelInfo(24, 1250, 5000);

		DoEditLabel(EUILabel.Label_GetGoldNum, "12345".CommaString());
		DoEditLabel(EUILabel.Label_ScoreNum, PCManagerInMission.instance.p_iScoreTotal.ToString().CommaString());
		DoEditLabel(EUILabel.Label_HighScoreNum, PCManagerInMission.instance.p_iScoreTotal.ToString().CommaString());

		PCManagerFramework.DoSetTimeScale( 0f );

		PCManagerFramework.p_pManagerSound.DoStopAllSound(false);
	}

	protected override void OnHide()
	{
		base.OnHide();

		PCManagerFramework.DoSetTimeScale( 1f );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
