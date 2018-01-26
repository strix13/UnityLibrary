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

public class CUGUIObjectBase : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	protected Dictionary<string, Text> _mapText = null;
	protected Dictionary<string, Image> _mapImage = null;
	protected Dictionary<string, Slider> _mapSlider = null;
	protected Dictionary<string, Button> _mapButton = null;
	protected Dictionary<string, Toggle> _mapToggle = null;
	protected Dictionary<string, Dropdown> _mapDropdown = null;
	protected Dictionary<string, CUGUIDropDown> _mapDropdownExtension = null;

#if TMPro
	protected Dictionary<string, TMPro.TMP_Text> _mapTextMesh = null;
#endif

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

#if TMPro
	public void DoEditTextMesh<T_TextMeshName>( T_TextMeshName tTextMeshProName, object strText )
	{
		if (strText == null)
			FindUIElement( _mapTextMesh, tTextMeshProName.ToString() ).text = "";
		else
			FindUIElement( _mapTextMesh, tTextMeshProName.ToString() ).text = strText.ToString();
	}
#endif

	public void DoEditText<T_TextName>( T_TextName tTextName, object strText, bool bShow = true )
	{
		Text pText = FindUIElement(_mapText, tTextName.ToString());

		if (strText == null)
			pText.text = "";
		else
			pText.text = strText.ToString();

		pText.gameObject.SetActive(bShow);
	}

	public void DoEditImage<T_ImageName>( T_ImageName tImageName, Sprite pSprite, bool bNativeSize = false )
	{
		Image pImage = FindUIElement(_mapImage, tImageName.ToString());
		pImage.sprite = pSprite;
		if (bNativeSize)
			pImage.SetNativeSize();
	}

	public Text GetText<T_TextName>( T_TextName tTextName, bool bIgnoreError = false )
	{
		return FindUIElement( _mapText, tTextName.ToString(), bIgnoreError );
	}

	public Image GetImage<T_ImageName>( T_ImageName tImageName, bool bIgnoreError = false )
	{
		return FindUIElement( _mapImage, tImageName.ToString(), bIgnoreError );
	}

	public Slider GetSlider<T_Slider>( T_Slider tSlider, bool bIgnoreError = false )
	{
		return FindUIElement( _mapSlider, tSlider.ToString(), bIgnoreError );
	}

	public Button GetButton<T_Button>(T_Button tButton, bool bIgnoreError = false)
	{
		return FindUIElement(_mapButton, tButton.ToString(), bIgnoreError);
	}

	public Dropdown GetDropdown<T_Dropdown>( T_Dropdown tDropdownName, bool bIgnoreError = false )
	{
		return FindUIElement( _mapDropdown, tDropdownName.ToString(), bIgnoreError );
	}

	public CUGUIDropDown GetDropdown_Extension<T_Dropdown>( T_Dropdown tDropdownName, bool bIgnoreError = false )
	{
		return FindUIElement( _mapDropdownExtension, tDropdownName.ToString(), bIgnoreError );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
