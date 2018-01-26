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
#if dUI_DoozyUI
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DoozyUI;

public class CManagerDoozyUI<CLASS, ENUM_UIELEMENT> : CSingletonBase<CLASS>
    where CLASS : CManagerDoozyUI<CLASS, ENUM_UIELEMENT>
    where ENUM_UIELEMENT  : System.IFormattable, System.IConvertible, System.IComparable
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	static public string p_strCategory
	{
		get
		{
			if (string.IsNullOrEmpty( _strCategory ))
				_strCategory = typeof( CLASS ).Name;

			return _strCategory;
		}
	}
	

	/* private - Field declaration           */

	private Dictionary<string, UnityAction> _mapRemoveAnimationsStart = new Dictionary<string, UnityAction>();
	private Dictionary<string, UnityAction> _mapRemoveAnimationsFinish = new Dictionary<string, UnityAction>();

	private Dictionary<ENUM_UIELEMENT, Component> _mapCachedTypeUIElement = new Dictionary<ENUM_UIELEMENT, Component>();

	private List<string> _listNavigation = new List<string>();

	static private string _strCategory = "";

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public static COMPONENT GetUITypeByElement<COMPONENT>(ENUM_UIELEMENT eUIElementName)
		where COMPONENT : Component
	{
		COMPONENT pCompo = null;

		if (instance._mapCachedTypeUIElement.ContainsKey(eUIElementName))
			return instance._mapCachedTypeUIElement[eUIElementName] as COMPONENT;

		return pCompo;
	}

	public static void DoShowUIElement(ENUM_UIELEMENT eUIElementName, UnityAction OnInAnimationsStart= null, UnityAction OnInAnimationsFinish = null)
	{
		if (instance._mapCachedTypeUIElement.ContainsKey(eUIElementName) == false) return;
		if (instance._mapCachedTypeUIElement[eUIElementName].gameObject.activeInHierarchy) return;

		ProcShowUIElement(eUIElementName.ToString(), OnInAnimationsStart, OnInAnimationsFinish);
	}

	public static void DoShowUIElementNav(ENUM_UIELEMENT eUIElementName, UnityAction OnInAnimationsStart = null, UnityAction OnInAnimationsFinish = null)
	{
		ProcShowUIElementAndAddNavigation(eUIElementName.ToString(), OnInAnimationsStart, OnInAnimationsFinish);
	}

	public static void DoHideUIElement(ENUM_UIELEMENT eUIElementName, UnityAction OnOutAnimationsStart = null, UnityAction OnOutAnimationsFinish = null)
	{
		if (instance._mapCachedTypeUIElement.ContainsKey(eUIElementName) == false) return;
		if (instance._mapCachedTypeUIElement[eUIElementName].gameObject.activeInHierarchy == false) return;

		ProcHideUIElement( eUIElementName.ToString(), OnOutAnimationsStart, OnOutAnimationsFinish);
	}

	public static UITYPE DoShowUIElement<UITYPE>(ENUM_UIELEMENT eUIElementName, UnityAction OnInAnimationsStart = null, UnityAction OnInAnimationsFinish = null)
		where UITYPE : Component
	{
		ProcShowUIElement( eUIElementName.ToString(), OnInAnimationsStart, OnInAnimationsFinish);
		return GetUITypeByElement<UITYPE>( eUIElementName);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected virtual void OnShow_ExitMenu() { }

	protected virtual void OnPreInitUI() { }
	protected virtual void OnInitUI() { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	private void Awake()
	{
		OnPreInitUI();
		ProcRegisterUIElement();
		OnInitUI();

		OnAwake();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (Input.GetButtonUp("Cancel"))
		{
			int iCount = _listNavigation.Count;
			if (iCount > 0)
			{
				string strUIElementName = _listNavigation[iCount - 1];
				ProcHideUIElement(strUIElementName, null, null);
			}
			else
				OnShow_ExitMenu();
		}
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private static void ProcShowUIElement(string strElementName, UnityAction OnInAnimationsStart, UnityAction OnInAnimationsFinish)
	{
		print("Show - " + strElementName);
		ProcInitAnimationEvent(strElementName, p_strCategory, Anim.AnimationType.In, OnInAnimationsStart, OnInAnimationsFinish);
		UIManager.ShowUiElement(strElementName, p_strCategory, false);
	}

	private static void ProcHideUIElement(string strElementName, UnityAction OnOutAnimationsStart, UnityAction OnOutAnimationsFinish)
	{
		print("Hide - " + strElementName);
		ProcInitAnimationEvent(strElementName, p_strCategory, Anim.AnimationType.Out, OnOutAnimationsStart, OnOutAnimationsFinish);
		UIManager.HideUiElement(strElementName, p_strCategory, false);

		if (instance._listNavigation.Contains(strElementName))
			instance._listNavigation.Remove(strElementName);
	}

	private static void ProcShowUIElementAndAddNavigation(string strElementName, UnityAction OnOutAnimationsStart, UnityAction OnOutAnimationsFinish)
	{
		ProcShowUIElement(strElementName, OnOutAnimationsStart, OnOutAnimationsFinish);

		if (instance._listNavigation.Contains(strElementName) == false)
			instance._listNavigation.Add(strElementName);
	}

	private static void ProcInitAnimationEvent(string strUIElementName, string strUICategoryName, Anim.AnimationType eAnimationType, UnityAction OnAnimationsStart, UnityAction OnAnimationsFinish)
	{
		List<UIElement> listUIElement = UIManager.GetUiElements(strUIElementName, strUICategoryName);
		int iCount = listUIElement.Count;
		for (int i = 0; i < iCount; i++)
		{
			UIElement pUIElement = listUIElement[i];

			// DoozyUI UIElement 의 애니메이션 콜백은 이미 있으므로
			// RemoveAllListener 를 해주면 기존 콜백 작동이 안된다.
			// 그래서 수동으로 넣고 임의로 삭제해준다...
			if (instance._mapRemoveAnimationsStart.ContainsKey(strUIElementName))
			{
				UnityAction onAction = instance._mapRemoveAnimationsStart[strUIElementName];

				pUIElement.OnInAnimationsStart.RemoveListener(onAction);
				pUIElement.OnOutAnimationsStart.RemoveListener(onAction);
				instance._mapRemoveAnimationsStart.Remove(strUIElementName);
			}

			if (instance._mapRemoveAnimationsFinish.ContainsKey(strUIElementName))
			{
				UnityAction onAction = instance._mapRemoveAnimationsFinish[strUIElementName];

				pUIElement.OnInAnimationsFinish.RemoveListener(onAction);
				pUIElement.OnOutAnimationsFinish.RemoveListener(onAction);
				instance._mapRemoveAnimationsFinish.Remove(strUIElementName);
			}

			switch (eAnimationType)
			{
				case Anim.AnimationType.In:
					if (OnAnimationsStart != null)
					{
						pUIElement.OnInAnimationsStart.AddListener(OnAnimationsStart);
						instance._mapRemoveAnimationsStart.Add(strUIElementName, OnAnimationsStart);
					}

					if (OnAnimationsFinish != null)
					{
						pUIElement.OnInAnimationsFinish.AddListener(OnAnimationsFinish);
						instance._mapRemoveAnimationsFinish.Add(strUIElementName, OnAnimationsFinish);
					}
					break;
				case Anim.AnimationType.Out:
					if (OnAnimationsStart != null)
					{
						pUIElement.OnOutAnimationsStart.AddListener(OnAnimationsStart);
						instance._mapRemoveAnimationsStart.Add(strUIElementName, OnAnimationsStart);
					}

					if (OnAnimationsFinish != null)
					{
						pUIElement.OnOutAnimationsFinish.AddListener(OnAnimationsFinish);
						instance._mapRemoveAnimationsFinish.Add(strUIElementName, OnAnimationsFinish);
					}
					break;
			}
		}
	}

	private void ProcRegisterUIElement()
	{
		print("Start Register UIElement");
		UIElement[] arrUIElement = GetComponentsInChildren<UIElement>(true);

		int iLen = arrUIElement.Length;
		for (int i = 0; i < iLen; i++)
		{
			UIElement pUIElement = arrUIElement[i];
			CUGUIPanelBase pPanel = pUIElement.GetComponent<CUGUIPanelBase>();

			if (pPanel == null) { pUIElement.RegisterToUIManager(); continue; }

			string strUIElementName = pUIElement.elementName;
			ENUM_UIELEMENT eUIElement = strUIElementName.ConvertEnum<ENUM_UIELEMENT>();

			if (_mapCachedTypeUIElement.ContainsKey(eUIElement))
			{
				Debug.LogWarning("이미 _mapCachedTypeUIElement 에 " + eUIElement + " 가 있습니다...");
			}
			else
			{
				_mapCachedTypeUIElement.Add(eUIElement, pPanel as Component);
				pUIElement.RegisterToUIManager();
			}

			pUIElement.gameObject.SetActive(true);
		}
		print("Finish Register UIElement");
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
   #endif