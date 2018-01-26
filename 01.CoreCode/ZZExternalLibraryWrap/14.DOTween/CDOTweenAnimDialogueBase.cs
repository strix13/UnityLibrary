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
using System;

#if DOTween
using DG.Tweening;
abstract public class CDOTweenAnimDialogueBase<ENUM_BUTTON> : CUGUIPanelHasInputBase<ENUM_BUTTON>, IDialogueListner
{
    /* const & readonly declaration             */

    private const float const_fTextAnimDuration = 0.2f;

    /* enum & struct declaration                */

    #region Field

    /* public - Field declaration            */

    /* protected - Field declaration         */

    /* private - Field declaration           */

    protected CManagerDialogue _pManagerDialogue;

    [SerializeField]
    private Text _pText_NPCName = null;
    [SerializeField]
    private Text _pText_Dialogue = null;

    #endregion Field

    private string _strDialogueEvent;

    #region Public

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    public void EventInit_DialogueScene(string strDialogueEvent)
    {
        _pManagerDialogue.DoAddListner(strDialogueEvent, this);
    }

    public void EventStart_DialogueScene(string strDialogueEvent )
    {
		_strDialogueEvent = strDialogueEvent;
		_pManagerDialogue.DoStart_DialogueScene(_strDialogueEvent);
    }

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

    virtual public void IDialogueListner_OnStartDialogue(SDataTable_Dialogue pDialogueData) { }
	virtual public void IDialogueListner_OnFinishDialogue() { }
	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	protected void EventNext_DialogueScene() { _pManagerDialogue.DoNext_DialogueScene(); }
    protected void EventPrev_DialogueScene() { _pManagerDialogue.DoPrev_DialogueScene(); }

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pManagerDialogue = CManagerDialogue.instance;
    }

	public void IDialogueListner_OnPlayDialogue( SDataTable_Dialogue pDialogueData, bool bIsLast )
	{
		_pText_NPCName.text = pDialogueData.str배우;

		_pText_Dialogue.text = "";
		_pText_Dialogue.DOText( pDialogueData.str대사, const_fTextAnimDuration )
					   .OnStart( OnStart_DialogueTextAnimation )
					   .OnUpdate( OnUpdate_DialogueTextAnimation )
					   .OnComplete( OnFinish_DialogueTextAnimation );
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