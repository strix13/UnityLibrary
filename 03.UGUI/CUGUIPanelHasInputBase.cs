#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public interface IDropDownInitializer
{
	void IDropDownInitializer_Regist_DropDownItem( CUGUIDropdownItem pItem );
}

//[RequireComponent( typeof( GraphicRaycaster ) )]
//[RequireComponent( typeof( CanvasScaler ) )]
abstract public class CUGUIPanelHasInputBase<Enum_InputName> : CUGUIPanelBase, IDropDownInitializer
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private List<CUGUIScrollItem> _listScrollView = new List<CUGUIScrollItem>();

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public Button GetButton<T_BUTTON>(T_BUTTON tButton)
	{
		 return FindUIElement(_mapButton, tButton.ToString());
	}

	public Toggle GetToggle<T_TOGGLE>(T_TOGGLE tToggle)
	{
		return FindUIElement(_mapToggle, tToggle.ToString());
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void IDropDownInitializer_Regist_DropDownItem( CUGUIDropdownItem pItem)
	{
		Dropdown pDropDown = pItem.GetComponentInParent<Dropdown>();
		Enum_InputName eDropDownName;
		if (pDropDown.name.ConvertEnum( out eDropDownName ))
			pItem.DoInitItem( eDropDownName.GetHashCode(), EventOnPointerEnter );
	}

	public void EventOnPointerEnter(int iOwnerID, CUGUIDropDown.SDropDownData pData, string strText)
	{
		OnDropDown_HoverItem( (Enum_InputName)(object)iOwnerID, pData, strText);
	}

	public void EventOnChangeDropDown( Enum_InputName eDropDownName, CUGUIDropDown pDropDown )
	{
		string strText = pDropDown.options[pDropDown.value].text;
		OnDropDown_SelectItem( eDropDownName, pDropDown.GetData( strText), strText );
	}

	public void EventInitScrollView<Data_ScrollItem>( List<Data_ScrollItem> listDataScrollItem )
		where Data_ScrollItem : IUGUIScrollItemData
	{
		listDataScrollItem.Sort( ComparerScrollItem );
		for (int i = 0; i < _listScrollView.Count && i < listDataScrollItem.Count; i++)
			_listScrollView[i].EventSetScrollData( listDataScrollItem[i] );
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	abstract public void OnClick_Buttons( Enum_InputName eButtonName );
	protected virtual void OnClick_Buttons(Enum_InputName eButtonName, Button pButton) { OnClick_Buttons(eButtonName); }
	protected virtual void OnClick_Toggles(Enum_InputName eToggle, bool bIsOn) { }

	virtual public void OnPress_And_Hold_Buttons( Enum_InputName eButtonName, bool bPress ) { }
	virtual public void OnScrollView_ClickItem( CUGUIScrollItem pScrollItem, IUGUIScrollItemData pScrollData, Enum_InputName eButtonName ) { }
	virtual public void OnDropDown_SelectItem( Enum_InputName eDropDownName, CUGUIDropDown.SDropDownData pData, string strItemText) { }
	virtual public void OnDropDown_HoverItem( Enum_InputName eDropDownName, CUGUIDropDown.SDropDownData pData, string strItemText ) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		Button[] arrButton = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < arrButton.Length; i++)
		{
			Button pButton = arrButton[i];
			string strButtonName = pButton.name;

			Enum_InputName eButtonName;
			if (strButtonName.ConvertEnum( out eButtonName ))
			{
				pButton.onClick.AddListener(() => { OnClick_Buttons(eButtonName, pButton); });
				if (_mapButton == null)
					_mapButton = new Dictionary<string, Button>();

				_mapButton.Add(strButtonName, pButton);

				CUGUIButton_Press pButtonPress = pButton.GetComponent<CUGUIButton_Press>();
				if (pButtonPress != null)
				{
					pButtonPress.p_Event_OnPress_Down.AddListener( delegate { OnPress_And_Hold_Buttons( eButtonName, true ); } );
					pButtonPress.p_Event_OnPress_Up.AddListener( delegate { OnPress_And_Hold_Buttons( eButtonName, false ); } );
				}
			}
		}

		Toggle[] arrToggle = GetComponentsInChildren<Toggle>(true);
		int iLen = arrToggle.Length;
		for (int i = 0; i < iLen; i++)
		{
			Toggle pToggle = arrToggle[i];
			Enum_InputName eToggleName;
			string strToggleName = pToggle.name;

			if (strToggleName.ConvertEnum(out eToggleName))
			{
				pToggle.onValueChanged.AddListener((bool bIsOn) => { OnClick_Toggles(eToggleName, bIsOn); });

				if (_mapToggle == null)
					_mapToggle = new Dictionary<string, Toggle>();

				if (_mapToggle.ContainsKey(strToggleName) == false)
					_mapToggle.Add(strToggleName, pToggle);
			}
		}

		CUGUIDropDown[] arrDropDown = GetComponentsInChildren<CUGUIDropDown>();
		for (int i = 0; i < arrDropDown.Length; i++)
		{
			CUGUIDropDown pDropDown = arrDropDown[i];
			Enum_InputName eDropDownName;
			if (pDropDown.name.ConvertEnum( out eDropDownName ))
				pDropDown.onValueChanged.AddListener( delegate { EventOnChangeDropDown( eDropDownName, pDropDown ); } );
		}

		GetComponentsInChildren( true, _listScrollView );
		for (int i = 0; i < _listScrollView.Count; i++)
			_listScrollView[i].EventInitScrollItem<Enum_InputName>( OnScrollView_ClickItem );
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */
	   

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	static private int ComparerScrollItem<Data_ScrollItem>( Data_ScrollItem pDataX, Data_ScrollItem pDataY )
	where Data_ScrollItem : IUGUIScrollItemData
	{
		int iSortOrderX = pDataX.IScrollData_GetSortOrder();
		int iSortOrderY = pDataY.IScrollData_GetSortOrder();

		if (iSortOrderX < iSortOrderY)
			return -1;
		else if (iSortOrderX > iSortOrderY)
			return 1;
		else
			return 0;
	}
	#endregion Private
}
