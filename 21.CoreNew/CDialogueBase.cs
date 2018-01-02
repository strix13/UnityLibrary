using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class CDialogueBase : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public event System.Action<string, string> p_EVENT_OnChange_DialogueScene;
	public event System.Action p_EVENT_OnFirst_DialogueScene;
	public event System.Action p_EVENT_OnEnd_DialogueScene;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<int, string> _mapDialogueText = new Dictionary<int, string>();

	private int _iCurDialogueScene; public int p_iCurrentDialogue { get { return _iCurDialogueScene; } }
	private int _iMaxDialogueScene;

	private string _strNPCName;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoInit_DialogueScene(string strNPCName, string strDialogueScene)
	{
		EventSet_NPCName(strNPCName);

		EventParse_DialogueScene(strDialogueScene);
	}
	
	public void DoStart_DialogueScene()
	{
		EventStart_DialogueScene();
	}

	public void DoSet_NPCName(string strNPCName)
	{
		EventSet_NPCName(strNPCName);
	}

	public void DoPrev_DialogueScene()
	{
		EventPrev_DialogueScene();
	}

	public void DoNext_DialogueScene()
	{
		EventNext_DialogueScene();
	}

	/* private - [Do] Function
	 * 내부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventStart_DialogueScene()
	{
		EventSet_DialogueScene(0);
	}

	private void EventNext_DialogueScene()
	{
		_iCurDialogueScene++;

		EventSet_DialogueScene(_iCurDialogueScene);
	}

	private void EventPrev_DialogueScene()
	{
		_iCurDialogueScene--;

		EventSet_DialogueScene(_iCurDialogueScene);
	}

	private void EventSet_DialogueScene(int iDialogueScene)
	{
		_iCurDialogueScene = Mathf.Clamp(iDialogueScene, 0, _iMaxDialogueScene);

		if (_iCurDialogueScene == 0 && p_EVENT_OnFirst_DialogueScene != null)
			p_EVENT_OnFirst_DialogueScene();
		else if (_iCurDialogueScene == _iMaxDialogueScene && p_EVENT_OnEnd_DialogueScene != null)
			p_EVENT_OnEnd_DialogueScene();

		EventSet_DialogueText(_iCurDialogueScene);
	}

	private void EventSet_NPCName(string strNPCName)
	{
		_strNPCName = strNPCName;
	}

	private void EventSet_DialogueText(int iDialogueScene)
	{
		string strDialogueText = _mapDialogueText[iDialogueScene];

		if (p_EVENT_OnChange_DialogueScene != null)
			p_EVENT_OnChange_DialogueScene(_strNPCName, strDialogueText);
	}

	private void EventParse_DialogueScene(string strDialogueScene)
	{
		ProcParse_DialogueEvent(strDialogueScene);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcParse_DialogueEvent(string strDialogueKey)
	{
		List<string> listValue = CManagerUILocalize.DoGetLocalizeValueContains(strDialogueKey);

		int iCount = listValue.Count;
		for (int i = 0; i < iCount; i++)
		{
			string strValue = listValue[i];
			string[] arrStrValue = strValue.Split(';');

			string strDialogueText = arrStrValue[0];
			string strMethodName = arrStrValue[1];

			if (string.IsNullOrEmpty(strDialogueText) == false)
				_mapDialogueText.Add(i, strDialogueText);

			if (string.IsNullOrEmpty(strMethodName) == false)
				print(strMethodName);
		}

		_iMaxDialogueScene = iCount - 1;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}