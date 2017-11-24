using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class CManagerUIBase<Class_Instance, ENUM_Panel_Name, Class_Panel, Class_Button> : CSingletonBase<Class_Instance>
	where Class_Instance : CManagerUIBase<Class_Instance, ENUM_Panel_Name, Class_Panel, Class_Button>
    where ENUM_Panel_Name : System.IFormattable, System.IConvertible, System.IComparable
	where Class_Panel : CUIPanelBase
{
	private CDictionary_ForEnumKey<ENUM_Panel_Name, Class_Panel> _mapPanelInstance = new CDictionary_ForEnumKey<ENUM_Panel_Name, Class_Panel>();

    private Camera _pUIcamera;  public Camera p_pUICamera {  get { return _pUIcamera; } }
    private int _iSortOrderTop; public int p_iSortOrderTop { get { return _iSortOrderTop; } }

    // ========================== [ Division ] ========================== //

    /// <summary>
    /// 지정된 기본 UI외의 Panel 모두 끕니다.
    /// </summary>
    public void DoShowDefaultPanel()
    {
        List<Class_Panel> listPanelAll = _mapPanelInstance.Values.ToList();
        for (int i = 0; i < listPanelAll.Count; i++)
			listPanelAll[i].gameObject.SetActive( listPanelAll[i].p_bAlwaysShow );

		OnDefaultPanelShow();
    }

	/// <summary>
	/// 지정된 Panel을 열거나 끕니다.
	/// </summary>
	/// <param name="ePanel">Panel 이름의 Enum</param>
	/// <param name="bShow"></param>
	public Class_Panel DoShowHide_Panel(ENUM_Panel_Name ePanel, bool bShow)
    {
		Class_Panel pPanel = _mapPanelInstance[ePanel];
		if(pPanel == null)
		{
			Debug.LogWarning( ePanel + "이 없습니다.. 하이어라키를 확인해주세요" );
			return null;
		}

        if (bShow)
        {
            int iSortOrder = 0;
			if (pPanel.p_bFixedSortOrder == false)
				iSortOrder = CaculateSortOrder_Top();

			pPanel.DoShow(iSortOrder);
		}
		else
            pPanel.DoHide();

		return pPanel;
	}

	public Class_Panel DoShowHide_Panel_Force( ENUM_Panel_Name ePanel, bool bShow )
	{
		Class_Panel pPanel = _mapPanelInstance[ePanel];
		if (pPanel == null)
		{
			Debug.LogWarning( ePanel + "이 없습니다.. 하이어라키를 확인해주세요" );
			return null;
		}
		pPanel.SetActive( bShow );

		return pPanel;
	}

	public bool DoCheckIsActive(ENUM_Panel_Name ePanel)
	{
		Class_Panel pPanel = _mapPanelInstance[ePanel];
		if (pPanel == null)
		{
			Debug.LogWarning( ePanel + "이 없습니다.. 하이어라키를 확인해주세요" );
			return false;
		}

		return pPanel.p_bIsShowCurrent;
	}

	public bool DoCheckIsPlayAnimation( ENUM_Panel_Name ePanel )
	{
		Class_Panel pPanel = _mapPanelInstance[ePanel];
		if (pPanel == null)
		{
			Debug.LogWarning( ePanel + "이 없습니다.. 하이어라키를 확인해주세요" );
			return false;
		}

		return pPanel.p_bIsPlayingUIAnimation;
	}

	public void DoChangePanel_FadeInout(ENUM_Panel_Name ePanelHide, ENUM_Panel_Name ePanelShow, float fFadeTime = 1f)
	{
		Class_Panel pPanelHide = _mapPanelInstance[ePanelHide];
		Class_Panel pPanelShow = _mapPanelInstance[ePanelShow];
		int iSortOrder = 0;
		if (pPanelShow.p_bFixedSortOrder == false)
			iSortOrder = CaculateSortOrder_Top();

		pPanelShow.EventUIPanel_SetOrder( iSortOrder );
		AutoFade.DoStartFade( fFadeTime, Color.black, pPanelHide.DoHide, pPanelShow.DoShow );
	}

	public void DoShowPanel_FadeIn(ENUM_Panel_Name ePanelShow, float fFadeTime = 1f)
	{
		Class_Panel pPanelShow = _mapPanelInstance[ePanelShow];

		int iSortOrder = 0;
		if (pPanelShow.p_bFixedSortOrder == false)
			iSortOrder = CaculateSortOrder_Top();

		pPanelShow.EventUIPanel_SetOrder( iSortOrder);
		AutoFade.DoStartFade(fFadeTime, Color.black, pPanelShow.DoShow);
	}

	public void DoHidePanel_FadeOut(ENUM_Panel_Name ePanelShow, float fFadeTime = 1f)
	{
		Class_Panel pPanelShow = _mapPanelInstance[ePanelShow];
		AutoFade.DoStartFade(fFadeTime, Color.black, pPanelShow.DoHide);
	}
	
	/// <summary>
	/// UI Panel을 얻어옵니다.
	/// </summary>
	/// <typeparam name="CUIPanel">얻고자 하는 Panel 타입</typeparam>
	/// <returns></returns>
	public CUIPanel GetUIPanel<CUIPanel>() where CUIPanel : CUIPanelBase
	{
		Class_Panel pFindPanel = null;
		ENUM_Panel_Name strKey = typeof( CUIPanel ).ToString().ConvertEnum<ENUM_Panel_Name>();
		bool bResult = _mapPanelInstance.TryGetValue( strKey, out pFindPanel );
		if (bResult == false)
			Debug.LogWarning( string.Format( "{0}을 찾을 수 없습니다.", strKey ) );

		return pFindPanel as CUIPanel;
	}

	/// <summary>
	/// UI Panel을 얻어옵니다.
	/// </summary>
	/// <typeparam name="CUIPanel">얻고자 하는 Panel 타입</typeparam>
	/// <returns></returns>
	public bool GetUIPanel<CUIPanel>(out CUIPanel pUIPanelOut) where CUIPanel : CUIPanelBase
    {
		Class_Panel pFindPanel = null;
        ENUM_Panel_Name strKey = typeof(CUIPanel).ToString().ConvertEnum<ENUM_Panel_Name>();
		bool bResult = _mapPanelInstance.TryGetValue(strKey, out pFindPanel);
		if (bResult == false)
		{
			pUIPanelOut = null;
			Debug.Log( string.Format("{0}을 찾을 수 없습니다.", strKey), this);
		}
		else
			pUIPanelOut = pFindPanel as CUIPanel;

		return bResult;
    }
	
	/// <summary>
	/// UI Panel을 얻어옵니다.
	public Class_Panel GetUIPanel(ENUM_Panel_Name eUIPanelName)
	{
		Class_Panel pFindPanel = null;
		if (_mapPanelInstance.TryGetValue(eUIPanelName, out pFindPanel) == false)
			Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eUIPanelName));

		return pFindPanel;
	}
	
	/// <summary>
	/// 타겟 카메라의 좌표로 UI 오브젝트를 이동합니다. ( 타겟 카메라의 WorldToView -> UI 카메라의 WorldToView )
	/// </summary>
	/// <param name="pCameraTarget">타겟 카메라</param>
	/// <param name="pTransTarget">이동시킬 UI 오브젝트</param>
	/// <param name="vecTargetPos">이동시킬 좌표</param>
	/// <returns></returns>
	public Vector3 DoSetUIToInGame3D(Camera pCameraTarget, Transform pTransTarget, Vector3 vecTargetPos)
    {
        Vector3 v3UIpos = _pUIcamera.ViewportToWorldPoint(pCameraTarget.WorldToViewportPoint(vecTargetPos));
        pTransTarget.position = new Vector3(v3UIpos.x, v3UIpos.y, 0);
        pTransTarget.localPosition = new Vector3(pTransTarget.localPosition.x, pTransTarget.localPosition.y, 0);

        return pTransTarget.localPosition;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        InitUIPanelInstance();		
        _pUIcamera = GetComponentInChildren<Camera>();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        DoShowDefaultPanel();
    }

	// ========================== [ Division ] ========================== //

	abstract protected void OnDefaultPanelShow();

    // ========================== [ Division ] ========================== //

    private void InitUIPanelInstance()
    {
		Class_Panel[] arrPanelInstance = GetComponentsInChildren<Class_Panel>(true);
        for (int i = 0; i < arrPanelInstance.Length; i++)
        {
			Class_Panel pPanel = arrPanelInstance[i];
			pPanel.EventOnAwake();

			string strComponentName = pPanel.GetType().Name;
			ENUM_Panel_Name ePanelName;
			if (strComponentName.ConvertEnum_IgnoreError( out ePanelName ))
				_mapPanelInstance.Add( ePanelName, pPanel );
		}
    }

    private int CaculateSortOrder_Top()
    {
        _iSortOrderTop = 0;
        List<Class_Panel> listUIPanel = _mapPanelInstance.Values.ToList();

        for (int i = 0; i < listUIPanel.Count; i++)
        {
            if (listUIPanel[i].isActiveAndEnabled)
            {
                if (listUIPanel[i].p_bFixedSortOrder)
                    ++_iSortOrderTop;
                else
                    listUIPanel[i].DoShow(++_iSortOrderTop);
            }
        }

		_iSortOrderTop += 1;
		return _iSortOrderTop;
    }

    // ========================== [ Division ] ========================== //
}
