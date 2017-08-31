using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Spine.Unity;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public enum EDialogue
{
	Tutorial_Main,
	Tutorial_Shop
}

public enum EPhaseDialogue
{
	None,
	Pause,
}

public class PCUISharedFrame_Dialogue : CUIFrameBase, IButton_OnClickListener<PCUISharedFrame_Dialogue.EUIButton>
{
	/* const & readonly declaration             */

	private const string const_strCommandPrefix = "/";

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_DialogueNext,
		Button_DialogueSkip
	}

	public enum EUILabel
	{
		Label_DialogueName,
		Label_DialogueText,
	}

	private struct SInfoEvent
	{
		public enum EEventType
		{
			SkipAndEvent,
			Event,
			Skip,
		}

		public int iEventID;
		public EEventType eEventType;
		public EventDelegate pEventDelegate;
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<int, SInfoEvent> _mapDialogueEvent = new Dictionary<int, SInfoEvent>();

	private UILabel _pUILabel_DialogueName;
	private UILabel _pUILabel_DialogueText;
	private TypewriterEffect _pTextEffect;

	private UIButton _pUIButton_DialogueNext;

	private SkeletonAnimation _pCharacter; //	나중에 캐릭 바뀔때 쓸꺼임
	private EventDelegate _pLastDialogueEvent = null;

	private int _iCurDialogue; public int p_iCurrentDialogue { get { return _iCurDialogue; } }

	private string _strDialogueType;
	private string _strCharacterName;

	private bool _bCurDialogueAnim;

	private EPhaseDialogue _ePhaseDialogue;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInitUI(ECharacterName eCharacterName, EDialogue eDialogueType, MonoBehaviour pEventTarget = null)
	{
		_strCharacterName = eCharacterName.ToString_GarbageSafe();
		_strDialogueType = eDialogueType.ToString_GarbageSafe();

		_mapDialogueEvent.Clear();

		if (pEventTarget == null) return;

		// /event 메서드명, /skip 명령어 파싱 시작
		List<string> listValue = CManagerUILocalize.DoGetLocalizeValueContains(eDialogueType.ToString_GarbageSafe());

		int iLen = listValue.Count;
		for (int i = 0; i < iLen; i++)
		{
			string strValue = listValue[i];

			if (strValue.Contains(const_strCommandPrefix) == false) continue;

			string strReplaceValue = strValue.Substring(strValue.IndexOf(const_strCommandPrefix)).Replace(const_strCommandPrefix, "");
			string[] arrStrValue = strReplaceValue.Split(' ');

			string strEventType = arrStrValue[0];
			string strMethodName = arrStrValue[1];

			SInfoEvent.EEventType eEventType = SCEnumHelper.ConvertEnum<SInfoEvent.EEventType>(strEventType);

			SInfoEvent sInfoEvent = new SInfoEvent();
			sInfoEvent.iEventID = i;
			sInfoEvent.eEventType = eEventType;
			sInfoEvent.pEventDelegate = new EventDelegate(pEventTarget, strMethodName);

			_mapDialogueEvent.Add(i, sInfoEvent);
		}

		Debug.LogWarning("로컬라이징 데이터 로딩 : " + CManagerUILocalize.p_bIsFinishParse);
		Debug.LogWarning("튜토리얼 이벤트 카운트" + _mapDialogueEvent.Count);
	}

	public void DoSetPhaseDialogue(EPhaseDialogue ePhase)
	{
		_ePhaseDialogue = ePhase;
	}

	public void DoSetIndexAndShowDialogue(int iDialogue)
	{
		DoSetIndexDialogue(iDialogue);
		DoShow();
	}

	public void DoSetIndexDialogue(int iDialogue)
	{
		_iCurDialogue = iDialogue;
	}

	public void DoSkipDialogue()
	{
		IEnumerator<KeyValuePair<int, SInfoEvent>> pIter = _mapDialogueEvent.GetEnumerator();
		while (pIter.MoveNext())
		{
			KeyValuePair<int, SInfoEvent> pCurrent = pIter.Current;
			SInfoEvent sInfoEvent = pCurrent.Value;

			int iEventID = pCurrent.Key;
			if (iEventID > _iCurDialogue && sInfoEvent.eEventType == SInfoEvent.EEventType.Skip)
			{
				DoSetIndexDialogue(iEventID);

				sInfoEvent.pEventDelegate.Execute();
				break;
			}
		}
	}

	/* private - [Do] Function
	 * 내부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void IOnClick_Buttons(EUIButton eButton)
	{
		switch (eButton)
		{
			case EUIButton.Button_DialogueNext:
				ProcAnimDialogueText();
				break;
		}
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventOnFinish_AnimText()
	{
		_bCurDialogueAnim = false;
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUILabel_DialogueName = GetUILabel(EUILabel.Label_DialogueName);
		_pUILabel_DialogueText = GetUILabel(EUILabel.Label_DialogueText);

		_pUIButton_DialogueNext = GetUIButton(EUIButton.Button_DialogueNext);

		_pTextEffect = _pUILabel_DialogueText.GetComponent<TypewriterEffect>();
		EventDelegate.Set(_pTextEffect.onFinished, EventOnFinish_AnimText);

		EventDelegate.Add(GetUIButton(EUIButton.Button_DialogueSkip).onClick, DoSkipDialogue);
	}

	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		_pTextEffect.enabled = false;
		_pTextEffect.enabled = true;

		_bCurDialogueAnim = false;
		_pLastDialogueEvent = null;

		DoSetPhaseDialogue(EPhaseDialogue.None);

		ProcAnimDialogueText();
	}

	protected override void OnHide()
	{
		base.OnHide();

		DoSetPhaseDialogue(EPhaseDialogue.Pause);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcAnimDialogueText()
	{
		if (_bCurDialogueAnim)
		{
			_pTextEffect.Finish();
			return;
		}

		if (_pLastDialogueEvent != null)
		{
			_pLastDialogueEvent.Execute();
			_pLastDialogueEvent = null;
		}

		// 성능 및 속도에 문제가 된다면 미리 캐슁을 해야한다.

		string strDialogueName = CManagerUILocalize.DoGetCurrentLocalizeValue(string.Format("{0}", _strCharacterName));
		string strDialogueText = CManagerUILocalize.DoGetCurrentLocalizeValue(string.Format("{0}_{1}", _strDialogueType, _iCurDialogue));
		string strReplaceValue = strDialogueText;

		if (strDialogueText.Contains(const_strCommandPrefix))
			strReplaceValue = strDialogueText.Substring(0, strDialogueText.IndexOf(const_strCommandPrefix));
		
		//

		_pUILabel_DialogueName.text = strDialogueName;
		_pUILabel_DialogueText.text = strReplaceValue;

		_pTextEffect.ResetToBeginning();
		_bCurDialogueAnim = true;

		if (_mapDialogueEvent.ContainsKey(_iCurDialogue))
		{
			SInfoEvent sInfoEvent = _mapDialogueEvent[_iCurDialogue];

			if (sInfoEvent.eEventType == SInfoEvent.EEventType.Event || sInfoEvent.eEventType == SInfoEvent.EEventType.SkipAndEvent)
				_pLastDialogueEvent = sInfoEvent.pEventDelegate;
		}

		if (_ePhaseDialogue != EPhaseDialogue.Pause)
			_iCurDialogue++;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
