#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIPanel))]
abstract public class CNGUIPanelBase : CUIPanelBase
{
	[SerializeField]
	private List<UIPanel> _listPanelFixedDepth = new List<UIPanel>();
	
	protected Dictionary<string, UIInput> _mapInput = null;
	protected Dictionary<string, UILabel> _mapLabel = null;
	protected Dictionary<string, UIButton> _mapButton = null;
	protected Dictionary<string, UISprite> _mapSprite = null;
	protected Dictionary<string, UIToggle> _mapUIToggle = null;
	protected Dictionary<string, UITexture> _mapUITexture = null;

	protected UIPanel _pUIPanel;

	private List<UIPanel> _listPanel = new List<UIPanel>();

	// ========================== [ Division ] ========================== //

	public void DoEditLabel<T_LabelName>( T_LabelName tLabelName, string strText)
	{
		FindUIElement( _mapLabel, tLabelName.ToString() ).text = strText;
	}

	/// <summary>
	/// NGUI Sprite를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UISprite GetUISprite<ENUM>( ENUM eCompoName )
	{
		return FindUIElement( _mapSprite, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Label을 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UILabel GetUILabel<ENUM>( ENUM eCompoName )
	{
		return FindUIElement( _mapLabel, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Button을 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIButton GetUIButton<ENUM>( ENUM eCompoName )
	{
		return FindUIElement( _mapButton, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Input를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIInput GetUIInput<ENUM>( ENUM eCompoName )
	{
		return _mapInput[eCompoName.ToString()];
	}

	/// <summary>
	/// UI Toggle 를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
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
	public void DoEnableButton_OnlyOne<ENUM_BUTTON>( ENUM_BUTTON eButtonEnable )
	{
		string strButtonName = eButtonEnable.ToString();
		_mapButton.Values.ToList( _listButton );

		for (int i = 0; i < _listButton.Count; i++)
			_listButton[i].isEnabled = strButtonName.Equals( _listButton[i].name );
	}

	public void DoEnableButton<ENUM_BUTTON>( ENUM_BUTTON eButtonEnable, bool bEnable )
	{
		string strButtonName = eButtonEnable.ToString();
		_mapButton[strButtonName].isEnabled = bEnable;
	}

	/// <summary>
	/// 해당 프레임의 버튼들을 전부 활성여부를 세팅합니다.
	/// </summary>
	public void DoEnableFrameButtons( bool bEnable )
	{
		if (_mapButton == null || _mapButton.Count == 0)
		{
			DebugCustom.Log_ForCore( EDebugFilterDefault.Warning_Core, "버튼이 없는데 DoEnableFrameButtons를 호출한다", this, 2 );
			return;
		}
		_mapButton.Values.ToList( _listButton );

		int iLen = _listButton.Count;
		for (int i = 0; i < iLen; i++)
			_listButton[i].isEnabled = bEnable;
	}

	// ========================== [ Division ] ========================== //

	void OnClick() { OnUIClick(); }
	void OnPress( bool bPress ) { OnUIPress( bPress ); }
	void OnHover( bool bHover ) { OnUIHover( bHover ); }
	void OnDragStart() { OnUIDrag( true ); }
	void OnDragEnd() { OnUIDrag( false ); }

	virtual protected void OnUIClick() { }
	virtual protected void OnUIPress( bool bPress ) { }
	virtual protected void OnUIHover( bool bHover ) { }
	virtual protected void OnUIDrag( bool bIsDrag ) { }

	// ========================== [ Division ] ========================== //

	protected override void OnSetSortOrder( int iSortOrder )
	{
		if (_bFixedSortOrder) return;

		_pUIPanel.depth = iSortOrder;
		_pUIPanel.sortingOrder = Mathf.FloorToInt( iSortOrder * 0.1f );

		int iChildPanelDepth = iSortOrder + 1;
		for (int i = 0; i < _listPanel.Count; i++)
		{
			if (_listPanelFixedDepth.Contains( _listPanel[i] ) == false)
				_listPanel[i].depth = iChildPanelDepth;
		}
	}

	// ========================== [ Division ] ========================== //

	protected override void OnAwake()
    {
        base.OnAwake();

		_pUIPanel = GetComponent<UIPanel>();
		if (_pUIPanel == null)
			_pUIPanel = p_pGameObjectCached.AddComponent<UIPanel>();

		GetComponentsInChildren( _listPanel );
		_listPanel.Remove( _pUIPanel );
    }
}
#endif