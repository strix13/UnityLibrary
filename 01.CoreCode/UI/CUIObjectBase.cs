using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface IButton_OnClickListener<ENUM_ButtonName> { void IOnClick_Buttons( ENUM_ButtonName eButtonName ); }

public class CUIObjectBase : CObjectBase
{
	static public HashSet<int> _setIsInit = new HashSet<int>();

	// ========================== [ Division ] ========================== //

	protected Dictionary<string, UIInput> _mapInput = null;
	protected Dictionary<string, UILabel> _mapLabel = null;
	protected Dictionary<string, UIButton> _mapButton = null;
	protected Dictionary<string, UISprite> _mapSprite = null;
	protected Dictionary<string, UIToggle> _mapUIToggle = null;
	protected Dictionary<string, UITexture> _mapUITexture = null;
	
	// ========================== [ Division ] ========================== //

	/// <summary>
	/// NGUI Label을 찾은 뒤 해당 Label의 Text를 수정합니다.
	/// </summary>
	/// <param name="eUILabel">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <param name="strText">수정할 텍스트 내용</param>
	public void DoEditLabel<EUILabel>( EUILabel eUILabel, string strText )
	{
		GetUILabel( eUILabel ).text = strText;
	}

	/// <summary>
	/// NGUI Sprite를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="strCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UISprite GetUISprite<ENUM>( ENUM eCompoName )
	{
		return ProcFindUIElement( _mapSprite, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Label을 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="strCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UILabel GetUILabel<ENUM>( ENUM eCompoName )
	{
		return ProcFindUIElement( _mapLabel, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Button을 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="strCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIButton GetUIButton<ENUM>( ENUM eCompoName )
	{
		return ProcFindUIElement(_mapButton, eCompoName.ToString());
	}

	/// <summary>
	/// NGUI Input를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="strCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIInput GetUIInput<ENUM>( ENUM eCompoName )
	{
		return _mapInput[eCompoName.ToString()];
	}

	/// <summary>
	/// UI Toggle 를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="strCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIToggle GetUIToggle<ENUM>( ENUM eCompoName )
	{
		return _mapUIToggle[eCompoName.ToString()];
	}

	/// <summary>
	/// 해당 프레임의 버튼들 중 1개만 활성화 시킵니다.
	/// </summary>
	/// 
	private List<UIButton> _listButton = new List<UIButton>();
	public void DoEnableButton_OnlyOne<ENUM_BUTTON>(ENUM_BUTTON eButtonEnable)
	{
		string strButtonName = eButtonEnable.ToString();
		_mapButton.Values.ToList(_listButton);

		for (int i = 0; i < _listButton.Count; i++)
			_listButton[i].isEnabled = strButtonName.Equals( _listButton[i].name );
	}
	
	public void DoEnableButton<ENUM_BUTTON>(ENUM_BUTTON eButtonEnable, bool bEnable)
	{
		string strButtonName = eButtonEnable.ToString();
		_mapButton[strButtonName].isEnabled = bEnable;
	}

	/// <summary>
	/// 해당 프레임의 버튼들을 전부 비, 활성 시킵니다.
	/// </summary>
	public void DoEnableFrameButtons(bool bEnable)
	{
		_mapButton.Values.ToList(_listButton);

		int iLen = _listButton.Count;
		for (int i = 0; i < iLen; i++)
			_listButton[i].isEnabled = bEnable;
	}

	static public System.Type EventGetInterface_GenericParameter( System.Type pClass, string strInterfaceName )
	{
		System.Type[] arrInterfaces = pClass.GetInterfaces();
		for (int i = 0; i < arrInterfaces.Length; i++)
		{
			if (arrInterfaces[i].Name.Contains( strInterfaceName ))
			{
				System.Type[] arrGenericParameter = arrInterfaces[i].GetGenericArguments();
				return arrGenericParameter[0];
			}
		}

		return null;
	}

	// ========================== [ Division ] ========================== //

	virtual protected void OnUIClick() { }
	virtual protected void OnUIPress( bool bPress ) { }
	virtual protected void OnUIHover( bool bHover ) { }
	virtual protected void OnUIDrag( bool bIsDrag ) { }

	// ========================== [ Division ] ========================== //

	void OnClick() { OnUIClick(); }
	void OnPress( bool bPress ) { OnUIPress( bPress ); }
	void OnHover( bool bHover ) { OnUIHover( bHover ); }
	void OnDragStart() { OnUIDrag( true ); }
	void OnDragEnd() { OnUIDrag( false ); }

	protected override void OnAwake()
	{
		base.OnAwake();
		
		if (GetComponentInChildren<UIButton>() != null)
		{
			System.Type pType = GetType();
			System.Type pEnumButton = EventGetInterface_GenericParameter(pType, "IButton_OnClickListener");
			if (pEnumButton != null)
			{
				// 제네릭 타입을 수동으로 지정해준 후 Invoke 호출해주는 방식
				MethodInfo Method_UIButton_OnClickListner = pType.GetMethod("EventInitUIButtons");
				MethodInfo Method_Generic = Method_UIButton_OnClickListner.MakeGenericMethod(pEnumButton);
				Method_Generic.Invoke(this, null);
			}
		}
	}

	// ========================== [ Division ] ========================== //

	public void EventInitUIButtons<ENUM_Button_Name>()
	{
		ENUM_Button_Name[] arrEnum = (ENUM_Button_Name[])System.Enum.GetValues( typeof( ENUM_Button_Name ) );
		for (int i = 0; i < arrEnum.Length; i++)
			_setIsInit.Add( arrEnum[i].GetHashCode() );

		UIButton[] arrButtons = GetComponentsInChildren<UIButton>( true );
		int iLen = arrButtons.Length;

		if(iLen != 0)
			_mapButton = new Dictionary<string, UIButton>();

		for (int i = 0; i < iLen; i++)
		{
			UIButton pButton = arrButtons[i];

			string strName = pButton.name;
			if (strName.Contains( "Toggle" )) continue;

			ENUM_Button_Name eButtonName;
			if (strName.ConvertEnum( out eButtonName ) == false)
				Debug.LogWarning( "button 등록 실패" + pButton.name, pButton );

			EventDelegate.Add( pButton.onClick, new EventDelegate( this, "IOnClick_Buttons", eButtonName.GetHashCode() ) );
			_setIsInit.Remove( eButtonName.GetHashCode() );

			if (_mapButton.ContainsKey( pButton.name ) == false)
				_mapButton.Add( strName, pButton );
			else
				Debug.LogWarning( "이미 Dictionary에 들어가있습니다" + strName );
		}

		IEnumerator<int> pIter = _setIsInit.GetEnumerator();
		while (pIter.MoveNext())
		{
			Debug.LogWarning( string.Format( "{0}의 Enum 중 Init이 안된 버튼 : {1}", this.name, arrEnum[pIter.Current] ), this );
		}
		_setIsInit.Clear();
	}

	private UI_ELEMENT ProcFindUIElement<UI_ELEMENT>( Dictionary<string, UI_ELEMENT> mapUIElements, string strUIElement )
		where UI_ELEMENT : Component
	{
		if (mapUIElements == null)
			mapUIElements = new Dictionary<string, UI_ELEMENT>();

		if (mapUIElements.ContainsKey( strUIElement ))
			return mapUIElements[strUIElement];

		UI_ELEMENT[] arrUIElements = GetComponentsInChildren<UI_ELEMENT>( true );

		int iLen = arrUIElements.Length;
		for (int i = 0; i < iLen; i++)
		{
			UI_ELEMENT pUIElement = arrUIElements[i];
			string strElementName = pUIElement.name;

			if (strElementName.Equals( strUIElement ))
			{
				if (mapUIElements.ContainsKey( strElementName ) == false)
					mapUIElements.Add( strUIElement, pUIElement );
				else
					Debug.LogWarning( strElementName + " 키 값이 중복되었습니다.", this );

				return pUIElement;
			}
		}

		Debug.LogWarning( strUIElement + " 를 찾을 수 없습니다... ", this );
		return null;
	}
}
