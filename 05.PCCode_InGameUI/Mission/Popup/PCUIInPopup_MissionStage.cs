using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-08-30 오전 11:19:49
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUIInPopup_MissionStage : CUIPopupBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	private enum EUIObj
	{
		UIObj_BossSpawn,
		UIObj_BossClear,
		UIObj_StageInfo
	}

	private enum ELabel
	{
		Label_StageNum
	}

	/* public - Variable declaration            */

	public event System.Action p_EVENT_OnFinishTween_BossSpawn;
	public event System.Action p_EVENT_OnFinishTween_BossClear;
	public event System.Action p_EVENT_OnFinishTween_StageInfo;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CNGUITweenAlphaExtend _pTweenAlpha_BossSpawn;
	private CNGUITweenAlphaExtend _pTweenAlpha_BossClear;
	private CNGUITweenAlphaExtend _pTweenAlpha_StageInfo;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoShowUI_BossSpawn(System.Action OnFinishTween_BossSpawn = null)
	{
		p_EVENT_OnFinishTween_BossSpawn = OnFinishTween_BossSpawn;

		GetGameObject(EUIObj.UIObj_BossSpawn).DoEnable();
	}

	public void DoShowUI_BossClear(System.Action OnFinishTween_BossClear = null)
	{
		p_EVENT_OnFinishTween_BossClear = OnFinishTween_BossClear;

		GetGameObject(EUIObj.UIObj_BossClear).DoEnable();
	}

	public void DoShowUI_StageInfo(int iCurStage, int iCurWave, System.Action OnFinishTween_StageInfo = null)
	{
		string strFormat = string.Format("{0}-{1}", iCurStage, iCurWave);

		DoEditLabel(ELabel.Label_StageNum, strFormat);

		p_EVENT_OnFinishTween_StageInfo = OnFinishTween_StageInfo;

		GetGameObject(EUIObj.UIObj_StageInfo).DoEnable();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void OnFinishTween_BossSpawn()
	{
		if (p_EVENT_OnFinishTween_BossSpawn != null)
			p_EVENT_OnFinishTween_BossSpawn();
	}

	private void OnFinishTween_BossClear()
	{
		if (p_EVENT_OnFinishTween_BossClear != null)
			p_EVENT_OnFinishTween_BossClear();
	}

	private void OnFinishTween_StageInfo()
	{
		if (p_EVENT_OnFinishTween_StageInfo != null)
			p_EVENT_OnFinishTween_StageInfo();
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pTweenAlpha_BossSpawn = GetGameObject(EUIObj.UIObj_BossSpawn).GetComponent<CNGUITweenAlphaExtend>();
		_pTweenAlpha_BossSpawn.SetOnFinished(OnFinishTween_BossSpawn);
		GetGameObject(EUIObj.UIObj_BossSpawn).DoDisable();

		_pTweenAlpha_BossClear = GetGameObject(EUIObj.UIObj_BossClear).GetComponent<CNGUITweenAlphaExtend>();
		_pTweenAlpha_BossClear.SetOnFinished(OnFinishTween_BossClear);
		GetGameObject(EUIObj.UIObj_BossClear).DoDisable();

		_pTweenAlpha_StageInfo = GetGameObject(EUIObj.UIObj_StageInfo).GetComponent<CNGUITweenAlphaExtend>();
		_pTweenAlpha_StageInfo.SetOnFinished(OnFinishTween_StageInfo);
		GetGameObject(EUIObj.UIObj_StageInfo).DoDisable();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
