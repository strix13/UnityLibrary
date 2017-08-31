using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

[CustomEditor(typeof(CUIInventorySlot))]
public class CEditorInspector_InventorySlot : Editor
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<CUIInventorySlot> _listSlotSibling = new List<CUIInventorySlot>();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	private void OnEnable()
	{
		_listSlotSibling.Clear();
		CUIInventorySlot pTarget = target as CUIInventorySlot;

		if (Application.isPlaying) return;

		CUIPopupBase pUIPopupOwner = pTarget.GetComponentInParent<CUIPopupBase>();
		pUIPopupOwner.GetComponentsInChildren<CUIInventorySlot>(_listSlotSibling);
		_listSlotSibling.Sort(Comparer_BySiblingIndex);

		for (int i = 0; i < _listSlotSibling.Count; i++)
		{
			if (_listSlotSibling[i] == pTarget)
				pTarget.p_iSlotIndex = i;
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private int Comparer_BySiblingIndex(CUIInventorySlot pObjectX, CUIInventorySlot pObjectY)
	{
		int iSiblingIndexX = pObjectX.transform.GetSiblingIndex();
		int iSiblingIndexY = pObjectY.transform.GetSiblingIndex();

		if(pObjectX.transform.parent != pObjectY.transform.parent)
		{
			iSiblingIndexX = pObjectX.transform.parent.GetSiblingIndex();
			iSiblingIndexY = pObjectY.transform.parent.GetSiblingIndex();
		}

		if (iSiblingIndexX < iSiblingIndexY)
			return -1;
		else if (iSiblingIndexX > iSiblingIndexY)
			return 1;
		else
			return 0;
	}

}
