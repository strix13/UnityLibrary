#region Header
/* ============================================ 
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CUGUITab<ENUM_TAB> : CUGUIPanelHasInputBase<ENUM_TAB>
	where ENUM_TAB : struct, System.IConvertible
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	[SerializeField] private ENUM_TAB _eFirstShowTab;
	[SerializeField] private GameObject[] _arrGoTab;

	private Button[] _arrButtonTab;

	private int _iCountTabs;

	private ENUM_TAB _eCurrentShowTab;


	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetTab(ENUM_TAB eTabName)
	{
		EventSetTab(eTabName);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected
	/* protected - [abstract & virtual]         */

	public override void OnClick_Buttons(ENUM_TAB eTabName)
	{
		EventSetTab(eTabName);
	}

	protected virtual void OnClickTab_All(ENUM_TAB eTabName, Button pButton, bool bIsSameTab) { }
	protected virtual void OnClickTab(ENUM_TAB eTabName) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void EventSetTab_Force(ENUM_TAB eTabName)
	{
		ProcSetTab(eTabName);
	}

	private void EventSetTab(ENUM_TAB eTabName)
	{
		if (_eCurrentShowTab.GetHashCode() == eTabName.GetHashCode()) return;

		ProcSetTab(eTabName);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_arrButtonTab = GetComponentsInChildren<Button>(true);
		_iCountTabs = _arrButtonTab.Length;
	}

	protected override void OnEnableSecond()
	{
		base.OnEnableSecond();

		EventSetTab_Force(_eFirstShowTab);
	}
	#endregion Protected

	// ========================================================================== //

	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcSetTab(ENUM_TAB eTabName)
	{
		for (int i = 0; i < _iCountTabs; i++)
		{
			// 박싱이 일어난다 딕셔너리로 캐슁해야함
			bool bIsSameTab = (i == eTabName.ToInt32(null));

			Button pButton = _arrButtonTab[i];

			if (_arrGoTab.Length > 0)
			{
				GameObject pGoTab = _arrGoTab[i];
				pGoTab.SetActive(bIsSameTab);
			}

			if (bIsSameTab)
				OnClickTab(eTabName);

			OnClickTab_All(eTabName, pButton, bIsSameTab);
		}

		_eCurrentShowTab = eTabName;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
