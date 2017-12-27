#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if DOTween
public class CDOTweenAnimDialogueBase<ENUM_BUTTON> : CUGUIPanelHasInputBase<ENUM_BUTTON>
{
	/* const & readonly declaration             */

	private const float const_fTextAnimDuration = 0.2f;

	/* enum & struct declaration                */

#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private CDialogueNPCBase _pDialogue;

	[SerializeField] private Text _pText_NPCName;
	[SerializeField] private Text _pText_Dialogue;

#endregion Field

#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

#endregion Public

	// ========================================================================== //

#region Protected

	/* protected - [abstract & virtual]         */

	protected virtual void OnStart_DialogueTextAnimation()
	{

	}

	protected virtual void OnFinish_DialogueTextAnimation()
	{

	}

	protected virtual void OnUpdate_DialogueTextAnimation()
	{

	}

	protected virtual void OnFirst_DialogueScene()
	{

	}

	protected virtual void OnEnd_DialogueScene()
	{

	}

	protected virtual void OnChange_DialogueScene(string strNPCName, string strDialogueText)
	{
		_pText_NPCName.text = strNPCName;

		_pText_Dialogue.text = "";
		_pText_Dialogue.DOText(strDialogueText, const_fTextAnimDuration)
					   .OnStart(OnStart_DialogueTextAnimation)
					   .OnUpdate(OnUpdate_DialogueTextAnimation)
					   .OnComplete(OnFinish_DialogueTextAnimation);
	}

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	protected void EventNext_DialogueScene()
	{
		_pDialogue.DoNext_DialogueScene();
	}
	
	protected void EventPrev_DialogueScene()
	{
		_pDialogue.DoPrev_DialogueScene();
	}

	protected void EventInit_DialogueScene(string strNPCName, string strDialogueScene)
	{
		_pDialogue.DoInit_DialogueScene(strNPCName, strDialogueScene);
	}

	/* protected - Override & Unity API         */

	public override void OnClick_Buttons(ENUM_BUTTON eButtonName)
	{

	}

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent(out _pDialogue);
		if (_pDialogue == null)
			_pDialogue = gameObject.AddComponent<CDialogueNPCBase>();

		_pDialogue.p_EVENT_OnChange_DialogueScene += OnChange_DialogueScene;
		_pDialogue.p_EVENT_OnFirst_DialogueScene += OnFirst_DialogueScene;
		_pDialogue.p_EVENT_OnEnd_DialogueScene += OnEnd_DialogueScene;
	}

#endregion Protected

	// ========================================================================== //

#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

#endregion Private
}


#endif