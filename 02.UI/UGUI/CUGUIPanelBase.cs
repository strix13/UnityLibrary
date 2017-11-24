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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( Canvas ))]
public class CUGUIPanelBase : CUIPanelBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	protected Dictionary<string, Text> _mapText = null;
	protected Dictionary<string, Image> _mapImage = null;
	protected Dictionary<string, Dropdown> _mapDropdown = null;
	protected Dictionary<string, CUGUIDropDown> _mapDropdownExtension = null;

	/* private - Field declaration           */

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoEditText<T_TextName>( T_TextName tTextName, object strText )
	{
		FindUIElement( _mapText, tTextName.ToString() ).text = strText.ToString();
	}

	public void DoEditImage<T_ImageName>( T_ImageName tImageName, Sprite pSprite )
	{
		FindUIElement( _mapImage, tImageName.ToString() ).sprite = pSprite;
	}

	public Text GetText<T_TextName>( T_TextName tTextName , bool bIgnoreError = false )
	{
		return FindUIElement( _mapText, tTextName.ToString(), bIgnoreError );
	}

	public Image GetImage<T_ImageName>(T_ImageName tImageName, bool bIgnoreError = false )
	{
		return FindUIElement( _mapImage, tImageName.ToString(), bIgnoreError );
	}

	public Dropdown GetDropdown<T_Dropdown>(T_Dropdown tDropdownName, bool bIgnoreError = false )
	{
		return FindUIElement(_mapDropdown, tDropdownName.ToString(), bIgnoreError );
	}

	public CUGUIDropDown GetDropdown_Extension<T_Dropdown>(T_Dropdown tDropdownName, bool bIgnoreError = false)
	{
		return FindUIElement(_mapDropdownExtension, tDropdownName.ToString(), bIgnoreError );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnSetSortOrder( int iSortOrder )
	{
		if (iSortOrder < 0)
			iSortOrder = 0;

		if (_bAlwaysShow)
			iSortOrder = 100;

		transform.SetSiblingIndex( iSortOrder );
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
